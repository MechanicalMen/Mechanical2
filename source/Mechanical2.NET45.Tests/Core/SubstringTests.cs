using System;
using System.Globalization;
using System.Linq;
using Mechanical.Core;
using NUnit.Framework;

namespace Mechanical.Tests.Core
{
    [TestFixture]
    public class SubstringTests
    {
        [Test]
        public static void ConstructorAndFieldTest()
        {
            Assert.Null(Substring.Null.Origin);
            Assert.AreEqual(0, Substring.Null.StartIndex);
            Assert.AreEqual(0, Substring.Null.Length);
            Assert.Null(Substring.Null.ToString());

            Assert.True(object.ReferenceEquals(string.Empty, Substring.Empty.Origin));
            Assert.AreEqual(0, Substring.Empty.StartIndex);
            Assert.AreEqual(0, Substring.Empty.Length);
            Test.OrdinalEquals(string.Empty, Substring.Empty.ToString());

            var substr = new Substring("asd");
            Test.OrdinalEquals("asd", substr.Origin);
            Assert.AreEqual(0, substr.StartIndex);
            Assert.AreEqual(3, substr.Length);
            Test.OrdinalEquals("asd", substr.ToString());

            substr = new Substring("asd", 1);
            Test.OrdinalEquals("asd", substr.Origin);
            Assert.AreEqual(1, substr.StartIndex);
            Assert.AreEqual(2, substr.Length);
            Test.OrdinalEquals("sd", substr.ToString());

            substr = new Substring("asd", 3);
            Test.OrdinalEquals("asd", substr.Origin);
            Assert.AreEqual(3, substr.StartIndex);
            Assert.AreEqual(0, substr.Length);
            Test.OrdinalEquals(string.Empty, substr.ToString());

            substr = new Substring("asd", 1, 1);
            Test.OrdinalEquals("asd", substr.Origin);
            Assert.AreEqual(1, substr.StartIndex);
            Assert.AreEqual(1, substr.Length);
            Test.OrdinalEquals("s", substr.ToString());

            substr = new Substring("asd", 1, 0);
            Test.OrdinalEquals("asd", substr.Origin);
            Assert.AreEqual(1, substr.StartIndex);
            Assert.AreEqual(0, substr.Length);
            Test.OrdinalEquals(string.Empty, substr.ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() => new Substring("asd", -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Substring("asd", 4));

            Assert.Throws<ArgumentException>(() => new Substring("asd", -1, 0));
            Assert.Throws<ArgumentException>(() => new Substring("asd", 4, 0));
            Assert.Throws<ArgumentException>(() => new Substring("asd", 0, -1));
            Assert.Throws<ArgumentException>(() => new Substring("asd", 0, 4));
            Assert.Throws<ArgumentException>(() => new Substring("asd", 3, 1));
        }

        [Test]
        public static void NullOrEmptyTest()
        {
            Assert.True(Substring.Null.NullOrEmpty);
            Assert.True(Substring.Empty.NullOrEmpty);
            Assert.False(new Substring(" a b ").NullOrEmpty);
        }

        [Test]
        public static void NullOrLengthyTest()
        {
            Assert.True(Substring.Null.NullOrLengthy);
            Assert.True(Substring.Empty.NullOrLengthy);
            Assert.True(new Substring(" ").NullOrLengthy);
            Assert.True(new Substring(" a").NullOrLengthy);
            Assert.True(new Substring("a ").NullOrLengthy);
            Assert.True(new Substring(" a ").NullOrLengthy);
            Assert.False(new Substring("a b").NullOrLengthy);
        }

        [Test]
        public static void NullOrWhiteSpaceTest()
        {
            Assert.True(Substring.Null.NullOrWhiteSpace);
            Assert.True(Substring.Empty.NullOrWhiteSpace);
            Assert.True(new Substring(" ").NullOrWhiteSpace);
            Assert.False(new Substring(" a ").NullOrWhiteSpace);
        }

        [Test]
        public static void IndexerTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Substring.Null[0].ToString());
            Assert.Throws<ArgumentOutOfRangeException>(() => Substring.Empty[0].ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() => new Substring(" ")[-1].ToString());
            Assert.AreEqual(' ', new Substring(" ")[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new Substring(" ")[1].ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() => new Substring("asd", 1, 1)[-1].ToString());
            Assert.AreEqual('s', new Substring("asd", 1, 1)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new Substring("asd", 1, 1)[1].ToString());
        }

