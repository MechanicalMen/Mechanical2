using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Xml;
using Mechanical.Events;

namespace Mechanical.Log
{
    /// <summary>
    /// Serializes log events.
    /// </summary>
    public class LogEventSerializer : DisposableObject, IEventHandler<LogEvent>
    {
        //// NOTE: We use FileOptions.WriteThrough and frequent flushing,
        ////       to make sure all logs end up on the disk, in case of a crash.
        ////       Should that happen, the resulting Xml file may be short a few closing elements.

        #region Private Fields

        private IEventQueue eventQueue;
        private XmlDataStoreWriter writer;
        private int logEntryIndex = 0;

        #endregion

        #region Constructors

        private LogEventSerializer( XmlDataStoreWriter writer, IEventQueue eventQueue )
        {
            Ensure.That(writer).NotNull().NotDisposed();
            Ensure.That(eventQueue).NotNull();

            this.writer = writer;
            this.eventQueue = eventQueue;
            this.eventQueue.Subscribe(this);
        }

        /// <summary>
        /// Returns a new <see cref="LogEventSerializer"/> instance.
        /// </summary>
        /// <param name="xmlFilePath">The path to the Xml log file.</param>
        /// <param name="eventQueue">The <see cref="IEventQueue"/> to subscribe to.</param>
        /// <returns>The new <see cref="LogEventSerializer"/> instance created.</returns>
        public static LogEventSerializer FromXml( string xmlFilePath, IEventQueue eventQueue )
        {
            try
            {
                var fs = new FileStream(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize: 4096, options: FileOptions.WriteThrough); // 4K is the default FileStream buffer size. May not be zero.
                var writer = new XmlDataStoreWriter(fs, writeRootObject: "LogEntries");
                return new LogEventSerializer(writer, eventQueue);
            }
            catch( Exception ex )
            {
                ex.Store("xmlFilePath", xmlFilePath);
                throw;
            }
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

                if( this.eventQueue.NotNullReference() )
                {
                    this.eventQueue.Unsubscribe(this);
                    this.eventQueue = null;
                }

                if( this.writer.NotNullReference() )
                {
                    this.writer.Dispose();
                    this.writer = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)


            base.OnDispose(disposing);
        }

        #endregion

        #region IEventHandler

        /// <summary>
        /// Handles the specified <see cref="IEvent"/>.
        /// </summary>
        /// <param name="evnt">The event to handle.</param>
        /// <param name="queue">The queue to use, to piggyback events.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation; or <c>null</c> for synchronous operations.</returns>
        public Task Handle( LogEvent evnt, IEventHandlerQueue queue )
        {
            this.writer.Write("e" + this.logEntryIndex.ToString(CultureInfo.InvariantCulture), evnt.Entry, LogEntry.Serializer.Default);
            this.writer.Flush();
            ++this.logEntryIndex;
            return null;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Deserializes the specified xml log file.
        /// </summary>
        /// <param name="xmlPath">The path to the xml file.</param>
        /// <returns>The deserialized log entries.</returns>
        public static List<LogEntry> Deserialize( string xmlPath )
        {
            var list = new List<LogEntry>();
            using( var sr = new StreamReader(xmlPath) )
            using( var reader = new XmlDataStoreReader(sr) )
            {
                reader.Read(
                    "LogEntries",
                    r =>
                    {
                        while( r.Token != DataStoreToken.ObjectEnd )
                            list.Add(r.Read(LogEntry.Serializer.Default));
                    });
            }
            return list;
        }

        #endregion
    }
}
