using Azure.Identity;
using Common.Extensions;
using Common.Models;
using Infrastructure.Services.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApplicationLogic.Services
{
    public class SyncService(ILogger<SyncService> log, IConfiguration config, IRepository repo) : IApplicationLogic
    {
        public async Task DoWorkAsync(string[] jobsToRun)
        {
            var syncJobs = config.GetRequiredSection<Dictionary<string, string[]>>("syncJobs");
            var syncSets = config.GetRequiredSection<Dictionary<string, SyncSet>>("syncSets");

            // Enumerate through the configured jobs and execute any that are included in the specified list of jobs

            foreach (var jobToRun in jobsToRun)
            {
                // Check that the name specified matches a configured job

                if (!syncJobs.TryGetValue(jobToRun, out var syncSetNames))
                {
                    log.Here().LogWarning("There is no configuration for a Sync Job called \"{syncJob}\" in the SyncAgent's configuration", jobToRun);
                    continue;
                }

                // Enumerate through the SyncSets in this job

                foreach (var syncSetName in syncSetNames)
                {
                    // Get the Sync Set configuration

                    if (!syncSets.TryGetValue(syncSetName, out var syncSet))
                    {
                        log.Here().LogWarning("There is no configuration for a Sync Set called \"{syncSet}\" in the SyncAgent's configuration", syncSetName);
                        continue;
                    }

                    // Check that the SyncSet is enabled

                    if (!syncSet.Enabled)
                    {
                        log.Here().LogInformation("Synchronisation for {syncSet} is currently disabled in the SyncAgent's configuration", syncSetName);
                        continue;
                    }

                    // Fetch existing progress for this SyncSet

                    var progressToDate = (await repo.GetProgressAsync(x => x.SyncSet == syncSetName, true))
                        .ToDictionary(x => x.Field, StringComparer.CurrentCultureIgnoreCase);

                    // Connect to the specified database

                    var cs = config.GetConnectionString(syncSet.Connection);
                    if (string.IsNullOrEmpty(cs))
                    {
                        log.Here().LogWarning("There is no Connection String called \"{connectionString}\" for {syncSet}", cs, syncSetName);
                        continue;
                    }

                    var con = new SqlConnection(cs);
                    try
                    {
                        await con.OpenAsync();
                    }
                    catch (Exception e)
                    {
                        log.Here().LogError(e, "Failed to open the database for {syncSet}", syncSetName);
                        continue;
                    }

                    // Build the SQL Query

                    var missingProgress = false;
                    var cmd = new SqlCommand(syncSet.Sql, con);
                    foreach (var fieldName in syncSet.Progress)
                    {
                        if (!progressToDate.TryGetValue(fieldName, out var progress))
                            missingProgress = true;

                        if (progress?.Value == null)
                        {
                            cmd.Parameters.AddWithValue(fieldName, DBNull.Value);
                            continue;
                        }

                        var dataType = Type.GetType(progress.Type) ?? typeof(string);
                        var value = Convert.ChangeType(progress.Value, dataType);

                        //object value = progress.Type switch
                        //{
                        //    "System.DateTime" => Convert.ToDateTime(progress.Value),
                        //    "System.Int32" => Convert.ToInt32(progress.Value),
                        //    "System.Decimal" => Convert.ToDecimal(progress.Value),
                        //    _ => progress.Value,
                        //};

                        cmd.Parameters.AddWithValue(fieldName, value);
                    }

                    // Execute the SQL Query

                    SqlDataReader dr;
                    try
                    {
                        dr = cmd.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        log.Here().LogError(e, "An error occurred executing the SQL query for {syncSet}", syncSetName);
                        throw;
                    }

                    if (!dr.HasRows)
                    {
                        log.Here().LogInformation("No updates found for {syncSet}", syncSetName);
                        continue;
                    }

                    // Ensure that we have all the records we need to record progress

                    if (missingProgress)
                    {
                        var columnTypes = Enumerable.Range(0, dr.FieldCount)
                            .ToDictionary(i => dr.GetName(i), i => dr.GetFieldType(i).FullName);

                        foreach (var progressField in syncSet.Progress)
                        {
                            if (progressToDate.ContainsKey(progressField))
                                continue;

                            var columnType = columnTypes[progressField];
                            if (columnType == null)
                            {
                                log.Here().LogError("Unable to store the progress of field {field} in Sync Set {syncSet} as it does not exist in the data retrieved", progressField, syncSetName);
                                continue;
                            }

                            var progress = new Progress { SyncSet = syncSetName, Field = progressField, Type = columnType, Value = null };
                            progress = await repo.SaveAsync(progress, false);
                            progressToDate.Add(progressField, progress);
                        }

                        await repo.FlushAsync();
                    }

                    // Create sql connections for any additional data queryies

                    var connections = syncSet.AdditionalData
                        .Select(x => x.Connection)
                        .Distinct(StringComparer.CurrentCultureIgnoreCase)
                        .ToDictionary(x => x, x =>
                        {
                            var cs = config.GetConnectionString(x);
                            var con = new SqlConnection(cs);
                            con.Open();
                            return con;
                        }, StringComparer.CurrentCultureIgnoreCase);

                    // Read the data and upload to the API

                    SqlConnection? collectionConnection = null;
                    while (dr.Read())
                    {
                        // Extract the data for this row

                        var row = GetRow(dr);

                        // Get any additional properties for this record

                        if (syncSet.AdditionalData != null)
                        {
                            foreach (var data in syncSet.AdditionalData)
                            {
                                var connection = connections[data.Connection];
                                using SqlDataReader reader = ExecuteReader(connection, data.Sql, row);
                                if (!reader.HasRows)
                                    continue;

                                await reader.ReadAsync();

                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    var fieldName = reader.GetName(i);
                                    if (row.ContainsKey(fieldName))
                                        continue;
                                    row.Add(fieldName, reader.GetValue(i));
                                }
                            }
                        }

                        // Add any Collections

                        if (syncSet.Collections != null)
                        {
                            foreach (var propertyName in syncSet.Collections.Keys)
                            {
                                if (collectionConnection == null)
                                {
                                    var ccs = config.GetConnectionString(syncSet.Connection);
                                    collectionConnection = new SqlConnection(ccs);
                                    await collectionConnection.OpenAsync();
                                }

                                if (!syncSet.Collections.TryGetValue(propertyName, out var sql) || string.IsNullOrEmpty(sql))
                                    continue;

                                using var reader = ExecuteReader(collectionConnection, sql, row);
                                if (!reader.HasRows)
                                    continue;

                                var items = new List<Dictionary<string, object>>();
                                while (await reader.ReadAsync())
                                    items.Add(GetRow(reader));
                                row.Add(propertyName, items);
                            }
                        }

                        // Get any additional files to include in the upload

                        var includedFiles = new List<IncludedFile>();
                        if (syncSet.Files != null)
                        {
                            foreach (var includedFile in syncSet.Files)
                            {
                                var path = includedFile.Path.ReplacePlaceholders(row);
                                if (!File.Exists(path))
                                    continue; // File not found

                                var name = string.IsNullOrEmpty(includedFile.Name)
                                    ? Path.GetFileName(path)
                                    : includedFile.Name.ReplacePlaceholders(row);

                                includedFiles.Add(new IncludedFile { Path = path, Name = name });
                            }
                        }

                        // Upload the object to the API

                        var json = row.ToJson(true);
                        Console.WriteLine($"{syncSet.Verb} to {syncSet.Url}: {json}");

                        // TODO... Upload the row to the API
                        // TODO... Include any IncludedFiles in the call

                        // Update our progress in the local DB

                        foreach (var progressField in syncSet.Progress)
                        {
                            var p = progressToDate[progressField];
                            p.Value = p.Type switch
                            {
                                "System.DateTime" => string.Format("{0:yyyy-MM-dd HH:mm:ss.ffffff}", row[progressField]),
                                _ => row[progressField].ToString()
                            };
                            await repo.SaveAsync(p, false);
                        }
                        await repo.FlushAsync();
                    }

                    // Tidy up

                    collectionConnection?.Dispose();

                    foreach (var sqlConnection in connections.Values)
                        await sqlConnection.DisposeAsync();
                }
            }

            // Done.
        }

        private static Dictionary<string, object> GetRow(SqlDataReader dr)
        {
            return Enumerable.Range(0, dr.FieldCount).ToDictionary(dr.GetName, dr.GetValue);
        }

        private static SqlDataReader ExecuteReader(SqlConnection con, string sql, Dictionary<string, object> parameters)
        {
            var cmd = new SqlCommand(sql, con);
            var parameterNames = sql.ExtractSqlParameters();
            foreach (var parameterName in parameterNames)
                cmd.Parameters.AddWithValue(parameterName, parameters[parameterName.Substring(1)]);
            var reader = cmd.ExecuteReader();
            return reader;
        }


    }
}
