using System;
using System.Collections.Generic;
using Mechanical.Core;
using Mechanical.MagicBag;
using Mechanical.MagicBag.Initializers;
using NUnit.Framework;

namespace Mechanical.Tests.MagicBag
{
    [TestFixture]
    public class CacheTests
    {
        [Test]
        public void TransientTest()
        {
            int value = 0;

            var mapping = Map<int>.To(() => ++value).AsTransient().ToMapping();
            Assert.AreEqual(0, value);
            Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(1, value);
            Assert.AreEqual(2, (int)mapping.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(2, value);

            var thread = new System.Threading.Thread(() =>
            {
                Assert.AreEqual(3, (int)mapping.Get(MagicBagTests.EmptyBag));
                Assert.AreEqual(3, value);
                Assert.AreEqual(4, (int)mapping.Get(MagicBagTests.EmptyBag));
                Assert.AreEqual(4, value);
            });
            thread.Start();
            thread.Join();
        }

        [Test]
        public void SingletonTest()
        {
            int value = 0;

            var mapping = Map<int>.To(() => ++value).AsSingleton().ToMapping();
            Assert.AreEqual(0, value);
            Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(1, value);
            Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(1, value);

            var thread = new System.Threading.Thread(() =>
            {
                Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
                Assert.AreEqual(1, value);
                Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
                Assert.AreEqual(1, value);
            });
            thread.Start();
            thread.Join();
        }

        [Test]
        public void ThreadLocalTest()
        {
            int value = 0;

            var mapping = Map<int>.To(() => ++value).AsThreadLocal().ToMapping();
            Assert.AreEqual(0, value);
            Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(1, value);
            Assert.AreEqual(1, (int)mapping.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(1, value);

            var thread = new System.Threading.Thread(() =>
            {
                Assert.AreEqual(2, (int)mapping.Get(MagicBagTests.EmptyBag));
                Assert.AreEqual(2, value);
                Assert.AreEqual(2, (int)mapping.Get(MagicBagTests.EmptyBag));
                Assert.AreEqual(2, value);
            });
            thread.Start();
            thread.Join();
        }
    }
}
