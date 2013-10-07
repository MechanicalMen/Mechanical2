using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;
using Mechanical.MagicBag;

namespace Mechanical.DataStores
{
    /// <summary>
    /// A data store reader.
    /// </summary>
    public interface IDataStoreReader
    {
        /// <summary>
        /// Reads the next data store token.
        /// </summary>
        /// <returns><c>false</c> if the end of the data store was reached; otherwise, <c>true</c>.</returns>
        bool Read();

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
        /// Gets the zero-based depth of the current object or value.
        /// </summary>
        /// <value>The zero-based depth of the current object or value.</value>
        int Depth { get; }

        /// <summary>
        /// Gets the absolute path to the current object or value.
        /// </summary>
        /// <value>The absolute path to the current object or value.</value>
        string Path { get; }

        /// <summary>
        /// Gets the reader of a binary value.
        /// There is exactly one - binary or text - reader for each value.
        /// Calling more than once, returns either the same reader, reset to the start of the value; or a new reader, while releasing the resources of the old one.
        /// </summary>
        /// <value>The reader of a binary value.</value>
        IBinaryReader BinaryValueReader { get; }

        /// <summary>
        /// Gets the reader of a text value.
        /// There is exactly one - binary or text - reader for each value.
        /// Calling more than once, returns either the same reader, reset to the start of the value; or a new reader, while releasing the resources of the old one.
        /// </summary>
        /// <value>The reader of a text value.</value>
        ITextReader TextValueReader { get; }

        /// <summary>
        /// Stores debug information about the current state of the reader, into the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to store the state of the reader in.</param>
        void StorePosition( Exception exception );
    }

    /// <content>
    /// <see cref="IDataStoreReader"/> extension methods.
    /// </content>
    public static partial class DataStoresExtensions
    {
        #region ThrowIfNull

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Conditional("DEBUG")]
        private static void ThrowIfNull(
            IDataStoreReader reader,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException("reader").StoreFileLine(filePath, memberName, lineNumber);
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void ThrowIfReadFails(
            IDataStoreReader reader,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            if( !reader.Read() )
                throw new InvalidOperationException("There is nothing more to read!").StoreFileLine(filePath, memberName, lineNumber);
        }

        #endregion

        #region Deserialize/Read( [name,] deserializer )

        /// <summary>
        /// Deserializes the current value of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The expected name of the current value, or <c>null</c> if it is not important.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T Deserialize<T>( this IDataStoreReader reader, IDataStoreValueDeserializer<T> deserializer, string name = null )
        {
            ThrowIfNull(reader);

            try
            {
                if( deserializer.NullReference() )
                    throw new ArgumentNullException("deserializer").StoreFileLine();

                if( name.NotNullReference()
                 && !DataStore.SameNames(name, reader.Name) )
                    throw new FormatException("Name mismatch!").StoreFileLine();

                var token = reader.Token;
                if( token == DataStoreToken.BinaryValue )
                    return deserializer.Deserialize(reader.Name, reader.BinaryValueReader);
                else if( token == DataStoreToken.TextValue )
                    return deserializer.Deserialize(reader.Name, reader.TextValueReader);
                else
                    throw new FormatException("Data store value expected!").StoreFileLine();
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("expectedName", name);
                ex.Store("Path", reader.Path);
                reader.StorePosition(ex);
                throw;
            }
        }

        /// <summary>
        /// Deserializes the current object of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Deserialize<T>( this IDataStoreReader reader, IDataStoreObjectDeserializer<T> deserializer, string name = null )
        {
            ThrowIfNull(reader);

            try
            {
                if( deserializer.NullReference() )
                    throw new ArgumentNullException("deserializer").StoreFileLine();

                if( reader.Token != DataStoreToken.ObjectStart )
                    throw new FormatException("Data store object start expected!").StoreFileLine();

                if( name.NotNullReference()
                 && !DataStore.SameNames(name, reader.Name) )
                    throw new FormatException("Name mismatch!").StoreFileLine();

                // deserialize
                int objDepth = reader.Depth;
                var obj = deserializer.Deserialize(reader);

                // read object to end (in case the deserializer didn't)
                while( !(reader.Token == DataStoreToken.ObjectEnd && reader.Depth == objDepth) )
                {
                    if( !reader.Read() )
                        throw new FormatException("Unexpected end of data store!").StoreFileLine();
                }

                return obj;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("expectedName", name);
                ex.Store("Path", reader.Path);
                reader.StorePosition(ex);
                throw;
            }
        }

        /// <summary>
        /// Deserializes the current value of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The name of the value read.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T Deserialize<T>( this IDataStoreReader reader, IDataStoreValueDeserializer<T> deserializer, out string name )
        {
            ThrowIfNull(reader);

            name = reader.Name;
            return Deserialize<T>(reader, deserializer, name: null);
        }

        /// <summary>
        /// Deserializes the current object of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The name of the object read.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Deserialize<T>( this IDataStoreReader reader, IDataStoreObjectDeserializer<T> deserializer, out string name )
        {
            ThrowIfNull(reader);

            name = reader.Name;
            return Deserialize<T>(reader, deserializer, name: null);
        }

        /// <summary>
        /// Reads the next token, and deserializes the value found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The expected name of the current value, or <c>null</c> if it is not important.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T Read<T>( this IDataStoreReader reader, IDataStoreValueDeserializer<T> deserializer, string name = null )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return Deserialize<T>(reader, deserializer, name);
        }

