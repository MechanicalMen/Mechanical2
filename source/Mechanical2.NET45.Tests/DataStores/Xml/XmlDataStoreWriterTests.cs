using System;
using System.Text;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Node;
using Mechanical.DataStores.Xml;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores.Xml
{
    [TestFixture]
    public class XmlDataStoreWriterTests
    {
        static XmlDataStoreWriterTests()
        {
            Test.MakeSureBootstrapRun();
        }

        private string WriteNode( IDataStoreNode node, bool indent )
        {
            var sb = new StringBuilder();
            using( var writer = new XmlDataStoreWriter(sb, indent) )
            {
                if( node.NotNullReference() )
                    writer.Write(node);
            }
            return sb.ToString();
        }

        [Test]
        public void WriterTests()
        {
            var xml = this.WriteNode(DataStoreTests.EmptyDataStore, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(XmlDataStoreReaderTests.EmptyDataStore, xml, StringComparison.Ordinal));

            xml = this.WriteNode(DataStoreTests.TextValueRoot, indent: false);
            Assert.True(string.Equals(@"<?xml version=""1.0"" encoding=""utf-8""?>" + XmlDataStoreReaderTests.TextValueRoot, xml, StringComparison.Ordinal));

            xml = this.WriteNode(DataStoreTests.EmptyObjectRoot, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(XmlDataStoreReaderTests.EmptyObjectRoot, xml, StringComparison.Ordinal));

            xml = this.WriteNode(DataStoreTests.EmptyValueRoot, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(XmlDataStoreReaderTests.EmptyValueRoot, xml, StringComparison.Ordinal));

            xml = this.WriteNode(DataStoreTests.SimpleRoot, indent: true) + DataStore.DefaultNewLine;
            Assert.True(string.Equals(XmlDataStoreReaderTests.SimpleRoot, xml, StringComparison.Ordinal));
        }
    }
}
