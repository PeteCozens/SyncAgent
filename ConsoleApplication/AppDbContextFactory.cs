using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

#if DEBUG

namespace ConsoleApp
{
    public class AppDbContextFactory() : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args) // = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() 
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // Add any other loggers you need
            });

            AppDbContext.ConfigureOptions(configuration, optionsBuilder);

            var logger = loggerFactory.CreateLogger<AppDbContext>();
            return new AppDbContext(configuration, logger, optionsBuilder.Options);
        }
    }
}

#endif