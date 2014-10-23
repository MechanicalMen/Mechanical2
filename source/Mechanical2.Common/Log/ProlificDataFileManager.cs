using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.IO;
using Mechanical.IO.FileSystem;

namespace Mechanical.Common.Log
{
    /* NOTE: This class is used when:
     *         - you have lots of data that you need to store
     *         - you only want to keep the latest data (deleting older ones)
     *         - there may be multiple streams of such data, and you don't want to worry about naming conflicts
     *         - you keep such data in the same directory
     * 
     *       Example 1: logging
     *         - you want to keep the last 30 days worth of logs
     *         - more than one instance of the application may run at the same time
     *           (by opening associated files through explorer, for example)
     * 
     *       Example 2: saving raw output from hardware or calculations taking a long time
     *         - you want to keep only the last 10MB of data
     *         - there is more than one hardware or calculation running in parallel (and you don't want to mix such data)
     * 
     *       The trick to managing all this is through a common, case insensitive naming pattern:
     *         <file manager instance id>_<creation time>_<app instance id><optional file extension>
     *         - app instance id: short hash generated once per AppDomain. Allows multiple instances of a single
     *           application, to dump data at the same time, to the same place.
     *         - file manager instance id: a humanly readable identifier of the data source. Necessary, when multiple file managers
     *           are used by the same app. Automatically generated, unless specified.
     *         - creation time: the creation time of the file.
     *         - optional file extension: obvious (though not strictly necessary)
     * 
     * NOTE: creation time precedes application instance ID, so that in "actual" file managers,
     *       sorting by name would sort by time as well.
     */

    /// <summary>
    /// Helps managing files according to a naming pattern.
    /// Useful for for files storing a continuous flow of data
    /// (like log files or raw hardware output).
    /// </summary>
    public class ProlificDataFileManager : DisposableObject
    {
        /* NOTE: The application ID must be unique, and we can only know that
         *       others won't use it, after we created the first file using it.
         *       We generate new application IDs, until we find an unused one.
         *       (We are also assuming that file creation is atomic).
         */

        #region Private and Public Fields

        private const int DefaultAppIDLength = 6;
        private const int DefaultManagerIDLength = 6;
        private const string CreationTimeFormat = "yyyyMMddHHmm";

        /// <summary>
        /// The <see cref="StringComparer"/> you can use for application instance IDs.
        /// </summary>
        public static readonly StringComparer AppInstanceIDComparer = StringComparer.OrdinalIgnoreCase;

        // 32, case insensitive, unique characters
        private static readonly char[] HashCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
        private static string appInstanceID = null;

        private readonly string fileManagerID;
        private readonly string fileExtension;
        private IFileSystem fileSystem;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProlificDataFileManager"/> class.
        /// </summary>
        /// <param name="fileSystem">The abstract file system to use the root directory of. The <see cref="IFileSystem"/> will be disposed along with this object, if possible.</param>
        /// <param name="fileManagerID">The string identifying this <see cref="ProlificDataFileManager"/> instance; or <c>null</c> to generate it randomly.</param>
        /// <param name="fileExtension">The optional file extension, or <c>null</c>.</param>
        public ProlificDataFileManager( IFileSystem fileSystem, string fileManagerID = null, string fileExtension = null )
        {
            try
            {
                Ensure.That(fileSystem).NotNull();
                this.fileSystem = fileSystem;

                // NOTE: Technically we only need to be this strict, when the file system does not escape names.
                //       We do this so that the calling code does not need to keep two identifiers.
                if( !fileManagerID.NullOrEmpty() )
                {
                    if( !DataStore.IsValidName(fileManagerID)
                     || fileManagerID.IndexOf('_') != -1 )
                        throw new ArgumentException("Invalid file manager id! The name must be data store compatible, without underscores.").StoreFileLine();

                    this.fileManagerID = fileManagerID;
                }
                else
                    this.fileManagerID = GenerateRandomHash(DefaultManagerIDLength);

                if( !fileExtension.NullOrEmpty() )
                {
                    if( !fileSystem.EscapesNames )
                        throw new ArgumentException("File extensions are not supported by the specified file system!").StoreFileLine();

                    this.fileExtension = fileExtension;
                }
                else
                    this.fileExtension = string.Empty;
            }
            catch( Exception ex )
            {
                ex.Store("fileManagerID", fileManagerID);
                ex.Store("fileExtension", fileExtension);
                throw;
            }
        }

