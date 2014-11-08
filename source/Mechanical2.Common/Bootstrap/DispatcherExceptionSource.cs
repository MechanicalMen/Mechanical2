using System;
using System.Windows;
using System.Windows.Threading;
using Mechanical.Bootstrap;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Common.Bootstrap
{
    /// <summary>
    /// Wraps <see cref="Application.DispatcherUnhandledException"/>.
    /// </summary>
    public class DispatcherExceptionSource : IExceptionSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherExceptionSource"/> class.
        /// </summary>
        /// <param name="app">The <see cref="System.Windows.Application"/> object that raises the event.</param>
        public DispatcherExceptionSource( Application app )
        {
            Ensure.That(app).NotNull();

            app.DispatcherUnhandledException += this.OnUnhandledExceptionRaised;
        }

        private void OnUnhandledExceptionRaised( object sender, DispatcherUnhandledExceptionEventArgs e )
        {
            var handler = this.ExceptionCaught;
            if( handler.NotNullReference() )
                handler(e.Exception);

            e.Handled = true;
        }

        /// <summary>
        /// Raised when an unhandled exception was caught.
        /// </summary>
        public event Action<Exception> ExceptionCaught;
    }
}
