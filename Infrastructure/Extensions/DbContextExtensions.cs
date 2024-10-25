using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Extensions
{
    internal static class DbContextExtensions
    {
        /// <summary>
        /// Performs the same job as a SQL MERGE statement, although because it brings the entire destination 
        /// dataset down into memory, it is best not used with large datasets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="ctx"></param>
        /// <param name="dbSetSelector"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static DbContext Merge<T, TContext>(this TContext ctx, IEnumerable<T> source, Expression<Func<TContext, DbSet<T>>> dbSetSelector)
            where T : class
            where TContext : DbContext
        {
            var dbSet = dbSetSelector.Compile()(ctx);

            // Get primary key(s) for T
            var keys = ctx.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.PropertyInfo)
                .ToList();

            if (keys == null || keys.Count == 0)
                throw new InvalidOperationException("No primary key defined for entity type " + typeof(T).Name);

            var existingItems = dbSet.ToList();
            foreach (var item in source)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var keyValues = keys.Select(k => k.GetValue(item)).ToArray();
                var existingItem = existingItems.SingleOrDefault(e => keys.All(k => k.GetValue(e).Equals(k.GetValue(item))));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                if (existingItem == null)
                    // Insert new item
                    dbSet.Add(item);
                else
                    // Update existing item
                    ctx.Entry(existingItem).CurrentValues.SetValues(item);
            }

            // Delete items not in the source list
            foreach (var existingItem in existingItems)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (!source.Any(i => keys.All(k => k.GetValue(i).Equals(k.GetValue(existingItem)))))
                    dbSet.Remove(existingItem);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return ctx;
        }

        /// <summary>
        /// Inserts and Updates the records into the destination DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="ctx"></param>
        /// <param name="dbSetSelector"></param>
        /// <returns></returns>
        public static TContext Upsert<T, TContext>(this TContext ctx, IEnumerable<T> items, Expression<Func<TContext, DbSet<T>>> dbSetSelector)
            where T : class
            where TContext : DbContext
        {
            if (items == null)
                throw new ArgumentException("items may not be null", nameof(items));

            if (ctx == null)
                throw new ArgumentException("context may not be null", nameof(ctx));

            if (dbSetSelector == null)
                throw new ArgumentException("dbSetSelector may not be null", nameof(dbSetSelector));

            var dbSet = dbSetSelector.Compile()(ctx);

            // Get primary key(s) for T

            var keys = ctx.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.PropertyInfo)
                .ToList();

            if (keys == null || keys.Count == 0)
                throw new InvalidOperationException("No primary key defined for entity type " + typeof(T).Name);

            // Insert or update records

            foreach (var item in items)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var existingItem = dbSet.SingleOrDefault(e => keys.All(k => k.GetValue(e).Equals(k.GetValue(item))));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (existingItem == null)
                    // Insert new item
                    dbSet.Add(item);
                else
                    // Update existing item
                    ctx.Entry(existingItem).CurrentValues.SetValues(item);
            }

            return ctx;
        }
    }
}
