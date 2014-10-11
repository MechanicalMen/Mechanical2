using System;
using System.Threading;
using System.Threading.Tasks;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    internal class ConsoleThreadHandler : IUIThreadHandler
    {
        //// NOTE: There is no UI thread for consoles.
        ////       We can make a token effort so that our code works some times,
        ////       but it's better not to call these from the console, if possible.

        //// NOTE: A nicer solution would be to set up an event queue (possibly on another thread),
        ////       and wrap delegates as "events", which are then run by the queue when it gets to them.

        private readonly TaskScheduler scheduler;

        internal ConsoleThreadHandler( TaskScheduler scheduler )
        {
            if( scheduler.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            this.scheduler = scheduler;
        }

        public bool IsOnUIThread()
        {
            return true;
        }

        public void Invoke( Action action )
        {
            action();
        }

        public Task InvokeAsync( Action action )
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }
    }
}
