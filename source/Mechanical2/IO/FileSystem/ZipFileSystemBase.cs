using System;
using System.Collections.Generic;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Helps implementing a .zip file based abstract file system.
    /// </summary>
    public abstract class ZipFileSystemBase : DisposableObject, IFileSystem
    {
        #region EchoStream

        private class EchoStream : Stream
        {
            //// NOTE: runs delegate on dispose. Does not automatically dispose wrapped stream.

            private readonly Stream wrappedStream;
            private Action onDispose;

            internal EchoStream( Stream stream, Action onDispose )
                : base()
            {
                Ensure.Debug(stream, s => s.NotNull());

                this.wrappedStream = stream;
                this.onDispose = onDispose;
            }

            protected override void Dispose( bool disposing )
            {
                try
                {
                    if( this.onDispose.NotNullReference() )
                        this.onDispose();
                }
                finally
                {
                    this.onDispose = null;
                    base.Dispose(disposing);
                }
            }

            public override void Flush()
            {
                this.wrappedStream.Flush();
            }

            public override int Read( byte[] buffer, int offset, int count )
            {
                return this.wrappedStream.Read(buffer, offset, count);
            }

            public override long Seek( long offset, SeekOrigin origin )
            {
                return this.wrappedStream.Seek(offset, origin);
            }

            public override void SetLength( long value )
            {
                this.wrappedStream.SetLength(value);
            }

            public override void Write( byte[] buffer, int offset, int count )
            {
                this.wrappedStream.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return this.wrappedStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return this.wrappedStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return this.wrappedStream.CanWrite; }
            }

            public override long Length
            {
                get { return this.wrappedStream.Length; }
            }

            public override long Position
            {
                get { return this.wrappedStream.Position; }
                set { this.wrappedStream.Position = value; }
            }
        }

        #endregion

        #region Private Fields

        private const char ZipDirectorySeparator = '/';

        private readonly object syncLock;
        private readonly bool escapeFileNames;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystemBase"/> class.
        /// </summary>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        protected ZipFileSystemBase( bool escapeFileNames )
        {
            this.syncLock = new object();
            this.escapeFileNames = escapeFileNames;
        }

        #endregion

        #region Private Methods

        private string ToZipPath( string dataStorePath, bool isDirectory )
        {
            try
            {
                if( dataStorePath.NullOrEmpty() )
                    return string.Empty;

                string zipPath;
                if( this.EscapesNames )
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
                ex.StoreFileLine();
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        private string FromZipPath( string zipPath )
        {
            try
            {
                if( zipPath[zipPath.Length - 1] == ZipDirectorySeparator )
                    zipPath = zipPath.Substring(startIndex: 0, length: zipPath.Length - 1);

                string dataStorePath;
                if( this.EscapesNames )
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
                ex.StoreFileLine();
                ex.Store("zipPath", zipPath);
                throw;
            }
        }

        private string[] GetNames( string dataStorePath, bool getFiles )
        {
            try
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                var zipPath = this.ToZipPath(dataStorePath, isDirectory: true);
                var results = new List<string>();

                bool isDirectory;
                string relativeZipFilePath;
                foreach( var zipEntryPath in this.GetZipEntryFilePaths() )
                {
                    if( zipEntryPath.StartsWith(zipPath) )
                    {
                        if( string.Equals(zipEntryPath, zipPath, StringComparison.Ordinal) )
                            continue;

                        isDirectory = zipEntryPath[zipEntryPath.Length - 1] == ZipDirectorySeparator;
                        if( (getFiles && !isDirectory)
                         || (!getFiles && isDirectory) )
                        {
                            relativeZipFilePath = zipEntryPath.Substring(startIndex: zipPath.Length, length: zipEntryPath.Length - zipPath.Length - (isDirectory ? 1 : 0));
                            if( relativeZipFilePath.IndexOf(ZipDirectorySeparator) == -1 )
                                results.Add(this.FromZipPath(relativeZipFilePath));
                        }
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

        private void CreateDirectoryRecursively( string dataStorePath )
        {
            // check whether the directory already exists
            var zipDirectoryPath = this.ToZipPath(dataStorePath, isDirectory: true);
            if( !this.ContainsEntry(zipDirectoryPath) )
            {
                // make sure parent directory exists
                var parentDataStorePath = DataStore.GetParentPath(dataStorePath);
                if( !parentDataStorePath.NullOrEmpty() )
                    this.CreateDirectoryRecursively(parentDataStorePath);

                // make sure path specified is not a file
                var zipFilePath = this.ToZipPath(dataStorePath, isDirectory: false);
                if( this.ContainsEntry(zipFilePath) )
                    throw new FileNotFoundException("File system entry already exists, but isn't a directory!").Store("filePath", dataStorePath);

                // create directory
                this.CreateDirectoryEntry(zipDirectoryPath);
            }
        }

        private void DeleteDirectoryRecursively( string dataStorePath )
        {
            // remove subdirectories
            foreach( var d in this.GetDirectoryNames(dataStorePath) )
                this.DeleteDirectoryRecursively(d);

            // remove files in current directory
            string zipPath;
            foreach( var f in this.GetFileNames(dataStorePath) )
            {
                zipPath = this.ToZipPath(f, isDirectory: false);
                this.RemoveEntry(zipPath);
            }

            // remove current directory
            zipPath = this.ToZipPath(dataStorePath, isDirectory: true);
            this.RemoveEntry(zipPath);
        }

        private Stream FlushAfterDispose( Stream stream )
        {
            return this.WrapDispose(
                stream,
                onDispose: () =>
                {
                    stream.Dispose(); // finish reading or writing the entry first
                    this.Flush();
                });
        }

        #endregion

        #region Protected Abstract Members

        /// <summary>
        /// Returns the standard zip paths of the entries.
        /// </summary>
        /// <returns>The file paths of the zip entries found.</returns>
        protected abstract string[] GetZipEntryFilePaths();

        /// <summary>
        /// Determines whether a zip entry with the specified zip path exists.
        /// </summary>
        /// <param name="zipPath">The zip path to search for.</param>
        /// <returns><c>true</c> if the zip path is in use; otherwise, <c>false</c>.</returns>
        protected abstract bool ContainsEntry( string zipPath );

        /// <summary>
        /// Opens an entry for reading.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        /// <returns>The stream to read the uncompressed contents of the zip entry from.</returns>
        protected abstract Stream OpenRead( string zipPath );

        /// <summary>
        /// Gets the uncompressed size of the zip entry.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        /// <returns>The uncompressed size of the zip entry; or <c>null</c>.</returns>
        protected abstract long? GetUncompressedSize( string zipPath );

        /// <summary>
        /// Creates a new zip entry representing a directory.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        protected abstract void CreateDirectoryEntry( string zipPath );

        /// <summary>
        /// Removes the specified zip entry.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        protected abstract void RemoveEntry( string zipPath );

        /// <summary>
        /// Creates a new zip entry representing a file.
        /// </summary>
        /// <param name="zipPath">The zip path identifying the entry.</param>
        /// <returns>The <see cref="Stream"/> to which to write the contents of the entry to.</returns>
        protected abstract Stream CreateFileEntry( string zipPath );

        /// <summary>
        /// Saves the current state of the zip file.
        /// </summary>
        protected abstract void Flush();

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>.
        /// The dispose action can be specified manually.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap</param>
        /// <param name="onDispose">The code to run when the wrapper is disposed; or <c>null</c> to do nothing (and leave the <paramref name="stream"/> parameter undisposed).</param>
        /// <returns>The wrapped <see cref="Stream"/>.</returns>
        protected Stream WrapDispose( Stream stream, Action onDispose )
        {
            return new EchoStream(stream, onDispose);
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
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.escapeFileNames;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostFilePath
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return true;
            }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        public string ToHostFilePath( string dataStorePath )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(null).StoreFileLine();

            if( dataStorePath.NullOrEmpty()
             || !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException("Invalid data store path!").StoreFileLine();

            lock( this.syncLock )
                return this.ToZipPath(dataStorePath, isDirectory: false);
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostDirectoryPath
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return true;
            }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        public string ToHostDirectoryPath( string dataStorePath )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(null).StoreFileLine();

            if( dataStorePath.NullOrEmpty()
             || !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException("Invalid data store path!").StoreFileLine();

            lock( this.syncLock )
                return this.ToZipPath(dataStorePath, isDirectory: true);
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
            lock( this.syncLock )
                return this.GetNames(dataStorePath, getFiles: true);
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            lock( this.syncLock )
                return this.GetNames(dataStorePath, getFiles: false);
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    var zipPath = this.ToZipPath(dataStorePath, isDirectory: false);
                    if( !this.ContainsEntry(zipPath) )
                        throw new FileNotFoundException().StoreFileLine();

                    return IOWrapper.ToTextReader(this.OpenRead(zipPath));
                }
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    var zipPath = this.ToZipPath(dataStorePath, isDirectory: false);
                    if( !this.ContainsEntry(zipPath) )
                        throw new FileNotFoundException().StoreFileLine();

                    return IOWrapper.ToBinaryReader(this.OpenRead(zipPath));
                }
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
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return true;
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    var zipPath = this.ToZipPath(dataStorePath, isDirectory: false);
                    var size = this.GetUncompressedSize(zipPath);
                    if( !size.HasValue )
                        throw new FileNotFoundException().StoreFileLine();
                    else
                        return size.Value;
                }
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    this.CreateDirectoryRecursively(dataStorePath);
                    this.Flush();
                }
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    var zipFilePath = this.ToZipPath(dataStorePath, isDirectory: false);
                    this.RemoveEntry(zipFilePath);
                    this.Flush();
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    this.DeleteDirectoryRecursively(dataStorePath);
                    this.Flush();
                }
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        public ITextWriter CreateNewText( string dataStorePath, bool overwriteIfExists )
        {
            try
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    // handle existing entry
                    var zipFilePath = this.ToZipPath(dataStorePath, isDirectory: false);
                    if( this.ContainsEntry(zipFilePath) )
                    {
                        if( overwriteIfExists )
                            this.RemoveEntry(zipFilePath);
                        else
                            throw new IOException("The specified file already exists!").Store("zipFilePath", zipFilePath);
                    }

                    // create directory
                    var parentDataStorePath = DataStore.GetParentPath(dataStorePath);
                    if( !parentDataStorePath.NullOrEmpty() )
                        this.CreateDirectoryRecursively(parentDataStorePath);

                    // create new entry
                    var stream = this.FlushAfterDispose(this.CreateFileEntry(zipFilePath));
                    return IOWrapper.ToTextWriter(stream, DataStore.DefaultEncoding, DataStore.DefaultNewLine);
                }
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                ex.Store("overwriteIfExists", overwriteIfExists);
                throw;
            }
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateNewBinary( string dataStorePath, bool overwriteIfExists )
        {
            try
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                lock( this.syncLock )
                {
                    // handle existing entry
                    var zipFilePath = this.ToZipPath(dataStorePath, isDirectory: false);
                    if( this.ContainsEntry(zipFilePath) )
                    {
                        if( overwriteIfExists )
                            this.RemoveEntry(zipFilePath);
                        else
                            throw new IOException("The specified file already exists!").Store("zipFilePath", zipFilePath);
                    }

                    // create directory
                    var parentDataStorePath = DataStore.GetParentPath(dataStorePath);
                    if( !parentDataStorePath.NullOrEmpty() )
                        this.CreateDirectoryRecursively(parentDataStorePath);

                    // create new entry
                    var stream = this.FlushAfterDispose(this.CreateFileEntry(zipFilePath));
                    return IOWrapper.ToBinaryWriter(stream);
                }
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                ex.Store("overwriteIfExists", overwriteIfExists);
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
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return false;
            }
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
            throw new NotSupportedException().StoreFileLine();
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsReadWriteBinary
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return false;
            }
        }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream ReadWriteBinary( string dataStorePath )
        {
            throw new NotSupportedException().StoreFileLine();
        }

        #endregion
    }
}
