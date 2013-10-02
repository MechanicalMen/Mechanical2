using System;
using System.Linq.Expressions;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MagicBag.Parameters
{
    /// <summary>
    /// An <see cref="IMagicBagParameter"/> that represents a delegate.
    /// </summary>
    /// <typeparam name="T">The return type of the specified delegate.</typeparam>
    public class DelegateParameter<T> : IMagicBagParameter
    {
        #region Private Fields

        private readonly Func<IMagicBag, T> method;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateParameter{T}"/> class.
        /// </summary>
        /// <param name="method">The delegate to wrap.</param>
        public DelegateParameter( Func<IMagicBag, T> method )
        {
            Ensure.That(method).NotNull();

            this.method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateParameter{T}"/> class.
        /// </summary>
        /// <param name="method">The delegate to wrap.</param>
        public DelegateParameter( Func<T> method )
            : this(bag => method())
        {
            Ensure.That(method).NotNull();
        }

        #endregion

        #region IMagicBagParameter

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public Type ParameterType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Converts this instance into an <see cref="Expression"/>.
        /// </summary>
        /// <param name="magicBagExpression">An expression that evaluates to the magic bag.</param>
        /// <returns>The <see cref="Expression"/> representing this parameter.</returns>
        public Expression ToExpression( Expression magicBagExpression )
        {
            Ensure.That(magicBagExpression).NotNull();

            return Expression.Call(Expression.Constant(this), Reveal.Method(() => this.InvokeDelegate(null)), magicBagExpression);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invokes the wrapped delegate.
        /// </summary>
        /// <param name="magicBag">The magic bag to use for evaluating the parameter.</param>
        /// <returns>The object returned by the delegate.</returns>
        public T InvokeDelegate( IMagicBag magicBag )
        {
            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreFileLine();

            return this.method(magicBag);
        }

        #endregion
    }
}
