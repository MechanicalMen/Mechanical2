using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mechanical.Core;
using NUnit.Framework;

namespace Mechanical.Tests.Core
{
    [TestFixture]
    public class SafeStringTests
    {
        #region Private Members

        private const int NullProvider = 0;
        private const int InvariantCulture = 1;

        private static CultureInfo GetCulture( int code )
        {
            switch( code )
            {
            case NullProvider:
                return null;

            case InvariantCulture:
                return CultureInfo.InvariantCulture;

            default:
                throw new System.Collections.Generic.KeyNotFoundException("Cold not find culture: " + code.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void TestException<TException>( IFormatProvider formatProvider, string formatFormat, string printFormat, object obj, string result )
            where TException : Exception
        {
            Assert.Throws<TException>(() => string.Format(formatProvider, formatFormat, obj));

            string tryPrintResult;
            Assert.False(SafeString.TryPrint(obj, printFormat, formatProvider, out tryPrintResult));
            Test.OrdinalEquals(result, tryPrintResult);
        }

        private static void TestException<TException>( IFormatProvider formatProvider, string format, object[] args, string result )
            where TException : Exception
        {
            Assert.Throws<TException>(() => string.Format(formatProvider, format, args));

            string tryFormatResult;
            Assert.False(SafeString.TryFormat(out tryFormatResult, formatProvider, format, args));
            Test.OrdinalEquals(result, tryFormatResult);
        }

        private class BadFormatProvider : IFormatProvider
        {
            public object GetFormat( Type formatType )
            {
                throw new MissingMemberException();
            }

            public static readonly BadFormatProvider Default = new BadFormatProvider();
        }

        private class BadFormatter : IFormatProvider, ICustomFormatter
        {
            public object GetFormat( Type formatType )
            {
                if( formatType == typeof(ICustomFormatter) )
                    return this;
                else
                    return null;
            }

            public string Format( string format, object arg, IFormatProvider formatProvider )
            {
                throw new MissingFieldException();
            }

            public static readonly BadFormatter Default = new BadFormatter();
        }

        private class BadFormattable : IFormattable
        {
            public string ToString( string format, IFormatProvider formatProvider )
            {
                throw new MissingMethodException();
            }

            public override string ToString()
            {
                return "BadFormattable";
            }

            public static readonly BadFormattable Default = new BadFormattable();
        }

        private class BadToString
        {
            public override string ToString()
            {
                throw new MemberAccessException();
            }

            public static readonly BadToString Default = new BadToString();
        }

        internal class Nested1
        {
            public Nested1()
            {
            }

            public Nested1( int i )
            {
            }

            internal struct Nested2<T>
            {
                internal interface INested3
                {
                }
            }
        }

        public static void ParamTestMethod( int p0, out float p1, ref byte p2, params object[] p3 )
        {
            p1 = default(float);
        }

        public static void GenericTestMethod<T1, T2>()
        {
        }

        #endregion

        #region TryPrint, Print

        [TestCase(InvariantCulture, null, 'a', "a"),
        TestCase(InvariantCulture, null, null, ""),
        TestCase(InvariantCulture, "x", 17, "11")]
        public static void PrintSuccessTest( int cultureCode, string format, object obj, string result )
        {
            var culture = GetCulture(cultureCode);
            var frmt = string.IsNullOrEmpty(format) ? "{0}" : "{0:" + format + "}";

            string formatResult = string.Format(culture, frmt, obj);
            Test.OrdinalEquals(result, formatResult);

            string tryPrintResult;
            Assert.True(SafeString.TryPrint(obj, format, culture, out tryPrintResult));
            Test.OrdinalEquals(result, tryPrintResult);
        }

        [Test]
        public static void PrintFailureTest()
        {
            TestException<MissingMemberException>(BadFormatProvider.Default, "{0}", null, 5, "5");
            TestException<MissingFieldException>(BadFormatter.Default, "{0}", null, 6, "6");
            TestException<MissingMethodException>(CultureInfo.InvariantCulture, "{0}", null, BadFormattable.Default, "BadFormattable");
            TestException<MemberAccessException>(CultureInfo.InvariantCulture, "{0}", null, BadToString.Default, string.Empty);
        }

        #endregion

        #region FormatProviders

        [Test]
        public static void EnumerableTest()
        {
            Test.OrdinalEquals("{}", SafeString.Print(new object[] { }, SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{}", SafeString.Print(new object[] { }, "R", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{}", SafeString.Print(new object[] { }, "G2", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals(string.Empty, SafeString.Print(new object[] { }, "G1", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals(string.Empty, SafeString.Print(new object[] { }, "G0", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{}", SafeString.Print(new object[] { }, "G-1", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{}", SafeString.Print(new object[] { }, "qwe", SafeString.FormatProvider.Enumerable.Default));

            Test.OrdinalEquals("{abcdef}", SafeString.Print(new object[] { "abcdef" }, "r", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{...}", SafeString.Print(new object[] { "abcdef" }, "G7", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals(string.Empty, SafeString.Print(new object[] { "abcdef" }, "G4", SafeString.FormatProvider.Enumerable.Default));

            Test.OrdinalEquals("{a, b, c, d}", SafeString.Print(new object[] { 'a', 'b', 'c', 'd' }, "G", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{a, ...}", SafeString.Print(new object[] { 'a', 'b', 'c', 'd' }, "g8", SafeString.FormatProvider.Enumerable.Default));
            Test.OrdinalEquals("{...}", SafeString.Print(new object[] { 'a', 'b', 'c', 'd' }, " G 7 ", SafeString.FormatProvider.Enumerable.Default));

            Test.OrdinalEquals("{11}", SafeString.Print(new object[] { 17 }, " G ; X ", SafeString.FormatProvider.Enumerable.Default));
        }

        [Test]
        public static void DebugTest()
        {
            // literals
            Test.OrdinalEquals("null", SafeString.DebugPrint(null));
            Test.OrdinalEquals("true", SafeString.DebugPrint(true));
            Test.OrdinalEquals("false", SafeString.DebugPrint(false));
            Test.OrdinalEquals("8y", SafeString.DebugPrint((sbyte)8));
            Test.OrdinalEquals("8uy", SafeString.DebugPrint((byte)8));
            Test.OrdinalEquals("8s", SafeString.DebugPrint((short)8));
            Test.OrdinalEquals("8us", SafeString.DebugPrint((ushort)8));
            Test.OrdinalEquals("8", SafeString.DebugPrint((int)8));
            Test.OrdinalEquals("8u", SafeString.DebugPrint(8u));
            Test.OrdinalEquals("8L", SafeString.DebugPrint(8L));
            Test.OrdinalEquals("8UL", SafeString.DebugPrint(8UL));
            Test.OrdinalEquals("8f", SafeString.DebugPrint(8f));
            Test.OrdinalEquals("8d", SafeString.DebugPrint(8d));
            Test.OrdinalEquals("8m", SafeString.DebugPrint(8m));
            Test.OrdinalEquals("'a'", SafeString.DebugPrint('a'));
            Test.OrdinalEquals(@"""a""", SafeString.DebugPrint("a"));
            Test.OrdinalEquals("\"a\"\"b\"", SafeString.DebugPrint("a\"b"));

            // Substring
            Test.OrdinalEquals(SafeString.DebugPrint("a"), SafeString.DebugPrint(new Substring("a")));
            Test.OrdinalEquals(SafeString.DebugPrint("a\"b"), SafeString.DebugPrint(new Substring("a\"b")));

            // DateTime, DateTimeOffset
            var now = DateTime.Now;
            Test.OrdinalEquals(now.ToString("o"), SafeString.DebugPrint(now));
            now = new DateTime(now.Ticks, DateTimeKind.Utc);
            Test.OrdinalEquals(now.ToString("o"), SafeString.DebugPrint(now));
            now = new DateTime(now.Ticks, DateTimeKind.Unspecified);
            Test.OrdinalEquals(now.ToString("o"), SafeString.DebugPrint(now));
            var now2 = new DateTimeOffset(now.Ticks, TimeSpan.FromHours(1));
            Test.OrdinalEquals(now2.ToString("o"), SafeString.DebugPrint(now2));
            now2 = new DateTimeOffset(now.Ticks, TimeSpan.Zero);
            Test.OrdinalEquals(now2.ToString("o"), SafeString.DebugPrint(now2));

            // built in types
            Test.OrdinalEquals("byte", SafeString.DebugPrint(typeof(byte)));
            Test.OrdinalEquals("sbyte", SafeString.DebugPrint(typeof(sbyte)));
            Test.OrdinalEquals("short", SafeString.DebugPrint(typeof(short)));
            Test.OrdinalEquals("ushort", SafeString.DebugPrint(typeof(ushort)));
            Test.OrdinalEquals("int", SafeString.DebugPrint(typeof(int)));
            Test.OrdinalEquals("uint", SafeString.DebugPrint(typeof(uint)));
            Test.OrdinalEquals("long", SafeString.DebugPrint(typeof(long)));
            Test.OrdinalEquals("ulong", SafeString.DebugPrint(typeof(ulong)));
            Test.OrdinalEquals("float", SafeString.DebugPrint(typeof(float)));
            Test.OrdinalEquals("double", SafeString.DebugPrint(typeof(double)));
            Test.OrdinalEquals("decimal", SafeString.DebugPrint(typeof(decimal)));
            Test.OrdinalEquals("char", SafeString.DebugPrint(typeof(char)));
            Test.OrdinalEquals("string", SafeString.DebugPrint(typeof(string)));
            Test.OrdinalEquals("bool", SafeString.DebugPrint(typeof(bool)));
            Test.OrdinalEquals("object", SafeString.DebugPrint(typeof(object)));
            Test.OrdinalEquals("void", SafeString.DebugPrint(typeof(void)));

            // generics
            Test.OrdinalEquals("Exception", SafeString.DebugPrint(typeof(Exception)));
            Test.OrdinalEquals("Tuple<Type, Exception>", SafeString.DebugPrint(typeof(Tuple<Type, Exception>)));
            Test.OrdinalEquals("Action<T1, T2>", SafeString.DebugPrint(typeof(Action<,>)));
            Test.OrdinalEquals("KeyValuePair<int, TValue>", SafeString.DebugPrint(typeof(KeyValuePair<,>).MakeGenericType(typeof(int), typeof(KeyValuePair<,>).GetGenericArguments()[1])));
            Test.OrdinalEquals("IEnumerable", SafeString.DebugPrint(typeof(System.Collections.IEnumerable)));
            Test.OrdinalEquals("IEnumerable<object>", SafeString.DebugPrint(typeof(IEnumerable<object>)));
            Test.OrdinalEquals("IEnumerator<T>", SafeString.DebugPrint(typeof(IEnumerator<>)));
            Test.OrdinalEquals("Action<KeyValuePair<IEnumerable<float>, Tuple<int, char, string>>, IEnumerator<Tuple<decimal>>>", SafeString.DebugPrint(typeof(Action<KeyValuePair<IEnumerable<float>, Tuple<int, char, string>>, IEnumerator<Tuple<decimal>>>)));

            // full namespace, nested types (& nested generics)
            Test.OrdinalEquals("Mechanical.Tests.Core.SafeStringTests", SafeString.DebugPrint(typeof(SafeStringTests)));
            Test.OrdinalEquals("Mechanical.Tests.Core.SafeStringTests.Nested1.Nested2<T>.INested3<T>", SafeString.DebugPrint(typeof(SafeStringTests.Nested1.Nested2<>.INested3)));
            Test.OrdinalEquals("Mechanical.Tests.Core.SafeStringTests.Nested1.Nested2<T>.INested3<int>", SafeString.DebugPrint(typeof(SafeStringTests.Nested1.Nested2<int>.INested3)));

            // array types
            Test.OrdinalEquals("int[]", SafeString.DebugPrint(typeof(int[])));
            Test.OrdinalEquals("int[,,]", SafeString.DebugPrint(typeof(int[,,])));
            Test.OrdinalEquals("IEnumerable<int>[,,,][][,,]", SafeString.DebugPrint(typeof(IEnumerable<int>[,,,][][,,])));

            // parameters
            Test.OrdinalEquals("int p0", SafeString.DebugPrint(typeof(SafeStringTests).GetMethod("ParamTestMethod").GetParameters()[0]));
            Test.OrdinalEquals("out float p1", SafeString.DebugPrint(typeof(SafeStringTests).GetMethod("ParamTestMethod").GetParameters()[1]));
            Test.OrdinalEquals("ref byte p2", SafeString.DebugPrint(typeof(SafeStringTests).GetMethod("ParamTestMethod").GetParameters()[2]));
            Test.OrdinalEquals("params object[] p3", SafeString.DebugPrint(typeof(SafeStringTests).GetMethod("ParamTestMethod").GetParameters()[3]));

            // methods, constructors
            Test.OrdinalEquals("void Mechanical.Tests.Core.SafeStringTests.ParamTestMethod(int p0, out float p1, ref byte p2, params object[] p3)", SafeString.DebugPrint(typeof(SafeStringTests).GetMethod("ParamTestMethod")));
            var genericTestMethod_GenericDefinition = typeof(SafeStringTests).GetMethod("GenericTestMethod").GetGenericMethodDefinition();
            Test.OrdinalEquals("void Mechanical.Tests.Core.SafeStringTests.GenericTestMethod<T1, T2>()", SafeString.DebugPrint(genericTestMethod_GenericDefinition));
            Test.OrdinalEquals("void Mechanical.Tests.Core.SafeStringTests.GenericTestMethod<int, float>()", SafeString.DebugPrint(genericTestMethod_GenericDefinition.MakeGenericMethod(typeof(int), typeof(float))));
            Test.OrdinalEquals("void Mechanical.Tests.Core.SafeStringTests.GenericTestMethod<int, T2>()", SafeString.DebugPrint(genericTestMethod_GenericDefinition.MakeGenericMethod(typeof(int), genericTestMethod_GenericDefinition.GetGenericArguments()[1])));
            Test.OrdinalEquals("Mechanical.Tests.Core.SafeStringTests.Nested1.ctor()", SafeString.DebugPrint(typeof(SafeStringTests.Nested1).GetConstructors().Where(ctor => ctor.GetParameters().Length == 0).First()));
            Test.OrdinalEquals("Mechanical.Tests.Core.SafeStringTests.Nested1.ctor(int i)", SafeString.DebugPrint(typeof(SafeStringTests.Nested1).GetConstructors().Where(ctor => ctor.GetParameters().Length == 1).First()));
        }

        #endregion

        #region TryFormat, Format

        [TestCase(NullProvider, "", new object[] { }, ""),
        TestCase(NullProvider, "asd", new object[] { }, "asd"),
        TestCase(NullProvider, "a{{b", new object[] { }, "a{b"),
        TestCase(NullProvider, "}}", new object[] { }, "}"),
        TestCase(InvariantCulture, "{0}", new object[] { 3 }, "3"),
        TestCase(InvariantCulture, "{0,3}", new object[] { 3 }, "  3"),
        TestCase(InvariantCulture, "{0,-3}", new object[] { 3 }, "3  "),
        TestCase(InvariantCulture, "{0,1}", new object[] { 99 }, "99"),
        TestCase(InvariantCulture, "{0:x}", new object[] { 17 }, "11"),
        TestCase(InvariantCulture, "{0,3:x}", new object[] { 17 }, " 11")]
        public static void FormatSuccessTest( int cultureCode, string format, object[] args, string result )
        {
            var culture = GetCulture(cultureCode);

            string formatResult = string.Format(culture, format, args);
            Test.OrdinalEquals(result, formatResult);

            string tryFormatResult;
            Assert.True(SafeString.TryFormat(out tryFormatResult, culture, format, args));
            Test.OrdinalEquals(result, tryFormatResult);
        }

        [Test]
        public static void FormatFailureTest()
        {
            TestException<ArgumentNullException>(CultureInfo.InvariantCulture, "{0}", null, string.Empty);
            TestException<ArgumentNullException>(CultureInfo.InvariantCulture, null, new object[] { 5 }, string.Empty);
            TestException<FormatException>(CultureInfo.InvariantCulture, "a{b", new object[] { }, "ab");
            TestException<FormatException>(CultureInfo.InvariantCulture, "a}b", new object[] { }, "ab");
            TestException<FormatException>(CultureInfo.InvariantCulture, "a{!!!}b", new object[] { }, "ab");
            TestException<FormatException>(CultureInfo.InvariantCulture, "a{-1}b", new object[] { }, "ab");
            TestException<FormatException>(CultureInfo.InvariantCulture, "a{0}b{1}c", new object[] { 5 }, "a5bc");
            TestException<FormatException>(CultureInfo.InvariantCulture, "a{0,!!!}b", new object[] { 5 }, "a5b");
        }

        #endregion
    }
}
