using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.CommonTests.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [DataTestMethod]
        [DataRow("2020-06-01", "2020-06-01")]
        [DataRow("2020-06-12", "2020-06-01")]
        [DataRow("2020-06-30", "2020-06-01")]
        public void StartOfMonthTests(string value, string expected)
        {
            // Arrange
            var v = DateTime.Parse(value);
            var e = DateTime.Parse(expected);

            // Act
            var result = v.StartOfMonth();

            // Assert
            Assert.AreEqual(e, result);
        }

        [DataTestMethod]
        [DataRow("2020-06-01", "2020-06-30")]
        [DataRow("2020-06-12", "2020-06-30")]
        [DataRow("2020-06-30", "2020-06-30")]
        public void EndOfMonthTests(string value, string expected)
        {
            // Arrange
            var v = DateTime.Parse(value);
            var e = DateTime.Parse(expected);

            // Act
            var result = v.EndOfMonth();

            // Assert
            Assert.AreEqual(e, result);
        }
    }
}
