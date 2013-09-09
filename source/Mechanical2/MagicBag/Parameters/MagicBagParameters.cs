using System;
using System.Linq.Expressions;
using System.Reflection;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MagicBag.Parameters
{
    /// <summary>
    /// A set of parameters, used to configure magic bags.
    /// </summary>
    public class MagicParameters : ReadOnlyList.Wrapper<IMagicBagParameter>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicParameters"/> class.
        /// </summary>
        public MagicParameters()
            : base()
        {
        }

        /// <summary>
        /// Helps specifying a set of parameters.
        /// </summary>
        /// <param name="parameters">The parameters to invoke the constructor with.</param>
        /// <returns>The parameters specified; or <c>null</c>.</returns>
        public static MagicParameters From( Action<MagicParameters> parameters = null )
        {
            MagicParameters magicParams = null;

            if( parameters.NotNullReference() )
            {
                magicParams = new MagicParameters();
                parameters(magicParams);
            }

            return magicParams;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Specifies a new parameter.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        /// <returns>The parameters that have been specified.</returns>
        public MagicParameters Add( IMagicBagParameter parameter )
        {
            Ensure.That(parameter).NotNull();

            this.Items.Add(parameter);
            return this;
        }

        /// <summary>
        /// Specifies a new constant parameter.
        /// </summary>
        /// <typeparam name="T">The type of the constant.</typeparam>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The parameters that have been specified.</returns>
        public MagicParameters Const<T>( T value )
        {
            return this.Add(ConstParameter.From<T>(value));
        }

        /// <summary>
        /// Specifies a new injection parameter.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The parameters that have been specified.</returns>
        public MagicParameters Inject<T>()
        {
            return this.Add(new InjectParameter<T>());
        }

        /// <summary>
        /// Specifies a new injection parameter.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The parameters that have been specified.</returns>
        public MagicParameters Inject( Type type )
        {
            return this.Add(new InjectParameter(type));
        }

        /// <summary>
        /// Specifies a new delegate parameter.
        /// </summary>
        /// <typeparam name="T">The return type of the specified delegate.</typeparam>
        /// <param name="method">The delegate to wrap.</param>
        /// <returns>The parameters that have been specified.</returns>
        public MagicParameters Func<T>( Func<IMagicBag, T> method )
        {
            return this.Add(new DelegateParameter<T>(method));
        }

        /// <summary>
        /// Specifies a new delegate parameter.
        /// </summary>
        /// <typeparam name="T">The return type of the specified delegate.</typeparam>
        /// <param name="method">The delegate to wrap.</param>
        /// <returns>The parameters that have been specified.</returns>
        public MagicParameters Func<T>( Func<T> method )
        {
            return this.Add(new DelegateParameter<T>(method));
        }

        #endregion

        #region Internal Methods

        internal Expression[] GetExpressions( Expression magicBagExpression )
        {
            var parameterExpressions = new Expression[this.Count];

            for( int i = 0; i < parameterExpressions.Length; ++i )
                parameterExpressions[i] = this[i].ToExpression(magicBagExpression);

            return parameterExpressions;
        }

        #endregion

        #region Internal Static Methods

        internal static Expression[] GetExpressions( MethodBase methodBase, MagicParameters parameters, Expression magicBagExpression )
        {
            Ensure.Debug(methodBase, m => m.NotNull());
            Ensure.Debug(magicBagExpression, b => b.NotNull());

            var methodBaseParameters = methodBase.GetParameters();
            if( methodBaseParameters.Length == 0 )
            {
                if( parameters.NotNullReference()
                 && parameters.Count != 0 )
                    throw new ArgumentException("Invalid number of parameters!").Store("NumExpectedParameters", 0).Store("NumActualParameters", parameters.Count);

                return null;
            }
            else
            {
                if( parameters.NullReference()
                 || parameters.Count != methodBaseParameters.Length )
                    throw new ArgumentException("Invalid number of parameters!").Store("NumExpectedParameters", methodBaseParameters.Length).Store("NumActualParameters", parameters.NullReference() ? 0 : parameters.Count);

                return parameters.GetExpressions(magicBagExpression);
            }
        }

        internal static MagicParameters InjectAllOf( MethodBase methodBase )
        {
            Ensure.Debug(methodBase, m => m.NotNull());

            MagicParameters magicParams = null;
            var methodBaseParameters = methodBase.GetParameters();
            if( methodBaseParameters.Length != 0 )
            {
                magicParams = new MagicParameters();
                for( int i = 0; i < methodBaseParameters.Length; ++i )
                {
                    var p = methodBaseParameters[i];
                    magicParams.Inject(p.ParameterType);
                }
            }

            return magicParams;
        }

        #endregion
    }
}
