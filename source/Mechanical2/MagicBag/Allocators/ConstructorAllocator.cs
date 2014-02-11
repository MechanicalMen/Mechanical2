using System;
using System.Linq.Expressions;
using System.Reflection;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Parameters;

namespace Mechanical.MagicBag.Allocators
{
    /// <summary>
    /// An <see cref="IMagicBagAllocator{T}"/> that uses a <see cref="ConstructorInfo"/> to create a new object instance.
    /// </summary>
    /// <typeparam name="T">The type of objects to create instances of.</typeparam>
    public class ConstructorAllocator<T> : IMagicBagAllocator<T>
    {
        #region Private Fields

        private readonly Func<IMagicBag, T> method;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorAllocator{T}"/> class.
        /// </summary>
        /// <param name="ctor">The constructor to use.</param>
        /// <param name="parameters">The parameters to invoke the constructor with.</param>
        public ConstructorAllocator( ConstructorInfo ctor, MagicParameters parameters = null )
        {
            Ensure.That(ctor).NotNull();

            // would 'T returnValue = ctor(...)' compile?
            if( !Reveal.CanAssignTo<T>(ctor.DeclaringType) )
                throw new ArgumentException("Can not assign constructed type to expected type!").Store("ExpectedType", typeof(T)).Store("ConstructedType", ctor.DeclaringType);


            var constructorParameters = ctor.GetParameters();
            var magicBagParameterExpression = Expression.Parameter(typeof(IMagicBag));
            var parameterExpressions = MagicParameters.GetExpressions(ctor, parameters, magicBagParameterExpression);

            var newExpr = Expression.New(ctor, parameterExpressions);
            var lambdaExpr = Expression.Lambda<Func<IMagicBag, T>>(newExpr, magicBagParameterExpression);
            this.method = lambdaExpr.Compile();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConstructorAllocator{T}"/> class.
        /// All parameters will be resolved using a magic bag.
        /// </summary>
        /// <param name="ctor">The constructor to use.</param>
        /// <returns>The <see cref="ConstructorAllocator{T}"/> created.</returns>
        public static ConstructorAllocator<T> InjectAll( ConstructorInfo ctor )
        {
            Ensure.That(ctor).NotNull();

            return new ConstructorAllocator<T>(ctor, MagicParameters.InjectAllOf(ctor));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConstructorAllocator{T}"/> class.
        /// </summary>
        /// <returns>The <see cref="ConstructorAllocator{T}"/> created.</returns>
        public static ConstructorAllocator<T> FromDefault()
        {
#if !MECHANICAL_NET4 && !SILVERLIGHT
            foreach( var ctor in typeof(T).GetTypeInfo().DeclaredConstructors )
#else
            foreach( var ctor in typeof(T).GetConstructors() )
#endif
            {
                var constructorParameters = ctor.GetParameters();
                if( constructorParameters.Length == 0 )
                    return new ConstructorAllocator<T>(ctor);
            }

            throw new InvalidOperationException("The specified type has no default constructor!").Store("Type", typeof(T));
        }

        #endregion

        #region IMagicBagAllocator

        /// <summary>
        /// Creates a new object instance.
        /// </summary>
        /// <param name="magicBag">The magic bag to use for dependency injection.</param>
        /// <returns>The new instance created.</returns>
        public T CreateInstance( IMagicBag magicBag )
        {
            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreFileLine();

            return this.method(magicBag);
        }

        #endregion
    }
}
