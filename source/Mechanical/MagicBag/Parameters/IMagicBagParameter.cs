using System;
using System.Linq.Expressions;

namespace Mechanical.MagicBag.Parameters
{
    /// <summary>
    /// Specifies a parameter to be used for injection.
    /// </summary>
    public interface IMagicBagParameter
    {
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        Type ParameterType { get; }

        /// <summary>
        /// Converts this instance into an <see cref="Expression"/>.
        /// </summary>
        /// <param name="magicBagExpression">An expression that evaluates to the magic bag.</param>
        /// <returns>The <see cref="Expression"/> representing this parameter.</returns>
        Expression ToExpression( Expression magicBagExpression );
    }
}
