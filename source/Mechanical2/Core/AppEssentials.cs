using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mechanical.Conditions;
#if !SILVERLIGHT
using Mechanical.Events;
#endif
using Mechanical.Log;
using Mechanical.MagicBag;
using Mechanical.MVVM;

namespace Mechanical.Core
{
    /// <summary>
    /// Handles basic tasks all applications need to take care of (like logging, events and unhandled exceptions).
    /// </summary>
    public abstract class AppEssentials
#if !SILVERLIGHT
 : IEventHandler<UnhandledExceptionEvent>,
   IEventHandler<EventQueueShutDownEvent>
#endif
    {
        //// NOTE: Using this class must not be a requirement!
        ////       Users should be allowed to pick and choose which parts of the  library
        ////       they want to use. It is of course up to them, to make sure all
        ////       requirements of those parts are fulfilled.

        private const string LogFileName = "log.xml";

        /// <summary>
        /// The object used for synchronization by this type, and it's inheritors.
        /// </summary>
        protected static readonly object SyncLock = new object();

        private static AppEssentials instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppEssentials"/> class.
        /// </summary>
        public AppEssentials()
        {
            var oldRef = Interlocked.CompareExchange(ref instance, this, comparand: null);
            if( oldRef.NotNullReference() )
                throw new InvalidOperationException("Only one instance of this class may be created!").StoreFileLine();
        }

        /// <summary>
        /// Gets the single instance of this type (assuming it was already initialized).
        /// </summary>
        /// <value>The single instance of this type.</value>
        public static AppEssentials Instance
        {
            get { return instance; }
        }

        #region Exceptions

        private bool appDomainHandled = false;
        private bool taskSchedulerHandled = false;
        private bool setTaskSchedulerExceptionObserved = true;

        /// <summary>
        /// Registers a handler for the <see cref="AppDomain.UnhandledException"/> event (unless one was already registered by this class).
        /// </summary>
        protected void HandleAppDomainExceptions()
        {
            lock( SyncLock )
            {
                if( !this.appDomainHandled )
                {
                    AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
                    this.appDomainHandled = true;
                }
            }
        }

        private void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e )
        {
            var exception = (Exception)e.ExceptionObject;
            HandleException(exception);
        }

        /// <summary>
        /// Registers a handler for the <see cref="TaskScheduler.UnobservedTaskException"/> event (unless one was already registered by this class).
        /// </summary>
        /// <param name="setObserved"><c>true</c> to call SetObserved() on the event arguments; otherwise, <c>false</c>.</param>
        protected void HandleUnobservedTaskExceptions( bool setObserved = true )
        {
            lock( SyncLock )
            {
                if( !this.taskSchedulerHandled )
                {
                    TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
                    this.taskSchedulerHandled = true;
                    this.setTaskSchedulerExceptionObserved = setObserved;
                }
            }
        }

        private void TaskScheduler_UnobservedTaskException( object sender, UnobservedTaskExceptionEventArgs e )
        {
            var exception = e.Exception;
            HandleException(exception);

            if( this.setTaskSchedulerExceptionObserved )
                e.SetObserved();
        }

