using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public static class ServiceFactory
    {
        private static IServiceProvider? ServiceProvider { get; set; } = null;

        public static void Initialise(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            ServiceProvider = scope.ServiceProvider;
        }

        /// <summary>
        /// Create an instance of a class that, whilst not registered in the service provider itself, requires 
        /// services from the service provider when instantiating
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T CreateInstance<T>() where T : class
        {
            if (ServiceProvider == null)
                throw new Exception("The ServiceProvider has not been set");
            return ActivatorUtilities.CreateInstance<T>(ServiceProvider);
        }

        /// <summary>
        /// Retrieve an optional service from the service provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T? GetService<T>() where T : class
        {
            if (ServiceProvider == null)
                throw new Exception("The ServiceProvider has not been set");
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Retrieve a required service from the service provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T GetRequiredService<T>() where T : class
        {
            if (ServiceProvider == null)
                throw new Exception("The ServiceProvider has not been set");
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
