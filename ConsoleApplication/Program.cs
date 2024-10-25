using ApplicationLogic.Extensions;
using ApplicationLogic.Services;
using CommandLine;
using Common;
using Common.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            var host = BuildHost(args);

            ServiceFactory.Initialise(host.Services);

            // Get a logger
            _log = ServiceFactory.GetRequiredService<ILogger<Program>>();

            // If we're running from the package manager, bail out at this point
            if (args.Contains("--applicationName"))
                return -1;

            // Ensure that the database is up-to-date
            _log.Here().LogDebug("Migrating the database");

            ServiceFactory.GetRequiredService<AppDbContext>().Migrate();

            try
            {
                // Parse the command line
                var result = Parser.Default.ParseArguments<CommandLineOptions>(args);   // Parse the command line and run the application. See https://github.com/commandlineparser/commandline

                // Handle parsing errors

                result = await result.WithNotParsedAsync(async errors =>                    // Command line parsing failed
                {
                    // Command line parsing failed

                    _log.Here().LogInformation("Command Line: {commandLine}", string.Join(" ", args));
                    HandleCommandLineError(errors);
                    await Task.Delay(50);
                });

                // Perform application processing

                result = await result.WithParsedAsync(async options =>
                {
                    // Command line parsed succesfully

                    _log.Here().LogDebug("Command Line Options: {@options}", options);

                    // Do some work

                    _log.Here().LogDebug("Starting Worker");

                    await DoWorkAsync(options);

                    _log.Here().LogDebug("Worker Complete");
                });

                return !result.Errors.Any() ? 1 : 0;
            }
            catch (Exception e)
            {
                _log.Here().LogError(e, "Unhandled Exception");
                return 0;
            }
        }

        private static IHost BuildHost(string[] args)
        {
            var runningFromPackageManager = args.Contains("--applicationName");

            return Host
                .CreateDefaultBuilder()

                // JSON Configuration. See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-7.0#host-configuration
                .ConfigureHostConfiguration(hostConfig =>
                {
                    hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                    hostConfig.AddJsonFile("appsettings.json", optional: false);
                    hostConfig.AddJsonFile("secrets.json", optional: false);

                    var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                    if (string.IsNullOrEmpty(environmentName))
                        environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    if (!string.IsNullOrEmpty(environmentName))
                    {
                        Console.WriteLine($"Adding {environmentName} configuration");
                        hostConfig.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                        hostConfig.AddJsonFile($"secrets.{environmentName}.json", optional: true);
                    }
                    else if (System.Diagnostics.Debugger.IsAttached || runningFromPackageManager)
                    {
                        Console.WriteLine("Adding development configuration");
                        hostConfig.AddJsonFile("appsettings.development.json", optional: true);
                        hostConfig.AddJsonFile("secrets.development.json", optional: true);
                    }
                })

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
        }

        private static void HandleCommandLineError(IEnumerable<Error> errors)
        {
            _log.Here().LogError("Command Line Parsing failed with the following errors:");
            foreach (var error in errors)
            {
                if (error is SetValueExceptionError)
                {
                    if (error is not SetValueExceptionError e)
                        continue;

                    var ex = e.Exception;
                    var msg = ex.Message;
                    while (ex.InnerException != null)
                    {
                        if (msg.Right(1) != ".")
                            msg += ".";
                        msg += $" {ex.InnerException.Message}";
                        ex = ex.InnerException;
                    }
                    _log.Here().LogWarning("* {param}: {error}", e.NameInfo.LongName, msg);
                }
                else
                {
                    _log.Here().LogWarning("* {error}", error.ToString());
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "For demonstration purposes only. This can be removed if not reqd.")]
        private async static Task DoWorkAsync(CommandLineOptions options)
        {
            var worker = ServiceFactory.CreateInstance<DummyService>();
            await worker.DoWorkAsync();
        }
    }
}