using System;
using Mechanical.Conditions;

namespace Mechanical.IO.FileSystem
{
    /* NOTE: Consider that the library has to work on platforms, with different:
     *         - case sensitiveness
     *         - invalid characters
     *         - maximum file name length
     *         - maximum path length
     *         - ... etc.
     * 
     *       The goal was not to find the common denominator (google for POSIX and file names about that one),
     *       rather to find a small, "reasonably" portable and reliable feature set, that could be
     *       made to fit well into the data store toolset (through a wrapper).
     * 
     *       The main way this is achieved is, that all files (and directories) are accessed through
     *       unique data store paths. The mapping from host path to data store path is left to the
     *       implementation. Consuming code should never have to worry about host paths
     *       (or what kind of platform they originated on). This way, a "recent file list" saved on windows,
     *       can be opened in linux without having to write a single extra line.
     * 
     *       Translating between paths is done consistently, in one of two ways:
     *         A) Only those original file names are preserved, which are data store compatible.
     *            The main benefit is, that file paths gain "cross-platform uniqueness",
     *            while the main drawback is a lack of file extensions.
     *            (This is recommended for resources created and edited programmatically.)
     * 
     *         B) All original file names can also be automatically escaped (using the usual data store method).
     *            This way, they preserve their names, but data store paths, used in code or in resources,
     *            may become somewhat unwieldy.
     *            (This is recommended for files named by the user, or files that should be
     *            easily identifiable with a non-data store aware file manager)
     * 
     *       As long as your needs are fairly basic, this will work out of the box,
     *       between multiple platforms, and with minimal effort.
     */

    /// <summary>
    /// Represents an abstract file system, that's both readable and writable.
    /// </summary>
    public interface IFileSystem : IFileSystemReader, IFileSystemWriter
    {
        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        bool SupportsReadWriteBinary { get; }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        IBinaryStream ReadWriteBinary( string dataStorePath );
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
