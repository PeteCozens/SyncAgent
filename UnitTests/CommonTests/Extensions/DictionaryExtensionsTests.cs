using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.CommonTests.Extensions
{
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void Get()
        {
            // Arrange

            Dictionary<string, string> dic = new()
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
                { "Key3", "Value3" }
            };

            // Act

            var result = dic.Get("Key4", (key) => { return key.ToUpperInvariant(); });
            var result2 = dic.Get("Key4", (key) => { return key.ToUpperInvariant(); });

            // Assert

            Assert.AreEqual(4, dic.Count());
            Assert.IsTrue(dic.ContainsKey("Key4"));
            Assert.AreEqual("KEY4", result);
            Assert.AreEqual("KEY4", dic["Key4"]);

            Assert.AreEqual(result, result2);
        }
    }
}
