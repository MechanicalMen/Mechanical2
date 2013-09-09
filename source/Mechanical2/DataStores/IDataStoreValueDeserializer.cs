using System;
using Mechanical.IO;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Deserializes objects of type <typeparamref name="T"/> from data store values.
    /// </summary>
    /// <typeparam name="T">The type of objects to deserialize.</typeparam>
    public interface IDataStoreValueDeserializer<T>
    {
        /// <summary>
        /// Deserializes a text-based data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize( string name, ITextReader reader );

        /// <summary>
        /// Deserializes a binary data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize( string name, IBinaryReader reader );
    }
}
