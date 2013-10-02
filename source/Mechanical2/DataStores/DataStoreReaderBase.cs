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
            public bool IsDisposed
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

                    /*
                    if( resource.NotNullReference() )
                    {
                        resource.Dispose();
                        resource = null;
                    }
                    */
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

        private readonly List<string> parents = new List<string>();

        private DataStoreToken currentToken = (DataStoreToken)(-1); // so that our first read call does not "follow" a valid token
        private string currentName;
        private string currentPath;

#if DEBUG
        private bool initialized = false;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreReaderBase"/> class.
        /// </summary>
        protected DataStoreReaderBase()
        {
        }

        #endregion

        #region Private Members

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ThrowIfAtDataStoreEnd( [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0 )
        {
            if( this.Token == DataStoreToken.DataStoreEnd )
            {
                var exception = new InvalidOperationException("End of data store already reached!").StoreFileLine(filePath, memberName, lineNumber);
                this.StoreReaderInfo(exception);
                throw exception;
            }
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void MoveToNextToken()
        {
            // we leave the token --> our name changes
            this.currentPath = null;

            // leaving object start --> new parent
            if( this.currentToken == DataStoreToken.ObjectStart )
            {
                this.ReadObjectStart();
                this.parents.Add(this.currentName);
            }

            this.currentToken = this.ReadToken(out this.currentName);
#if DEBUG
            if( !DataStore.IsValidName(this.currentName) )
                throw new Exception("Data store implementation returned invalid name!").Store("currentToken", this.currentToken).Store("currentName", this.currentName);
#endif

            switch( this.currentToken )
            {
            case DataStoreToken.DataStoreEnd:
                this.currentName = null;

                if( this.parents.Count != 0 )
                    this.ThrowIfAtDataStoreEnd();
                break;

            case DataStoreToken.ObjectStart:
                break;

            case DataStoreToken.ObjectEnd:
                {
                    // clean up
                    this.ReadObjectEnd();

                    // decrease depth
                    int lastAt = this.parents.Count - 1;
                    this.currentName = this.parents[lastAt];
                    this.parents.RemoveAt(lastAt);
                }
                break;

            case DataStoreToken.Value:
                break;

            default:
                throw new Exception("Data store reader implementation returned invalid token!");
            }
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ReadUntilObjectEnd( int depth )
        {
            while( !(this.currentToken == DataStoreToken.ObjectEnd && this.Depth == depth) )
            {
                this.MoveToNextToken();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes the reader.
        /// </summary>
        protected void Initialize()
        {
            //// NOTE: we can't put this into the constructor,
            ////       since that is called before the actual constructor
            ////       and that would not allow us to process it's parameters

            try
            {
#if DEBUG
                if( this.initialized )
                    throw new InvalidOperationException("Already initialized!");
#endif
                this.MoveToNextToken();
            }
            catch( Exception e )
            {
                e.StoreFileLine();
                this.StoreReaderInfo(e);
                throw;
            }
        }

        /// <summary>
        /// Moves the reader to the next <see cref="DataStoreToken"/>.
        /// </summary>
        /// <param name="name">The name of the data store node found; or <c>null</c>.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected abstract DataStoreToken ReadToken( out string name );

        /// <summary>
        /// Reads the data store value the reader is currently at.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to use; or <c>null</c>.</param>
        /// <param name="binaryReader">The <see cref="IBinaryReader"/> to use; or <c>null</c>.</param>
        protected abstract void ReadValue( out ITextReader textReader, out IBinaryReader binaryReader );

        /// <summary>
        /// Begins reading the data store object the reader is currently at.
        /// </summary>
        protected abstract void ReadObjectStart();

        /// <summary>
        /// Ends reading the data store object the reader is currently at.
        /// </summary>
        protected abstract void ReadObjectEnd();

        /// <summary>
        /// Stores additional information about the state of the reader, on exceptions being thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception being thrown.</typeparam>
        /// <param name="exception">The exception being thrown.</param>
        protected virtual void StoreReaderInfo<TException>( TException exception )
            where TException : Exception
        {
            exception.Store("Token", this.Token);
            exception.Store("Name", this.Name);
            exception.Store("Depth", this.Depth);

            if( this.Token != DataStoreToken.DataStoreEnd ) // prevent StackOverflow
                exception.Store("Path", this.Path);
        }

        #endregion

        #region IDataStoreReader

        /// <summary>
        /// Gets the current data store token.
        /// </summary>
        /// <value>The current data store token.</value>
        public DataStoreToken Token
        {
            get
            {
#if DEBUG
                if( this.initialized )
                {
                    var exception = new InvalidOperationException("Already initialized!").StoreFileLine();
                    this.StoreReaderInfo(exception);
                    throw exception;
                }
#endif
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
                this.ThrowIfAtDataStoreEnd();

                return this.currentName;
            }
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store value.</returns>
        public T Read<T>( IDataStoreValueDeserializer<T> deserializer )
        {
            this.ThrowIfAtDataStoreEnd();

            if( deserializer.NullReference() )
            {
                var exception = new ArgumentNullException("deserializer").StoreFileLine();
                this.StoreReaderInfo(exception);
                throw exception;
            }

            if( this.currentToken != DataStoreToken.Value )
            {
                var exception = new FormatException("Data store value expected!");
                this.StoreReaderInfo(exception);
                throw exception;
            }

            // deserialize
            T obj;
            ITextReader textReader;
            IBinaryReader binaryReader;
            try
            {
                this.ReadValue(out textReader, out binaryReader);
#if DEBUG
                if( (textReader.NullReference() && binaryReader.NullReference())
                 || (textReader.NotNullReference() && binaryReader.NotNullReference()) )
                    throw new Exception("Data store reader implementation must return exactly one value-reader!");
#endif
                if( textReader.NullReference() )
                    obj = deserializer.Deserialize(this.currentName, binaryReader);
                else
                    obj = deserializer.Deserialize(this.currentName, textReader);

                // move to next sibling, if there is one
                this.MoveToNextToken();
            }
            catch( Exception e )
            {
                e.StoreFileLine();
                this.StoreReaderInfo(e);
                throw;
            }

            return obj;
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store object.</returns>
        public T Read<T>( IDataStoreObjectDeserializer<T> deserializer )
        {
            this.ThrowIfAtDataStoreEnd();

            if( deserializer.NullReference() )
            {
                var exception = new ArgumentNullException("deserializer").StoreFileLine();
                this.StoreReaderInfo(exception);
                throw exception;
            }

            if( this.currentToken != DataStoreToken.ObjectStart )
            {
                var exception = new FormatException("Data store object start expected!");
                this.StoreReaderInfo(exception);
                throw exception;
            }

            // deserialize
            int originalDepth = this.parents.Count;
            T obj;
            try
            {
                var name = this.currentName;
                this.MoveToNextToken(); // move to first child or object end
                obj = deserializer.Deserialize(name, this);

                // read object to end (in case the deserializer didn't)
                this.ReadUntilObjectEnd(originalDepth);

                // move to next sibling, if there is one
                this.MoveToNextToken();
            }
            catch( Exception e )
            {
                e.StoreFileLine();
                this.StoreReaderInfo(e);
                throw;
            }

            return obj;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the current depth of the reader.
        /// </summary>
        /// <value>The current depth of the reader.</value>
        public int Depth
        {
            get { return this.parents.Count; }
        }

        /// <summary>
        /// Gets the current data store path.
        /// </summary>
        /// <value>The current data store path.</value>
        public string Path
        {
            get
            {
                this.ThrowIfAtDataStoreEnd();

                if( this.currentPath.NullReference() )
                {
                    if( this.parents.Count == 0 )
                        this.currentPath = this.currentName;
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

        #endregion
    }
}
