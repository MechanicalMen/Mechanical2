using System;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.Events;

namespace Mechanical.Log
{
    /// <summary>
    /// Enqueues logged messages as events.
    /// </summary>
    public class EventLogger : ILog
    {
        #region Private Fields

        private readonly IEventQueue eventQueue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogger"/> class.
        /// </summary>
        /// <param name="eventQueue">The <see cref="IEventQueue"/> to use.</param>
        public EventLogger( IEventQueue eventQueue )
        {
            Ensure.That(eventQueue).NotNull();

            this.eventQueue = eventQueue;
        }

        #endregion

        #region Private Methods

        private void Enqueue(
            LogLevel level,
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            var info = ex.NullReference() ? null : ExceptionInfo.From(ex);
            var entry = new LogEntry(level, message, info, filePath, memberName, lineNumber);
            var evnt = new LogEvent(entry);
            this.eventQueue.Enqueue(evnt, TaskResult.Null);
        }

        #endregion

        #region ILog

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public void Debug(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            this.Enqueue(LogLevel.Debug, message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public void Info(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            this.Enqueue(LogLevel.Information, message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public void Warn(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            this.Enqueue(LogLevel.Warning, message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public void Error(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            this.Enqueue(LogLevel.Error, message, ex, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log; or <c>null</c>.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        public void Fatal(
            string message,
            Exception ex = null,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            this.Enqueue(LogLevel.Fatal, message, ex, filePath, memberName, lineNumber);
        }

        #endregion
    }
}
