using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.FileFormats;
using Mechanical.IO;

namespace Mechanical.DataStores.File
{
    /// <summary>
    /// Represents a file based data store. Requires the use of a CSV file.
    /// </summary>
    public class ZipDataStoreWriter : FileDataStoreWriterBase
    {
        #region Private Fields

        private readonly string zipFilePath;
        private readonly CompressionLevel compressionLevel;
        private ZipArchive zipArchive;
        private string currentValuePath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipDataStoreWriter"/> class.
        /// </summary>
        /// <param name="zipFilePath">The path of the .zip file to use.</param>
        /// <param name="compressionLevel">Gets the <see cref="CompressionLevel"/> to use.</param>
        public ZipDataStoreWriter( string zipFilePath, CompressionLevel compressionLevel = CompressionLevel.Optimal )
            : base()
        {
            try
            {
                Ensure.That(compressionLevel).IsDefined();

                this.zipFilePath = Path.GetFullPath(zipFilePath);
                this.compressionLevel = compressionLevel;

                this.zipArchive = new ZipArchive(
                    new FileStream(this.zipFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None),
                    ZipArchiveMode.Update,
                    leaveOpen: false,
                    entryNameEncoding: Encoding.UTF8);
            }
            catch( Exception ex )
            {
                ex.Store("zipFilePath", zipFilePath);
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

                if( this.zipArchive.NotNullReference() )
                {
                    this.SaveEntries();
                    this.zipArchive.Dispose();
                    this.zipArchive = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

        private ZipArchiveEntry GetZipEntry( string zipPath )
        {
            var zipEntry = this.zipArchive.GetEntry(zipPath);
            if( zipEntry.NullReference() )
                zipEntry = this.zipArchive.CreateEntry(zipPath, this.compressionLevel);
            return zipEntry;
        }

        private Stream GetZipStream( string zipPath )
        {
            var zipEntry = this.GetZipEntry(zipPath);
            var stream = zipEntry.Open();
            stream.SetLength(0);
            return stream;
        }

        #endregion

        #region FileDataStoreWriterBase

        /// <summary>
        /// Saves the file entries of the data store.
        /// </summary>
        protected override void SaveEntries()
        {
            // this method gets called by the base class as it is disposed
            if( this.zipArchive.NotNullReference() )
            {
                using( var stream = this.GetZipStream(ZipDataStoreReader.CsvFileName) )
                using( var sw = new System.IO.StreamWriter(stream, Encoding.UTF8) )
                using( var writer = new CsvWriter(sw, CsvFormat.International) )
                    this.Entries.SaveTo(writer);
            }
        }

        /// <summary>
        /// Writes the specified file data store entry.
        /// </summary>
        /// <param name="entry">The file data store entry to open for reading.</param>
        protected override void WriteEntry( FileDataStoreEntry entry )
        {
            string path = null;
            try
            {
                path = ZipDataStoreReader.ToStandardZipPath(this.Entries, entry);
                switch( entry.Token )
                {
                case DataStoreToken.BinaryValue:
                case DataStoreToken.TextValue:
                    this.currentValuePath = path;
                    break;

                case DataStoreToken.ObjectStart:
                    this.GetZipEntry(path);
                    break;

                default:
                    throw new ArgumentException("Invalid token!").StoreFileLine();
                }
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("entry", entry);
                ex.Store("path", path);
                throw;
            }
        }

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override IBinaryWriter OpenBinaryWriter()
        {
            var stream = this.GetZipStream(this.currentValuePath);
            return IOWrapper.ToBinaryWriter(stream);
        }

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override ITextWriter OpenTextWriter()
        {
            var stream = this.GetZipStream(this.currentValuePath);
            return IOWrapper.ToTextWriter(stream);
        }

        /// <summary>
        /// Releases any resources held by an open writer.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected override void CloseWriter( IBinaryWriter writer )
        {
            ((IDisposable)writer).Dispose();
            this.currentValuePath = null;
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected override void CloseWriter( ITextWriter writer )
        {
            ((IDisposable)writer).Dispose();
            this.currentValuePath = null;
        }

        #endregion
    }
}
