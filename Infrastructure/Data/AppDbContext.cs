using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext(IConfiguration config, ILogger<AppDbContext> log, DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        private const string ConnectionStringName = "SyncAgent";

        private const string DefaultDatabaseSchema = "dbo";

        //internal virtual DbSet<EnumValue> Enums { get; set; }
        public virtual DbSet<Progress> Progress { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// This method should only be called at application startup to ensure that the database exists,
        /// is up-to-date and contains the required seed data
        /// </summary>
        public void Migrate()
        {
            // Ensure that the DB is created and up-to-date

            Database.Migrate();

            // Map any Dapper models

            DapperUtils.MapColumnAttributesForDapper();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigureOptions(config, optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        public static void ConfigureOptions(IConfiguration config, DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder
                .UseSqlServer(config.GetConnectionString(ConnectionStringName),
                    options => options
                    .MigrationsHistoryTable("__MigrationsHistory", DefaultDatabaseSchema)
                    .MigrationsAssembly("Infrastructure")
                ).ReplaceService<IHistoryRepository, MigrationsHistoryRepository>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultDatabaseSchema);

            modelBuilder.Entity<Progress>().HasKey(x => new { x.SyncSet, x.Field });

            base.OnModelCreating(modelBuilder);
        }
    }
}
