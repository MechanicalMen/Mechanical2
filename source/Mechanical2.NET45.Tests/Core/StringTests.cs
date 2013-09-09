using System;
using Mechanical.Core;
using NUnit.Framework;

namespace Mechanical.Tests.Core
{
    [TestFixture]
    public class StringTests
    {
        [Test]
        public static void NullOrEmptyTest()
        {
            Assert.True(((string)null).NullOrEmpty());
            Assert.True(string.Empty.NullOrEmpty());
            Assert.False(" a b ".NullOrEmpty());
        }

        [Test]
        public static void NullOrLengthyTest()
        {
            Assert.True(((string)null).NullOrLengthy());
            Assert.True(string.Empty.NullOrLengthy());
            Assert.True(" ".NullOrLengthy());
            Assert.True(" a".NullOrLengthy());
            Assert.True("a ".NullOrLengthy());
            Assert.True(" a ".NullOrLengthy());
            Assert.False("a b".NullOrLengthy());
        }

        [Test]
        public static void NullOrWhiteSpaceTest()
        {
            Assert.True(((string)null).NullOrWhiteSpace());
            Assert.True(string.Empty.NullOrWhiteSpace());
            Assert.True(" ".NullOrWhiteSpace());
            Assert.False(" a ".NullOrWhiteSpace());
        }
    }
}
