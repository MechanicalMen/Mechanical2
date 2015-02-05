using System;

namespace Mechanical.Events
{
    /// <summary>
    /// This event is raised by an <see cref="IEventQueue"/> to indicate that it is being shut down.
    /// No more events can be enqueued, after this event and all piggybacked events
    /// have finished being handled.
    /// </summary>
    public class EventQueueShuttingDownEvent : IEvent
    {
        internal EventQueueShuttingDownEvent()
        {
        }
    }
}
