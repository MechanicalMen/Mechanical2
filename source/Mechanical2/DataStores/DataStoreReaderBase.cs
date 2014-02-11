using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores
{
    /// <summary>
    /// A base class that helps implementing <see cref="IDataStoreReader"/>
    /// </summary>
    public abstract class DataStoreReaderBase : IDataStoreReader
    {
        #region Disposable

        /// <summary>
        /// A base class that helps implementing <see cref="IDataStoreReader"/> and <see cref="IDisposableObject"/>
        /// </summary>
        public abstract class Disposable : DataStoreReaderBase, IDisposableObject
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Disposable"/> class.
            /// </summary>
            protected Disposable()
                : base()
            {
            }

            #endregion

            #region IDisposableObject

            private readonly object disposeLock = new object();
            private bool isDisposed = false;

            /// <summary>
            /// Gets a value indicating whether this object has been disposed of.
            /// </summary>
            /// <value><c>true</c> if this object has been disposed of; otherwise, <c>false</c>.</value>
            public new bool IsDisposed
            {
                get { return this.isDisposed; }
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="Disposable"/> class.
            /// </summary>
            /// <remarks>
            /// Releases unmanaged resources and performs other cleanup operations before the
            /// object is reclaimed by garbage collection.
            /// </remarks>
            ~Disposable()
            {
                // call Dispose with false. Since we're in the destructor call,
                // the managed resources will be disposed of anyways.
                this.Dispose(false);
            }

            /// <summary>
            /// Performs tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // dispose of the managed and unmanaged resources
                this.Dispose(true);

                // tell the GC that Finalize no longer needs
                // to be run for this object.
                System.GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Called before the object is disposed of. Inheritors must call base.OnDisposing to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, the method was called by Dispose; otherwise by the destructor.</param>
            protected virtual void OnDisposing( bool disposing )
            {
                // see notes on DataStoreWriterBase.OnDisposing
                if( disposing )
                    this.CloseReaders();
            }

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected virtual void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)
                }

                //// shared cleanup logic
                //// (unmanaged resources)
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            private void Dispose( bool disposing )
            {
                // don't lock unless we have to
                if( !this.isDisposed )
                {
                    // not only do we want only one Dispose to run at a time,
                    // we also want to halt other callers until Dispose finishes.
                    lock( this.disposeLock )
                    {
                        // necessary if there are multiple concurrent calls
                        if( !this.isDisposed )
                        {
                            this.OnDisposing(disposing);
                            this.OnDispose(disposing);
                            this.isDisposed = true; // if an exception interrupts the process, we may not have been properly disposed of! (and isDisposed correctly stores false).
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Private Fields

        private readonly IDisposableObject asDisposableObject;
        private readonly List<string> parents = new List<string>();
        private DataStoreToken currentToken = DataStoreToken.DataStoreStart;
        private string currentName;
        private string currentPath;
        private IBinaryReader currentBinaryReader;
        private ITextReader currentTextReader;

#if DEBUG
        private bool rootNodeRead = false;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreReaderBase"/> class.
        /// </summary>
        protected DataStoreReaderBase()
        {
            this.asDisposableObject = this as IDisposableObject;
        }

        #endregion

        #region Private Members

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsDisposed()
        {
            return this.asDisposableObject.NotNullReference() && this.asDisposableObject.IsDisposed;
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ThrowIfDisposed()
        {
            if( this.IsDisposed() )
                throw new ObjectDisposedException(null).StoreFileLine();
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CloseReaders()
        {
            if( this.currentBinaryReader.NotNullReference() )
            {
                this.CloseReader(this.currentBinaryReader);
                this.currentBinaryReader = null;
            }

            if( this.currentTextReader.NotNullReference() )
            {
                this.CloseReader(this.currentTextReader);
                this.currentTextReader = null;
            }
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Reads the next name and token.
        /// </summary>
        /// <param name="name">The name of the data store node found. Ignored for ObjectEnd, DataStoreEnd tokens.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected abstract DataStoreToken ReadToken( out string name );

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected abstract IBinaryReader OpenBinaryReader();

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected abstract ITextReader OpenTextReader();

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected abstract void CloseReader( IBinaryReader reader );

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected abstract void CloseReader( ITextReader reader );

        #endregion

        #region IDataStoreReader

        /// <summary>
        /// Reads the next data store token.
        /// </summary>
        /// <returns><c>false</c> if the end of the data store was reached; otherwise, <c>true</c>.</returns>
        public bool Read()
        {
            this.ThrowIfDisposed();

            try
            {
                // data store end already reached, no need do anything else
                if( this.currentToken == DataStoreToken.DataStoreEnd )
                    return false;

                // we leave the token --> our name changes
                this.currentPath = null;

                // leaving object start --> new parent
                if( this.currentToken == DataStoreToken.ObjectStart )
                    this.parents.Add(this.currentName);

                // leaving value --> close any open readers
                this.CloseReaders();

                // read next token and name
                this.currentToken = this.ReadToken(out this.currentName);

#if DEBUG
                if( !this.currentToken.Wrap().IsDefined
                 || this.currentToken == DataStoreToken.DataStoreStart )
                    throw new Exception("Data store implementation returned invalid token!").StoreFileLine();

                if( this.currentToken != DataStoreToken.ObjectEnd
                 && this.currentToken != DataStoreToken.DataStoreEnd
                 && !DataStore.IsValidName(this.currentName) )
                    throw new Exception("Data store implementation returned invalid name!").StoreFileLine();

                if( this.parents.Count == 0
                 && this.currentToken != DataStoreToken.DataStoreEnd )
                {
                    if( !this.rootNodeRead )
                        this.rootNodeRead = true;
                    else if( this.currentToken != DataStoreToken.ObjectEnd ) // first object end token gets a pass, any others throw an exception later
                        throw new FormatException("There can be at most one root node!").StoreFileLine();
                }
#endif
                // process the token read
                switch( this.currentToken )
                {
                case DataStoreToken.DataStoreEnd:
                    // release any resources
                    this.currentName = null;
                    this.currentPath = null;

                    if( this.parents.Count != 0 )
                        throw new FormatException("Data store ended unexpectedly: there are still objects open!").Store("Depth", this.parents.Count); // not stored automatically, since Depth would throw an exception
                    break;

                case DataStoreToken.ObjectStart:
                    // next Read increases Depth (see above)
                    break;

                case DataStoreToken.ObjectEnd:
                    {
                        if( this.parents.Count == 0 )
                            throw new FormatException("Unexpected object end token found: there are no objects open!").StoreFileLine();

                        // decrease depth
                        int lastAt = this.parents.Count - 1;
                        this.currentName = this.parents[lastAt];
                        this.parents.RemoveAt(lastAt);
                    }
                    break;

                case DataStoreToken.BinaryValue:
                case DataStoreToken.TextValue:
                    // we don't create the readers if we don't have to
                    break;

                default:
                    throw new Exception("Data store reader implementation returned invalid token!").StoreFileLine();
                }

                return this.currentToken != DataStoreToken.DataStoreEnd;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                this.StorePosition(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the current data store token.
        /// </summary>
        /// <value>The current data store token.</value>
        public DataStoreToken Token
        {
            get
            {
                // NOTE: we don't throw any exceptions from here, in case this property is stored as some exception's data
                if( this.IsDisposed() )
                    return (DataStoreToken)(-1);

                return this.currentToken;
            }
        }

        /// <summary>
        /// Gets the name of the current object or value.
        /// </summary>
        /// <value>The name of the current object or value.</value>
        public string Name
        {
            get
            {
                // NOTE: we don't throw any exceptions from here, in case this property is stored as some exception's data
                if( this.IsDisposed()
                 || this.currentToken == DataStoreToken.DataStoreStart
                 || this.currentToken == DataStoreToken.DataStoreEnd )
                    return null;

                return this.currentName;
            }
        }

        /// <summary>
        /// Gets the zero-based depth of the current object or value.
        /// </summary>
        /// <value>The zero-based depth of the current object or value.</value>
        public int Depth
        {
            get
            {
                // NOTE: we don't throw any exceptions from here, in case this property is stored as some exception's data
                if( this.IsDisposed()
                 || this.currentToken == DataStoreToken.DataStoreStart
                 || this.currentToken == DataStoreToken.DataStoreEnd )
                    return -1;

                return this.parents.Count;
            }
        }

        /// <summary>
        /// Gets the absolute path to the current object or value.
        /// </summary>
        /// <value>The absolute path to the current object or value.</value>
        public string Path
        {
            get
            {
                // NOTE: we don't throw any exceptions from here, in case this property is stored as some exception's data
                if( this.IsDisposed()
                 || this.currentToken == DataStoreToken.DataStoreStart
                 || this.currentToken == DataStoreToken.DataStoreEnd )
                    return null;

                if( this.currentPath.NullReference() )
                {
                    if( this.parents.Count == 0 )
                    {
                        this.currentPath = this.currentName;
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for( int i = 0; i < this.parents.Count; ++i )
                        {
                            sb.Append(this.parents[i]);
                            sb.Append(DataStore.PathSeparator);
                        }
                        sb.Append(this.currentName);

                        this.currentPath = sb.ToString();
                    }
                }

                return this.currentPath;
            }
        }

        /// <summary>
        /// Gets the reader of a binary value.
        /// There is exactly one - binary or text - reader for each value.
        /// Calling more than once, returns either the same reader, reset to the start of the value; or a new reader, while releasing the resources of the old one.
        /// </summary>
        /// <value>The reader of a binary value.</value>
        public IBinaryReader BinaryValueReader
        {
            get
            {
                try
                {
                    this.ThrowIfDisposed();

                    if( this.currentToken != DataStoreToken.BinaryValue )
                        throw new InvalidOperationException("Invalid token!").StoreFileLine();

                    // already open? close!
                    this.CloseReaders();

                    // open reader
                    this.currentBinaryReader = this.OpenBinaryReader();
                    return this.currentBinaryReader;
                }
                catch( Exception ex )
                {
                    ex.StoreFileLine();
                    this.StorePosition(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the reader of a text value.
        /// There is exactly one - binary or text - reader for each value.
        /// Calling more than once, returns either the same reader, reset to the start of the value; or a new reader, while releasing the resources of the old one.
        /// </summary>
        /// <value>The reader of a text value.</value>
        public ITextReader TextValueReader
        {
            get
            {
                try
                {
                    this.ThrowIfDisposed();

                    if( this.currentToken != DataStoreToken.TextValue )
                        throw new InvalidOperationException("Invalid token!").StoreFileLine();

                    // already open? close!
                    this.CloseReaders();

                    // open reader
                    this.currentTextReader = this.OpenTextReader();
                    return this.currentTextReader;
                }
                catch( Exception ex )
                {
                    ex.StoreFileLine();
                    this.StorePosition(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Stores debug information about the current state of the reader, into the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to store the state of the reader in.</param>
        public virtual void StorePosition( Exception exception )
        {
            // NOTE: we don't throw any exceptions from here, in case this property is used to store as some exception's data
            if( exception.NotNullReference() )
            {
                exception.Store("Token", this.Token);
                exception.Store("Name", this.Name);

                // prevent stack overflow (these properties throw exceptions)
                if( this.Token != DataStoreToken.DataStoreStart
                 || this.Token != DataStoreToken.DataStoreEnd )
                {
                    exception.Store("Depth", this.Depth);
                    exception.Store("Path", this.Path);
                }
            }
        }

        #endregion
    }
}
