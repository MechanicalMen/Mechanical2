using System;
using Mechanical.Conditions;
using Mechanical.FileFormats;
using Mechanical.IO;

namespace Mechanical.DataStores.File
{
    //// NOTE: this data store does not search for files, instead it looks at
    ////       the declarations in the file entry CSV file.

    //// NOTE: since Windows is case-insensitive, a 'dataStore.csv' file with bad
    ////       file name cases, will probably break the data store on case sensitive systems.

    /// <summary>
    /// Represents a file based data store. Requires the use of a CSV file.
    /// </summary>
    public class FileDataStoreReader : FileDataStoreReaderBase
    {
        #region Private Fields

        private readonly string csvFilePath;
        private readonly string baseDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStoreReader"/> class.
        /// </summary>
        /// <param name="csvFilePath">The path of the CSV file to use.</param>
        public FileDataStoreReader( string csvFilePath )
            : base()
        {
            try
            {
                this.csvFilePath = System.IO.Path.GetFullPath(csvFilePath);
                this.baseDirectory = System.IO.Path.GetDirectoryName(this.csvFilePath);
            }
            catch( Exception ex )
            {
                ex.Store("csvFilePath", csvFilePath);
                throw;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads the file data store entries. Children are placed after their parents, in the correct order.
        /// </summary>
        /// <returns>The file data store entries found.</returns>
        protected override FileDataStoreEntryList LoadEntries()
        {
            using( var sr = new System.IO.StreamReader(this.csvFilePath) )
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
            string absolutePath = null;
            try
            {
                var relativePath = entries.BuildFilePath(entry, System.IO.Path.DirectorySeparatorChar);
                absolutePath = System.IO.Path.Combine(this.baseDirectory, relativePath);
                var sr = new System.IO.StreamReader(absolutePath);
                return IOWrapper.Wrap(sr);
            }
            catch( Exception ex )
            {
                ex.Store("entry", entry);
                ex.Store("absolutePath", absolutePath);
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
            string absolutePath = null;
            try
            {
                var relativePath = entries.BuildFilePath(entry, System.IO.Path.DirectorySeparatorChar);
                absolutePath = System.IO.Path.Combine(this.baseDirectory, relativePath);
                var stream = System.IO.File.OpenRead(absolutePath);
                return IOWrapper.ToBinaryReader(stream);
            }
            catch( Exception ex )
            {
                ex.Store("entry", entry);
                ex.Store("absolutePath", absolutePath);
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
