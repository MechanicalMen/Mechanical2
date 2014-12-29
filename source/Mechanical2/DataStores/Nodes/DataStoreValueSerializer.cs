using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Nodes
{
    /// <summary>
    /// Serializes and deserializes <see cref="IDataStoreValue"/>.
    /// </summary>
    public class DataStoreValueSerializer : IDataStoreValueSerializer<IDataStoreValue>,
                                            IDataStoreValueDeserializer<IDataStoreValue>
    {
        #region Private Fields

        private readonly int maxLength = -1;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreValueSerializer"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum number of characters or bytes to read; or <c>-1</c> to read them all.</param>
        public DataStoreValueSerializer( int maxLength = -1 )
        {
            this.maxLength = maxLength;
        }

        #endregion

        #region IDataStoreValueSerializer

        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( IDataStoreValue obj, ITextWriter writer )
        {
            if( obj.NullReference() )
                throw new ArgumentNullException("obj").StoreFileLine();

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            var textValue = obj as IDataStoreTextValue;
            if( textValue.NotNullReference() )
                writer.Write(textValue.Content);
            else
                throw new InvalidOperationException("Binary values can not be serialized using text writers!").StoreFileLine();
        }

        /// <summary>
        /// Serializes to a binary-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( IDataStoreValue obj, IBinaryWriter writer )
        {
            if( obj.NullReference() )
                throw new ArgumentNullException("obj").StoreFileLine();

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            var binary = obj as IDataStoreBinaryValue;
            if( binary.NotNullReference() )
            {
                var bytes = binary.Content;
                writer.Write(bytes.Array, bytes.Offset, bytes.Count);
            }
            else
                throw new InvalidOperationException("Text values can not be serialized using binary writers!").StoreFileLine();
        }

        #endregion

        #region IDataStoreValueDeserializer

        /// <summary>
        /// Deserializes a text-based data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        public IDataStoreValue Deserialize( string name, ITextReader reader )
        {
            if( !DataStore.IsValidName(name) )
                throw new ArgumentException().Store("name", name);

            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine();

            Substring value;
            if( this.maxLength < 0 )
                reader.ReadToEnd(out value);
            else
                value = reader.Read(this.maxLength);

            return new DataStoreTextValue(name, value);
        }

        /// <summary>
        /// Deserializes a binary data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        public IDataStoreValue Deserialize( string name, IBinaryReader reader )
        {
            if( !DataStore.IsValidName(name) )
                throw new ArgumentException().Store("name", name);

            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine();

            ArraySegment<byte> bytes;
            if( this.maxLength < 0 )
                reader.ReadToEnd(out bytes);
            else
                reader.Read(this.maxLength, out bytes);

            return new DataStoreBinaryValue(name, bytes);
        }

        #endregion
    }
}
