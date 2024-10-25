using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace UnitTests.CommonTests.Extensions
{
    [TestClass]
    public class GenericExtensionsTests
    {
        private static object Payload => new
        {
            StringValue = "Hello World",
            IntValue = 1,
            DateValue = new DateTime(2000, 1, 1).Date
        };


        [TestMethod]
        public void ToJsonWithSimpleValues()
        {
            var json = GenericExtensions.ToJson((string?)null);
            Assert.AreEqual("null", json);

            json = "Hello".ToJson();
            Assert.AreEqual(@"""Hello""", json);

            json = 1.ToJson();
            Assert.AreEqual("1", json);
        }


        [TestMethod]
        public void ToJsonWithComplexObjectIndented()
        {
            var json = Payload.ToJson(true);
            var expected = @"{
  ""StringValue"": ""Hello World"",
  ""IntValue"": 1,
  ""DateValue"": ""2000-01-01T00:00:00""
}";
            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void ToJsonWithComplexObjectMinimal()
        {
            var json = Payload.ToJson(false);
            var expected = @"{""StringValue"":""Hello World"",""IntValue"":1,""DateValue"":""2000-01-01T00:00:00""}";
            Assert.AreEqual(expected, json);
        }


        [DataRow(new[] { "apples", "oranges", "bananas" }, "apples", true)]
        [DataRow(new[] { "apples", "oranges", "bananas" }, "kiwis", false)]
        [DataRow(new string[] { }, "apples", false)]
        [DataTestMethod]
        public void In_String(string[] items, string value, bool expected)
        {
            Assert.AreEqual(expected, value.In(items));
        }

        [DataRow(new[] { 1, 2, 3, 4, 5 }, 5, true)]
        [DataRow(new[] { 1, 2, 3, 4, 5 }, 6, false)]
        [DataRow(new int[] { }, 0, false)]
        [DataTestMethod]
        public void In_Int(int[] items, int value, bool expected)
        {
            Assert.AreEqual(expected, value.In(items));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'source')")]
        public void In_Null()
        {
            var i = 0;
            int[]? items = null;
#pragma warning disable CS8604 // Possible null reference argument.
            var result = i.In(items);
#pragma warning restore CS8604 // Possible null reference argument.
                              // The following code cannot be run as the previous line throws an ArgumentNullException
            Assert.IsFalse(result);
        }


        [DataTestMethod]
        [DataRow(1, 2, 3, true)]
        [DataRow(1, 0, 3, false)]
        [DataRow(3, 2, 1, true)]
        public void BetweenInt(int low, int value, int high, bool expected)
        {
            var result = value.Between(low, high);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("2020-09-01", "2020-11-05", "2020-12-31", true)]
        [DataRow("2020-09-01", "2021-11-05", "2020-12-31", false)]
        [DataRow("2020-12-31", "2020-11-05", "2020-09-01", true)]
        public void BetweenDate(string low, string value, string high, bool expected)
        {
            var l = DateTime.Parse(low);
            var v = DateTime.Parse(value);
            var h = DateTime.Parse(high);

            var result = v.Between(l, h);
            Assert.AreEqual(expected, result);
        }

    }
}
