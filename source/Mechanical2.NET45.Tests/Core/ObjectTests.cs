using System;
using Mechanical.Core;
using NUnit.Framework;

namespace Mechanical.Tests.Core
{
    [TestFixture]
    public class ObjectTests
    {
        [Test]
        public void Test()
        {
            var obj = new object();
            Assert.AreEqual(true, obj.NotNullReference());
            Assert.AreEqual(false, obj.NullReference());

            obj = null;
            Assert.AreEqual(false, obj.NotNullReference());
            Assert.AreEqual(true, obj.NullReference());
        }
    }
}
