using System;
using System.IO;
using System.Text;
using Mechanical.Conditions;
using Mechanical.FileFormats;
using Mechanical.IO;

namespace Mechanical.DataStores.File
{
    /// <summary>
    /// An XML based data store writer.
    /// </summary>
    public class FileDataStoreWriter : FileDataStoreWriterBase
    {
        #region Private Fields

        private readonly string csvFilePath;
        private readonly string rootParentPath;
        private string currentValuePath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStoreWriter"/> class.
        /// </summary>
        /// <param name="csvFilePath">The path to create the CSV file at.</param>
        /// <param name="rootParentPath">The directory path, where the root node of the data store will be written to.</param>
        public FileDataStoreWriter( string csvFilePath, string rootParentPath )
            : base()
        {
            try
            {
                this.csvFilePath = Path.GetFullPath(csvFilePath);
                this.rootParentPath = Path.GetFullPath(rootParentPath);

                if( !Directory.Exists(this.rootParentPath) )
                    Directory.CreateDirectory(this.rootParentPath);
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("csvFilePath", csvFilePath);
                ex.Store("rootParentPath", rootParentPath);
                throw;
            }
        }

        #endregion

        #region FileDataStoreWriterBase

        /// <summary>
        /// Saves the file entries of the data store.
        /// </summary>
        protected override void SaveEntries()
        {
            using( var sw = new StreamWriter(this.csvFilePath, append: false, encoding: Encoding.UTF8) )
            using( var writer = new CsvWriter(sw, CsvFormat.International) )
                this.Entries.SaveTo(writer);
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
                path = this.Entries.BuildFilePath(entry, Path.DirectorySeparatorChar);
                path = Path.Combine(this.rootParentPath, path);

                switch( entry.Token )
                {
                case DataStoreToken.BinaryValue:
                case DataStoreToken.TextValue:
                    this.currentValuePath = path;
                    break;

                case DataStoreToken.ObjectStart:
                    Directory.CreateDirectory(path);
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
            try
            {
                return IOWrapper.ToBinaryWriter(System.IO.File.OpenWrite(this.currentValuePath));
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("currentValuePath", this.currentValuePath);
                throw;
            }
        }

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override ITextWriter OpenTextWriter()
        {
            try
            {
                return IOWrapper.ToTextWriter(System.IO.File.OpenWrite(this.currentValuePath));
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("currentValuePath", this.currentValuePath);
                throw;
            }
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
