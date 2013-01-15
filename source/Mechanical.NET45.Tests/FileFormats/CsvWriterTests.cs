using System;
using System.IO;
using Mechanical.FileFormats;
using NUnit.Framework;

namespace Mechanical.Tests.FileFormats
{
    [TestFixture]
    public class CsvWriterTests
    {
        private static string Write( CsvFormat csvFormat, Action<CsvWriter> action )
        {
            using( var sw = new StringWriter() )
            using( var writer = new CsvWriter(sw, csvFormat) )
            {
                action(writer);

                sw.Flush();
                return sw.ToString();
            }
        }

        [Test]
        public void FormatTests()
        {
            string output = Write(
                CsvFormat.International,
                w =>
                {
                    w.Write("a");
                    w.Write(3.14d);
                    w.Write("b");
                });
            Test.OrdinalEquals("a,3.14,b", output);

            output = Write(
                CsvFormat.Continental,
                w =>
                {
                    w.Write("a");
                    w.Write(3.14d);
                    w.Write("b");
                });
            Test.OrdinalEquals("a;3,14;b", output);
        }

        [Test]
        public void SimpleTests()
        {
            string output = Write(
                CsvFormat.International,
                w =>
                {
                    w.WriteLine();

                    w.Write("a");
                    w.Write("b");
                    w.Write("c");
                    w.WriteLine();

                    w.Write();
                    w.Write();
                    w.Write();
                    w.WriteLine();
                });
            Test.OrdinalEquals("\r\na,b,c\r\n,,\r\n", output);
        }

        [Test]
        public void QuotingTests()
        {
            string output = Write(
                CsvFormat.International,
                w =>
                {
                    w.Write();
                    w.Write(string.Empty);
                    w.Write('"');
                    w.Write("\"\"");
                    w.Write(" ");
                    w.Write(" a");
                    w.Write("a ");
                    w.Write(" , ");
                });
            Test.OrdinalEquals(",,\"\"\"\",\"\"\"\"\"\",\" \",\" a\",\"a \",\" , \"", output);

            output = Write(
                CsvFormat.International,
                w =>
                {
                    w.Write("asd \" \r\n\r\n qwe\"");
                });
            Test.OrdinalEquals("\"asd \"\" \r\n\r\n qwe\"\"\"", output);
        }
    }
}
