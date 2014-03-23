using System;
using System.Text;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Json;
using Mechanical.DataStores.Node;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores.Json
{
    [TestFixture]
    public class JsonDataStoreWriterTests
    {
        static JsonDataStoreWriterTests()
        {
            Test.MakeSureBootstrapRun();
        }

        private string WriteNode( IDataStoreNode node, bool indent )
        {
            var sb = new StringBuilder();
            using( var writer = new JsonDataStoreWriter(sb, indent) )
            {
                if( node.NotNullReference() )
                    writer.Write(node);
            }
            return sb.ToString();
        }

        [Test]
        public void WriterTests()
        {
            var json = this.WriteNode(DataStoreTests.EmptyDataStore, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(JsonDataStoreReaderTests.EmptyDataStore, json, StringComparison.Ordinal));

            json = this.WriteNode(DataStoreTests.TextValueRoot, indent: false);
            Assert.True(string.Equals(JsonDataStoreReaderTests.TextValueRoot, json, StringComparison.Ordinal));

            json = this.WriteNode(DataStoreTests.EmptyObjectRoot, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(JsonDataStoreReaderTests.EmptyObjectRoot, json, StringComparison.Ordinal));

            json = this.WriteNode(DataStoreTests.EmptyValueRoot, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(JsonDataStoreReaderTests.EmptyValueRoot, json, StringComparison.Ordinal));

            json = this.WriteNode(DataStoreTests.SimpleRoot, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(JsonDataStoreReaderTests.SimpleRoot, json, StringComparison.Ordinal));
        }
    }
}
