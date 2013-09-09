using System;
using System.Globalization;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;
using Mechanical.MagicBag;

namespace Mechanical.DataStores
{
    //// NOTE: DateTime is not a "built in type", it was just convenient to put it here.
    ////       There are some important notes in it's region, you might want to read them, if you haven't already.

    internal class BasicSerialization : IDataStoreValueSerializer<sbyte>,
                                        IDataStoreValueDeserializer<sbyte>,
                                        IDataStoreValueSerializer<byte>,
                                        IDataStoreValueDeserializer<byte>,
                                        IDataStoreValueSerializer<short>,
                                        IDataStoreValueDeserializer<short>,
                                        IDataStoreValueSerializer<ushort>,
                                        IDataStoreValueDeserializer<ushort>,
                                        IDataStoreValueSerializer<int>,
                                        IDataStoreValueDeserializer<int>,
                                        IDataStoreValueSerializer<uint>,
                                        IDataStoreValueDeserializer<uint>,
                                        IDataStoreValueSerializer<long>,
                                        IDataStoreValueDeserializer<long>,
                                        IDataStoreValueSerializer<ulong>,
                                        IDataStoreValueDeserializer<ulong>,
                                        IDataStoreValueSerializer<float>,
                                        IDataStoreValueDeserializer<float>,
                                        IDataStoreValueSerializer<double>,
                                        IDataStoreValueDeserializer<double>,
                                        IDataStoreValueSerializer<decimal>,
                                        IDataStoreValueDeserializer<decimal>,
                                        IDataStoreValueSerializer<bool>,
                                        IDataStoreValueDeserializer<bool>,
                                        IDataStoreValueSerializer<char>,
                                        IDataStoreValueDeserializer<char>,
                                        IDataStoreValueSerializer<string>,
                                        IDataStoreValueDeserializer<string>,
                                        IDataStoreValueSerializer<Substring>,
                                        IDataStoreValueDeserializer<Substring>,
                                        IDataStoreValueSerializer<DateTime>,
                                        IDataStoreValueDeserializer<DateTime>,
                                        IDataStoreValueSerializer<TimeSpan>,
                                        IDataStoreValueDeserializer<TimeSpan>
    {
        #region Static Members

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        internal static readonly BasicSerialization Default = new BasicSerialization();

        internal static Mapping[] GetMappings()
        {
            return new Mapping[]
            {
                Map<IDataStoreValueSerializer<sbyte>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<sbyte>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<byte>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<byte>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<short>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<short>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<ushort>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<ushort>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<int>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<int>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<uint>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<uint>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<long>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<long>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<ulong>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<ulong>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<float>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<float>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<double>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<double>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<decimal>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<decimal>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<bool>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<bool>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<char>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<char>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<string>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<string>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<DateTime>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<DateTime>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueSerializer<TimeSpan>>.To(() => Default).AsTransient(),
                Map<IDataStoreValueDeserializer<TimeSpan>>.To(() => Default).AsTransient()
            };
        }

        #endregion

        #region SByte

        void IDataStoreValueSerializer<sbyte>.Serialize( sbyte obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<sbyte>.Serialize( sbyte obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        sbyte IDataStoreValueDeserializer<sbyte>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return sbyte.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        sbyte IDataStoreValueDeserializer<sbyte>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadSByte();
        }

        #endregion

        #region Byte

        void IDataStoreValueSerializer<byte>.Serialize( byte obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<byte>.Serialize( byte obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        byte IDataStoreValueDeserializer<byte>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return byte.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        byte IDataStoreValueDeserializer<byte>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadByte();
        }

        #endregion

        #region Int16

        void IDataStoreValueSerializer<short>.Serialize( short obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<short>.Serialize( short obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        short IDataStoreValueDeserializer<short>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return short.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        short IDataStoreValueDeserializer<short>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadInt16();
        }

        #endregion

        #region UInt16

        void IDataStoreValueSerializer<ushort>.Serialize( ushort obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<ushort>.Serialize( ushort obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        ushort IDataStoreValueDeserializer<ushort>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return ushort.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        ushort IDataStoreValueDeserializer<ushort>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadUInt16();
        }

        #endregion

        #region Int32

        void IDataStoreValueSerializer<int>.Serialize( int obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<int>.Serialize( int obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        int IDataStoreValueDeserializer<int>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return int.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        int IDataStoreValueDeserializer<int>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadInt32();
        }

        #endregion

        #region UInt32

        void IDataStoreValueSerializer<uint>.Serialize( uint obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<uint>.Serialize( uint obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        uint IDataStoreValueDeserializer<uint>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return uint.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        uint IDataStoreValueDeserializer<uint>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadUInt32();
        }

        #endregion

        #region Int64

        void IDataStoreValueSerializer<long>.Serialize( long obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<long>.Serialize( long obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        long IDataStoreValueDeserializer<long>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return long.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        long IDataStoreValueDeserializer<long>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadInt64();
        }

        #endregion

        #region UInt64

        void IDataStoreValueSerializer<ulong>.Serialize( ulong obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("D", Culture));
        }

        void IDataStoreValueSerializer<ulong>.Serialize( ulong obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        ulong IDataStoreValueDeserializer<ulong>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return ulong.Parse(reader.ReadToEnd(), NumberStyles.Integer, Culture);
        }

        ulong IDataStoreValueDeserializer<ulong>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadUInt64();
        }

        #endregion

        #region Single

        void IDataStoreValueSerializer<float>.Serialize( float obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("R", Culture));
        }

        void IDataStoreValueSerializer<float>.Serialize( float obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        float IDataStoreValueDeserializer<float>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return float.Parse(reader.ReadToEnd(), NumberStyles.Float, Culture);
        }

        float IDataStoreValueDeserializer<float>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadSingle();
        }

        #endregion

        #region Double

        void IDataStoreValueSerializer<double>.Serialize( double obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("R", Culture));
        }

        void IDataStoreValueSerializer<double>.Serialize( double obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        double IDataStoreValueDeserializer<double>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return double.Parse(reader.ReadToEnd(), NumberStyles.Float, Culture);
        }

        double IDataStoreValueDeserializer<double>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadDouble();
        }

        #endregion

        #region Decimal

        void IDataStoreValueSerializer<decimal>.Serialize( decimal obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("F", Culture));
        }

        void IDataStoreValueSerializer<decimal>.Serialize( decimal obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        decimal IDataStoreValueDeserializer<decimal>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return decimal.Parse(reader.ReadToEnd(), NumberStyles.Float, Culture);
        }

        decimal IDataStoreValueDeserializer<decimal>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadDecimal();
        }

