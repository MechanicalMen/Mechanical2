using System;
using System.Threading.Tasks;

namespace Mechanical.Events
{
    /// <summary>
    /// Has events handled by their subscribers.
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of events to subscribe to.</typeparam>
        /// <param name="handler">The event handler to subscribe.</param>
        void Subscribe<T>( IEventHandler<T> handler ) where T : IEvent;

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of events to unsubscribe from.</typeparam>
        /// <param name="handler">The event handler to unsubscribe.</param>
        void Unsubscribe<T>( IEventHandler<T> handler ) where T : IEvent;

        /// <summary>
        /// Registers an event to be handled.
        /// </summary>
        /// <param name="evnt">An event to register for handling.</param>
        /// <param name="taskResult">Configures the return value.</param>
        /// <returns><c>null</c>, or a valid <see cref="Task"/>; based on the <paramref name="taskResult"/> parameter.</returns>
        Task Enqueue( IEvent evnt, TaskResult taskResult );

        /// <summary>
        /// Begins the shutdown sequence. Does nothing, if it already begun.
        /// </summary>
        void BeginShutdown();

        /// <summary>
        /// Gets the task handing out events to subscribers. Use ContinueWith on this, if you need to do something after the queue has shut down.
        /// </summary>
        /// <value>The task handing out events to subscribers.</value>
        Task EventTask { get; }
    }
}
