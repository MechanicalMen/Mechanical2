using System;
using System.Linq.Expressions;
using System.Reflection;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    /// <summary>
    /// Provides static reflection services (based on lambdas, instead of "magic strings").
    /// </summary>
    public static class Reveal
    {
        #region Private Static Methods

        private static Expression SkipNodes( Expression expr )
        {
            Ensure.Debug(expr, e => e.NotNull());

            switch( expr.NodeType )
            {
            case ExpressionType.Lambda:
                return SkipNodes(((LambdaExpression)expr).Body);

            case ExpressionType.Convert:
                return SkipNodes(((UnaryExpression)expr).Operand);

            default:
                return expr;
            }
        }

        private static MemberInfo GetInfo( ref Expression expr )
        {
            Ensure.Debug(expr, e => e.NotNull());

            switch( expr.NodeType )
            {
            case ExpressionType.MemberAccess:
                {
                    var e = (MemberExpression)expr;
                    expr = e.Expression;
                    return e.Member;
                }

            case ExpressionType.Call:
                {
                    var e = (MethodCallExpression)expr;
                    expr = e.Object;
                    return e.Method;
                }

            case ExpressionType.New:
                {
                    var e = (NewExpression)expr;
                    expr = null;
                    return e.Constructor;
                }

            case ExpressionType.Constant:
                {
                    var e = (ConstantExpression)expr;
                    expr = null;
                    return null;
                }

            default:
                throw new ArgumentException("The expression could not be explored!").Store("ExpressionType", expr.NodeType);
            }
        }

        #endregion

        #region Info

        /// <summary>
        /// Extracts a <see cref="MemberInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="T">The type (or return value) of the member.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MemberInfo"/> extracted.</returns>
        public static MemberInfo Info<T>( Expression<Func<T>> expr )
        {
            var e = SkipNodes(expr);
            var info = GetInfo(ref e);

            if( info.NotNullReference() )
                return info;
            else
                throw new ArgumentException("Expression is not a member!").StoreDefault();
        }

        /// <summary>
        /// Extracts a <see cref="MemberInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="T">The declaring type.</typeparam>
        /// <typeparam name="TReturn">The type (or return value) of the member.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MemberInfo"/> extracted.</returns>
        public static MemberInfo Info<T, TReturn>( Expression<Func<T, TReturn>> expr )
        {
            var e = SkipNodes(expr);
            var info = GetInfo(ref e);

            if( info.NotNullReference() )
                return info;
            else
                throw new ArgumentException("Expression is not a member!").StoreDefault();
        }

        /// <summary>
        /// Extracts a <see cref="MemberInfo"/> from the expression.
        /// </summary>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MemberInfo"/> extracted.</returns>
        public static MemberInfo Info( Expression<Action> expr )
        {
            var e = SkipNodes(expr);
            var info = GetInfo(ref e);

            if( info.NotNullReference() )
                return info;
            else
                throw new ArgumentException("Expression is not a member!").StoreDefault();
        }

        /// <summary>
        /// Extracts a <see cref="MemberInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="T">The type declaring the member.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MemberInfo"/> extracted.</returns>
        public static MemberInfo Info<T>( Expression<Action<T>> expr )
        {
            var e = SkipNodes(expr);
            var info = GetInfo(ref e);

            if( info.NotNullReference() )
                return info;
            else
                throw new ArgumentException("Expression is not a member!").StoreDefault();
        }

        #endregion

        #region Name

        /// <summary>
        /// Extracts the member name from the expression.
        /// </summary>
        /// <typeparam name="T">The type (or return value) of the member.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The name extracted.</returns>
        public static string Name<T>( Expression<Func<T>> expr )
        {
            return Info(expr).Name;
        }

        /// <summary>
        /// Extracts the member name from the expression.
        /// </summary>
        /// <typeparam name="T">The declaring type.</typeparam>
        /// <typeparam name="TReturn">The type (or return value) of the member.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The name extracted.</returns>
        public static string Name<T, TReturn>( Expression<Func<T, TReturn>> expr )
        {
            return Info(expr).Name;
        }

        /// <summary>
        /// Extracts the member name from the expression.
        /// </summary>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The name extracted.</returns>
        public static string Name( Expression<Action> expr )
        {
            return Info(expr).Name;
        }

        /// <summary>
        /// Extracts the member name from the expression.
        /// </summary>
        /// <typeparam name="T">The type declaring the member.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The name extracted.</returns>
        public static string Name<T>( Expression<Action<T>> expr )
        {
            return Info(expr).Name;
        }

        #endregion

        #region Field

        /// <summary>
        /// Extracts a <see cref="FieldInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="FieldInfo"/> extracted.</returns>
        public static FieldInfo Field<TField>( Expression<Func<TField>> expr )
        {
            return (FieldInfo)Info(expr);
        }

        /// <summary>
        /// Extracts a <see cref="FieldInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="T">The declaring type.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="FieldInfo"/> extracted.</returns>
        public static FieldInfo Field<T, TField>( Expression<Func<T, TField>> expr )
        {
            return (FieldInfo)Info(expr);
        }

        #endregion

        #region Property

        /// <summary>
        /// Extracts a <see cref="PropertyInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="PropertyInfo"/> extracted.</returns>
        public static PropertyInfo Property<TProperty>( Expression<Func<TProperty>> expr )
        {
            return (PropertyInfo)Info(expr);
        }

        /// <summary>
        /// Extracts a <see cref="PropertyInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="T">The declaring type.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="PropertyInfo"/> extracted.</returns>
        public static PropertyInfo Property<T, TProperty>( Expression<Func<T, TProperty>> expr )
        {
            return (PropertyInfo)Info(expr);
        }

        #endregion

        #region Method

        /// <summary>
        /// Extracts a <see cref="MethodInfo"/> from the expression.
        /// </summary>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MethodInfo"/> extracted.</returns>
        public static MethodInfo Method( Expression<Action> expr )
        {
            return (MethodInfo)Info(expr);
        }

        /// <summary>
        /// Extracts a <see cref="MethodInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="T">The type declaring the method.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MethodInfo"/> extracted.</returns>
        public static MethodInfo Method<T>( Expression<Action<T>> expr )
        {
            return (MethodInfo)Info(expr);
        }

        /// <summary>
        /// Extracts a <see cref="MethodInfo"/> from the expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the method's return value.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="MethodInfo"/> extracted.</returns>
        public static MethodInfo Method<TResult>( Expression<Func<TResult>> expr )
        {
            return (MethodInfo)Info(expr);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Extracts a <see cref="ConstructorInfo"/> from the expression specified.
        /// </summary>
        /// <typeparam name="T">The type to get the constructor for.</typeparam>
        /// <param name="expr">The expression to process.</param>
        /// <returns>The <see cref="ConstructorInfo"/> extracted.</returns>
        public static ConstructorInfo Constructor<T>( Expression<Func<T>> expr )
        {
            return (ConstructorInfo)Info(expr);
        }

        #endregion

        #region IsByRef

        private static class IsByRefCache<T>
        {
            internal static readonly bool Value;

            static IsByRefCache()
            {
                Value = typeof(T).IsByRef;
            }
        }

        /// <summary>
        /// Indicates whether the type <typeparamref name="T"/> is passed by reference.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns><c>true</c> if the type <typeparamref name="T"/> is passed by reference; otherwise <c>false</c>.</returns>
        public static bool IsByRef<T>()
        {
            return IsByRefCache<T>.Value;
        }

        #endregion


        #region CanAssignTo

        //// NOTE: not static reflection, but IsAssignableFrom always confuses me
        ////       and this seemed the best place to put it for now...

        //// NOTE: Name was choosen to hint at the underlying implementation.

        /// <summary>
        /// <c>true</c> if <typeparamref name="TInheritor"/> and <typeparamref name="TBase"/> represent the same type, or if <typeparamref name="TBase"/> is in the inheritance hierarchy of <typeparamref name="TInheritor"/>, or if <typeparamref name="TBase"/> is an interface that <typeparamref name="TInheritor"/> implements, or if <typeparamref name="TInheritor"/> is a generic type parameter and <typeparamref name="TBase"/> represents one of the constraints of <typeparamref name="TInheritor"/>. <c>false</c> if none of these conditions are true.
        /// </summary>
        /// <typeparam name="TBase">The type to assign to.</typeparam>
        /// <typeparam name="TInheritor">The type to be assigned.</typeparam>
        /// <returns>See summary.</returns>
        public static bool CanAssignTo<TBase, TInheritor>()
        {
            return CanAssignTo(typeof(TBase), typeof(TInheritor));
        }

        /// <summary>
        /// <c>true</c> if <paramref name="inheritor"/> and <typeparamref name="TBase"/> represent the same type, or if <typeparamref name="TBase"/> is in the inheritance hierarchy of <paramref name="inheritor"/>, or if <typeparamref name="TBase"/> is an interface that <paramref name="inheritor"/> implements, or if <paramref name="inheritor"/> is a generic type parameter and <typeparamref name="TBase"/> represents one of the constraints of <paramref name="inheritor"/>. <c>false</c> if none of these conditions are true, or if <paramref name="inheritor"/> is <c>null</c>.
        /// </summary>
        /// <typeparam name="TBase">The type to assign to.</typeparam>
        /// <param name="inheritor">The type to be assigned.</param>
        /// <returns>See summary.</returns>
        public static bool CanAssignTo<TBase>( Type inheritor )
        {
            return CanAssignTo(typeof(TBase), inheritor);
        }

        /// <summary>
        /// <c>true</c> if <paramref name="inheritor"/> and <paramref name="baseType"/> represent the same type, or if <paramref name="baseType"/> is in the inheritance hierarchy of <paramref name="inheritor"/>, or if <paramref name="baseType"/> is an interface that <paramref name="inheritor"/> implements, or if <paramref name="inheritor"/> is a generic type parameter and <paramref name="baseType"/> represents one of the constraints of <paramref name="inheritor"/>. <c>false</c> if none of these conditions are true, or if <paramref name="inheritor"/> is <c>null</c>.
        /// </summary>
        /// <param name="baseType">The type to assign to.</param>
        /// <param name="inheritor">The type to be assigned.</param>
        /// <returns>See summary.</returns>
        public static bool CanAssignTo( Type baseType, Type inheritor )
        {
            if( baseType.NullReference() )
                throw new ArgumentNullException().StoreDefault();

            return baseType.IsAssignableFrom(inheritor);
        }

        #endregion
    }
}
