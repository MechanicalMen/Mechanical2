using System;
using System.IO;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Nodes;
using Mechanical.DataStores.Xml;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores.Xml
{
    [TestFixture]
    public class XmlDataStoreReaderTests
    {
        internal static readonly string EmptyDataStore = Test.ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root></root>");

        internal static readonly string TextValueRoot = @"<root><textRoot>abc</textRoot></root>";

        internal static readonly string EmptyObjectRoot = Test.ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <objectRoot></objectRoot>
</root>");

        internal static readonly string EmptyValueRoot = Test.ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <valueRoot />
</root>");

        internal static readonly string SimpleRoot = Test.ReplaceNewLines(@"<?xml version=""1.0"" encoding=""utf-8""?>
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
                return reader.ReadAsNode();
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
