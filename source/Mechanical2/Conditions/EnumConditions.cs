using System;
using System.Diagnostics;
using Mechanical.Core;

namespace Mechanical.Conditions
{
    /// <content>
    /// <see cref="Enum{T}"/> conditions.
    /// </content>
    public static partial class ConditionsExtensions
    {
        #region IsDefined

        /// <summary>
        /// Throws an exception, if the enumeration value is not defined.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<TEnum> IsDefined<TEnum>( this IConditionContext<TEnum> context )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            return IsDefined<TEnum>(context, () => new ArgumentException("The enumeration value is not defined!"));
        }

        /// <summary>
        /// Throws an exception, if the enumeration value is not defined.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<Enum<TEnum>> IsDefined<TEnum>( this IConditionContext<Enum<TEnum>> context )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            return IsDefined<TEnum>(context, () => new ArgumentException("The enumeration value is not defined!"));
        }

        /// <summary>
        /// Throws an exception, if the enumeration value is not defined.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<TEnum> IsDefined<TEnum>( this IConditionContext<TEnum> context, Func<Exception> createException )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !((Enum<TEnum>)context.Object).IsDefined )
                throw createException().Store(context);
            else
                return context;
        }

        /// <summary>
        /// Throws an exception, if the enumeration value is not defined.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<Enum<TEnum>> IsDefined<TEnum>( this IConditionContext<Enum<TEnum>> context, Func<Exception> createException )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !context.Object.IsDefined )
                throw createException().Store(context);
            else
                return context;
        }

        #endregion

        #region IsValid

        /// <summary>
        /// Throws an exception, if the enumeration value is not valid.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<TEnum> IsValid<TEnum>( this IConditionContext<TEnum> context )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            return IsValid<TEnum>(context, () => new ArgumentException("The enumeration value is not valid!"));
        }

        /// <summary>
        /// Throws an exception, if the enumeration value is not valid.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<Enum<TEnum>> IsValid<TEnum>( this IConditionContext<Enum<TEnum>> context )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            return IsValid<TEnum>(context, () => new ArgumentException("The enumeration value is not valid!"));
        }

        /// <summary>
        /// Throws an exception, if the enumeration value is not valid.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<TEnum> IsValid<TEnum>( this IConditionContext<TEnum> context, Func<Exception> createException )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !((Enum<TEnum>)context.Object).IsValid )
                throw createException().Store(context);
            else
                return context;
        }

        /// <summary>
        /// Throws an exception, if the enumeration value is not valid.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<Enum<TEnum>> IsValid<TEnum>( this IConditionContext<Enum<TEnum>> context, Func<Exception> createException )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !context.Object.IsValid )
                throw createException().Store(context);
            else
                return context;
        }

        #endregion
    }
}
