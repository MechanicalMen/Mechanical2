using System;
using System.Diagnostics;
using Mechanical.Core;

namespace Mechanical.Conditions
{
    /// <content>
    /// <see cref="String"/> conditions.
    /// </content>
    public static partial class ConditionsExtensions
    {
        #region NotNullOrEmpty

        /// <summary>
        /// Throws an exception, if the string is <c>null</c> or empty.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotNullOrEmpty( this IConditionContext<string> context )
        {
            return NotNullOrEmpty(context, () => new ArgumentException("The string is either null or empty!"));
        }

        /// <summary>
        /// Throws an exception, if the string is <c>null</c> or empty.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotNullOrEmpty( this IConditionContext<string> context, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.NullOrEmpty() )
                throw createException().StoreDefault(context);
            else
                return context;
        }

        #endregion

        #region NotNullOrLengthy

        /// <summary>
        /// Throws an exception, if the string is <c>null</c>, empty, or if it has leading or trailing white-space characters.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotNullOrLengthy( this IConditionContext<string> context )
        {
            return NotNullOrLengthy(context, () => new ArgumentException("The string is either null, empty, or lengthy (has leading or trailing white-space characters)!"));
        }

        /// <summary>
        /// Throws an exception, if the string is <c>null</c>, empty, or if it has leading or trailing white-space characters.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotNullOrLengthy( this IConditionContext<string> context, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.NullOrLengthy() )
                throw createException().StoreDefault(context);
            else
                return context;
        }

        #endregion

        #region NotNullOrWhiteSpace

        /// <summary>
        /// Throws an exception, if the string is <c>null</c>, empty, or if it consists only of white-space characters.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotNullOrWhiteSpace( this IConditionContext<string> context )
        {
            return NotNullOrWhiteSpace(context, () => new ArgumentException("The string is either null, empty or whitespace!"));
        }

        /// <summary>
        /// Throws an exception, if the string is <c>null</c>, empty, or if it consists only of white-space characters.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotNullOrWhiteSpace( this IConditionContext<string> context, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.NullOrWhiteSpace() )
                throw createException().StoreDefault(context);
            else
                return context;
        }

        #endregion
    }
}
