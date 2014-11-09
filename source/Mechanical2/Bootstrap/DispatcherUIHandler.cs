using System;
#if SILVERLIGHT
using System.Threading;
#endif
using System.Threading.Tasks;
using System.Windows.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// Executes delegates on the UI thread, using a <see cref="Dispatcher"/>.
    /// </summary>
    public sealed class DispatcherUIHandler : IUIThreadHandler
    {
        #region Private Fields

        private static readonly object[] Parameters = new object[0];

        private readonly Dispatcher dispatcher;
#if !SILVERLIGHT
        private readonly DispatcherPriority priority;
#endif

        #endregion

        #region Constructors
#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherUIHandler"/> class.
        /// </summary>
        /// <param name="dispatcher">The UI <see cref="Dispatcher"/>.</param>
        /// <param name="priority">The <see cref="DispatcherPriority"/> to invoke the calls with.</param>
        public DispatcherUIHandler( Dispatcher dispatcher, DispatcherPriority priority = DispatcherPriority.Normal )
        {
            Ensure.That(dispatcher).NotNull();
            Ensure.That(priority).IsDefined();

            this.dispatcher = dispatcher;
            this.priority = priority;
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherUIHandler"/> class.
        /// </summary>
        /// <param name="dispatcher">The UI <see cref="Dispatcher"/>.</param>
        public DispatcherUIHandler( Dispatcher dispatcher )
        {
            Ensure.That(dispatcher).NotNull();

            this.dispatcher = dispatcher;
        }
#endif

        #endregion

        /// <summary>
        /// Determines whether the calling code is running on the UI thread.
        /// </summary>
        /// <returns><c>true</c> if the calling code is running on the UI thread; otherwise, <c>false</c>.</returns>
        public bool IsOnUIThread()
        {
            return this.dispatcher.CheckAccess();
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        public void Invoke( Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

#if !SILVERLIGHT
            this.dispatcher.Invoke(action, this.priority);
#else
            // NOTE: code segment originally from Caliburn.Micro
            var waitHandle = new ManualResetEvent(initialState: false); // initialState = non-signaled
            Exception exception = null;
            this.dispatcher.BeginInvoke(() =>
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
                throw new System.Reflection.TargetInvocationException("An error occurred while dispatching a call to the UI Thread", exception).StoreFileLine();
#endif
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        /// <returns>The <see cref="Task"/> representing the operation.</returns>
        public Task InvokeAsync( Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

#if MECHANICAL_NET4
            var tsc = new TaskCompletionSource<object>();
            action = () =>
            {
                try
                {
                    action();
                    tsc.SetResult(null); // the Completed event would probably work just as well
                }
                catch( Exception ex )
                {
                    tsc.SetException(ex);
                }
            };
            var op = this.dispatcher.BeginInvoke(action, this.priority, Parameters);
            op.Aborted += ( s, e ) => tsc.SetCanceled(); // DispatcherOperations can be aborted, before code starts executing (since we don't keep a reference to it, this should only happen if the Dispatcher is shut down, before execution starts)
            return tsc.Task;
#elif SILVERLIGHT
            var tsc = new TaskCompletionSource<object>();
            var op = this.dispatcher.BeginInvoke(() =>
            {
                try
                {
                    action();
                    tsc.SetResult(null);
                }
                catch( Exception ex )
                {
                    tsc.SetException(ex);
                }
            });
            return tsc.Task;
#else
            return this.dispatcher.InvokeAsync(action, this.priority).Task;
#endif
        }
    }
}
