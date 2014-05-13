using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Wraps the contents of a directory, as an <see cref="IFileSystemReaderWriter"/>.
    /// </summary>
    public class SubdirectoryFileSystemReaderWriter : IFileSystemReaderWriter
    {
        #region Private Fields

        private readonly IFileSystemReaderWriter parentFileSystem;

        /// <summary>
        /// Represents the data store compatible path to the subdirectory.
        /// </summary>
        protected readonly string SubDirPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdirectoryFileSystemReaderWriter"/> class
        /// </summary>
        /// <param name="parentFileSystem">The file system to represent a subdirectory of.</param>
        /// <param name="dataStorePath">The (data store) path of the directory to represent.</param>
        public SubdirectoryFileSystemReaderWriter( IFileSystemReaderWriter parentFileSystem, string dataStorePath )
        {
            try
            {
                if( parentFileSystem.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                if( !DataStore.IsValidPath(dataStorePath) ) // empty string is acceptable!
                    throw new ArgumentException().StoreFileLine();

                this.parentFileSystem = parentFileSystem;
                this.SubDirPath = dataStorePath;
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

        #region IFileSystemReader

        /// <summary>
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        public bool EscapesNames
        {
            get { return ((IFileSystemReader)this.parentFileSystem).EscapesNames; }
        }

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

            string path;
            if( dataStorePath.Length == 0 )
                path = this.SubDirPath;
            else
                path = DataStore.Combine(this.SubDirPath, dataStorePath);

            return this.parentFileSystem.GetFileNames(path);
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

            string path;
            if( dataStorePath.Length == 0 )
                path = this.SubDirPath;
            else
                path = DataStore.Combine(this.SubDirPath, dataStorePath);

            return this.parentFileSystem.GetDirectoryNames(path);
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

            return this.parentFileSystem.ReadText(DataStore.Combine(this.SubDirPath, dataStorePath));
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

            return this.parentFileSystem.ReadBinary(DataStore.Combine(this.SubDirPath, dataStorePath));
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

            return this.parentFileSystem.GetFileSize(DataStore.Combine(this.SubDirPath, dataStorePath));
        }

        #endregion

        #region IFileSystemWriter

        /// <summary>
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        bool IFileSystemWriter.EscapesNames
        {
            get { return ((IFileSystemWriter)this.parentFileSystem).EscapesNames; }
        }

        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        public void CreateDirectory( string dataStorePath )
        {
            if( !DataStore.IsValidPath(dataStorePath)
                || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            this.parentFileSystem.CreateDirectory(DataStore.Combine(this.SubDirPath, dataStorePath));
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

            this.parentFileSystem.DeleteFile(DataStore.Combine(this.SubDirPath, dataStorePath));
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

            this.parentFileSystem.DeleteDirectory(DataStore.Combine(this.SubDirPath, dataStorePath));
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

            return this.parentFileSystem.CreateNewText(DataStore.Combine(this.SubDirPath, dataStorePath));
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

            return this.parentFileSystem.CreateNewBinary(DataStore.Combine(this.SubDirPath, dataStorePath));
        }

        #endregion
    }
}
