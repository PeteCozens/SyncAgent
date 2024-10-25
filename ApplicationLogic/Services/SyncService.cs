using Common.Extensions;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services
{
    public class SyncService(ILogger<SyncService> log, IConfiguration config) : IApplicationLogic
    {
        public async Task DoWorkAsync(string[] jobs)
        {
            var jobConfigDictionary = config.GetRequiredSection<Dictionary<string, JobConfig>>("jobs");

            // Enumerate through the configured jobs and execute any that are included in the specified list of jobs

            foreach (var job in jobs)
            {
                // Check that the name specified matches a configured job

                if (!jobConfigDictionary.TryGetValue(job, out var jobConfig))
                {
                    log.LogWarning("There is no configuration for a job called \"{jobName}\" in the SyncAgent's configuration", job);
                    continue;
                }

                // Check that the job is enabled

                if (!jobConfig.Enabled)
                {
                    log.LogInformation("The job \"{jobName}\" is currently disabled in the SyncAgent's configuration", job);
                    continue;
                }

                // Fetch existing progress for his Job



                // Connect to the specified database

                // Execute the SQL Query

                // Order the results

                // Process the 
            }
        }
    }
}
