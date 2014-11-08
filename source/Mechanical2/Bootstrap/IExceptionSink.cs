using System;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// Processes unhandled exceptions.
    /// </summary>
    public interface IExceptionSink
    {
        /// <summary>
        /// Processes unhandled exceptions.
        /// </summary>
        /// <param name="exception">The unhandled exception to process.</param>
        void Handle( Exception exception );
    }
}
