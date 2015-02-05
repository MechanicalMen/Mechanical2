using System;

namespace Mechanical.Events
{
    /// <summary>
    /// This event is raised by the <see cref="IEventQueue"/> to indicate a request to shut it down.
    /// The request may be cancelled by event handlers. The event queue is in no way restricted by this event.
    /// This is the only kind of event, whose handling may be skipped by the event queue. This only
    /// happens, if the shutdown of the event queue already begun, by the time it would be handled.
    /// </summary>
    public class EventQueueShutDownRequestEvent : IEvent
    {
        internal EventQueueShutDownRequestEvent()
        {
            this.Reset();
        }

        internal void Reset()
        {
            this.CanShutDown = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event queue can begin shutting down.
        /// May be ignored, if the event queue already begun shutting down.
        /// </summary>
        /// <value><c>true</c> if the event queue can begin shutting down; otherwise, <c>false</c>.</value>
        public bool CanShutDown
        {
            get;
            set;
        }
    }
}
