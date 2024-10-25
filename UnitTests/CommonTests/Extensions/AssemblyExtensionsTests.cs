using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace UnitTests.CommonTests.Extensions
{
    [TestClass]
    public class AssemblyExtensionsTests
    {
        private const string ResourceName = @"UnitTests.CommonTests.Extensions.EmbeddedResource.txt";

        [TestMethod]
        public void GetEmbeddedResourceNamesTest()
        {
            var names = typeof(AssemblyExtensionsTests).Assembly.GetEmbeddedResourceNames();
            Assert.IsTrue(ResourceName.In(names));
        }

        [TestMethod]
        public void GetEmbeddedResourceStringTest()
        {
            var expected = "This is an Embedded Resource";
            var value = typeof(AssemblyExtensionsTests).Assembly.GetEmbeddedResourceString(ResourceName);
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void GetEmbeddedResourceDataTest()
        {
            var expected = Encoding.UTF8.GetBytes("This is an Embedded Resource");
            var value = typeof(AssemblyExtensionsTests).Assembly.GetEmbeddedResourceData(ResourceName);
            CollectionAssert.AreEqual(expected, value);
        }
    }
}
