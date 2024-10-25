using Common.Extensions;
using Infrastructure.Data;
using Infrastructure.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all of the Infrastructure Services with the application's IServiceCollection 
        /// </summary>
        /// <param name="services">This is the application's IServiceCollection</param>
        /// <param name="config">Application's IConfiguration</param>
        public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration config, ILogger? log = null)
        {
            // Database & Repositories

            services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);
            services.AddTransient<IRepository, Repository>();

            // Configurable Service Selection (use the "Services" section in the appsettings file to determine which services are used for each interface)

            RegisterConfigurableServices(services, config, log);

            // Other services

            //TODO... services.RegisterTransientServiceAndConfig<MyService>(config);
        }

        private static void RegisterTransientServiceAndConfig<TService>(this IServiceCollection services, IConfiguration config)
            where TService : class
        {
            services.AddTransient<TService>();

            var serviceName = typeof(TService).Name;
            var configName = $"{serviceName}Config";
            var configType = typeof(ServiceCollectionExtensions).Assembly.GetTypes().SingleOrDefault(x => x.IsClass && !x.IsAbstract && x.Name.Equals(configName))
                ?? throw new Exception($"No configuration class found with the name {configName}");
            var configItem = config.GetSection(serviceName).Get(configType)
                ?? throw new Exception($"No configuration section found with the name {serviceName}");

            services.AddSingleton(configType, configItem);
        }

        /// <summary>
        /// This method looks at the "Services" section in the config file, and for each interface listed registers the specified service.
        /// This allows you to specify different services, based on which environment you are operating in.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        private static void RegisterConfigurableServices(IServiceCollection services, IConfiguration config, ILogger? log)
        {
            var serviceConfig = config.GetSection<Dictionary<string, string>>("InfrastructureServices");
            if (serviceConfig == null)
                return;

            var interfaceTypes = typeof(IRepository).Assembly.GetTypes().Where(x => x.IsInterface).OrderBy(x => x.Name).ToArray();
            var serviceTypes = typeof(ServiceCollectionExtensions).Assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract).OrderBy(x => x.Name).ToArray();

            foreach (var interfaceName in serviceConfig.Keys)
            {
                // Determine the interface type from the config key

                var interfaceType = interfaceTypes.SingleOrDefault(x => x.Name.Equals(interfaceName, StringComparison.InvariantCultureIgnoreCase));
                if (interfaceType == null)
                {
                    log?.Here().LogWarning("Invalid interface type {interfaceType}", interfaceName);
                    continue;
                }

                // Determine the service type from the config value

                var serviceName = serviceConfig[interfaceName];
                var serviceType = serviceTypes.SingleOrDefault(x => x.Name.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase));
                if (serviceType == null)
                {
                    log?.Here().LogWarning("Invalid service type {serviceType}", serviceName);
                    continue;
                }

                // Ensure that the service implements the interface

                if (!interfaceType.IsAssignableFrom(serviceType))
                {
                    log?.Here().LogWarning("Invalid service. {serviceType} does not implement {interfaceType}", serviceName, interfaceName);
                    continue;
                }

                // Register the service

                services.AddTransient(interfaceType, serviceType);
                log?.Here()?.LogInformation("Registered {type}", serviceType.Name);

                // Check to see if there's a matching configuration that needs registering

                var configType = serviceTypes.SingleOrDefault(x => x.Name.Equals($"{serviceName}Config", StringComparison.InvariantCultureIgnoreCase));
                if (configType == null)
                    continue;

                // Read the config

                var configItem = config.GetSection(serviceName).Get(configType);
                if (configItem == null)
                    continue;

                // Register the config

                services.AddSingleton(configType, configItem);
            }
        }
    }
}
