using System;
using System.IO;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Node;
using Mechanical.DataStores.Xml;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores.Xml
{
    [TestFixture]
    public class XmlDataStoreReaderTests
    {
        static XmlDataStoreReaderTests()
        {
            Test.MakeSureBootstrapRun();
        }

        private static string ReplaceNewLines( string str )
        {
            using( var reader = new StringReader(str) )
            using( var writer = new StringWriter() )
            {
                writer.NewLine = DataStore.DefaultNewLine;

                string line;
                while( (line = reader.ReadLine()).NotNullReference() )
                    writer.WriteLine(line);

                return writer.ToString();
            }
        }

        internal static readonly string EmptyDataStore = ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root></root>");

        internal static readonly string TextValueRoot = @"<root><textRoot>abc</textRoot></root>";

        internal static readonly string EmptyObjectRoot = ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <objectRoot></objectRoot>
</root>");

        internal static readonly string EmptyValueRoot = ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <valueRoot />
</root>");

        internal static readonly string SimpleRoot = ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <rootObject>
    <a>a2</a>
    <a2>aa</a2>
    <aa />
    <o>
      <o>
        <o></o>
      </o>
    </o>
    <p></p>
  </rootObject>
</root>");

        private IDataStoreNode ReadNode( string xml )
        {
            using( var reader = XmlDataStoreReader.FromXml(xml) )
                return reader.ReadNode();
        }

        [Test]
        public void ReaderTests()
        {
            var node = this.ReadNode(EmptyDataStore);
            Assert.True(object.ReferenceEquals(DataStoreTests.EmptyDataStore, node));

            node = this.ReadNode(TextValueRoot);
            Assert.True(DataStoreTests.TextValueRoot.Equals(node));

            node = this.ReadNode(EmptyObjectRoot);
            Assert.True(DataStoreTests.EmptyObjectRoot.Equals(node));

            node = this.ReadNode(EmptyValueRoot);
            Assert.True(DataStoreTests.EmptyValueRoot.Equals(node));

            node = this.ReadNode(SimpleRoot);
            Assert.True(DataStoreTests.SimpleRoot.Equals(node));
        }
    }
}
