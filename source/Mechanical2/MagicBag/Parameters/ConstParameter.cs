using System;
using System.Linq.Expressions;
using Mechanical.Conditions;

namespace Mechanical.MagicBag.Parameters
{
    /// <summary>
    /// An <see cref="IMagicBagParameter"/> that represents a constant.
    /// </summary>
    public class ConstParameter : IMagicBagParameter
    {
        #region Private Fields

        private readonly Type parameterType;
        private readonly object constant;

        #endregion

        #region Constructor

        private ConstParameter( Type type, object constant )
        {
            this.parameterType = type;
            this.constant = constant;
        }

        /// <summary>
        /// Wraps the specified constant.
        /// </summary>
        /// <typeparam name="T">The type of the constant to wrap.</typeparam>
        /// <param name="constant">The constant to wrap.</param>
        /// <returns>The new <see cref="ConstParameter"/> instant representing the specified constant.</returns>
        public static ConstParameter From<T>( T constant )
        {
            return new ConstParameter(typeof(T), constant);
        }

        #endregion

        #region IMagicBagParameter

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public Type ParameterType
        {
            get { return this.parameterType; }
        }

        /// <summary>
        /// Converts this instance into an <see cref="Expression"/>.
        /// </summary>
        /// <param name="magicBagExpression">An expression that evaluates to the magic bag.</param>
        /// <returns>The <see cref="Expression"/> representing this parameter.</returns>
        public Expression ToExpression( Expression magicBagExpression )
        {
            Ensure.That(magicBagExpression).NotNull();

            return Expression.Constant(this.constant, this.parameterType);
        }

        #endregion
    }
}