        #endregion

        #region IDisposableObject

        /// <summary>
        /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
        protected override void OnDispose( bool disposing )
        {
            if( disposing )
            {
                //// dispose-only (i.e. non-finalizable) logic
                //// (managed, disposable resources you own)

                if( this.fileSystem.NotNullReference() )
                {
                    var asDisposable = this.fileSystem as IDisposable;
                    if( asDisposable.NotNullReference() )
                        asDisposable.Dispose();
                    this.fileSystem = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            base.OnDispose(disposing);
        }

        #endregion

        #region Application Hash

        private static void AppendAsBase32( StringBuilder sb, int value )
        {
            //// NOTE: code is partially based on:
            ////       http://www.anotherchris.net/csharp/friendly-unique-id-generation-part-2/#base62

            if( sb.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( value < 0 )
                throw new ArgumentOutOfRangeException().Store("value", value);

            int remainder = 0;
            int quotient = Math.DivRem(value /* dividend */, HashCharacters.Length /* divisor */, out remainder);
            if( quotient > 0 )
                AppendAsBase32(sb, quotient);

            sb.Append(HashCharacters[remainder]);
        }

        private static string GenerateRandomHash( int length )
        {
            if( length < 0 )
                throw new ArgumentOutOfRangeException().Store("length", length);

            if( length == 0 )
                return string.Empty;

            var sb = new StringBuilder();
            var bytes = new byte[4];
            int value;
            using( var random = new RNGCryptoServiceProvider() )
            {
                while( sb.Length < length )
                {
                    random.GetBytes(bytes);

                    value = BitConverter.ToInt32(bytes, startIndex: 0);
                    if( value != int.MinValue )
                        value = Math.Abs(value);
                    else
                        value = int.MaxValue;

                    AppendAsBase32(sb, value);
                }
            }

            return sb.ToString(startIndex: 0, length: length);
        }

        #endregion

        #region Data store name

        private static ProlificDataFileInfo ToFileInfo( string fileManagerID, string appInstanceID, string fileExtension, bool escapesNames )
        {
            var creationTime = DateTime.UtcNow;
            string dataStoreName;
            if( escapesNames )
            {
                string unescaped = SafeString.InvariantFormat(
                    "{0}_{1}_{2}{3}",
                    fileManagerID, // always non-empty
                    creationTime.ToString(CreationTimeFormat, CultureInfo.InvariantCulture), // always non-empty
                    appInstanceID, // always non-empty
                    fileExtension); // may be empty

                dataStoreName = DataStore.EscapeName(unescaped);
            }
            else
            {
                dataStoreName = SafeString.InvariantFormat(
                    "{0}_{1}_{2}",
                    fileManagerID,
                    creationTime.ToString(CreationTimeFormat, CultureInfo.InvariantCulture),
                    appInstanceID);
            }

            return new ProlificDataFileInfo(fileManagerID, appInstanceID, creationTime, fileExtension, dataStoreName);
        }

        private static bool TryParseDataStoreName( string dataStoreName, bool escapesFileNames, out ProlificDataFileInfo fileInfo )
        {
            if( dataStoreName.NullOrEmpty() )
            {
                fileInfo = default(ProlificDataFileInfo);
                return false;
            }

            string fileName = escapesFileNames ? DataStore.UnescapeName(dataStoreName) : dataStoreName;
            string[] parts = fileName.Split(new char[] { '_' }, StringSplitOptions.None);
            if( parts.Length != 3 )
            {
                fileInfo = default(ProlificDataFileInfo);
                return false;
            }

            string fileManagerID = parts[0];
            string creationTimeString = parts[1];
            string appInstanceID = parts[2];
            string fileExtension = null;

            if( escapesFileNames
             && appInstanceID.Length > DefaultAppIDLength )
            {
                fileExtension = appInstanceID.Substring(startIndex: DefaultAppIDLength + 1);
                appInstanceID = appInstanceID.Substring(startIndex: 0, length: DefaultAppIDLength);
            }

            DateTime creationTime;
            if( !DateTime.TryParseExact(creationTimeString, CreationTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out creationTime) )
            {
                fileInfo = default(ProlificDataFileInfo);
                return false;
            }

            fileInfo = new ProlificDataFileInfo(fileManagerID, appInstanceID, creationTime, fileExtension, dataStoreName);
            return true;
        }

        #endregion

        #region Find files

        /// <summary>
        /// Finds all data files in the root of the specified file system. The result is unsorted.
        /// </summary>
        /// <param name="fileSystem">The abstract file system to search the root directory of.</param>
        /// <returns>All data files found.</returns>
        public static ProlificDataFileInfo[] FindAllFiles( IFileSystem fileSystem )
        {
            if( fileSystem.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var fileDataStoreNames = fileSystem.GetFileNames();
            var results = new List<ProlificDataFileInfo>();
            ProlificDataFileInfo fileInfo;
            foreach( var name in fileDataStoreNames )
            {
                if( TryParseDataStoreName(name, fileSystem.EscapesNames, out fileInfo) )
                    results.Add(fileInfo);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns all files recently created by this app instance.
        /// The results are in descending order (according to creation time).
        /// </summary>
        /// <param name="fileSystem">The abstract file system to search the root directory of.</param>
        /// <param name="maxFileAge">The maximum age of the files returned, or <c>null</c>.</param>
        /// <param name="maxAppInstanceCount">The maximum number of app instances to include, or <c>null</c>.</param>
        /// <param name="maxTotalFileSize">The maximum total file size, or <c>null</c>.</param>
        /// <returns>All files recently created by this app instance.</returns>
        public static ProlificDataFileInfo[] FindLatestFiles( IFileSystem fileSystem, TimeSpan? maxFileAge, int? maxAppInstanceCount, long? maxTotalFileSize )
        {
            if( fileSystem.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( maxFileAge.HasValue
             && maxFileAge.Value.Ticks < 0 )
                throw new ArgumentOutOfRangeException().Store("maxFileAge", maxFileAge);

            if( maxAppInstanceCount.HasValue
             && maxAppInstanceCount.Value < 0 )
                throw new ArgumentOutOfRangeException().Store("maxAppInstanceCount", maxAppInstanceCount);

            if( maxTotalFileSize.HasValue )
            {
                if( maxTotalFileSize.Value < 0 )
                    throw new ArgumentOutOfRangeException().Store("maxTotalFileSize", maxTotalFileSize);

                if( !fileSystem.SupportsGetFileSize )
                    throw new NotSupportedException("The specified file system does not support GetFileSize!").StoreFileLine();
            }


            // sort in descending order by CreationTime
            var files = FindAllFiles(fileSystem).ToList();
            files.Sort(( x, y ) => y.CreationTime.CompareTo(x.CreationTime));

            // determine the minimum CreationTime (if there is one)
            DateTime? minTime = null;
            if( maxFileAge.HasValue )
                minTime = DateTime.UtcNow - maxFileAge.Value;

            HashSet<string> appInstancesAllowed = null;
            if( maxAppInstanceCount.HasValue )
                appInstancesAllowed = new HashSet<string>(AppInstanceIDComparer);

            long totalFileSize = 0;
            long currentFileSize;

            ProlificDataFileInfo fileInfo;
            for( int i = 0; i < files.Count; )
            {
                fileInfo = files[i];

                // too old?
                if( minTime.HasValue
                 && fileInfo.CreationTime < minTime )
                {
                    files.RemoveAt(i);
                    continue;
                }

                // too many app instances?
                if( maxAppInstanceCount.HasValue
                 && !appInstancesAllowed.Contains(fileInfo.AppInstanceID) )
                {
                    if( appInstancesAllowed.Count < maxAppInstanceCount.Value )
                    {
                        //// there is still room for one more app instance,
                        //// but we do not record the new app instance ID here,
                        //// since another rule may still throw this file out...
                    }
                    else
                    {
                        // a yet unseen app instance ID, but we have no more room for it
                        files.RemoveAt(i);
                        continue;
                    }
                }

                // too many bytes?
                if( maxTotalFileSize.HasValue )
                {
                    currentFileSize = fileSystem.GetFileSize(fileInfo.DataStoreName);
                    if( totalFileSize + currentFileSize > maxTotalFileSize.Value )
                    {
                        // adding this file would exceed the file size limit, so we'll skip it
                        files.RemoveAt(i);

                        // we will also raise the total file size recorded beyond the maximum:
                        // it could be possible that later there would be a file small
                        // enough to still fit in, but an unbroken time line seems more
                        // important than cramming as many bytes in as possible...
                        // It's just a feeling though, maybe this is stupid :)
                        totalFileSize = maxTotalFileSize.Value + 1;
                        continue;
                    }
                    else
                    {
                        //// there is still room for these bytes,
                        //// but we do not update the total file size here,
                        //// since another rule may still throw this file out...
                        //// (one added at a later time)
                    }
                }
                else
                    currentFileSize = 0;

                // no rule removed this file: update variables and continue
                appInstancesAllowed.Add(fileInfo.AppInstanceID);
                totalFileSize += currentFileSize;
                ++i;
            }

            return files.ToArray();
        }

        #endregion

        #region Create file

        /// <summary>
        /// Creates a new file, with the correct naming scheme.
        /// </summary>
        /// <param name="fileInfo">The <see cref="ProlificDataFileInfo"/> instance representing the name of the file created.</param>
        /// <param name="useWriteThroughIfSupported"><c>true</c> to try to use <see cref="IFileSystemWriter.CreateWriteThroughBinary"/> instead of <see cref="IFileSystemWriter.CreateNewBinary"/>.</param>
        /// <returns>The <see cref="IBinaryWriter"/> representing the file created.</returns>
        public IBinaryWriter CreateFile( out ProlificDataFileInfo fileInfo, bool useWriteThroughIfSupported = false )
        {
            Ensure.That(this).NotDisposed();

            bool writeThroughSupported = false;
            if( useWriteThroughIfSupported )
                writeThroughSupported = this.fileSystem.SupportsCreateWriteThroughBinary;

            // we may have to try multiple times, if we don't yet have an
            // app instance ID, that's verified to be unique
            while( true )
            {
                string appID = appInstanceID;
                bool isNewAppID = false;
                if( appID.NullReference() )
                {
                    // NOTE: random does not necessarily mean unique. But since I find it unlikely to
                    //       have even a few dozen hashes at the same time, we should be in the clear:
                    //       32^6 ~= 1+ billion; and we probably wouldn't see conflicts before we had
                    //       40000-50000 hashes present at the same time. Based on an answer here:
                    //       http://stackoverflow.com/a/9543797
                    appID = GenerateRandomHash(DefaultAppIDLength);
                    isNewAppID = true;
                }

                // try to create the file
                var file = ToFileInfo(this.fileManagerID, appID, this.fileExtension, this.fileSystem.EscapesNames);
                IBinaryWriter fileStream;
                try
                {
                    if( useWriteThroughIfSupported && writeThroughSupported )
                        fileStream = this.fileSystem.CreateWriteThroughBinary(file.DataStoreName, overwriteIfExists: false);
                    else
                        fileStream = this.fileSystem.CreateNewBinary(file.DataStoreName, overwriteIfExists: false);
                }
                catch( Exception ex )
                {
                    //// the file already exists, or there was another problem

                    bool fileAlreadyExists;
                    try
                    {
                        fileAlreadyExists = FindAllFiles(this.fileSystem)
                            .Where(f => DataStore.Comparer.Equals(f.DataStoreName, file.DataStoreName))
                            .FirstOrNullable()
                            .HasValue;
                    }
                    catch( Exception exx )
                    {
                        // there is definitely a problem with the file system: report both exceptions
                        throw new AggregateException("Failed to create file!", ex, exx).StoreFileLine();
                    }

                    if( fileAlreadyExists )
                    {
                        //// NOTE: we are assuming here, that files created by the same source,
                        ////       in the same app instance, are at least a minute apart.

                        // try again with a new app instance ID
                        continue;
                    }
                    else
                    {
                        ex.StoreFileLine();
                        throw; // there was some kind of problem, trying to create the file...
                    }
                }

                // alright, we successfully created the file, but that does not mean, that
                // the app instance ID was not already in use, sometime before that
                if( isNewAppID )
                {
                    bool appIDAlreadyInUse = FindAllFiles(this.fileSystem)
                        .Where(f => !DataStore.Comparer.Equals(f.DataStoreName, file.DataStoreName))
                        .Where(f => AppInstanceIDComparer.Equals(f.AppInstanceID, appID))
                        .FirstOrNullable()
                        .HasValue;

                    if( appIDAlreadyInUse )
                    {
                        // remove this file to avoid confusion, and try again
                        this.fileSystem.DeleteFile(file.DataStoreName);
                        continue;
                    }
                    else
                    {
                        // a new and unique app ID was found
                        var prevAppID = Interlocked.CompareExchange(ref appInstanceID, appID, comparand: null);
                        if( !prevAppID.NullReference() )
                        {
                            // however, this method was called concurrently, and another unique app ID was already established
                            // clean up, and try again with that ID
                            this.fileSystem.DeleteFile(file.DataStoreName);
                            continue;
                        }
                    }
                }

                // either the app ID was not generated just now,
                // or it was, and we successfully saved it
                fileInfo = file;
                return fileStream;
            }
        }

        #endregion

        #region Delete old files

        /// <summary>
        /// Deletes all but the most recent files.
        /// </summary>
        /// <param name="fileSystem">The abstract file system to search the root directory of.</param>
        /// <param name="maxFileAge">The maximum age of the files to keep, or <c>null</c>.</param>
        /// <param name="maxAppInstanceCount">The maximum number of app instances to keep, or <c>null</c>.</param>
        /// <param name="maxTotalFileSize">The maximum total file size to keep, or <c>null</c>.</param>
        public static void DeleteOldFiles( IFileSystem fileSystem, TimeSpan? maxFileAge, int? maxAppInstanceCount, long? maxTotalFileSize )
        {
            var allFiles = FindAllFiles(fileSystem);
            var filesToKeep = FindLatestFiles(fileSystem, maxFileAge, maxAppInstanceCount, maxTotalFileSize);

            foreach( var file in allFiles )
            {
                bool keepFile = false;
                foreach( var f in filesToKeep )
                {
                    if( DataStore.Comparer.Equals(file.DataStoreName, f.DataStoreName) )
                    {
                        keepFile = true;
                        break;
                    }
                }

                if( keepFile )
                    continue;
                else
                    fileSystem.DeleteFile(file.DataStoreName);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the string identifying the current application instance.
        /// </summary>
        /// <value>The string identifying the current application instance.</value>
        public static string AppInstanceID
        {
            get
            {
                var appID = appInstanceID;
                if( appID.NullReference() )
                    throw new InvalidOperationException("The application instance ID can not be determined, before the first file is created!").StoreFileLine();

                return appID;
            }
        }

        /// <summary>
        /// Gets the <see cref="IFileSystem"/> in use by this instance.
        /// </summary>
        /// <value>The <see cref="IFileSystem"/> in use.</value>
        public IFileSystem FileSystem
        {
            get
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.fileSystem;
            }
        }

        #endregion
    }
}
