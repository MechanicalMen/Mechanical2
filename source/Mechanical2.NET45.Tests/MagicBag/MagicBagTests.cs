using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mechanical.MagicBag;
using NUnit.Framework;

namespace Mechanical.Tests.MagicBag
{
    [TestFixture]
    public class MagicBagTests
    {
        internal static readonly IMagicBag EmptyBag = new Mechanical.MagicBag.MagicBag.Basic();

        [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "New lines would waste a lot of screen space.")]
        internal class IntWrapper
        {
            public IntWrapper() : this(0) { }
            public IntWrapper( int value ) { this.Value = value; }
            public int Value { get; set; }
            public void AddAssign( int value ) { this.Value += value; }
            public IntWrapper Add( int value ) { return new IntWrapper(this.Value + value); }
        }

        internal static int GetIntFromWrapper( object obj )
        {
            var wrapper = (IntWrapper)obj;
            return wrapper.Value;
        }

        [Test]
        public void BasicTests()
        {
            var bag = new Mechanical.MagicBag.MagicBag.Basic(
                Map<int>.To(() => 5).AsTransient(),
                Map<float>.To(() => 3.14f).AsTransient());

            Assert.True(bag.IsRegistered<int>());
            Assert.True(bag.IsRegistered<float>());
            Assert.False(bag.IsRegistered<object>());

            Assert.AreEqual(5, bag.Pull<int>());
            Assert.AreEqual(3.14f, bag.Pull<float>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<object>());
        }

        [Test]
        public void InheritTests()
        {
            var parentBag = new Mechanical.MagicBag.MagicBag.Basic(
                Map<int>.To(() => 5).AsTransient(),
                Map<float>.To(() => 3.14f).AsTransient());

            var bag = new Mechanical.MagicBag.MagicBag.Inherit(
                parentBag,
                Map<int>.To(() => 6).AsTransient(),
                Map<string>.To(() => "abc").AsTransient());

            Assert.True(bag.IsRegistered<int>());
            Assert.True(bag.IsRegistered<float>());
            Assert.True(bag.IsRegistered<string>());
            Assert.False(bag.IsRegistered<object>());

            Assert.AreEqual(6, bag.Pull<int>());
            Assert.AreEqual(3.14f, bag.Pull<float>());
            Test.OrdinalEquals("abc", bag.Pull<string>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<object>());
        }

        [Test]
        public void SupplementTests()
        {
            var parentBag = new Mechanical.MagicBag.MagicBag.Basic(
                Map<int>.To(() => 5).AsTransient(),
                Map<float>.To(() => 3.14f).AsTransient());

            var bag = new Mechanical.MagicBag.MagicBag.Extend(
                parentBag,
                Map<int>.To(() => 6).AsTransient(),
                Map<string>.To(() => "abc").AsTransient());

            Assert.True(bag.IsRegistered<int>());
            Assert.True(bag.IsRegistered<float>());
            Assert.True(bag.IsRegistered<string>());
            Assert.False(bag.IsRegistered<object>());

            Assert.AreEqual(5, bag.Pull<int>());
            Assert.AreEqual(3.14f, bag.Pull<float>());
            Test.OrdinalEquals("abc", bag.Pull<string>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<object>());
        }

        [Test]
        public void BlacklistTests()
        {
            var parentBag = new Mechanical.MagicBag.MagicBag.Basic(
                Map<int>.To(() => 5).AsTransient(),
                Map<float>.To(() => 3.14f).AsTransient());

            var bag = new Mechanical.MagicBag.MagicBag.Blacklist(
                parentBag,
                typeof(int),
                typeof(string));

            Assert.True(bag.IsRegistered<float>());
            Assert.False(bag.IsRegistered<int>());
            Assert.False(bag.IsRegistered<string>());
            Assert.False(bag.IsRegistered<object>());

            Assert.AreEqual(3.14f, bag.Pull<float>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<int>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<string>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<object>());
        }

        [Test]
        public void WhitelistTests()
        {
            var parentBag = new Mechanical.MagicBag.MagicBag.Basic(
                Map<int>.To(() => 5).AsTransient(),
                Map<float>.To(() => 3.14f).AsTransient());

            var bag = new Mechanical.MagicBag.MagicBag.Whitelist(
                parentBag,
                typeof(int),
                typeof(string));

            Assert.True(bag.IsRegistered<int>());
            Assert.False(bag.IsRegistered<float>());
            Assert.False(bag.IsRegistered<string>());
            Assert.False(bag.IsRegistered<object>());

            Assert.AreEqual(5, bag.Pull<int>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<float>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<string>());
            Assert.Throws<KeyNotFoundException>(() => bag.Pull<object>());
        }
    }
}
