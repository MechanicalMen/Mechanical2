using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mechanical.Bootstrap;
using Mechanical.Conditions;
#if !SILVERLIGHT
using Mechanical.Events;
#endif
using Mechanical.Logs;
using Mechanical.MagicBag;
using Mechanical.MVVM;

namespace Mechanical.Core
{
    /// <summary>
    /// Helps setting up and accessing the most basic resources of the application.
    /// </summary>
    public class AppCore
    {
        //// NOTE: the class is not 'static', so that it's static
        ////       members can be inherited. Inheritors should
        ////       have private constructors, never used.

        #region Constructor

        static AppCore()
        {
            InitializeLogging();
#if !SILVERLIGHT
            InitializeEventQueue();
#endif
            InitializeMagicBag();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCore"/> class.
        /// </summary>
        protected AppCore()
        {
            throw new NotSupportedException("Never call the constructor of this class!").StoreFileLine();
        }

        #endregion

        #region Exceptions

        /// <summary>
        /// Used to synchronize exception handling.
        /// </summary>
        private static readonly object ExceptionSync = new object();

        #region Exception sources

        //// NOTE: we keep a reference of all sources, so that the GC does not collect them.

        private static readonly List<IExceptionSource> ExceptionSources = new List<IExceptionSource>();

        /// <summary>
        /// Registers an <see cref="IExceptionSource"/> object.
        /// </summary>
        /// <param name="source">The <see cref="IExceptionSource"/> instance to register.</param>
        protected static void Register( IExceptionSource source )
        {
            if( source.NullReference() )
                return;

            lock( ExceptionSync )
                ExceptionSources.Add(source);

            source.ExceptionCaught += OnExceptionCaught;
        }

        private static void OnExceptionCaught( Exception exception )
        {
            //// NOTE: multiple exception sources may call this method in parallel
            HandleException(exception);
        }

        #endregion

        #region Exception sinks

        //// NOTE: "fallback" exception sinks are only used, when at least one
        ////       of the "main" sinks fails.

        private static readonly List<IExceptionSink> MainExceptionSinks = new List<IExceptionSink>();
        private static readonly List<IExceptionSink> FallbackExceptionSinks = new List<IExceptionSink>();

        /// <summary>
        /// Registers an <see cref="IExceptionSink"/> object.
        /// </summary>
        /// <param name="sink">The <see cref="IExceptionSink"/> instance to register.</param>
        /// <param name="isFallback"><c>true</c> if this sink is only to be invoked, if one of the main sinks fails; <c>false</c> to always try to invoke it.</param>
        protected static void Register( IExceptionSink sink, bool isFallback )
        {
            if( sink.NullReference() )
                return;

            lock( ExceptionSync )
            {
                if( !isFallback )
                    MainExceptionSinks.Add(sink);
                else
                    FallbackExceptionSinks.Add(sink);
            }
        }

        #endregion

        #region HandleException

        private static readonly List<Exception> SinkExceptions = new List<Exception>();

        /// <summary>
        /// Processes an unhandled exception.
        /// </summary>
        /// <param name="exception">The unhandled exception to process.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void HandleException(
            Exception exception,
            [CallerFilePath] string filePath = "",
            [CallerMemberNameAttribute] string memberName = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            if( exception.NullReference() )
                return;

            // record where the unhandled exception was caught
            exception.StoreFileLine(filePath, memberName, lineNumber);

            lock( ExceptionSync )
            {
                // try all "main" handlers
                for( int i = 0; i < MainExceptionSinks.Count; ++i )
                {
                    try
                    {
                        MainExceptionSinks[i].Handle(exception);
                    }
                    catch( Exception ex )
                    {
                        SinkExceptions.Add(ex);
                    }
                }

                // invoke "fallback" handlers, only if necessary
                if( SinkExceptions.Count != 0 )
                {
                    exception = new AggregateException("One or more of the main exception sinks failed!", SinkExceptions.ToArray());

                    for( int i = 0; i < FallbackExceptionSinks.Count; ++i )
                    {
                        try
                        {
                            FallbackExceptionSinks[i].Handle(exception);
                        }
                        catch
                        {
                        }
                    }

                    SinkExceptions.Clear();
                }
            }
        }

        #endregion

        #endregion

        #region Logging

        //// NOTE: the current logger is never 'null'.

        private class ExceptionLog : LogBase
        {
            public override void Log( LogEntry entry )
            {
                throw new InvalidOperationException("The application has already shut down. No more logging is possible!").StoreFileLine(entry.FileName, entry.MemberName, entry.LineNumber);
            }
        }

#if !SILVERLIGHT
        private static readonly ManualResetEventSlim LogAccess = new ManualResetEventSlim(initialState: true); // signaled
#else
        private static readonly ManualResetEvent LogAccess = new ManualResetEvent(initialState: true); // signaled
#endif
        private static ILog currentLogger;

        private static void InitializeLogging()
        {
            currentLogger = new MemoryLog();
            currentLogger.Debug("Memory logger initialized!");
        }

