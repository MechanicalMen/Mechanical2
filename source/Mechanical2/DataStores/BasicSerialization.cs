using System;
using System.Globalization;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;
using Mechanical.MagicBag;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Default serialization handlers for the most basic data types. When writing serialization, assume
    /// that the current application overrides at least some of then (e.g. DateTime).
    /// Only use these for serialization code that either needs to run before the magic bag is set up,
    /// or needs to run independently of any application using it.
    /// For performance critical scenarios, it's better to cache what you need from a magic bag.
    /// </summary>
    public static class BasicSerialization
    {
        /* NOTE: Some serializers have restrictions on what they accept.
         *       Take a look at their accompanying notes:
         *        - string
         *        - Substring
         *        - DateTime
         */

        #region Static Members

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        internal static Mapping[] GetMappings()
        {
            return new Mapping[]
            {
                Map<IDataStoreValueSerializer<sbyte>>.To(() => BasicSerialization.SByte.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<sbyte>>.To(() => BasicSerialization.SByte.Default).AsTransient(),
                Map<IDataStoreValueSerializer<byte>>.To(() => BasicSerialization.Byte.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<byte>>.To(() => BasicSerialization.Byte.Default).AsTransient(),
                Map<IDataStoreValueSerializer<short>>.To(() => BasicSerialization.Int16.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<short>>.To(() => BasicSerialization.Int16.Default).AsTransient(),
                Map<IDataStoreValueSerializer<ushort>>.To(() => BasicSerialization.UInt16.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<ushort>>.To(() => BasicSerialization.UInt16.Default).AsTransient(),
                Map<IDataStoreValueSerializer<int>>.To(() => BasicSerialization.Int32.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<int>>.To(() => BasicSerialization.Int32.Default).AsTransient(),
                Map<IDataStoreValueSerializer<uint>>.To(() => BasicSerialization.UInt32.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<uint>>.To(() => BasicSerialization.UInt32.Default).AsTransient(),
                Map<IDataStoreValueSerializer<long>>.To(() => BasicSerialization.Int64.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<long>>.To(() => BasicSerialization.Int64.Default).AsTransient(),
                Map<IDataStoreValueSerializer<ulong>>.To(() => BasicSerialization.UInt64.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<ulong>>.To(() => BasicSerialization.UInt64.Default).AsTransient(),
                Map<IDataStoreValueSerializer<float>>.To(() => BasicSerialization.Single.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<float>>.To(() => BasicSerialization.Single.Default).AsTransient(),
                Map<IDataStoreValueSerializer<double>>.To(() => BasicSerialization.Double.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<double>>.To(() => BasicSerialization.Double.Default).AsTransient(),
                Map<IDataStoreValueSerializer<decimal>>.To(() => BasicSerialization.Decimal.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<decimal>>.To(() => BasicSerialization.Decimal.Default).AsTransient(),
                Map<IDataStoreValueSerializer<bool>>.To(() => BasicSerialization.Boolean.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<bool>>.To(() => BasicSerialization.Boolean.Default).AsTransient(),
                Map<IDataStoreValueSerializer<char>>.To(() => BasicSerialization.Char.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<char>>.To(() => BasicSerialization.Char.Default).AsTransient(),
                Map<IDataStoreValueSerializer<string>>.To(() => BasicSerialization.String.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<string>>.To(() => BasicSerialization.String.Default).AsTransient(),
                Map<IDataStoreValueSerializer<Mechanical.Core.Substring>>.To(() => Substring.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<Mechanical.Core.Substring>>.To(() => Substring.Default).AsTransient(),
                Map<IDataStoreValueSerializer<System.DateTime>>.To(() => DateTime.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<System.DateTime>>.To(() => DateTime.Default).AsTransient(),
                Map<IDataStoreValueSerializer<System.TimeSpan>>.To(() => TimeSpan.Default).AsTransient(),
                Map<IDataStoreValueDeserializer<System.TimeSpan>>.To(() => TimeSpan.Default).AsTransient()
            };
        }

        #endregion

        #region SByte

        /// <summary>
        /// Serialization and deserialization for <see cref="sbyte"/>.
        /// </summary>
        public sealed class SByte : IDataStoreValueSerializer<sbyte>,
                                    IDataStoreValueDeserializer<sbyte>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.SByte Default = new BasicSerialization.SByte();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( sbyte obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( sbyte obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public sbyte Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return sbyte.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public sbyte Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadSByte();
            }
        }

        #endregion

        #region Byte

        /// <summary>
        /// Serialization and deserialization for <see cref="byte"/>.
        /// </summary>
        public sealed class Byte : IDataStoreValueSerializer<byte>,
                                   IDataStoreValueDeserializer<byte>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Byte Default = new BasicSerialization.Byte();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( byte obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( byte obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public byte Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return byte.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public byte Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadByte();
            }
        }

        #endregion

        #region Int16

        /// <summary>
        /// Serialization and deserialization for <see cref="short"/>.
        /// </summary>
        public sealed class Int16 : IDataStoreValueSerializer<short>,
                                    IDataStoreValueDeserializer<short>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Int16 Default = new BasicSerialization.Int16();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( short obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( short obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public short Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return short.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public short Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadInt16();
            }
        }

        #endregion

        #region UInt16

        /// <summary>
        /// Serialization and deserialization for <see cref="ushort"/>.
        /// </summary>
        public sealed class UInt16 : IDataStoreValueSerializer<ushort>,
                                     IDataStoreValueDeserializer<ushort>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.UInt16 Default = new BasicSerialization.UInt16();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( ushort obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( ushort obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public ushort Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return ushort.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public ushort Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadUInt16();
            }
        }

        #endregion

        #region Int32

        /// <summary>
        /// Serialization and deserialization for <see cref="int"/>.
        /// </summary>
        public sealed class Int32 : IDataStoreValueSerializer<int>,
                                    IDataStoreValueDeserializer<int>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Int32 Default = new BasicSerialization.Int32();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( int obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( int obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public int Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return int.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public int Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadInt32();
            }
        }

        #endregion

        #region UInt32

        /// <summary>
        /// Serialization and deserialization for <see cref="uint"/>.
        /// </summary>
        public sealed class UInt32 : IDataStoreValueSerializer<uint>,
                                     IDataStoreValueDeserializer<uint>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.UInt32 Default = new BasicSerialization.UInt32();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( uint obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( uint obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public uint Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return uint.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public uint Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadUInt32();
            }
        }

        #endregion

        #region Int64

        /// <summary>
        /// Serialization and deserialization for <see cref="long"/>.
        /// </summary>
        public sealed class Int64 : IDataStoreValueSerializer<long>,
                                    IDataStoreValueDeserializer<long>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Int64 Default = new BasicSerialization.Int64();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( long obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( long obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public long Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return long.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public long Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadInt64();
            }
        }

        #endregion

        #region UInt64

        /// <summary>
        /// Serialization and deserialization for <see cref="ulong"/>.
        /// </summary>
        public sealed class UInt64 : IDataStoreValueSerializer<ulong>,
                                     IDataStoreValueDeserializer<ulong>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.UInt64 Default = new BasicSerialization.UInt64();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( ulong obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("D", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( ulong obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public ulong Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return ulong.Parse(str, NumberStyles.Integer, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public ulong Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadUInt64();
            }
        }

        #endregion

        #region Single

        /// <summary>
        /// Serialization and deserialization for <see cref="float"/>.
        /// </summary>
        public sealed class Single : IDataStoreValueSerializer<float>,
                                     IDataStoreValueDeserializer<float>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Single Default = new BasicSerialization.Single();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( float obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("R", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( float obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public float Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return float.Parse(str, NumberStyles.Float, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public float Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadSingle();
            }
        }

        #endregion

        #region Double

        /// <summary>
        /// Serialization and deserialization for <see cref="double"/>.
        /// </summary>
        public sealed class Double : IDataStoreValueSerializer<double>,
                                     IDataStoreValueDeserializer<double>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Double Default = new BasicSerialization.Double();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( double obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("R", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( double obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public double Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return double.Parse(str, NumberStyles.Float, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public double Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadDouble();
            }
        }

        #endregion

        #region Decimal

        /// <summary>
        /// Serialization and deserialization for <see cref="decimal"/>.
        /// </summary>
        public sealed class Decimal : IDataStoreValueSerializer<decimal>,
                                      IDataStoreValueDeserializer<decimal>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Decimal Default = new BasicSerialization.Decimal();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( decimal obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("F", Culture));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( decimal obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public decimal Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    return decimal.Parse(str, NumberStyles.Float, Culture);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public decimal Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadDecimal();
            }
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Serialization and deserialization for <see cref="bool"/>.
        /// </summary>
        public sealed class Boolean : IDataStoreValueSerializer<bool>,
                                      IDataStoreValueDeserializer<bool>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Boolean Default = new BasicSerialization.Boolean();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( bool obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj ? "true" : "false");
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( bool obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public bool Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                Mechanical.Core.Substring value;
                reader.ReadToEnd(out value);
                if( value.Origin.NullReference() )
                    throw new FormatException().Store("value", value);
                else
                    value = value.Trim();

                if( value.Equals("true", CompareOptions.OrdinalIgnoreCase, Culture) )
                    return true;
                else if( value.Equals("false", CompareOptions.OrdinalIgnoreCase, Culture) )
                    return false;
                else
                    throw new FormatException().Store("value", value);
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public bool Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadBoolean();
            }
        }

        #endregion

        #region Char

        /// <summary>
        /// Serialization and deserialization for <see cref="char"/>.
        /// </summary>
        public sealed class Char : IDataStoreValueSerializer<char>,
                                   IDataStoreValueDeserializer<char>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.Char Default = new BasicSerialization.Char();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( char obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( char obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public char Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                var value = reader.Read();
                if( value == -1 )
                    throw new FormatException("Unexpected end of stream!").StoreFileLine();
                else if( reader.Read() != -1 )
                    throw new FormatException("More than one character in stream!").StoreFileLine();
                else
                    return (char)value;
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public char Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadChar();
            }
        }

        #endregion

        #region String

        /// <summary>
        /// Serialization and deserialization for <see cref="string"/>.
        /// </summary>
        public sealed class String : IDataStoreValueSerializer<string>,
                                     IDataStoreValueDeserializer<string>
        {
            //// NOTE: Handling 'null' strings is left to the stream,
            ////       but our current implementations throw exceptions for them.
            ////       .NET's BinaryWriter throws as well, but StreamWriter
            ////       and StringBuilder simply ignore them.

            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly BasicSerialization.String Default = new BasicSerialization.String();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( string obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( string obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public string Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadToEnd();
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public string Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadString();
            }
        }

        #endregion

        #region Substring

        /// <summary>
        /// Serialization and deserialization for <see cref="Mechanical.Core.Substring"/>.
        /// </summary>
        public sealed class Substring : IDataStoreValueSerializer<Mechanical.Core.Substring>,
                                        IDataStoreValueDeserializer<Mechanical.Core.Substring>
        {
            //// NOTE: Handling 'null' strings is left to the stream,
            ////       but our current implementations throw exceptions for them.
            ////       .NET's BinaryWriter throws as well, but StreamWriter
            ////       and StringBuilder simply ignore them.

            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly Substring Default = new Substring();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( Mechanical.Core.Substring obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( Mechanical.Core.Substring obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public Mechanical.Core.Substring Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                Mechanical.Core.Substring result;
                reader.ReadToEnd(out result);
                return result;
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public Mechanical.Core.Substring Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                return reader.ReadString();
            }
        }

        #endregion

        #region DateTime

        /// <summary>
        /// Serialization and deserialization for <see cref="System.DateTime"/>.
        /// </summary>
        public sealed class DateTime : IDataStoreValueSerializer<System.DateTime>,
                                       IDataStoreValueDeserializer<System.DateTime>
        {
            /* NOTE: Our goal - for default serializers - is, that you should get back the same thing you put into it,
             *       no matter on which platform, or where on earth you are. Unfortunatelly in this
             *       case portability won over comfort.
             *       
             *       There are some serious limitations to how this implementation handles DateTime!
             *         - UTC DateTime is handled correctly
             *         - Local DateTime is converted to UTC (and therefore deserializes as such)
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

            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly DateTime Default = new DateTime();

            internal static System.DateTime ConvertDateTime( System.DateTime obj )
            {
                if( obj.Kind == DateTimeKind.Unspecified )
                    throw new NotSupportedException("DateTimeKind.Unspecified is not supported! Utc is, and Local is converted to Utc.").Store("DateTime", obj);

                return obj.ToUniversalTime();
            }

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( System.DateTime obj, ITextWriter writer )
            {
                obj = ConvertDateTime(obj);

                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("o"));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( System.DateTime obj, IBinaryWriter writer )
            {
                obj = ConvertDateTime(obj);

                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.Ticks);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public System.DateTime Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    // not using the Culture field on purpose!
                    return System.DateTime.ParseExact(str, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public System.DateTime Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                var ticks = reader.ReadInt64();
                return new System.DateTime(ticks, DateTimeKind.Utc);
            }
        }

        #endregion

        #region TimeSpan

        /// <summary>
        /// Serialization and deserialization for <see cref="System.TimeSpan"/>.
        /// </summary>
        public sealed class TimeSpan : IDataStoreValueSerializer<System.TimeSpan>,
                                       IDataStoreValueDeserializer<System.TimeSpan>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly TimeSpan Default = new TimeSpan();

            /// <summary>
            /// Serializes to a text-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( System.TimeSpan obj, ITextWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.ToString("c"));
            }

            /// <summary>
            /// Serializes to a binary-based data store value.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The writer to use.</param>
            public void Serialize( System.TimeSpan obj, IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(obj.Ticks);
            }

            /// <summary>
            /// Deserializes a text-based data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public System.TimeSpan Deserialize( string name, ITextReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                string str = reader.ReadToEnd();
                try
                {
                    // not using the Culture field on purpose!
                    return System.TimeSpan.ParseExact(str, "c", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                }
                catch( Exception ex )
                {
                    ex.Store("str", str);
                    throw;
                }
            }

            /// <summary>
            /// Deserializes a binary data store value.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public System.TimeSpan Deserialize( string name, IBinaryReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                var ticks = reader.ReadInt64();
                return new System.TimeSpan(ticks);
            }
        }

        #endregion
    }
}
