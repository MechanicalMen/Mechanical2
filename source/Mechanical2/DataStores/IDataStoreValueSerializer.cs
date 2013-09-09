using System;
using Mechanical.IO;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Serializes objects of type <typeparamref name="T"/> to data store values.
    /// </summary>
    /// <typeparam name="T">The type of objects to serialize.</typeparam>
    public interface IDataStoreValueSerializer<T>
    {
        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        void Serialize( T obj, ITextWriter writer );

        /// <summary>
        /// Serializes to a binary-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        void Serialize( T obj, IBinaryWriter writer );
    }
}
