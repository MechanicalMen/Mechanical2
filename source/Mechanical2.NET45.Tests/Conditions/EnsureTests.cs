using System;
using System.IO;
using Mechanical.Conditions;
using NUnit.Framework;

namespace Mechanical.Tests.Conditions
{
    [TestFixture]
    public class ObjectTests
    {
        [Test]
        public void That()
        {
            var context = Ensure.That(5);
            Assert.NotNull(context);
            Assert.AreEqual(5, context.Object);
            Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context.FilePath));
            Assert.AreEqual(14, context.LineNumber);

            var context2 = Ensure.That(3.14);
            Assert.NotNull(context2);
            Assert.AreEqual(3.14, context2.Object);
            Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context2.FilePath));
            Assert.AreEqual(20, context2.LineNumber);
        }

        [Test]
        public void Debug()
        {
            Ensure.Debug(
                5,
                context =>
                {
                    Assert.NotNull(context);
                    Assert.AreEqual(5, context.Object);
                    Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context.FilePath));
                    Assert.AreEqual(30, context.LineNumber);
                });

            Ensure.Debug(
                3.14,
                context =>
                {
                    Assert.NotNull(context);
                    Assert.AreEqual(3.14, context.Object);
                    Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context.FilePath));
                    Assert.AreEqual(40, context.LineNumber);
                });
        }
    }
}
