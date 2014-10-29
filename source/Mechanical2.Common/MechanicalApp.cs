using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Mechanical.Common.Log;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.Events;
using Mechanical.IO.FileSystem;
using Mechanical.MVVM;

namespace Mechanical.Common
{
    /// <summary>
    /// Helps initialize and keep track of commonly used application wide resources.
    /// </summary>
    public class MechanicalApp : AppEssentials, IEventHandler<EventQueueShutDownEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MechanicalApp"/> class.
        /// </summary>
        protected MechanicalApp()
            : base()
        {
        }

        /// <summary>
        /// Gets the single instance of the <see cref="MechanicalApp"/> class.
        /// </summary>
        /// <value>The single instance of the <see cref="MechanicalApp"/> class.</value>
        public static new MechanicalApp Instance
        {
            get { return (MechanicalApp)AppEssentials.Instance; }
        }

        #region Exceptions

        private bool dispatcherExceptionsHandled = false;
        private bool setDispatcherExceptionHandled = true;

        /// <summary>
        /// Registers a handler for the <see cref="Application.DispatcherUnhandledException"/> event (unless one was already registered by this class).
        /// </summary>
        /// <param name="currentApp">The <see cref="System.Windows.Application"/> object that raises the event.</param>
        /// <param name="setHandled">The value to set the Handled property on the event arguments.</param>
        protected void HandleDispatcherExceptions( Application currentApp, bool setHandled = true )
        {
            if( currentApp.NullReference() )
                throw new ArgumentNullException("currentApp").StoreFileLine();

            lock( AppEssentials.SyncLock )
            {
                if( !this.dispatcherExceptionsHandled )
                {
                    currentApp.DispatcherUnhandledException += this.CurrentApp_DispatcherUnhandledException;
                    this.dispatcherExceptionsHandled = true;
                    this.setDispatcherExceptionHandled = setHandled;
                }
            }
        }

        private void CurrentApp_DispatcherUnhandledException( object sender, DispatcherUnhandledExceptionEventArgs e )
        {
            var exception = e.Exception;
            AppEssentials.HandleException(exception);
            e.Handled = this.setDispatcherExceptionHandled;
        }

