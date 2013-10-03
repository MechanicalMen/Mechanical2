using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.FileFormats;
using Mechanical.IO;

namespace Mechanical.DataStores.File
{
    //// NOTE: - escape invalid (non-english, ... etc.) characters
    ////       - zip is case sensitive, but the host system may not be
    ////       - System.IO.Compression.ZipArchiveEntry.Name.get:  Path.GetFileName(this.FullName)

    //// TODO: method to explicitly check CSV file for correct raw path character casing

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
        {
            try
            {
                this.csvFilePath = csvFilePath;
                this.baseDirectory = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(csvFilePath));

                this.Initialize();
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
        /// Gets all file data store entries. Children are placed after their parents, in the correct order.
        /// </summary>
        /// <returns>The file data store entries found.</returns>
        protected override FileDataStoreEntry[] GetAllEntries()
        {
            using( var sr = new System.IO.StreamReader(this.csvFilePath) )
            using( var reader = new CsvReader(sr, CsvFormat.International) )
            {
                return this.LoadEntries(reader);
            }
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="rawPath">The file path.</param>
        /// <returns>The <see cref="ITextReader"/> representing the file contents.</returns>
        protected override ITextReader OpenTextFile( string rawPath )
        {
            string path = null;
            try
            {
                path = System.IO.Path.Combine(this.baseDirectory, rawPath);
                var sr = new System.IO.StreamReader(path);
                return IOWrapper.Wrap(sr);
            }
            catch( Exception ex )
            {
                ex.Store("rawPath", rawPath);
                ex.Store("path", path);
                throw;
            }
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="rawPath">The file path.</param>
        /// <returns>The <see cref="IBinaryReader"/> representing the file contents.</returns>
        protected override IBinaryReader OpenBinaryFile( string rawPath )
        {
            string path = null;
            try
            {
                path = System.IO.Path.Combine(this.baseDirectory, rawPath);
                var stream = System.IO.File.OpenRead(path);
                return IOWrapper.ToBinaryReader(stream);
            }
            catch( Exception ex )
            {
                ex.Store("rawPath", rawPath);
                ex.Store("path", path);
                throw;
            }
        }

        /// <summary>
        /// Invoked after the value has been read. Used to release resources.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> used; or <c>null</c>.</param>
        /// <param name="binaryReader">The <see cref="IBinaryReader"/> used; or <c>null</c>.</param>
        protected override void OnValueRead( ITextReader textReader, IBinaryReader binaryReader )
        {
            base.OnValueRead(textReader, binaryReader);

            if( textReader.NotNullReference() )
                ((IDisposable)textReader).Dispose();

            if( binaryReader.NotNullReference() )
                ((IDisposable)binaryReader).Dispose();
        }

        #endregion
    }
}
