using System;
using System.Runtime.CompilerServices;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.Logs
{
    /// <summary>
    /// A base class implementing <see cref="ILog"/>.
    /// </summary>
    public abstract class LogBase : DisposableObject, ILog
    {
        #region Private Methods

        private void Log(
            LogLevel level,
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            var info = ex.NullReference() ? null : ExceptionInfo.From(ex);
            var entry = new LogEntry(level, message, info, file, member, line);
            this.Log(entry);
        }

        #endregion

        #region ILog

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public void Debug(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            this.Log(LogLevel.Debug, message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public void Info(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            this.Log(LogLevel.Information, message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public void Warn(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            this.Log(LogLevel.Warning, message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public void Error(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            this.Log(LogLevel.Error, message, ex, file, member, line);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public void Fatal(
            string message,
            Exception ex = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            this.Log(LogLevel.Fatal, message, ex, file, member, line);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Logs the specified <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="entry">The <see cref="LogEntry"/> to log.</param>
        public abstract void Log( LogEntry entry );

        #endregion
    }
}
