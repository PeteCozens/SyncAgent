using ApplicationLogic.Extensions;
using ApplicationLogic.Services;
using Common;
using Common.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;

namespace ConsoleApp
{
    internal class Program
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static ILogger<Program> _log;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Application Startup");

            // Update the console window's title
            var name = Assembly.GetExecutingAssembly()?.GetName()?.Name;
            if (!string.IsNullOrEmpty(name))
                Console.Title = name;

            // Build the application and register all services
            var runningFromPackageManager = args.Contains("--applicationName");

            var host = Host
                .CreateDefaultBuilder()

                // Configure Logging. See https://kenlea.blog/net-7-console-app-with-serilog-step-by-step-instructions
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .Enrich.FromLogContext()
                        .ReadFrom.Configuration(context.Configuration);
                })

                // Services for Dependency Injection
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(context.Configuration);
                    if (!runningFromPackageManager)
                        services.RegisterApplicationLogicServices(context.Configuration);
                    services.RegisterInfrastructureServices(context.Configuration);
                })
                .Build();

            ServiceFactory.Initialise(host.Services);

            // Get a logger
            _log = ServiceFactory.GetRequiredService<ILogger<Program>>();

            // If we're running from the package manager, bail out at this point
            if (runningFromPackageManager)
                return -1;

            // Ensure that the database is up-to-date
            ServiceFactory.GetRequiredService<AppDbContext>().Migrate();

            try
            {
                // TODO... Single instance at a time - don't allow overlap

                var worker = ServiceFactory.CreateInstance<SyncService>();
                await worker.DoWorkAsync(args);

                return 0; // No Errors
            }
            catch (Exception e)
            {
                _log.Here().LogError(e, "Unhandled Exception");

                return 1; // Error(s) Occurred
            }
        }
    }
}