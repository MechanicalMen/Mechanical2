using System;
using System.Threading.Tasks;
using Mechanical.Core;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// Wraps <see cref="TaskScheduler.UnobservedTaskException"/>.
    /// </summary>
    public sealed class UnobservedTaskExceptionSource : IExceptionSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnobservedTaskExceptionSource"/> class.
        /// </summary>
        public UnobservedTaskExceptionSource()
        {
            TaskScheduler.UnobservedTaskException += this.OnUnhandledExceptionRaised;
        }

        private void OnUnhandledExceptionRaised( object sender, UnobservedTaskExceptionEventArgs e )
        {
            var handler = this.ExceptionCaught;
            if( handler.NotNullReference() )
                handler(e.Exception);

            e.SetObserved();
        }

        /// <summary>
        /// Raised when an unhandled exception was caught.
        /// </summary>
        public event Action<Exception> ExceptionCaught;
    }
}
