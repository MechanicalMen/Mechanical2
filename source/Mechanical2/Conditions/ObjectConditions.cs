using System;
using System.Diagnostics;

namespace Mechanical.Conditions
{
    /// <content>
    /// <see cref="Object"/> conditions.
    /// </content>
    public static partial class ConditionsExtensions
    {
        #region NotNull

        /// <summary>
        /// Throws an exception, if the object is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotNull<T>( this IConditionContext<T> context )
        {
            return NotNull<T>(context, () => new ArgumentNullException("The object is null!"));
        }

        /// <summary>
        /// Throws an exception, if the object is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotNull<T>( this IConditionContext<T> context, Func<Exception> createException )
        {
            Ensure.Debug(object.ReferenceEquals(context, null), isNull => isNull.IsFalse(() => new ArgumentNullException("The condition context specified is null!")));

            if( object.ReferenceEquals(context.Object, null) )
                throw createException().Store(context);
            else
                return context;
        }

        #endregion

        #region Null

        /// <summary>
        /// Throws an exception, if the object is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Null<T>( this IConditionContext<T> context )
        {
            return Null<T>(context, () => new ArgumentException("The object is not null!"));
        }

        /// <summary>
        /// Throws an exception, if the object is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Null<T>( this IConditionContext<T> context, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !object.ReferenceEquals(context.Object, null) )
                throw createException().Store(context);
            else
                return context;
        }

        #endregion

        #region SameAs

        /// <summary>
        /// Throws an exception, if the two objects are not the same (reference).
        /// </summary>
        /// <typeparam name="T">The type of the first object.</typeparam>
        /// <typeparam name="U">The type of the second object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> SameAs<T, U>( this IConditionContext<T> context, U obj )
        {
            return SameAs<T, U>(context, obj, () => new ArgumentException("The objects are not the same!"));
        }

        /// <summary>
        /// Throws an exception, if the two objects are not the same (reference).
        /// </summary>
        /// <typeparam name="T">The type of the first object.</typeparam>
        /// <typeparam name="U">The type of the second object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="obj">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> SameAs<T, U>( this IConditionContext<T> context, U obj, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !object.ReferenceEquals(context.Object, obj) )
                throw createException().Store(context).Store("OtherObject", obj);
            else
                return context;
        }

        #endregion

        #region NotSameAs

        /// <summary>
        /// Throws an exception, if the two objects are the same (reference).
        /// </summary>
        /// <typeparam name="T">The type of the first object.</typeparam>
        /// <typeparam name="U">The type of the second object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotSameAs<T, U>( this IConditionContext<T> context, U obj )
        {
            return NotSameAs<T, U>(context, obj, () => new ArgumentException("The objects are the same!"));
        }

        /// <summary>
        /// Throws an exception, if the two objects are the same (reference).
        /// </summary>
        /// <typeparam name="T">The type of the first object.</typeparam>
        /// <typeparam name="U">The type of the second object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="obj">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotSameAs<T, U>( this IConditionContext<T> context, U obj, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( object.ReferenceEquals(context.Object, obj) )
                throw createException().Store(context).Store("OtherObject", obj);
            else
                return context;
        }

        #endregion
    }
}
