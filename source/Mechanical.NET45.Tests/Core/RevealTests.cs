using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mechanical.Core;
using NUnit.Framework;

namespace Mechanical.Tests.Core
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "All elements are empty, and only there to be reflected upon by this class.")]
    public class RevealTests
    {
        #region Field

        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "The functionality needs to be tested.")]
        public int Field;

        [Test]
        public void TestField()
        {
            var member = Reveal.Info(() => (object)this.Field); // test for casting operation as well
            Assert.NotNull(member);
            Test.OrdinalEquals("Field", member.Name);
            Assert.IsInstanceOf<FieldInfo>(member);

            var field = Reveal.Field(() => this.Field);
            Assert.NotNull(field);
            Test.OrdinalEquals("Field", field.Name);

            Test.OrdinalEquals("Field", Reveal.Name(() => this.Field));


            member = Reveal.Info(( RevealTests rt ) => (object)rt.Field); // test for casting operation as well
            Assert.NotNull(member);
            Test.OrdinalEquals("Field", member.Name);
            Assert.IsInstanceOf<FieldInfo>(member);

            field = Reveal.Field(( RevealTests rt ) => rt.Field);
            Assert.NotNull(field);
            Test.OrdinalEquals("Field", field.Name);

            Test.OrdinalEquals("Field", Reveal.Name(( RevealTests rt ) => this.Field));
        }

        #endregion

        #region Property

        public float Property { get { return 0; } }
        public string this[int index] { get { return null; } }

        [Test]
        public void TestProperty()
        {
            var member = Reveal.Info(() => this.Property);
            Assert.NotNull(member);
            Test.OrdinalEquals("Property", member.Name);
            Assert.IsInstanceOf<PropertyInfo>(member);

            var property = Reveal.Property(() => this.Property);
            Assert.NotNull(property);
            Test.OrdinalEquals("Property", property.Name);

            Test.OrdinalEquals("Property", Reveal.Name(() => this.Property));


            member = Reveal.Info(( RevealTests t ) => t.Property);
            Assert.NotNull(member);
            Test.OrdinalEquals("Property", member.Name);
            Assert.IsInstanceOf<PropertyInfo>(member);

            property = Reveal.Property(( RevealTests t ) => t.Property);
            Assert.NotNull(property);
            Test.OrdinalEquals("Property", property.Name);

            Test.OrdinalEquals("Property", Reveal.Name(( RevealTests t ) => t.Property));


            member = Reveal.Info(() => this[0]);
            Assert.NotNull(member);
            Test.OrdinalEquals("get_Item", member.Name);
            Assert.IsInstanceOf<MethodInfo>(member);

            var method = Reveal.Method(() => this[0]);
            Assert.NotNull(property);
            Test.OrdinalEquals("get_Item", method.Name);

            Test.OrdinalEquals("get_Item", Reveal.Name(() => this[0]));
        }

        #endregion

        #region Method

        public void Action0() { }
        public void Action1( int p1 ) { }
        public void Action2( int p1, int p2 ) { }

        public int Func0() { return 0; }
        public int Func1( int p1 ) { return 0; }
        public int Func2( int p1, int p2 ) { return 0; }

        public void GenericMethod<T>() { }

        private void TestMethodInfo( MethodInfo method, string methodName )
        {
            Assert.NotNull(method);
            Test.OrdinalEquals(methodName, method.Name);
        }

        [Test]
        public void TestMethods()
        {
            var member = Reveal.Info<RevealTests>(rt => rt.Action0());
            Assert.NotNull(member);
            Test.OrdinalEquals("Action0", member.Name);
            Assert.IsInstanceOf<MethodInfo>(member);

            this.TestMethodInfo(Reveal.Method<RevealTests>(rt => rt.Action0()), "Action0");
            this.TestMethodInfo(Reveal.Method<RevealTests>(rt => rt.Action1(0)), "Action1");
            this.TestMethodInfo(Reveal.Method<RevealTests>(rt => rt.Action2(0, 0)), "Action2");
            Test.OrdinalEquals(Reveal.Name<RevealTests>(rt => rt.Action0()), "Action0");
            Test.OrdinalEquals(Reveal.Name<RevealTests>(rt => rt.Action1(0)), "Action1");
            Test.OrdinalEquals(Reveal.Name<RevealTests>(rt => rt.Action2(0, 0)), "Action2");


            this.TestMethodInfo(Reveal.Method(( RevealTests rt ) => rt.Func0()), "Func0");
            this.TestMethodInfo(Reveal.Method(( RevealTests rt ) => rt.Func1(0)), "Func1");
            this.TestMethodInfo(Reveal.Method(( RevealTests rt ) => rt.Func2(0, 0)), "Func2");
            Test.OrdinalEquals(Reveal.Name(( RevealTests rt ) => rt.Func0()), "Func0");
            Test.OrdinalEquals(Reveal.Name(( RevealTests rt ) => rt.Func1(0)), "Func1");
            Test.OrdinalEquals(Reveal.Name(( RevealTests rt ) => rt.Func2(0, 0)), "Func2");

            member = Reveal.Info(( RevealTests rt ) => rt.Func0());
            Assert.NotNull(member);
            Test.OrdinalEquals("Func0", member.Name);
            Assert.IsInstanceOf<MethodInfo>(member);


            this.TestMethodInfo(Reveal.Method(() => this.Action0()), "Action0");
            this.TestMethodInfo(Reveal.Method(() => this.Func1(0)), "Func1");
            Test.OrdinalEquals(Reveal.Name(() => this.Action0()), "Action0");
            Test.OrdinalEquals(Reveal.Name(() => this.Func1(0)), "Func1");


            this.TestMethodInfo(Reveal.Method(( int notUsed ) => this.GenericMethod<float>()), "GenericMethod");
            Test.OrdinalEquals(Reveal.Name(( int notUsed ) => this.GenericMethod<float>()), "GenericMethod");

            member = Reveal.Info(( int notUsed ) => this.GenericMethod<float>());
            Assert.NotNull(member);
            Test.OrdinalEquals("GenericMethod", member.Name);
            Assert.IsInstanceOf<MethodInfo>(member);
        }

        #endregion

        #region Constructor

        private class TestType
        {
            public TestType() { }

            public TestType( int i ) { }

            public TestType( int i1, int i2 ) { }
        }

        [Test]
        public void TestConstructors()
        {
            var info = Reveal.Constructor(() => new TestType());
            Assert.NotNull(info);
            Assert.AreEqual(0, info.GetParameters().Length);

            info = Reveal.Constructor(() => new TestType(0));
            Assert.NotNull(info);
            Assert.AreEqual(1, info.GetParameters().Length);

            info = Reveal.Constructor(() => new TestType(0, 0));
            Assert.NotNull(info);
            Assert.AreEqual(2, info.GetParameters().Length);


            Test.OrdinalEquals(Reveal.Name(() => new TestType()), ".ctor");
            Test.OrdinalEquals(Reveal.Name(() => new TestType(0)), ".ctor");
            Test.OrdinalEquals(Reveal.Name(() => new TestType(0, 0)), ".ctor");


            var member = Reveal.Info(() => new TestType(0, 0));
            Assert.NotNull(member);
            Test.OrdinalEquals(".ctor", member.Name);
            Assert.IsInstanceOf<ConstructorInfo>(member);
        }

        #endregion
    }
}
