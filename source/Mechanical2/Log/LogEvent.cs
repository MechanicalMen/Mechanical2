using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.Events;

namespace Mechanical.Log
{
    /// <summary>
    /// Wraps a <see cref="LogEntry"/>, to be handled.
    /// </summary>
    public class LogEvent : IEvent
    {
        private readonly LogEntry entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="entry">The log entry, to be handled.</param>
        public LogEvent( LogEntry entry )
        {
            if( entry.NullReference() )
                throw new ArgumentNullException("entry").StoreDefault();

            this.entry = entry;
        }

        /// <summary>
        /// Gets the log entry, to be handled.
        /// </summary>
        /// <value>The log entry, to be handled.</value>
        public LogEntry Entry
        {
            get { return this.entry; }
        }
    }
}
