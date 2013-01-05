using System;
using NUnit.Framework;

namespace Mechanical.Tests
{
    internal static class Test
    {
        internal static void OrdinalEquals( string expected, string actual )
        {
            Assert.True(string.Equals(expected, actual, StringComparison.Ordinal), message: string.Format("Expected: \"{1}\"{0}  Actual:   \"{2}\"{0}", Environment.NewLine, expected, actual));
        }
    }
}
