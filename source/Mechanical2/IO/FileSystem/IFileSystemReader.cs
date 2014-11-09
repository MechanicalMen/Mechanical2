using System;
using System.Collections.Generic;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents an abstract, readable file system.
    /// </summary>
    public interface IFileSystemReader : IFileSystemBase
    {
        /// <summary>
        /// Gets the names of the files found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files.</param>
        /// <returns>The names of the files found.</returns>
        string[] GetFileNames( string dataStorePath = "" );

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        string[] GetDirectoryNames( string dataStorePath = "" );

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        ITextReader ReadText( string dataStorePath );

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        IBinaryReader ReadBinary( string dataStorePath );


        /// <summary>
        /// Gets a value indicating whether the GetFileSize method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        bool SupportsGetFileSize { get; }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        long GetFileSize( string dataStorePath );
    }

    /// <content>
    /// Methods extending the <see cref="IFileSystemReader"/> type.
    /// </content>
    public static partial class FileSystemExtensions
    {
        #region GetFileSystemEntryNames

        /// <summary>
        /// Gets the names of the files or directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files or directories.</param>
        /// <returns>The names of the files or directories found.</returns>
        public static string[] GetFileSystemEntryNames( this IFileSystemReader fileSystem, string dataStorePath = "" )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            var entries = new List<string>();
            entries.AddRange(fileSystem.GetFileNames(dataStorePath));
            entries.AddRange(fileSystem.GetDirectoryNames(dataStorePath));
            return entries.ToArray();
        }

        #endregion

        #region Get*Recursively

        private static void AddEntriesRecursively( IFileSystemReader fileSystem, List<string> entries, string directory, bool addFiles, bool addDirectories, string prefix = "" )
        {
            if( addDirectories
             && !directory.NullOrEmpty() )
                entries.Add(directory);

            foreach( var d in fileSystem.GetDirectoryNames(directory) )
                AddEntriesRecursively(fileSystem, entries, DataStore.Combine(directory, d), addFiles, addDirectories, DataStore.Combine(prefix, d));

            if( addFiles )
            {
                foreach( var f in fileSystem.GetFileNames(directory) )
                    entries.Add(DataStore.Combine(prefix, f));
            }
        }

        /// <summary>
        /// Gets the names of the files found in the specified directory.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files.</param>
        /// <returns>The names of the files found.</returns>
        public static string[] GetFileNamesRecursively( this IFileSystemReader fileSystem, string dataStorePath = "" )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            var entries = new List<string>();
            AddEntriesRecursively(fileSystem, entries, dataStorePath, addFiles: true, addDirectories: false);
            return entries.ToArray();
        }

        /// <summary>
        /// Gets the names of the directories found in the specified directory.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public static string[] GetDirectoryNamesRecursively( this IFileSystemReader fileSystem, string dataStorePath = "" )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            var entries = new List<string>();
            AddEntriesRecursively(fileSystem, entries, dataStorePath, addFiles: false, addDirectories: true);
            return entries.ToArray();
        }

        /// <summary>
        /// Gets the names of the files or directories found in the specified directory.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files or directories.</param>
        /// <returns>The names of the files or directories found.</returns>
        public static string[] GetFileSystemEntryNamesRecursively( this IFileSystemReader fileSystem, string dataStorePath = "" )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            var entries = new List<string>();
            AddEntriesRecursively(fileSystem, entries, dataStorePath, addFiles: true, addDirectories: true);
            return entries.ToArray();
        }

        #endregion

        #region GetDirectorySize

        /// <summary>
        /// Gets the size, in bytes, of all files in the specified directory, recursively.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The size of all files in the specified directory in bytes.</returns>
        public static long GetDirectorySize( this IFileSystemReader fileSystem, string dataStorePath = "" )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            long sum = 0;
            foreach( var f in fileSystem.GetFileNamesRecursively(dataStorePath) )
                sum += fileSystem.GetFileSize(DataStore.Combine(dataStorePath, f));
            return sum;
        }

        #endregion

        #region *Exists

        private static bool Exists( IFileSystemReader fileSystem, string dataStorePath, Func<IFileSystemReader, string, string[]> getEntries )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            if( dataStorePath.NullOrEmpty()
             || !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            var parentPath = DataStore.GetParentPath(dataStorePath);
            var entries = getEntries(fileSystem, parentPath);
            foreach( var f in entries )
            {
                if( DataStore.Comparer.Equals(dataStorePath, DataStore.Combine(parentPath, f)) )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specifies file exists.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the file to search for.</param>
        /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
        public static bool FileExists( this IFileSystemReader fileSystem, string dataStorePath )
        {
            return Exists(fileSystem, dataStorePath, ( fileSys, path ) => fileSys.GetFileNames(path));
        }

        /// <summary>
        /// Determines whether the specifies directory exists.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the directory to search for.</param>
        /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
        public static bool DirectoryExists( this IFileSystemReader fileSystem, string dataStorePath )
        {
            return Exists(fileSystem, dataStorePath, ( fileSys, path ) => fileSys.GetDirectoryNames(path));
        }

        /// <summary>
        /// Determines whether the specifies file or directory exists.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the file or directory to search for.</param>
        /// <returns><c>true</c> if the file or directory exists; otherwise, <c>false</c>.</returns>
        public static bool Exists( this IFileSystemReader fileSystem, string dataStorePath )
        {
            return Exists(fileSystem, dataStorePath, ( fileSys, path ) => fileSys.GetFileSystemEntryNames(path));
        }

        #endregion

        #region ReadAllText

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="fileSystem">The file system to query.</param>
        /// <param name="dataStorePath">The data store path specifying the file to read.</param>
        /// <returns>A string containing all lines of the file.</returns>
        public static string ReadAllText( this IFileSystemReader fileSystem, string dataStorePath )
        {
            Ensure.Debug(fileSystem, f => f.NotNull());

            var reader = fileSystem.ReadText(dataStorePath);
            var result = reader.ReadToEnd();
            reader.Close();
            return result;
        }

        #endregion

        #region CopyAllTo

        /// <summary>
        /// Copies all files and directories to the specified file system.
        /// </summary>
        /// <param name="sourceFileSystem">The file system to query.</param>
        /// <param name="destinationFileSystem">The file system to copy to.</param>
        public static void CopyAllTo( this IFileSystemReader sourceFileSystem, IFileSystemWriter destinationFileSystem )
        {
            Ensure.Debug(sourceFileSystem, f => f.NotNull());
            Ensure.That(destinationFileSystem).NotNull();

            foreach( var dir in sourceFileSystem.GetDirectoryNamesRecursively() )
                destinationFileSystem.CreateDirectory(dir);

            foreach( var file in sourceFileSystem.GetFileNamesRecursively() )
            {
                var sourceStream = sourceFileSystem.ReadBinary(file);
                var destinationStream = destinationFileSystem.CreateNewBinary(file, overwriteIfExists: true);
                sourceStream.CopyTo(destinationStream);
                sourceStream.Close();
                destinationStream.Close();
            }
        }

        #endregion
    }
}
