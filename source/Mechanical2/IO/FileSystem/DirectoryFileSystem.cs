using System;
using System.Collections.Generic;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    //// NOTE: file names that are non data-store compatible
    ////       (possibly after escaping) are ignored.

    /// <summary>
    /// Wraps the data store compatible contents of a normal directory, as a data store file system.
    /// </summary>
    public class DirectoryFileSystem : IFileSystem
    {
        #region Private Fields

        private readonly string rootDirectoryFullPath;
        private readonly bool escapeFileNames;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileSystem"/> class
        /// </summary>
        /// <param name="directoryPath">The path specifying the contents at the root of this abstract file system. If the directory does not exist, it is created.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public DirectoryFileSystem( string directoryPath, bool escapeFileNames )
        {
            try
            {
                if( !Directory.Exists(directoryPath) )
                    Directory.CreateDirectory(directoryPath);

                this.rootDirectoryFullPath = Path.GetFullPath(directoryPath);
                this.escapeFileNames = escapeFileNames;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("directoryPath", directoryPath);
                ex.Store("escapeFileNames", escapeFileNames);
                throw;
            }
        }

        #endregion

        #region Private Methods

        private string ToRelativeHostPath( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty() )
                    return string.Empty;

                string hostPath;
                if( this.EscapesNames )
                {
                    hostPath = DataStore.UnescapePath(dataStorePath);
                    hostPath = hostPath.Replace(DataStore.PathSeparator, Path.DirectorySeparatorChar);
                }
                else
                    hostPath = dataStorePath.Replace(DataStore.PathSeparator, Path.DirectorySeparatorChar);

                return hostPath;
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                ex.Store("EscapesNames", this.EscapesNames);
                throw;
            }
        }

        private string ToFullHostPath( string dataStorePath )
        {
            var relativeHostPath = this.ToRelativeHostPath(dataStorePath);
            var fullHostPath = Path.Combine(this.rootDirectoryFullPath, relativeHostPath);
            return fullHostPath;
        }

        private string FromRelativeHostPath( string hostPath )
        {
            try
            {
                string name;
                string parentPath = Path.GetDirectoryName(hostPath);
                if( parentPath.NullOrEmpty() )
                {
                    parentPath = string.Empty;
                    name = hostPath;
                }
                else
                {
                    parentPath = this.FromRelativeHostPath(parentPath);
                    name = Path.GetFileName(hostPath);
                }

                if( this.EscapesNames )
                    name = DataStore.EscapeName(name);
                if( !DataStore.IsValidName(name) )
                    throw new ArgumentException("Invalid data store name!");

                return DataStore.Combine(parentPath, name);
            }
            catch( Exception ex )
            {
                ex.Store("hostPath", hostPath);
                ex.Store("EscapesNames", this.EscapesNames);
                throw;
            }
        }

        private string[] GetNames( string dataStorePath, Func<string, string[]> getFilesOrDirectories )
        {
            try
            {
                if( !dataStorePath.NullOrEmpty()
                 && !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                var filesOrDirectories = getFilesOrDirectories(fullHostPath);

                string relativeHostPath;
                var results = new List<string>(filesOrDirectories.Length);
                foreach( var f in filesOrDirectories )
                {
                    relativeHostPath = f.Substring(startIndex: fullHostPath.Length + 1);
                    try
                    {
                        dataStorePath = this.FromRelativeHostPath(relativeHostPath);
                        results.Add(dataStorePath);
                    }
                    catch
                    {
                    }
                }
                return results.ToArray();
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        private static void RemoveReadOnlyAttribute( string fileOrDirectory )
        {
            var attributes = File.GetAttributes(fileOrDirectory).Wrap();
            if( attributes.HasFlag(FileAttributes.ReadOnly) )
                attributes.RemoveFlag(FileAttributes.ReadOnly);
        }

        private static void RecursivelyDeleteExistingDirectory( string directoryPath )
        {
            foreach( var d in Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly) )
                RecursivelyDeleteExistingDirectory(d);

            foreach( var f in Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly) )
            {
                RemoveReadOnlyAttribute(f);
                File.Delete(f);
            }

            RemoveReadOnlyAttribute(directoryPath);
            Directory.Delete(directoryPath);
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
            get { return this.escapeFileNames; }
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostFilePath
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        public string ToHostFilePath( string dataStorePath )
        {
            return this.ToFullHostPath(dataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostDirectoryPath
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        public string ToHostDirectoryPath( string dataStorePath )
        {
            return this.ToFullHostPath(dataStorePath);
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
            return this.GetNames(dataStorePath, path => Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly));
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            return this.GetNames(dataStorePath, path => Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly));
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        public ITextReader ReadText( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                var fs = new FileStream(fullHostPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return IOWrapper.ToTextReader(fs);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        public IBinaryReader ReadBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                return IOWrapper.ToBinaryReader(new FileStream(fullHostPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the GetFileSize method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsGetFileSize
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        public long GetFileSize( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                return new FileInfo(fullHostPath).Length;
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        #endregion

        #region IFileSystemWriter

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

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                Directory.CreateDirectory(fullHostPath);
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

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                if( File.Exists(fullHostPath) )
                {
                    RemoveReadOnlyAttribute(fullHostPath);
                    File.Delete(fullHostPath);
                }
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

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                if( Directory.Exists(fullHostPath) )
                    RecursivelyDeleteExistingDirectory(fullHostPath);
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

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullHostPath));
                return IOWrapper.ToTextWriter(new FileStream(fullHostPath, FileMode.Create, FileAccess.Write, FileShare.Read), DataStore.DefaultEncoding, DataStore.DefaultNewLine);
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

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullHostPath));
                return IOWrapper.ToBinaryWriter(new FileStream(fullHostPath, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the CreateWriteThroughBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsCreateWriteThroughBinary
        {
            get
            {
#if !SILVERLIGHT
                return true;
#else
                return false;
#endif
            }
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
#if SILVERLIGHT
            throw new NotSupportedException().StoreFileLine();
#else
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullHostPath));
                return IOWrapper.ToBinaryWriter(new FileStream(fullHostPath, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize: 4096, options: FileOptions.WriteThrough)); // 4K is the default FileStream buffer size. May not be zero.
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
#endif
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsReadWriteBinary
        {
            get { return true; }
        }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream ReadWriteBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullHostPath(dataStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullHostPath));
                return IOWrapper.ToBinaryStream(new FileStream(fullHostPath, FileMode.OpenOrCreate, FileAccess.ReadWrite));
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
