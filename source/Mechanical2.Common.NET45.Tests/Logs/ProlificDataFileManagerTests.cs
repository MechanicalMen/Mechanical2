using System;
using System.Collections.Generic;
using System.Linq;
using Mechanical.Collections;
using Mechanical.Common.Logs;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.IO.FileSystem;
using Mechanical.Tests;
using NUnit.Framework;

namespace Mechanical.Common.Tests.Logs
{
    [TestFixture]
    public class ProlificDataFileManagerTests
    {
        static ProlificDataFileManagerTests()
        {
            TestApp.Initialize();
        }

        [Test]
        public void FixedIDTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager = new ProlificDataFileManager(fs, fileManagerID: "aBc") )
            {
                Assert.True(ProlificDataFileManager.IDComparer.Equals(manager.FileManagerID, "aBc"));

                ProlificDataFileInfo fileInfo;
                var file = manager.CreateFile(out fileInfo);
                file.Close();

                Assert.True(ProlificDataFileManager.IDComparer.Equals(fileInfo.FileManagerID, "aBc"));
                Assert.True(ProlificDataFileManager.IDComparer.Equals(manager.FileManagerID, "aBc"));
            }
        }

        [Test]
        public void GeneratedIDTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager1 = new ProlificDataFileManager(fs) )
            using( var manager2 = new ProlificDataFileManager(fs) )
            {
                ProlificDataFileInfo fileInfo1;
                ProlificDataFileInfo fileInfo2;
                var file1 = manager1.CreateFile(out fileInfo1);
                var file2 = manager2.CreateFile(out fileInfo2);
                file1.Close();
                file2.Close();

                // same app instance IDs
                Assert.True(ProlificDataFileManager.IDComparer.Equals(fileInfo1.AppInstanceID, fileInfo2.AppInstanceID));

                // different file manager IDs
                Assert.False(ProlificDataFileManager.IDComparer.Equals(fileInfo1.FileManagerID, fileInfo2.FileManagerID));
                Assert.False(ProlificDataFileManager.IDComparer.Equals(manager1.FileManagerID, manager2.FileManagerID));

                // file manager and the files it creates share the same ID
                Assert.True(ProlificDataFileManager.IDComparer.Equals(manager1.FileManagerID, fileInfo1.FileManagerID));
            }
        }

        [Test]
        public void TimeFormatTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager = new ProlificDataFileManager(fs) )
            {
                var dateTimeProvider = (TestDateTimeProvider)AppCore.MagicBag.Pull<IDateTimeProvider>();
                dateTimeProvider.UtcNow = new DateTime(2014, 12, 31, 23, 59, 57, DateTimeKind.Utc); // precision in seconds

                ProlificDataFileInfo fileInfo;
                var file = manager.CreateFile(out fileInfo);
                file.Close();

                Assert.AreEqual(fileInfo.CreationTime, new DateTime(2014, 12, 31, 23, 59, 0, DateTimeKind.Utc)); // precision in minutes
            }
        }

        [Test]
        public void FileExtensionTest()
        {
            var unescapedFileSystem = new DataStoreObjectFileSystem(escapeFileNames: false);
            var escapedFileSystem = new DataStoreObjectFileSystem(escapeFileNames: true);

            // you can't have file extensions, unless you escape file names
            Assert.Throws(typeof(ArgumentException), () => new ProlificDataFileManager(unescapedFileSystem, fileExtension: ".dat"));

            using( var manager1 = new ProlificDataFileManager(unescapedFileSystem, fileExtension: null) )
            using( var manager2 = new ProlificDataFileManager(escapedFileSystem, fileExtension: ".dat") )
            {
                ProlificDataFileInfo fileInfo1;
                ProlificDataFileInfo fileInfo2;
                var file1 = manager1.CreateFile(out fileInfo1);
                var file2 = manager2.CreateFile(out fileInfo2);
                file1.Close();
                file2.Close();

                Assert.IsNullOrEmpty(fileInfo1.FileExtension);
                Assert.True(string.Equals(fileInfo2.FileExtension, ".dat", StringComparison.Ordinal));
            }
        }

        [Test]
        public void OneAppInstancePerAppDomainTest()
        {
            var secondAppDomain = AppDomain.CreateDomain(
                "OneAppInstancePerAppDomainTest",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
            try
            {
                SetAppInstanceID();
                var appID1 = (string)AppDomain.CurrentDomain.GetData("AppInstanceID");
                secondAppDomain.DoCallBack(SetAppInstanceID);
                var appID2 = (string)secondAppDomain.GetData("AppInstanceID");

                Assert.False(ProlificDataFileManager.IDComparer.Equals(appID1, appID2));
            }
            finally
            {
                AppDomain.Unload(secondAppDomain);
            }
        }

        private static void SetAppInstanceID()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager = new ProlificDataFileManager(fs) )
            {
                ProlificDataFileInfo fileInfo;
                var file = manager.CreateFile(out fileInfo);
                file.Close();

                AppDomain.CurrentDomain.SetData("AppInstanceID", ProlificDataFileManager.ApplicationID);
            }
        }

        [Test]
        public void FilePathTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager = new ProlificDataFileManager(fs, fileManagerID: "aBc") )
            {
                Assert.True(fs.GetFileNames().NullOrEmpty());

                ProlificDataFileInfo fileInfo;
                var file = manager.CreateFile(out fileInfo);
                file.Close();

                Assert.AreEqual(1, fs.GetFileNames().Length);
                Assert.True(DataStore.Comparer.Equals(fileInfo.DataStoreName, fs.GetFileNames()[0]));
            }
        }

        [Test]
        public void FindAllFilesTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager1 = new ProlificDataFileManager(fs) )
            using( var manager2 = new ProlificDataFileManager(fs) )
            {
                ProlificDataFileInfo fileInfo1;
                ProlificDataFileInfo fileInfo2;
                var file1 = manager1.CreateFile(out fileInfo1);
                var file2 = manager2.CreateFile(out fileInfo2);
                file1.Close();
                file2.Close();

                var files1 = manager1.FindAllFiles(currentFileManagerOnly: true);
                var files2 = manager2.FindAllFiles(currentFileManagerOnly: true);
                Assert.AreEqual(1, files1.Length);
                Assert.AreEqual(1, files2.Length);
                Assert.True(DataStore.Comparer.Equals(fileInfo1.DataStoreName, files1[0].DataStoreName));
                Assert.True(DataStore.Comparer.Equals(fileInfo2.DataStoreName, files2[0].DataStoreName));

                var filesFromBoth = new HashSet<string>(manager1.FindAllFiles(currentFileManagerOnly: false).Select(fi => fi.DataStoreName), DataStore.Comparer);
                Assert.AreEqual(2, filesFromBoth.Count);
                Assert.True(filesFromBoth.Contains(fileInfo1.DataStoreName));
                Assert.True(filesFromBoth.Contains(fileInfo2.DataStoreName));
            }
        }

        [Test]
        public void FindLatestFilesTest()
        {
            var dateTimeProvider = (TestDateTimeProvider)AppCore.MagicBag.Pull<IDateTimeProvider>();

            // create 3 files from two managers
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            using( var manager1 = new ProlificDataFileManager(fs, fileManagerID: "m1") )
            using( var manager2 = new ProlificDataFileManager(fs, fileManagerID: "m2") )
            {
                ProlificDataFileInfo fileInfo1;
                ProlificDataFileInfo fileInfo2;
                ProlificDataFileInfo fileInfo3;

                dateTimeProvider.UtcNow = DateTime.UtcNow;
                var file1 = manager1.CreateFile(out fileInfo1);
                file1.Write((byte)1);
                file1.Close();

                dateTimeProvider.UtcNow = dateTimeProvider.UtcNow.AddMinutes(1);
                var file2 = manager2.CreateFile(out fileInfo2);
                file2.Write((byte)2);
                file2.Close();

                dateTimeProvider.UtcNow = dateTimeProvider.UtcNow.AddMinutes(1);
                var file3 = manager1.CreateFile(out fileInfo3);
                file3.Close();

                // change app instance ID of file3
                {
                    var newAppID = new string('a', count: fileInfo3.AppInstanceID.Length + 1);
                    fs.DeleteFile(fileInfo3.DataStoreName);
                    var newDataStoreName = fileInfo3.DataStoreName.Substring(startIndex: 0, length: fileInfo3.DataStoreName.Length - fileInfo3.AppInstanceID.Length) + newAppID;
                    fileInfo3 = new ProlificDataFileInfo(fileInfo3.FileManagerID, newAppID, fileInfo3.CreationTime, fileInfo3.FileExtension, newDataStoreName);

                    file3 = fs.CreateNewBinary(newDataStoreName, overwriteIfExists: false);
                    file3.Write((byte)3);
                    file3.Close();
                }

                // all files, in descending order
                var files = manager1.FindLatestFiles(currentFileManagerOnly: false, maxFileAge: null, maxAppInstanceCount: null, maxTotalFileSize: null);
                Assert.AreEqual(3, files.Length);
                Assert.True(DataStore.Comparer.Equals(files[0].DataStoreName, fileInfo3.DataStoreName));
                Assert.True(DataStore.Comparer.Equals(files[1].DataStoreName, fileInfo2.DataStoreName));
                Assert.True(DataStore.Comparer.Equals(files[2].DataStoreName, fileInfo1.DataStoreName));

                // file manager filter
                files = manager1.FindLatestFiles(currentFileManagerOnly: true, maxFileAge: null, maxAppInstanceCount: null, maxTotalFileSize: null);
                Assert.AreEqual(2, files.Length);
                Assert.True(DataStore.Comparer.Equals(files[0].DataStoreName, fileInfo3.DataStoreName));
                Assert.True(DataStore.Comparer.Equals(files[1].DataStoreName, fileInfo1.DataStoreName));

                // file age filter
                dateTimeProvider.UtcNow = dateTimeProvider.UtcNow.AddMinutes(1);
                files = manager1.FindLatestFiles(currentFileManagerOnly: false, maxFileAge: TimeSpan.FromMinutes(1), maxAppInstanceCount: null, maxTotalFileSize: null);
                Assert.AreEqual(1, files.Length);
                Assert.True(DataStore.Comparer.Equals(files[0].DataStoreName, fileInfo3.DataStoreName));

                // max. app instance count filter
                files = manager1.FindLatestFiles(currentFileManagerOnly: false, maxFileAge: null, maxAppInstanceCount: 1, maxTotalFileSize: null);
                Assert.AreEqual(1, files.Length);
                Assert.True(DataStore.Comparer.Equals(files[0].DataStoreName, fileInfo3.DataStoreName));

                // max. total file size filter
                files = manager1.FindLatestFiles(currentFileManagerOnly: false, maxFileAge: null, maxAppInstanceCount: null, maxTotalFileSize: 2);
                Assert.AreEqual(2, files.Length);
                Assert.True(DataStore.Comparer.Equals(files[0].DataStoreName, fileInfo3.DataStoreName));
                Assert.True(DataStore.Comparer.Equals(files[1].DataStoreName, fileInfo2.DataStoreName));
            }
        }

        [Test]
        public void BadFileSystemTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: false);
            ProlificDataFileInfo fileInfo1;
            using( var manager1 = new ProlificDataFileManager(fs) )
            {
                var file1 = manager1.CreateFile(out fileInfo1);
                file1.Close();
            }

            var noWriteFS = new ErrorFileSystem(fs, throwOnRead: false, throwOnWrite: true, throwOnList: false);
            using( var manager2 = new ProlificDataFileManager(noWriteFS) )
            {
                ProlificDataFileInfo fileInfo;
                Assert.Throws<System.IO.IOException>(() => manager2.CreateFile(out fileInfo));

                //// NOTE this may not seem like much, but we're actually testing
                ////      whether we fall into an endless loop, while trying to
                ////      generate the file manager ID
            }

            var noReadFS = new ErrorFileSystem(fs, throwOnRead: true, throwOnWrite: false, throwOnList: false);
            using( var manager3 = new ProlificDataFileManager(noReadFS) )
            {
                // since we don't open the files themselves, this still works
                var files = manager3.FindAllFiles(currentFileManagerOnly: false);
                Assert.AreEqual(1, files.Length);
                Assert.True(DataStore.Comparer.Equals(files[0].DataStoreName, fileInfo1.DataStoreName));
            }

            var noReadOrListFS = new ErrorFileSystem(fs, throwOnRead: true, throwOnWrite: false, throwOnList: true);
            using( var manager4 = new ProlificDataFileManager(noReadOrListFS) )
            {
                Assert.Throws<System.IO.IOException>(() => manager4.FindAllFiles(currentFileManagerOnly: false));
                Assert.Throws<System.IO.IOException>(() => manager4.FindLatestFiles(currentFileManagerOnly: false, maxFileAge: TimeSpan.FromTicks(1), maxAppInstanceCount: 1, maxTotalFileSize: 1));
            }
        }
    }
}
