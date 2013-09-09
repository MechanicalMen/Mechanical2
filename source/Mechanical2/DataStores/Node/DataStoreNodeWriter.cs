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
    public class DataStoreNodeWriter : DisposableObject, IDataStoreWriter
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
                throw new ObjectDisposedException("this").StoreDefault();
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ThrowIfSecondRoot()
        {
            if( this.parents.Count == 0
             && this.root.NotNullReference() )
                throw new InvalidOperationException("Root node already written! (There can be only one)").StoreDefault();
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void AddChild( IDataStoreNode node )
        {
            if( this.parents.Count == 0 )
                this.root = node;
            else
            {
                var parent = this.parents[this.parents.Count - 1];
                parent.Nodes.Add(node);
            }
        }

        #endregion

        #region IDataStoreWriter

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="name">The name assigned to the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        public void Write<T>( string name, T obj, IDataStoreValueSerializer<T> serializer )
        {
            this.ThrowIfDisposed();
            this.ThrowIfSecondRoot();

            if( serializer.NullReference() )
                throw new ArgumentNullException("serializer").StoreDefault();

            IDataStoreValue value;
            if( this.isBinary )
            {
                if( this.memoryStream.NullReference() )
                {
                    this.memoryStream = new System.IO.MemoryStream();
                    this.binaryWriter = new BinaryStreamWriterLE(this.memoryStream);
                }

                serializer.Serialize(obj, this.binaryWriter);

                value = new DataStoreBinaryValue(name, this.memoryStream.ToArray());
                this.memoryStream.SetLength(0);
            }
            else
            {
                if( this.textWriter.NullReference() )
                    this.textWriter = new StringWriter();

                serializer.Serialize(obj, this.textWriter);

                value = new DataStoreTextValue(name, this.textWriter.ToString());
                this.textWriter.Clear();
            }

            this.AddChild(value);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="name">The name assigned to the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        public void Write<T>( string name, T obj, IDataStoreObjectSerializer<T> serializer )
        {
            this.ThrowIfDisposed();
            this.ThrowIfSecondRoot();

            if( serializer.NullReference() )
                throw new ArgumentNullException("serializer").StoreDefault();

            var node = new DataStoreObject(name);
            this.parents.Add(node);

            serializer.Serialize(obj, this);

            this.parents.RemoveAt(this.parents.Count - 1);
            this.AddChild(node);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets a value indicating whether values should be binary or text based.
        /// </summary>
        /// <value>Determines whether values should be binary or text based.</value>
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