        /// <summary>
        /// Reads the next token, and deserializes the value found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Read<T>( this IDataStoreReader reader, IDataStoreObjectDeserializer<T> deserializer, string name = null )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return Deserialize<T>(reader, deserializer, name);
        }

        /// <summary>
        /// Reads the next token, and deserializes the value found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The name of the value read.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T Read<T>( this IDataStoreReader reader, IDataStoreValueDeserializer<T> deserializer, out string name )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return Deserialize<T>(reader, deserializer, out name);
        }

        /// <summary>
        /// Reads the next token, and deserializes the value found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The name of the object read.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T Read<T>( this IDataStoreReader reader, IDataStoreObjectDeserializer<T> deserializer, out string name )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return Deserialize<T>(reader, deserializer, out name);
        }

        #endregion

        #region DeserializeAsValue, DeserializeAsObject, ReadAs*

        /// <summary>
        /// Deserializes the current value of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T DeserializeAsValue<T>( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<T>>();
            return Deserialize<T>(reader, deserializer, name);
        }

        /// <summary>
        /// Deserializes the current value of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T DeserializeAsValue<T>( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<T>>();
            return Deserialize<T>(reader, deserializer, out name);
        }

        /// <summary>
        /// Reads the next token, and deserializes the value found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current value, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T ReadAsValue<T>( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return DeserializeAsValue<T>(reader, name, magicBag);
        }

        /// <summary>
        /// Reads the next token, and deserializes the value found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the value read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static T ReadAsValue<T>( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return DeserializeAsValue<T>(reader, out name, magicBag);
        }

        /// <summary>
        /// Deserializes the current object of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T DeserializeAsObject<T>( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreObjectDeserializer<T>>();
            return Deserialize<T>(reader, deserializer, name);
        }

        /// <summary>
        /// Deserializes the current object of the data store.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T DeserializeAsObject<T>( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var deserializer = magicBag.Pull<IDataStoreObjectDeserializer<T>>();
            return Deserialize<T>(reader, deserializer, out name);
        }

        /// <summary>
        /// Reads the next token, and deserializes the object found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T ReadAsObject<T>( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return DeserializeAsObject<T>(reader, name, magicBag);
        }

        /// <summary>
        /// Reads the next token, and deserializes the object found.
        /// </summary>
        /// <typeparam name="T">The type to return an instance of.</typeparam>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static T ReadAsObject<T>( this IDataStoreReader reader, out string name, IMagicBag magicBag = null )
        {
            ThrowIfNull(reader);

            ThrowIfReadFails(reader);
            return DeserializeAsObject<T>(reader, out name, magicBag);
        }

        #endregion

        #region Read basic types

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static sbyte ReadSByte( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static byte ReadByte( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static short ReadInt16( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static ushort ReadUInt16( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static int ReadInt32( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static uint ReadUInt32( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static long ReadInt64( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static ulong ReadUInt64( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static float ReadSingle( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static double ReadDouble( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static decimal ReadDecimal( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static bool ReadBoolean( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static char ReadChar( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static string ReadString( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static DateTime ReadDateTime( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static TimeSpan ReadTimeSpan( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
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

        #region Read nodes

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static Node.IDataStoreValue ReadValueNode( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            return ReadAsValue<Node.IDataStoreValue>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store object.</returns>
        public static Node.IDataStoreObject ReadObjectNode( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            return ReadAsObject<Node.IDataStoreObject>(reader, name, magicBag);
        }

        /// <summary>
        /// Deserializes the current value of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The expected name of the current object, or <c>null</c> if it is not important.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized data store value.</returns>
        public static Node.IDataStoreNode ReadNode( this IDataStoreReader reader, string name = null, IMagicBag magicBag = null )
        {
            ThrowIfNull(reader);

            if( !reader.Read() )
                return null;

            switch( reader.Token )
            {
            case DataStoreToken.DataStoreEnd:
            case DataStoreToken.ObjectEnd:
                return null;

            case DataStoreToken.BinaryValue:
            case DataStoreToken.TextValue:
                return DeserializeAsValue<Node.IDataStoreValue>(reader, name, magicBag);

            case DataStoreToken.ObjectStart:
                return DeserializeAsObject<Node.IDataStoreObject>(reader, name, magicBag);

            default:
                throw new ArgumentException("Invalid token!").Store("Token", reader.Token);
            }
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
        public static T Read<T>( this IDataStoreReader reader, string name, Func<T> func )
        {
            if( func.NullReference() )
                throw new ArgumentNullException("func").StoreFileLine();

            var deserializer = DelegateSerialization<T>.Default;
            deserializer.ReadDelegate = func;
            return Read<T>(reader, deserializer, name);
        }

        /// <summary>
        /// Deserializes the current object of the data store, and moves to the next token.
        /// </summary>
        /// <param name="reader">The <see cref="IDataStoreReader"/> to use.</param>
        /// <param name="name">The name of the object read.</param>
        /// <param name="action">The delegate performing the actual serialization.</param>
        public static void Read( this IDataStoreReader reader, string name, Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException("action").StoreFileLine();

            Read<object>(reader, name, () => { action(); return null; });
        }

        #endregion
    }

    //// TODO: TryReadAs*(string name, ...) // check name; check token: object end or data store end; check token: value or object start
    //// TODO: unit tests for extension methods
}
