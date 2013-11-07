using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Xml;

namespace Mechanical.Log
{
    /// <summary>
    /// Serializes log events.
    /// </summary>
    public class LogEntrySerializer : LogBase
    {
        //// NOTE: We use FileOptions.WriteThrough and frequent flushing,
        ////       to make sure all logs end up on the disk, in case of a crash.
        ////       Should that happen, the resulting Xml file may be short a few closing elements.

        #region Private Fields

        private XmlDataStoreWriter writer;
        private int logEntryIndex = 0;

        #endregion

        #region Constructors

        private LogEntrySerializer( XmlDataStoreWriter writer )
        {
            Ensure.That(writer).NotNull().NotDisposed();

            this.writer = writer;
        }

        /// <summary>
        /// Returns a new <see cref="LogEntrySerializer"/> instance.
        /// This is the recommended way of creating an instance.
        /// </summary>
        /// <param name="xmlFilePath">The file to write the Xml contents to.</param>
        /// <returns>The new <see cref="LogEntrySerializer"/> instance created.</returns>
        public static LogEntrySerializer ToXmlFile( string xmlFilePath )
        {
            try
            {
#if !SILVERLIGHT
                var fs = new FileStream(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize: 4096, options: FileOptions.WriteThrough); // 4K is the default FileStream buffer size. May not be zero.
#else
                var fs = new FileStream(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
#endif
                return ToXmlStream(fs);
            }
            catch( Exception ex )
            {
                ex.Store("xmlFilePath", xmlFilePath);
                throw;
            }
        }

        /// <summary>
        /// Returns a new <see cref="LogEntrySerializer"/> instance.
        /// If possible, use the other method to create an instance.
        /// </summary>
        /// <param name="xmlStream">The <see cref="Stream"/> to write the Xml contents to.</param>
        /// <returns>The new <see cref="LogEntrySerializer"/> instance created.</returns>
        public static LogEntrySerializer ToXmlStream( Stream xmlStream )
        {
            var writer = new XmlDataStoreWriter(xmlStream);
            writer.WriteObjectStart("LogEntries");
            return new LogEntrySerializer(writer);
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

                if( this.writer.NotNullReference() )
                {
                    this.writer.WriteObjectEnd();
                    this.writer.Dispose();
                    this.writer = null;
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
        protected override void Log( LogEntry entry )
        {
            // NOTE: when the disk is full, trying to write and then failing
            //       may result in an infinite cycle
            try
            {
                this.writer.Write("e" + this.logEntryIndex.ToString(CultureInfo.InvariantCulture), entry, LogEntry.Serializer.Default);
                this.writer.Flush();
                ++this.logEntryIndex;
            }
            catch
            {
            }
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Deserializes the specified xml log file.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <returns>The deserialized log entries.</returns>
        public static List<LogEntry> Deserialize( IDataStoreReader reader )
        {
            var list = new List<LogEntry>();
            reader.Read(
                "LogEntries",
                () =>
                {
                    while( reader.Read()
                        && reader.Token == DataStoreToken.ObjectStart )
                        list.Add(reader.Deserialize(LogEntry.Serializer.Default));
                });
            return list;
        }

        #endregion
    }
}
