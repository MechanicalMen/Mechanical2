using System;
using Mechanical.Conditions;
using NUnit.Framework;

namespace Mechanical.Tests.Conditions
{
    [TestFixture]
    public class BooleanConditionTests
    {
        [Test]
        public void Tests()
        {
            Ensure.That(true).IsTrue();
            Assert.Throws<ArgumentException>(() => Ensure.That(false).IsTrue());

            Ensure.That(false).IsFalse();
            Assert.Throws<ArgumentException>(() => Ensure.That(true).IsFalse());
        }
    }
}
