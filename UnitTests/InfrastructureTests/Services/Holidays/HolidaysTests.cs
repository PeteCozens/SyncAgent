using Common;
using Common.Extensions;
using Infrastructure.Services.Holidays;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.InfrastructureTests.Services.Holidays
{
    [TestClass]
    public class HolidaysTests
    {
        private CalendarificService GetService()
        {
            var root = ServiceFactory.GetService<IConfiguration>()
               ?? throw new Exception("Unable to access Unit Test configuration");

            var config = root.GetSection<CalendarificServiceConfig>("CalendarificService")
                ?? throw new Exception("Unable to find the CalendarificService section in the unit test configuration");

            return new CalendarificService(config);
        }

        [TestMethod]
        public async Task GetSupportedCountriesAsync()
        {
            // Arrange

            var svc = GetService();

            // Act

            var countries = await svc.GetSupportedCountriesAsync();

            // Assert

            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Any());

            var gb = countries.SingleOrDefault(x => x.Code == "GB");
            Assert.IsNotNull(gb);
            Assert.AreEqual("United Kingdom", gb.Description);
        }

        [TestMethod]
        public async Task GetHolidaysAsync()
        {
            // Arrange

            var svc = GetService();

            // Act

            var holidays = await svc.GetHolidaysAsync("GB", 2024);

            // Assert

            Assert.IsNotNull(holidays);
            Assert.AreEqual(92, holidays.Length);

            Assert.IsTrue(holidays.Any(x => x.Date.ToString("yyyy-MM-dd") == "2024-01-01"));
            Assert.IsTrue(holidays.Any(x => x.Date.ToString("yyyy-MM-dd") == "2024-12-25"));
            Assert.IsTrue(holidays.Any(x => x.Date.ToString("yyyy-MM-dd") == "2024-12-26"));
        }
    }
}