        /// <summary>
        /// Handles the specified exception.
        /// Does not terminate the application.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void HandleException(
            Exception exception,
            [CallerFilePath] string filePath = "",
            [CallerMemberNameAttribute] string memberName = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            if( exception.NotNullReference() )
            {
                // are we allowed to handle it?
                bool canHandle;
                try
                {
                    exception.StoreFileLine(filePath, memberName, lineNumber);
                    canHandle = Instance.CanHandleException(exception);
                }
                catch( Exception ex )
                {
                    exception = new AggregateException("Failed to determine whether the exception should be handled!", ex, exception).StoreFileLine();
                    canHandle = true;
                }

                if( canHandle )
                {
                    // handle it
                    Exception mainException = null;
                    try
                    {
                        Instance.MainExceptionHandler(exception);
                    }
                    catch( Exception ex )
                    {
                        mainException = ex;
                    }

                    // ... or try again
                    if( mainException.NotNullReference() )
                    {
                        try
                        {
                            Instance.FallbackExceptionHandler(exception, mainException);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether to handle the specified exception, or skip doing so.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <returns><c>true</c> to handle the specified exception; otherwise, <c>false</c>.</returns>
        protected virtual bool CanHandleException( Exception exception )
        {
            return true;
        }

        /// <summary>
        /// Handles the specified exception.
        /// The default implementation simply logs the exception.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        protected virtual void MainExceptionHandler( Exception exception )
        {
            if( exception.NotNullReference() )
            {
                var logger = Mechanical.Log.Log.Instance;
                if( logger.NotNullReference() )
                    logger.Error("Unhandled exception caught!", exception);
            }
        }

        /// <summary>
        /// Handles exceptions when the main method to do so fails
        /// (e.g. when trying to set up logging raises an exception).
        /// Inheritors should inform the user (and fall back the
        /// base implementation in case that fails).
        /// This needs to always work.
        /// </summary>
        /// <param name="initialException">The exception to be handle.</param>
        /// <param name="mainException">The exception thrown by the main exception handlers.</param>
        protected virtual void FallbackExceptionHandler( Exception initialException, Exception mainException )
        {
            string str;
            try
            {
                var exception = new AggregateException("Failed to handle exception!", initialException, mainException).StoreFileLine();
                str = SafeString.DebugPrint(exception);
            }
            catch
            {
                str = "An unhandled exception caused a critical error!";
            }

#if !SILVERLIGHT
            Trace.WriteLine(str);
#else
                Debug.WriteLine(str);
#endif
        }

        #endregion

        #region Logging

        private ILog logger = null;

        /// <summary>
        /// Sets up in-memory logging.
        /// </summary>
        protected void StartMemoryLog()
        {
            lock( SyncLock )
            {
                if( this.logger.NullReference() )
                {
                    this.logger = new MemoryLog();
                    Mechanical.Log.Log.Set(this.logger);
                }
            }
        }

        /// <summary>
        /// Sets up a custom logger. Transfer memory logs, if there are any.
        /// </summary>
        /// <param name="customLogger">The new logger to use.</param>
        public void StartCustomLog( ILog customLogger )
        {
            if( this.logger.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            lock( SyncLock )
            {
                // check if we neet to run at all
                MemoryLog memoryLogger = null;
                if( this.logger.NotNullReference() )
                {
                    memoryLogger = this.logger as MemoryLog;
                    if( memoryLogger.NullReference() )
                        return; // another logger is already set up
                }

                // no logger, or memory logger set up:
                // setup custom logger
                this.logger = customLogger;
                Mechanical.Log.Log.Set(this.logger);

                // transmit previous log entries, if applicable
                var asLogBase = customLogger as LogBase;
                if( memoryLogger.NotNullReference() )
                {
                    if( asLogBase.NullReference() )
                        customLogger.Warn("Logger does not implement LogBase. Memory logs are present, but could not be passed on!");
                    else
                        memoryLogger.FlushLogs(asLogBase);
                    memoryLogger.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates a new log file in the specified directory.
        /// If no directory is specified, the log file is placed
        /// into the application folder.
        /// </summary>
        /// <param name="directory">The directory to create the log file at, or <c>null</c> to use the application folder.</param>
        public void StartXmlLog( string directory = null )
        {
            if( directory.NullOrEmpty() )
            {
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                directory = Path.GetFullPath(directory);
            }

            var filePath = Path.Combine(directory, LogFileName);

            if( File.Exists(filePath) )
                File.Delete(filePath);
            var newLogger = LogEntrySerializer.ToXmlFile(filePath);

            this.StartCustomLog(newLogger);
        }

        private void ReleaseLog()
        {
            lock( SyncLock )
            {
                if( this.logger.NotNullReference() )
                {
                    Mechanical.Log.Log.Set(null); // so that we will receive exceptions for logging attempts, after the shutdown.
                    var asDisposable = this.logger as IDisposable;
                    if( asDisposable.NotNullReference() )
                        asDisposable.Dispose();
                    this.logger = null;
                }
            }
        }

        #endregion

        #region UI

        private bool uiInitialized = false;

        //// NOTE: We do not set the UI thread handler from the magic bag,
        ////       in case the magic bag initialization depends on it.

        /// <summary>
        /// Initializes the UI dispatcher and scheduler from the current ones.
        /// </summary>
        protected void InitializeUIFromCurrent()
        {
#if ANDROID
            throw new NotSupportedException().StoreFileLine();
#else
            lock( SyncLock )
            {
                if( !this.uiInitialized )
                {
                    UI.SetDispatcherFromCurrent();
                    UI.SetSchedulerFromCurrent();
                    UI.SetUIThreadHandler(new UIDispatcherHandler(UI.Dispatcher, UI.Scheduler));
                    this.uiInitialized = true;
                }
            }
#endif
        }

        /// <summary>
        /// Initializes the UI dispatcher and scheduler from the default ones.
        /// </summary>
        protected void InitializeUIForConsole()
        {
            lock( SyncLock )
            {
                if( !this.uiInitialized )
                {
#if !ANDROID
                    UI.SetDispatcherForConsole();
#endif
                    UI.SetSchedulerForConsole();
                    UI.SetUIThreadHandler(new ConsoleThreadHandler(UI.Scheduler));
                    this.uiInitialized = true;
                }
            }
        }

#if ANDROID
        /// <summary>
        /// Initializes the UI dispatcher and scheduler from the default ones.
        /// </summary>
        protected void InitializeUIForAndroid( Func<Android.App.Activity> getActivity )
        {
            lock( SyncLock )
            {
                if( !this.uiInitialized )
                {
                    UI.SetSchedulerForConsole();
                    UI.SetUIThreadHandler(new AndroidUIThreadHandler(getActivity));
                    this.uiInitialized = true;
                }
            }
        }
#endif

        #endregion

        #region MagicBag

        private bool magicBagInitialized = false;

        /// <summary>
        /// Gets the main magic bag.
        /// </summary>
        /// <value>The main magic bag.</value>
        public static IMagicBag MagicBag
        {
            get { return Mechanical.MagicBag.MagicBag.Default; }
        }

        /// <summary>
        /// Creates the main magic bag.
        /// </summary>
        /// <param name="parentBag">If not <c>null</c>, it's mappings are used to create the main magic bag.</param>
        protected void InitializeMagicBag( IMagicBag parentBag = null )
        {
            lock( SyncLock )
            {
                if( !this.magicBagInitialized )
                {
                    Mechanical.MagicBag.MagicBag.CreateDefault(parentBag);
                    this.magicBagInitialized = true;
                }
            }
        }

        #endregion

        #region Events

#if !SILVERLIGHT
        private bool eventQueueInitialized = false;

        /// <summary>
        /// Gets the main event queue of the application.
        /// </summary>
        /// <value>The main event queue of the application.</value>
        public static IEventQueue EventQueue
        {
            get { return Mechanical.Events.EventQueue.Default; }
        }

        /// <summary>
        /// Creates the main event queue.
        /// </summary>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to use; or <c>null</c> to use the default scheduler. Warning: the UI scheduler will end up blocking the UI thread.</param>
        protected void InitializeEvents( TaskScheduler scheduler = null )
        {
            lock( SyncLock )
            {
                if( !this.eventQueueInitialized )
                {
                    Mechanical.Events.EventQueue.CreateDefault(scheduler);
                    EventQueue.Subscribe<UnhandledExceptionEvent>(this);
                    EventQueue.Subscribe<EventQueueShutDownEvent>(this);
                    this.eventQueueInitialized = true;
                }
            }
        }

        Task IEventHandler<UnhandledExceptionEvent>.Handle( UnhandledExceptionEvent evnt, IEventHandlerQueue queue )
        {
            var exception = evnt.Exception;
            HandleException(exception);
            return null;
        }

        Task IEventHandler<EventQueueShutDownEvent>.Handle( EventQueueShutDownEvent evnt, IEventHandlerQueue queue )
        {
            this.OnEventQueueShutDown();
            return null;
        }

        /// <summary>
        /// Called when the <see cref="EventQueueShutDownEvent"/> is being handled.
        /// </summary>
        protected virtual void OnEventQueueShutDown()
        {
            this.ReleaseLog();
        }
#endif

        #endregion

        /// <summary>
        /// Registers basic exception handlers, and creates a memory logger.
        /// No files or windows are touched.
        /// UI, MagicBag and EventQueue are not available.
        /// </summary>
        protected void SetupReadOnlyMemory()
        {
            // catch basic unhandled exceptions:
            // so that we know if something goes amiss as soon as possible
            this.HandleAppDomainExceptions();
            this.HandleUnobservedTaskExceptions();

            // start logging into memory
            // this can be easily replaced later, without any loss of information
            this.StartMemoryLog();
        }

        /// <summary>
        /// Initializes a common console application.
        /// </summary>
        /// <param name="logDirectory">The directory to create the log file at, or <c>null</c> to use the application folder.</param>
        /// <param name="parentBag">Optional mappings overriding any of the default ones.</param>
        protected void SetupConsole( string logDirectory = null, IMagicBag parentBag = null )
        {
            // memory logging may or may not have been set up
            // instead of checking, just create a common baseline
            this.SetupReadOnlyMemory();

            // start logging, so that we can have
            // something other then screenshots when debugging
            // (this may have been set up as well, in which case it will be silently skipped)
            this.StartXmlLog(logDirectory);

            // specify UI Dispatcher and TaskScheduler:
            // do these first, in case some mappings need them next
            this.InitializeUIForConsole();

            // initialize our IoC container, so that
            // we will have basic resources available
            this.InitializeMagicBag(parentBag);

            // spin up the event queue
#if !SILVERLIGHT
            this.InitializeEvents();
#endif
        }

        /// <summary>
        /// Initializes a common GUI application.
        /// </summary>
        /// <param name="logDirectory">The directory to create the log file at, or <c>null</c> to use the application folder.</param>
        /// <param name="parentBag">Optional mappings overriding any of the default ones.</param>
        protected void SetupBasicWindow( string logDirectory = null, IMagicBag parentBag = null )
        {
            // memory logging may or may not have been set up
            // instead of checking, just create a common baseline
            this.SetupReadOnlyMemory();

            // start logging, so that we can have
            // something other then screenshots when debugging
            // (this may have been set up as well, in which case it will be silently skipped)
            this.StartXmlLog(logDirectory);

            // specify UI Dispatcher and TaskScheduler:
            // do these first, in case some mappings need them next
            this.InitializeUIFromCurrent();

            // initialize our IoC container, so that
            // we will have basic resources available
            this.InitializeMagicBag(parentBag);

            // spin up the event queue
#if !SILVERLIGHT
            this.InitializeEvents();
#endif
        }
    }
}

//// TODO: make IEvent handlers private-instance, and create protected-virtual methods where necessary
