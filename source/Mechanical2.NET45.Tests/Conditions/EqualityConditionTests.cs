using System;
using System.Collections.Generic;
using Mechanical.Conditions;
using NUnit.Framework;

namespace Mechanical.Tests.Conditions
{
    [TestFixture]
    public class EqualityConditionTests
    {
        #region Equatable, EqualityComparer

        public class Equatable : IEquatable<Equatable>
        {
            public int Value { get; set; }

            public bool Equals( Equatable other )
            {
                return this.Value == other.Value;
            }
        }

        public class EqualityComparer : IEqualityComparer<Equatable>
        {
            public static readonly EqualityComparer Default = new EqualityComparer();

            public bool Equals( Equatable x, Equatable y )
            {
                return x.Value.Equals(y.Value);
            }

            public int GetHashCode( Equatable obj )
            {
                return obj.GetHashCode();
            }
        }

        #endregion

        #region Equal

        [Test]
        public void EqualTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)5).Equal((sbyte)6));
            Ensure.That((sbyte)5).Equal((sbyte)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)5).Equal((byte)6));
            Ensure.That((byte)5).Equal((byte)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((short)5).Equal((short)6));
            Ensure.That((short)5).Equal((short)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)5).Equal((ushort)6));
            Ensure.That((ushort)5).Equal((ushort)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((int)5).Equal((int)6));
            Ensure.That((int)5).Equal((int)5);

            Assert.Throws<ArgumentException>(() => Ensure.That(5u).Equal(6u));
            Ensure.That(5u).Equal(5u);

            Assert.Throws<ArgumentException>(() => Ensure.That(5L).Equal(6L));
            Ensure.That(5L).Equal(5L);

            Assert.Throws<ArgumentException>(() => Ensure.That(5UL).Equal(6UL));
            Ensure.That(5UL).Equal(5UL);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Equal(6f));
            Ensure.That(5f).Equal(5f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Equal(5.1f, maxError: 0.01f));
            Ensure.That(5f).Equal(5.1f, maxError: 0.1f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Equal(6d));
            Ensure.That(5d).Equal(5d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Equal(5.1d, maxError: 0.01d));
            Ensure.That(5d).Equal(5.1d, maxError: 0.1d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Equal(6m));
            Ensure.That(5m).Equal(5m);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Equal(5.1m, maxError: 0.01m));
            Ensure.That(5m).Equal(5.1m, maxError: 0.1m);

            Assert.Throws<ArgumentException>(() => Ensure.That("5").Equal("6"));
            Ensure.That("5").Equal("5");

            Assert.Throws<ArgumentException>(() => Ensure.That("5").Equal("6", StringComparison.Ordinal));
            Ensure.That("5").Equal("5", StringComparison.Ordinal);

            var equatable5 = new Equatable() { Value = 5 };
            var equatable5b = new Equatable() { Value = 5 };
            var equatable6 = new Equatable() { Value = 6 };
            Assert.Throws<ArgumentException>(() => Ensure.That(equatable5).Equal(equatable6));
            Ensure.That(equatable5).Equal(equatable5b);

            Assert.Throws<ArgumentException>(() => Ensure.That(equatable5).Equal(equatable6, EqualityComparer.Default));
            Ensure.That(equatable5).Equal(equatable5b, EqualityComparer.Default);
        }

        #endregion

        #region NotEqual

        [Test]
        public void NotEqualTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)5).NotEqual((sbyte)5));
            Ensure.That((sbyte)5).NotEqual((sbyte)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)5).NotEqual((byte)5));
            Ensure.That((byte)5).NotEqual((byte)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((short)5).NotEqual((short)5));
            Ensure.That((short)5).NotEqual((short)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)5).NotEqual((ushort)5));
            Ensure.That((ushort)5).NotEqual((ushort)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((int)5).NotEqual((int)5));
            Ensure.That((int)5).NotEqual((int)6);

            Assert.Throws<ArgumentException>(() => Ensure.That(5u).NotEqual(5u));
            Ensure.That(5u).NotEqual(6u);

            Assert.Throws<ArgumentException>(() => Ensure.That(5L).NotEqual(5L));
            Ensure.That(5L).NotEqual(6L);

            Assert.Throws<ArgumentException>(() => Ensure.That(5UL).NotEqual(5UL));
            Ensure.That(5UL).NotEqual(6UL);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).NotEqual(5f));
            Ensure.That(5f).NotEqual(6f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).NotEqual(5.1f, maxError: 0.1f));
            Ensure.That(5f).NotEqual(5.1f, maxError: 0.01f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).NotEqual(5d));
            Ensure.That(5d).NotEqual(6d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).NotEqual(5.1d, maxError: 0.1d));
            Ensure.That(5d).NotEqual(5.1d, maxError: 0.01d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).NotEqual(5m));
            Ensure.That(5m).NotEqual(6m);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).NotEqual(5.1m, maxError: 0.1m));
            Ensure.That(5m).NotEqual(5.1m, maxError: 0.01m);

            Assert.Throws<ArgumentException>(() => Ensure.That("5").NotEqual("5"));
            Ensure.That("5").NotEqual("6");

            Assert.Throws<ArgumentException>(() => Ensure.That("5").NotEqual("5", StringComparison.Ordinal));
            Ensure.That("5").NotEqual("6", StringComparison.Ordinal);

            var equatable5 = new Equatable() { Value = 5 };
            var equatable5b = new Equatable() { Value = 5 };
            var equatable6 = new Equatable() { Value = 6 };
            Assert.Throws<ArgumentException>(() => Ensure.That(equatable5).NotEqual(equatable5b));
            Ensure.That(equatable5).NotEqual(equatable6);

            Assert.Throws<ArgumentException>(() => Ensure.That(equatable5).NotEqual(equatable5b, EqualityComparer.Default));
            Ensure.That(equatable5).NotEqual(equatable6, EqualityComparer.Default);
        }

        #endregion


        #region NonZero

        [Test]
        public void NonZeroTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)0).NonZero());
            Ensure.That((sbyte)1).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)0).NonZero());
            Ensure.That((byte)1).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That((short)0).NonZero());
            Ensure.That((short)1).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)0).NonZero());
            Ensure.That((ushort)1).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That((int)0).NonZero());
            Ensure.That((int)1).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0u).NonZero());
            Ensure.That(1u).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0L).NonZero());
            Ensure.That(1L).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0UL).NonZero());
            Ensure.That(1UL).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0f).NonZero());
            Ensure.That(1f).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0.1f).NonZero(maxError: 0.1f));
            Ensure.That(0.1f).NonZero(maxError: 0.01f);

            Assert.Throws<ArgumentException>(() => Ensure.That(0d).NonZero());
            Ensure.That(1d).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0.1d).NonZero(maxError: 0.1d));
            Ensure.That(0.1d).NonZero(maxError: 0.01d);

            Assert.Throws<ArgumentException>(() => Ensure.That(0m).NonZero());
            Ensure.That(1m).NonZero();

            Assert.Throws<ArgumentException>(() => Ensure.That(0.1m).NonZero(maxError: 0.1m));
            Ensure.That(0.1m).NonZero(maxError: 0.01m);
        }

        #endregion

        #region NotNaN

        [Test]
        public void NotNaNTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That(float.NaN).NotNaN());
            Ensure.That(1f).NotNaN();

            Assert.Throws<ArgumentException>(() => Ensure.That(double.NaN).NotNaN());
            Ensure.That(1d).NotNaN();
        }

        #endregion

        #region NotInfinity

        [Test]
        public void NotInfinityTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That(float.PositiveInfinity).NotInfinity());
            Assert.Throws<ArgumentException>(() => Ensure.That(float.NegativeInfinity).NotInfinity());
            Ensure.That(1f).NotInfinity();

            Assert.Throws<ArgumentException>(() => Ensure.That(double.PositiveInfinity).NotInfinity());
            Assert.Throws<ArgumentException>(() => Ensure.That(double.NegativeInfinity).NotInfinity());
            Ensure.That(1d).NotInfinity();
        }

        #endregion

        #region NotInfinityOrNaN

        [Test]
        public void NotInfinityOrNaNTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That(float.PositiveInfinity).NotInfinityOrNaN());
            Assert.Throws<ArgumentException>(() => Ensure.That(float.NegativeInfinity).NotInfinityOrNaN());
            Assert.Throws<ArgumentException>(() => Ensure.That(float.NaN).NotInfinityOrNaN());
            Ensure.That(1f).NotInfinityOrNaN();

            Assert.Throws<ArgumentException>(() => Ensure.That(double.PositiveInfinity).NotInfinityOrNaN());
            Assert.Throws<ArgumentException>(() => Ensure.That(double.NegativeInfinity).NotInfinityOrNaN());
            Assert.Throws<ArgumentException>(() => Ensure.That(double.NaN).NotInfinityOrNaN());
            Ensure.That(1d).NotInfinityOrNaN();
        }

        #endregion
    }
}
