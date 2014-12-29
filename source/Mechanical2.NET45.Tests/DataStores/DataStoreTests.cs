using System;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Nodes;
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
            Assert.False(DataStore.IsValidName(new string('a', count: 255)));
            Assert.True(DataStore.IsValidName(new string('a', count: 254)));
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
            Assert.True(DataStore.Comparer.Equals("a", "a"));
            Assert.False(DataStore.Comparer.Equals("a", "A"));
            Assert.False(DataStore.Comparer.Equals("a", "aa"));
        }

        [Test]
        public void EscapeTests()
        {
            Assert.True(string.Equals(DataStore.EscapeName("a"), "a", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("AA"), "AA", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("_"), "__", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("á"), "_00E1", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("3"), "_0033", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("a3"), "a3", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName(" "), "_0020", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName(" a"), "_0020a", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("a "), "a_0020", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("_a"), "__a", StringComparison.Ordinal));
            Assert.True(string.Equals(DataStore.EscapeName("a_"), "a__", StringComparison.Ordinal));
            Assert.False(DataStore.IsValidName(DataStore.EscapeName(new string('_', count: 255))));

            Assert.True(string.Equals("a", DataStore.UnescapeName("a"), StringComparison.Ordinal));
            Assert.True(string.Equals("A", DataStore.UnescapeName("A"), StringComparison.Ordinal));
            Assert.True(string.Equals("_", DataStore.UnescapeName("__"), StringComparison.Ordinal));
            Assert.True(string.Equals("á", DataStore.UnescapeName("_00E1"), StringComparison.Ordinal));
            Assert.True(string.Equals("3", DataStore.UnescapeName("_0033"), StringComparison.Ordinal));
            Assert.True(string.Equals("a3", DataStore.UnescapeName("a3"), StringComparison.Ordinal));
            Assert.True(string.Equals(" ", DataStore.UnescapeName("_0020"), StringComparison.Ordinal));
            Assert.True(string.Equals(" a", DataStore.UnescapeName("_0020a"), StringComparison.Ordinal));
            Assert.True(string.Equals("a ", DataStore.UnescapeName("a_0020"), StringComparison.Ordinal));
            Assert.True(string.Equals("_a", DataStore.UnescapeName("__a"), StringComparison.Ordinal));
            Assert.True(string.Equals("a_", DataStore.UnescapeName("a__"), StringComparison.Ordinal));
            Assert.Throws<FormatException>(() => DataStore.UnescapeName("_"));
            Assert.Throws<FormatException>(() => DataStore.UnescapeName("abc_"));
            Assert.Throws<FormatException>(() => DataStore.UnescapeName("_0"));
            Assert.Throws<FormatException>(() => DataStore.UnescapeName("_01"));
            Assert.Throws<FormatException>(() => DataStore.UnescapeName("_012"));
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
