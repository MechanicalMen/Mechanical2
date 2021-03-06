﻿using System;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Nodes
{
    /// <summary>
    /// A binary based data store value.
    /// </summary>
    public class DataStoreBinaryValue : DataStoreNode, IDataStoreBinaryValue
    {
        #region Private Fields

        private static readonly ArraySegment<byte> EmptyBytes = new ArraySegment<byte>(new byte[0]);

        private ArraySegment<byte> content;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreBinaryValue"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        public DataStoreBinaryValue( string name )
            : this(name, EmptyBytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreBinaryValue"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        /// <param name="content">The content of the data store value.</param>
        public DataStoreBinaryValue( string name, ArraySegment<byte> content )
            : base(name)
        {
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreBinaryValue"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        /// <param name="content">The content of the data store value.</param>
        public DataStoreBinaryValue( string name, byte[] content )
            : base(name)
        {
            Ensure.That(content).NotNull();

            this.Content = new ArraySegment<byte>(content);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreBinaryValue"/> class.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="writeContent">Writes the content of the data store value to the <see cref="IBinaryWriter"/> provided.</param>
        public DataStoreBinaryValue( string name, Action<IBinaryWriter> writeContent )
            : base(name)
        {
            Ensure.That(writeContent).NotNull();

            byte[] array;
            using( var ms = new MemoryStream() )
            {
                var writer = new BinaryStreamWriterLE(ms);
                writeContent(writer);

                ms.Flush();
                array = ms.ToArray();
            }
            this.Content = new ArraySegment<byte>(array);
        }

        #endregion

        #region IDataStoreBinaryValue

        /// <summary>
        /// Gets or sets the content of the data store value.
        /// </summary>
        /// <value>The content of the data store value.</value>
        public ArraySegment<byte> Content
        {
            get
            {
                return this.content;
            }
            set
            {
                if( value.Array.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.content = value;
            }
        }

        #endregion
    }
}
