using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Mechanical.Bootstrap;
using Mechanical.Common.Bootstrap;
using Mechanical.Common.Logs;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.Events;
using Mechanical.IO.FileSystem;
using Mechanical.MagicBag;
using Mechanical.MVVM;

namespace Mechanical.Common
{
    /// <summary>
    /// Helps setting up and accessing the most basic resources of the application.
    /// </summary>
    public sealed class MechanicalApp : AppCore
    {
        private static readonly object InitializationSync = new object();
        private static bool safeModeInitialized = false;
        private static bool fullyInitialized = false;

        private MechanicalApp()
            : base()
        {
        }

        #region Logging

        /// <summary>
        /// Creates a new <see cref="AdvancedLogEntrySerializer"/> instance.
        /// </summary>
        /// <param name="directory">The directory to create the log files at, or <c>null</c> to use the application folder.</param>
        /// <param name="logFilePrefix">The first part of log file names. You can use this to identify the application generating them.</param>
        /// <returns>The new <see cref="AdvancedLogEntrySerializer"/> instance.</returns>
        private static AdvancedLogEntrySerializer CreateAdvancedLogEntrySerializer( string directory = null, string logFilePrefix = "log" )
        {
            if( directory.NullOrEmpty() )
            {
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                directory = Path.GetFullPath(directory);
            }

            var fileSystem = new DirectoryFileSystem(directory, escapeFileNames: true); // we escape file names to have file extensions
            return new AdvancedLogEntrySerializer(
                fileSystem,
                maxFileAge: null,
                maxAppInstanceCount: 3,
                maxTotalFileSize: null,
                singleFileSizeThreshold: null,
                logFilePrefix: logFilePrefix);
        }

        #endregion

        #region Window

        private class WindowEventsSubscriber : IEventHandler<EventQueueShutDownEvent>
        {
            private readonly Window window;
            private bool canWindowClose = false;

            internal WindowEventsSubscriber( Window wnd )
            {
                this.window = wnd;
                this.window.Closing += this.OnWindowClosing;
            }

            private void OnWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
            {
                // don't close the window, unless the event queue has already shut down
                if( !this.canWindowClose )
                {
                    e.Cancel = true;
                    EventQueue.BeginShutdown();
                }
            }

            public Task Handle( EventQueueShutDownEvent evnt, IEventHandlerQueue queue )
            {
                this.canWindowClose = true;
                UI.Invoke(() => this.window.Close());
                return null;
            }
        }

        private static WindowEventsSubscriber windowSubscriber;

        /// <summary>
        /// Makes sure the specified <see cref="Window"/> is only closed, when the main event queue has finished shutting down.
        /// The main event queue shutdown signal is automatically sent, when the <see cref="Window"/> is about to be closed.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to link to the lifespan of the main event queue.</param>
        private static void LinkWindowToEventQueue( Window window )
        {
            if( window.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( windowSubscriber.NotNullReference() )
                throw new InvalidOperationException().StoreFileLine();

            windowSubscriber = new WindowEventsSubscriber(window);
            EventQueue.Subscribe<EventQueueShutDownEvent>(windowSubscriber);
        }

        #endregion

        private static IMagicBag BuildMagicBag()
        {
            return new Mechanical.MagicBag.MagicBag.Inherit(
                AppCore.MagicBag,
                Map<IDateTimeProvider>.To(() => new DateTimeProvider()).AsSingleton());
        }

        /// <summary>
        /// Initializes the core framework for safe mode.
        /// That means that no resources, not the internet,
        /// and not even the file system is accessible.
        /// The UI thread is not accessible.
        /// Only the most basic MagicBag mappings are set.
        /// </summary>
        /// <returns><c>true</c> if this is the first time this method was called, and initialization was successful; otherwise, <c>false</c>.</returns>
        public static bool InitializeSafeMode()
        {
            if( safeModeInitialized )
                return false;

            lock( InitializationSync )
            {
                if( !safeModeInitialized )
                {
                    // exceptions sources
                    AppCore.Register(new AppDomainExceptionSource());
                    AppCore.Register(new UnobservedTaskExceptionSource());

                    // exception sinks
                    var logSink = new LogExceptionSink();
                    AppCore.Register(logSink, isFallback: false);
                    AppCore.Register(logSink, isFallback: true);
                    AppCore.Register(new TraceExceptionSink(), isFallback: true);

                    safeModeInitialized = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Initializes all parts of the core framework.
        /// </summary>
        /// <param name="logFilePrefix">The first part of log file names. You can use this to identify the application generating them.</param>
        /// <returns><c>true</c> if this is the first time this method was called, and initialization was successful; otherwise, <c>false</c>.</returns>
        public static bool InitializeService( string logFilePrefix = "log" )
        {
            if( fullyInitialized )
                return false;

            // initialize previous stage
            InitializeSafeMode();

            lock( InitializationSync )
            {
                if( !fullyInitialized )
                {
                    // magic bag
                    AppCore.MagicBag = BuildMagicBag();

                    // logging
                    AppCore.Log = CreateAdvancedLogEntrySerializer(directory: null, logFilePrefix: logFilePrefix);

                    // UI
                    AppCore.Register((IUIThreadHandler)null);

                    fullyInitialized = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Initializes all parts of the core framework.
        /// </summary>
        /// <param name="app">The <see cref="System.Windows.Application"/> raising the DispatcherUnhandledException event.</param>
        /// <param name="window">The <see cref="Window"/> to link to the lifespan of the main event queue.</param>
        /// <param name="logFilePrefix">The first part of log file names. You can use this to identify the application generating them.</param>
        /// <returns><c>true</c> if this is the first time this method was called, and initialization was successful; otherwise, <c>false</c>.</returns>
        public static bool InitializeWindow( Application app, Window window, string logFilePrefix = "log" )
        {
            if( fullyInitialized )
                return false;

            // initialize previous stage
            InitializeSafeMode();

            lock( InitializationSync )
            {
                if( !fullyInitialized )
                {
                    // exceptions sources
                    AppCore.Register(new DispatcherExceptionSource(app));

                    // exception sinks
                    AppCore.Register(new MessageBoxExceptionSink(), isFallback: false);

                    // magic bag
                    AppCore.MagicBag = BuildMagicBag();

                    // logging
                    AppCore.Log = CreateAdvancedLogEntrySerializer(directory: null, logFilePrefix: logFilePrefix);

                    // UI
                    AppCore.Register(new DispatcherUIHandler(Dispatcher.CurrentDispatcher));

                    // main window
                    LinkWindowToEventQueue(window);

                    fullyInitialized = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
