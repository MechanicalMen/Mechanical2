using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents a zip file as an abstract file system.
    /// </summary>
    public class ZipFileSystem : IFileSystem
    {
        #region Non-Public Fields

        internal const char ZipDirectorySeparator = '/';
        internal static readonly Encoding ZipEntryNameEncoding = Encoding.UTF8;
        internal static readonly CompressionLevel CompressionLevel = CompressionLevel.Optimal;

        private ZipArchive zipArchive;
        private ZipFileSystemReader reader;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystem"/> class
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that contains the archive.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystem( Stream stream, bool escapeFileNames )
        {
            try
            {
                this.zipArchive = new ZipArchive(
                    stream, 
                    ZipArchiveMode.Update,
                    leaveOpen: false,
                    entryNameEncoding: ZipEntryNameEncoding);

                this.reader = new ZipFileSystemReader(this.zipArchive, escapeFileNames);
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("escapeFileNames", escapeFileNames);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystem"/> class
        /// </summary>
        /// <param name="stream">The <see cref="IBinaryStream"/> that contains the archive.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystem( IBinaryStream stream, bool escapeFileNames )
            : this(IOWrapper.Wrap(stream), escapeFileNames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystem"/> class
        /// </summary>
        /// <param name="zipFilePath">The path of the .zip file to open.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystem( string zipFilePath, bool escapeFileNames )
            : this(new FileStream(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite), escapeFileNames)
        {
        }

        #endregion

        #region Internal Static Methods

        internal static string ToZipPath( string dataStorePath, bool escapeNames, bool isDirectory )
        {
            try
            {
                if( dataStorePath.NullOrEmpty() )
                    return string.Empty;

                string zipPath;
                if( escapeNames )
                {
                    zipPath = DataStore.UnescapePath(dataStorePath);
                    zipPath = zipPath.Replace(DataStore.PathSeparator, ZipDirectorySeparator);
                }
                else
                    zipPath = dataStorePath.Replace(DataStore.PathSeparator, ZipDirectorySeparator);

                if( isDirectory )
                    return zipPath + ZipDirectorySeparator;
                else
                    return zipPath;
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                ex.Store("escapeNames", escapeNames);
                throw;
            }
        }

        internal static string FromZipPath( string zipPath, bool escapeNames )
        {
            try
            {
                if( zipPath[zipPath.Length - 1] == ZipDirectorySeparator )
                    zipPath = zipPath.Substring(startIndex: 0, length: zipPath.Length - 1);

                string dataStorePath;
                if( escapeNames )
                {
                    dataStorePath = zipPath.Replace(ZipDirectorySeparator, DataStore.PathSeparator);
                    dataStorePath = DataStore.EscapePath(dataStorePath);
                }
                else
                    dataStorePath = zipPath.Replace(ZipDirectorySeparator, DataStore.PathSeparator);

                return dataStorePath;
            }
            catch( Exception ex )
            {
                ex.Store("zipPath", zipPath);
                ex.Store("escapeNames", escapeNames);
                throw;
            }
        }

        #endregion

        #region Private Methods

        private ZipArchiveEntry CreateDirectoryRecursively( string dataStorePath )
        {
            // check whether the directory already exists
            var zipDirectoryPath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: true);
            var zipEntry = this.zipArchive.GetEntry(zipDirectoryPath);
            if( zipEntry.NullReference() )
            {
                // make sure parent directory exists
                var parentDataStorePath = DataStore.GetParentPath(dataStorePath);
                if( !parentDataStorePath.NullOrEmpty() )
                    this.CreateDirectoryRecursively(parentDataStorePath);

                // make sure path specified is not a file
                var zipFilePath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: false);
                zipEntry = this.zipArchive.GetEntry(zipFilePath);
                if( zipEntry.NotNullReference() )
                    throw new FileNotFoundException("File system entry already exists, but isn't a directory!").Store("filePath", dataStorePath);

                // create directory
                zipEntry = this.zipArchive.CreateEntry(zipDirectoryPath, CompressionLevel);
            }

            return zipEntry;
        }

        private void DeleteDirectoryRecursively( string dataStorePath )
        {
            foreach( var d in this.GetDirectoryNames(dataStorePath) )
                this.DeleteDirectoryRecursively(d);

            foreach( var f in this.GetFileNames(dataStorePath) )
                this.DeleteFile(f);

            var zipDirectoryPath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: true);
            var zipEntry = this.zipArchive.GetEntry(zipDirectoryPath);
            if( zipEntry.NotNullReference() )
                zipEntry.Delete();
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
            get { return this.reader.EscapesNames; }
        }

        /// <summary>
        /// Gets the names of the files found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files.</param>
        /// <returns>The names of the files found.</returns>
        public string[] GetFileNames( string dataStorePath = "" )
        {
            return this.reader.GetFileNames(dataStorePath);
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            return this.reader.GetDirectoryNames(dataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        public ITextReader ReadText( string dataStorePath )
        {
            return this.reader.ReadText(dataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        public IBinaryReader ReadBinary( string dataStorePath )
        {
            return this.reader.ReadBinary(dataStorePath);
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        public long GetFileSize( string dataStorePath )
        {
            return this.reader.GetFileSize(dataStorePath);
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
            get { return this.reader.EscapesNames; }
        }

        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        public void CreateDirectory( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                this.CreateDirectoryRecursively(dataStorePath);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified file. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to delete.</param>
        public void DeleteFile( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var zipFilePath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipFilePath);
                if( zipEntry.NotNullReference() )
                    zipEntry.Delete();
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified directory. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to delete.</param>
        public void DeleteDirectory( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                this.DeleteDirectoryRecursively(dataStorePath);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        public ITextWriter CreateNewText( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                // delete existing entry
                var zipFilePath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipFilePath);
                if( zipEntry.NotNullReference() )
                    zipEntry.Delete();

                // create directory
                this.CreateDirectoryRecursively(DataStore.GetParentPath(dataStorePath));

                // create new entry
                zipEntry = this.zipArchive.CreateEntry(zipFilePath, CompressionLevel);
                return IOWrapper.ToTextWriter(zipEntry.Open(), DataStore.DefaultEncoding, DataStore.DefaultNewLine);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateNewBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                // delete existing entry
                var zipFilePath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipFilePath);
                if( zipEntry.NotNullReference() )
                    zipEntry.Delete();

                // create directory
                this.CreateDirectoryRecursively(DataStore.GetParentPath(dataStorePath));

                // create new entry
                zipEntry = this.zipArchive.CreateEntry(zipFilePath, CompressionLevel);
                return IOWrapper.ToBinaryWriter(zipEntry.Open());
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Opens an existing file, or creates a new one, for reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream OpenBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                // try to open existing entry
                var zipFilePath = ToZipPath(dataStorePath, this.EscapesNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipFilePath);
                if( zipEntry.NullReference() )
                {
                    // create directory
                    this.CreateDirectoryRecursively(DataStore.GetParentPath(dataStorePath));

                    // create new entry
                    zipEntry = this.zipArchive.CreateEntry(zipFilePath, CompressionLevel);
                }

                return IOWrapper.ToBinaryStream(zipEntry.Open());
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        #endregion
    }
}
