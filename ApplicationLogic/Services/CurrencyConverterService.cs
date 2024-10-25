using Microsoft.Extensions.Logging;
using Common.Models;
using Infrastructure.Services.Repositories;

namespace ApplicationLogic.Services
{
    public class CurrencyConverterService(ILogger<CurrencyConverterService> log, IRepository repo) : IApplicationLogic
    {

        /// <summary>
        /// Returns the Base currency used for FX rates in the database
        /// </summary>
        public string BaseCurrency => repo.BaseCurrency;

        /// <summary>
        /// Perform a currency conversion.
        /// </summary>
        /// <param name="amount">Monetary amount</param>
        /// <param name="fromCcyCode">Currency that the amount is in</param>
        /// <param name="toCcyCode">Currency that we want to convert to</param>
        /// <param name="rateDate">The date at which the currency conversion is to take place</param>
        /// <param name="roundToDecimals">The number of decimal places to round the result to. If the number is negative, then no rounding is performed</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<decimal> ConvertAsync(decimal amount, string fromCcyCode, string toCcyCode, DateTime rateDate, int roundToDecimals = -1)
        {
            if (string.IsNullOrWhiteSpace(fromCcyCode))
                throw new ArgumentException("Please specify the code of the currency we're converting from", nameof(fromCcyCode));

            if (string.IsNullOrWhiteSpace(toCcyCode))
                throw new ArgumentException("Please specify the code of the currency we're converting to", nameof(toCcyCode));

            log.LogDebug("Converting {amount} from {fromCcy} to {toCcy} as at {date}", amount, fromCcyCode, toCcyCode, rateDate);

            if (amount == 0 || fromCcyCode.Equals(toCcyCode, StringComparison.InvariantCultureIgnoreCase))
                return amount;

            if (fromCcyCode != repo.BaseCurrency)
            {
                var exchangeRate = await GetRateAsync(fromCcyCode, rateDate.Date)
                    ?? throw new Exception($"No rates available for {fromCcyCode} on {rateDate:dd MMM yyyy}");
                amount /= exchangeRate.Rate;
            }

            if (toCcyCode != repo.BaseCurrency)
            {
                var exchangeRate = await GetRateAsync(toCcyCode, rateDate.Date)
                    ?? throw new Exception($"No rates available for {toCcyCode} on {rateDate:dd MMM yyyy}");
                amount *= exchangeRate.Rate;
            }

            return roundToDecimals < 0
                ? amount
                : decimal.Round(amount, roundToDecimals);
        }

        private async Task<FxRate?> GetRateAsync(string currencyCode, DateTime rateDate)
        {
            return (await repo.GetRatesAsync(x =>
                x.Currency != null &&
                x.Currency.Code == currencyCode &&
                x.FromDate <= rateDate &&
                x.ToDate >= rateDate, false, false, x => x.Currency)
            ).SingleOrDefault();
        }
    }
}
