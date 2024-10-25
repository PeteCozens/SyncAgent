using Common.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationLogic.Services
{
    public class DummyServiceConfig
    {
        public string Foo { get; set; } = string.Empty;
        public string Bar { get; set; } = string.Empty;
    }


    /// <summary>
    /// This class exists solely to demonstrate the use of Application Logic Services in the Console Application. It may be deleted as soon as it is
    /// no longer required
    /// </summary>
    /// <param name="log"></param>
    /// <param name="converter"></param>
    /// <param name="config"></param>
    [ExcludeFromCodeCoverage]
    public class DummyService(ILogger<DummyService> log, CurrencyConverterService converter, DummyServiceConfig config) : IApplicationLogic
    {
        public async Task DoWorkAsync()
        {
            log.Here().LogInformation("Fetching currency conversion. {foo} {bar}", config.Foo, config.Bar);

            var fromCcy = "GBP";
            var toCcy = "USD";
            var rateDate = DateTime.Today.AddMonths(-1);
            var amount = 1000;

            try
            {
                var converted = await converter.ConvertAsync(amount, fromCcy, toCcy, rateDate, 2);
                log.Here().LogDebug("{amount} {fromCcy} is {converted} {toCcy} on {rateDate}", amount, fromCcy, converted, toCcy, rateDate);

                rateDate = DateTime.Today.Date;

                converted = await converter.ConvertAsync(amount, fromCcy, toCcy, rateDate, 2);
                log.Here().LogDebug("{amount} {fromCcy} is {converted} {toCcy} on {rateDate}", amount, fromCcy, converted, toCcy, rateDate);
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to convert {amount} {fromCcy} to {toCcy} on {rateDate}", amount, fromCcy, toCcy, rateDate);
                throw;
            }
        }
    }
}
