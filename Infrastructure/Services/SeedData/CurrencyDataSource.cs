using Common.Interfaces;
using Common.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.SeedData
{
    [ExcludeFromCodeCoverage]
    public class CurrencyDataSource
    {
        private class CurrencyJson
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("demonym")]
            public string Demonym { get; set; } = string.Empty;
            [JsonPropertyName("majorSingle")]
            public string MajorSingle { get; set; } = string.Empty;
            [JsonPropertyName("majorPlural")]
            public string MajorPlural { get; set; } = string.Empty;
            [JsonPropertyName("symbol")]
            public string Symbol { get; set; } = string.Empty;
            [JsonPropertyName("minorSingle")]
            public string MinorSingle { get; set; } = string.Empty;
            [JsonPropertyName("minorPlural")]
            public string MinorPlural { get; set; } = string.Empty;
            //[JsonPropertyName("ISOdigits")]
            //public int? ISOdigits { get; set; }
            [JsonPropertyName("decimals")]
            public int? Decimals { get; set; }
            [JsonPropertyName("numToBasic")]
            public int? NumToBasic { get; set; }
        }

        public static IEnumerable<Currency> GetData()
        {
            var url = "https://raw.githubusercontent.com/ourworldincode/currency/main/currencies.json";

            using var handler = new HttpClientHandler { UseDefaultCredentials = true };
            using var client = new HttpClient(handler);
            var json = client.GetStringAsync(url).Result;
            if (string.IsNullOrEmpty(json))
                throw new Exception("Unable to retrieve data from " + url);

            var currencies = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, CurrencyJson>>(json);
            if (currencies == null || currencies.Count == 0)
                return Enumerable.Empty<Currency>();

            var items = new List<Currency>();
            foreach (var key in currencies.Keys)
            {
                var ccy = currencies[key];
                items.Add(new Currency {
                    Code = key,
                    Description = ccy.Name,
                    Symbol = ccy.Symbol,
                    Decimals = ccy.Decimals ?? 0,
                    MinorUnits = ccy.NumToBasic ?? 0,
                    MajorSingle = ccy.MajorSingle,
                    MajorPlural = ccy.MajorPlural,      
                    MinorSingle = ccy.MinorSingle,
                    MinorPlural = ccy.MinorPlural,
                });
            }
            return items.OrderBy(x => x.Code);
        }
    }
}
