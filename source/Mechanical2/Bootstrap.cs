using System;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.MagicBag;
using Mechanical.MVVM;

namespace Mechanical
{
    /// <summary>
    /// Initializes the Mechanical2 library.
    /// </summary>
    public static class Bootstrap
    {
        #region Private Static Fields

        private static object syncLock = new object();
        private static bool isInitialized = false;

        #endregion

        #region Internal Static Members

        internal static void ThrowIfUninitialized(
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            // NOTE: no locking needed here
            if( !isInitialized )
                throw new InvalidOperationException("Library not yet initialized! Look at the Mechanical.Bootstrap class.").StoreDefault(filePath, memberName, lineNumber);
        }

        internal static void ThrowIfAlreadyInitialized(
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            // NOTE: no locking needed here
            if( isInitialized )
                throw new InvalidOperationException("Library already initialized!").StoreDefault(filePath, memberName, lineNumber);
        }

        #endregion

        #region Initialize

        private static void InitializeCore( IMagicBag magicBag )
        {
            Mechanical.MagicBag.MagicBag.CreateDefault(magicBag);
            isInitialized = true;
        }

        /// <summary>
        /// Initializes the Mechanical2 library, for an MVVM application.
        /// The current dispatcher and scheduler will be used for UI operations.
        /// </summary>
        /// <param name="magicBag">The magic bag to use internally in the library.</param>
        public static void Initialize( IMagicBag magicBag = null )
        {
            lock( syncLock )
            {
                ThrowIfAlreadyInitialized();

                // not yet initialized
                UI.SetDispatcherFromCurrent();
                UI.SetSchedulerFromCurrent();
                InitializeCore(magicBag);
            }
        }

        /// <summary>
        /// Initializes the Mechanical2 library, for a console application.
        /// Sets the UI dispatcher to <c>null</c>, and the scheduler to <see cref="System.Threading.Tasks.TaskScheduler.Default"/>.
        /// </summary>
        /// <param name="magicBag">The magic bag to use internally in the library.</param>
        public static void InitializeConsole( IMagicBag magicBag = null )
        {
            lock( syncLock )
            {
                ThrowIfAlreadyInitialized();

                // not yet initialized
                UI.SetDispatcherForConsole();
                UI.SetSchedulerForConsole();
                InitializeCore(magicBag);
            }
        }

        #endregion
    }
}
