using System;
using System.Threading.Tasks;

namespace Mechanical.Events
{
    /// <summary>
    /// Used to register child events under the event currently being handled.
    /// </summary>
    public interface IEventHandlerQueue
    {
        /// <summary>
        /// Registers a child event to be handled before the next event in the main queue.
        /// </summary>
        /// <param name="childEvent">The child event to register for handling.</param>
        /// <param name="taskResult">Configures the return value.</param>
        /// <returns><c>null</c>, or a valid <see cref="Task"/>; based on the <paramref name="taskResult"/> parameter.</returns>
        Task Piggyback( IEvent childEvent, TaskResult taskResult );
    }
}
