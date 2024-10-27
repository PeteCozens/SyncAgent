using Common.Extensions;
using Common.Models;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services
{
    public class SyncService(ILogger<SyncService> log, IConfiguration config, AppDbContext db) : IApplicationLogic
    {
        private Dictionary<string, SqlConnection> Connections = new (StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Checks the database to see if another sync is running before syncing data according to the jobs specified
        /// </summary>
        /// <param name="jobsToRun"></param>
        /// <returns></returns>
        public async Task DoWorkAsync(string[] jobsToRun)
        {
            log.Here().LogInformation("DoWorkAsync");

            // Check to see if the sync is already running by checking for a lock on the database

            const string key = "lock";

            var appLock = db.Settings.SingleOrDefault(x => x.Key == key);
            if (appLock == null)
            {
                appLock = new Setting { Key = key };
                db.Settings.Add(appLock);
            }
            else
            {
                if(!string.IsNullOrEmpty(appLock.Value))
                {
                    // Application is already locked
                    log.Here().LogWarning("Data Sync is unable to run as another instance is currently running with the following arguments: {args}", jobsToRun);
                    return;
                }
            }

            // Place a lock in the database

            log.LogDebug("Applying sync lock");
            appLock.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await db.SaveChangesAsync();

            // Perform the sync

            try
            {
                await ExecuteSyncJobsAsync(jobsToRun);
            }
            catch (Exception e)
            {
                log.Here().LogError(e, "Unhandled exception thrown whilst running Data Sync");
                throw;
            }
            finally
            {
                // Whatever happens, remove the lock
                log.Here().LogDebug("Releasing sync lock");
                appLock.Value = string.Empty;
                await db.SaveChangesAsync();
            }
        }

        private async Task<SqlConnection> GetConnection(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "Default";

            if (Connections.TryGetValue(name, out var con))
                return con;

            var cs = config.GetConnectionString(name);
            con = new SqlConnection(cs);
            try
            {
                await con.OpenAsync();
                Connections.Add(name, con); 
                return con;
            }
            catch (Exception e)
            {
                log.Here().LogError(e, "Unable to connect to the SQL database {database}", name);
                throw;
            }
        }

        /// <summary>
        /// Data Synchronisation processing
        /// </summary>
        /// <param name="jobsToRun">list of sync jobs to be run</param>
        /// <returns></returns>
        private async Task ExecuteSyncJobsAsync(string[] jobsToRun)
        {
            var syncJobs = config.GetRequiredSection<Dictionary<string, string[]>>("syncJobs");
            var syncSets = config.GetRequiredSection<Dictionary<string, SyncSet>>("syncSets");

            Connections.Clear();

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

                    var progressToDate = db.Progress.Where(x => x.SyncSet == syncSetName).ToDictionary(x => x.Field, StringComparer.CurrentCultureIgnoreCase);

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

                    var cmd = new SqlCommand(syncSet.Sql, con);
                    var missingProgress = AddProgressParameters(syncSet, progressToDate, cmd);

                    // Execute the SQL Query

                    SqlDataReader dr;
                    try
                    {
                        dr = cmd.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        log.Here().LogError(e, "An error occurred executing the SQL query for {syncSet}", syncSetName);
                        continue;
                    }

                    if (!dr.HasRows)
                    {
                        log.Here().LogInformation("No updates found for {syncSet}", syncSetName);
                        continue;
                    }

                    // Ensure that we have all the records we need to record progress

                    if (missingProgress)
                        await AddMissingProgressRecords(syncSetName, syncSet, dr, progressToDate);

                    // Read the data and upload to the API

                    while (dr.Read())
                    {
                        // Extract the data for this row

                        var row = GetRow(dr);
                        await PopulateAdditionalData(syncSet,  row);
                        await PopulateCollections(syncSet, row);
                        var includedFiles = GetIncludedFiles(syncSet, row);

                        // Upload the object to the API

                        log.LogDebug("{verb} to {url}: {payload}", syncSet.Verb, syncSet.Url, row);

                        // TODO... Upload the row to the API
                        // TODO... Include any IncludedFiles in the call

                        // Update our progress in the local DB

                        await UpdateProgress(syncSet, progressToDate, row);
                    }
                }
            }

            // Tidy up

            foreach (var con in Connections.Values)
            {
                await con.CloseAsync();
                await con.DisposeAsync();
            }
            Connections.Clear();
        }

        /// <summary>
        /// Adds parameters to the SqlCommand specified to indicate sync progress to date
        /// </summary>
        /// <param name="syncSet"></param>
        /// <param name="progressToDate"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool AddProgressParameters(SyncSet syncSet, Dictionary<string, Progress> progressToDate, SqlCommand cmd)
        {
            var missingProgress = false;
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

                cmd.Parameters.AddWithValue(fieldName, value);
            }

            return missingProgress;
        }

        /// <summary>
        /// Update the current progress in the database, so that next time the sync runs we're not trying to sync the same records
        /// </summary>
        /// <param name="syncSet"></param>
        /// <param name="progressToDate"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task UpdateProgress(SyncSet syncSet, Dictionary<string, Progress> progressToDate, Dictionary<string, object> row)
        {
            foreach (var progressField in syncSet.Progress)
            {
                var p = progressToDate[progressField];
                p.Value = p.Type switch
                {
                    "System.DateTime" => string.Format("{0:yyyy-MM-dd HH:mm:ss.ffffff}", row[progressField]),
                    _ => row[progressField].ToString()
                };
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns a list of the files that need to be included with this record when it is uploaded
        /// </summary>
        /// <param name="syncSet"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static List<IncludedFile> GetIncludedFiles(SyncSet syncSet, Dictionary<string, object> row)
        {
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

            return includedFiles;
        }

        /// <summary>
        /// Queries the relevant database(s) for any collection properties to be applied to the row
        /// </summary>
        /// <param name="syncSet"></param>
        /// <param name="collectionConnection"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task PopulateCollections(SyncSet syncSet, Dictionary<string, object> row)
        {
            if ((syncSet.Collections == null) || (syncSet.Collections.Count == 0))  
                return;


            foreach (var key in syncSet.Collections.Keys)
            {
                var subQuery = syncSet.Collections[key];

                var connectionName = string.IsNullOrEmpty(subQuery.Connection)
                    ? syncSet.Connection
                    : subQuery.Connection;

                var connection = await GetConnection(connectionName);

                using var reader = ExecuteReader(connection, subQuery.Sql, row);
                if (!reader.HasRows)
                    continue;

                var items = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                    items.Add(GetRow(reader));
                row.Add(key, items);
            }
        }

        /// <summary>
        /// Queries the relevant database(s) for any additional properties to be applied to the row from alternate data sources
        /// </summary>
        /// <param name="syncSet"></param>
        /// <param name="connections"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task PopulateAdditionalData(SyncSet syncSet, Dictionary<string, object> row)
        {
            if (syncSet.AdditionalData == null)
                return;

            foreach (var data in syncSet.AdditionalData)
            {
                var connectionName = string.IsNullOrEmpty(data.Connection) 
                    ? syncSet.Connection
                    : data.Connection;

                var connection = await GetConnection(connectionName);

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

        /// <summary>
        /// If we're missing any progress records (should only be the first time the sync is run), then create them, ready to be 
        /// saved to the database once the first data record is sync'd
        /// </summary>
        /// <param name="syncSetName"></param>
        /// <param name="syncSet"></param>
        /// <param name="dr"></param>
        /// <param name="progressToDate"></param>
        /// <returns></returns>
        private async Task AddMissingProgressRecords(string syncSetName, SyncSet syncSet, SqlDataReader dr, Dictionary<string, Progress> progressToDate)
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
                db.Progress.Add(progress);
                progressToDate.Add(progressField, progress);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Reads the current row from a SqlDataReader and returns it as a dictionary
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetRow(SqlDataReader dr)
        {
            return Enumerable.Range(0, dr.FieldCount).ToDictionary(dr.GetName, dr.GetValue);
        }

        /// <summary>
        /// Executes the specified sql query, having first extracted any paramters from the SQL script and populated them
        /// from the paramters dictionary supplied
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
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
