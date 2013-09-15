using System;
using System.Threading.Tasks;

namespace Mechanical.Events
{
    /// <summary>
    /// Encapsulates the ability to handle events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of events to handle. Must implement <see cref="IEvent"/>.</typeparam>
    public interface IEventHandler<T>
        where T : IEvent
    {
        /// <summary>
        /// Handles the specified <see cref="IEvent"/>.
        /// </summary>
        /// <param name="evnt">The event to handle.</param>
        /// <param name="queue">The queue to use, to piggyback events.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation; or <c>null</c> for synchronous operations.</returns>
        Task Handle( T evnt, IEventHandlerQueue queue );
    }
}
