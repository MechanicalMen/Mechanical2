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
        //// NOTE: Data store names have strict restrictions
        ////         - they may not be null
        ////         - they may not be empty
        ////         - they may only contain english letters of either case, digits, or underscores
        ////         - they may not start with a digit
        ////         - they are considered case-sensitive
        ////         - they may be no longer than 255 characters

        //// NOTE: In contrast, data store paths have a few more restrictions:
        ////         - all valid data store names are valid data store paths
        ////         - a path separator may only occur between valid data store names (so paths may not start or end with them, and they may not be next to each other)
        ////       but are also more forgiving:
        ////         - empty strings are valid paths as well (they represent the parent "directory" of the root node)
        ////         - there is no length restriction

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

        #region DefaultEncoding, DefaultNewLine, PathSeparator, Comparer

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

        /// <summary>
        /// The comparer used for data store names and paths.
        /// </summary>
        public static readonly StringComparer Comparer = StringComparer.Ordinal;

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

        #region Combine, GetNodeName, GetParentPath

        /// <summary>
        /// Combines two data store paths. The result is NOT a valid data store name.
        /// </summary>
        /// <param name="path1">The first data store path.</param>
        /// <param name="path2">The second data store path.</param>
        /// <returns>The combined data store path.</returns>
        public static string Combine( string path1, string path2 )
        {
            if( path1.NullReference()
             || path2.NullOrEmpty() )
                throw new ArgumentNullException().Store("path1", path1).Store("path2", path2);

            if( path1.Length == 0 )
                return path2;
            else
                return path1 + PathSeparator + path2;
        }

        /// <summary>
        /// Gets the name of the node, the data store path points to.
        /// </summary>
        /// <param name="path">The data store path to look at.</param>
        /// <returns>The data store name found.</returns>
        public static string GetNodeName( string path )
        {
            if( path.NullOrEmpty() )
                throw new ArgumentException().Store("path", path);

            int index = path.LastIndexOf(PathSeparator);
            if( index != -1 )
                return path.Substring(startIndex: index + 1);
            else
                return path;
        }

        /// <summary>
        /// Gets the path to the parent data store object of the specified node.
        /// </summary>
        /// <param name="path">The data store path to look at.</param>
        /// <returns>The data store path found.</returns>
        public static string GetParentPath( string path )
        {
            if( path.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( path.Length != 0 )
            {
                int index = path.LastIndexOf(PathSeparator);
                if( index != -1 )
                    return path.Substring(startIndex: 0, length: index);
            }

            return path;
        }

        #endregion

        //// TODO: [un]escape paths
        //// TODO: unit tests
        //// TODO: seekable reader/writer

        //// TODO: binary data store (seekable?! - what about network streams?)
        //// TODO: json data store (do not store number/true/false/null as strings)
        //// TODO: IDataStoreNode & Co. --> default [de]serialization mappings  (instead of reader.ReadNode & Co.)
        //// TODO: test datastores for attempting to read or write multiple roots

        //// TODO: refactor IDataStoreReader, DataStoreReaderBase, into a "token reader", and perhaps extension methods to replace them (introduce DataStoreStart token!)
        //// TODO: optimize ensure.debug from datastore reading/writing extension methods (private static method + ConditionalAttribute + Inlining)
    }
}
