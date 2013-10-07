using System;
using System.Collections.Generic;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.File
{
    /// <summary>
    /// A base class for file (or archive) based data store writers.
    /// </summary>
    public abstract class FileDataStoreWriterBase : DataStoreWriterBase.Disposable
    {
        #region Private Fields

        private readonly FileDataStoreEntryList entries;
        private readonly List<FileDataStoreEntry> parents;
        private bool isBinary = true;
        private string fileName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStoreWriterBase"/> class.
        /// </summary>
        protected FileDataStoreWriterBase()
            : base()
        {
            this.entries = new FileDataStoreEntryList();
            this.parents = new List<FileDataStoreEntry>();
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

                this.SaveEntries();
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            base.OnDispose(disposing);
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Gets the entries of the file data store.
        /// </summary>
        /// <value>The entries of the file data store.</value>
        protected FileDataStoreEntryList Entries
        {
            get { return this.entries; }
        }

        /// <summary>
        /// Saves the file entries of the data store.
        /// </summary>
        protected abstract void SaveEntries();

        /// <summary>
        /// Writes the specified file data store entry.
        /// </summary>
        /// <param name="entry">The file data store entry to open for reading.</param>
        protected abstract void WriteEntry( FileDataStoreEntry entry );

        /// <summary>
        /// Writes an ObjectStart, ObjectEnd, or a value.
        /// </summary>
        /// <param name="name">The data store name to use; or <c>null</c> for the ObjectEnd token.</param>
        /// <param name="isObjectStart"><c>true</c> if an ObjectStart token is to be written, <c>false</c> for ObjectEnd, and <c>null</c> for a binary or text value.</param>
        /// <returns>A value determining whether the value to be written is binary or not. <c>null</c> if an object was written.</returns>
        protected override bool? Write( string name, bool? isObjectStart )
        {
            try
            {
                if( isObjectStart != false
                 && !DataStore.IsValidName(name) )
                    throw new ArgumentException("Invalid data store name!").StoreFileLine();

                bool? isBinary;
                FileDataStoreEntry entry;
                if( isObjectStart.HasValue )
                {
                    if( isObjectStart.Value )
                    {
                        if( this.parents.Count == 0 )
                            entry = new FileDataStoreEntry(DataStoreToken.ObjectStart, name, this.FileName);
                        else
                            entry = new FileDataStoreEntry(DataStoreToken.ObjectStart, DataStore.Combine(this.parents[this.parents.Count - 1].DataStorePath, name), this.FileName);
                        this.parents.Add(entry);

                        isBinary = null;
                    }
                    else
                    {
                        this.parents.RemoveAt(this.parents.Count - 1);
                        return null; // no WriteEntry call for ObjectEnd!
                    }
                }
                else
                {
                    if( this.parents.Count == 0 )
                        entry = new FileDataStoreEntry(this.IsBinary ? DataStoreToken.BinaryValue : DataStoreToken.TextValue, name, this.FileName);
                    else
                        entry = new FileDataStoreEntry(this.IsBinary ? DataStoreToken.BinaryValue : DataStoreToken.TextValue, DataStore.Combine(this.parents[this.parents.Count - 1].DataStorePath, name), this.FileName);

                    isBinary = this.IsBinary;
                }

                this.FileName = null;
                this.entries.AddToParent(entry);
                this.WriteEntry(entry);
                return isBinary;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("name", name);
                ex.Store("isObjectStart", isObjectStart);
                throw;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets a value indicating whether the next value written should be binary or text based.
        /// </summary>
        /// <value>Determines whether the next value written should be binary or text based.</value>
        public bool IsBinary
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.isBinary;
            }
            set
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.isBinary = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file (or directory) to be written. <c>null</c> if it is the same as the data store name.
        /// </summary>
        /// <value>The name of the file (or directory).</value>
        public string FileName
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.fileName;
            }
            set
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( value.NullOrEmpty() )
                    this.fileName = null;
                else
                    this.fileName = value;
            }
        }

        #endregion
    }
}
