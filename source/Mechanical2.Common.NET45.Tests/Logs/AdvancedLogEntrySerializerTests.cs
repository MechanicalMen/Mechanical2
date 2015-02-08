using System;
using System.Linq;
using Mechanical.Collections;
using Mechanical.Common.Logs;
using Mechanical.Core;
using Mechanical.IO.FileSystem;
using Mechanical.Tests;
using NUnit.Framework;

namespace Mechanical.Common.Tests.Logs
{
    [TestFixture]
    public class AdvancedLogEntrySerializerTests
    {
        static AdvancedLogEntrySerializerTests()
        {
            TestApp.Initialize();
        }

        [Test]
        public void NewLogFileTest()
        {
            var fs = new DataStoreObjectFileSystem(escapeFileNames: true);
            Assert.True(fs.GetFileNames().NullOrEmpty());

            var dateTimeProvider = (TestDateTimeProvider)AppCore.MagicBag.Pull<IDateTimeProvider>();
            using( var logger = new AdvancedLogEntrySerializer(fs, null, null, null, null) )
            {
                Assert.AreEqual(1, fs.GetFileNames().Length);

                dateTimeProvider.UtcNow = dateTimeProvider.UtcNow.Add(TimeSpan.FromSeconds(1));
                logger.StartNewLogFile(reason: "test");
                Assert.AreEqual(2, fs.GetFileNames().Length);

                var closedFiles = logger.GetClosedLogFiles().Select(pair => Tuple.Create(pair.Key.DataStoreName, pair.Value.ReadToEnd())).ToArray();
                Assert.AreEqual(1, closedFiles.Length);
                Assert.False(closedFiles[0].Item2.NullOrEmpty()); // log file not empty
            }
        }

        [Test]
        public void BadFileSystemTest()
        {
            var noWriteFS = new ErrorFileSystem(new DataStoreObjectFileSystem(escapeFileNames: true), ErrorFileSystem.ThrowOptions.Write);
            Assert.Throws<System.IO.IOException>(() => new AdvancedLogEntrySerializer(noWriteFS, null, null, null, null));
        }
    }
}
