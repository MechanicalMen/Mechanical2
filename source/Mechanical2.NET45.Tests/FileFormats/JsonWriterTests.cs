using System;
using Mechanical.Core;
using Mechanical.FileFormats;
using Mechanical.IO;
using NUnit.Framework;

namespace Mechanical.Tests.FileFormats
{
    [TestFixture]
    public class JsonWriterTests
    {
        private static string Write( bool indent, bool produceAscii, Action<JsonWriter> action )
        {
            var sw = new StringWriter();
            using( var writer = new JsonWriter(sw, indent, produceAscii) )
            {
                action(writer);

                sw.Flush();
                return sw.ToString();
            }
        }

        internal const string NonStringJson1 = "{\n  \"key\" : null\n}";
        internal const string NonStringJson2 = "[true,false,123,3.14,1.23,90000000000]";

        [Test]
        public void NonStringLiteralTests()
        {
            string output = Write(
                indent: true,
                produceAscii: false,
                action: w =>
                {
                    w.WriteObjectStart();
                    w.WriteName("key");
                    w.WriteNullValue();
                    w.WriteObjectEnd();
                });
            Test.OrdinalEquals(NonStringJson1, output);

            output = Write(
                indent: false,
                produceAscii: false,
                action: w =>
                {
                    w.WriteArrayStart();
                    w.WriteValue(true);
                    w.WriteValue(false);
                    w.WriteValue(123L);
                    w.WriteValue(3.14d);
                    w.WriteValue(1.23m);
                    w.WriteValue(9e10d);
                    w.WriteArrayEnd();
                });
            Test.OrdinalEquals(NonStringJson2, output);
        }

        internal const string StringJson1 = @"[null,"""","""",""abc"","" def  "",""bc d""]";
        internal const string StringJson2 = @"[""\""\\\/\b\f\n\r\t"",""'\u0000\u0007\u000B""]";
        internal const string StringJson3 = @"[""π""]";
        internal const string StringJson4 = @"[""\u03C0""]";

        [Test]
        public void StringLiteralTests()
        {
            string output = Write(
                indent: false,
                produceAscii: false,
                action: w =>
                {
                    w.WriteArrayStart();
                    w.WriteValue((string)null);
                    w.WriteValue(string.Empty);
                    w.WriteValue(Substring.Empty);
                    w.WriteValue("abc");
                    w.WriteValue(" def  ");
                    w.WriteValue(new Substring("abc def", startIndex: 1, length: 4));
                    w.WriteArrayEnd();
                });
            Test.OrdinalEquals(StringJson1, output);

            output = Write(
                indent: false,
                produceAscii: false,
                action: w =>
                {
                    w.WriteArrayStart();
                    w.WriteValue("\"\\/\b\f\n\r\t");
                    w.WriteValue("\'\0\a\v");
                    w.WriteArrayEnd();
                });
            Test.OrdinalEquals(StringJson2, output);

            output = Write(
                indent: false,
                produceAscii: false,
                action: w =>
                {
                    w.WriteArrayStart();
                    w.WriteValue("π");
                    w.WriteArrayEnd();
                });
            Test.OrdinalEquals(StringJson3, output);

            output = Write(
                indent: false,
                produceAscii: true,
                action: w =>
                {
                    w.WriteArrayStart();
                    w.WriteValue("π");
                    w.WriteArrayEnd();
                });
            Test.OrdinalEquals(StringJson4, output);
        }

        internal static readonly string ComplexJson1 = Test.ReplaceNewLines(@"{
  ""a"" : 0,
  ""b"" : [],
  ""c"" : {},
  ""d"" : {
    ""e"" : null
  },
  ""f"" : [
    {},
    {},
    true,
    "" ab\u000Bc "",
    {
      ""g"" : 1000000000000,
      ""h"" : 0.1234567890123456789
    },
    [
      ""asd\""\\\/\b\f\n\r\t á"",
      null
    ],
    false
  ],
  ""g"" : [
    [
      []
    ]
  ]
}").TrimEnd(); // TrimEnd() is necessary, because ReplaceNewLines appends an extra new line at the end

        [Test]
        public void ComplexTests()
        {
            string output = Write(
                indent: true,
                produceAscii: false,
                action: w =>
                {
                    w.WriteObjectStart();
                    w.WriteName("a");
                    w.WriteValue(0);

                    w.WriteName("b");
                    w.WriteArrayStart();
                    w.WriteArrayEnd();

                    w.WriteName("c");
                    w.WriteObjectStart();
                    w.WriteObjectEnd();

                    w.WriteName("d");
                    w.WriteObjectStart();
                    w.WriteName("e");
                    w.WriteNullValue();
                    w.WriteObjectEnd();

                    w.WriteName("f");
                    w.WriteArrayStart();
                    w.WriteObjectStart();
                    w.WriteObjectEnd();
                    w.WriteObjectStart();
                    w.WriteObjectEnd();
                    w.WriteValue(true);
                    w.WriteValue(" ab\vc ");

                    w.WriteObjectStart();
                    w.WriteName("g");
                    w.WriteValue(10e11d);
                    w.WriteName("h");
                    w.WriteValue(0.1234567890123456789m);
                    w.WriteObjectEnd();

                    w.WriteArrayStart();
                    w.WriteValue("asd\"\\/\b\f\n\r\t á");
                    w.WriteValue((string)null);
                    w.WriteArrayEnd();

                    w.WriteValue(false);
                    w.WriteArrayEnd();

                    w.WriteName("g");
                    w.WriteArrayStart();
                    w.WriteArrayStart();
                    w.WriteArrayStart();
                    w.WriteArrayEnd();
                    w.WriteArrayEnd();
                    w.WriteArrayEnd();

                    w.WriteObjectEnd();
                });
            Test.OrdinalEquals(ComplexJson1, output);
        }

