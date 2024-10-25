using ApplicationLogic.Services;
using Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all of the Application Logic Services with the application's IServiceCollection 
        /// </summary>
        /// <param name="services">This is the application's IServiceCollection</param>
        /// <param name="config">Should any of the Application Logic Services require configuration, you can get it from here</param>
        public static void RegisterApplicationLogicServices(this IServiceCollection services, IConfiguration config, ILogger? log = null)
        {
            var types = typeof(ServiceCollectionExtensions).Assembly.GetTypes();
            var appLogicServices = types.Where(x => typeof(IApplicationLogic).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToArray();

            foreach (var svcType in appLogicServices)
            {
                // Register the service

                log?.Here()?.LogInformation("Registered {type}", svcType.Name);
                services.AddTransient(svcType);

                // Do we have a config class for this service? If so, populate an instance from the IConfiguration and register it

                var name = $"{svcType.Name}Config";
                var configType = types.SingleOrDefault(x => x.Name == name && !x.IsAbstract && x.IsClass);
                if (configType == null)
                    continue;

                var svcConfig = config.GetSection(svcType.Name).Get(configType) 
                    ?? Activator.CreateInstance(configType)
                    ?? throw new Exception($"Unable to instantiate an instance of {configType.FullName}");

                log?.Here()?.LogInformation("Registered {type}", name);
                services.AddSingleton(configType, svcConfig);
            }
        }
    }
}
