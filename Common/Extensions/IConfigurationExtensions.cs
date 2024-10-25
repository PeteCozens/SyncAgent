using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Common.Extensions
{
    /// <summary>
    /// Methods to help extract data from IConfiguration objects
    /// </summary>
    /// <remarks>
    /// Requires the NuGet package Microsoft.Extensions.Configuration.Binder
    /// </remarks>
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Casts the IConfiguration object to the type specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetRoot<T>(this IConfiguration configuration) where T : class 
        {
            ArgumentNullException.ThrowIfNull(configuration);

#pragma warning disable CS8603 // Possible null reference return.
            return configuration.Get<T>() ?? default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Casts the specified section of the IConfiguration to the type specified. 
        /// Returns a default instance of the type if the section is not found or cannot be mapped
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T? GetSection<T>(this IConfiguration configuration, string key) where T : class 
        {
            ArgumentNullException.ThrowIfNull(configuration);
            return configuration.GetSection(key)?.Get<T>();
        }

        /// <summary>
        /// Returns a specific value from the configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T? GetValue<T>(this IConfiguration configuration, string path)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            path = path.Replace("\\", ":");
            return (T?)configuration.GetValue(typeof(T), path);
        }

        /// <summary>
        /// Casts the specified section of the IConfiguration to the type specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetRequiredSection<T>(this IConfiguration configuration, string key) where T : class
        {
            ArgumentNullException.ThrowIfNull(configuration);
#pragma warning disable CS8603 // Possible null reference return.
            return configuration.GetRequiredSection(key).Get<T>();
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
