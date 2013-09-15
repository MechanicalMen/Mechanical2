using System;

namespace Mechanical.Events
{
    /// <summary>
    /// Configures the return value of event publishing methods.
    /// </summary>
    public enum TaskResult
    {
        /// <summary>
        /// The task returns once all subscribers handled the event. May throw the exceptions of subscribers.
        /// </summary>
        CompletedOrException,

        /// <summary>
        /// The task returns once all subscribers handled the event. The exceptions of subscribers are silently wrapped in <see cref="UnhandledExceptionEvent"/>.
        /// </summary>
        Completed,

        /// <summary>
        /// <c>null</c> is returned instead of a task. Basically the "fire &amp; forget" mode. May silently create an <see cref="UnhandledExceptionEvent"/>.
        /// </summary>
        Null
    }
}