        #endregion

        #region Boolean

        void IDataStoreValueSerializer<bool>.Serialize( bool obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj ? "true" : "false");
        }

        void IDataStoreValueSerializer<bool>.Serialize( bool obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        bool IDataStoreValueDeserializer<bool>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            string value = reader.ReadToEnd();
            if( value.NullReference() )
                throw new FormatException().Store("value", value);
            else
                value = value.Trim();

            if( string.Equals("true", value, StringComparison.OrdinalIgnoreCase) )
                return true;
            else if( string.Equals("false", value, StringComparison.OrdinalIgnoreCase) )
                return false;
            else
                throw new FormatException().Store("value", value);
        }

        bool IDataStoreValueDeserializer<bool>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadBoolean();
        }

        #endregion

        #region Char

        void IDataStoreValueSerializer<char>.Serialize( char obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        void IDataStoreValueSerializer<char>.Serialize( char obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        char IDataStoreValueDeserializer<char>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            var value = reader.Read();
            if( value == -1 )
                throw new FormatException("Unexpected end of stream!").StoreDefault();
            else if( reader.Read() != -1 )
                throw new FormatException("Unexpected character(s) in stream!").StoreDefault();
            else
                return (char)value;
        }

        char IDataStoreValueDeserializer<char>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadChar();
        }

        #endregion

        #region String

        void IDataStoreValueSerializer<string>.Serialize( string obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        void IDataStoreValueSerializer<string>.Serialize( string obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        string IDataStoreValueDeserializer<string>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadToEnd();
        }

        string IDataStoreValueDeserializer<string>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadString();
        }

        #endregion

        #region Substring

        void IDataStoreValueSerializer<Substring>.Serialize( Substring obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        void IDataStoreValueSerializer<Substring>.Serialize( Substring obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj);
        }

        Substring IDataStoreValueDeserializer<Substring>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            Substring result;
            reader.ReadToEnd(out result);
            return result;
        }

        Substring IDataStoreValueDeserializer<Substring>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            return reader.ReadString();
        }

        #endregion

        #region DateTime

        /* NOTE: Our goal - for default serializers - is, that you should get back the same thing you put into it,
         *       no matter what on what platform, or where on earth you are. Unfortunatelly in this
         *       case portability won over comfort.
         *       
         *       There are some serious limitations to how this implementation handles DateTime!
         *         - UTC DateTime is handled correctly
         *         - Local DateTime is converted to UTC
         *         - Unspecified DateTime results in an exception (at serialization)
         *         
         *       Reason: With Local values, you may read it in a different time zone as the one it was serialized in.
         *       Saving the time zone offset and returning UTC only when it differs from the host offset
         *       may be acceptable (though unpredictable), but the offset alone is ambiguous. Saving time zone information (for each call)
         *       may not be efficient, and would require (or at the very least suggest) a new type, and this is
         *       not something we want to deal with. (search for "Noda Time" instead)
         *               As for Unspecified values: ToLocalTime assumes it's in UTC, ToUniversalTime assumes it's
         *       in local time. We think you should be more specific :)
         */

        internal static DateTime ConvertDateTime( DateTime obj )
        {
            if( obj.Kind == DateTimeKind.Unspecified )
                throw new NotSupportedException("DateTimeKind.Unspecified is not supported! Utc is, and Local is converted to Utc.").Store("DateTime", obj);

            return obj.ToUniversalTime();
        }

        void IDataStoreValueSerializer<DateTime>.Serialize( DateTime obj, ITextWriter writer )
        {
            obj = ConvertDateTime(obj);

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("o"));
        }

        void IDataStoreValueSerializer<DateTime>.Serialize( DateTime obj, IBinaryWriter writer )
        {
            obj = ConvertDateTime(obj);

            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.Ticks);
        }

        DateTime IDataStoreValueDeserializer<DateTime>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            // not using the Culture field on purpose!
            return DateTime.ParseExact(reader.ReadToEnd(), "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
        }

        DateTime IDataStoreValueDeserializer<DateTime>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            var ticks = reader.ReadInt64();
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        #endregion

        #region TimeSpan

        void IDataStoreValueSerializer<TimeSpan>.Serialize( TimeSpan obj, ITextWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.ToString("c"));
        }

        void IDataStoreValueSerializer<TimeSpan>.Serialize( TimeSpan obj, IBinaryWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreDefault();

            writer.Write(obj.Ticks);
        }

        TimeSpan IDataStoreValueDeserializer<TimeSpan>.Deserialize( string name, ITextReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            // not using the Culture field on purpose!
            return TimeSpan.ParseExact(reader.ReadToEnd(), "c", CultureInfo.InvariantCulture, TimeSpanStyles.None);
        }

        TimeSpan IDataStoreValueDeserializer<TimeSpan>.Deserialize( string name, IBinaryReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreDefault();

            var ticks = reader.ReadInt64();
            return new TimeSpan(ticks);
        }

        #endregion
    }
}
