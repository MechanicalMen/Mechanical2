using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// Serializes and deserializes <see cref="IDataStoreObject"/>.
    /// </summary>
    public class DataStoreObjectSerializer : IDataStoreObjectSerializer<IDataStoreObject>,
                                             IDataStoreObjectDeserializer<IDataStoreObject>
    {
        #region Private Fields

        private readonly DataStoreValueSerializer valueSerializer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreObjectSerializer"/> class.
        /// </summary>
        /// <param name="valueSerializer">The value serializer to use.</param>
        public DataStoreObjectSerializer( DataStoreValueSerializer valueSerializer )
        {
            Ensure.That(valueSerializer).NotNull();

            this.valueSerializer = valueSerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreObjectSerializer"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum number of characters or bytes to read; or <c>-1</c> to read them all.</param>
        public DataStoreObjectSerializer( int maxLength = -1 )
            : this(new DataStoreValueSerializer(maxLength))
        {
        }

        #endregion

        #region IDataStoreObjectSerializer

        /// <summary>
        /// Serializes to a data store object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The data store writer to use.</param>
        public void Serialize( IDataStoreObject obj, IDataStoreWriter writer )
        {
            if( obj.NullReference() )
                throw new ArgumentNullException("obj").StoreFileLine();

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            IDataStoreValue value;
            foreach( var node in obj.Nodes )
            {
                value = node as IDataStoreValue;
                if( value.NotNullReference() )
                {
                    writer.Write(value.Name, value, this.valueSerializer);
                }
                else
                {
                    writer.Write(node.Name, (IDataStoreObject)node, this);
                }
            }
        }

        #endregion

        #region IDataStoreObjectDeserializer

        /// <summary>
        /// Deserializes a data store object.
        /// </summary>
        /// <param name="reader">The data store reader to use.</param>
        /// <returns>The deserialized object.</returns>
        public IDataStoreObject Deserialize( IDataStoreReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine();

            var obj = new DataStoreObject(reader.Name);
            while( reader.Token != DataStoreToken.ObjectEnd )
            {
                if( reader.Token == DataStoreToken.BinaryValue
                 || reader.Token == DataStoreToken.TextValue )
                {
                    var value = reader.Read(this.valueSerializer);
                    obj.Nodes.Add(value);
                }
                else
                {
                    var o = reader.Read(this);
                    obj.Nodes.Add(o);
                }
            }

            return obj;
        }

        #endregion
    }
}
