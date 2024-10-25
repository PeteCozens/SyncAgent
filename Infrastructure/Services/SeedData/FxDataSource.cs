using Common.Models;
using Infrastructure.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Linq;

namespace Infrastructure.Services.SeedData
{
    [ExcludeFromCodeCoverage]
    public class FxDataSource
    {
        private class RateDate
        {
            public DateTime Date { get; set; }
            public List<Rate> Rates { get; set; } = [];
        }

        private class Rate
        {
            public string Currency { get; set; } = string.Empty;
            public decimal RateValue { get; set; }
        }

        [SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Readability")]
        public static IEnumerable<FxRate> GetData(AppDbContext ctx)
        {
            // Download FXC Rate data as XML

            var url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml";

            using var handler = new HttpClientHandler { UseDefaultCredentials = true };
            using var client = new HttpClient(handler);
            var xml = client.GetStringAsync(url).Result;
            if (string.IsNullOrEmpty(xml))
                throw new Exception("Unable to retrieve data from " + url);

            // Parse the XML

            var items = new List<FxRate>();

            XDocument doc = XDocument.Parse(xml);
            XNamespace eurofxref = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

            var cubeElements = doc.Descendants(eurofxref + "Cube").Where(x => x.Attribute("time") != null);

            var ccyLookup = ctx.Currencies.ToDictionary(x => x.Code, x => x.CurrencyId, StringComparer.InvariantCultureIgnoreCase);

            foreach (var cube in cubeElements)
            {
                var value = cube.Attribute("time")?.Value;
                if (value == null)
                    continue;

                var date = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                foreach (var currencyElement in cube.Elements(eurofxref + "Cube"))
                {
                    var currency = currencyElement.Attribute("currency")?.Value;
                    var rate = currencyElement.Attribute("rate")?.Value;

                    if (currency == null || rate == null) 
                        continue;

                    if (!ccyLookup.TryGetValue(currency, out var currencyId))
                        continue;   // Unknown currency code

                    items.Add(new FxRate { CurrencyId = currencyId, FromDate = date, Rate = decimal.Parse(rate) });
                }
            }

            // Calculate the appropriate ToDates to account for gaps in the data

            items = items.OrderBy(x => x.CurrencyId).ThenBy(x => x.FromDate).ToList();

            for (var i = 0; i < items.Count - 1; i++)
            {
                if (items[i].CurrencyId == items[i + 1].CurrencyId)
                {
                    // Currency is the same
                    items[i].ToDate = items[i + 1].FromDate.AddDays(-1);
                }
                else
                {
                    // Currency is changing
                    items[i].ToDate = DateTime.MaxValue.Date;
                }
            }
            items.Last().ToDate = DateTime.MaxValue.Date;

            // Return the results

            return items;
        }
    }
}
