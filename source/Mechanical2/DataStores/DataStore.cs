using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;
using Mechanical.MagicBag;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Encapsulates generic, data store related functionality.
    /// </summary>
    public static class DataStore
    {
        #region IsValidName

        /// <summary>
        /// Determines whether the specified string is a valid data store name.
        /// </summary>
        /// <param name="name">The string to examine.</param>
        /// <returns><c>true</c> if the specified string is a valid data store name; otherwise, <c>false</c>.</returns>
        public static bool IsValidName( string name )
        {
            if( name.NullOrEmpty() )
                return false;

            if( name.Length > 255 ) // max. NTFS file name length
                return false;

            if( !ValidFirstCharacter(name[0]) )
                return false;

            for( int i = 1; i < name.Length; ++i )
            {
                if( !ValidMiddleCharacter(name[i]) )
                    return false;
            }

            return true;
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool ValidFirstCharacter( char ch )
        {
            return ('a' <= ch && ch <= 'z')
                || ('A' <= ch && ch <= 'Z')
                || ch == '_';
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool ValidMiddleCharacter( char ch )
        {
            return ValidFirstCharacter(ch)
                || ('0' <= ch && ch <= '9');
        }

        #endregion

        #region SameNames

        /// <summary>
        /// Determines whether the two data store names are the same.
        /// For non data store names, the result is undetermined.
        /// </summary>
        /// <param name="name1">The first data store name.</param>
        /// <param name="name2">The second data store name.</param>
        /// <returns><c>true</c>, if the two data store names are the same; otherwise <c>false</c>.</returns>
        public static bool SameNames( string name1, string name2 )
        {
            return string.Equals(name1, name2, StringComparison.Ordinal);
        }

        #endregion

        #region Escape, Unescape

        private const char EscapeCharacter = '_';

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void InitializeEscape( ref StringBuilder sb, string str, int i )
        {
            if( sb.NullReference() )
            {
                sb = new StringBuilder();
                if( i > 0 )
                    sb.Append(str, startIndex: 0, count: i);
            }
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void AppendEscaped( StringBuilder sb, char ch )
        {
            sb.Append(EscapeCharacter);
            sb.Append(((short)ch).ToString("X4", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Escapes the specified string using valid data store identifier characters.
        /// The result may not be a valid identifier, if it is too long.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The escaped string. May or may not be a valid data store identifier.</returns>
        public static string Escape( string str )
        {
            if( str.NullOrEmpty() )
                throw new ArgumentException().Store("str", str);

            StringBuilder sb = null;
            char ch;
            for( int i = 0; i < str.Length; ++i )
            {
                ch = str[i];
                if( i == 0 )
                {
                    if( !ValidFirstCharacter(ch) )
                    {
                        InitializeEscape(ref sb, str, i);
                        AppendEscaped(sb, ch);
                    }
                    else if( ch == EscapeCharacter )
                    {
                        InitializeEscape(ref sb, str, i);
                        sb.Append(EscapeCharacter, repeatCount: 2);
                    }
                    else if( sb.NotNullReference() )
                    {
                        sb.Append(ch);
                    }
                }
                else
                {
                    if( !ValidMiddleCharacter(ch) )
                    {
                        InitializeEscape(ref sb, str, i);
                        AppendEscaped(sb, ch);
                    }
                    else if( ch == EscapeCharacter )
                    {
                        InitializeEscape(ref sb, str, i);
                        sb.Append(EscapeCharacter, repeatCount: 2);
                    }
                    else if( sb.NotNullReference() )
                    {
                        sb.Append(ch);
                    }
                }
            }

            if( sb.NotNullReference() )
                return sb.ToString();
            else
                return str;
        }

        /// <summary>
        /// Converts an escaped string to it's original format.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>The original string contents.</returns>
        public static string Unescape( string str )
        {
            if( str.NullOrEmpty() )
                throw new ArgumentException().Store("str", str);

            StringBuilder sb = null;
            short value;
            for( int i = 0; i < str.Length; )
            {
                if( str[i] == EscapeCharacter )
                {
                    InitializeEscape(ref sb, str, i);

                    if( i + 1 < str.Length )
                    {
                        if( str[i + 1] == EscapeCharacter )
                        {
                            sb.Append(EscapeCharacter);
                            i += 2;
                            continue;
                        }
                        else if( i + 4 < str.Length )
                        {
                            var hex = str.Substring(i + 1, 4);
                            if( short.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out value) )
                            {
                                sb.Append((char)value);
                                i += 5;
                                continue;
                            }
                        }
                    }

                    throw new FormatException().Store("str", str).Store("i", i);
                }
                else
                {
                    if( sb.NotNullReference() )
                        sb.Append(str[i]);
                    ++i;
                }
            }

            if( sb.NotNullReference() )
                return sb.ToString();
            else
                return str;
        }

        #endregion

        #region DefaultEncoding, DefaultNewLine, PathSeparator

        /// <summary>
        /// The default <see cref="Encoding"/> of data stores.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// The default line terminator string of data stores.
        /// </summary>
        public const string DefaultNewLine = "\n";

        /// <summary>
        /// The character separating data store identifiers in a data store path.
        /// </summary>
        public const char PathSeparator = '/';

        #endregion

        #region ToString, [Try]Parse

        private static readonly ThreadLocal<StringWriter> StringWriter = new ThreadLocal<StringWriter>(() => new StringWriter());
        private static readonly ThreadLocal<StringReader> StringReader = new ThreadLocal<StringReader>(() => new StringReader());

        /// <summary>
        /// Serializes the specified value to it's text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The text-based data store value representation.</returns>
        public static string ToString<T>( T value, IMagicBag magicBag = null )
        {
            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var writer = StringWriter.Value;
            var serializer = magicBag.Pull<IDataStoreValueSerializer<T>>();
            serializer.Serialize(value, writer);

            var result = writer.ToString();
            writer.Clear();
            return result;
        }

        /// <summary>
        /// Deserializes the specified text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="value">The text-based data store value representation.</param>
        /// <param name="name">The data store name of the value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns>The deserialized value.</returns>
        public static T Parse<T>( Substring value, string name = "a", IMagicBag magicBag = null )
        {
            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var reader = StringReader.Value;
            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<T>>();
            reader.Set(value);

            var result = deserializer.Deserialize(name, reader);
            reader.Set(Substring.Empty);
            return result;
        }

        /// <summary>
        /// Tries to deserialize the specified text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="value">The text-based data store value representation.</param>
        /// <param name="result">The deserialized value; or the default value of the type, if deserialization failed.</param>
        /// <param name="name">The data store name of the value.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <returns><c>true</c> if the deserialization succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse<T>( Substring value, out T result, string name = "a", IMagicBag magicBag = null )
        {
            try
            {
                result = Parse<T>(value, name, magicBag);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        #endregion

        #region Combine

        /// <summary>
        /// Combines two data store paths. The result is NOT a valid data store name.
        /// </summary>
        /// <param name="str1">The first data store name or path.</param>
        /// <param name="str2">The second data store name or path.</param>
        /// <returns>The combined data store path.</returns>
        public static string Combine( string str1, string str2 )
        {
            return str1 + PathSeparator + str2;
        }

        #endregion

        //// TODO: data store paths
        //// TODO: [un]escape paths
        //// TODO: unit tests
        //// TODO: seekable reader/writer

        //// TODO: binary data store (seekable?! - what about network streams?)
        //// TODO: json data store (do not store number/true/false/null as strings)
        //// TODO: IDataStoreNode & Co. --> default [de]serialization mappings  (instead of reader.ReadNode & Co.)
        //// TODO: test datastores for attempting to read or write multiple roots

        //// TODO: MVVM.UI -> Scheduler, Dispatcher --> MagicBag ?!?! (or at least a synchronizationContext ?!)

        //// TODO: replace StoreDefault with StoreFileLine (resource heavy data should rather be logged separately)
    }
}
