using System;
using Mechanical.Core;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// Uses <see cref="Log"/> to log unhandled exceptions;
    /// </summary>
    public sealed class LogExceptionSink : IExceptionSink
    {
        /// <summary>
        /// Processes unhandled exceptions.
        /// </summary>
        /// <param name="exception">The unhandled exception to process.</param>
        public void Handle( Exception exception )
        {
            if( exception.NullReference() )
                return;

            AppCore.Log.Error("Unhandled exception caught!", exception);
        }
    }
}
