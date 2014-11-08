using System;
using System.Threading;
using System.Threading.Tasks;
using Mechanical.Bootstrap;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    /// <summary>
    /// UI thread and GUI related helpers.
    /// </summary>
    public static class UI
    {
        #region Private Static Fields

        private static IUIThreadHandler uiThreadHandler = null;

        #endregion

        #region Internal Members

        internal static IUIThreadHandler UIThreadHandler
        {
            private get
            {
                if( !IsSupported )
                    throw new NotSupportedException("The app has no UI!").StoreFileLine();

                return uiThreadHandler;
            }
            set
            {
                //// NOTE: a null reference is perfectly valid, if the app has no UI

                var oldValue = Interlocked.CompareExchange(ref uiThreadHandler, value, comparand: null);
                if( oldValue.NotNullReference() )
                    throw new InvalidOperationException("UI already initialized!").StoreFileLine();
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets a value indicating whether this app supports a traditional UI interface.
        /// </summary>
        /// <value>Indicates whether this app supports a traditional UI interface.</value>
        public static bool IsSupported
        {
            get
            {
                var hasUI = AppCore.HasUI;
                if( !hasUI.HasValue )
                    throw new InvalidOperationException("App UI not yet initialized!").StoreFileLine();
                else
                    return hasUI.Value;
            }
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        public static void Invoke( Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var handler = UIThreadHandler;
            if( handler.IsOnUIThread() )
                action();
            else
                handler.Invoke(action);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously on the UI thread.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="func">The delegate to invoke.</param>
        /// <returns>The return value of the delegate.</returns>
        public static TResult Invoke<TResult>( Func<TResult> func )
        {
            if( func.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var handler = UIThreadHandler;
            if( handler.IsOnUIThread() )
            {
                return func();
            }
            else
            {
                var result = default(TResult);
                handler.Invoke(() => result = func());
                return result;
            }
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        /// <returns>The <see cref="Task"/> representing the operation.</returns>
        public static Task InvokeAsync( Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var handler = UIThreadHandler;
            return handler.InvokeAsync(action);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously on the UI thread.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="func">The delegate to invoke.</param>
        /// <returns>The <see cref="Task{TResult}"/> representing the operation.</returns>
        public static Task<TResult> InvokeAsync<TResult>( Func<TResult> func )
        {
            if( func.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var handler = UIThreadHandler;
            var result = default(TResult);
            return handler.InvokeAsync(() => result = func())
                          .ContinueWith(prevTask => result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        #endregion
    }
}
