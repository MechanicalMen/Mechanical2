using System;
using Mechanical.Conditions;
using NUnit.Framework;

namespace Mechanical.Tests.Conditions
{
    [TestFixture]
    public class StringConditionTests
    {
        [Test]
        public static void NotNullOrEmptyTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((string)null).NotNullOrEmpty());
            Assert.Throws<ArgumentException>(() => Ensure.That(string.Empty).NotNullOrEmpty());
            Ensure.That(" a b ").NotNullOrEmpty();
        }

        [Test]
        public static void NotNullOrLengthyTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((string)null).NotNullOrLengthy());
            Assert.Throws<ArgumentException>(() => Ensure.That(string.Empty).NotNullOrLengthy());
            Assert.Throws<ArgumentException>(() => Ensure.That(" ").NotNullOrLengthy());
            Assert.Throws<ArgumentException>(() => Ensure.That(" a").NotNullOrLengthy());
            Assert.Throws<ArgumentException>(() => Ensure.That("a ").NotNullOrLengthy());
            Assert.Throws<ArgumentException>(() => Ensure.That(" a ").NotNullOrLengthy());
            Ensure.That("a b").NotNullOrLengthy();
        }

        [Test]
        public static void NotNullOrWhiteSpaceTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((string)null).NotNullOrWhiteSpace());
            Assert.Throws<ArgumentException>(() => Ensure.That(string.Empty).NotNullOrWhiteSpace());
            Assert.Throws<ArgumentException>(() => Ensure.That(" ").NotNullOrWhiteSpace());
            Ensure.That(" a ").NotNullOrWhiteSpace();
        }
    }
}
