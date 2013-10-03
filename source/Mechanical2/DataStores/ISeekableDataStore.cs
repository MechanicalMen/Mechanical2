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
        /// Data store readers position themselves before the specified node, so that the next Read call can process it; or the data store end, if the path is not found.
        /// Data store writers position themselves after the specified node, so that the contents of the next Write call are placed after it; or throw an exception, if the path is not found.
        /// </summary>
        /// <param name="path">The data store path to seek.</param>
        void Seek( string path );

        // all paths absolute?!
        // seeking into the middle of an object?!

        // files.csv:
        //  - rawName
        //  - alias
        //  - isBinary
    }
}
