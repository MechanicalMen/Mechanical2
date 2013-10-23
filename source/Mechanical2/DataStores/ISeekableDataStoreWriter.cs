using System;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Adds seeking to data store writer.
    /// </summary>
    public interface ISeekableDataStoreWriter
    {
        /// <summary>
        /// Seeks the specified data store path.
        /// Creates parent data store objects, if necessary.
        /// </summary>
        /// <param name="absoluteParentPath">The absolute data store path of the parent node, of what is to be written next.</param>
        /// <param name="insertBefore">The name of the next sibling to insert the node before; or <c>null</c>, to insert after the last child of the parent.</param>
        void SeekInsert( string absoluteParentPath, string insertBefore );

        /// <summary>
        /// Removes the specified data store node.
        /// </summary>
        /// <param name="absolutePath">The absolute data store path to seek.</param>
        /// <returns><c>true</c> if the data store node was found; otherwise, <c>false</c>.</returns>
        bool Remove( string absolutePath );
    }
}
