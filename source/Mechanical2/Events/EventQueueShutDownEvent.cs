using System;

namespace Mechanical.Events
{
    /// <summary>
    /// This event is raised by the <see cref="IEventQueue"/> to indicate that it is being shut down.
    /// This is always the last event in the queue. No more events can be enqueued or piggybacked.
    /// </summary>
    public class EventQueueShutDownEvent : IEvent
    {
        internal EventQueueShutDownEvent()
        {
        }
    }
}
