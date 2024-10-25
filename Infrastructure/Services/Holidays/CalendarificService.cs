using Common.Models;
using Flurl;
using Flurl.Http;
using System.ComponentModel;
using System.Net;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.Holidays
{
    internal class CalendarificServiceConfig
    {
        public string ApiKey { get; set; } = string.Empty;
    }

    internal class CalendarificService(CalendarificServiceConfig config) : IHolidaySource
    {
        #region API Response Classes

        private class CalendarificResponse
        {
            public CalendarificMeta Meta { get; set; } = new();
            public CalendarificInnerResponse Response { get; set; } = new();
        }

        private class CalendarificMeta
        {
            public HttpStatusCode Code { get; set; }
            [JsonPropertyName("error_type")]
            public string ErrorType { get; set; } = string.Empty;   // eg: "auth failed"
            [JsonPropertyName("error_detail")]
            public string ErrorDetail { get; set; } = string.Empty; // eg: "Missing or invalid api credentials. See https://calendarific.com/api-documentation for details."
        }

        private class CalendarificInnerResponse
        {
            [JsonPropertyName("url")]
            public string Url { get; set; } = string.Empty;
            [JsonPropertyName("countries")]
            public CalendarificCountry[]? Countries { get; set; } = null;
            [JsonPropertyName("holidays")]
            public CalendarificHoliday[]? Holidays { get; set; } = null;
        }

        private class CalendarificCountry
        {
            /*
                "country_name": "Afghanistan",
                "iso-3166": "AF",
                "total_holidays": 24,
                "supported_languages": 2,
                "uuid": "f0357a3f154bc2ffe2bff55055457068",
                "flag_unicode": "🇦🇫"             
             */
            [JsonPropertyName("country_name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("iso-3166")]
            public string Code { get; set; } = string.Empty;

            public override string ToString() => $"{Code}: {Name}";
        }

        private class CalendarificHoliday
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("description")]
            public string Description { get; set; } = string.Empty;
            [JsonPropertyName("date")]
            public CalendarificDate Date { get; set; } = new();
            [JsonPropertyName("type")]
            public string[] Type { get; set; } = [];

            public override string ToString() => $"{Date.Iso}: {Name}";
        }

        private class CalendarificDate
        {
            [JsonPropertyName("iso")]
            public string Iso { get; set; } = string.Empty;
            [JsonPropertyName("datetime")]
            public CalendarificDateParts DateTime { get; set; } = new();
        }

        private class CalendarificDateParts
        {
            [JsonPropertyName("year")]
            public int Year { get; set; }
            [JsonPropertyName("month")]
            public int Month { get; set; }
            [JsonPropertyName("day")]
            public int Day { get; set; }

            public DateTime ToDate() => new DateTime(Year, Month, Day);
        }

        #endregion

        public async Task<CodeTable[]> GetSupportedCountriesAsync()
        {
            var url = "https://calendarific.com/api/v2/countries".AppendQueryParam("api_key", config.ApiKey);
            var response = await url.GetJsonAsync<CalendarificResponse>();
            if (response.Meta.Code != HttpStatusCode.OK)
                throw new Exception(response.Meta.ErrorDetail);

            if (response.Response.Countries == null)
                throw new Exception("No country data extracted from the JSON returned");

            return response.Response
                    .Countries
                    .Select(x => new CodeTable { Code = x.Code, Description = x.Name })
                    .OrderBy(x => x.Code)
                    .ToArray();
        }

        public async Task<Holiday[]> GetHolidaysAsync(string countryCode, int year)
        {
            var url = "https://calendarific.com/api/v2/holidays"
                .AppendQueryParam("api_key", config.ApiKey)
                .AppendQueryParam("country", countryCode)
                .AppendQueryParam("year", year);

            var response = await url.GetJsonAsync<CalendarificResponse>();
            if (response.Meta.Code != HttpStatusCode.OK)
                throw new Exception(response.Meta.ErrorDetail);

            if (response.Response.Holidays == null)
                throw new Exception("No holiday data extracted from the JSON returned");

            return response.Response
                .Holidays
                //.Where(x => x.Type.Any(t => t == "National holiday"))
                .Select(x => new Holiday { Date = new DateTime(x.Date.DateTime.Year, x.Date.DateTime.Month, x.Date.DateTime.Day), Name = x.Name })
                .OrderBy(x => x.Date)
                .ToArray();
        }


    }
}
