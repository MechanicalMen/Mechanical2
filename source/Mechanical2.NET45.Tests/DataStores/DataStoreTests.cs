using System;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Node;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores
{
    [TestFixture]
    public class DataStoreTests
    {
        [Test]
        public void IsValidNameTests()
        {
            Assert.False(DataStore.IsValidName(null));
            Assert.False(DataStore.IsValidName(string.Empty));
            Assert.False(DataStore.IsValidName(new string('a', count: 256)));
            Assert.True(DataStore.IsValidName(new string('a', count: 255)));
            Assert.True(DataStore.IsValidName("A"));
            Assert.True(DataStore.IsValidName("_"));
            Assert.False(DataStore.IsValidName("á"));
            Assert.False(DataStore.IsValidName("3"));
            Assert.False(DataStore.IsValidName(" "));
            Assert.False(DataStore.IsValidName(" a"));
            Assert.False(DataStore.IsValidName("a "));
        }

        [Test]
        public void SameNameTests()
        {
            Assert.True(DataStore.SameNames("a", "a"));
            Assert.False(DataStore.SameNames("a", "A"));
            Assert.False(DataStore.SameNames("a", "aa"));
        }

        [Test]
        public void EscapeTests()
        {
            Assert.True(string.Equals(DataStore.Escape("a"), "a", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("AA"), "AA", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("_"), "__", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("á"), "_00E1", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("3"), "_0033", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("a3"), "a3", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape(" "), "_0020", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape(" a"), "_0020a", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("a "), "a_0020", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("_a"), "__a", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.Escape("a_"), "a__", StringComparison.Ordinal));
            Assert.False(DataStore.IsValidName(DataStore.Escape(new string('_', count: 255))));

            Assert.True(string.Equals("a", DataStore.Unescape("a"), StringComparison.Ordinal));
            Assert.True(string.Equals("A", DataStore.Unescape("A"), StringComparison.Ordinal));
            Assert.True(string.Equals("_", DataStore.Unescape("__"), StringComparison.Ordinal));
            Assert.True(string.Equals("á", DataStore.Unescape("_00E1"), StringComparison.Ordinal));
            Assert.True(string.Equals("3", DataStore.Unescape("_0033"), StringComparison.Ordinal));
            Assert.True(string.Equals("a3", DataStore.Unescape("a3"), StringComparison.Ordinal));
            Assert.True(string.Equals(" ", DataStore.Unescape("_0020"), StringComparison.Ordinal));
            Assert.True(string.Equals(" a", DataStore.Unescape("_0020a"), StringComparison.Ordinal));
            Assert.True(string.Equals("a ", DataStore.Unescape("a_0020"), StringComparison.Ordinal));
            Assert.True(string.Equals("_a", DataStore.Unescape("__a"), StringComparison.Ordinal));
            Assert.True(string.Equals("a_", DataStore.Unescape("a__"), StringComparison.Ordinal));
            Assert.Throws<FormatException>(() => DataStore.Unescape("_"));
            Assert.Throws<FormatException>(() => DataStore.Unescape("abc_"));
            Assert.Throws<FormatException>(() => DataStore.Unescape("_0"));
            Assert.Throws<FormatException>(() => DataStore.Unescape("_01"));
            Assert.Throws<FormatException>(() => DataStore.Unescape("_012"));
        }

        internal static readonly DataStoreNode EmptyDataStore = null;

        internal static readonly DataStoreNode TextValueRoot = new DataStoreTextValue("textRoot", "abc");

        internal static readonly DataStoreNode EmptyObjectRoot = new DataStoreObject("objectRoot");

        internal static readonly DataStoreNode EmptyValueRoot = new DataStoreTextValue("valueRoot", Substring.Empty);

        internal static readonly DataStoreNode SimpleRoot =
            new DataStoreObject(
                "rootObject",
                new DataStoreTextValue("a", "a2"),
                new DataStoreTextValue("a2", "aa"),
                new DataStoreTextValue("aa", Substring.Empty),
                new DataStoreObject(
                    "o",
                    new DataStoreObject(
                        "o",
                        new DataStoreObject("o"))),
                new DataStoreObject("p"));
    }
}
