using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.CommonTests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [DataRow("Hello World", 100, "Hello World")]
        [DataRow("Hello World", 11, "Hello World")]
        [DataRow("Hello World", 10, "Hello Worl")]
        [DataRow("Hello World", 4, "Hell")]
        [DataRow("Hello World", 0, "")]
        [DataRow("", 10, "")]
        [DataRow(null, 10, null)]
        [DataTestMethod]
        public void Left(string value, int length, string expected)
        {
            var result = value.Left(length);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "The length provided may not be less than Zero (Parameter 'length')")]
        public void LeftException()
        {
            var value = "ExceptionTest";
            _ = value.Left(-1);
            // The following code cannot be run as an ArgumentOutOfRangeException is thrown by the previous line
            Assert.Fail();
        }

        [DataRow("Hello World", 0, 4, "Hell")]
        [DataRow("Hello World", 6, 5, "World")]
        [DataRow("Hello World", 3, 4, "lo W")]
        [DataRow("Hello World", 4, 0, "")]
        [DataRow("", 10, 1, "")]
        [DataRow(null, 10, 1, null)]
        [DataTestMethod]
        public void Mid(string value, int index, int length, string expected)
        {
            var result = value.Mid(index, length);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(-1, 1)]
        [DataRow(-1, -1)]
        [DataRow(1, -1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MidException(int index, int length)
        {
            var value = "ExceptionTest";
            _ = value.Mid(index, length);
            // The following code cannot be run as an ArgumentOutOfRangeException is thrown by the previous line
            Assert.Fail();
        }

        [DataRow("Hello World", 100, "Hello World")]
        [DataRow("Hello World", 11, "Hello World")]
        [DataRow("Hello World", 10, "ello World")]
        [DataRow("Hello World", 4, "orld")]
        [DataRow("Hello World", 0, "")]
        [DataRow("", 10, "")]
        [DataRow(null, 10, null)]
        [DataTestMethod]
        public void Right(string value, int length, string expected)
        {
            var result = value.Right(length);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "The length provided may not be less than Zero (Parameter 'length')")]
        public void RightException()
        {
            var value = "ExceptionTest";
            _ = value.Right(-1);
            // The following code cannot be run as an ArgumentOutOfRangeException is thrown by the previous line
            Assert.Fail();
        }

        [DataRow("apples", new[] { "apples", "oranges", "pears" }, true)]
        [DataRow("APPLES", new[] { "apples", "oranges", "pears" }, true)]
        [DataRow("AppLES", new[] { "apples", "oranges", "pears" }, true)]
        [DataRow("Kiwis", new[] { "apples", "oranges", "pears" }, false)]
        [DataRow("", new[] { "apples", "oranges", "pears" }, false)]
        [DataTestMethod]
        public void In(string value, string[] items, bool expected)
        {
            Assert.AreEqual(expected, value.In(items, StringComparer.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void InException()
        {
            var v = "hello".In(["a", "b"]);
            Assert.IsFalse(v);
        }

        [TestMethod]
        public void InEmptyArray()
        {
            var v = "hello".In([]);
            Assert.IsFalse(v);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'source')")]
        public void InNullArray()
        {
            string[]? empty = null;
#pragma warning disable CS8604 // Possible null reference argument.
            var v = "hello".In(empty);
#pragma warning restore CS8604 // Possible null reference argument.
                              // The following code cannot be run as an ArgumentNullException is thrown by the previous line
            Assert.IsFalse(v);
        }


        [DataRow("Hello World", "HelloWorld")]
        [DataRow("Hello World 123", "HelloWorld123")]
        [DataRow("Hello_World#123", "HelloWorld123")]
        [DataTestMethod]
        public void RemoveNonAlphaNumerics(string value, string expected)
        {
            var result = value.RemoveNonAlphaNumerics();
            Assert.AreEqual(expected, result);
        }

        [DataRow("1", 1)]
        [DataRow("123", 123)]
        [DataRow("-123", -123)]
        [DataRow("aaaa", 0)]
        [DataTestMethod]
        public void ToInt(string value, int expected)
        {
            var result = value.ToInt();
            Assert.AreEqual(expected, result);
        }

        [DataRow("aaaa", "aaaa")]
        [DataRow("Hello World", "Hello-World")]
        [DataRow("Hello World!", "Hello-World")]
        [DataRow("Hello-World", "Hello-World")]
        [DataRow("Hello_World", "Hello_World")]
        [DataRow("Hello    World", "Hello-World")]
        [DataRow(" Hello World ", "Hello-World")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_")]
        [DataRow("Hello World!\"£$%^&*()\\|<,>.?/:;@'~#{[}]=`¬", "Hello-World")]
        [DataTestMethod]
        public void ToSlug(string value, string expected)
        {
            var result = value.ToSlug();
            Assert.AreEqual(expected, result);

            result = value.ToSlug(true);
            Assert.AreEqual(expected.ToLowerInvariant(), result);
        }

        [DataRow("aaa", "aaa")]
        [DataRow("aaa,bbb", "aaa", "bbb")]
        [DataRow("aaa,bbb,ccc", "aaa", "bbb", "ccc")]
        [DataRow("aaa,bbb,ccc,", "aaa", "bbb", "ccc")]
        [DataRow(",aaa,bbb,ccc,", "aaa", "bbb", "ccc")]
        [DataRow(",aaa,bbb,,,ccc,", "aaa", "bbb", "ccc")]
        [DataRow("")]
        [DataTestMethod]
        public void SplitEx(string value, params string[] expected)
        {
            var result = value.SplitEx(",");
            CollectionAssert.AreEqual(expected, result);

            result = value.SplitEx(new[] { "," }, StringSplitOptions.None);
            var list = value.Split([',']);
            CollectionAssert.AreEqual(list, result);

            result = value.SplitEx(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ParseJson()
        {
            var fs = new FileData
            {
                Name = "foo",
                Created = DateTime.UtcNow.AddDays(-1),
                Modified = DateTime.UtcNow
            };

            var json = fs.ToJson();
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Length > 0);

            var result = json.ParseJson<FileData>();
            Assert.IsNotNull(result);
            Assert.AreEqual(fs.Name, result.Name);
            Assert.AreEqual(fs.Created, result.Created);
            Assert.AreEqual(fs.Modified, result.Modified);
        }

        class FileData
        {
            public string Name { get; set; } = string.Empty;
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
        }

        [DataTestMethod]
        [DataRow("Hello World", "World")]
        [DataRow("World", "World")]
        [DataRow("Here's another test", "test")]
        [DataRow("", "")]
        public void LastWord(string value, string expected)
        {
            var result = value.LastWord();
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("HelloWorld", "Hello World")]
        [DataRow("World", "World")]
        [DataRow("YetAnotherTest", "Yet Another Test")]
        [DataRow("", "")]
        [DataRow("ACMEAnvil", "ACME Anvil")]
        [DataRow("BILL_NUM", "BILL NUM")]
        public void InsertSpacesBeforeCaptials(string value, string expected)
        {
            var result = value.InsertSpacesBeforeCaptials();
            Assert.AreEqual(expected, result);
        }

    }
}
