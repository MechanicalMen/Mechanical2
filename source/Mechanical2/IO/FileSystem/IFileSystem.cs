using System;
using Mechanical.Conditions;

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

    /// <content>
    /// Methods extending the <see cref="IFileSystem"/> type.
    /// </content>
    public static partial class FileSystemExtensions
    {
        #region AppendAllText

        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file. If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
        /// </summary>
        /// <param name="fileSystem">The file system to use.</param>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="contents">The string to append to the file.</param>
        public static void AppendAllText( this IFileSystem fileSystem, string dataStorePath, string contents )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            if( fileSystem.FileExists(dataStorePath) )
                contents = fileSystem.ReadAllText(dataStorePath) + contents;

            fileSystem.WriteAllText(dataStorePath, contents);
        }

        #endregion
    }
}