        [Test]
        public static void TrimTest()
        {
            var trimChars = new char[] { 'a' };

            Test.OrdinalEquals("aba", new Substring("aba").TrimStart().ToString());
            Test.OrdinalEquals("ba", new Substring("aba").TrimStart(trimChars).ToString());
            Test.OrdinalEquals("aba   ", new Substring("  aba   ").TrimStart().ToString());

            Test.OrdinalEquals("aba", new Substring("aba").TrimEnd().ToString());
            Test.OrdinalEquals("ab", new Substring("aba").TrimEnd(trimChars).ToString());
            Test.OrdinalEquals("  aba", new Substring("  aba   ").TrimEnd().ToString());

            Test.OrdinalEquals("aba", new Substring("aba").Trim().ToString());
            Test.OrdinalEquals("b", new Substring("aba").Trim(trimChars).ToString());
            Test.OrdinalEquals("aba", new Substring("  aba   ").Trim().ToString());
        }

        [Test]
        public static void StartsWith_EndsWithTest()
        {
            Assert.True(new Substring("abc").StartsWith("a", CompareOptions.Ordinal));
            Assert.True(new Substring("abc").StartsWith("ab", CompareOptions.Ordinal));
            Assert.True(new Substring("abc").StartsWith("abc", CompareOptions.Ordinal));
            Assert.False(new Substring("abc").StartsWith("x", CompareOptions.Ordinal));
            Assert.False(new Substring("abc").StartsWith("abcd", CompareOptions.Ordinal));

            Assert.True(new Substring("abc").EndsWith("c", CompareOptions.Ordinal));
            Assert.True(new Substring("abc").EndsWith("bc", CompareOptions.Ordinal));
            Assert.True(new Substring("abc").EndsWith("abc", CompareOptions.Ordinal));
            Assert.False(new Substring("abc").EndsWith("x", CompareOptions.Ordinal));
            Assert.False(new Substring("abc").EndsWith("abcd", CompareOptions.Ordinal));
        }

        [Test]
        public static void SplitTest()
        {
            Assert.Throws<NullReferenceException>(() => ((string)null).Split());
            Assert.Throws<NullReferenceException>(() => Substring.Null.Split());

            var substr = Substring.Null;
            Substring.SplitFirst(ref substr);

            var separator = (char[])null;
            AssertEqual(string.Empty, Substring.Empty, separator, StringSplitOptions.None);
            AssertEqual(string.Empty, Substring.Empty, separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual(" ", " ", separator, StringSplitOptions.None);
            AssertEqual(" ", " ", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("asd", "asd", separator, StringSplitOptions.None);
            AssertEqual("asd", "asd", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("a a", "a a", separator, StringSplitOptions.None);
            AssertEqual("a a", "a a", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("a ", "a ", separator, StringSplitOptions.None);
            AssertEqual("a ", "a ", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual(" a", " a", separator, StringSplitOptions.None);
            AssertEqual(" a", " a", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual(" a ", " a ", separator, StringSplitOptions.None);
            AssertEqual(" a ", " a ", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("   ", "   ", separator, StringSplitOptions.None);
            AssertEqual("   ", "   ", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("a a a a", "a a a a", separator, StringSplitOptions.None);
            AssertEqual("a a a a", "a a a a", separator, StringSplitOptions.RemoveEmptyEntries);

            separator = new char[] { '/' };
            AssertEqual(string.Empty, Substring.Empty, separator, StringSplitOptions.None);
            AssertEqual(string.Empty, Substring.Empty, separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("/", "/", separator, StringSplitOptions.None);
            AssertEqual("/", "/", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("asd", "asd", separator, StringSplitOptions.None);
            AssertEqual("asd", "asd", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("a/a", "a/a", separator, StringSplitOptions.None);
            AssertEqual("a/a", "a/a", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("a/", "a/", separator, StringSplitOptions.None);
            AssertEqual("a/", "a/", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("/a", "/a", separator, StringSplitOptions.None);
            AssertEqual("/a", "/a", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("/a/", "/a/", separator, StringSplitOptions.None);
            AssertEqual("/a/", "/a/", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("///", "///", separator, StringSplitOptions.None);
            AssertEqual("///", "///", separator, StringSplitOptions.RemoveEmptyEntries);
            AssertEqual("a/a/a/a", "a/a/a/a", separator, StringSplitOptions.None);
            AssertEqual("a/a/a/a", "a/a/a/a", separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void AssertEqual( string str, Substring substr, char[] separator, StringSplitOptions options )
        {
            var arr1 = str.Split(separator, options);
            var arr2 = substr.Split(separator, options).Select(s => s.ToString()).ToArray();

            Assert.AreEqual(arr1.Length, arr2.Length);
            for( int i = 0; i < arr1.Length; ++i )
                Test.OrdinalEquals(arr1[i], arr2[i]);
        }
    }
}
