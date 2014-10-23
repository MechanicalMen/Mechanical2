using System;
using System.Collections.Generic;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Log
{
    /// <summary>
    /// Saves log entries in memory.
    /// </summary>
    public class MemoryLog : LogBase
    {
        #region Private Fields

        private readonly object syncLock = new object();
        private readonly List<LogEntry> entries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLog"/> class.
        /// </summary>
        public MemoryLog()
            : base()
        {
            this.entries = new List<LogEntry>();
        }

        #endregion

        #region IDisposableObject

        /// <summary>
        /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
        protected override void OnDispose( bool disposing )
        {
            if( disposing )
            {
                //// dispose-only (i.e. non-finalizable) logic
                //// (managed, disposable resources you own)
            }

            //// shared cleanup logic
            //// (unmanaged resources)
            this.entries.Clear();

            base.OnDispose(disposing);
        }

        #endregion

        #region LogBase

        /// <summary>
        /// Logs the specified <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="entry">The <see cref="LogEntry"/> to log.</param>
        public override void Log( LogEntry entry )
        {
            lock( this.syncLock )
                this.entries.Add(entry);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transmits all saved log entries to the specified logger,
        /// and then clears it's buffer.
        /// </summary>
        /// <param name="log">The logger to transmit log entries to.</param>
        public void FlushLogs( LogBase log )
        {
            if( log.NullReference() )
                throw new ArgumentNullException("log").StoreFileLine();

            lock( this.syncLock )
            {
                foreach( var e in this.entries )
                    log.Log(e);

                this.entries.Clear();
            }
        }

        #endregion
    }
}
