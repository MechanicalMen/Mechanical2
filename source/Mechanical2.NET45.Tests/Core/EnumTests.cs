using System;
using System.Diagnostics.CodeAnalysis;
using Mechanical.Conditions;
using Mechanical.Core;
using NUnit.Framework;

namespace Mechanical.Tests.Core
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "The enumerations are private.")]
    public class EnumTests
    {
        private enum EmptyEnum
        {
        }

        private enum NoFlags : sbyte
        {
            X = -1,
            Zero = 0,
            A = 1,
            [System.ComponentModel.Description("BBB")]
            B = 2,
            C = 3,
            [System.ComponentModel.Description("bbb")]
            b = B,
            D = 4
        }

        [Flags]
        private enum Flags1 : ushort
        {
            A = 1,
            ////B = 2
            ////C = 4
            D = 8,
            AB = 1 | 2,
            BC = 2 | 4
        }

        [Flags]
        private enum Flags2 : long
        {
            AB = 1 | 2,
            CD = 4 | 8
        }

        [Test]
        public void StaticTests()
        {
            Assert.NotNull(Enum<EmptyEnum>.Names);
            Assert.AreEqual(0, Enum<EmptyEnum>.Names.Count);
            Assert.NotNull(Enum<EmptyEnum>.Values);
            Assert.AreEqual(0, Enum<EmptyEnum>.Values.Count);
            Assert.NotNull(Enum<EmptyEnum>.Descriptions);
            Assert.AreEqual(0, Enum<EmptyEnum>.Descriptions.Count);
            Assert.AreEqual(false, Enum<EmptyEnum>.DefinesFlags);
            Assert.AreEqual(false, Enum<EmptyEnum>.HasZero);
            Assert.Throws<InvalidOperationException>(() => Enum<EmptyEnum>.SimpleFlags.ToString());

            Assert.NotNull(Enum<NoFlags>.Names);
            Assert.AreEqual(7, Enum<NoFlags>.Names.Count);
            Assert.NotNull(Enum<NoFlags>.Values);
            Assert.AreEqual(7, Enum<NoFlags>.Values.Count);
            Assert.NotNull(Enum<NoFlags>.Descriptions);
            Assert.AreEqual(1, Enum<NoFlags>.Descriptions.Count); // because of alias
            Assert.AreEqual(false, Enum<NoFlags>.DefinesFlags);
            Assert.AreEqual(true, Enum<NoFlags>.HasZero);
            Assert.Throws<InvalidOperationException>(() => Enum<NoFlags>.SimpleFlags.ToString());

            Assert.True(Enum<Flags1>.DefinesFlags);
            Assert.NotNull(Enum<Flags1>.SimpleFlags);
            Assert.AreEqual(3, Enum<Flags1>.SimpleFlags.Count);
            Assert.AreEqual(Flags1.A, Enum<Flags1>.SimpleFlags[0]);
            Assert.AreEqual(Flags1.BC, Enum<Flags1>.SimpleFlags[1]);
            Assert.AreEqual(Flags1.D, Enum<Flags1>.SimpleFlags[2]);
            Assert.True(new Enum<Flags1>(Flags1.A).IsSimpleFlag);
            Assert.True(new Enum<Flags1>(Flags1.BC).IsSimpleFlag);
            Assert.True(new Enum<Flags1>(Flags1.D).IsSimpleFlag);
            Assert.False(new Enum<Flags1>(Flags1.AB).IsSimpleFlag);

            Assert.True(Enum<Flags2>.DefinesFlags);
            Assert.NotNull(Enum<Flags2>.SimpleFlags);
            Assert.AreEqual(2, Enum<Flags2>.SimpleFlags.Count);
            Assert.AreEqual(Flags2.AB, Enum<Flags2>.SimpleFlags[0]);
            Assert.AreEqual(Flags2.CD, Enum<Flags2>.SimpleFlags[1]);
            Assert.True(new Enum<Flags2>(Flags2.AB).IsSimpleFlag);
            Assert.True(new Enum<Flags2>(Flags2.CD).IsSimpleFlag);
            Assert.False(Enum<Flags2>.Zero.IsSimpleFlag);
            Assert.False(new Enum<Flags2>((Flags2)long.MaxValue).IsSimpleFlag);
        }

        [Test]
        public void ToStringTests()
        {
            Test.OrdinalEquals(Enum<NoFlags>.Names[0], "X");
            Test.OrdinalEquals(Enum<NoFlags>.Names[1], "Zero");
            Test.OrdinalEquals(Enum<NoFlags>.Names[2], "A");
            Test.OrdinalEquals(Enum<NoFlags>.Names[3], "B");
            Test.OrdinalEquals(Enum<NoFlags>.Names[4], "b");
            Test.OrdinalEquals(Enum<NoFlags>.Names[5], "C");
            Test.OrdinalEquals(Enum<NoFlags>.Names[6], "D");
            Test.OrdinalEquals(Enum<NoFlags>.Values[0].ToString(), "X");
            Test.OrdinalEquals(Enum<NoFlags>.Values[1].ToString(), "Zero");
            Test.OrdinalEquals(Enum<NoFlags>.Values[2].ToString(), "A");
            Test.OrdinalEquals(Enum<NoFlags>.Values[3].ToString(), "b"); //// Aliases can only be distinguished by name!
            Test.OrdinalEquals(Enum<NoFlags>.Values[4].ToString(), "b");
            Test.OrdinalEquals(Enum<NoFlags>.Values[5].ToString(), "C");
            Test.OrdinalEquals(Enum<NoFlags>.Values[6].ToString(), "D");

            Test.OrdinalEquals(Enum<Flags2>.Names[0], "AB");
            Test.OrdinalEquals(Enum<Flags2>.Names[1], "CD");
            Test.OrdinalEquals(Enum<Flags2>.Values[0].ToString(), "AB");
            Test.OrdinalEquals(Enum<Flags2>.Values[1].ToString(), "CD");
            Test.OrdinalEquals(new Enum<Flags2>(Flags2.AB | Flags2.CD).ToString(), "AB, CD");
            Test.OrdinalEquals(new Enum<Flags2>((Flags2)2).ToString(), "2");
        }

        [Test]
        public void ZeroTests()
        {
            Test.OrdinalEquals(Enum<EmptyEnum>.Zero.ToString(), "0");
            Test.OrdinalEquals(Enum<NoFlags>.Zero.ToString(), "Zero");
            Assert.True(new Enum<NoFlags>(NoFlags.Zero).IsZero);
            Assert.AreEqual(0, (sbyte)NoFlags.Zero);
            Assert.AreEqual(NoFlags.Zero, Enum<NoFlags>.Zero);

            Assert.False(Enum<Flags2>.HasZero);
            Assert.False(new Enum<Flags2>(Flags2.AB).IsZero);
            Assert.False(new Enum<Flags2>(Flags2.CD).IsZero);
        }

        [Test]
        public void IsDefined_IsValid()
        {
            Assert.True(new Enum<NoFlags>(NoFlags.X).IsDefined);
            Assert.True(new Enum<NoFlags>(NoFlags.Zero).IsDefined);
            Assert.True(new Enum<NoFlags>(NoFlags.A).IsDefined);
            Assert.True(new Enum<NoFlags>(NoFlags.B).IsDefined);
            Assert.True(new Enum<NoFlags>(NoFlags.C).IsDefined);
            Assert.True(new Enum<NoFlags>(NoFlags.b).IsDefined);
            Assert.True(new Enum<NoFlags>(NoFlags.D).IsDefined);
            Assert.False(new Enum<NoFlags>((NoFlags)sbyte.MaxValue).IsDefined);


            Assert.True(new Enum<Flags1>(Flags1.A).IsDefined);
            Assert.True(new Enum<Flags1>(Flags1.D).IsDefined);
            Assert.True(new Enum<Flags1>(Flags1.AB).IsDefined);
            Assert.True(new Enum<Flags1>(Flags1.BC).IsDefined);
            Assert.False(new Enum<Flags1>((Flags1)0).IsDefined);

            Assert.True(new Enum<Flags1>(Flags1.A).IsValid);
            Assert.True(new Enum<Flags1>(Flags1.D).IsValid);
            Assert.True(new Enum<Flags1>(Flags1.AB).IsValid);
            Assert.True(new Enum<Flags1>(Flags1.BC).IsValid);
            Assert.True(new Enum<Flags1>(Flags1.A | Flags1.D).IsValid);
            Assert.False(new Enum<Flags1>((Flags1)0).IsValid);
            Assert.False(new Enum<Flags1>((Flags1)ushort.MaxValue).IsValid);
            Assert.False(new Enum<Flags1>((Flags1)2).IsValid);
        }

        [Test]
        public void IsDefined_IsValid_Conditions()
        {
            // NOTE: we'll can a little lazy here, since we know the conditions are only a thin wrapper around the properties
            Ensure.That(NoFlags.X).IsDefined();
            Ensure.That(NoFlags.Zero).IsDefined();
            Ensure.That(NoFlags.A).IsDefined();
            Assert.Throws<ArgumentException>(() => Ensure.That((NoFlags)sbyte.MaxValue).IsDefined());

            Ensure.That(new Enum<Flags1>(Flags1.BC)).IsDefined();
            Assert.Throws<ArgumentException>(() => Ensure.That(new Enum<Flags1>((Flags1)0)).IsDefined());

            Ensure.That(Flags1.BC).IsValid();
            Ensure.That(Flags1.A | Flags1.D).IsValid();
            Assert.Throws<ArgumentException>(() => Ensure.That((Flags1)0).IsValid());
        }

        [Test]
        public void DescriptionTests()
        {
            Assert.False(new Enum<NoFlags>(NoFlags.X).HasDescription);
            Assert.False(new Enum<NoFlags>(NoFlags.Zero).HasDescription);
            Assert.False(new Enum<NoFlags>(NoFlags.A).HasDescription);
            Assert.True(new Enum<NoFlags>(NoFlags.B).HasDescription);
            Assert.True(new Enum<NoFlags>(NoFlags.b).HasDescription);
            Assert.False(new Enum<NoFlags>((NoFlags)sbyte.MaxValue).HasDescription);

            Test.OrdinalEquals(string.Empty, new Enum<NoFlags>(NoFlags.X).Description);
            Test.OrdinalEquals(string.Empty, new Enum<NoFlags>(NoFlags.Zero).Description);
            Test.OrdinalEquals(string.Empty, new Enum<NoFlags>(NoFlags.A).Description);
            Test.OrdinalEquals("bbb", new Enum<NoFlags>(NoFlags.B).Description);
            Test.OrdinalEquals("bbb", new Enum<NoFlags>(NoFlags.b).Description);
            Test.OrdinalEquals(string.Empty, new Enum<NoFlags>((NoFlags)sbyte.MaxValue).Description);
        }

        [Test]
        public void FlagTests()
        {
            Assert.True(new Enum<Flags1>(Flags1.AB).HasFlag(Flags1.AB));
            Assert.True(new Enum<Flags1>(Flags1.AB).HasFlag(Flags1.A));
            Assert.True(new Enum<Flags1>(Flags1.AB).HasFlag((Flags1)2));
            Assert.True(new Enum<Flags1>(Flags1.AB).HasFlag(Enum<Flags1>.Zero));
            Assert.False(new Enum<Flags1>(Flags1.AB).HasFlag(Flags1.D));

            Assert.AreEqual(new Enum<Flags1>(Flags1.A), new Enum<Flags1>(Flags1.A).SetFlag(Flags1.A));
            Assert.AreEqual(new Enum<Flags1>(Flags1.A), new Enum<Flags1>(Flags1.A).SetFlag(Enum<Flags1>.Zero));
            Assert.AreEqual(new Enum<Flags1>(Flags1.AB), new Enum<Flags1>(Flags1.A).SetFlag((Flags1)2));
            Assert.AreEqual(new Enum<Flags1>(Flags1.AB), new Enum<Flags1>((Flags1)2).SetFlag(Flags1.A));

            Assert.AreEqual(new Enum<Flags1>(Flags1.A), new Enum<Flags1>(Flags1.AB).RemoveFlag((Flags1)2));
            Assert.AreEqual(new Enum<Flags1>((Flags1)2), new Enum<Flags1>(Flags1.AB).RemoveFlag(Flags1.A));
            Assert.AreEqual(new Enum<Flags1>(Flags1.AB), new Enum<Flags1>(Flags1.AB).RemoveFlag(Enum<Flags1>.Zero));
            Assert.AreEqual(new Enum<Flags1>(Enum<Flags1>.Zero), new Enum<Flags1>(Flags1.AB).RemoveFlag(Flags1.AB));

            //// IsSimpleFlag and others can be found among the static tests

            Enum<Flags1> e = Flags1.A;
            Assert.NotNull(e.AsSimpleFlags);
            Assert.AreEqual(1, e.AsSimpleFlags.Length);
            Assert.AreEqual(new Enum<Flags1>(Flags1.A), e.AsSimpleFlags[0]);

            e = Flags1.A | Flags1.D;
            Assert.NotNull(e.AsSimpleFlags);
            Assert.AreEqual(2, e.AsSimpleFlags.Length);
            Assert.AreEqual(new Enum<Flags1>(Flags1.A), e.AsSimpleFlags[0]);
            Assert.AreEqual(new Enum<Flags1>(Flags1.D), e.AsSimpleFlags[1]);

            e = Flags1.AB;
            Assert.NotNull(e.AsSimpleFlags);
            Assert.AreEqual(1, e.AsSimpleFlags.Length);
            Assert.AreEqual(new Enum<Flags1>(Flags1.A), e.AsSimpleFlags[0]);

            e = Enum<Flags1>.Zero;
            Assert.NotNull(e.AsSimpleFlags);
            Assert.AreEqual(0, e.AsSimpleFlags.Length);

            e = (Flags1)256;
            Assert.NotNull(e.AsSimpleFlags);
            Assert.AreEqual(0, e.AsSimpleFlags.Length);
        }

        [Test]
        public void ParseTests()
        {
            //// NOTE: since we only wrap the call, this test is not as extensive as it could be
            Assert.AreEqual(new Enum<NoFlags>(NoFlags.X), Enum<NoFlags>.Parse("X"));
            Assert.AreEqual(new Enum<NoFlags>(NoFlags.X), Enum<NoFlags>.Parse("x", ignoreCase: true));
            Assert.AreEqual(new Enum<NoFlags>(NoFlags.X), Enum<NoFlags>.Parse("-1"));
            Assert.AreEqual(new Enum<NoFlags>(NoFlags.X), Enum<NoFlags>.Parse("-1", ignoreCase: true));
            Assert.AreEqual(new Enum<NoFlags>((NoFlags)(-2)), Enum<NoFlags>.Parse("-2"));
            Assert.AreEqual(new Enum<NoFlags>((NoFlags)(-2)), Enum<NoFlags>.Parse("-2", ignoreCase: true));

            Assert.AreEqual(new Enum<Flags1>(Flags1.A | Flags1.D), Enum<Flags1>.Parse("A, D"));
            Assert.AreEqual(new Enum<Flags1>(Flags1.A | Flags1.D), Enum<Flags1>.Parse("A, d", ignoreCase: true));
            Assert.AreEqual(new Enum<Flags1>(Flags1.AB), Enum<Flags1>.Parse("AB"));
            Assert.AreEqual(new Enum<Flags1>(Flags1.AB), Enum<Flags1>.Parse("aB", ignoreCase: true));
            Assert.Throws<ArgumentException>(() => Enum<Flags1>.Parse("A, 2"));
            Assert.Throws<ArgumentException>(() => Enum<Flags1>.Parse("a, 2", ignoreCase: true));
            Assert.Throws<ArgumentException>(() => Enum<Flags1>.Parse("A, 4"));
            Assert.Throws<ArgumentException>(() => Enum<Flags1>.Parse("a, 4", ignoreCase: true));
        }
    }
}
