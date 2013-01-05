using System;
using System.Diagnostics;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    /// <content>
    /// <see cref="IDisposableObject"/> conditions.
    /// </content>
    public static partial class CoreExtensions
    {
        /// <summary>
        /// Throws an exception, if the object has already been disposed of.
        /// </summary>
        /// <typeparam name="TDisposable">The type of the disposable object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<TDisposable> NotDisposed<TDisposable>( this IConditionContext<TDisposable> context )
            where TDisposable : IDisposableObject
        {
            return NotDisposed(context, () => new ObjectDisposedException("Disposed object accessed!", innerException: null));
        }

        /// <summary>
        /// Throws an exception, if the object has already been disposed of.
        /// </summary>
        /// <typeparam name="TDisposable">The type of the disposable object.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<TDisposable> NotDisposed<TDisposable>( this IConditionContext<TDisposable> context, Func<Exception> createException )
            where TDisposable : IDisposableObject
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.IsDisposed )
                throw createException().StoreDefault(context);
            else
                return context;
        }
    }
}
