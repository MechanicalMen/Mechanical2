using System;
using System.Runtime.CompilerServices;
using Mechanical.MagicBag;

namespace Mechanical.Log
{
    /// <summary>
    /// Logs messages using an <see cref="ILog"/> interface from the default <see cref="IMagicBag"/>.
    /// </summary>
    public static class Log
    {
        private static readonly ILog LogInstance;

        static Log()
        {
            var magicBag = Mechanical.MagicBag.MagicBag.Default;
            LogInstance = magicBag.Pull<ILog>();
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
            LogInstance.Debug(message, ex, filePath, memberName, lineNumber);
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
            LogInstance.Info(message, ex, filePath, memberName, lineNumber);
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
            LogInstance.Warn(message, ex, filePath, memberName, lineNumber);
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
            LogInstance.Error(message, ex, filePath, memberName, lineNumber);
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
            LogInstance.Fatal(message, ex, filePath, memberName, lineNumber);
        }
    }
}
