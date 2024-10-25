using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Data
{
    /// <summary>
    /// Customise the MigrationsHistory table to include adtional columns
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.",
        Justification = "Base class may be removed in the future, but if so, we anticipate the equivalent functionality to be provided in a new form beforehand")]
    public class MigrationsHistoryRepository(HistoryRepositoryDependencies dependencies) : SqlServerHistoryRepository(dependencies)
    {
        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.Property<DateTime>("AppliedAtUtc")
                .HasDefaultValueSql("GetUtcDate()");

            history.Property<string>("AppliedBySqlUser")
                .HasMaxLength(100)
                .HasDefaultValueSql("ORIGINAL_LOGIN()");

            history.Property<string>("AppliedByHost")
                .HasMaxLength(100)
                .HasDefaultValueSql("HOST_NAME()");
        }
    }
}
