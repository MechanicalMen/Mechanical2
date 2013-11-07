using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents a zip file as an abstract file system.
    /// </summary>
    public class ZipFileSystem : ZipFileSystemBase
    {
        #region Private Fields

        private static readonly Encoding ZipEntryNameEncoding = Encoding.UTF8;
        private static readonly CompressionLevel CompressionLevel = CompressionLevel.Optimal;

        private Stream zipStream;
        private ZipArchive zipArchive;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystem"/> class
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that contains the archive.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystem( Stream stream, bool escapeFileNames )
            : base(escapeFileNames)
        {
            try
            {
                this.zipStream = stream;
                this.Flush();
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("escapeFileNames", escapeFileNames);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystem"/> class
        /// </summary>
        /// <param name="stream">The <see cref="IBinaryStream"/> that contains the archive.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystem( IBinaryStream stream, bool escapeFileNames )
            : this(IOWrapper.Wrap(stream), escapeFileNames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystem"/> class
        /// </summary>
        /// <param name="zipFilePath">The path of the .zip file to open.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystem( string zipFilePath, bool escapeFileNames )
            : this(new FileStream(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite), escapeFileNames)
        {
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
                    this.zipArchive.Dispose();
                    this.zipArchive = null;
                }

                if( this.zipStream.NotNullReference() )
                {
                    this.zipStream.Dispose();
                    this.zipStream = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            base.OnDispose(disposing);
        }

        #endregion

        #region ZipFileSystemBase

        /// <summary>
        /// Returns the standard zip paths of the entries.
        /// </summary>
        /// <returns>The file paths of the zip entries found.</returns>
        protected override string[] GetZipEntryFilePaths()
        {
            return this.zipArchive.Entries.Select(e => e.FullName).ToArray();
        }

        /// <summary>
        /// Determines whether a zip entry with the specified zip path exists.
        /// </summary>
        /// <param name="zipPath">The zip path to search for.</param>
        /// <returns><c>true</c> if the zip path is in use; otherwise, <c>false</c>.</returns>
        protected override bool ContainsEntry( string zipPath )
        {
            return this.zipArchive.GetEntry(zipPath).NotNullReference();
        }

        /// <summary>
        /// Opens an entry for reading.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        /// <returns>The stream to read the uncompressed contents of the zip entry from.</returns>
        protected override Stream OpenRead( string zipPath )
        {
            var zipEntry = this.zipArchive.GetEntry(zipPath);
            if( zipEntry.NotNullReference() )
                return zipEntry.Open();
            else
                return null;
        }

        /// <summary>
        /// Gets the uncompressed size of the zip entry.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        /// <returns>The uncompressed size of the zip entry; or <c>null</c>.</returns>
        protected override long? GetUncompressedSize( string zipPath )
        {
            var zipEntry = this.zipArchive.GetEntry(zipPath);
            if( zipEntry.NotNullReference() )
                return zipEntry.Length;
            else
                return null;
        }

        /// <summary>
        /// Creates a new zip entry representing a directory.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        protected override void CreateDirectoryEntry( string zipPath )
        {
            this.zipArchive.CreateEntry(zipPath, CompressionLevel);
        }

        /// <summary>
        /// Removes the specified zip entry.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        protected override void RemoveEntry( string zipPath )
        {
            var zipEntry = this.zipArchive.GetEntry(zipPath);
            if( zipEntry.NotNullReference() )
                zipEntry.Delete();
        }

        /// <summary>
        /// Creates a new zip entry representing a file.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        /// <returns>The <see cref="Stream"/> to which to write the contents of the entry to.</returns>
        protected override Stream CreateFileEntry( string zipPath )
        {
            var zipEntry = this.zipArchive.CreateEntry(zipPath, CompressionLevel);
            return zipEntry.Open();
        }

        /// <summary>
        /// Saves the current state of the zip file.
        /// </summary>
        protected override void Flush()
        {
            // ZipArchive has to be disposed to make it write its content to its underlying stream.
            if( this.zipArchive.NotNullReference() )
                this.zipArchive.Dispose();

            // (re)load the existing archive (if there is one)
            this.zipArchive = new ZipArchive(
                    this.zipStream,
                    ZipArchiveMode.Update,
                    leaveOpen: true, // makes the stream reusable
                    entryNameEncoding: ZipEntryNameEncoding);
        }

        #endregion
    }
}
