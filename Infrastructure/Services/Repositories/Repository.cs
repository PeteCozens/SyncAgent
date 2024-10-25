using Common.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Infrastructure.Services.Repositories
{
    [ExcludeFromCodeCoverage]
    public class Repository(AppDbContext ctx) : IRepository
    {
        public Task FlushAsync() => ctx.SaveChangesAsync();

        #region Internal

        public void Dispose() => ctx.Dispose();

        /// <summary>
        /// Determines whether or not the record needs to be updated on the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool IsUpdateRequired<T>(T value) where T : class
        {
            var state = ctx.Entry(value).State;
            return state switch
            {
                EntityState.Unchanged => false,
                EntityState.Modified => true,
                EntityState.Detached => throw new Exception("Unable to updated detached entity"),
                _ => throw new Exception($"Unhandled State: {state}"),
            };
        }

        /// <summary>
        /// Generic logic for saving a record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet">Represents the table that the record is to be written to</param>
        /// <param name="value">The record to be written to the Database</param>
        /// <param name="isNewRecord">Determines whether or not [value] represents a new record to be inserted into the database</param>
        /// <param name="autoFlush">If true, the update will be saved to the database immediately. If false, then no data will be written to the DB until FlushAsync() is called</param>
        /// <returns></returns>
        private async Task<T> SaveAsync<T>(DbSet<T> dbSet, T value, bool isNewRecord, bool autoFlush) where T : class
        {
            if (isNewRecord)
                dbSet.Add(value);
            else if (!IsUpdateRequired(value))
                return value;
            if (autoFlush)
                await ctx.SaveChangesAsync();
            return value;
        }

        /// <summary>
        /// Retrieve records from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="filter">Where clause</param>
        /// <param name="withTracking">Whether or not the results should be tracked (so that they can be updated)</param>
        /// <param name="includeDeleted">Whether or not to include records where SysIsDeleted=true</param>
        /// <param name="propertiesToInclude">Any virtual properties to include</param>
        /// <returns></returns>
        private static IQueryable<T> Get<T>(DbSet<T> dbSet,
            Expression<Func<T, bool>>? filter,
            bool withTracking,
            bool includeDeleted,
            params Expression<Func<T, object?>>[] propertiesToInclude) where T : class
        {
            IQueryable<T> query = dbSet;
            if (!withTracking)
                query = query.AsNoTracking();
            if (includeDeleted)
                query = query.IgnoreQueryFilters();
            if (propertiesToInclude != null)
                foreach (var property in propertiesToInclude)
                    if (property != null)
                        query = query.Include(property);
            if (filter != null)
                query = query.Where(filter);
            return query;
        }

        #endregion

        public string BaseCurrency => "EUR";

        public Task<List<Currency>> GetCurrenciesAsync(Expression<Func<Currency, bool>>? filter = null, bool withTracking = false, bool includeDeleted = false, params Expression<Func<Currency, object?>>[] propertiesToInclude)
             => Get(ctx.Currencies, filter, withTracking, includeDeleted, propertiesToInclude).ToListAsync();
        public Task<List<TResult>> SelectCurrenciesAsync<TResult>(Expression<Func<Currency, TResult>> selector, Expression<Func<Currency, bool>>? filter = null,
                bool includeDeleted = false, params Expression<Func<Currency, object?>>[] propertiesToInclude)
            => Get(ctx.Currencies, filter, false, includeDeleted, propertiesToInclude).Select(selector).ToListAsync();
        public Task<Currency> SaveAsync(Currency value, bool autoFlush = true)
            => SaveAsync(ctx.Currencies, value, value.SysRowVersion.Length == 0, autoFlush);



        public Task<List<FxRate>> GetRatesAsync(Expression<Func<FxRate, bool>>? filter = null, bool withTracking = false, bool includeDeleted = false, params Expression<Func<FxRate, object?>>[] propertiesToInclude)
             => Get(ctx.FxRates, filter, withTracking, includeDeleted, propertiesToInclude).ToListAsync();

        public Task<List<TResult>> SelectRatesAsync<TResult>(Expression<Func<FxRate, TResult>> selector, Expression<Func<FxRate, bool>>? filter = null,
                bool includeDeleted = false, params Expression<Func<FxRate, object?>>[] propertiesToInclude)
            => Get(ctx.FxRates, filter, false, includeDeleted, propertiesToInclude).Select(selector).ToListAsync();
        public Task<FxRate> SaveAsync(FxRate value, bool autoFlush = true)
            => SaveAsync(ctx.FxRates, value, value.SysRowVersion.Length == 0, autoFlush);
    }
}
