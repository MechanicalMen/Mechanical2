using System;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents an abstract file system, both readable and writable.
    /// </summary>
    public interface IFileSystem : IFileSystemReader, IFileSystemWriter
    {
        /// <summary>
        /// Opens an existing file, or creates a new one, for reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        IBinaryStream OpenBinary( string dataStorePath );
    }
}