        [Test]
        public void WriteUnknownValueTests()
        {
            // boolean values, and whitespaces
            this.WriteUnknownValueEquals(expectedOutput: @"true", valueToPrint: "true");
            this.WriteUnknownValueEquals(expectedOutput: @""" true""", valueToPrint: " true");
            this.WriteUnknownValueEquals(expectedOutput: @"false", valueToPrint: "false");
            this.WriteUnknownValueEquals(expectedOutput: @"""false  """, valueToPrint: "false  ");

            // numbers, whitespaces, and optional leading signs
            this.WriteUnknownValueEquals(expectedOutput: @""" 5""", valueToPrint: " 5");
            this.WriteUnknownValueEquals(expectedOutput: @"5", valueToPrint: "5");
            this.WriteUnknownValueEquals(expectedOutput: @"-5", valueToPrint: "-5");
            this.WriteUnknownValueEquals(expectedOutput: @"""+5""", valueToPrint: "+5");

            // numbers and leading zeroes
            this.WriteUnknownValueEquals(expectedOutput: @"1", valueToPrint: "1");
            this.WriteUnknownValueEquals(expectedOutput: @"0", valueToPrint: "0");
            this.WriteUnknownValueEquals(expectedOutput: @"10", valueToPrint: "10");
            this.WriteUnknownValueEquals(expectedOutput: @"""01""", valueToPrint: "01");
            this.WriteUnknownValueEquals(expectedOutput: @"""100 000""", valueToPrint: "100 000");
            this.WriteUnknownValueEquals(expectedOutput: @"""100,000""", valueToPrint: "100,000");

            // numbers with fractional parts only
            this.WriteUnknownValueEquals(expectedOutput: @"0.1", valueToPrint: "0.1");
            this.WriteUnknownValueEquals(expectedOutput: @"0.01", valueToPrint: "0.01");
            this.WriteUnknownValueEquals(expectedOutput: @"0.1000", valueToPrint: "0.1000");
            this.WriteUnknownValueEquals(expectedOutput: @"123.456", valueToPrint: "123.456");
            this.WriteUnknownValueEquals(expectedOutput: @"""123.""", valueToPrint: "123.");
            this.WriteUnknownValueEquals(expectedOutput: @"""123.a""", valueToPrint: "123.a");

            // numbers with exponential parts only
            this.WriteUnknownValueEquals(expectedOutput: @"0e1", valueToPrint: "0e1");
            this.WriteUnknownValueEquals(expectedOutput: @"0e+1", valueToPrint: "0e+1");
            this.WriteUnknownValueEquals(expectedOutput: @"0e-1", valueToPrint: "0e-1");
            this.WriteUnknownValueEquals(expectedOutput: @"0E1", valueToPrint: "0E1");
            this.WriteUnknownValueEquals(expectedOutput: @"0E+1", valueToPrint: "0E+1");
            this.WriteUnknownValueEquals(expectedOutput: @"0E-1", valueToPrint: "0E-1");
            this.WriteUnknownValueEquals(expectedOutput: @"0e001", valueToPrint: "0e001");
            this.WriteUnknownValueEquals(expectedOutput: @"0E123", valueToPrint: "0E123");
            this.WriteUnknownValueEquals(expectedOutput: @"""0e""", valueToPrint: "0e");
            this.WriteUnknownValueEquals(expectedOutput: @"""0e+""", valueToPrint: "0e+");

            // numbers with both fractional and exponential parts
            this.WriteUnknownValueEquals(expectedOutput: @"0.1e+2", valueToPrint: "0.1e+2");

            // strings
            this.WriteUnknownValueEquals(expectedOutput: @"""a""", valueToPrint: "a");
        }

        private void WriteUnknownValueEquals( string expectedOutput, Substring valueToPrint )
        {
            string output = Write(
                indent: false,
                produceAscii: false,
                action: w =>
                {
                    w.WriteArrayStart();
                    w.WriteUnknownValue(valueToPrint);
                    w.WriteArrayEnd();
                });

            // remove array characters
            output = output.Substring(startIndex: 1, length: output.Length - 2);

            Test.OrdinalEquals(expectedOutput, output);
        }
    }
}
