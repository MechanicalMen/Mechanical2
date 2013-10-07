using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// A data store writer, which writes to an <see cref="IDataStoreNode"/>.
    /// </summary>
    public class DataStoreNodeWriter : DataStoreWriterBase.Disposable
    {
        #region Private Fields

        private readonly List<IDataStoreObject> parents;
        private readonly DataStoreValueSerializer valueSerializer;
        private readonly DataStoreObjectSerializer objectSerializer;
        private IDataStoreNode root;
        private System.IO.MemoryStream memoryStream;
        private BinaryStreamWriterLE binaryWriter;
        private StringWriter textWriter;
        private bool isBinary = true;
        private string currentValueNodeName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreNodeWriter"/> class.
        /// </summary>
        /// <param name="valueSerializer">The <see cref="DataStoreValueSerializer"/> to use.</param>
        /// <param name="objectSerializer">The <see cref="DataStoreObjectSerializer"/> to use.</param>
        public DataStoreNodeWriter( DataStoreValueSerializer valueSerializer, DataStoreObjectSerializer objectSerializer )
        {
            Ensure.That(valueSerializer).NotNull();
            Ensure.That(objectSerializer).NotNull();

            this.root = null;
            this.parents = new List<IDataStoreObject>();
            this.valueSerializer = valueSerializer;
            this.objectSerializer = objectSerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreNodeWriter"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum number of characters or bytes to read; or <c>-1</c> to read them all.</param>
        public DataStoreNodeWriter( int maxLength = -1 )
        {
            this.root = null;
            this.parents = new List<IDataStoreObject>();
            this.valueSerializer = new DataStoreValueSerializer(maxLength);
            this.objectSerializer = new DataStoreObjectSerializer(this.valueSerializer);
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

                if( this.memoryStream.NotNullReference() )
                {
                    this.memoryStream.Dispose();
                    this.memoryStream = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)
            this.root = null;
            this.binaryWriter = null;
            this.textWriter = null;

            base.OnDispose(disposing);
        }

        #endregion

        #region Private Members

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ThrowIfDisposed()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void AddChild( IDataStoreNode node )
        {
            if( this.parents.Count == 0 )
            {
                if( this.root.NotNullReference() )
                    throw new InvalidOperationException("There can be at most one root node!").StoreFileLine();
                else
                    this.root = node;
            }
            else
            {
                var parent = this.parents[this.parents.Count - 1];
                parent.Nodes.Add(node);
            }
        }

        #endregion

        #region DataStoreWriterBase

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
                if( isObjectStart.HasValue )
                {
                    if( isObjectStart.Value )
                    {
                        var node = new DataStoreObject(name);
                        this.parents.Add(node);
                    }
                    else
                    {
                        var index = this.parents.Count - 1;
                        var node = this.parents[index];
                        this.parents.RemoveAt(index);
                        this.AddChild(node);
                    }

                    isBinary = null;
                }
                else
                {
                    this.currentValueNodeName = name;
                    isBinary = this.IsBinary;
                }

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

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override IBinaryWriter OpenBinaryWriter()
        {
            if( this.memoryStream.NullReference() )
            {
                this.memoryStream = new System.IO.MemoryStream();
                this.binaryWriter = new BinaryStreamWriterLE(this.memoryStream);
            }

            return this.binaryWriter;
        }

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override ITextWriter OpenTextWriter()
        {
            if( this.textWriter.NullReference() )
                this.textWriter = new StringWriter();

            return this.textWriter;
        }

        /// <summary>
        /// Releases any resources held by an open writer.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected override void CloseWriter( IBinaryWriter writer )
        {
            var valueNode = new DataStoreBinaryValue(this.currentValueNodeName, this.memoryStream.ToArray());
            this.memoryStream.SetLength(0);

            this.AddChild(valueNode);
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected override void CloseWriter( ITextWriter writer )
        {
            var valueNode = new DataStoreTextValue(this.currentValueNodeName, this.textWriter.ToString());
            this.currentValueNodeName = null;
            this.textWriter.Clear();

            this.AddChild(valueNode);
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
                this.ThrowIfDisposed();

                return this.isBinary;
            }
            set
            {
                this.ThrowIfDisposed();

                this.isBinary = value;
            }
        }

        /// <summary>
        /// Gets the root node of the data store being written.
        /// </summary>
        /// <value>The root node of the data store being written.</value>
        public IDataStoreNode Root
        {
            get
            {
                this.ThrowIfDisposed();

                return this.root;
            }
        }

        /// <summary>
        /// Clears the writer (just as if it were newly created).
        /// </summary>
        public void Clear()
        {
            this.ThrowIfDisposed();

            this.root = null;
            this.parents.Clear();
        }

        #endregion
    }
}
