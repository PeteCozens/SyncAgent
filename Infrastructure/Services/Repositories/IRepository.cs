using Common.Models;
using System.Linq.Expressions;

namespace Infrastructure.Services.Repositories
{
    public interface IRepository
    {
        Task FlushAsync();
        //string BaseCurrency { get; }

        //Task<List<Currency>> GetCurrenciesAsync(Expression<Func<Currency, bool>>? filter = null, bool withTracking = false, bool includeDeleted = false, params Expression<Func<Currency, object?>>[] propertiesToInclude);
        //Task<List<TResult>> SelectCurrenciesAsync<TResult>(Expression<Func<Currency, TResult>> selector, Expression<Func<Currency, bool>>? filter = null, bool includeDeleted = false, params Expression<Func<Currency, object?>>[] propertiesToInclude);
        //Task<Currency> SaveAsync(Currency value, bool autoFlush = true);


        //Task<List<FxRate>> GetRatesAsync(Expression<Func<FxRate, bool>>? filter = null, bool withTracking = false, bool includeDeleted = false, params Expression<Func<FxRate, object?>>[] propertiesToInclude);
        //Task<List<TResult>> SelectRatesAsync<TResult>(Expression<Func<FxRate, TResult>> selector, Expression<Func<FxRate, bool>>? filter = null, bool includeDeleted = false, params Expression<Func<FxRate, object?>>[] propertiesToInclude);
        //Task<FxRate> SaveAsync(FxRate value, bool autoFlush = true);
    }
}
