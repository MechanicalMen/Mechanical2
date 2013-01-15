using System;
using System.Collections.Generic;
using Mechanical.Core;
using Mechanical.MagicBag;
using Mechanical.MagicBag.Initializers;
using NUnit.Framework;

namespace Mechanical.Tests.MagicBag
{
    [TestFixture]
    public class AllocatorTests
    {
        [Test]
        public void DelegateAllocatorTest()
        {
            var mapping = Map<int>.To(bag => 5).AsTransient().ToMapping();
            Assert.AreEqual(5, mapping.Get(MagicBagTests.EmptyBag));

            mapping = Map<int>.To(bag => bag.NotNullReference() ? 1 : 0).AsTransient().ToMapping();
            Assert.AreEqual(1, mapping.Get(MagicBagTests.EmptyBag));
        }

        [Test]
        public void ConstructorAllocatorTest()
        {
            var mapping = Map<MagicBagTests.IntWrapper>.ToDefault<MagicBagTests.IntWrapper>().AsTransient().ToMapping();
            Assert.AreEqual(0, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            var ctor = Reveal.Constructor(() => new KeyValuePair<int, int>(0, 0));
            mapping = Map<KeyValuePair<int, int>>.To<KeyValuePair<int, int>>(ctor, p => p.Const(6).Func(() => 7)).AsTransient().ToMapping();
            var pair = (KeyValuePair<int, int>)mapping.Get(MagicBagTests.EmptyBag);
            Assert.AreEqual(6, pair.Key);
            Assert.AreEqual(7, pair.Value);

            var bag = new Mechanical.MagicBag.MagicBag.Basic(Map<int>.To(() => 8).AsTransient());
            mapping = Map<KeyValuePair<int, int>>.ToInject<KeyValuePair<int, int>>(ctor).AsTransient().ToMapping();
            pair = (KeyValuePair<int, int>)mapping.Get(bag);
            Assert.AreEqual(8, pair.Key);
            Assert.AreEqual(8, pair.Value);
        }

        [Test]
        public void InitializingAllocatorTest()
        {
            var mapping = Map<int>.To(() => 5).AsTransient().Call(new InitializerTests.IntInitializer()).ToMapping();
            Assert.AreEqual(6, (int)mapping.Get(MagicBagTests.EmptyBag));
        }
    }
}
