using System;
using Mechanical.DataStores;
using Mechanical.DataStores.Json;
using Mechanical.DataStores.Nodes;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores.Json
{
    [TestFixture]
    public class JsonDataStoreReaderTests
    {
        internal static readonly string EmptyDataStore = Test.ReplaceNewLines(@"{}");

        internal static readonly string TextValueRoot = @"{""textRoot"":""abc""}";

        internal static readonly string EmptyObjectRoot = Test.ReplaceNewLines(@"{
  ""objectRoot"" : {}
}");

        internal static readonly string EmptyValueRoot = Test.ReplaceNewLines(@"{
  ""valueRoot"" : """"
}");

        internal static readonly string SimpleRoot = Test.ReplaceNewLines(@"{
  ""rootObject"" : {
    ""a"" : ""a2"",
    ""a2"" : ""aa"",
    ""aa"" : """",
    ""o"" : {
      ""o"" : {
        ""o"" : {}
      }
    },
    ""p"" : {}
  }
}");

        private IDataStoreNode ReadNode( string json, bool isDataStoreJson )
        {
            using( var reader = JsonDataStoreReader.FromJson(json, isDataStoreJson) )
                return reader.ReadAsNode();
        }

        [Test]
        public void ReaderTests()
        {
            var node = this.ReadNode(EmptyDataStore, isDataStoreJson: true);
            Assert.True(object.ReferenceEquals(DataStoreTests.EmptyDataStore, node));

            node = this.ReadNode(TextValueRoot, isDataStoreJson: true);
            Assert.True(DataStoreTests.TextValueRoot.Equals(node));

            node = this.ReadNode(EmptyObjectRoot, isDataStoreJson: true);
            Assert.True(DataStoreTests.EmptyObjectRoot.Equals(node));

            node = this.ReadNode(EmptyValueRoot, isDataStoreJson: true);
            Assert.True(DataStoreTests.EmptyValueRoot.Equals(node));

            node = this.ReadNode(SimpleRoot, isDataStoreJson: true);
            Assert.True(DataStoreTests.SimpleRoot.Equals(node));


            node = this.ReadNode(EmptyDataStore, isDataStoreJson: false);
            IDataStoreNode expectedNode = new DataStoreObject("root");
            Assert.True(expectedNode.Equals(node));

            node = this.ReadNode(TextValueRoot, isDataStoreJson: false);
            expectedNode = new DataStoreObject(
                "root",
                new DataStoreTextValue("textRoot", "abc"));
            Assert.True(expectedNode.Equals(node));

            node = this.ReadNode(EmptyObjectRoot, isDataStoreJson: false);
            expectedNode = new DataStoreObject(
                "root",
                new DataStoreObject("objectRoot"));
            Assert.True(expectedNode.Equals(node));
        }
    }
}
