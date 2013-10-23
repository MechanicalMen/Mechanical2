using System;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Adds seeking to data store reader.
    /// </summary>
    public interface ISeekableDataStoreReader
    {
        /// <summary>
        /// Positions the reader at the start of the data store.
        /// </summary>
        void SeekStart();

        /// <summary>
        /// Positions the reader at the end of the data store.
        /// </summary>
        void SeekEnd();

        /// <summary>
        /// Seeks the specified data store path.
        /// Positions the reader at the specified ObjectStart or value token if the path is found, or at DataStoreEnd otherwise.
        /// Throws an exception, if the path does not exist.
        /// </summary>
        /// <param name="absolutePath">The absolute data store path to seek.</param>
        void Seek( string absolutePath );

        /// <summary>
        /// Tries to seek the specified data store path.
        /// Positions the reader at the specified ObjectStart or value token if the path is found, or at DataStoreEnd otherwise.
        /// </summary>
        /// <param name="absolutePath">The absolute data store path to seek.</param>
        /// <returns><c>true</c> if the path was found; otherwise, <c>false</c>.</returns>
        bool TrySeek( string absolutePath );
    }
}
