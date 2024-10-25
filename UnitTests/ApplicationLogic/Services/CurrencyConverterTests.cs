using ApplicationLogic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ApplicationLogic.Services
{
    [TestClass]
    public class CurrencyConverterTests
    {
        private const string _baseCcy = @"EUR";

        [TestMethod]
        public void CheckBaseCcy()
        {
            // Arrange

            var log = MockFactory.GetLogger<CurrencyConverterService>();
            var repo = MockFactory.GetRepository();
            var converter = new CurrencyConverterService(log.Object, repo.Object);

            // Act

            var result = converter.BaseCurrency;

            // Assert

            Assert.AreEqual(_baseCcy, result);
        }

        [DataTestMethod]
        [DataRow(1000, "EUR", "EUR", 1000)]
        [DataRow(1000, "GBP", "GBP", 1000)]
        [DataRow(1000, "EUR", "GBP", 800)]
        [DataRow(1000, "GBP", "EUR", 1250)]
        [DataRow(1000, "GBP", "USD", 1375)]
        [DataRow(0, "GBP", "USD", 0)]
        public void Convert(double amount, string fromCcy, string toCcy, double expected)
        {
            // Arrange

            var log = MockFactory.GetLogger<CurrencyConverterService>();
            var repo = MockFactory.GetRepository();
            var converter = new CurrencyConverterService(log.Object, repo.Object);

            // Act

            var result = converter.ConvertAsync((decimal)amount, fromCcy, toCcy, DateTime.Today, 2).Result;

            // Assert

            Assert.AreEqual((decimal)expected, result);
        }

        [TestMethod]
        public async Task TestMockRepo()
        {
            // Arrange

            var repo = MockFactory.GetRepository().Object;

            // Act

            var gbp = (await repo.GetCurrenciesAsync(x => x.Code == "GBP")).SingleOrDefault();
            var activeCurrencies = await repo.GetCurrenciesAsync();
            var allCurrencies = await repo.GetCurrenciesAsync(null, false, true);

            // Assert

            Assert.IsNotNull(gbp);
            Assert.AreEqual("£", gbp.Symbol);

            Assert.AreEqual(3, activeCurrencies.Count);
            Assert.IsFalse(activeCurrencies.Any(x => x.Code == "DEM"));

            Assert.AreEqual(4, allCurrencies.Count);
            Assert.IsTrue(allCurrencies.Any(x => x.Code == "DEM"));
        }
    }
}
