using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.Events
{
    /// <summary>
    /// Has events handled by their subscribers.
    /// </summary>
    public class EventQueue : IEventQueue
    {
        #region HandlerWrapper

        private abstract class HandlerWrapper
        {
            internal abstract Type EventType { get; }

            internal abstract object TemporaryStrongRef { get; }

            internal abstract IEventHandler<IEvent> StrongHandlerWrapper { get; }
        }

        private class HandlerWrapper<T> : HandlerWrapper, IEventHandler<IEvent>
            where T : IEvent
        {
#if !MECHANICAL_NET4CP && !SILVERLIGHT
            private readonly WeakReference<IEventHandler<T>> weakRef;
            private IEventHandler<T> temporaryStrongRef = null;

            internal HandlerWrapper( IEventHandler<T> handler )
            {
                this.weakRef = new WeakReference<IEventHandler<T>>(handler);
            }

            internal override object TemporaryStrongRef
            {
                get
                {
                    IEventHandler<T> tmp;
                    if( this.weakRef.TryGetTarget(out tmp) )
                        return tmp;
                    else
                        return null;
                }
            }

            internal override IEventHandler<IEvent> StrongHandlerWrapper
            {
                get
                {
                    if( this.weakRef.TryGetTarget(out this.temporaryStrongRef) )
                        return this;
                    else
                        return null;
                }
            }
#else
            private readonly WeakReference weakRef;
            private IEventHandler<T> temporaryStrongRef = null;

            internal HandlerWrapper( IEventHandler<T> handler )
            {
                this.weakRef = new WeakReference(handler);
            }

            internal override object TemporaryStrongRef
            {
                get { return this.weakRef.Target; }
            }

            internal override IEventHandler<IEvent> StrongHandlerWrapper
            {
                get
                {
                    this.temporaryStrongRef = (IEventHandler<T>)this.weakRef.Target;
                    if( this.temporaryStrongRef.NotNullReference() )
                        return this;
                    else
                        return null;
                }
            }
#endif
            internal override Type EventType
            {
                get { return typeof(T); }
            }

            public Task Handle( IEvent evnt, IEventHandlerQueue queue )
            {
                Task result = null;
                if( this.temporaryStrongRef.NotNullReference() )
                {
                    result = this.temporaryStrongRef.Handle((T)evnt, queue);
                    this.temporaryStrongRef = null;
                }
                return result;
            }
        }

        #endregion

        #region EventWrapper

        private class EventWrapper
        {
            internal readonly IEvent Event;
            private readonly TaskCompletionSource<object> tsc;
            private readonly TaskResult taskResult;

            internal EventWrapper( IEvent evnt, TaskResult taskResult )
            {
                Ensure.That(evnt).NotNull();
                Ensure.That(taskResult).IsDefined();

                this.Event = evnt;
                this.taskResult = taskResult;
                this.tsc = this.taskResult == TaskResult.Null ? null : new TaskCompletionSource<object>();
            }

            internal Task Task
            {
                get { return this.tsc.NullReference() ? null : this.tsc.Task; }
            }

            internal UnhandledExceptionEvent SetCompletedOrExceptionAsync( List<Exception> unhandledExceptions )
            {
                if( unhandledExceptions.NullReference() )
                    throw new ArgumentNullException("unhandledExceptions").StoreFileLine();

                if( unhandledExceptions.Count == 0 )
                {
                    if( this.taskResult != TaskResult.Null )
                        this.tsc.SetResult(null);
                }
                else
                {
                    Exception ex;
                    if( unhandledExceptions.Count == 1 )
                        ex = unhandledExceptions[0];
                    else
                        ex = new AggregateException("Event handlers threw exceptions!", unhandledExceptions).StoreFileLine();

                    if( this.taskResult == TaskResult.CompletedOrException )
                    {
                        this.tsc.SetException(ex);
                    }
                    else
                    {
                        if( this.taskResult == TaskResult.Completed )
                            this.tsc.SetResult(null);

                        return new UnhandledExceptionEvent(ex);
                    }
                }

                return null;
            }
        }

        #endregion

        #region Private Fields

        private const int TRUE = 1;
        private const int FALSE = 0;

        private static readonly EventQueueShuttingDownEvent ShuttingDownEvent = new EventQueueShuttingDownEvent();
        private static readonly EventQueueShutDownEvent ShutDownEvent = new EventQueueShutDownEvent();
        private static readonly ShutDownEventHandlerQueue ShutDownHandlerQueue = new ShutDownEventHandlerQueue();

        private readonly object subscriberLock = new object();
        private readonly List<HandlerWrapper> subscribers = new List<HandlerWrapper>();
        private readonly Task workerTask;
        private BlockingCollection<EventWrapper> queue = new BlockingCollection<EventWrapper>();
        private int shutdownStarted = FALSE;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventQueue"/> class.
        /// </summary>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to use; or <c>null</c> to use the default scheduler. Warning: the UI scheduler will end up blocking the UI thread.</param>
        public EventQueue( TaskScheduler scheduler = null )
        {
            if( scheduler.NullReference() )
                scheduler = TaskScheduler.Default;

            this.workerTask = Task.Factory.StartNew(
                this.TaskBody,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                scheduler);
        }

        #endregion

        #region Private Methods

        private int FindHandlerIndex_NotLocked( object handler, Type eventType )
        {
            object tmpStrongRef;
            for( int i = 0; i < this.subscribers.Count; )
            {
                if( Reveal.CanAssignTo(this.subscribers[i].EventType, eventType) )
                {
                    tmpStrongRef = this.subscribers[i].TemporaryStrongRef;
                    if( tmpStrongRef.NullReference() )
                    {
                        this.subscribers.RemoveAt(i);
                    }
                    else
                    {
                        if( object.ReferenceEquals(handler, tmpStrongRef) )
                            return i;
                        else
                            ++i;
                    }
                }
                else
                    ++i;
            }
            return -1;
        }

        private List<IEventHandler<IEvent>> GatherSubscribers_NotLocked( Type eventType )
        {
            var eventSubscribers = new List<IEventHandler<IEvent>>();

            HandlerWrapper wrapper;
            IEventHandler<IEvent> strong;
            for( int i = 0; i < this.subscribers.Count; )
            {
                wrapper = this.subscribers[i];

                if( Reveal.CanAssignTo(wrapper.EventType, eventType) )
                {
                    strong = wrapper.StrongHandlerWrapper;
                    if( strong.NotNullReference() )
                    {
                        eventSubscribers.Add(strong);
                        ++i;
                    }
                    else
                        this.subscribers.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            return eventSubscribers;
        }

        private async Task HandleEventAsync( EventWrapper eventWrapper )
        {
            // get applicable event subscribers
            List<IEventHandler<IEvent>> eventSubscribers;
            lock( this.subscriberLock )
                eventSubscribers = this.GatherSubscribers_NotLocked(eventWrapper.Event.GetType());

            // handle event
            IEventHandlerQueue childQueue;
            if( !object.ReferenceEquals(eventWrapper.Event, ShutDownEvent) )
                childQueue = new OneTimeEventHandlerQueue();
            else
                childQueue = ShutDownHandlerQueue;

            var unhandledExceptions = new List<Exception>();
            Task task;
            for( int i = 0; i < eventSubscribers.Count; ++i )
            {
                try
                {
                    task = eventSubscribers[i].Handle(eventWrapper.Event, childQueue);
                    if( task.NotNullReference() )
                        await task;
                }
                catch( Exception ex )
                {
                    ex.Store("ExceptionSource", "EventQueue subscriber");
                    unhandledExceptions.Add(ex);
                }
            }

            // handle accumulated child events
            if( !object.ReferenceEquals(eventWrapper.Event, ShutDownEvent) )
            {
                var q = (OneTimeEventHandlerQueue)childQueue;
                for( int i = 0; i < q.Count; ++i )
                {
                    var childEventWrapper = q[i];
                    await this.HandleEventAsync(childEventWrapper);
                }
            }

            // the event is handled, notify listeners
            var unhandledExceptionEvent = eventWrapper.SetCompletedOrExceptionAsync(unhandledExceptions);
            if( unhandledExceptionEvent.NotNullReference() )
                await this.HandleEventAsync(new EventWrapper(unhandledExceptionEvent, TaskResult.Null));

            // shutting down event?
            if( object.ReferenceEquals(eventWrapper.Event, ShuttingDownEvent) )
                this.queue.CompleteAdding();
        }

        private class OneTimeEventHandlerQueue : ReadOnlyList.Wrapper<EventWrapper>, IEventHandlerQueue
        {
            //// NOTE: since subscribers are not called in parallel, there ise no need for 'locking'

            public Task Piggyback( IEvent childEvent, TaskResult taskResult )
            {
                var wrapper = new EventWrapper(childEvent, taskResult);
                this.Items.Add(wrapper);
                return wrapper.Task;
            }
        }

        private class ShutDownEventHandlerQueue : IEventHandlerQueue
        {
            public Task Piggyback( IEvent childEvent, TaskResult taskResult )
            {
                throw new InvalidOperationException("No events can be piggybacked unto an EventQueueShutDownEvent!").Store("childEvent", childEvent).Store("taskResult", taskResult);
            }
        }

        private void TaskBody()
        {
#if DEBUG
            try
            {
#endif
                Task t;
                foreach( var eventWrapper in this.queue.GetConsumingEnumerable().Concat(new EventWrapper[] { new EventWrapper(ShutDownEvent, TaskResult.Null) }) )
                {
                    t = this.HandleEventAsync(eventWrapper);
                    t.Wait();
                }

                this.queue.Dispose();
                this.queue = null;

                this.subscribers.Clear();
#if DEBUG
            }
            catch( Exception ex )
            {
                ex.NotNullReference(); // makes compiler happy
                System.Diagnostics.Debugger.Break();
            }
#endif
        }

        #endregion

        #region IEventQueue

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of events to subscribe to.</typeparam>
        /// <param name="handler">The event handler to subscribe.</param>
        public void Subscribe<T>( IEventHandler<T> handler )
            where T : IEvent
        {
            Ensure.That(this.queue).NotNull(() => new InvalidOperationException("EventQueue already shut down!"));
            Ensure.That(handler).NotNull();

            lock( this.subscriberLock )
            {
                if( this.FindHandlerIndex_NotLocked(handler, typeof(T)) == -1 )
                    this.subscribers.Add(new HandlerWrapper<T>(handler));
            }
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of events to unsubscribe from.</typeparam>
        /// <param name="handler">The event handler to unsubscribe.</param>
        public void Unsubscribe<T>( IEventHandler<T> handler )
            where T : IEvent
        {
            Ensure.That(this.queue).NotNull(() => new InvalidOperationException("EventQueue already shut down!"));
            Ensure.That(handler).NotNull();

            lock( this.subscriberLock )
            {
                int index = this.FindHandlerIndex_NotLocked(handler, typeof(T));
                if( index != -1 )
                    this.subscribers.RemoveAt(index);
            }
        }

        /// <summary>
        /// Registers an event to be handled. Does nothing, after the <see cref="EventQueueShuttingDownEvent"/> has been fully handled.
        /// </summary>
        /// <param name="evnt">An event to register for handling.</param>
        /// <param name="taskResult">Configures the return value.</param>
        /// <returns><c>null</c>, or a valid <see cref="Task"/>; based on the <paramref name="taskResult"/> parameter.</returns>
        public Task Enqueue( IEvent evnt, TaskResult taskResult )
        {
            try
            {
                var eventWrapper = new EventWrapper(evnt, taskResult);
                this.queue.Add(eventWrapper);
                return eventWrapper.Task;
            }
            catch( ArgumentException )
            {
                // bad arguments (thrown by EventWrapper)
                throw;
            }
            catch( InvalidOperationException )
            {
                // adding disabled on queue, or queue disposed
                return taskResult == TaskResult.Null ? null : Task.Factory.StartNew(() => { });
            }
            catch( NullReferenceException )
            {
                // queue null
                return taskResult == TaskResult.Null ? null : Task.Factory.StartNew(() => { });
            }
        }

        /// <summary>
        /// Begins the shutdown sequence. Does nothing, if it already begun.
        /// </summary>
        public void BeginShutdown()
        {
            if( Interlocked.CompareExchange(ref this.shutdownStarted, TRUE, comparand: FALSE) == FALSE )
            {
                this.Enqueue(ShuttingDownEvent, TaskResult.Null);
            }
        }

        /// <summary>
        /// Gets the task handing out events to subscribers. Use ContinueWith on this, if you need to do something after the queue has shut down.
        /// </summary>
        /// <value>The task handing out events to subscribers.</value>
        public Task EventTask
        {
            get { return this.workerTask; }
        }

        #endregion

        #region Mappings

        internal static Mapping[] GetMappings()
        {
            return new Mapping[]
            {
                Map<IEventQueue>.To(() => new EventQueue()).AsSingleton(),
            };
        }

        #endregion
    }

    //// TODO: GenericEvent(id, payload)
}
