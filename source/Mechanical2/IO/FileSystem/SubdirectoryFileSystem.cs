using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Wraps the contents of a subdirectory, as an <see cref="IFileSystem"/>.
    /// </summary>
    public class SubdirectoryFileSystem : IFileSystem
    {
        #region Private Fields

        private readonly IFileSystem parentFileSystem;
        private readonly string subDirPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdirectoryFileSystem"/> class
        /// </summary>
        /// <param name="parentFileSystem">The file system to represent a subdirectory of.</param>
        /// <param name="dataStorePath">The (data store) path of the directory to represent.</param>
        public SubdirectoryFileSystem( IFileSystem parentFileSystem, string dataStorePath )
        {
            try
            {
                if( parentFileSystem.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                if( !DataStore.IsValidPath(dataStorePath) ) // empty string is acceptable!
                    throw new ArgumentException().StoreFileLine();

                this.parentFileSystem = parentFileSystem;
                this.subDirPath = dataStorePath;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("parentFileSystemType", parentFileSystem.NullReference() ? null : parentFileSystem.GetType());
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        #endregion

        #region Private Methods

        private string ToParentDataStorePath( string dataStorePath )
        {
            if( dataStorePath.Length == 0 )
                return this.subDirPath;
            else
                return DataStore.Combine(this.subDirPath, dataStorePath);
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
            get { return this.parentFileSystem.EscapesNames; }
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostFilePath
        {
            get { return this.parentFileSystem.SupportsToHostFilePath; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        public string ToHostFilePath( string dataStorePath )
        {
            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.ToHostFilePath(parentDataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostDirectoryPath
        {
            get { return this.parentFileSystem.SupportsToHostDirectoryPath; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        public string ToHostDirectoryPath( string dataStorePath )
        {
            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.ToHostDirectoryPath(parentDataStorePath);
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
            if( !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.GetFileNames(parentDataStorePath);
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            if( !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.GetDirectoryNames(parentDataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        public ITextReader ReadText( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.ReadText(parentDataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        public IBinaryReader ReadBinary( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.ReadBinary(parentDataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the GetFileSize method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsGetFileSize
        {
            get { return this.parentFileSystem.SupportsGetFileSize; }
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        public long GetFileSize( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.GetFileSize(parentDataStorePath);
        }

        #endregion

        #region IFileSystemWriter

        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        public void CreateDirectory( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            this.parentFileSystem.CreateDirectory(parentDataStorePath);
        }

        /// <summary>
        /// Deletes the specified file. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to delete.</param>
        public void DeleteFile( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            this.parentFileSystem.DeleteFile(parentDataStorePath);
        }

        /// <summary>
        /// Deletes the specified directory. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to delete.</param>
        public void DeleteDirectory( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            this.parentFileSystem.DeleteDirectory(parentDataStorePath);
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        public ITextWriter CreateNewText( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.CreateNewText(parentDataStorePath);
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateNewBinary( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.CreateNewBinary(parentDataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the CreateWriteThroughBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsCreateWriteThroughBinary
        {
            get { return this.parentFileSystem.SupportsCreateWriteThroughBinary; }
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// No intermediate buffers are kept: all operations access the file directly.
        /// This hurts performance, but is important for log files (less is lost in case of a crash).
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateWriteThroughBinary( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.CreateWriteThroughBinary(parentDataStorePath);
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsReadWriteBinary
        {
            get { return this.parentFileSystem.SupportsReadWriteBinary; }
        }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream ReadWriteBinary( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentDataStorePath = this.ToParentDataStorePath(dataStorePath);
            return this.parentFileSystem.ReadWriteBinary(parentDataStorePath);
        }

        #endregion
    }
}
