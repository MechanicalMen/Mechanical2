using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents a zip file as an abstract readable file system.
    /// </summary>
    public class ZipFileSystemReader : IFileSystemReader
    {
        #region Private Fields

        private readonly bool escapeFileNames;
        private ZipArchive zipArchive;

        #endregion

        #region Constructors

        internal ZipFileSystemReader( ZipArchive archive, bool escapeFileNames )
        {
            Ensure.Debug(archive, a => a.NotNull());

            this.zipArchive = archive;
            this.escapeFileNames = escapeFileNames;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystemReader"/> class
        /// </summary>
        /// <param name="stream">The seekable <see cref="Stream"/> that contains the archive to be read.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystemReader( Stream stream, bool escapeFileNames )
        {
            try
            {
                // ZipArchive would do this automatically, but we want the user to
                // recognize that their whole .zip file would be in the memory
                Ensure.That(stream).NotNull();
                Ensure.That(stream.CanSeek).IsTrue(() => new ArgumentException("Not a seekable stream! Copy to a MemoryStream first (if feasible)."));

                this.zipArchive = new ZipArchive(
                    stream,
                    ZipArchiveMode.Read,
                    leaveOpen: false,
                    entryNameEncoding: ZipFileSystem.ZipEntryNameEncoding);
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("escapeFileNames", escapeFileNames);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystemReader"/> class
        /// </summary>
        /// <param name="reader">The <see cref="ISeekableBinaryReader"/> that contains the archive to be read.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystemReader( ISeekableBinaryReader reader, bool escapeFileNames )
            : this(IOWrapper.Wrap(reader, canSeek: true), escapeFileNames) // the .NET 4.5 ZipArchive and ZipArchiveEntry do not use SetLength in Read mode
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileSystemReader"/> class
        /// </summary>
        /// <param name="zipFilePath">The path of the .zip file to open.</param>
        /// <param name="escapeFileNames">Indicates whether the original file names are escaped.</param>
        public ZipFileSystemReader( string zipFilePath, bool escapeFileNames )
            : this(File.OpenRead(zipFilePath), escapeFileNames)
        {
        }

        #endregion

        #region Private Methods

        private string[] GetNames( string dataStorePath, bool getFiles )
        {
            try
            {
                var zipPath = ZipFileSystem.ToZipPath(dataStorePath, this.escapeFileNames, isDirectory: true);
                var results = new List<string>();

                bool isDirectory;
                string relativeZipFilePath;
                foreach( var zipEntry in this.zipArchive.Entries )
                {
                    if( zipEntry.FullName.StartsWith(zipPath) )
                    {
                        if( string.Equals(zipEntry.FullName, zipPath, StringComparison.Ordinal) )
                            continue;

                        isDirectory = zipEntry.FullName[zipEntry.FullName.Length - 1] == ZipFileSystem.ZipDirectorySeparator;
                        if( (getFiles && !isDirectory)
                         || (!getFiles && isDirectory) )
                        {
                            relativeZipFilePath = zipEntry.FullName.Substring(startIndex: zipPath.Length, length: zipEntry.FullName.Length - zipPath.Length - (isDirectory ? 1 : 0));
                            if( relativeZipFilePath.IndexOf(ZipFileSystem.ZipDirectorySeparator) == -1 )
                                results.Add(ZipFileSystem.FromZipPath(relativeZipFilePath, this.escapeFileNames));
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

        #endregion

        #region IDataStoreFileSystemReader

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
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var zipPath = ZipFileSystem.ToZipPath(dataStorePath, this.escapeFileNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipPath);
                if( zipEntry.NullReference() )
                    throw new FileNotFoundException().StoreFileLine();

                return IOWrapper.ToTextReader(zipEntry.Open());
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

                var zipPath = ZipFileSystem.ToZipPath(dataStorePath, this.escapeFileNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipPath);
                if( zipEntry.NullReference() )
                    throw new FileNotFoundException().StoreFileLine();

                return IOWrapper.ToBinaryReader(zipEntry.Open());
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

                var zipPath = ZipFileSystem.ToZipPath(dataStorePath, this.escapeFileNames, isDirectory: false);
                var zipEntry = this.zipArchive.GetEntry(zipPath);
                if( zipEntry.NullReference() )
                    throw new FileNotFoundException().StoreFileLine();

                return zipEntry.Length;
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
