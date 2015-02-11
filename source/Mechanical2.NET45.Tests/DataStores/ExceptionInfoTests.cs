using System;
using Mechanical.DataStores;
using Mechanical.DataStores.Xml;
using Mechanical.IO;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores
{
    [TestFixture]
    public class ExceptionInfoTests
    {
        private const string ExceptionObjectName = "Exception";
        private const string ExceptionType = "TestException";
        private const string ExceptionMessage = " Test message.  ";
        private const string ExceptionStackTrace = "   at TestClass.TestMethod()";
        private const string TestValue1Key = "TestValue1";
        private const string TestValue1Value = @" Test value 1 (~) ";
        private const string TestValue2Key = "TestValue2";
        private const string TestValue2Value = "";
        private const string TestValue3Key = "TestValue3";
        private const string TestValue3Value = null;

        private const string Version1Xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <Exception>
    <Type>TestException</Type>
    <Message> Test message.  </Message>
    <StackTrace>   at TestClass.TestMethod()</StackTrace>
    <Store>
      <TestValue1>_ Test value 1 (~) </TestValue1>
      <TestValue2>_</TestValue2>
      <TestValue3/>
    </Store>
    <InnerExceptions></InnerExceptions>
  </Exception>
</root>";

        private const string Version2Xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <Exception>
    <FormatVersion>2</FormatVersion>
    <Type>TestException</Type>
    <Message> Test message.  </Message>
    <StackTrace>   at TestClass.TestMethod()</StackTrace>
    <Store>
      <TestValue1> Test value 1 (~~) </TestValue1>
      <TestValue2 />
      <TestValue3>~</TestValue3>
    </Store>
    <InnerExceptions></InnerExceptions>
  </Exception>
</root>";

        [Test]
        public void Version1Test()
        {
            using( var reader = new XmlDataStoreReader(new StringReader(Version1Xml)) )
            {
                var info = reader.Read(ExceptionInfo.Serializer.Default, ExceptionObjectName);
                Assert.NotNull(info);
                Assert.True(string.Equals(info.Type, ExceptionType, StringComparison.Ordinal));
                Assert.True(string.Equals(info.Message, ExceptionMessage, StringComparison.Ordinal));
                Assert.True(string.Equals(info.StackTrace, ExceptionStackTrace, StringComparison.Ordinal));
                Assert.AreEqual(3, info.Store.Count);
                Assert.True(string.Equals(info.Store[TestValue1Key], TestValue1Value, StringComparison.Ordinal));
                Assert.True(string.Equals(info.Store[TestValue2Key], TestValue2Value, StringComparison.Ordinal));
                Assert.True(string.Equals(info.Store[TestValue3Key], TestValue3Value, StringComparison.Ordinal));
                Assert.AreEqual(0, info.InnerExceptions.Count);
                Assert.Null(info.InnerException);
            }
        }

        [Test]
        public void Version2Test()
        {
            using( var reader = new XmlDataStoreReader(new StringReader(Version2Xml)) )
            {
                var info = reader.Read(ExceptionInfo.Serializer.Default, ExceptionObjectName);
                Assert.NotNull(info);
                Assert.True(string.Equals(info.Type, ExceptionType, StringComparison.Ordinal));
                Assert.True(string.Equals(info.Message, ExceptionMessage, StringComparison.Ordinal));
                Assert.True(string.Equals(info.StackTrace, ExceptionStackTrace, StringComparison.Ordinal));
                Assert.AreEqual(3, info.Store.Count);
                Assert.True(string.Equals(info.Store[TestValue1Key], TestValue1Value, StringComparison.Ordinal));
                Assert.True(string.Equals(info.Store[TestValue2Key], TestValue2Value, StringComparison.Ordinal));
                Assert.True(string.Equals(info.Store[TestValue3Key], TestValue3Value, StringComparison.Ordinal));
                Assert.AreEqual(0, info.InnerExceptions.Count);
                Assert.Null(info.InnerException);
            }
        }
    }
}
