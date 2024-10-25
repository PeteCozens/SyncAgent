using Common.Models;

namespace Infrastructure.Services.Holidays
{
    internal interface IHolidaySource
    {
        Task<CodeTable[]> GetSupportedCountriesAsync();
        Task<Holiday[]> GetHolidaysAsync(string countryCode, int year);
    }
}
