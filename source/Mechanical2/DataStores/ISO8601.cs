using System;
using System.Globalization;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Serializes and deserializes <see cref="DateTime"/> and <see cref="TimeSpan"/>. Uses (a subset of) the ISO 8601 format.
    /// Has similar characteristics to the default serializer (returns UTC; Unspecified throws exceptions.)
    /// Sub-second precision will be lost in text format!
    /// Binary format is the same as the default one (and therefore keeps precision).
    /// TimeSpan values must be less than a day (and positive).
    /// </summary>
    public class ISO8601 : IDataStoreValueSerializer<DateTime>,
                           IDataStoreValueDeserializer<DateTime>,
                           IDataStoreValueSerializer<DateTimeOffset>,
                           IDataStoreValueDeserializer<DateTimeOffset>,
                           IDataStoreValueSerializer<TimeSpan>,
                           IDataStoreValueDeserializer<TimeSpan>
    {
        //// NOTE: Some cases do not handle well:
        ////        - DateTime parsing fails on "<some date>T24:00:00Z" (though this is a valid ISO8601 time format)
        ////        - TimeSpan string conversion fails, when the value is less than zero, or greater or equal to a day
        ////          (which is perfectly valid, as long as TimeSpan values refer to the time part of a DateTime - which this serializer is intended for)
        ////        - DateTime string conversion of UTC values ands in "Z", while the same for DateTimeOffset ends in "+00:00"
        ////          (both are valid, but it's inconsistent)

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ISO8601"/> class.
        /// </summary>
        public ISO8601()
        {
        }

        #endregion

        #region DateTime

        private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ssK";

        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( DateTime obj, ITextWriter writer )
        {
            if( obj.Kind == DateTimeKind.Unspecified )
                throw new ArgumentException("DateTimeKind.Unspecified is not supported! Utc is, and Local is converted to Utc.").Store("DateTime", obj);

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            //// NOTE: this will produce different outputs for Local and UTC values.
            writer.Write(obj.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Serializes to a binary-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( DateTime obj, IBinaryWriter writer )
        {
            BasicSerialization.DateTime.Default.Serialize(obj, writer);
        }

        /// <summary>
        /// Deserializes a text-based data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        DateTime IDataStoreValueDeserializer<DateTime>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine();

            return DateTime.ParseExact(reader.ReadToEnd(), DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        /// <summary>
        /// Deserializes a binary data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        DateTime IDataStoreValueDeserializer<DateTime>.Deserialize( string name, IBinaryReader reader )
        {
            return BasicSerialization.DateTime.Default.Deserialize(name, reader);
        }

        #endregion

        #region DateTimeOffset

        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( DateTimeOffset obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            writer.Write(obj.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Serializes to a binary-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( DateTimeOffset obj, IBinaryWriter writer )
        {
            BasicSerialization.DateTimeOffset.Default.Serialize(obj, writer);
        }

        /// <summary>
        /// Deserializes a text-based data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        DateTimeOffset IDataStoreValueDeserializer<DateTimeOffset>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine();

            return DateTimeOffset.ParseExact(reader.ReadToEnd(), DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
        }

        /// <summary>
        /// Deserializes a binary data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        DateTimeOffset IDataStoreValueDeserializer<DateTimeOffset>.Deserialize( string name, IBinaryReader reader )
        {
            return BasicSerialization.DateTimeOffset.Default.Deserialize(name, reader);
        }

        #endregion

        #region TimeSpan

        private const string TimeSpanFormat = "hh':'mm':'ss";

        private static readonly TimeSpan OneDayTimeSpan = TimeSpan.FromDays(1);

        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( TimeSpan obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            if( obj < TimeSpan.Zero
             || obj >= OneDayTimeSpan )
                throw new ArgumentOutOfRangeException().Store("timeSpan", obj);

            // NOTE: string conversion rounds to the lowest seconds, so "00:00:00.999" prints as "00:00:00"!
            writer.Write(obj.ToString(TimeSpanFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Serializes to a binary-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( TimeSpan obj, IBinaryWriter writer )
        {
            BasicSerialization.TimeSpan.Default.Serialize(obj, writer);
        }

        /// <summary>
        /// Deserializes a text-based data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        TimeSpan IDataStoreValueDeserializer<TimeSpan>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine();

            return TimeSpan.ParseExact(reader.ReadToEnd(), TimeSpanFormat, CultureInfo.InvariantCulture, TimeSpanStyles.None);
        }

        /// <summary>
        /// Deserializes a binary data store value.
        /// </summary>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>The deserialized object.</returns>
        TimeSpan IDataStoreValueDeserializer<TimeSpan>.Deserialize( string name, IBinaryReader reader )
        {
            return BasicSerialization.TimeSpan.Default.Deserialize(name, reader);
        }

        #endregion

        /// <summary>
        /// The default instance of the class.
        /// </summary>
        public static readonly ISO8601 Default = new ISO8601();
    }
}
