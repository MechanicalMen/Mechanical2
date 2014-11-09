using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;
using Mechanical.IO.FileSystem;
using Mechanical.Logs;

namespace Mechanical.Common.Logs
{
    /// <summary>
    /// Serializes log entries to files. Can handle multiple processes logging at the same time.
    /// </summary>
    public class AdvancedLogEntrySerializer : LogBase
    {
        #region Private Fields

        private ProlificDataFileManager fileManager;
        private LogEntrySerializer logEntrySerializer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedLogEntrySerializer"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> whose root directory the log files will be placed at.</param>
        /// <param name="maxLogFileCount">The maximum number of log files to keep.</param>
        public AdvancedLogEntrySerializer( IFileSystem fileSystem, int maxLogFileCount )
            : base()
        {
            if( maxLogFileCount < 1 )
                throw new ArgumentOutOfRangeException().Store("maxLogFileCount", maxLogFileCount);

            // initialize file manager
            this.fileManager = new ProlificDataFileManager(fileSystem, fileManagerID: "log", fileExtension: fileSystem.EscapesNames ? ".xml" : null);

            // create new file
            ProlificDataFileInfo fileInfo;
            var binaryStream = this.fileManager.CreateFile(out fileInfo, useWriteThroughIfSupported: true);
            this.logEntrySerializer = LogEntrySerializer.ToXmlStream(IOWrapper.Wrap(binaryStream));

            // delete old files
            // (we have one log file for each app instance)
            ProlificDataFileManager.DeleteOldFiles(this.fileManager.FileSystem, maxFileAge: null, maxAppInstanceCount: maxLogFileCount, maxTotalFileSize: null);
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
                if( this.logEntrySerializer.NotNullReference() )
                {
                    this.logEntrySerializer.Dispose();
                    this.logEntrySerializer = null;
                }

                if( this.fileManager.NotNullReference() )
                {
                    this.fileManager.Dispose();
                    this.fileManager = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)


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
            if( this.IsDisposed )
                throw new ObjectDisposedException(null).StoreFileLine();

            // logEntrySerializer handles thread-safety
            this.logEntrySerializer.Log(entry);
        }

        #endregion
    }
}
