using System;
using System.Runtime.CompilerServices;

namespace Mechanical.Logs
{
    /// <summary>
    /// A basic logger. Easily implementable using your custom logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        void Debug(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 );

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        void Info(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 );

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        void Warn(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 );

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        void Error(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 );

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        void Fatal(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 );
    }
}
