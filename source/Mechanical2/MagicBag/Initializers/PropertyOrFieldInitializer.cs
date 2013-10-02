using System;
using System.Linq.Expressions;
using System.Reflection;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Parameters;

namespace Mechanical.MagicBag.Initializers
{
    /// <summary>
    /// Assigns an object to a property or field of newly allocated objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to initialize.</typeparam>
    public class PropertyOrFieldInitializer<T> : IMagicBagInitializer<T>
    {
        #region Private Fields

        private readonly Func<T, IMagicBag, T> assign;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrFieldInitializer{T}"/> class.
        /// </summary>
        /// <param name="propertyOrField">The field or property to assign to.</param>
        /// <param name="parameter">The parameter being assigned.</param>
        public PropertyOrFieldInitializer( MemberInfo propertyOrField, IMagicBagParameter parameter )
        {
            Ensure.That(propertyOrField).NotNull();
            Ensure.That(parameter).NotNull();

            if( !(propertyOrField is FieldInfo)
             && !(propertyOrField is PropertyInfo) )
                throw new ArgumentException("The specified member is neither a property, nor a field!").Store("MemberType", propertyOrField.MemberType);

            //// NOTE: we generate the following code:
            //// {
            ////     uninitialized.PropertyOrField = parameter;
            ////     return uninitialized;
            //// }

            var uninitializedExpr = Expression.Parameter(typeof(T));
            var magicBagExpr = Expression.Parameter(typeof(IMagicBag));

            var memberParamExpr = parameter.ToExpression(magicBagExpr);
            var memberAccessExpr = Expression.MakeMemberAccess(uninitializedExpr, propertyOrField);

            var assignmentExpr = Expression.Assign(memberAccessExpr, memberParamExpr);
            var returnTarget = Expression.Label(typeof(T));
            var bodyExpr = Expression.Block(
                typeof(T),
                assignmentExpr,
                Expression.Return(returnTarget, uninitializedExpr),
                Expression.Label(returnTarget, Expression.Constant(default(T), typeof(T))));

            var lamdaExpr = Expression.Lambda<Func<T, IMagicBag, T>>(bodyExpr, uninitializedExpr, magicBagExpr);
            this.assign = lamdaExpr.Compile();
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
                throw new ArgumentNullException("allocated").StoreFileLine();

            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreFileLine();

            return this.assign(allocated, magicBag);
        }

        #endregion
    }
}
