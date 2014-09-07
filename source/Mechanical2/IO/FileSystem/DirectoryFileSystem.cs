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

        #region Internal Static Methods

        internal static string ToRelativeHostPath( string dataStorePath, bool escapeNames )
        {
            try
            {
                if( dataStorePath.NullOrEmpty() )
                    return string.Empty;

                string hostPath;
                if( escapeNames )
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
                ex.Store("escapeNames", escapeNames);
                throw;
            }
        }

        internal static string FromRelativeHostPath( string hostPath, bool escapeNames )
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
                    parentPath = FromRelativeHostPath(parentPath, escapeNames);
                    name = Path.GetFileName(hostPath);
                }

                if( escapeNames )
                    name = DataStore.EscapeName(name);
                if( !DataStore.IsValidName(name) )
                    throw new ArgumentException("Invalid data store name!");

                return DataStore.Combine(parentPath, name);
            }
            catch( Exception ex )
            {
                ex.Store("hostPath", hostPath);
                ex.Store("escapeNames", escapeNames);
                throw;
            }
        }

        #endregion

        #region Private Methods

        private string[] GetNames( string dataStorePath, Func<string, string[]> getFilesOrDirectories )
        {
            try
            {
                if( !dataStorePath.NullOrEmpty()
                 && !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var fullHostPath = this.ToFullFilePath(dataStorePath);
                var filesOrDirectories = getFilesOrDirectories(fullHostPath);

                string relativeHostPath;
                var results = new List<string>(filesOrDirectories.Length);
                foreach( var f in filesOrDirectories )
                {
                    relativeHostPath = f.Substring(startIndex: fullHostPath.Length + 1);
                    try
                    {
                        dataStorePath = FromRelativeHostPath(relativeHostPath, this.EscapesNames);
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

        #region IFileSystemReader

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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
                var fs = File.OpenRead(fullHostPath);
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
                return IOWrapper.ToBinaryReader(File.OpenRead(fullHostPath));
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
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
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        bool IFileSystemWriter.EscapesNames
        {
            get { return this.escapeFileNames; }
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullHostPath));
                return IOWrapper.ToBinaryWriter(new FileStream(fullHostPath, FileMode.Create, FileAccess.Write, FileShare.Read));
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
        /// Opens an existing file, or creates a new one, for both reading and writing.
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

                var fullHostPath = this.ToFullFilePath(dataStorePath);
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

        #region Public Methods

        /// <summary>
        /// Converts a dataStorePath of the abstract file system,
        /// to a full file path of the operating system.
        /// If at all possible, use streams (see <see cref="IOWrapper"/>) instead of this method.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file or directory.</param>
        /// <returns>The full path to the file or directory.</returns>
        public string ToFullFilePath( string dataStorePath )
        {
            var relativeHostPath = ToRelativeHostPath(dataStorePath, this.EscapesNames);
            var fullHostPath = Path.Combine(this.rootDirectoryFullPath, relativeHostPath);
            return fullHostPath;
        }

        #endregion
    }
}
