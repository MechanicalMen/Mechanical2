using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
#if !ANDROID
using System.Windows.Threading;
#endif
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    /// <summary>
    /// UI thread and GUI related helpers.
    /// </summary>
    public static class UI
    {
        #region Static Constructor

        static UI()
        {
#if !ANDROID
            SetIsInDesigner();
#endif
        }

        #endregion

        #region Dispatcher
#if !ANDROID
        private static Tuple<Dispatcher> dispatcher = null;

        internal static void SetDispatcherFromCurrent()
        {
#if !SILVERLIGHT
            var previousValue = Interlocked.CompareExchange(ref dispatcher, Tuple.Create(Dispatcher.CurrentDispatcher), comparand: null);
#else
            var previousValue = Interlocked.CompareExchange(ref dispatcher, Tuple.Create(Deployment.Current.Dispatcher), comparand: null);
#endif
            if( previousValue.NotNullReference() )
                throw new InvalidOperationException("UI Dispatcher already initialized!").StoreFileLine();
        }

        internal static void SetDispatcherForConsole()
        {
            var previousValue = Interlocked.CompareExchange(ref dispatcher, Tuple.Create<Dispatcher>(null), comparand: null); // not strictly necessary. Only here to be explicit.
            if( previousValue.NotNullReference() )
                throw new InvalidOperationException("UI Dispatcher already initialized!").StoreFileLine();
        }

        /// <summary>
        /// Gets the UI <see cref="Dispatcher"/>. <c>null</c> for console applications.
        /// </summary>
        /// <value>The UI <see cref="Dispatcher"/>.</value>
        public static Dispatcher Dispatcher
        {
            get
            {
                var d = dispatcher;
                if( d.NullReference() )
                    throw new InvalidOperationException("The UI Dispatcher was not yet initialized!").StoreFileLine();
                else
                    return d.Item1;
            }
        }
#endif
        #endregion

        #region Scheduler

        private static TaskScheduler scheduler = null;

        internal static void SetSchedulerFromCurrent()
        {
            var previousValue = Interlocked.CompareExchange(ref scheduler, TaskScheduler.FromCurrentSynchronizationContext(), comparand: null);
            if( previousValue.NotNullReference() )
                throw new InvalidOperationException("UI Scheduler already initialized!").StoreFileLine();
        }

        internal static void SetSchedulerForConsole()
        {
            var previousValue = Interlocked.CompareExchange(ref scheduler, TaskScheduler.Default, comparand: null); // not strictly necessary. Only here to be explicit.
            if( previousValue.NotNullReference() )
                throw new InvalidOperationException("UI Scheduler already initialized!").StoreFileLine();
        }

        /// <summary>
        /// Gets the UI scheduler.
        /// </summary>
        /// <value>The UI scheduler.</value>
        public static TaskScheduler Scheduler
        {
            get
            {
                var s = scheduler;
                if( s.NullReference() )
                    throw new InvalidOperationException("The UI Scheduler was not yet initialized!").StoreFileLine();
                else
                    return s;
            }
        }

        #endregion

        #region IsConsole
#if !ANDROID
        /// <summary>
        /// Gets a value indicating whether this is a console application.
        /// </summary>
        /// <value><c>true</c> if this is a console application; otherwise <c>false</c>.</value>
        public static bool IsConsole
        {
            get { return Dispatcher.NullReference(); }
        }
#endif
        #endregion

        #region IsInDesigner
#if !ANDROID
        private static bool isInDesigner = false;

        private static void SetIsInDesigner()
        {
#if SILVERLIGHT
            isInDesigner = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            isInDesigner = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

            if( !isInDesigner
             && System.Diagnostics.Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal) )
                isInDesigner = true;
#endif
        }

        /// <summary>
        /// Gets a value indicating whether the application is being edited inside the designer.
        /// </summary>
        /// <value><c>true</c> if the application is being edited inside the designer; otherwise <c>false</c>.</value>
        public static bool IsInDesigner
        {
            get { return isInDesigner; }
        }
#endif
        #endregion

        #region Invoke, InvokeAsync

        private static IUIThreadHandler uiThreadHandler = null;

        internal static void SetUIThreadHandler( IUIThreadHandler handler )
        {
            var previousValue = Interlocked.CompareExchange(ref uiThreadHandler, handler, comparand: null);
            if( previousValue.NotNullReference() )
                throw new InvalidOperationException("UI thread handler already initialized!").StoreFileLine();
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        public static void Invoke( Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var handler = uiThreadHandler;
            if( handler.NullReference() )
                throw new InvalidOperationException("The UI thread handler was not yet initialized!").StoreFileLine();

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

            var handler = uiThreadHandler;
            if( handler.NullReference() )
                throw new InvalidOperationException("The UI thread handler was not yet initialized!").StoreFileLine();

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

            var handler = uiThreadHandler;
            if( handler.NullReference() )
                throw new InvalidOperationException("The UI thread handler was not yet initialized!").StoreFileLine();

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

            var handler = uiThreadHandler;
            if( handler.NullReference() )
                throw new InvalidOperationException("The UI thread handler was not yet initialized!").StoreFileLine();

            var result = default(TResult);
            return handler.InvokeAsync(() => result = func())
                          .ContinueWith(prevTask => result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        #endregion
    }
}
