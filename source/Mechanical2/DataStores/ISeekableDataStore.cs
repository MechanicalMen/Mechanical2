using System;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Adds data store node seeking to a data store reader or writer.
    /// </summary>
    public interface ISeekableDataStore
    {
        /// <summary>
        /// Seeks the start of the data store reader or writer.
        /// </summary>
        void SeekStart();

        /// <summary>
        /// Seeks the specified data store path.
        /// Data store readers position themselves at the specified node, so no further reading is necessary for deserialization.
        /// Data store writers can behave in one of two ways: if the name from the next Write call is the same as the one specified,
        /// the data store node gets overwritten, otherwise it gets a new sibling inserted right after it.
        /// Both readers and writers throw an exception, if the path does not exist.
        /// </summary>
        /// <param name="absolutePath">The absolute data store path to seek.</param>
        void Seek( string absolutePath );
    }
}
