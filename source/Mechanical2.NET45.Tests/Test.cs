using System;
using System.IO;
using Mechanical.Core;
using Mechanical.DataStores;
using NUnit.Framework;

namespace Mechanical.Tests
{
    internal static class Test
    {
        internal class App : AppEssentials
        {
            protected App()
                : base()
            {
            }

            public static new Test.App Instance
            {
                get { return (Test.App)AppEssentials.Instance; }
            }

            public static void Initialize()
            {
                if( Instance.NullReference() )
                {
                    new Test.App();

                    Instance.SetupReadOnlyMemory();
                    Instance.InitializeUIForConsole();
                    Instance.InitializeMagicBag();
                }
            }
        }

        internal static void OrdinalEquals( string expected, string actual )
        {
            Assert.True(string.Equals(expected, actual, StringComparison.Ordinal), message: string.Format("Expected: \"{1}\"{0}  Actual:   \"{2}\"{0}", Environment.NewLine, expected, actual));
        }

        internal static void OrdinalEquals( string expected, Substring actual )
        {
            OrdinalEquals(expected, actual.ToString());
        }

        internal static void OrdinalEquals( Substring expected, string actual )
        {
            OrdinalEquals(expected.ToString(), actual);
        }

        internal static void OrdinalEquals( Substring expected, Substring actual )
        {
            OrdinalEquals(expected.ToString(), actual.ToString());
        }

        internal static string ReplaceNewLines( string str )
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
    }
}
