using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.Common.Logs
{
    /// <summary>
    /// Represents general, immutable information about a data file (see <see cref="ProlificDataFileManager"/> for more information).
    /// </summary>
    public struct ProlificDataFileInfo
    {
        #region Private Fields

        private readonly string fileManagerID;
        private readonly string appInstanceID;
        private readonly DateTime creationTime;
        private readonly string fileExtension;
        private readonly string dataStoreName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProlificDataFileInfo"/> struct.
        /// </summary>
        /// <param name="fileManagerID">The string identifying the <see cref="ProlificDataFileManager"/> instance.</param>
        /// <param name="appInstanceID">The string identifying the current application instance.</param>
        /// <param name="creationTime">The creation time of the file based on it's name.</param>
        /// <param name="fileExtension">The optional file extension, or <c>null</c>.</param>
        /// <param name="dataStoreName">The data store name used to access the file through the abstract file system.</param>
        public ProlificDataFileInfo( string fileManagerID, string appInstanceID, DateTime creationTime, string fileExtension, string dataStoreName )
        {
            try
            {
                if( fileManagerID.NullOrLengthy() )
                    throw new ArgumentException("Bad argument: fileManagerID").StoreFileLine();

                if( appInstanceID.NullOrLengthy() )
                    throw new ArgumentException("Bad argument: appInstanceID").StoreFileLine();

                if( creationTime.Kind == DateTimeKind.Unspecified )
                    throw new ArgumentException("Unspecified DateTimeKind is not supported!").StoreFileLine();

                if( !DataStore.IsValidName(dataStoreName) )
                    throw new ArgumentException("Bad argument: dataStoreName").StoreFileLine();

                this.fileManagerID = fileManagerID;
                this.appInstanceID = appInstanceID;
                this.creationTime = creationTime.ToUniversalTime();
                this.fileExtension = fileExtension.NullReference() ? string.Empty : fileExtension;
                this.dataStoreName = dataStoreName;
            }
            catch( Exception ex )
            {
                ex.Store("fileManagerID", fileManagerID);
                ex.Store("appInstanceID", appInstanceID);
                ex.Store("creationTime", creationTime);
                ex.Store("fileExtension", fileExtension);
                ex.Store("dataStoreName", dataStoreName);
                throw;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the string identifying the <see cref="ProlificDataFileManager"/> instance.
        /// </summary>
        /// <value>The string identifying the <see cref="ProlificDataFileManager"/> instance.</value>
        public string FileManagerID
        {
            get { return this.fileManagerID; }
        }

        /// <summary>
        /// Gets the string identifying the application instance.
        /// </summary>
        /// <value>The string identifying the application instance.</value>
        public string AppInstanceID
        {
            get { return this.appInstanceID; }
        }

        /// <summary>
        /// Gets the creation time of the file based on it's name.
        /// The full precision is NOT used (use the file system API for that).
        /// </summary>
        /// <value>The creation time of the file based on it's name.</value>
        public DateTime CreationTime
        {
            get { return this.creationTime; }
        }

        /// <summary>
        /// Gets the optional file extension. May be <c>null</c>.
        /// </summary>
        /// <value>The optional file extension, or <c>null</c>.</value>
        public string FileExtension
        {
            get { return this.fileExtension; }
        }

        /// <summary>
        /// Gets the data store name used to access the file through the abstract file system.
        /// </summary>
        /// <value>The data store name used to access the file through the abstract file system.</value>
        public string DataStoreName
        {
            get { return this.dataStoreName; }
        }

        #endregion
    }
}
