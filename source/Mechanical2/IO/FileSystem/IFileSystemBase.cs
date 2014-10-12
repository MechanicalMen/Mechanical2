using System;
using Mechanical.Conditions;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Represents file system related functionality, that is
    /// not directly reading or writing related.
    /// </summary>
    public interface IFileSystemBase
    {
        /// <summary>
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        bool EscapesNames { get; }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        bool SupportsToHostFilePath { get; }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        string ToHostFilePath( string dataStorePath );


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        bool SupportsToHostDirectoryPath { get; }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        string ToHostDirectoryPath( string dataStorePath );
    }
}
