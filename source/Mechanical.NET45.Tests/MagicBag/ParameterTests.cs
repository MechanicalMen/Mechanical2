using System;
using System.Linq.Expressions;
using Mechanical.MagicBag;
using Mechanical.MagicBag.Parameters;
using NUnit.Framework;

namespace Mechanical.Tests.MagicBag
{
    [TestFixture]
    public class ParameterTests
    {
        private static object InvokeParam( IMagicBagParameter parameter, IMagicBag magicBag )
        {
            var magicPagExpr = Expression.Constant(magicBag);
            var body = parameter.ToExpression(magicPagExpr);
            body = Expression.Convert(body, typeof(object));
            var func = Expression.Lambda<Func<object>>(body).Compile();
            return func();
        }

        [Test]
        public void Tests()
        {
            Assert.AreEqual(5, InvokeParam(ConstParameter.From(5), MagicBagTests.EmptyBag));

            Assert.AreEqual(6, InvokeParam(new DelegateParameter<int>(() => 6), MagicBagTests.EmptyBag));
            Assert.AreEqual(true, InvokeParam(new DelegateParameter<bool>(bag => object.ReferenceEquals(bag, MagicBagTests.EmptyBag)), MagicBagTests.EmptyBag));

            var magicBag = new Mechanical.MagicBag.MagicBag.Basic(Map<int>.To<int>(() => 7).AsTransient());
            Assert.AreEqual(7, InvokeParam(new InjectParameter<int>(), magicBag));
            Assert.AreEqual(7, InvokeParam(new InjectParameter(typeof(int)), magicBag));
        }
    }
}
