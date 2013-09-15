using System;

namespace Mechanical.Events
{
    /// <summary>
    /// This event is raised by an <see cref="IEventQueue"/> to indicate that it is being shut down.
    /// No more events can be enqueued, once it has been fully handled. Piggybacking is still allowed.
    /// </summary>
    public class EventQueueShuttingDownEvent : IEvent
    {
        internal EventQueueShuttingDownEvent()
        {
        }
    }
}