        /// <summary>
        /// Handles the specified exception.
        /// Shows a short message box and logs the full exception.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        protected override void MainExceptionHandler( Exception exception )
        {
            if( exception.NotNullReference() )
            {
                // show a message box
                Exception msgBoxException = null;
                try
                {
                    UI.Invoke(() =>
                    {
                        MessageBox.Show(
                            SafeString.DebugFormat("An unhandled exception of type '{0}' was caught and logged!", exception.GetType()),
                            "Unhandled exception!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                }
                catch( Exception ex )
                {
                    msgBoxException = ex;
                    msgBoxException.StoreFileLine();
                }

                // invoke other handlers as well (e.g. logging)
                Exception baseException = null;
                try
                {
                    var e = exception;
                    if( msgBoxException.NotNullReference() )
                        e = new AggregateException("Failed to show MessageBox!", msgBoxException, exception).StoreFileLine();

                    base.MainExceptionHandler(e);
                }
                catch( Exception ex )
                {
                    baseException = ex;
                    baseException.StoreFileLine();
                }

                // rethrow any exceptions
                if( msgBoxException.NotNullReference() )
                {
                    if( baseException.NotNullReference() )
                        throw new AggregateException("The main handlers failed for the exception!", msgBoxException, baseException).StoreFileLine();
                    else
                        throw msgBoxException.StoreFileLine();
                }
                else if( baseException.NotNullReference() )
                {
                    throw baseException.StoreFileLine();
                }
            }
        }

        #endregion

        #region Logging

        private static AdvancedLogEntrySerializer advLogger = null;

        private static AdvancedLogEntrySerializer CreateAdvancedLogEntrySerializer( string directory = null )
        {
            lock( AppEssentials.SyncLock )
            {
                if( advLogger.NullReference() )
                {
                    if( directory.NullOrEmpty() )
                    {
                        directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        directory = Path.GetFullPath(directory);
                    }

                    var fileSystem = new DirectoryFileSystem(directory, escapeFileNames: true); // we escape file names to have file extensions
                    advLogger = new AdvancedLogEntrySerializer(fileSystem, maxLogFileCount: 3);
                }

                return advLogger;
            }
        }

        /// <summary>
        /// Creates a new log file in the specified directory.
        /// If no directory is specified, the log file is placed
        /// into the application folder.
        /// </summary>
        /// <param name="directory">The directory to create the log file at, or <c>null</c> to use the application folder.</param>
        public new void StartXmlLog( string directory = null )
        {
            var newLogger = CreateAdvancedLogEntrySerializer(directory);
            this.StartCustomLog(newLogger);
        }

        #endregion

        #region Window

        private bool windowInitialized = false;
        private bool canWindowClose = false;
        private Window window;

        /// <summary>
        /// Makes sure the specified <see cref="Window"/> is only closed, when the main event queue has finished shutting down.
        /// The main event queue shutdown signal is automatically sent, when the <see cref="Window"/> is about to be closed.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to link to the lifespan of the main event queue.</param>
        protected void LinkWindowToEventQueue( Window window )
        {
            if( window.NullReference() )
                throw new ArgumentNullException("window").StoreFileLine();

            lock( AppEssentials.SyncLock )
            {
                if( !this.windowInitialized )
                {
                    this.window = window;
                    this.window.Closing += this.OnWindowClosing;
                    this.windowInitialized = true;
                }
            }
        }

        private void OnWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            lock( AppEssentials.SyncLock )
            {
                // don't close the window, unless the event queue has already shut down
                if( !this.canWindowClose )
                {
                    e.Cancel = true;
                    EventQueue.BeginShutdown();
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="EventQueueShutDownEvent"/> is being handled.
        /// </summary>
        protected override void OnEventQueueShutDown()
        {
            lock( AppEssentials.SyncLock )
                this.canWindowClose = true;
            UI.Invoke(() => this.window.Close()); // NOTE: calling from inside the 'lock' would result in a dead-lock!

            // release log file, ... etc.
            base.OnEventQueueShutDown();
        }

        #endregion

        /// <summary>
        /// Registers basic exception handlers, and creates a memory logger.
        /// No files or windows are touched.
        /// UI, MagicBag and EventQueue are not available.
        /// </summary>
        public static void InitializeReadOnlyMemory()
        {
            // create instance if necessary
            if( Instance.NullReference() )
                new MechanicalApp();

            MechanicalApp.Instance.SetupReadOnlyMemory();
        }

        /// <summary>
        /// Call this at the start of the application, from the main thread.
        /// </summary>
        /// <param name="logDirectory">The directory to create the log file at, or <c>null</c> to use the application folder.</param>
        public static void InitializeConsole( string logDirectory = null )
        {
            // create instance if necessary
            if( Instance.NullReference() )
                new MechanicalApp();

            // do basic initialization
            var customLogger = CreateAdvancedLogEntrySerializer(logDirectory);
            MechanicalApp.Instance.SetupConsole(customLogger);
        }

        /// <summary>
        /// Call this at the start of the application, from the UI thread.
        /// </summary>
        /// <param name="currentApp">The <see cref="System.Windows.Application"/> object that raises the event.</param>
        /// <param name="window">The <see cref="Window"/> to link to the lifespan of the main event queue.</param>
        /// <param name="logDirectory">The directory to create the log file at, or <c>null</c> to use the application folder.</param>
        public static void InitializeGUI( Application currentApp, Window window, string logDirectory = null )
        {
            // create instance if necessary
            if( Instance.NullReference() )
                new MechanicalApp();

            // register WPF exceptions
            MechanicalApp.Instance.HandleDispatcherExceptions(currentApp);

            // do basic initialization
            var customLogger = CreateAdvancedLogEntrySerializer(logDirectory);
            MechanicalApp.Instance.SetupBasicWindow(customLogger);

            // handle window closing
            MechanicalApp.Instance.LinkWindowToEventQueue(window);
        }
    }
}
