using System;
using System.Collections.Generic;
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

        #region Private Fields

        private const char EscapeCharacter = '_';
        private static readonly char[] ValidFirstCharacters;

        #endregion

        #region Static Constructor

        static DataStore()
        {
            ValidFirstCharacters = GetValidFirstCharacters();
        }

        #endregion

        #region IsValidName

        private static char[] GetValidFirstCharacters()
        {
            var chars = new List<char>();
            for( char ch = 'a'; ch <= 'z'; ++ch )
                chars.Add(ch);
            for( char ch = 'A'; ch <= 'Z'; ++ch )
                chars.Add(ch);
            chars.Add('_');
            return chars.ToArray();
        }

        /// <summary>
        /// Determines whether the specified string is a valid data store name.
        /// </summary>
        /// <param name="name">The string to examine.</param>
        /// <returns><c>true</c> if the specified string is a valid data store name; otherwise, <c>false</c>.</returns>
        public static bool IsValidName( Substring name )
        {
            if( name.NullOrEmpty )
                return false;

            if( name.Length > 255 ) // max. NTFS file name length
                return false;

            if( !IsValidFirstCharacter(name[0]) )
                return false;

            for( int i = 1; i < name.Length; ++i )
            {
                if( !IsValidMiddleCharacter(name[i]) )
                    return false;
            }

            return true;
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsValidFirstCharacter( char ch )
        {
            return ('a' <= ch && ch <= 'z')
                || ('A' <= ch && ch <= 'Z')
                || ch == '_';
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsValidMiddleCharacter( char ch )
        {
            return IsValidFirstCharacter(ch)
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

        #region Escape, Unescape, GenerateName

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
                    if( !IsValidFirstCharacter(ch) )
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
                    if( !IsValidMiddleCharacter(ch) )
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

        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Generates a data store name. The generated name may still not be valid
        /// if it is added to a data store object, with a child of the same name.
        /// </summary>
        /// <param name="from">The string to generate the name from; or <c>null</c> to generate a GUID based name.</param>
        /// <returns>The data store name generated.</returns>
        public static string GenerateName( string from = null )
        {
            if( !from.NullOrEmpty() )
            {
                var escaped = Escape(from);
                if( DataStore.IsValidName(escaped) )
                    return escaped;
            }

            var guid = Guid.NewGuid().ToString("N");
            if( !IsValidFirstCharacter(guid[0]) )
                guid = ValidFirstCharacters[Rnd.Next(ValidFirstCharacters.Length)] + guid.Substring(startIndex: 1);
            return guid;
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

        #region NameAndPathComparer, Comparer

        //// NOTE: Comparer needs to be able to handle invalid data store names and paths!
        //// NOTE: carefully review code, if Comparer sensitivity changes, since some code
        ////       uses it for strict file-data store name, file-file comparisons.

        #region NameAndPathComparer

        /// <summary>
        /// Compares data store names and paths.
        /// </summary>
        public class NameAndPathComparer : IComparer<string>,
                                           IComparer<Substring>,
                                           IEqualityComparer<string>,
                                           IEqualityComparer<Substring>
        {
            /// <summary>
            /// The default instance of the type.
            /// </summary>
            public static readonly NameAndPathComparer Default = new NameAndPathComparer();

            #region IComparer

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
            public int Compare( string x, string y )
            {
                return string.CompareOrdinal(x, y);
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
            public int Compare( Substring x, Substring y )
            {
                return x.CompareTo(y, CompareOptions.Ordinal);
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
            public int Compare( Substring x, string y )
            {
                return x.CompareTo(y, CompareOptions.Ordinal);
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
            public int Compare( string x, Substring y )
            {
                return new Substring(x).CompareTo(y, CompareOptions.Ordinal);
            }

            #endregion

            #region IEqualityComparer

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
            public bool Equals( string x, string y )
            {
                return this.Compare(x, y) == 0;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
            public bool Equals( Substring x, Substring y )
            {
                return this.Compare(x, y) == 0;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
            public bool Equals( Substring x, string y )
            {
                return this.Compare(x, y) == 0;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
            public bool Equals( string x, Substring y )
            {
                return this.Compare(x, y) == 0;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <param name="obj">The object for which a hash code is to be returned.</param>
            /// <returns>A hash code for the specified object.</returns>
            public int GetHashCode( string obj )
            {
                return StringComparer.Ordinal.GetHashCode(obj);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <param name="obj">The object for which a hash code is to be returned.</param>
            /// <returns>A hash code for the specified object.</returns>
            public int GetHashCode( Substring obj )
            {
                return StringComparer.Ordinal.GetHashCode(obj.ToString());
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// The comparer used for data store names and paths.
        /// </summary>
        public static readonly NameAndPathComparer Comparer = NameAndPathComparer.Default;

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
            Substring result;
            GetNodeName(path, out result);
            return result.ToString();
        }

        /// <summary>
        /// Gets the name of the node, the data store path points to.
        /// </summary>
        /// <param name="path">The data store path to look at.</param>
        /// <param name="result">The data store name found.</param>
        public static void GetNodeName( Substring path, out Substring result )
        {
            if( path.NullOrEmpty )
                throw new ArgumentException().Store("path", path);

            int index = path.LastIndexOf(PathSeparator);
            if( index != -1 )
                result = path.Substr(startIndex: index + 1);
            else
                result = path;
        }

        /// <summary>
        /// Gets the path to the parent data store object of the specified node.
        /// </summary>
        /// <param name="path">The data store path to look at.</param>
        /// <returns>The data store path found.</returns>
        public static string GetParentPath( string path )
        {
            Substring result;
            GetParentPath(path, out result);
            return result.ToString();
        }

        /// <summary>
        /// Gets the path to the parent data store object of the specified node.
        /// </summary>
        /// <param name="path">The data store path to look at.</param>
        /// <param name="result">The data store path found.</param>
        public static void GetParentPath( Substring path, out Substring result )
        {
            if( path.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( path.Length != 0 )
            {
                int index = path.LastIndexOf(PathSeparator);
                if( index != -1 )
                {
                    result = path.Substr(startIndex: 0, length: index);
                    return;
                }
            }

            result = Substring.Empty;
        }

        #endregion

        //// TODO: [un]escape paths
        //// TODO: unit tests

        //// TODO: binary data store (seekable?! - what about network streams?)
        //// TODO: json data store (do not store number/true/false/null as strings)
        //// TODO: IDataStoreNode & Co. --> default [de]serialization mappings  (instead of reader.ReadNode & Co.)
        //// TODO: test datastores for attempting to read or write multiple roots
    }
}
