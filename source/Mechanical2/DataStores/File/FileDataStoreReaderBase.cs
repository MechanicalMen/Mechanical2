using System;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.File
{
    /// <summary>
    /// A base class for file (or archive) based data store readers.
    /// </summary>
    public abstract class FileDataStoreReaderBase : DataStoreReaderBase.Disposable
    {
        #region Private Fields

        private FileDataStoreEntryList entries = null;
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
        /// Loads the file data store entries. Children are placed after their parents, in the correct order.
        /// </summary>
        /// <returns>The file data store entries found.</returns>
        protected abstract FileDataStoreEntryList LoadEntries();

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="entries">The entries of the file data store.</param>
        /// <param name="entry">The file data store entry to open for reading.</param>
        /// <returns>The <see cref="ITextReader"/> representing the file contents.</returns>
        protected abstract ITextReader OpenTextFile( FileDataStoreEntryList entries, FileDataStoreEntry entry );

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="entries">The entries of the file data store.</param>
        /// <param name="entry">The file data store entry to open for reading.</param>
        /// <returns>The <see cref="IBinaryReader"/> representing the file contents.</returns>
        protected abstract IBinaryReader OpenBinaryFile( FileDataStoreEntryList entries, FileDataStoreEntry entry );

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads the next name and token.
        /// </summary>
        /// <param name="name">The name of the data store node found. Ignored for ObjectEnd, DataStoreEnd tokens.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected sealed override DataStoreToken ReadToken( out string name )
        {
            if( this.Token == DataStoreToken.DataStoreStart )
            {
                this.entries = this.LoadEntries();
                this.currentEntryAt = -1;
                this.nextEntryAt = this.entries.NullOrEmpty() ? -1 : 0;
            }

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
                    if( this.nextEntryAt == this.entries.Count )
                        this.nextEntryAt = -1;

                    var currEntry = this.entries[this.currentEntryAt];
                    name = DataStore.GetNodeName(currEntry.DataStorePath);
                    result = currEntry.Token;
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
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected sealed override IBinaryReader OpenBinaryReader()
        {
            var currEntry = this.entries[this.currentEntryAt];
            return this.OpenBinaryFile(this.entries, currEntry);
        }

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected sealed override ITextReader OpenTextReader()
        {
            var currEntry = this.entries[this.currentEntryAt];
            return this.OpenTextFile(this.entries, currEntry);
        }

        /// <summary>
        /// Stores debug information about the current state of the reader, into the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to store the state of the reader in.</param>
        public override void StorePosition( Exception exception )
        {
            base.StorePosition(exception);

            if( exception.NotNullReference() )
            {
                exception.Store("CurrentEntryAt", this.currentEntryAt);
                exception.Store("NextEntryAt", this.currentEntryAt);
                if( this.entries.NotNullReference() )
                {
                    if( 0 <= this.currentEntryAt && this.currentEntryAt < this.entries.Count )
                    {
                        var entry = this.entries[this.currentEntryAt];
                        exception.Store("CurrentEntry", entry);
                    }

                    if( 0 <= this.nextEntryAt && this.nextEntryAt < this.entries.Count )
                    {
                        var entry = this.entries[this.nextEntryAt];
                        exception.Store("NextEntry", entry);
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the file (or directory).
        /// </summary>
        /// <value>The name of the file (or directory).</value>
        public string FileName
        {
            get
            {
                try
                {
                    if( this.IsDisposed )
                        throw new ObjectDisposedException(null).StoreFileLine();

                    var token = this.Token;
                    if( token == DataStoreToken.DataStoreStart
                     || token == DataStoreToken.DataStoreEnd
                     || token == DataStoreToken.ObjectEnd )
                        throw new InvalidOperationException("Invalid token!").StoreFileLine();

                    var currEntry = this.entries[this.currentEntryAt];
                    if( currEntry.FileName.NullOrEmpty() )
                        return this.Name;
                    else
                        return currEntry.FileName;
                }
                catch( Exception ex )
                {
                    ex.StoreFileLine();
                    this.StorePosition(ex);
                    throw;
                }
            }
        }

        #endregion
    }
}
