using System;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// A generic interface for catching unhandled exceptions of some kind.
    /// </summary>
    public interface IExceptionSource
    {
        /// <summary>
        /// Raised when an unhandled exception was caught.
        /// </summary>
        event Action<Exception> ExceptionCaught;
    }
}
