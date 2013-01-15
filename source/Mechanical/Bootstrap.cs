﻿using System;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.MagicBag;

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
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            // NOTE: no locking needed here
            if( !isInitialized )
                throw new InvalidOperationException("Library not yet initialized! Look at the Mechanical.Bootstrap class.").StoreDefault(filePath, lineNumber);
        }

        internal static void ThrowIfAlreadyInitialized(
            [CallerFilePath] string filePath = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            // NOTE: no locking needed here
            if( isInitialized )
                throw new InvalidOperationException("Library already initialized!").StoreDefault(filePath, lineNumber);
        }

        #endregion

        #region Public Static Members

        /// <summary>
        /// Initializes the Mechanical2 library.
        /// </summary>
        /// <param name="magicBag">The magic bag to use internally in the library.</param>
        public static void Initialize( IMagicBag magicBag = null )
        {
            lock( syncLock )
            {
                ThrowIfAlreadyInitialized();

                // not yet initialized
                Mechanical.MagicBag.MagicBag.CreateDefault(magicBag);
                isInitialized = true;
            }
        }

        #endregion
    }
}