        private static void ReleaseLogger()
        {
            lock( LogAccess )
            {
                if( currentLogger.NotNullReference() )
                {
                    var asDisposable = currentLogger as IDisposable;
                    if( asDisposable.NotNullReference() )
                        asDisposable.Dispose();

                    currentLogger = new ExceptionLog();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current logger in use.
        /// </summary>
        /// <value>The current logger in use.</value>
        public static ILog Log
        {
            get
            {
#if !SILVERLIGHT
                LogAccess.Wait();
#else
                LogAccess.WaitOne();
#endif
                return currentLogger;
            }
            set
            {
                if( value.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                // is the previous logger a memory log?
                var previousMemoryLog = currentLogger as MemoryLog;
                var previousDisposableLog = currentLogger as IDisposable;

                // block getter until replacement finishes
                LogAccess.Reset();
                try
                {
                    // set new logger
                    Interlocked.Exchange(ref currentLogger, value);

                    // transfer previous log entries, if applicable
                    if( previousMemoryLog.NotNullReference() )
                    {
                        var asLogBase = currentLogger as LogBase;
                        if( asLogBase.NullReference() )
                        {
                            currentLogger.Warn("Logger does not implement LogBase. Memory logs are present, but could not be passed on!");
                        }
                        else
                        {
                            previousMemoryLog.FlushLogs(asLogBase);
                            currentLogger.Debug("Memory logger was replaced. All previous logs were transferred.");
                        }
                    }

                    // dispose of old logger, if applicable
                    if( previousDisposableLog.NotNullReference() )
                        previousDisposableLog.Dispose();
                }
                finally
                {
                    // unblock getter
                    LogAccess.Set();
                }
            }
        }

        #endregion

        #region UI

        //// NOTE: Some apps have no traditional UI (web services, console apps, background processes, ... etc.)
        ////       and therefore they have no concept (or need) of a UI thread.

        private const int HasUITrue = 1;
        private const int HasUIFalse = 0;
        private const int HasUIUninitialized = -1;

        private static int hasUI = HasUIUninitialized;

        /// <summary>
        /// Gets a value indicating whether this app supports a traditional UI interface.
        /// <c>null</c> means that this is unknown for now.
        /// </summary>
        /// <value>Indicates whether this app supports a traditional UI interface; or <c>null</c> if that is currently unknown.</value>
        public static bool? HasUI
        {
            get
            {
                switch( hasUI )
                {
                case HasUITrue:
                    return true;
                case HasUIFalse:
                    return false;
                default:
                    return null;
                }
            }
        }

        /// <summary>
        /// Registers an <see cref="IUIThreadHandler"/> object.
        /// </summary>
        /// <param name="uiThreadHandler">The <see cref="IUIThreadHandler"/> instance to register.</param>
        protected static void Register( IUIThreadHandler uiThreadHandler )
        {
            // update hasUI property
            int newValue = uiThreadHandler.NotNullReference() ? HasUITrue : HasUIFalse;
            var oldValue = Interlocked.CompareExchange(ref hasUI, newValue, comparand: HasUIUninitialized);
            if( oldValue != HasUIUninitialized )
                throw new InvalidOperationException("UI variables should be set exactly once!").StoreFileLine();

            // set app UI
            UI.UIThreadHandler = uiThreadHandler;
        }

        #endregion

        #region MagicBag

        //// NOTE: the current magic bag is never 'null'.

        private static IMagicBag currentMagicBag;

        private static void InitializeMagicBag()
        {
            // NOTE: specify default mappings here
            var mappings = new List<Mapping>();
            mappings.Add(Map<DateTime>.To(() => DateTime.UtcNow).AsTransient());
            mappings.AddRange(Mechanical.DataStores.BasicSerialization.GetMappings());
            mappings.AddRange(Mechanical.DataStores.Node.DataStoreNode.GetMappings());
#if !SILVERLIGHT
            mappings.Add(Map<IEventQueue>.To(() => AppCore.EventQueue).AsTransient());
#endif

            MagicBag = new Mechanical.MagicBag.MagicBag.Basic(mappings.ToArray(), MappingGenerators.Defaults);
        }

        /// <summary>
        /// Gets or sets the current magic bag in use.
        /// </summary>
        /// <value>The current magic bag in use.</value>
        public static IMagicBag MagicBag
        {
            get
            {
                return currentMagicBag;
            }
            set
            {
                if( value.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                // replace magic bag
                Interlocked.Exchange(ref currentMagicBag, value);

                //// NOTE: We do not dispose of the old magic bag, in case it was simply "extended".
                ////       Any disposal of it needs to be done by the calling code.
                ////       A race condition is possible, when getting the old magic bag, to replace it
                ////       with an extended version. It seemed like over-engineering to prepare for it.
            }
        }

        #endregion

        #region EventQueue
#if !SILVERLIGHT
        private class EventQueueSubscriber : IEventHandler<UnhandledExceptionEvent>,
                                             IEventHandler<EventQueueShutDownEvent>
        {
            public Task Handle( UnhandledExceptionEvent evnt, IEventHandlerQueue queue )
            {
                HandleException(evnt.Exception);
                return null;
            }

            public Task Handle( EventQueueShutDownEvent evnt, IEventHandlerQueue queue )
            {
                ReleaseLogger();
                return null;
            }
        }

        private static readonly EventQueue Queue = new EventQueue();
        private static readonly EventQueueSubscriber EventSubscriber = new EventQueueSubscriber(); // event queue keeps only weak references

        private static void InitializeEventQueue()
        {
            EventQueue.Subscribe<UnhandledExceptionEvent>(EventSubscriber);
            EventQueue.Subscribe<EventQueueShutDownEvent>(EventSubscriber);
        }

        /// <summary>
        /// Gets the main event queue of the application.
        /// </summary>
        /// <value>The main event queue of the application.</value>
        public static IEventQueue EventQueue
        {
            get { return Queue; }
        }
#endif
        #endregion

        //// TODO: initialization stages:
        ////        - in memory only (direct call)
        ////        - most basic resources only (file system, file logging, internet availability) (async call)
        ////        - <optional splash screen>
        ////        - final initialization (with optional magic bag extensions, ... etc.)
    }
}
