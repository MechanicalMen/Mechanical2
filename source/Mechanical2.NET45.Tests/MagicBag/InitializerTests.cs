using System;
using System.Collections.Generic;
using Mechanical.Core;
using Mechanical.MagicBag;
using Mechanical.MagicBag.Initializers;
using NUnit.Framework;

namespace Mechanical.Tests.MagicBag
{
    [TestFixture]
    public class InitializerTests
    {
        internal class IntInitializer : IMagicBagInitializer<int>
        {
            public int Initialize( int allocated, Mechanical.MagicBag.IMagicBag magicBag )
            {
                return allocated + 1;
            }
        }

        private int field = 0;

        private int Property { get; set; }

        [Test]
        public void CallTests()
        {
            var mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().Call(( wrapper, bag ) => wrapper.AddAssign(1)).ToMapping();
            Assert.AreEqual(3, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().Call(wrapper => wrapper.AddAssign(2)).ToMapping();
            Assert.AreEqual(4, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().Call(( wrapper, bag ) => wrapper.Add(3)).ToMapping();
            Assert.AreEqual(5, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().Call(wrapper => wrapper.Add(4)).ToMapping();
            Assert.AreEqual(6, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            var method = Reveal.Method(( MagicBagTests.IntWrapper w ) => w.AddAssign(0));
            mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().Call(method, p => p.Const(5), useMethodResult: false).ToMapping();
            Assert.AreEqual(7, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            method = Reveal.Method(( MagicBagTests.IntWrapper w ) => w.Add(0));
            mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().Call(method, p => p.Const(6), useMethodResult: true).ToMapping();
            Assert.AreEqual(8, MagicBagTests.GetIntFromWrapper(mapping.Get(MagicBagTests.EmptyBag)));

            method = Reveal.Method(( MagicBagTests.IntWrapper w ) => w.AddAssign(0));
            var magicBag = new Mechanical.MagicBag.MagicBag.Basic(Map<int>.To<int>(() => 7).AsTransient());
            mapping = Map<MagicBagTests.IntWrapper>.To<MagicBagTests.IntWrapper>(() => new MagicBagTests.IntWrapper(2)).AsTransient().CallInject(method).ToMapping();
            Assert.AreEqual(9, MagicBagTests.GetIntFromWrapper(mapping.Get(magicBag)));
        }

        [Test]
        public void SetTests()
        {
            System.Reflection.MemberInfo member;

            member = Reveal.Field(() => this.field);
            var mapping = Map<InitializerTests>.ToDefault<InitializerTests>().AsTransient().Set(member, p => p.Const(3)).ToMapping();
            var obj = (InitializerTests)mapping.Get(MagicBagTests.EmptyBag);
            Assert.AreEqual(3, obj.field);

            member = Reveal.Property(() => this.Property);
            mapping = Map<InitializerTests>.ToDefault<InitializerTests>().AsTransient().Set(member, p => p.Const(4)).ToMapping();
            obj = (InitializerTests)mapping.Get(MagicBagTests.EmptyBag);
            Assert.AreEqual(4, obj.Property);
        }
    }
}
