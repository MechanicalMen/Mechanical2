using System;
using System.Runtime.CompilerServices;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.Core
{
    /// <summary>
    /// Syntactic sugar for logging through the current application logger (see: <see cref="AppCore.Log"/>).
    /// </summary>
    public static class Log
    {
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
            AppCore.Log.Debug(message, ex, filePath, memberName, lineNumber);
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
            AppCore.Log.Info(message, ex, filePath, memberName, lineNumber);
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
            AppCore.Log.Warn(message, ex, filePath, memberName, lineNumber);
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
            AppCore.Log.Error(message, ex, filePath, memberName, lineNumber);
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
            AppCore.Log.Fatal(message, ex, filePath, memberName, lineNumber);
        }
    }
}
