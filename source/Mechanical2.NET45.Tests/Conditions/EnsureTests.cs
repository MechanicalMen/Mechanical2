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
            Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context.SourcePos.File));
            Assert.AreEqual(14, context.SourcePos.Line);

            var context2 = Ensure.That(3.14);
            Assert.NotNull(context2);
            Assert.AreEqual(3.14, context2.Object);
            Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context2.SourcePos.File));
            Assert.AreEqual(20, context2.SourcePos.Line);
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
                    Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context.SourcePos.File));
                    Assert.AreEqual(30, context.SourcePos.Line);
                });

            Ensure.Debug(
                3.14,
                context =>
                {
                    Assert.NotNull(context);
                    Assert.AreEqual(3.14, context.Object);
                    Test.OrdinalEquals("EnsureTests.cs", Path.GetFileName(context.SourcePos.File));
                    Assert.AreEqual(40, context.SourcePos.Line);
                });
        }
    }
}
