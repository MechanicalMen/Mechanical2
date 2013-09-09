using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mechanical.Conditions;
using NUnit.Framework;

namespace Mechanical.Tests.Conditions
{
    [TestFixture]
    public class ComparisonConditionTests
    {
        #region Comparable, Comparer

        public class Comparable : IComparable<Comparable>
        {
            public int Value { get; set; }

            public int CompareTo( Comparable other )
            {
                return this.Value.CompareTo(other.Value);
            }
        }

        public class Comparer : IComparer<Comparable>
        {
            public static readonly Comparer Default = new Comparer();

            public int Compare( Comparable x, Comparable y )
            {
                return x.Value.CompareTo(y.Value);
            }
        }

        #endregion

        #region Less

        [Test]
        public static void LessTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)6).Less((sbyte)5));
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)5).Less((sbyte)5));
            Ensure.That((sbyte)5).Less((sbyte)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)6).Less((byte)5));
            Assert.Throws<ArgumentException>(() => Ensure.That((byte)5).Less((byte)5));
            Ensure.That((byte)5).Less((byte)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((short)6).Less((short)5));
            Assert.Throws<ArgumentException>(() => Ensure.That((short)5).Less((short)5));
            Ensure.That((short)5).Less((short)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)6).Less((ushort)5));
            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)5).Less((ushort)5));
            Ensure.That((ushort)5).Less((ushort)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((int)6).Less((int)5));
            Assert.Throws<ArgumentException>(() => Ensure.That((int)5).Less((int)5));
            Ensure.That((int)5).Less((int)6);

            Assert.Throws<ArgumentException>(() => Ensure.That(6u).Less(5u));
            Assert.Throws<ArgumentException>(() => Ensure.That(5u).Less(5u));
            Ensure.That(5u).Less(6u);

            Assert.Throws<ArgumentException>(() => Ensure.That(6L).Less(5L));
            Assert.Throws<ArgumentException>(() => Ensure.That(5L).Less(5L));
            Ensure.That(5L).Less(6L);

            Assert.Throws<ArgumentException>(() => Ensure.That(6UL).Less(5UL));
            Assert.Throws<ArgumentException>(() => Ensure.That(5UL).Less(5UL));
            Ensure.That(5UL).Less(6UL);

            Assert.Throws<ArgumentException>(() => Ensure.That(6f).Less(5f));
            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Less(5f));
            Ensure.That(5f).Less(6f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5.11f).Less(5f, maxError: 0.1f));
            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Less(5.1f, maxError: 0.1f));
            Ensure.That(5f).Less(5.11f, maxError: 0.1f);

            Assert.Throws<ArgumentException>(() => Ensure.That(6d).Less(5d));
            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Less(5d));
            Ensure.That(5d).Less(6d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5.11d).Less(5d, maxError: 0.1d));
            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Less(5.1d, maxError: 0.1d));
            Ensure.That(5d).Less(5.11d, maxError: 0.1d);

            Assert.Throws<ArgumentException>(() => Ensure.That(6m).Less(5m));
            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Less(5m));
            Ensure.That(5m).Less(6m);

            Assert.Throws<ArgumentException>(() => Ensure.That(5.11m).Less(5m, maxError: 0.1m));
            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Less(5.1m, maxError: 0.1m));
            Ensure.That(5m).Less(5.11m, maxError: 0.1m);

            var comparable5 = new Comparable() { Value = 5 };
            var comparable5b = new Comparable() { Value = 5 };
            var comparable6 = new Comparable() { Value = 6 };
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable6).Less(comparable5));
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).Less(comparable5b));
            Ensure.That(comparable5).Less(comparable6);

            Assert.Throws<ArgumentException>(() => Ensure.That(comparable6).Less(comparable5, Comparer.Default));
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).Less(comparable5b, Comparer.Default));
            Ensure.That(comparable5).Less(comparable6, Comparer.Default);
        }

        #endregion

        #region LessOrEqual

        [Test]
        public static void LessOrEqualTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)6).LessOrEqual((sbyte)5));
            Ensure.That((sbyte)5).LessOrEqual((sbyte)5);
            Ensure.That((sbyte)5).LessOrEqual((sbyte)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)6).LessOrEqual((byte)5));
            Ensure.That((byte)5).LessOrEqual((byte)5);
            Ensure.That((byte)5).LessOrEqual((byte)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((short)6).LessOrEqual((short)5));
            Ensure.That((short)5).LessOrEqual((short)5);
            Ensure.That((short)5).LessOrEqual((short)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)6).LessOrEqual((ushort)5));
            Ensure.That((ushort)5).LessOrEqual((ushort)5);
            Ensure.That((ushort)5).LessOrEqual((ushort)6);

            Assert.Throws<ArgumentException>(() => Ensure.That((int)6).LessOrEqual((int)5));
            Ensure.That((int)5).LessOrEqual((int)5);
            Ensure.That((int)5).LessOrEqual((int)6);

            Assert.Throws<ArgumentException>(() => Ensure.That(6u).LessOrEqual(5u));
            Ensure.That(5u).LessOrEqual(5u);
            Ensure.That(5u).LessOrEqual(6u);

            Assert.Throws<ArgumentException>(() => Ensure.That(6L).LessOrEqual(5L));
            Ensure.That(5L).LessOrEqual(5L);
            Ensure.That(5L).LessOrEqual(6L);

            Assert.Throws<ArgumentException>(() => Ensure.That(6UL).LessOrEqual(5UL));
            Ensure.That(5UL).LessOrEqual(5UL);
            Ensure.That(5UL).LessOrEqual(6UL);

            Assert.Throws<ArgumentException>(() => Ensure.That(6f).LessOrEqual(5f));
            Ensure.That(5f).LessOrEqual(5f);
            Ensure.That(5f).LessOrEqual(6f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5.11f).LessOrEqual(5f, maxError: 0.1f));
            Ensure.That(5f).LessOrEqual(5.1f, maxError: 0.1f);
            Ensure.That(5f).LessOrEqual(5.11f, maxError: 0.1f);

            Assert.Throws<ArgumentException>(() => Ensure.That(6d).LessOrEqual(5d));
            Ensure.That(5d).LessOrEqual(5d);
            Ensure.That(5d).LessOrEqual(6d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5.11d).LessOrEqual(5d, maxError: 0.1d));
            Ensure.That(5d).LessOrEqual(5.1d, maxError: 0.1d);
            Ensure.That(5d).LessOrEqual(5.11d, maxError: 0.1d);

            Assert.Throws<ArgumentException>(() => Ensure.That(6m).LessOrEqual(5m));
            Ensure.That(5m).LessOrEqual(5m);
            Ensure.That(5m).LessOrEqual(6m);

            Assert.Throws<ArgumentException>(() => Ensure.That(5.11m).LessOrEqual(5m, maxError: 0.1m));
            Ensure.That(5m).LessOrEqual(5.1m, maxError: 0.1m);
            Ensure.That(5m).LessOrEqual(5.11m, maxError: 0.1m);

            var comparable5 = new Comparable() { Value = 5 };
            var comparable5b = new Comparable() { Value = 5 };
            var comparable6 = new Comparable() { Value = 6 };
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable6).LessOrEqual(comparable5));
            Ensure.That(comparable5).LessOrEqual(comparable5b);
            Ensure.That(comparable5).LessOrEqual(comparable6);

            Assert.Throws<ArgumentException>(() => Ensure.That(comparable6).LessOrEqual(comparable5, Comparer.Default));
            Ensure.That(comparable5).LessOrEqual(comparable5b, Comparer.Default);
            Ensure.That(comparable5).LessOrEqual(comparable6, Comparer.Default);
        }

        #endregion

        #region Greater

        [Test]
        public static void GreaterTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)5).Greater((sbyte)6));
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)5).Greater((sbyte)5));
            Ensure.That((sbyte)6).Greater((sbyte)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)5).Greater((byte)6));
            Assert.Throws<ArgumentException>(() => Ensure.That((byte)5).Greater((byte)5));
            Ensure.That((byte)6).Greater((byte)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((short)5).Greater((short)6));
            Assert.Throws<ArgumentException>(() => Ensure.That((short)5).Greater((short)5));
            Ensure.That((short)6).Greater((short)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)5).Greater((ushort)6));
            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)5).Greater((ushort)5));
            Ensure.That((ushort)6).Greater((ushort)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((int)5).Greater((int)6));
            Assert.Throws<ArgumentException>(() => Ensure.That((int)5).Greater((int)5));
            Ensure.That((int)6).Greater((int)5);

            Assert.Throws<ArgumentException>(() => Ensure.That(5u).Greater(6u));
            Assert.Throws<ArgumentException>(() => Ensure.That(5u).Greater(5u));
            Ensure.That(6u).Greater(5u);

            Assert.Throws<ArgumentException>(() => Ensure.That(5L).Greater(6L));
            Assert.Throws<ArgumentException>(() => Ensure.That(5L).Greater(5L));
            Ensure.That(6L).Greater(5L);

            Assert.Throws<ArgumentException>(() => Ensure.That(5UL).Greater(6UL));
            Assert.Throws<ArgumentException>(() => Ensure.That(5UL).Greater(5UL));
            Ensure.That(6UL).Greater(5UL);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Greater(6f));
            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Greater(5f));
            Ensure.That(6f).Greater(5f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Greater(5.11f, maxError: 0.1f));
            Assert.Throws<ArgumentException>(() => Ensure.That(5f).Greater(5.1f, maxError: 0.1f));
            Ensure.That(5.11f).Greater(5f, maxError: 0.1f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Greater(6d));
            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Greater(5d));
            Ensure.That(6d).Greater(5d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Greater(5.11d, maxError: 0.1d));
            Assert.Throws<ArgumentException>(() => Ensure.That(5d).Greater(5.1d, maxError: 0.1d));
            Ensure.That(5.11d).Greater(5d, maxError: 0.1d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Greater(6m));
            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Greater(5m));
            Ensure.That(6m).Greater(5m);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Greater(5.11m, maxError: 0.1m));
            Assert.Throws<ArgumentException>(() => Ensure.That(5m).Greater(5.1m, maxError: 0.1m));
            Ensure.That(5.11m).Greater(5m, maxError: 0.1m);

            var comparable5 = new Comparable() { Value = 5 };
            var comparable5b = new Comparable() { Value = 5 };
            var comparable6 = new Comparable() { Value = 6 };
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).Greater(comparable6));
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).Greater(comparable5b));
            Ensure.That(comparable6).Greater(comparable5);

            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).Greater(comparable6, Comparer.Default));
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).Greater(comparable5b, Comparer.Default));
            Ensure.That(comparable6).Greater(comparable5, Comparer.Default);
        }

        #endregion

        #region GreaterOrEqual

        [Test]
        public static void GreaterOrEqualTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)5).GreaterOrEqual((sbyte)6));
            Ensure.That((sbyte)5).GreaterOrEqual((sbyte)5);
            Ensure.That((sbyte)6).GreaterOrEqual((sbyte)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((byte)5).GreaterOrEqual((byte)6));
            Ensure.That((byte)5).GreaterOrEqual((byte)5);
            Ensure.That((byte)6).GreaterOrEqual((byte)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((short)5).GreaterOrEqual((short)6));
            Ensure.That((short)5).GreaterOrEqual((short)5);
            Ensure.That((short)6).GreaterOrEqual((short)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)5).GreaterOrEqual((ushort)6));
            Ensure.That((ushort)5).GreaterOrEqual((ushort)5);
            Ensure.That((ushort)6).GreaterOrEqual((ushort)5);

            Assert.Throws<ArgumentException>(() => Ensure.That((int)5).GreaterOrEqual((int)6));
            Ensure.That((int)5).GreaterOrEqual((int)5);
            Ensure.That((int)6).GreaterOrEqual((int)5);

            Assert.Throws<ArgumentException>(() => Ensure.That(5u).GreaterOrEqual(6u));
            Ensure.That(5u).GreaterOrEqual(5u);
            Ensure.That(6u).GreaterOrEqual(5u);

            Assert.Throws<ArgumentException>(() => Ensure.That(5L).GreaterOrEqual(6L));
            Ensure.That(5L).GreaterOrEqual(5L);
            Ensure.That(6L).GreaterOrEqual(5L);

            Assert.Throws<ArgumentException>(() => Ensure.That(5UL).GreaterOrEqual(6UL));
            Ensure.That(5UL).GreaterOrEqual(5UL);
            Ensure.That(6UL).GreaterOrEqual(5UL);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).GreaterOrEqual(6f));
            Ensure.That(5f).GreaterOrEqual(5f);
            Ensure.That(6f).GreaterOrEqual(5f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5f).GreaterOrEqual(5.11f, maxError: 0.1f));
            Ensure.That(5f).GreaterOrEqual(5.1f, maxError: 0.1f);
            Ensure.That(5.11f).GreaterOrEqual(5f, maxError: 0.1f);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).GreaterOrEqual(6d));
            Ensure.That(5d).GreaterOrEqual(5d);
            Ensure.That(6d).GreaterOrEqual(5d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5d).GreaterOrEqual(5.11d, maxError: 0.1d));
            Ensure.That(5d).GreaterOrEqual(5.1d, maxError: 0.1d);
            Ensure.That(5.11d).GreaterOrEqual(5d, maxError: 0.1d);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).GreaterOrEqual(6m));
            Ensure.That(5m).GreaterOrEqual(5m);
            Ensure.That(6m).GreaterOrEqual(5m);

            Assert.Throws<ArgumentException>(() => Ensure.That(5m).GreaterOrEqual(5.11m, maxError: 0.1m));
            Ensure.That(5m).GreaterOrEqual(5.1m, maxError: 0.1m);
            Ensure.That(5.11m).GreaterOrEqual(5m, maxError: 0.1m);

            var comparable5 = new Comparable() { Value = 5 };
            var comparable5b = new Comparable() { Value = 5 };
            var comparable6 = new Comparable() { Value = 6 };
            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).GreaterOrEqual(comparable6));
            Ensure.That(comparable5).GreaterOrEqual(comparable5b);
            Ensure.That(comparable6).GreaterOrEqual(comparable5);

            Assert.Throws<ArgumentException>(() => Ensure.That(comparable5).GreaterOrEqual(comparable6, Comparer.Default));
            Ensure.That(comparable5).GreaterOrEqual(comparable5b, Comparer.Default);
            Ensure.That(comparable6).GreaterOrEqual(comparable5, Comparer.Default);
        }

        #endregion


        #region IsNegative

        [Test]
        public static void IsNegativeTest()
        {
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)1).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)0).IsNegative());
            Ensure.That((sbyte)-1).IsNegative();

            //// byte

            Assert.Throws<ArgumentException>(() => Ensure.That((short)1).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That((short)0).IsNegative());
            Ensure.That((short)-1).IsNegative();

            //// ushort

            Assert.Throws<ArgumentException>(() => Ensure.That((int)1).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That((int)0).IsNegative());
            Ensure.That((int)-1).IsNegative();

            //// uint

            Assert.Throws<ArgumentException>(() => Ensure.That(1L).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That(0L).IsNegative());
            Ensure.That(-1L).IsNegative();

            //// ulong

            Assert.Throws<ArgumentException>(() => Ensure.That(1f).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That(0f).IsNegative());
            Ensure.That(-1f).IsNegative();

            Assert.Throws<ArgumentException>(() => Ensure.That(0.11f).IsNegative(maxError: 0.1f));
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.1f).IsNegative(maxError: 0.1f));
            Ensure.That(-0.11f).IsNegative(maxError: 0.1f);

            Assert.Throws<ArgumentException>(() => Ensure.That(1d).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That(0d).IsNegative());
            Ensure.That(-1d).IsNegative();

            Assert.Throws<ArgumentException>(() => Ensure.That(0.11d).IsNegative(maxError: 0.1d));
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.1d).IsNegative(maxError: 0.1d));
            Ensure.That(-0.11d).IsNegative(maxError: 0.1d);

            Assert.Throws<ArgumentException>(() => Ensure.That(1m).IsNegative());
            Assert.Throws<ArgumentException>(() => Ensure.That(0m).IsNegative());
            Ensure.That(-1m).IsNegative();

            Assert.Throws<ArgumentException>(() => Ensure.That(0.11m).IsNegative(maxError: 0.1m));
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.1m).IsNegative(maxError: 0.1m));
            Ensure.That(-0.11m).IsNegative(maxError: 0.1m);
        }

        #endregion

        #region NonNegative

        [Test]
        public static void NonNegativeTest()
        {
            Ensure.That((sbyte)1).NonNegative();
            Ensure.That((sbyte)0).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)-1).NonNegative());

            //// byte

            Ensure.That((short)1).NonNegative();
            Ensure.That((short)0).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That((short)-1).NonNegative());

            //// ushort

            Ensure.That((int)1).NonNegative();
            Ensure.That((int)0).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That((int)-1).NonNegative());

            //// uint

            Ensure.That(1L).NonNegative();
            Ensure.That(0L).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That(-1L).NonNegative());

            //// ulong

            Ensure.That(1f).NonNegative();
            Ensure.That(0f).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That(-1f).NonNegative());

            Ensure.That(0.11f).NonNegative(maxError: 0.1f);
            Ensure.That(-0.1f).NonNegative(maxError: 0.1f);
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.11f).NonNegative(maxError: 0.1f));

            Ensure.That(1d).NonNegative();
            Ensure.That(0d).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That(-1d).NonNegative());

            Ensure.That(0.11d).NonNegative(maxError: 0.1d);
            Ensure.That(-0.1d).NonNegative(maxError: 0.1d);
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.11d).NonNegative(maxError: 0.1d));

            Ensure.That(1m).NonNegative();
            Ensure.That(0m).NonNegative();
            Assert.Throws<ArgumentException>(() => Ensure.That(-1m).NonNegative());

            Ensure.That(0.11m).NonNegative(maxError: 0.1m);
            Ensure.That(-0.1m).NonNegative(maxError: 0.1m);
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.11m).NonNegative(maxError: 0.1m));
        }

        #endregion

        #region IsPositive

        [Test]
        public static void IsPositiveTest()
        {
            Ensure.That((sbyte)1).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)0).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That((sbyte)-1).IsPositive());

            Ensure.That((byte)1).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That((byte)0).IsPositive());
            ////Assert.Throws<ArgumentException>(() => Ensure.That((byte)-1).IsPositive());

            Ensure.That((short)1).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That((short)0).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That((short)-1).IsPositive());

            Ensure.That((ushort)1).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That((ushort)0).IsPositive());
            ////Assert.Throws<ArgumentException>(() => Ensure.That((ushort)-1).IsPositive());

            Ensure.That((int)1).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That((int)0).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That((int)-1).IsPositive());

            Ensure.That((uint)1).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That((uint)0).IsPositive());
            ////Assert.Throws<ArgumentException>(() => Ensure.That((uint)-1).IsPositive());

            Ensure.That(1L).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That(0L).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That(-1L).IsPositive());

            Ensure.That(1UL).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That(0UL).IsPositive());
            ////Assert.Throws<ArgumentException>(() => Ensure.That(-1UL).IsPositive());

            Ensure.That(1f).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That(0f).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That(-1f).IsPositive());

            Ensure.That(0.11f).IsPositive(maxError: 0.1f);
            Assert.Throws<ArgumentException>(() => Ensure.That(0.1f).IsPositive(maxError: 0.1f));
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.11f).IsPositive(maxError: 0.1f));

            Ensure.That(1d).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That(0d).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That(-1d).IsPositive());

            Ensure.That(0.11d).IsPositive(maxError: 0.1d);
            Assert.Throws<ArgumentException>(() => Ensure.That(0.1d).IsPositive(maxError: 0.1d));
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.11d).IsPositive(maxError: 0.1d));

            Ensure.That(1m).IsPositive();
            Assert.Throws<ArgumentException>(() => Ensure.That(0m).IsPositive());
            Assert.Throws<ArgumentException>(() => Ensure.That(-1m).IsPositive());

            Ensure.That(0.11m).IsPositive(maxError: 0.1m);
            Assert.Throws<ArgumentException>(() => Ensure.That(0.1m).IsPositive(maxError: 0.1m));
            Assert.Throws<ArgumentException>(() => Ensure.That(-0.11m).IsPositive(maxError: 0.1m));
        }

        #endregion

        #region InRange

        [Test]
        public static void InRangeTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((sbyte)0).InRange((sbyte)1, (sbyte)3));
            Ensure.That((sbyte)1).InRange((sbyte)1, (sbyte)3);
            Ensure.That((sbyte)2).InRange((sbyte)1, (sbyte)3);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((sbyte)3).InRange((sbyte)1, (sbyte)3));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((byte)0).InRange((byte)1, (byte)3));
            Ensure.That((byte)1).InRange((byte)1, (byte)3);
            Ensure.That((byte)2).InRange((byte)1, (byte)3);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((byte)3).InRange((byte)1, (byte)3));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((short)0).InRange((short)1, (short)3));
            Ensure.That((short)1).InRange((short)1, (short)3);
            Ensure.That((short)2).InRange((short)1, (short)3);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((short)3).InRange((short)1, (short)3));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((ushort)0).InRange((ushort)1, (ushort)3));
            Ensure.That((ushort)1).InRange((ushort)1, (ushort)3);
            Ensure.That((ushort)2).InRange((ushort)1, (ushort)3);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((ushort)3).InRange((ushort)1, (ushort)3));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((int)0).InRange((int)1, (int)3));
            Ensure.That((int)1).InRange((int)1, (int)3);
            Ensure.That((int)2).InRange((int)1, (int)3);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That((int)3).InRange((int)1, (int)3));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0u).InRange(1u, 3u));
            Ensure.That(1u).InRange(1u, 3u);
            Ensure.That(2u).InRange(1u, 3u);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(3u).InRange(1u, 3u));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0L).InRange(1L, 3L));
            Ensure.That(1L).InRange(1L, 3L);
            Ensure.That(2L).InRange(1L, 3L);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(3L).InRange(1L, 3L));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0UL).InRange(1UL, 3UL));
            Ensure.That(1UL).InRange(1UL, 3UL);
            Ensure.That(2UL).InRange(1UL, 3UL);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(3UL).InRange(1UL, 3UL));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0f).InRange(1f, 3f));
            Ensure.That(1f).InRange(1f, 3f);
            Ensure.That(2f).InRange(1f, 3f);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(3f).InRange(1f, 3f));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0.89f).InRange(1f, 3f, maxError: 0.1f));
            Ensure.That(0.91f).InRange(1f, 3f, maxError: 0.1f);
            Ensure.That(2.89f).InRange(1f, 3f, maxError: 0.1f);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(2.91f).InRange(1f, 3f, maxError: 0.1f));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0d).InRange(1d, 3d));
            Ensure.That(1d).InRange(1d, 3d);
            Ensure.That(2d).InRange(1d, 3d);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(3d).InRange(1d, 3d));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0.89d).InRange(1d, 3d, maxError: 0.1d));
            Ensure.That(0.91d).InRange(1d, 3d, maxError: 0.1d);
            Ensure.That(2.89d).InRange(1d, 3d, maxError: 0.1d);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(2.91d).InRange(1d, 3d, maxError: 0.1d));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0m).InRange(1m, 3m));
            Ensure.That(1m).InRange(1m, 3m);
            Ensure.That(2m).InRange(1m, 3m);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(3m).InRange(1m, 3m));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0.89m).InRange(1m, 3m, maxError: 0.1m));
            Ensure.That(0.91m).InRange(1m, 3m, maxError: 0.1m);
            Ensure.That(2.89m).InRange(1m, 3m, maxError: 0.1m);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(2.91m).InRange(1m, 3m, maxError: 0.1m));

            var comparable0 = new Comparable() { Value = 0 };
            var comparable1 = new Comparable() { Value = 1 };
            var comparable2 = new Comparable() { Value = 2 };
            var comparable3 = new Comparable() { Value = 3 };
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(comparable0).InRange(comparable1, comparable3));
            Ensure.That(comparable1).InRange(comparable1, comparable3);
            Ensure.That(comparable2).InRange(comparable1, comparable3);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(comparable3).InRange(comparable1, comparable3));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(comparable0).InRange(comparable1, comparable3, Comparer.Default));
            Ensure.That(comparable1).InRange(comparable1, comparable3, Comparer.Default);
            Ensure.That(comparable2).InRange(comparable1, comparable3, Comparer.Default);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(comparable3).InRange(comparable1, comparable3, Comparer.Default));
        }

        #endregion

        #region IsIndex

        [Test]
        public static void IsIndexTest()
        {
            ICollection<int> emptyCollection = new List<int>();
            IReadOnlyCollection<int> emptyReadOnlyCollection = new ReadOnlyCollection<int>(new List<int>());
            var emptyArray = new int[0];

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0).IsIndex(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(0));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(emptyCollection));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0).IsIndex(emptyCollection));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(emptyCollection));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(emptyReadOnlyCollection));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0).IsIndex(emptyReadOnlyCollection));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(emptyReadOnlyCollection));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(emptyArray));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(0).IsIndex(emptyArray));
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(emptyArray));


            var array = new int[] { 5 };
            ICollection<int> collection = new List<int>(array);
            IReadOnlyCollection<int> readOnlyCollection = new ReadOnlyCollection<int>(new List<int>(array));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(1));
            Ensure.That(0).IsIndex(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(1));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(collection));
            Ensure.That(0).IsIndex(collection);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(collection));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(readOnlyCollection));
            Ensure.That(0).IsIndex(readOnlyCollection);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(readOnlyCollection));

            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(-1).IsIndex(array));
            Ensure.That(0).IsIndex(array);
            Assert.Throws<ArgumentOutOfRangeException>(() => Ensure.That(1).IsIndex(array));
        }

        #endregion
    }
}
