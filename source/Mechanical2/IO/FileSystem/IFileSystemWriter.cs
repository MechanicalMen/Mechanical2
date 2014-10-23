using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents an abstract, writable file system.
    /// </summary>
    public interface IFileSystemWriter : IFileSystemBase
    {
        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        void CreateDirectory( string dataStorePath );

        /// <summary>
        /// Deletes the specified file. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to delete.</param>
        void DeleteFile( string dataStorePath );

        /// <summary>
        /// Deletes the specified directory. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to delete.</param>
        void DeleteDirectory( string dataStorePath );

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        ITextWriter CreateNewText( string dataStorePath, bool overwriteIfExists );

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        IBinaryWriter CreateNewBinary( string dataStorePath, bool overwriteIfExists );


        /// <summary>
        /// Gets a value indicating whether the CreateWriteThroughBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        bool SupportsCreateWriteThroughBinary { get; }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// No intermediate buffers are kept: all operations access the file directly.
        /// This hurts performance, but is important for log files (less is lost in case of a crash).
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        IBinaryWriter CreateWriteThroughBinary( string dataStorePath, bool overwriteIfExists );
    }

    /// <content>
    /// Methods extending the <see cref="IFileSystemWriter"/> type.
    /// </content>
    public static partial class FileSystemExtensions
    {
        #region WriteAllText

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="fileSystem">The file system to use.</param>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="contents">The string to write to the file.</param>
        public static void WriteAllText( this IFileSystemWriter fileSystem, string dataStorePath, string contents )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            var writer = fileSystem.CreateNewText(dataStorePath, overwriteIfExists: true);
            if( contents.NotNullReference() ) // same 'null' handling as the standard .NET implementation
                writer.Write(contents);
            writer.Close();
        }

        #endregion
    }
}
