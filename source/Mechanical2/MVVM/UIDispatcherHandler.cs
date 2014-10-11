using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    internal class UIDispatcherHandler : IUIThreadHandler
    {
        private readonly Dispatcher dispatcher;
        private readonly TaskScheduler scheduler;

        internal UIDispatcherHandler( Dispatcher dispatcher, TaskScheduler scheduler )
        {
            if( dispatcher.NullReference() )
                throw new ArgumentNullException("dispatcher").StoreFileLine();

            if( scheduler.NullReference() )
                throw new ArgumentNullException("scheduler").StoreFileLine();

            this.dispatcher = dispatcher;
            this.scheduler = scheduler;
        }

        public bool IsOnUIThread()
        {
            return this.dispatcher.CheckAccess();
        }

        public void Invoke( Action action )
        {
#if !SILVERLIGHT
            this.dispatcher.Invoke(action);
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

        public Task InvokeAsync( Action action )
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }
    }
}
