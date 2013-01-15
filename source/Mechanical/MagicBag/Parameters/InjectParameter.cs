using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MagicBag.Parameters
{
    /// <summary>
    /// An <see cref="IMagicBagParameter"/> that represents resolving a type (from a magic bag).
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are very small, and share most of their code.")]
    public class InjectParameter<T> : IMagicBagParameter
    {
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
            return CreatePullExpression(magicBagExpression);
        }

        #endregion

        #region Internal Static Methods

        internal static Expression CreatePullExpression( Expression magicBagExpression )
        {
            Ensure.That(magicBagExpression).NotNull();

            return Expression.Call(
                magicBagExpression,
                Reveal.Method<IMagicBag>(bag => bag.Pull<T>()));
        }

        #endregion
    }

    /// <summary>
    /// An <see cref="IMagicBagParameter"/> that represents resolving a type (from a magic bag).
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are very small, and share most of their code.")]
    public class InjectParameter : IMagicBagParameter
    {
        #region Private Fields

        private readonly Type type;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectParameter"/> class.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        public InjectParameter( Type type )
        {
            Ensure.That(type).NotNull();

            this.type = type;
        }

        #endregion

        #region IMagicBagParameter

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public Type ParameterType
        {
            get { return this.type; }
        }

        /// <summary>
        /// Converts this instance into an <see cref="Expression"/>.
        /// </summary>
        /// <param name="magicBagExpression">An expression that evaluates to the magic bag.</param>
        /// <returns>The <see cref="Expression"/> representing this parameter.</returns>
        public Expression ToExpression( Expression magicBagExpression )
        {
            return CreatePullExpression(magicBagExpression, this.type);
        }

        #endregion

        #region Internal Static Methods

        internal static Expression CreatePullExpression( Expression magicBagExpression, Type type )
        {
            Ensure.That(magicBagExpression).NotNull();

            var method = Reveal.Method<IMagicBag>(bag => bag.Pull<int>());
            method = method.GetGenericMethodDefinition();
            method = method.MakeGenericMethod(type);

            return Expression.Call(
                magicBagExpression,
                method);
        }

        #endregion
    }
}
