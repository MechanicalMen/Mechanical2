using System;
using System.IO;
using System.Linq;
using Mechanical.FileFormats;
using NUnit.Framework;

namespace Mechanical.Tests.FileFormats
{
    [TestFixture]
    public class CsvReaderTests
    {
        private static void Read( CsvFormat csvFormat, string content, params Action<string[]>[] actions )
        {
            using( var sr = new StringReader(content) )
            using( var reader = new CsvReader(sr, csvFormat) )
            {
                foreach( var a in actions )
                {
                    Assert.True(reader.Read());

                    Assert.NotNull(reader.Record);
                    a(reader.Record.ToArray());
                }

                Assert.False(reader.Read());
            }
        }

        [Test]
        public void FormatTests()
        {
            Read(
                CsvFormat.International,
                "a,3.14,b",
                record =>
                {
                    Assert.AreEqual(3, record.Length);
                    Test.OrdinalEquals("a", record[0]);
                    Test.OrdinalEquals("3.14", record[1]);
                    Test.OrdinalEquals("b", record[2]);
                });

            Read(
                CsvFormat.Continental,
                "a;3,14;b",
                record =>
                {
                    Assert.AreEqual(3, record.Length);
                    Test.OrdinalEquals("a", record[0]);
                    Test.OrdinalEquals("3,14", record[1]);
                    Test.OrdinalEquals("b", record[2]);
                });
        }

        [Test]
        public void SimpleTests()
        {
            Read(
                CsvFormat.International,
                "\r\na,b,c\r\n,,\r\n",
                record =>
                {
                    Assert.AreEqual(1, record.Length);
                    Test.OrdinalEquals(string.Empty, record[0]);
                },
                record =>
                {
                    Assert.AreEqual(3, record.Length);
                    Test.OrdinalEquals("a", record[0]);
                    Test.OrdinalEquals("b", record[1]);
                    Test.OrdinalEquals("c", record[2]);
                },
                record =>
                {
                    Assert.AreEqual(3, record.Length);
                    Test.OrdinalEquals(string.Empty, record[0]);
                    Test.OrdinalEquals(string.Empty, record[1]);
                    Test.OrdinalEquals(string.Empty, record[2]);
                });
        }

        [Test]
        public void QuotingTests()
        {
            Read(
                CsvFormat.International,
                ",,\"\"\"\",\"\"\"\"\"\",\" \",\" a\",\"a \",\" , \"",
                record =>
                {
                    Assert.AreEqual(8, record.Length);
                    Test.OrdinalEquals(string.Empty, record[0]);
                    Test.OrdinalEquals(string.Empty, record[1]);
                    Test.OrdinalEquals("\"", record[2]);
                    Test.OrdinalEquals("\"\"", record[3]);
                    Test.OrdinalEquals(" ", record[4]);
                    Test.OrdinalEquals(" a", record[5]);
                    Test.OrdinalEquals("a ", record[6]);
                    Test.OrdinalEquals(" , ", record[7]);
                });

            Read(
                CsvFormat.International,
                "\"asd \"\" \r\n\r\n qwe\"\"\"",
                record =>
                {
                    Assert.AreEqual(1, record.Length);
                    Test.OrdinalEquals("asd \" \r\n\r\n qwe\"", record[0]);
                });
        }
    }
}
