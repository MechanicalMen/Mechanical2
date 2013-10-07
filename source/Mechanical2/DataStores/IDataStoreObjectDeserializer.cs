using System;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Deserializes objects of type <typeparamref name="T"/> from data store objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to deserialize.</typeparam>
    public interface IDataStoreObjectDeserializer<T>
    {
        /// <summary>
        /// Deserializes a data store object.
        /// </summary>
        /// <param name="reader">The data store reader to use.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize( IDataStoreReader reader );
    }
}
