using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
            SetIsInDesigner();
        }

        #endregion

        #region Dispatcher

        private static Dispatcher dispatcher = null;

        internal static void SetDispatcherFromCurrent()
        {
            Bootstrap.ThrowIfAlreadyInitialized();

#if !SILVERLIGHT
            Interlocked.Exchange(ref dispatcher, Dispatcher.CurrentDispatcher);
#else
            Interlocked.Exchange(ref dispatcher, Deployment.Current.Dispatcher);
#endif
        }

        internal static void SetDispatcherForConsole()
        {
            Interlocked.Exchange(ref dispatcher, null); // not strictly necessary. Only here to be explicit.
        }

        /// <summary>
        /// Gets the UI <see cref="Dispatcher"/>. <c>null</c> for console applications.
        /// </summary>
        /// <value>The UI <see cref="Dispatcher"/>.</value>
        public static Dispatcher Dispatcher
        {
            get
            {
                Bootstrap.ThrowIfUninitialized();

                return dispatcher;
            }
        }

        #endregion

        #region Scheduler

        private static TaskScheduler scheduler = null;

        internal static void SetSchedulerFromCurrent()
        {
            Bootstrap.ThrowIfAlreadyInitialized();

            Interlocked.Exchange(ref scheduler, TaskScheduler.FromCurrentSynchronizationContext());
        }

        internal static void SetSchedulerForConsole()
        {
            Interlocked.Exchange(ref scheduler, TaskScheduler.Default); // not strictly necessary. Only here to be explicit.
        }

        /// <summary>
        /// Gets the UI scheduler.
        /// </summary>
        /// <value>The UI scheduler.</value>
        public static TaskScheduler Scheduler
        {
            get
            {
                Bootstrap.ThrowIfUninitialized();

                return scheduler;
            }
        }

        #endregion

        #region IsConsole

        /// <summary>
        /// Gets a value indicating whether this is a console application.
        /// </summary>
        /// <value><c>true</c> if this is a console application; otherwise <c>false</c>.</value>
        public static bool IsConsole
        {
            get { return Dispatcher.NullReference(); }
        }

        #endregion

        #region IsInDesigner

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

        #endregion

        #region Invoke, InvokeAsync

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        public static void Invoke( Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( IsConsole
             || Dispatcher.CheckAccess() )
            {
                action();
            }
            else
            {
#if !SILVERLIGHT
                Dispatcher.Invoke(action);
#else
                // NOTE: code segment comes from Caliburn.Micro
                var waitHandle = new ManualResetEvent(initialState: false); // initialState = non-signaled
                Exception exception = null;

                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        action();
                    }
                    catch( Exception ex )
                    {
                        exception = ex;
                    }
                    waitHandle.Set();
                });

                waitHandle.WaitOne();
                if( exception.NotNullReference() )
                    throw new System.Reflection.TargetInvocationException("An error occurred while dispatching a call to the UI Thread", exception);
#endif
            }
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

            if( IsConsole
             || Dispatcher.CheckAccess() )
            {
                return func();
            }
            else
            {
#if MECHANICAL_NET4
                return (TResult)Dispatcher.Invoke(func);
#elif SILVERLIGHT
                var waitHandle = new ManualResetEvent(initialState: false); // initialState = non-signaled
                Exception exception = null;
                TResult retval = default(TResult);

                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        retval = func();
                    }
                    catch( Exception ex )
                    {
                        exception = ex;
                    }
                    waitHandle.Set();
                });

                waitHandle.WaitOne();
                if( exception.NotNullReference() )
                    throw new System.Reflection.TargetInvocationException("An error occurred while dispatching a call to the UI Thread", exception);
                else
                    return retval;
#else
                return Dispatcher.Invoke(func);
#endif
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

            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, Scheduler);
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

            return Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, Scheduler);
        }

        #endregion
    }
}
