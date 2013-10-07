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
    //// NOTE: - The .zip file has a CSV file and a directory in it's root.
    ////       - The .csv file stores the data store paths, file names, and node types.
    ////       - File paths use forward slashes, as per convention.
    ////       - All files are stored in a directory. We do this, to avoid
    ////         file name conflicts between the .csv file and the root file (or dir.)
    ////       - Zip entry names are explicitly encoded in utf-8

    //// NOTE: for additional notes, see FileDataStoreReader.cs

    /// <summary>
    /// Represents a file based data store. Requires the use of a CSV file.
    /// </summary>
    public class ZipDataStoreReader : FileDataStoreReaderBase
    {
        #region Non-Public Fields

        private const char ZipDirectorySeparator = '/';
        internal const string CsvFileName = "dataStore.csv";
        internal const string DirectoryName = "DataStore";

        private readonly string zipFilePath;
        private ZipArchive zipArchive;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipDataStoreReader"/> class.
        /// </summary>
        /// <param name="zipFilePath">The path of the .zip file to use.</param>
        public ZipDataStoreReader( string zipFilePath )
            : base()
        {
            try
            {
                this.zipFilePath = System.IO.Path.GetFullPath(zipFilePath);
                this.zipArchive = new ZipArchive(
                    System.IO.File.OpenRead(zipFilePath),
                    ZipArchiveMode.Read,
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
                    this.zipArchive.Dispose();
                    this.zipArchive = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            base.OnDispose(disposing);
        }

        #endregion

        #region Zip Paths

        internal static string ToStandardZipPath( FileDataStoreEntryList entries, FileDataStoreEntry entry )
        {
            var path = DirectoryName + ZipDirectorySeparator + entries.BuildFilePath(entry, ZipDirectorySeparator);

            if( entry.Token == DataStoreToken.ObjectStart )
                path += ZipDirectorySeparator;

            return path;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads the file data store entries. Children are placed after their parents, in the correct order.
        /// </summary>
        /// <returns>The file data store entries found.</returns>
        protected override FileDataStoreEntryList LoadEntries()
        {
            var csvEntry = this.zipArchive.GetEntry(CsvFileName);
            if( csvEntry.NullReference() )
                throw new FileNotFoundException("Data store entry CSV file could not be found!").StoreFileLine();

            using( var stream = csvEntry.Open() )
            using( var sr = new System.IO.StreamReader(stream) )
            using( var reader = new CsvReader(sr, CsvFormat.International) )
                return FileDataStoreEntryList.LoadFrom(reader);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="entries">The entries of the file data store.</param>
        /// <param name="entry">The file data store entry to open for reading.</param>
        /// <returns>The <see cref="ITextReader"/> representing the file contents.</returns>
        protected override ITextReader OpenTextFile( FileDataStoreEntryList entries, FileDataStoreEntry entry )
        {
            string path = null;
            try
            {
                path = ToStandardZipPath(entries, entry);
                var zipEntry = this.zipArchive.GetEntry(path);
                if( zipEntry.NullReference() )
                    throw new FileNotFoundException().StoreFileLine();

                var stream = zipEntry.Open();
                return IOWrapper.ToTextReader(stream);
            }
            catch( Exception ex )
            {
                ex.Store("entry", entry);
                ex.Store("path", path);
                throw;
            }
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="entries">The entries of the file data store.</param>
        /// <param name="entry">The file data store entry to open for reading.</param>
        /// <returns>The <see cref="IBinaryReader"/> representing the file contents.</returns>
        protected override IBinaryReader OpenBinaryFile( FileDataStoreEntryList entries, FileDataStoreEntry entry )
        {
            string path = null;
            try
            {
                path = ToStandardZipPath(entries, entry);
                var zipEntry = this.zipArchive.GetEntry(path);
                if( zipEntry.NullReference() )
                    throw new FileNotFoundException().StoreFileLine();

                var stream = zipEntry.Open();
                return IOWrapper.ToBinaryReader(stream);
            }
            catch( Exception ex )
            {
                ex.Store("entry", entry);
                ex.Store("path", path);
                throw;
            }
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected override void CloseReader( IBinaryReader reader )
        {
            ((IDisposable)reader).Dispose();
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected override void CloseReader( ITextReader reader )
        {
            ((IDisposable)reader).Dispose();
        }

        #endregion
    }
}
