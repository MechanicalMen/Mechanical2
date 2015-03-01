using System;
using System.IO;
using Mechanical.DataStores;
using Mechanical.IO;
using NUnit.Framework;

namespace Mechanical.Tests.DataStores
{
    [TestFixture]
    public class ISO8601Tests
    {
        private static void CheckDateTime( string str, DateTime dt, bool skipStringComparison = false )
        {
            string serialized = DataStore.ToString(dt, ISO8601.Default);
            var deserialized = DataStore.Parse<DateTime>(str, ISO8601.Default);

            if( !skipStringComparison )
                Assert.True(string.Equals(str, serialized, StringComparison.Ordinal));

            dt = dt.ToUniversalTime();
            Assert.AreEqual(dt.Ticks, deserialized.Ticks);
            Assert.AreEqual(dt.Kind, deserialized.Kind);
        }

        private static void CheckDateTimeOffset( string str, DateTimeOffset dto, bool skipStringComparison = false )
        {
            string serialized = DataStore.ToString(dto, ISO8601.Default);
            var deserialized = DataStore.Parse<DateTimeOffset>(str, ISO8601.Default);

            if( !skipStringComparison )
                Assert.True(string.Equals(str, serialized, StringComparison.Ordinal));
            Assert.AreEqual(dto, deserialized);
        }

        private static void CheckTimeSpan( string str, TimeSpan ts, TimeSpan? deserializedComparand = null )
        {
            string serialized = DataStore.ToString(ts, ISO8601.Default);
            var deserialized = DataStore.Parse<TimeSpan>(str, ISO8601.Default);

            if( !deserializedComparand.HasValue )
                deserializedComparand = ts;
            Assert.True(string.Equals(str, serialized, StringComparison.Ordinal));
            Assert.AreEqual(deserializedComparand.Value.Ticks, deserialized.Ticks);
        }

        [Test]
        public void ISO8601DateTimeTests()
        {
            // UTC date and time
            CheckDateTime(
                "2015-02-28T20:16:12Z",
                new DateTime(2015, 02, 28, 20, 16, 12, DateTimeKind.Utc));

            // UTC date and time
            CheckDateTime(
                "2015-02-28T20:16:12+00:00",
                new DateTime(2015, 02, 28, 20, 16, 12, DateTimeKind.Utc),
                skipStringComparison: true); // DateTime serialization produces strings ending in "Z"

            //// NOTE: time zone offset was tested manually, on 2015.03.01
            ////       unfortunately the current time zone can not be set
            ////       on a per-thread or per-appDomain level.

            /*// Local date and time (manual test)
            CheckDateTime(
                "2015-02-28T20:16:12+01:00",
                new DateTime(2015, 02, 28, 20, 16, 12, DateTimeKind.Local), // at the time of testing, time zone offset was +1 hour
                skipStringComparison: false);*/

            // Local date and time (string test skipped)
            CheckDateTime(
                "2015-02-28T20:16:12+03:00",
                new DateTimeOffset(2015, 02, 28, 20, 16, 12, TimeSpan.FromHours(3)).UtcDateTime,
                skipStringComparison: true); // we will convert to UTC before printing, so it will be different by design (in string)

            // sub-second precision is lost in text...
            var originalDateTime = new DateTime(ticks: 635608139293323722L, kind: DateTimeKind.Utc); // 2015-03-01T13:38:49.3323722
            var str = DataStore.ToString(originalDateTime, ISO8601.Default);
            var restoredTextDateTime = DataStore.Parse<DateTime>(str, ISO8601.Default);
            Assert.AreEqual(originalDateTime.Date, restoredTextDateTime.Date);
            Assert.AreEqual(originalDateTime.Hour, restoredTextDateTime.Hour);
            Assert.AreEqual(originalDateTime.Minute, restoredTextDateTime.Minute);
            Assert.AreEqual(originalDateTime.Second, restoredTextDateTime.Second);
            Assert.AreNotEqual(originalDateTime, restoredTextDateTime);

            // ... but preserved in binary
            var ms = new MemoryStream();
            var binaryWriter = IOWrapper.ToBinaryWriter(ms);
            ISO8601.Default.Serialize(originalDateTime, binaryWriter);
            binaryWriter.Close();
            ms = new MemoryStream(ms.ToArray()); // at this point, the first "ms" is disposed
            var binaryReader = IOWrapper.ToBinaryReader(ms);
            var restoredBinaryDateTime = ((IDataStoreValueDeserializer<DateTime>)ISO8601.Default).Deserialize(name: "a", reader: binaryReader);
            binaryReader.Close();
            Assert.AreEqual(originalDateTime, restoredBinaryDateTime);
        }

