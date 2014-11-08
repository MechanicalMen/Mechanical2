using System;
using Mechanical.Core;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// Wraps <see cref="AppDomain.UnhandledException"/>.
    /// </summary>
    public sealed class AppDomainExceptionSource : IExceptionSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainExceptionSource"/> class.
        /// </summary>
        public AppDomainExceptionSource()
        {
            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledExceptionRaised;
        }

        private void OnUnhandledExceptionRaised( object sender, UnhandledExceptionEventArgs e )
        {
            var handler = this.ExceptionCaught;
            if( handler.NotNullReference() )
                handler((Exception)e.ExceptionObject);
        }

        /// <summary>
        /// Raised when an unhandled exception was caught.
        /// </summary>
        public event Action<Exception> ExceptionCaught;
    }
}
