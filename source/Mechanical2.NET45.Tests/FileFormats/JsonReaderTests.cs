using System;
using System.Linq;
using Mechanical.Core;
using Mechanical.FileFormats;
using Mechanical.IO;
using NUnit.Framework;

namespace Mechanical.Tests.FileFormats
{
    [TestFixture]
    public class JsonReaderTests
    {
        private struct RawJson
        {
            internal readonly JsonToken Token;
            internal readonly string RawValue;

            internal RawJson( JsonToken token, string rawValue )
            {
                this.Token = token;
                this.RawValue = rawValue;
            }
        }

        private static void Read( string content, params RawJson[] raw )
        {
            var sr = new StringReader(content);
            using( var reader = new JsonReader(sr) )
            {
                Assert.Throws<InvalidOperationException>(() => reader.Token.NotNullReference());
                Assert.Throws<InvalidOperationException>(() => reader.RawValue.NotNullReference());

                if( raw.NullReference()
                 || raw.Length == 0 )
                {
                    Assert.False(reader.Read());
                }
                else
                {
                    foreach( var r in raw )
                    {
                        Assert.True(reader.Read());

                        Assert.AreEqual(r.Token, reader.Token);
                        Test.OrdinalEquals(r.RawValue, reader.RawValue);
                    }
                    Assert.False(reader.Read());
                }

                Assert.False(reader.Read());
                Assert.Throws<InvalidOperationException>(() => reader.Token.NotNullReference());
                Assert.Throws<InvalidOperationException>(() => reader.RawValue.NotNullReference());
            }
        }

        [Test]
        public void EmptyTests()
        {
            Read(string.Empty);
            Read("   ");
            Read("\n");
            Read("\r\n   \n  \r\t");
        }

        [Test]
        public void NonStringTests()
        {
            Read(
                JsonWriterTests.NonStringJson1,
                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.Name, "key"),
                new RawJson(JsonToken.NullValue, null),
                new RawJson(JsonToken.ObjectEnd, "}"));

            Read(
                JsonWriterTests.NonStringJson2,
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.BooleanValue, "true"),
                new RawJson(JsonToken.BooleanValue, "false"),
                new RawJson(JsonToken.NumberValue, "123"),
                new RawJson(JsonToken.NumberValue, "3.14"),
                new RawJson(JsonToken.NumberValue, "1.23"),
                new RawJson(JsonToken.NumberValue, "90000000000"),
                new RawJson(JsonToken.ArrayEnd, "]"));
        }

        [Test]
        public void StringTests()
        {
            Read(
                JsonWriterTests.StringJson1,
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.NullValue, null),
                new RawJson(JsonToken.StringValue, string.Empty),
                new RawJson(JsonToken.StringValue, string.Empty),
                new RawJson(JsonToken.StringValue, "abc"),
                new RawJson(JsonToken.StringValue, " def  "),
                new RawJson(JsonToken.StringValue, "bc d"),
                new RawJson(JsonToken.ArrayEnd, "]"));

            Read(
                JsonWriterTests.StringJson2,
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.StringValue, "\"\\/\b\f\n\r\t"),
                new RawJson(JsonToken.StringValue, "\'\0\a\v"),
                new RawJson(JsonToken.ArrayEnd, "]"));

            Read(
                JsonWriterTests.StringJson3,
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.StringValue, "π"),
                new RawJson(JsonToken.ArrayEnd, "]"));

            Read(
                JsonWriterTests.StringJson4,
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.StringValue, "π"),
                new RawJson(JsonToken.ArrayEnd, "]"));
        }

        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "In this case, readability is much improved (by not disregarding this rule).")]
        public void ComplexTests()
        {
            Read(
                JsonWriterTests.ComplexJson1,
                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.Name, "a"),
                new RawJson(JsonToken.NumberValue, "0"),

                new RawJson(JsonToken.Name, "b"),
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.ArrayEnd, "]"),

                new RawJson(JsonToken.Name, "c"),
                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.ObjectEnd, "}"),

                new RawJson(JsonToken.Name, "d"),
                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.Name, "e"),
                new RawJson(JsonToken.NullValue, null),
                new RawJson(JsonToken.ObjectEnd, "}"),

                new RawJson(JsonToken.Name, "f"),
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.ObjectEnd, "}"),
                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.ObjectEnd, "}"),
                new RawJson(JsonToken.BooleanValue, "true"),
                new RawJson(JsonToken.StringValue, " ab\vc "),

                new RawJson(JsonToken.ObjectStart, "{"),
                new RawJson(JsonToken.Name, "g"),
                new RawJson(JsonToken.NumberValue, "1000000000000"),
                new RawJson(JsonToken.Name, "h"),
                new RawJson(JsonToken.NumberValue, "0.1234567890123456789"),
                new RawJson(JsonToken.ObjectEnd, "}"),

                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.StringValue, "asd\"\\/\b\f\n\r\t á"),
                new RawJson(JsonToken.NullValue, null),
                new RawJson(JsonToken.ArrayEnd, "]"),

                new RawJson(JsonToken.BooleanValue, "false"),
                new RawJson(JsonToken.ArrayEnd, "]"),

                new RawJson(JsonToken.Name, "g"),
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.ArrayStart, "["),
                new RawJson(JsonToken.ArrayEnd, "]"),
                new RawJson(JsonToken.ArrayEnd, "]"),
                new RawJson(JsonToken.ArrayEnd, "]"),

                new RawJson(JsonToken.ObjectEnd, "}"));
        }
    }
}
