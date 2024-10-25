using Common.Attributes;
using Common.Extensions;
using Common.Models;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Common.Interfaces;
using static Dapper.SqlMapper;

namespace Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext(IConfiguration config, ILogger<AppDbContext> log, DbContextOptions options) : DbContext(options)
    {
        private const string ConnectionStringName = "SyncAgent";

        public const string DefaultDatabaseSchema = "dbo";

        //internal virtual DbSet<EnumValue> Enums { get; set; }
        public virtual DbSet<Progress> Progress { get; set; }

        /// <summary>
        /// This method should only be called at application startup to ensure that the database exists,
        /// is up-to-date and contains the required seed data
        /// </summary>
        public void Migrate()
        {
            // Ensure that the DB is created and up-to-date

            Database.Migrate();

            // Seed the database

            //SeedEnums();
            //SeedCurrencies();
            //SeedFxRates();

            // Map any Dapper models

            DapperUtils.MapColumnAttributesForDapper();
        }

        //private void SeedEnums()
        //{
        //    this.Merge(GetEnumValuesFromDataModels(), x => x.Enums);
        //    SaveChanges();
        //}

        //private void SeedCurrencies()
        //{
        //    if (Currencies.Any())
        //        return;
        //    Currencies.AddRange(CurrencyDataSource.GetData());
        //    SaveChanges();
        //}

        //private void SeedFxRates()
        //{
        //    if (FxRates.Any())
        //        return;
        //    FxRates.AddRange(FxDataSource.GetData(this));
        //    SaveChanges();
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(config.GetConnectionString(ConnectionStringName),
                        options => options
                        .MigrationsHistoryTable("__MigrationsHistory", DefaultDatabaseSchema)
                        .MigrationsAssembly("Infrastructure")
                    ).ReplaceService<IHistoryRepository, MigrationsHistoryRepository>();
            }

            base.OnConfiguring(optionsBuilder);
        }

        //public override int SaveChanges()
        //{
        //    UpdateTemporalEntries();
        //    PerformRegExValidation();
        //    return base.SaveChanges();
        //}

        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        //{
        //    UpdateTemporalEntries();
        //    PerformRegExValidation();
        //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        /// <summary>
        /// Perform RegEx validation on any properties that have the [RegularExpression] applied
        /// </summary>
        /// <exception cref="Exception"></exception>
        //private void PerformRegExValidation()
        //{
        //    var entities = ChangeTracker.Entries()
        //       .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
        //       .Select(e => e.Entity);

        //    foreach (var entity in entities)
        //    {
        //        var entityType = entity.GetType();
        //        var properties = entityType.GetProperties();

        //        foreach (var property in properties)
        //        {
        //            var regexAttributes = property.GetCustomAttributes<RegularExpressionAttribute>();

        //            foreach (var regexAttribute in regexAttributes)
        //            {
        //                var propertyValue = property.GetValue(entity)?.ToString();

        //                if (string.IsNullOrEmpty(propertyValue) || regexAttribute.IsValid(propertyValue))
        //                    continue;

        //                throw new Exception($"Invalid {property.Name}: {propertyValue}");
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Enforce Soft Deletes and set property values from ITemporalTables
        /// </summary>
        //private void UpdateTemporalEntries()
        //{
        //    var userName = @$"{Environment.UserDomainName}\{Environment.UserName}";
        //    var machineName = @$"{Environment.UserDomainName}\{Environment.MachineName}";

        //    var tt = typeof(ITemporalTable);
        //    foreach (var e in ChangeTracker.Entries().Where(x => x.Metadata.ClrType.IsAssignableTo(tt)))
        //    {
        //        // Enforce Soft Deletes
        //        if (e.State == EntityState.Deleted)
        //        {
        //            e.State = EntityState.Modified;
        //            e.Property("SysIsDeleted").CurrentValue = true;
        //        }

        //        // Ensure that the application user's identity and machine name are stamped on the record
        //        if (e.State != EntityState.Unchanged)
        //        {
        //            e.Property("SysUpdatedByUser").CurrentValue = userName;
        //            e.Property("SysUpdateHostMachine").CurrentValue = machineName;
        //        }
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            log.Here().LogDebug("Creating the Entity Framework Model");

            modelBuilder.HasDefaultSchema(DefaultDatabaseSchema);

            //ConfigureTemporalTables(modelBuilder);
            //ConfigureDecimalDataTypes(modelBuilder);
            //ConfigureDateOnlyDataTypes(modelBuilder);
            //ConfigureCompoundKeys(modelBuilder);
            //ConfigureForeignKeys(modelBuilder);
            ConfigureRowVersions(modelBuilder);

            // TODO... Any additional config that can't be done in the model classes

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Applies the Timestamp property to the SysRowVersion field on every model class that's NOT a temporal table
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void ConfigureRowVersions(ModelBuilder modelBuilder)
        {
            var tt = typeof(ITemporalTable);
            var types = modelBuilder
                .Model
                .GetEntityTypes()
                .Select(x => x.ClrType)
                .Where(x => !tt.IsAssignableFrom(x));

            foreach (var t in types)
            {
                modelBuilder.Entity(t)
                    .Property("SysRowVersion")?
                    .IsRequired(true)?
                    .IsRowVersion();
            }
        }

        /// <summary>
        /// Configure all tables/classes that inherit from ITemporalTable
        /// </summary>
        //private static void ConfigureTemporalTables(ModelBuilder modelBuilder)
        //{
        //    var tt = typeof(ITemporalTable);
        //    var temporalTypes = modelBuilder
        //        .Model
        //        .GetEntityTypes()
        //        .Select(x => x.ClrType)
        //        .Where(tt.IsAssignableFrom);

        //    foreach (var t in temporalTypes)
        //    {
        //        var entity = modelBuilder.Entity(t);

        //        // Flag this table as a temporal table and configure the properties for the date ranges they're valid for

        //        entity.ToTable(x => x.IsTemporal(t =>
        //        {
        //            t.HasPeriodStart("SysPeriodStart");
        //            t.HasPeriodEnd("SysPeriodEnd");
        //        }));

        //        // Configure the Soft Delete
        //        // This adds (in a reflection type way), a Global Query Filter that always excludes deleted items.
        //        // You can opt out by using dbSet.IgnoreQueryFilters()
        //        // https://docs.microsoft.com/en-us/ef/core/querying/filters
        //        // https://dotnetcoretutorials.com/2022/03/17/using-ef-core-global-query-filters-to-ignore-soft-deleted-entities/

        //        var parameter = Expression.Parameter(t, "p");
        //        var deletedCheck = Expression.Lambda(
        //                Expression.Equal(Expression.Property(parameter, "SysIsDeleted"), Expression.Constant(false))
        //                , parameter);
        //        entity.HasQueryFilter(deletedCheck);

        //        // Set the properties defined in this interface

        //        entity.Property("SysUpdatedByUser").IsRequired(true).HasMaxLength(128).HasDefaultValue("");
        //        entity.Property("SysUpdateHostMachine").IsRequired(true).HasMaxLength(128).HasDefaultValue("");
        //        entity.Property("SysIsDeleted").IsRequired(true).HasDefaultValue(false);
        //        entity.Property("SysRowVersion").IsRequired(true).IsRowVersion();   // Applies the Timestamp property
        //    }
        //}

        /// <summary>
        /// Ensure that any decimal properties in the models have a SQL type of decimal(20,2) unless otherwise specified in a [Column] attribute
        /// </summary>
        /// <param name="modelBuilder"></param>
        //private static void ConfigureDecimalDataTypes(ModelBuilder modelBuilder)
        //{
        //    // Decimal

        //    var t1 = typeof(decimal);
        //    var t2 = typeof(decimal?);

        //    var properties = modelBuilder.Model
        //        .GetEntityTypes()
        //        .SelectMany(x => x.GetProperties())
        //        .Where(x => x.ClrType == t1 || x.ClrType == t2);

        //    foreach (var property in properties)
        //    {
        //        if (string.IsNullOrEmpty(property.GetColumnType()) &&
        //            property?.PropertyInfo?.GetCustomAttribute<ColumnAttribute>() is null)
        //        {
        //            property?.SetColumnType("decimal(20,2)");
        //        }
        //    }
        //}

        /// <summary>
        /// Ensure that any DateTime properties with [DateOnly] attributes in the models have a SQL type of DATE
        /// </summary>
        /// <param name="modelBuilder"></param>
        //private static void ConfigureDateOnlyDataTypes(ModelBuilder modelBuilder)
        //{
        //    var t = typeof(DateTime);
        //    var properties = modelBuilder.Model
        //        .GetEntityTypes()
        //        .SelectMany(x => x.GetProperties())
        //        .Where(x => x.ClrType == t);

        //    foreach (var property in properties)
        //    {
        //        var dateOnlyAttribute = property.PropertyInfo?.GetCustomAttribute<DateOnlyAttribute>();
        //        if (dateOnlyAttribute != null)
        //        {
        //            property.SetColumnType("date");
        //        }
        //    }
        //}

        /// <summary>
        /// Configure compound primary keys on any models that require them
        /// </summary>
        /// <param name="modelBuilder"></param>
        //private static void ConfigureCompoundKeys(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<EnumValue>().HasKey(x => new { x.SchemaName, x.TableName, x.ColumnName, x.Value });
        //    modelBuilder.Entity<FxRate>().HasKey(x => new { x.CurrencyId, x.FromDate });
        //}

        /// <summary>
        /// Configure Foreign Keys on any models that require them
        /// </summary>
        /// <param name="modelBuilder"></param>
        //private static void ConfigureForeignKeys(ModelBuilder modelBuilder)
        //{
        //    ConfigureForeignKey<FxRate, int, Currency>(modelBuilder, x => x.CurrencyId, x => x.Currency);

        //    //modelBuilder.Entity<FxRate>()
        //    //  .HasOne(fx => fx.Currency) // FxRate has one Currency
        //    //  .WithMany()                // No navigation property in Currency
        //    //  .HasForeignKey(fx => fx.CurrencyId) // Foreign key
        //    //  .OnDelete(DeleteBehavior.Restrict); // Prevent 
        //}

        /// <summary>
        /// Configure Many To One Relationships
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="foreignKeyPropertyExpression"></param>
        /// <param name="navigationPropertyExpression"></param>
        /// <exception cref="ArgumentException"></exception>
        //private static void ConfigureForeignKey<TEntity, TProperty, TRelatedEntity>(
        //    ModelBuilder modelBuilder,
        //    Expression<Func<TEntity, TProperty>> foreignKeyPropertyExpression,
        //    Expression<Func<TEntity, TRelatedEntity?>>? navigationPropertyExpression = null,
        //    Expression<Func<TRelatedEntity, IEnumerable<TEntity>?>>? collectionPropertyExpression = null)
        //    where TEntity : class
        //    where TRelatedEntity : class
        //{
        //    var foreignKeyProperty = (foreignKeyPropertyExpression.Body as MemberExpression)?.Member?.Name
        //        ?? throw new ArgumentException($"FK? Property Expression is not a Member Expression for <{typeof(TEntity).Name}, {typeof(TRelatedEntity).Name}>",
        //            nameof(foreignKeyPropertyExpression));

        //    modelBuilder.Entity<TEntity>()
        //        .HasOne(navigationPropertyExpression)
        //        .WithMany(collectionPropertyExpression)
        //        .HasForeignKey(foreignKeyProperty)
        //        .OnDelete(DeleteBehavior.Restrict);
        //}

        //internal IEnumerable<EnumValue> GetEnumValuesFromDataModels()
        //{
        //    var results = new List<EnumValue>();

        //    // Use reflection to find all DbSet properties
        //    var dbSetProperties = GetType().GetProperties()
        //        .Where(p => p.PropertyType.IsGenericType &&
        //                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        //    foreach (var prop in dbSetProperties)
        //    {
        //        var entityType = prop.PropertyType.GetGenericArguments()[0];
        //        var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
        //        var schemaName = tableAttribute?.Schema ?? DefaultDatabaseSchema;
        //        var tableName = tableAttribute?.Name ?? entityType.Name;

        //        // Find enum properties in the entity type
        //        var enumProperties = entityType.GetProperties()
        //            .Where(p => p.PropertyType.IsEnum || p.PropertyType.IsGenericType &&
        //                                                 p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
        //                                                 p.PropertyType.GetGenericArguments()[0].IsEnum);

        //        foreach (var enumProp in enumProperties)
        //        {
        //            var enumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
        //            var enumNames = Enum.GetNames(enumType);
        //            var enumValues = Enum.GetValues(enumType);

        //            var columnAttribute = enumProp.GetCustomAttribute<ColumnAttribute>();
        //            var columnName = columnAttribute?.Name ?? enumProp.Name;

        //            for (int i = 0; i < enumNames.Length; i++)
        //            {
        //                var value = Convert.ToInt32(enumValues.GetValue(i));
        //                var name = enumNames[i];
        //                var descriptionAttribute = enumType.GetField(name)?
        //                    .GetCustomAttributes(typeof(DescriptionAttribute), false)?
        //                    .FirstOrDefault() as DescriptionAttribute;
        //                var description = descriptionAttribute?.Description
        //                    ?? name.InsertSpacesBeforeCaptials();

        //                results.Add(new EnumValue
        //                {
        //                    SchemaName = schemaName,
        //                    TableName = tableName,
        //                    ColumnName = columnName,
        //                    Value = value,
        //                    Name = name,
        //                    Description = description
        //                });
        //            }
        //        }
        //    }

        //    return results;
        //}
    }
}
