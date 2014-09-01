using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.Log
{
    /// <summary>
    /// Logs messages using an <see cref="ILog"/> interface from the default <see cref="IMagicBag"/>.
    /// </summary>
    public static class Log
    {
        private static ILog logInstance;

        /// <summary>
        /// Sets the <see cref="ILog"/> interface to use.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> interface to use.</param>
        public static void Set( ILog log )
        {
            Interlocked.Exchange(ref logInstance, log);
        }

        /// <summary>
        /// Gets the logger currently in use, if there is one.
        /// Otherwise an exception is thrown.
        /// </summary>
        /// <value>The logger currently in use.</value>
        public static ILog Instance
        {
            get
            {
                var instance = logInstance;
                if( instance.NullReference() )
                    throw new InvalidOperationException("No logger set!").StoreFileLine();
                else
                    return instance;
            }
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void Debug(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            Instance.Debug(message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void Info(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            Instance.Info(message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void Warn(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            Instance.Warn(message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void Error(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            Instance.Error(message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public static void Fatal(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            Instance.Fatal(message, ex, filePath, memberName, lineNumber);
        }
    }
}
