using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.DataStores
{
    /// <summary>
    /// A data store reader.
    /// </summary>
    public interface IDataStoreReader
    {
        /// <summary>
        /// Gets the current data store token.
        /// </summary>
        /// <value>The current data store token.</value>
        DataStoreToken Token { get; }

        /// <summary>
        /// Gets the name of the current object or value.
        /// </summary>
        /// <value>The name of the current object or value.</value>
        string Name { get; }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store value.</returns>
        T Read<T>( IDataStoreValueDeserializer<T> deserializer );

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store object.</returns>
        T Read<T>( IDataStoreObjectDeserializer<T> deserializer );
    }

    /// <content>
    /// <see cref="IDataStoreReader"/> extension methods.
    /// </content>
    public static partial class DataStoresExtensions
    {
        #region Read( name, deserializer )

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T Read<T>( this IDataStoreReader reader, string name, IDataStoreValueDeserializer<T> deserializer )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( !DataStore.SameNames(name, reader.Name) )
                throw new FormatException("Name mismatch!").Store("expectedName", name).Store("actualName", reader.Name);

            return reader.Read<T>(deserializer);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Read<T>( this IDataStoreReader reader, string name, IDataStoreObjectDeserializer<T> deserializer )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( !DataStore.SameNames(name, reader.Name) )
                throw new FormatException("Name mismatch!").Store("expectedName", name).Store("actualName", reader.Name);

            return reader.Read<T>(deserializer);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T Read<T>( this IDataStoreReader reader, out string name, IDataStoreValueDeserializer<T> deserializer )
        {
            Ensure.Debug(reader, r => r.NotNull());

            name = reader.Name;
            return reader.Read<T>(deserializer);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Read<T>( this IDataStoreReader reader, out string name, IDataStoreObjectDeserializer<T> deserializer )
        {
            Ensure.Debug(reader, r => r.NotNull());

            name = reader.Name;
            return reader.Read<T>(deserializer);
        }

        #endregion

        #region ReadAsValue, ReadAsObject

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T ReadAsValue<T>( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<T>>();
            return reader.Read<T>(name, deserializer);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T ReadAsValue<T>( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<T>>();
            return reader.Read<T>(out name, deserializer);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T ReadAsObject<T>( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreObjectDeserializer<T>>();
            return reader.Read<T>(name, deserializer);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T ReadAsObject<T>( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreObjectDeserializer<T>>();
            return reader.Read<T>(out name, deserializer);
        }

        #endregion

        #region Basic types

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static sbyte ReadSByte( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<sbyte>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static sbyte ReadSByte( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<sbyte>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static byte ReadByte( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<byte>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static byte ReadByte( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<byte>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static short ReadInt16( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<short>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static short ReadInt16( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<short>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static ushort ReadUInt16( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<ushort>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static ushort ReadUInt16( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<ushort>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static int ReadInt32( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<int>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static int ReadInt32( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<int>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static uint ReadUInt32( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<uint>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static uint ReadUInt32( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<uint>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static long ReadInt64( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<long>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static long ReadInt64( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<long>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static ulong ReadUInt64( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<ulong>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static ulong ReadUInt64( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<ulong>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static float ReadSingle( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<float>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static float ReadSingle( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<float>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static double ReadDouble( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<double>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static double ReadDouble( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<double>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static decimal ReadDecimal( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<decimal>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static decimal ReadDecimal( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<decimal>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static bool ReadBoolean( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<bool>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static bool ReadBoolean( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<bool>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static char ReadChar( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<char>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static char ReadChar( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<char>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static string ReadString( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<string>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static string ReadString( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<string>(reader, out name, magicBag);
        }

        //// NOTE: Substring is not here, because you should only use it in speed-critical scenarios, and the magic bag defeats that purpose.

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static DateTime ReadDateTime( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<DateTime>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static DateTime ReadDateTime( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<DateTime>(reader, out name, magicBag);
        }


        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static TimeSpan ReadTimeSpan( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<TimeSpan>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static TimeSpan ReadTimeSpan( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            return ReadAsValue<TimeSpan>(reader, out name, magicBag);
        }

        #endregion

        #region Nodes

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static Node.IDataStoreValue ReadValueNode( this IDataStoreReader reader, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<Node.IDataStoreValue>>();
            return reader.Read<Node.IDataStoreValue>(deserializer);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static Node.IDataStoreValue ReadValueNode( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( !DataStore.SameNames(name, reader.Name) )
                throw new FormatException("Name mismatch!").Store("expectedName", name).Store("actualName", reader.Name);

            return ReadValueNode(reader, magicBag);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static Node.IDataStoreObject ReadObjectNode( this IDataStoreReader reader, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreObjectDeserializer<Node.IDataStoreObject>>();
            return reader.Read<Node.IDataStoreObject>(deserializer);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static Node.IDataStoreObject ReadObjectNode( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( !DataStore.SameNames(name, reader.Name) )
                throw new FormatException("Name mismatch!").Store("expectedName", name).Store("actualName", reader.Name);

            return ReadObjectNode(reader, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static Node.IDataStoreNode ReadNode( this IDataStoreReader reader, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( reader.Token == DataStoreToken.DataStoreEnd
             || reader.Token == DataStoreToken.ObjectEnd )
                return null;
            else if( reader.Token == DataStoreToken.Value )
                return ReadValueNode(reader, magicBag);
            else if( reader.Token == DataStoreToken.ObjectStart )
                return ReadObjectNode(reader, magicBag);
            else
                throw new ArgumentException("Invalid token!").Store("reader.Token", reader.Token);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static Node.IDataStoreNode ReadNode( this IDataStoreReader reader, string name, IMagicBag magicBag = null )
        {
            Ensure.Debug(reader, r => r.NotNull());

            if( reader.Token == DataStoreToken.DataStoreEnd
             || reader.Token == DataStoreToken.ObjectEnd )
                return null;
            else if( reader.Token == DataStoreToken.Value )
                return ReadValueNode(reader, name, magicBag);
            else if( reader.Token == DataStoreToken.ObjectStart )
                return ReadObjectNode(reader, name, magicBag);
            else
                throw new ArgumentException("Invalid token!").Store("reader.Token", reader.Token);
        }

        #endregion

        #region Delegate

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="func">The delegate performing the actual serialization.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Read<T>( this IDataStoreReader reader, string name, Func<IDataStoreReader, T> func )
        {
            Ensure.Debug(reader, w => w.NotNull());

            if( func.NullReference() )
                throw new ArgumentNullException("func").StoreFileLine();

            var deserializer = DelegateSerialization<T>.Default;
            deserializer.ReadDelegate = func;
            return reader.Read<T>(name, deserializer);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="action">The delegate performing the actual serialization.</param>
        public static void Read( this IDataStoreReader reader, string name, Action<IDataStoreReader> action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException("action").StoreFileLine();

            Read<object>(reader, name, r => { action(r); return null; });
        }

        #endregion
    }

    //// TODO: TryReadAs*(string name, ...) // check name; check token: object end or data store end; check token: value or object start
    //// TODO: unit tests for extension methods
}
