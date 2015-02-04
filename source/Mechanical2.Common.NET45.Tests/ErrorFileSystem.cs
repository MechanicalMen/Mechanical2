using System;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.IO;
using Mechanical.IO.FileSystem;

namespace Mechanical.Common.Tests
{
    /// <summary>
    /// Thinly wraps an <see cref="IFileSystem"/>, though some members may instead throw exceptions.
    /// </summary>
    public class ErrorFileSystem : IFileSystem
    {
        #region Private Fields

        private readonly IFileSystem fileSystem;
        private readonly bool throwOnRead;
        private readonly bool throwOnWrite;
        private readonly bool throwOnList;

        #endregion

        #region Constructors

        public ErrorFileSystem( IFileSystem fileSystem, bool throwOnRead, bool throwOnWrite, bool throwOnList )
        {
            Ensure.That("fileSystem", fileSystem).NotNull();

            this.fileSystem = fileSystem;
            this.throwOnRead = throwOnRead;
            this.throwOnWrite = throwOnWrite;
            this.throwOnList = throwOnList;
        }

        #endregion

        #region Private Methods

        private void Throw(
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            throw new System.IO.IOException("Operation not supported").StoreFileLine(filePath, memberName, lineNumber);
        }

        #endregion

        #region IFileSystemBase

        /// <summary>
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        public bool EscapesNames
        {
            get { return this.fileSystem.EscapesNames; }
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostFilePath
        {
            get { return this.fileSystem.SupportsToHostFilePath; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        public string ToHostFilePath( string dataStorePath )
        {
            return this.fileSystem.ToHostFilePath(dataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostDirectoryPath
        {
            get { return this.fileSystem.SupportsToHostDirectoryPath; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        public string ToHostDirectoryPath( string dataStorePath )
        {
            return this.fileSystem.ToHostDirectoryPath(dataStorePath);
        }

        #endregion

        #region IFileSystemReader

        /// <summary>
        /// Gets the names of the files found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files.</param>
        /// <returns>The names of the files found.</returns>
        public string[] GetFileNames( string dataStorePath = "" )
        {
            if( this.throwOnList )
                this.Throw();

            return this.fileSystem.GetFileNames(dataStorePath);
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            if( this.throwOnList )
                this.Throw();

            return this.fileSystem.GetDirectoryNames(dataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        public ITextReader ReadText( string dataStorePath )
        {
            if( this.throwOnRead )
                this.Throw();

            return this.fileSystem.ReadText(dataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        public IBinaryReader ReadBinary( string dataStorePath )
        {
            if( this.throwOnRead )
                this.Throw();

            return this.fileSystem.ReadBinary(dataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the GetFileSize method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsGetFileSize
        {
            get { return this.fileSystem.SupportsGetFileSize; }
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        public long GetFileSize( string dataStorePath )
        {
            if( this.throwOnRead )
                this.Throw();

            return this.fileSystem.GetFileSize(dataStorePath);
        }

        #endregion

        #region IFileSystemWriter

        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        public void CreateDirectory( string dataStorePath )
        {
            if( this.throwOnWrite )
                this.Throw();

            this.fileSystem.CreateDirectory(dataStorePath);
        }

        /// <summary>
        /// Deletes the specified file. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to delete.</param>
        public void DeleteFile( string dataStorePath )
        {
            if( this.throwOnWrite )
                this.Throw();

            this.fileSystem.DeleteFile(dataStorePath);
        }

        /// <summary>
        /// Deletes the specified directory. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to delete.</param>
        public void DeleteDirectory( string dataStorePath )
        {
            if( this.throwOnWrite )
                this.Throw();

            this.fileSystem.DeleteDirectory(dataStorePath);
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        public ITextWriter CreateNewText( string dataStorePath, bool overwriteIfExists )
        {
            if( this.throwOnWrite )
                this.Throw();

            return this.fileSystem.CreateNewText(dataStorePath, overwriteIfExists);
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateNewBinary( string dataStorePath, bool overwriteIfExists )
        {
            if( this.throwOnWrite )
                this.Throw();

            return this.fileSystem.CreateNewBinary(dataStorePath, overwriteIfExists);
        }


        /// <summary>
        /// Gets a value indicating whether the CreateWriteThroughBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsCreateWriteThroughBinary
        {
            get { return this.SupportsCreateWriteThroughBinary; }
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// No intermediate buffers are kept: all operations access the file directly.
        /// This hurts performance, but is important for log files (less is lost in case of a crash).
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateWriteThroughBinary( string dataStorePath, bool overwriteIfExists )
        {
            if( this.throwOnWrite )
                this.Throw();

            return this.fileSystem.CreateWriteThroughBinary(dataStorePath, overwriteIfExists);
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsReadWriteBinary
        {
            get { return this.fileSystem.SupportsReadWriteBinary; }
        }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream ReadWriteBinary( string dataStorePath )
        {
            if( this.throwOnWrite )
                this.Throw();

            return this.fileSystem.ReadWriteBinary(dataStorePath);
        }

        #endregion
    }
}
