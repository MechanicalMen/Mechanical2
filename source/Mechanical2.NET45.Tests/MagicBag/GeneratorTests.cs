using System;
using System.Collections.Generic;
using System.Linq;
using Mechanical.MagicBag;
using NUnit.Framework;

namespace Mechanical.Tests.MagicBag
{
    [TestFixture]
    public class GeneratorTests
    {
        private static readonly Mapping[] SourceMappings = new Mapping[]
        {
            Map<int>.To(() => 5).AsTransient(),
            Map<int>.To(() => 6).AsTransient(),
            Map<float>.To(() => 3.14f).AsTransient(),
            Map<int>.To(() => 7).AsTransient(),
        };

        [Test]
        public void FuncTests()
        {
            var mappings = MappingGenerators.Func.Generate(SourceMappings);
            Assert.AreEqual(4, mappings.Length);

            var m = mappings[0];
            Assert.AreEqual(m.From, typeof(Func<int>));
            Assert.IsInstanceOf<Func<int>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(5, ((Func<int>)m.Get(MagicBagTests.EmptyBag))());

            m = mappings[1];
            Assert.AreEqual(m.From, typeof(Func<int>));
            Assert.IsInstanceOf<Func<int>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(6, ((Func<int>)m.Get(MagicBagTests.EmptyBag))());

            m = mappings[2];
            Assert.AreEqual(m.From, typeof(Func<float>));
            Assert.IsInstanceOf<Func<float>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(3.14f, ((Func<float>)m.Get(MagicBagTests.EmptyBag))());

            m = mappings[3];
            Assert.AreEqual(m.From, typeof(Func<int>));
            Assert.IsInstanceOf<Func<int>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(7, ((Func<int>)m.Get(MagicBagTests.EmptyBag))());
        }

        [Test]
        public void LazyTests()
        {
            var mappings = MappingGenerators.Lazy.Generate(SourceMappings);
            Assert.AreEqual(4, mappings.Length);

            var m = mappings[0];
            Assert.AreEqual(m.From, typeof(Lazy<int>));
            Assert.IsInstanceOf<Lazy<int>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(5, ((Lazy<int>)m.Get(MagicBagTests.EmptyBag)).Value);

            m = mappings[1];
            Assert.AreEqual(m.From, typeof(Lazy<int>));
            Assert.IsInstanceOf<Lazy<int>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(6, ((Lazy<int>)m.Get(MagicBagTests.EmptyBag)).Value);

            m = mappings[2];
            Assert.AreEqual(m.From, typeof(Lazy<float>));
            Assert.IsInstanceOf<Lazy<float>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(3.14f, ((Lazy<float>)m.Get(MagicBagTests.EmptyBag)).Value);

            m = mappings[3];
            Assert.AreEqual(m.From, typeof(Lazy<int>));
            Assert.IsInstanceOf<Lazy<int>>(m.Get(MagicBagTests.EmptyBag));
            Assert.AreEqual(7, ((Lazy<int>)m.Get(MagicBagTests.EmptyBag)).Value);
        }

        [Test]
        public void IEnumerableTests()
        {
            var mappings = MappingGenerators.IEnumerable.Generate(SourceMappings);
            Assert.AreEqual(2, mappings.Length);

            var m = mappings[0];
            Assert.AreEqual(m.From, typeof(IEnumerable<int>));
            Assert.IsInstanceOf<IEnumerable<int>>(m.Get(MagicBagTests.EmptyBag));
            var arr0 = ((IEnumerable<int>)m.Get(MagicBagTests.EmptyBag)).ToArray();
            Assert.AreEqual(3, arr0.Length);
            Assert.AreEqual(5, arr0[0]);
            Assert.AreEqual(6, arr0[1]);
            Assert.AreEqual(7, arr0[2]);

            m = mappings[1];
            Assert.AreEqual(m.From, typeof(IEnumerable<float>));
            Assert.IsInstanceOf<IEnumerable<float>>(m.Get(MagicBagTests.EmptyBag));
            var arr1 = ((IEnumerable<float>)m.Get(MagicBagTests.EmptyBag)).ToArray();
            Assert.AreEqual(1, arr1.Length);
            Assert.AreEqual(3.14f, arr1[0]);
        }

        [Test]
        public void IListTests()
        {
            var mappings = MappingGenerators.IList.Generate(SourceMappings);
            Assert.AreEqual(2, mappings.Length);

            var m = mappings[0];
            Assert.AreEqual(m.From, typeof(IList<int>));
            Assert.IsInstanceOf<IList<int>>(m.Get(MagicBagTests.EmptyBag));
            var arr0 = ((IEnumerable<int>)m.Get(MagicBagTests.EmptyBag)).ToArray();
            Assert.AreEqual(3, arr0.Length);
            Assert.AreEqual(5, arr0[0]);
            Assert.AreEqual(6, arr0[1]);
            Assert.AreEqual(7, arr0[2]);

            m = mappings[1];
            Assert.AreEqual(m.From, typeof(IList<float>));
            Assert.IsInstanceOf<IList<float>>(m.Get(MagicBagTests.EmptyBag));
            var arr1 = ((IEnumerable<float>)m.Get(MagicBagTests.EmptyBag)).ToArray();
            Assert.AreEqual(1, arr1.Length);
            Assert.AreEqual(3.14f, arr1[0]);
        }

        [Test]
        public void ArrayTests()
        {
            var mappings = MappingGenerators.Array.Generate(SourceMappings);
            Assert.AreEqual(2, mappings.Length);

            var m = mappings[0];
            Assert.AreEqual(m.From, typeof(int[]));
            Assert.IsInstanceOf<int[]>(m.Get(MagicBagTests.EmptyBag));
            var arr0 = (int[])m.Get(MagicBagTests.EmptyBag);
            Assert.AreEqual(3, arr0.Length);
            Assert.AreEqual(5, arr0[0]);
            Assert.AreEqual(6, arr0[1]);
            Assert.AreEqual(7, arr0[2]);

            m = mappings[1];
            Assert.AreEqual(m.From, typeof(float[]));
            Assert.IsInstanceOf<float[]>(m.Get(MagicBagTests.EmptyBag));
            var arr1 = (float[])m.Get(MagicBagTests.EmptyBag);
            Assert.AreEqual(1, arr1.Length);
            Assert.AreEqual(3.14f, arr1[0]);
        }
    }
}
