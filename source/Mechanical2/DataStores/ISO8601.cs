using System;
using System.Globalization;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Serializes and deserializes <see cref="DateTime"/>. Uses the ISO 8601 format.
    /// Has the same characteristics as the default serializer (everything is UTC, Unspecified throws an exception.)
    /// Sub-second precision will be lost! Binary format is the same as the default one (and therefore keeps precision).
    /// TimeSpan can not store more than 24 hours
    /// </summary>
    public class ISO8601 : IDataStoreValueSerializer<DateTime>,
                           IDataStoreValueDeserializer<DateTime>,
                           IDataStoreValueSerializer<TimeSpan>,
                           IDataStoreValueDeserializer<TimeSpan>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ISO8601"/> class.
        /// </summary>
        public ISO8601()
        {
        }

        #endregion

        #region DateTime

        private const string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( DateTime obj, ITextWriter writer )
        {
            obj = BasicSerialization.DateTime.ConvertDateTime(obj);

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

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

            return DateTime.ParseExact(reader.ReadToEnd(), DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.RoundtripKind);
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

        #region TimeSpan

        private const string TimeSpanFormat = "hh':'mm':'ss";

        private static readonly TimeSpan MaxTimeSpan = new TimeSpan(hours: 23, minutes: 59, seconds: 59);

        /// <summary>
        /// Serializes to a text-based data store value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize( TimeSpan obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine();

            if( obj > MaxTimeSpan )
                obj = MaxTimeSpan;

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
