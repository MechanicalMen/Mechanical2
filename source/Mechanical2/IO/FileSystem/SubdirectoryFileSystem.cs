using System;
using Mechanical.Conditions;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Wraps the contents of a directory, as an <see cref="IFileSystem"/>.
    /// </summary>
    public class SubdirectoryFileSystem : SubdirectoryFileSystemReaderWriter, IFileSystem
    {
        #region Private Fields

        private readonly IFileSystem parentFileSystem;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdirectoryFileSystem"/> class
        /// </summary>
        /// <param name="parentFileSystem">The file system to represent a subdirectory of.</param>
        /// <param name="dataStorePath">The (data store) path of the directory to represent.</param>
        public SubdirectoryFileSystem( IFileSystem parentFileSystem, string dataStorePath )
            : base(parentFileSystem, dataStorePath)
        {
            this.parentFileSystem = parentFileSystem; // base class already checked for null
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
            if( !DataStore.IsValidPath(dataStorePath)
             || dataStorePath.Length == 0 )
                throw new ArgumentException().Store("dataStorePath", dataStorePath);

            return this.parentFileSystem.OpenBinary(DataStore.Combine(this.SubDirPath, dataStorePath));
        }

        #endregion
    }
}
