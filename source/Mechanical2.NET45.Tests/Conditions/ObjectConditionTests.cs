using System;
using Mechanical.Conditions;
using NUnit.Framework;

namespace Mechanical.Tests.Conditions
{
    [TestFixture]
    public class ObjectConditionTests
    {
        [Test]
        public void Tests()
        {
            var obj1 = new object();
            var obj2 = new object();

            Ensure.That((string)null).Null();
            Assert.Throws<ArgumentException>(() => Ensure.That(obj1).Null());

            Ensure.That(obj1).NotNull();
            Assert.Throws<ArgumentNullException>(() => Ensure.That((object)null).NotNull());

            Ensure.That(obj1).SameAs(obj1);
            Assert.Throws<ArgumentException>(() => Ensure.That(obj1).SameAs(obj2));

            Ensure.That(obj1).NotSameAs(obj2);
            Assert.Throws<ArgumentException>(() => Ensure.That(obj1).NotSameAs(obj1));
        }
    }
}