        [Test]
        public void ISO8601DateTimeOffsetTests()
        {
            // UTC date and time
            CheckDateTimeOffset(
                "2015-02-28T20:16:12+00:00",
                new DateTimeOffset(2015, 02, 28, 20, 16, 12, TimeSpan.Zero),
                skipStringComparison: false);

            // UTC date and time
            CheckDateTimeOffset(
                "2015-02-28T20:16:12Z",
                new DateTimeOffset(2015, 02, 28, 20, 16, 12, TimeSpan.Zero),
                skipStringComparison: true); // DateTimeOffset serialization produces strings ending in "+00:00"

            // Local date and time
            CheckDateTimeOffset(
                "2015-02-28T20:16:12+03:00",
                new DateTimeOffset(2015, 02, 28, 20, 16, 12, TimeSpan.FromHours(3)),
                skipStringComparison: false);

            // sub-second precision is lost in text...
            var originalDateTimeOffset = new DateTimeOffset(ticks: 635608139293323722L, offset: TimeSpan.Zero); // 2015-03-01T13:38:49.3323722
            var str = DataStore.ToString(originalDateTimeOffset, ISO8601.Default);
            var restoredTextDateTimeOffset = DataStore.Parse<DateTimeOffset>(str, ISO8601.Default);
            Assert.AreEqual(originalDateTimeOffset.Date, restoredTextDateTimeOffset.Date);
            Assert.AreEqual(originalDateTimeOffset.Offset, restoredTextDateTimeOffset.Offset);
            Assert.AreEqual(originalDateTimeOffset.Hour, restoredTextDateTimeOffset.Hour);
            Assert.AreEqual(originalDateTimeOffset.Minute, restoredTextDateTimeOffset.Minute);
            Assert.AreEqual(originalDateTimeOffset.Second, restoredTextDateTimeOffset.Second);
            Assert.AreNotEqual(originalDateTimeOffset, restoredTextDateTimeOffset);

            // ... but preserved in binary
            var ms = new MemoryStream();
            var binaryWriter = IOWrapper.ToBinaryWriter(ms);
            ISO8601.Default.Serialize(originalDateTimeOffset, binaryWriter);
            binaryWriter.Close();
            ms = new MemoryStream(ms.ToArray()); // at this point, the first "ms" is disposed
            var binaryReader = IOWrapper.ToBinaryReader(ms);
            var restoredBinaryDateTimeOffset = ((IDataStoreValueDeserializer<DateTimeOffset>)ISO8601.Default).Deserialize(name: "a", reader: binaryReader);
            binaryReader.Close();
            Assert.AreEqual(originalDateTimeOffset, restoredBinaryDateTimeOffset);
        }

        [Test]
        public void ISO8601TimeSpanTests()
        {
            CheckTimeSpan(
                "20:16:12",
                new TimeSpan(20, 16, 12));

            CheckTimeSpan(
                "00:00:00",
                TimeSpan.Zero);

            CheckTimeSpan(
                "00:00:00",
                TimeSpan.FromSeconds(0.999),
                deserializedComparand: TimeSpan.Zero);

            // everything less than a day (and positive) should be printable
            CheckTimeSpan(
                "23:59:59",
                TimeSpan.FromDays(1) - TimeSpan.FromTicks(1),
                deserializedComparand: new TimeSpan(23, 59, 59));

            Assert.Throws<ArgumentOutOfRangeException>(() => DataStore.ToString(TimeSpan.FromDays(1), ISO8601.Default));
            Assert.Throws<ArgumentOutOfRangeException>(() => DataStore.ToString(TimeSpan.FromTicks(-1), ISO8601.Default));
        }
    }
}
