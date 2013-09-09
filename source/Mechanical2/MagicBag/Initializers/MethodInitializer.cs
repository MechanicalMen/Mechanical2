using System;
using System.Linq.Expressions;
using System.Reflection;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Parameters;

namespace Mechanical.MagicBag.Initializers
{
    /// <summary>
    /// Invokes a method of newly allocated objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to initialize.</typeparam>
    public class MethodInitializer<T> : IMagicBagInitializer<T>
    {
        #region Private Fields

        private readonly Func<T, IMagicBag, T> invoke;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInitializer{T}"/> class.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="parameters">The parameters to invoke the method with.</param>
        /// <param name="useMethodResult">If <c>true</c>, returns the method's return values instead of the allocated objects.</param>
        public MethodInitializer( MethodInfo method, MagicParameters parameters = null, bool useMethodResult = false )
        {
            Ensure.That(method).NotNull();

            var uninitializedExpr = Expression.Parameter(typeof(T));
            var bagExpr = Expression.Parameter(typeof(IMagicBag));

            var methodParamExprs = MagicParameters.GetExpressions(method, parameters, bagExpr);
            var methodCallExpr = Expression.Call(uninitializedExpr, method, methodParamExprs);

            Expression bodyExpr;
            if( useMethodResult )
            {
                //// NOTE: we generate the following code:
                //// return uninitialized.Method(<parameters>);
                if( Reveal.CanAssignTo(typeof(T), method.ReturnType) )
                    bodyExpr = methodCallExpr;
                else
                    throw new InvalidOperationException("The return type of the method is not convertible to the necessary type.").Store("T", typeof(T)).Store("ReturnType", method.ReturnType);
            }
            else
            {
                //// NOTE: we generate the following code:
                //// {
                ////     uninitialized.Method(<parameters>);
                ////     return uninitialized;
                //// }

                var returnTarget = Expression.Label(typeof(T));
                bodyExpr = Expression.Block(
                    typeof(T),
                    methodCallExpr,
                    Expression.Return(returnTarget, uninitializedExpr),
                    Expression.Label(returnTarget, Expression.Constant(default(T), typeof(T))));
            }

            var lambdaExpr = Expression.Lambda<Func<T, IMagicBag, T>>(bodyExpr, uninitializedExpr, bagExpr);
            this.invoke = lambdaExpr.Compile();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MethodInitializer{T}"/> class.
        /// All parameters will be resolved using a magic bag.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="useMethodResult">If <c>true</c>, returns the method's return values instead of the allocated objects.</param>
        /// <returns>The <see cref="MethodInitializer{T}"/> created.</returns>
        public static MethodInitializer<T> InjectAll( MethodInfo method, bool useMethodResult = false )
        {
            Ensure.That(method).NotNull();

            return new MethodInitializer<T>(method, MagicParameters.InjectAllOf(method), useMethodResult);
        }

        #endregion

        #region IMagicBagInitializer

        /// <summary>
        /// Initializes the newly allocated object.
        /// </summary>
        /// <param name="allocated">The object just allocated.</param>
        /// <param name="magicBag">The magic bag to use.</param>
        /// <returns>The initialized object.</returns>
        public T Initialize( T allocated, IMagicBag magicBag )
        {
            if( Reveal.IsByRef<T>()
             && ((object)allocated).NotNullReference() )
                throw new ArgumentNullException("allocated").StoreDefault();

            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreDefault();

            return this.invoke(allocated, magicBag);
        }

        #endregion
    }
}
