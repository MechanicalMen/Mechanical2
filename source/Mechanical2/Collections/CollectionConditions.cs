using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanical.Conditions;

namespace Mechanical.Collections
{
    /// <content>
    /// Validation methods related to the Mechanical.Collections namespace.
    /// </content>
    public static partial class CollectionsExtensions
    {
        #region NotNullOrEmpty

        /// <summary>
        /// Throws an exception, if the enumerable is <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="site">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<IEnumerable<T>> NotNullOrEmpty<T>( this IConditionContext<IEnumerable<T>> site )
        {
            return NotNullOrEmpty(site, () => new ArgumentException("The enumerable is null or empty!"));
        }

        /// <summary>
        /// Throws an exception, if the enumerable is <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="site">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<IEnumerable<T>> NotNullOrEmpty<T>( this IConditionContext<IEnumerable<T>> site, Func<Exception> createException )
        {
            Ensure.Debug(site, s => s.NotNull());

            if( site.Object.NullOrEmpty() )
                throw createException().Store(site);
            else
                return site;
        }

        #endregion

        #region NotNullEmptyOrSparse

        /// <summary>
        /// Throws an exception, if the enumerable is null, empty, or contains <c>null</c> references.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="site">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<IEnumerable<T>> NotNullEmptyOrSparse<T>( this IConditionContext<IEnumerable<T>> site )
        {
            return NotNullEmptyOrSparse(site, () => new ArgumentException("The enumerable is null, empty, or sparse (contains null)!"));
        }

        /// <summary>
        /// Throws an exception, if the enumerable is null, empty, or contains <c>null</c> references.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="site">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<IEnumerable<T>> NotNullEmptyOrSparse<T>( this IConditionContext<IEnumerable<T>> site, Func<Exception> createException )
        {
            Ensure.Debug(site, s => s.NotNull());

            if( site.Object.NullEmptyOrSparse() )
                throw createException().Store(site);
            else
                return site;
        }

        #endregion

        #region NotNullOrSparse

        /// <summary>
        /// Throws an exception, if the enumerable is null, or contains <c>null</c> references.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="site">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<IEnumerable<T>> NotNullOrSparse<T>( this IConditionContext<IEnumerable<T>> site )
        {
            return NotNullOrSparse(site, () => new ArgumentException("The enumerable is null, or sparse (contains null)!"));
        }

        /// <summary>
        /// Throws an exception, if the enumerable is null, empty, or contains <c>null</c> references.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="site">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<IEnumerable<T>> NotNullOrSparse<T>( this IConditionContext<IEnumerable<T>> site, Func<Exception> createException )
        {
            Ensure.Debug(site, s => s.NotNull());

            if( site.Object.NullOrSparse() )
                throw createException().Store(site);
            else
                return site;
        }

        #endregion
    }
}
