using System;
using System.Runtime.CompilerServices;

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
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public static void Debug(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            AppCore.Log.Debug(message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public static void Info(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            AppCore.Log.Info(message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public static void Warn(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            AppCore.Log.Warn(message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public static void Error(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            AppCore.Log.Error(message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public static void Fatal(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            AppCore.Log.Fatal(message, ex, file, member, line);
        }
    }
}
