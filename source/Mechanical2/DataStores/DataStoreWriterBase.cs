using System;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores
{
    /// <summary>
    /// A base class that helps implementing <see cref="IDataStoreWriter"/>
    /// </summary>
    public abstract class DataStoreWriterBase : IDataStoreWriter
    {
        #region Disposable

        /// <summary>
        /// A base class that helps implementing <see cref="IDataStoreWriter"/> and <see cref="IDisposableObject"/>
        /// </summary>
        public abstract class Disposable : DataStoreWriterBase, IDisposableObject
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
                // NOTE: it is important to close any open writers,
                //       before the disposal of the subclass:
                //       in case the root node is a value, and
                //       the subclass writes content when the writers are closed! (XmlDataStoreWriter)
                if( disposing )
                    this.CloseWriters();
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
        private IBinaryWriter currentBinaryWriter;
        private ITextWriter currentTextWriter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreWriterBase"/> class.
        /// </summary>
        protected DataStoreWriterBase()
        {
            this.asDisposableObject = this as IDisposableObject;
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ThrowIfDisposed()
        {
            if( this.asDisposableObject.NotNullReference()
             && this.asDisposableObject.IsDisposed )
                throw new ObjectDisposedException(null).StoreFileLine();
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CloseWriters()
        {
            if( this.currentBinaryWriter.NotNullReference() )
            {
                this.CloseWriter(this.currentBinaryWriter);
                this.currentBinaryWriter = null;
            }

            if( this.currentTextWriter.NotNullReference() )
            {
                this.CloseWriter(this.currentTextWriter);
                this.currentTextWriter = null;
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Writes an ObjectStart, ObjectEnd, or a value.
        /// </summary>
        /// <param name="name">The data store name to use; or <c>null</c> for the ObjectEnd token.</param>
        /// <param name="isObjectStart"><c>true</c> if an ObjectStart token is to be written, <c>false</c> for ObjectEnd, and <c>null</c> for a binary or text value.</param>
        /// <returns>A value determining whether the value to be written is binary or not. <c>null</c> if an object was written.</returns>
        protected abstract bool? Write( string name, bool? isObjectStart );

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected abstract IBinaryWriter OpenBinaryWriter();

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected abstract ITextWriter OpenTextWriter();

        /// <summary>
        /// Releases any resources held by an open writer.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected abstract void CloseWriter( IBinaryWriter writer );

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected abstract void CloseWriter( ITextWriter writer );

        #endregion

        #region IDataStoreWriter

        /// <summary>
        /// Writes a BinaryValue or TextValue token. The writer implementation chooses which one.
        /// There is exactly one - binary or text - reader for each value.
        /// </summary>
        /// <param name="name">The name of the data store value.</param>
        /// <param name="binaryWriter">The <see cref="IBinaryWriter"/> that can be used to write the value; or <c>null</c>.</param>
        /// <param name="textWriter">The <see cref="ITextWriter"/> that can be used to write the value; or <c>null</c>.</param>
        public void WriteValue( string name, out IBinaryWriter binaryWriter, out ITextWriter textWriter )
        {
            this.ThrowIfDisposed();

            if( !DataStore.IsValidName(name) )
                throw new ArgumentException("Invalid data store name!").Store("name", name);

            this.CloseWriters();
            var isBinary = this.Write(name, isObjectStart: null);
            if( isBinary.HasValue )
            {
                this.currentBinaryWriter = isBinary.Value ? this.OpenBinaryWriter() : null;
                this.currentTextWriter = isBinary.Value ? null : this.OpenTextWriter();
                binaryWriter = this.currentBinaryWriter;
                textWriter = this.currentTextWriter;
            }
            else
            {
                binaryWriter = null;
                textWriter = null;
            }
        }

        /// <summary>
        /// Writes an ObjectStart token.
        /// </summary>
        /// <param name="name">The name of the data store object.</param>
        public void WriteObjectStart( string name )
        {
            this.ThrowIfDisposed();

            if( !DataStore.IsValidName(name) )
                throw new ArgumentException("Invalid data store name!").Store("name", name);

            this.CloseWriters();
#if DEBUG
            var isBinary = this.Write(name, isObjectStart: true);
            if( isBinary.HasValue )
                throw new Exception("Data store implementation indicated a value, while an object is being handled!").StoreFileLine();
#else
            this.Write(name, isObjectStart: true);
#endif
        }

        /// <summary>
        /// Writes an ObjectEnd token.
        /// </summary>
        public void WriteObjectEnd()
        {
            this.ThrowIfDisposed();

            this.CloseWriters();
#if DEBUG
            var isBinary = this.Write(name: null, isObjectStart: false);
            if( isBinary.HasValue )
                throw new Exception("Data store implementation indicated a value, while an object is being handled!").StoreFileLine();
#else
            this.Write(name: null, isObjectStart: false);
#endif
        }

        #endregion
    }
}
