using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace UnitTests
{
    /// <see cref="https://www.automatetheplanet.com/mstest-cheat-sheet/"/>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Global
    {
        /// <summary>
        /// Executes once BEFORE the test run.
        /// </summary>
        /// <param name="context"></param>
        [AssemblyInitialize]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "AssemblyInit() requires a TestContext parameter, even if we're not using it")]
        public static void AssemblyInit(TestContext context)
        {
            ServiceFactory.Initialise(BuildServiceCollection().BuildServiceProvider());
        }

        /// <summary>
        /// Populates the specified Service Collection with all the services requred by the Unit Tests
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions?view=dotnet-plat-ext-6.0"/>
        public static IServiceCollection BuildServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("devsettings.json")
                .AddJsonFile("secrets.json")
                .Build();
            serviceCollection.AddSingleton<IConfiguration>(config);

            serviceCollection.AddLogging(builder => {
                builder.AddSerilog(dispose: true);
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);    
            });

            return serviceCollection;
        }

        /// <summary>
        /// Returns the full path to a file in the TestData folder
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string TestData(string relativePath)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectory, @"..\..\..\TestData");

            if (!string.IsNullOrEmpty(relativePath))
                path = Path.Combine(path, relativePath);

            path = Path.GetFullPath(path);

            var dir = Path.GetDirectoryName(path)
                ?? throw new Exception("Unable to determine the location of the test directory");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return path;
        }
    }
}
