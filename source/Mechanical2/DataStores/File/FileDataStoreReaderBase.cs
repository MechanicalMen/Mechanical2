using System;
using System.Collections.Generic;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.File
{
    /// <summary>
    /// A base class for file (or archive) based data stores.
    /// </summary>
    public abstract class FileDataStoreReaderBase : DataStoreReaderBase.Disposable
    {
        #region Private Fields

        private FileDataStoreEntry[] entries = null;
        private int currentEntryAt = -1;
        private int nextEntryAt = -1;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStoreReaderBase"/> class.
        /// </summary>
        protected FileDataStoreReaderBase()
            : base()
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
            }

            //// shared cleanup logic
            //// (unmanaged resources)
            this.entries = null;
            this.currentEntryAt = -1;
            this.nextEntryAt = -1;

            base.OnDispose(disposing);
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Gets all file data store entries. Children are placed after their parents, in the correct order.
        /// </summary>
        /// <returns>The file data store entries found.</returns>
        protected abstract FileDataStoreEntry[] GetAllEntries();

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="rawPath">The file path.</param>
        /// <returns>The <see cref="ITextReader"/> representing the file contents.</returns>
        protected abstract ITextReader OpenTextFile( string rawPath );

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="rawPath">The file path.</param>
        /// <returns>The <see cref="IBinaryReader"/> representing the file contents.</returns>
        protected abstract IBinaryReader OpenBinaryFile( string rawPath );

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes the reader.
        /// </summary>
        protected new void Initialize()
        {
            try
            {
                this.entries = this.GetAllEntries();
                this.currentEntryAt = -1;
                this.nextEntryAt = this.entries.NullOrEmpty() ? -1 : 0;

                base.Initialize();
            }
            catch( Exception e )
            {
                e.StoreFileLine();
                this.StoreReaderInfo(e);
                throw;
            }
        }

        /// <summary>
        /// Loads file data store entries from a CSV reader.
        /// </summary>
        /// <param name="reader">The CSV reader to use.</param>
        /// <returns>The file data store entries found.</returns>
        protected FileDataStoreEntry[] LoadEntries( Mechanical.FileFormats.CsvReader reader )
        {
            reader.Read(); // skip headers

            var list = new List<FileDataStoreEntry>();
            while( reader.Read() )
                list.Add(FileDataStoreEntry.FromCsvLine(reader));

            return list.ToArray();
        }

        /// <summary>
        /// Moves the reader to the next <see cref="DataStoreToken"/>.
        /// </summary>
        /// <param name="name">The name of the data store node found; or <c>null</c>.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected sealed override DataStoreToken ReadToken( out string name )
        {
            if( this.currentEntryAt == -1
             && this.nextEntryAt == -1 )
            {
                // data store end already reached previously
                name = null;
                return DataStoreToken.DataStoreEnd;
            }

            DataStoreToken result;
            if( this.nextEntryAt == -1 )
            {
                // last entry
                name = null;
                if( this.Depth == 0 )
                {
                    result = DataStoreToken.DataStoreEnd;
                    this.currentEntryAt = -1;
                }
                else
                    result = DataStoreToken.ObjectEnd;
            }
            else
            {
                // non-last entry
                bool firstEntry = this.currentEntryAt == -1;

                string currentParentPath = null;
                var nextEntry = default(FileDataStoreEntry);
                if( !firstEntry )
                {
                    // neither first, nor last entry
                    nextEntry = this.entries[this.nextEntryAt];
                    currentParentPath = DataStore.GetParentPath(this.Path);
                    if( currentParentPath.NullOrEmpty() )
                        throw new FormatException("Multiple root nodes, or entries ar in bad order!").StoreFileLine();
                }

                if( firstEntry
                 || DataStore.Comparer.Equals(currentParentPath, DataStore.GetParentPath(nextEntry.DataStorePath)) )
                {
                    // first entry, or
                    // both entries share the same parent, e.g.:
                    //  dir1/dir2/a
                    //  dir1/dir2/b
                    this.currentEntryAt = this.nextEntryAt;
                    ++this.nextEntryAt;
                    if( this.nextEntryAt == this.entries.Length )
                        this.nextEntryAt = -1;

                    var currEntry = this.entries[this.currentEntryAt];
                    name = DataStore.GetNodeName(currEntry.DataStorePath);
                    result = currEntry.IsFile ? DataStoreToken.Value : DataStoreToken.ObjectStart;
                }
                else
                {
                    // not the first entry, and
                    // both entries have different parents, e.g.:
                    //  dir1/dir2/dir3/a
                    //  dir1/dir4
                    name = null;
                    result = DataStoreToken.ObjectEnd;

                    //// NOTE: entry index is increased by the other 'if' branch, once the common parent is found
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the data store value the reader is currently at.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to use; or <c>null</c>.</param>
        /// <param name="binaryReader">The <see cref="IBinaryReader"/> to use; or <c>null</c>.</param>
        protected sealed override void ReadValue( out ITextReader textReader, out IBinaryReader binaryReader )
        {
            var currEntry = this.entries[this.currentEntryAt];
            if( currEntry.IsBinary )
            {
                textReader = null;
                binaryReader = this.OpenBinaryFile(currEntry.RawPath);
            }
            else
            {
                textReader = this.OpenTextFile(currEntry.RawPath);
                binaryReader = null;
            }
        }

        /// <summary>
        /// Begins reading the data store object the reader is currently at.
        /// </summary>
        protected sealed override void ReadObjectStart()
        {
        }

        /// <summary>
        /// Ends reading the data store object the reader is currently at.
        /// </summary>
        protected sealed override void ReadObjectEnd()
        {
        }

        /// <summary>
        /// Stores additional information about the state of the reader, on exceptions being thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception being thrown.</typeparam>
        /// <param name="exception">The exception being thrown.</param>
        protected override void StoreReaderInfo<TException>( TException exception )
        {
            base.StoreReaderInfo<TException>(exception);

            exception.Store("CurrentEntryAt", this.currentEntryAt);
            exception.Store("NextEntryAt", this.currentEntryAt);
            if( this.entries.NotNullReference() )
            {
                if( 0 <= this.currentEntryAt && this.currentEntryAt < this.entries.Length )
                {
                    var entry = this.entries[this.currentEntryAt];
                    exception.Store("CurrentRawPath", entry.RawPath);
                    exception.Store("CurrentDataStorePath", entry.DataStorePath);
                }

                if( 0 <= this.nextEntryAt && this.nextEntryAt < this.entries.Length )
                {
                    var entry = this.entries[this.nextEntryAt];
                    exception.Store("NextRawPath", entry.RawPath);
                    exception.Store("NextDataStorePath", entry.DataStorePath);
                }
            }
        }

        #endregion
    }
}
