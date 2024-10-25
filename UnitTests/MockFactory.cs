using Common.Models;
using Infrastructure.Services.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace UnitTests
{
    internal static class MockFactory
    {
        public static Mock<ILogger<T>> GetLogger<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }

        private static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> items) where T : class
        {
            foreach (var item in items)
                yield return item;
            await Task.Yield();     // Does nothing, but allows this method to be async
        }

        public static Mock<IRepository> GetRepository()
        {
            var mock = new Mock<IRepository>();

            //MockCurrencies(mock);
            //MockFxRates(mock);

            return mock;
        }

        //private static void MockCurrencies(Mock<IRepository> mock)
        //{
        //    List<Currency> currencies = [
        //        new Currency { CurrencyId = 1, Code = "EUR", Symbol = "€" },
        //        new Currency { CurrencyId = 2, Code = "GBP", Symbol = "£" },
        //        new Currency { CurrencyId = 3, Code = "USD", Symbol = "$" },
        //        new Currency { CurrencyId = 4, Code = "DEM", Symbol = "DEM", SysIsDeleted = true }
        //    ];

        //    mock.Setup(x => x.GetCurrenciesAsync(
        //            It.IsAny<Expression<Func<Currency, bool>>>(), 
        //            It.IsAny<bool>(), 
        //            It.IsAny<bool>(),
        //            It.IsAny<Expression<Func<Currency, object?>>[]>()))
        //        .Returns((Expression<Func<Currency, bool>> filter, bool withTracking, bool includeDeleted, params Expression<Func<Currency, object?>>[] propertiesToInclude) =>
        //        {
        //            return Task.Run(() =>
        //            {
        //                var query = currencies.Where(x => x.SysIsDeleted == false || includeDeleted);
        //                if (filter != null)
        //                    query = query.Where(filter.Compile());
        //                return query.ToList();
        //            });
        //        });

        //    mock.Setup(x => x.SaveAsync(It.IsAny<Currency>(), It.IsAny<bool>()))
        //        .ReturnsAsync((Currency value, bool autoFlush) =>
        //        {
        //            var existing = currencies.SingleOrDefault(x => x.Code == value.Code);
        //            if (existing != null)
        //                currencies.Remove(existing);
        //            currencies.Add(value);
        //            if (value.CurrencyId == 0)
        //                value.CurrencyId = currencies.Max(x => x.CurrencyId) + 1;
        //            return value;
        //        });
        //}

        //private static void MockFxRates(Mock<IRepository> mock)
        //{
        //    var currencies = mock.Object.GetCurrenciesAsync(null, false, true).Result;

        //    var fromDate = DateTime.MinValue.Date;
        //    var toDate = DateTime.MaxValue.Date;
        //    var eur = currencies.Single(x => x.Code == "EUR");
        //    var gbp = currencies.Single(x => x.Code == "GBP");
        //    var usd = currencies.Single(x => x.Code == "USD");
        //    var dem = currencies.Single(x => x.Code == "DEM");

        //    List<FxRate> rates = [
        //        new() { CurrencyId = eur.CurrencyId, Currency = eur, Rate = 1.0m, FromDate = fromDate, ToDate = toDate },
        //        new() { CurrencyId = gbp.CurrencyId, Currency = gbp, Rate = 0.8m, FromDate = fromDate, ToDate = toDate },
        //        new() { CurrencyId = usd.CurrencyId, Currency = usd, Rate = 1.1m, FromDate = fromDate, ToDate = toDate },
        //        new() { CurrencyId = dem.CurrencyId, Currency = dem, Rate = 2.0m, FromDate = fromDate, ToDate = toDate }
        //    ];

        //    mock.SetupGet(x => x.BaseCurrency).Returns("EUR");

        //    var lookup = currencies.ToDictionary(x => x.CurrencyId);

        //    mock.Setup(x => x.GetRatesAsync(
        //            It.IsAny<Expression<Func<FxRate, bool>>>(), 
        //            It.IsAny<bool>(), 
        //            It.IsAny<bool>(),
        //            It.IsAny<Expression<Func<FxRate, object?>>[]>()))
        //        .Returns((Expression<Func<FxRate, bool>> filter, bool withTracking, bool includeDeleted, params Expression<Func<FxRate, object?>>[] propertiesToInclude) =>
        //        {
        //            return Task.Run(() =>
        //            {
        //                var query = rates.Where(x => x.SysIsDeleted == false || includeDeleted);
        //                if (filter != null)
        //                    query = query.Where(filter.Compile());
                        
        //                // Poor man's include
        //                var items = query.ToList();
        //                foreach (var item in items)
        //                    if (lookup.TryGetValue(item.CurrencyId, out var currency))
        //                        item.Currency = currency;

        //                return items;
        //            });
        //        });

        //    mock.Setup(x => x.SaveAsync(It.IsAny<FxRate>(), It.IsAny<bool>()))
        //      .ReturnsAsync((FxRate value, bool autoFlush) =>
        //      {
        //          var existing = rates.SingleOrDefault(x => x.CurrencyId == value.CurrencyId && x.FromDate == value.FromDate);
        //          if (existing != null)
        //              rates.Remove(existing);
        //          rates.Add(value);
        //          return value;
        //      });
        //}
    }
}
