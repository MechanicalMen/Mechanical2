using System;
using System.Collections.Generic;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.IO;
using Mechanical.IO.FileSystem;
using Mechanical.Logs;

namespace Mechanical.Common.Logs
{
    /// <summary>
    /// Serializes log entries to files. Can handle multiple processes logging at the same time.
    /// Optionally removes old log files, if there are too many of them, or they take up too much space.
    /// </summary>
    public class AdvancedLogEntrySerializer : LogBase
    {
        #region Private Fields

        private static readonly TimeSpan LogFileCheckInterval = TimeSpan.FromMinutes(5);

        private readonly object syncLock = new object();
        private readonly TimeSpan? maxFileAge;
        private readonly int? maxAppInstanceCount;
        private readonly long? maxTotalFileSize;
        private readonly long? singleFileSizeThreshold;
        private ProlificDataFileManager fileManager;
        private ProlificDataFileInfo currentLogFileInfo;
        private LogEntrySerializer logEntrySerializer;
        private long lastSingleFileSizeCheckTicks;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedLogEntrySerializer"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> whose root directory the log files will be placed at.</param>
        /// <param name="maxFileAge">The maximum age of the files returned, or <c>null</c>.</param>
        /// <param name="maxAppInstanceCount">The maximum number of app instances to include, or <c>null</c>.</param>
        /// <param name="maxTotalFileSize">The maximum total file size, or <c>null</c>.</param>
        /// <param name="singleFileSizeThreshold">If a log file exceeds this size, a new file is automatically opened. <c>null</c> to disable checking for this.</param>
        /// <param name="logFilePrefix">The first part of log file names. You can use this to identify the application generating them.</param>
        public AdvancedLogEntrySerializer( IFileSystem fileSystem, TimeSpan? maxFileAge, int? maxAppInstanceCount, long? maxTotalFileSize, long? singleFileSizeThreshold, string logFilePrefix = "log" )
            : base()
        {
            if( maxAppInstanceCount < 1 )
                throw new ArgumentOutOfRangeException().Store("maxAppInstanceCount", maxAppInstanceCount);

            if( singleFileSizeThreshold.HasValue )
            {
                if( singleFileSizeThreshold.Value <= 0 )
                    throw new ArgumentOutOfRangeException().Store("singleFileSizeThreshold", singleFileSizeThreshold.Value);

                if( fileSystem.NullReference() )
                    throw new ArgumentNullException("fileSystem").StoreFileLine();

                if( !fileSystem.SupportsGetFileSize )
                    throw new ArgumentException("Can not check single log file size, if the file system does not support it!").StoreFileLine();
            }

            this.maxFileAge = maxFileAge;
            this.maxAppInstanceCount = maxAppInstanceCount;
            this.maxTotalFileSize = maxTotalFileSize;
            this.singleFileSizeThreshold = singleFileSizeThreshold;
            this.fileManager = new ProlificDataFileManager(fileSystem, fileManagerID: logFilePrefix, fileExtension: fileSystem.EscapesNames ? ".xml" : null);

            this.StartNewLogFile(reason: "new logger created.");
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

        #region Private Methods

        private void RemoveOldLogFiles()
        {
            // NOTE: whether a single log file's size passed the threshold (or not), is irrelevant.
            const bool CurrentFileManagerOnly = false; // if there are other files around, assume they are related log files
            this.fileManager.DeleteOldFiles(CurrentFileManagerOnly, this.maxFileAge, this.maxAppInstanceCount, this.maxTotalFileSize);
        }

        private void CheckSingleFileSizeThreshold()
        {
            // only check if we care about single log file sizes
            if( this.singleFileSizeThreshold.HasValue )
            {
                // do not check for every single log entry
                var now = DateTime.UtcNow;
                long lastTicks = 0;
                Interlocked.Read(ref this.lastSingleFileSizeCheckTicks);
                if( now.Ticks - lastTicks > LogFileCheckInterval.Ticks )
                {
                    Interlocked.Exchange(ref this.lastSingleFileSizeCheckTicks, now.Ticks);

                    bool createNewFile;
                    lock( this.syncLock ) // lock as rarely as possible
                    {
                        var currentSize = this.fileManager.FileSystem.GetFileSize(this.currentLogFileInfo.DataStoreName);
                        createNewFile = currentSize >= this.singleFileSizeThreshold.Value;
                    }

                    if( createNewFile )
                        this.StartNewLogFile(reason: "maximum size of single log file exceeded."); // NOTE: this locks, so watch out for dead-locks
                }
            }
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

            // log file too long?
            this.CheckSingleFileSizeThreshold();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the <see cref="IFileSystem"/> in use by this instance.
        /// </summary>
        /// <value>The <see cref="IFileSystem"/> in use.</value>
        public IFileSystem FileSystem
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.fileManager.FileSystem;
            }
        }

        /// <summary>
        /// Closes the current log file and starts a new one.
        /// Only closed log files are guaranteed to be accessible.
        /// </summary>
        /// <param name="reason">The reason for starting a new log file.</param>
        public void StartNewLogFile( string reason )
        {
            lock( this.syncLock )
            {
                // create new file and log serializer
                ProlificDataFileInfo fileInfo;
                var binaryStream = this.fileManager.CreateFile(out fileInfo, useWriteThroughIfSupported: true);
                var newLogSerializer = LogEntrySerializer.ToXmlStream(IOWrapper.Wrap(binaryStream));
                newLogSerializer.Debug("New log file started. Reason: " + (reason.NullOrEmpty() ? "not specified." : reason));

                // switch with current log serializer, dispose of the old one
                var oldLogSerializer = this.logEntrySerializer;
                this.logEntrySerializer = newLogSerializer;
                if( oldLogSerializer.NotNullReference() )
                    oldLogSerializer.Dispose();

                // delete old files
                this.RemoveOldLogFiles();

                this.currentLogFileInfo = fileInfo;
                this.lastSingleFileSizeCheckTicks = DateTime.UtcNow.Ticks;
            }
        }

        /// <summary>
        /// All log files but the one currently in use,
        /// are opened one-by-one. Each new stream closes the previous one.
        /// </summary>
        /// <returns>Key-value pairs representing file name information, and file streams.</returns>
        public IEnumerable<KeyValuePair<ProlificDataFileInfo, IBinaryReader>> GetClosedLogFiles()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(null).StoreFileLine();

            lock( this.syncLock )
            {
                // remove old files
                this.RemoveOldLogFiles();

                // get remaining log files
                // NOTE: whether a single log file's size passed the threshold (or not), is irrelevant.
                const bool CurrentFileManagerOnly = false; // if there are other files around, assume they are related log files
                var fileInfos = this.fileManager.FindAllFiles(CurrentFileManagerOnly);

                foreach( var f in fileInfos )
                {
                    // skip the current log file
                    if( DataStore.Comparer.Equals(f.DataStoreName, this.currentLogFileInfo.DataStoreName) )
                        continue;

                    // try to open log file
                    IBinaryReader reader = null;
                    try
                    {
                        reader = this.fileManager.FileSystem.ReadBinary(f.DataStoreName);
                    }
                    catch( Exception ex )
                    {
                        ex.Store("logFileDataStorePath", f.DataStoreName);
                        Mechanical.Core.Log.Warn("Failed to open log file! It may be in use by a process.", ex);
                    }

                    // return with file contents, if it could be opened
                    if( reader.NotNullReference() )
                    {
                        yield return new KeyValuePair<ProlificDataFileInfo, IBinaryReader>(f, reader);
                        reader.Close();
                    }
                }
            }
        }

        #endregion
    }
}
