using System;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Serializes objects of type <typeparamref name="T"/> to data store objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to serialize.</typeparam>
    public interface IDataStoreObjectSerializer<T>
    {
        /// <summary>
        /// Serializes to a data store object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The data store writer to use.</param>
        void Serialize( T obj, IDataStoreWriter writer );
    }
}
