using System;
using System.Diagnostics;

namespace Mechanical.Conditions
{
    /// <content>
    /// <see cref="Boolean"/> conditions.
    /// </content>
    public static partial class ConditionsExtensions
    {
        #region IsTrue

        /// <summary>
        /// Throws an exception, if the value is <c>false</c>.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<bool> IsTrue( this IConditionContext<bool> context )
        {
            return IsTrue(context, () => new ArgumentException("The expression evaluates to false!"));
        }

        /// <summary>
        /// Throws an exception, if the value is <c>false</c>.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<bool> IsTrue( this IConditionContext<bool> context, Func<Exception> createException )
        {
#if DEBUG
            if( object.ReferenceEquals(context, null) )
                throw new ArgumentNullException("context", "The condition context specified is null!").StoreDefault();
#endif
            if( !context.Object )
                throw createException().StoreDefault(context);
            else
                return context;
        }

        #endregion

        #region IsFalse

        /// <summary>
        /// Throws an exception, if the value is <c>true</c>.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<bool> IsFalse( this IConditionContext<bool> context )
        {
            return IsFalse(context, () => new ArgumentException("The expression evaluates to true!"));
        }

        /// <summary>
        /// Throws an exception, if the value is <c>true</c>.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<bool> IsFalse( this IConditionContext<bool> context, Func<Exception> createException )
        {
#if DEBUG
            if( object.ReferenceEquals(context, null) )
                throw new ArgumentNullException("context", "The condition context specified is null!").StoreDefault();
#endif
            if( context.Object )
                throw createException().StoreDefault(context);
            else
                return context;
        }

        #endregion
    }
}
