﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private const int MaxNameLength = 255 - 1; // '\0' --> -1
        private const int MaxPathLength = 260 - 3 - 1; // "c:\" --> -3
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

            if( name.Length > MaxNameLength ) // max. NTFS file name length
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

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsValidFirstCharacter( char ch )
        {
            return ('a' <= ch && ch <= 'z')
                || ('A' <= ch && ch <= 'Z')
                || ch == '_';
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsValidMiddleCharacter( char ch )
        {
            return IsValidFirstCharacter(ch)
                || ('0' <= ch && ch <= '9');
        }

        #endregion

        #region GenerateHash, GenerateNames

        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Generates a data store compatible hash string. The generated name may still not be valid
        /// if it is added to a data store object, with a child of the same name.
        /// </summary>
        /// <param name="from">The string to generate the name from deterministically; or <c>null</c> to generate a random hash based name.</param>
        /// <returns>The data store name generated.</returns>
        public static string GenerateHash( string from = null )
        {
            if( !from.NullOrEmpty() )
            {
                // generate deterministic (case-sensitive) hash
                byte[] bytes = DefaultEncoding.GetBytes(from);
                var sb = new StringBuilder(32);
#if !SILVERLIGHT
                bytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes); // MD5 is fine, since we don't worry about cryptographic security, and it's shorter than SHA1 and others
                foreach( var b in bytes )
                    sb.Append(b.ToString("x2"));
#else
                sb.Append(Convert.ToBase64String(bytes), startIndex: 0, count: 32);
#endif
                if( !IsValidFirstCharacter(sb[0]) )
                    sb[0] = '_'; // does not decrease hash deviation (unlike a letter would)
                return sb.ToString();
            }
            else
            {
                // generate random hash
                var guid = Guid.NewGuid().ToString("N");
                if( !IsValidFirstCharacter(guid[0]) )
                    guid = ValidFirstCharacters[Rnd.Next(ValidFirstCharacters.Length)] + guid.Substring(startIndex: 1);
                return guid;
            }
        }

        /// <summary>
        /// Generates unique data store names, starting with the shortest
        /// possible lengths.
        /// </summary>
        /// <returns>The data store names generated.</returns>
        public static IEnumerable<string> GenerateNames()
        {
            var sb = new StringBuilder();
            var digits = new List<IEnumerator<char>>();
            while( true )
            {
                MoveNext(digits);
                if( digits.Count > MaxNameLength )
                    yield break;

                sb.Clear();
                for( int i = 0; i < digits.Count; ++i )
                    sb.Append(digits[i].Current);

                yield return sb.ToString();
            }
        }

        private static IEnumerable<char> GenerateUniqueCharacters( bool firstCharacter )
        {
            char ch;

            for( ch = 'a'; ch <= 'z'; ++ch )
                yield return ch;

            for( ch = 'A'; ch <= 'Z'; ++ch )
                yield return ch;

            if( !firstCharacter )
            {
                for( ch = '0'; ch <= '9'; ++ch )
                    yield return ch;
            }

            yield return '_';
        }

        private static void MoveNext( List<IEnumerator<char>> digits )
        {
            // increment "digits"
            for( int i = digits.Count - 1; i >= 0; --i )
            {
                if( digits[i].MoveNext() )
                {
                    // "digit" increased, nothing to "carry over"
                    return;
                }
                else
                {
                    // no more characters for current "digit":
                    // reset it, and let the previous one be increased
                    digits[i] = GenerateUniqueCharacters(firstCharacter: false).GetEnumerator();
                    digits[i].MoveNext();
                }
            }

            // all "digits" reset: we need an extra digit at the front
            digits.Insert(0, GenerateUniqueCharacters(firstCharacter: true).GetEnumerator());
            digits[0].MoveNext();
        }

        /// <summary>
        /// Generates data store names. Each item (starting with the second),
        /// appends a unique suffix to the end.
        /// </summary>
        /// <param name="from">The string to generate the names from.</param>
        /// <returns>The data store names generated.</returns>
        public static IEnumerable<string> GenerateNames( string from )
        {
            if( from.NullOrEmpty() )
                return GenerateNames();

            from = RemoveSpecialCharacters(from);
            if( from.NullOrEmpty() )
                return GenerateNames(from);

            return GenerateNamesUsing(from);
        }

        private static string RemoveSpecialCharacters( string str )
        {
            //// NOTE: string.Normalize might come in handy here,
            ////       but Silverlight and WinRT do no currently support it

            // do nothing if there is nothing to work with
            if( str.NullOrEmpty() )
                return str;

            // do nothing if already valid...
            str = str.Trim(); // ... except for leading and trailing white space
            if( DataStore.IsValidName(str) )
                return str;

#if !SILVERLIGHT
            // remove diacritics
            str = string.Join(string.Empty, str.Normalize(NormalizationForm.FormD).Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
#else
            //// silverlight is missing String.Normalize and Encoding.ASCII. I know of no simple way to do this, without reimplementing those features.
#endif
            if( str.NullOrEmpty()
             || DataStore.IsValidName(str) )
                return str;

            // remove unsupported characters
            // (replaces ' ' with '_')
            var chars = new List<char>(str.ToCharArray());
            while( chars.Count > 0
                && !IsValidFirstCharacter(chars[0]) )
            {
                if( chars[0] == ' ' )
                {
                    chars[0] = '_';
                    break;
                }
                else
                    chars.RemoveAt(0);
            }
            for( int i = 1; i < chars.Count; )
            {
                if( IsValidMiddleCharacter(chars[i]) )
                {
                    ++i;
                }
                else
                {
                    if( chars[i] == ' ' )
                    {
                        chars[i] = '_';
                        ++i;
                    }
                    else
                        chars.RemoveAt(i);
                }
            }
            return new string(chars.ToArray());
        }

        private static IEnumerable<string> GenerateNamesUsing( string dataStoreName )
        {
            yield return dataStoreName;

            int length, newLength;
            foreach( var suffix in GenerateNames() )
            {
                length = dataStoreName.Length + 1 + suffix.Length;
                if( length > MaxNameLength )
                {
                    newLength = dataStoreName.Length - (length - MaxNameLength);
                    if( newLength <= 0 )
                        yield return suffix; // return suffixes only from this point on
                    else
                        yield return dataStoreName.Substring(0, newLength) + '_' + suffix;
                }

                yield return dataStoreName + '_' + suffix;
            }
        }

        #endregion

        #region EscapeName, UnescapeName, EscapePath, UnescapePath

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void InitializeEscapeBuffer( ref StringBuilder sb, ref bool stringBuilderUsedBefore, Substring str )
        {
            if( !stringBuilderUsedBefore )
            {
                stringBuilderUsedBefore = true;

                if( sb.NullReference() )
                    sb = new StringBuilder();

                sb.Append(str);
            }
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void AppendEscaped( StringBuilder sb, char ch )
        {
            sb.Append(EscapeCharacter);
            sb.Append(((short)ch).ToString("X4", CultureInfo.InvariantCulture));
        }

        private static bool EscapeName( Substring name, ref StringBuilder sb )
        {
            if( name.NullOrEmpty )
                throw new ArgumentException().Store("name", name);

            bool stringBuilderUsed = false;
            char ch;
            for( int i = 0; i < name.Length; ++i )
            {
                ch = name[i];
                if( i == 0 )
                {
                    if( !IsValidFirstCharacter(ch) )
                    {
                        InitializeEscapeBuffer(ref sb, ref stringBuilderUsed, name.Substr(startIndex: 0, length: i));
                        AppendEscaped(sb, ch);
                    }
                    else if( ch == EscapeCharacter )
                    {
                        InitializeEscapeBuffer(ref sb, ref stringBuilderUsed, name.Substr(startIndex: 0, length: i));
                        sb.Append(EscapeCharacter, repeatCount: 2);
                    }
                    else if( stringBuilderUsed )
                    {
                        sb.Append(ch);
                    }
                }
                else
                {
                    if( !IsValidMiddleCharacter(ch) )
                    {
                        InitializeEscapeBuffer(ref sb, ref stringBuilderUsed, name.Substr(startIndex: 0, length: i));
                        AppendEscaped(sb, ch);
                    }
                    else if( ch == EscapeCharacter )
                    {
                        InitializeEscapeBuffer(ref sb, ref stringBuilderUsed, name.Substr(startIndex: 0, length: i));
                        sb.Append(EscapeCharacter, repeatCount: 2);
                    }
                    else if( stringBuilderUsed )
                    {
                        sb.Append(ch);
                    }
                }
            }

            return stringBuilderUsed;
        }

        /// <summary>
        /// Escapes the specified string using valid data store identifier characters.
        /// The result may not be a valid identifier, if it is too long.
        /// </summary>
        /// <param name="name">The string to escape.</param>
        /// <returns>The escaped string. May or may not be a valid data store identifier.</returns>
        public static string EscapeName( string name )
        {
            StringBuilder sb = null;
            var stringBuilderUsed = EscapeName(name, ref sb);
            if( stringBuilderUsed )
                return sb.ToString();
            else
                return name;
        }

        /// <summary>
        /// Escapes the specified string using valid data store path characters.
        /// The result may not be a valid path, if it is too long.
        /// </summary>
        /// <param name="path">The string to escape.</param>
        /// <returns>The escaped string. May or may not be a valid data store path.</returns>
        public static string EscapePath( string path )
        {
            if( path.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( path.Length == 0 )
                return string.Empty;

            if( path.IndexOf(PathSeparator) == -1 )
                return EscapeName(path);

            var sb = new StringBuilder();
            bool stringBuilderUsed;
            Substring remainingPath = path;
            Substring name;
            do
            {
                if( sb.Length != 0 )
                    sb.Append(PathSeparator);

                name = Substring.SplitFirst(ref remainingPath, PathSeparatorArray, StringSplitOptions.None);

                stringBuilderUsed = EscapeName(name, ref sb);
                if( !stringBuilderUsed )
                    sb.Append(name);
            }
            while( !remainingPath.NullOrEmpty );

            return sb.ToString();
        }

        private static bool UnescapeName( Substring name, ref StringBuilder sb )
        {
            if( name.NullOrEmpty )
                throw new ArgumentException().Store("name", name);

            bool stringBuilderUsed = false;
            short value;
            for( int i = 0; i < name.Length; )
            {
                if( name[i] == EscapeCharacter )
                {
                    InitializeEscapeBuffer(ref sb, ref stringBuilderUsed, name.Substr(startIndex: 0, length: i));

                    if( i + 1 < name.Length )
                    {
                        if( name[i + 1] == EscapeCharacter )
                        {
                            sb.Append(EscapeCharacter);
                            i += 2;
                            continue;
                        }
                        else if( i + 4 < name.Length )
                        {
                            var hex = name.Substr(i + 1, 4);
                            if( short.TryParse(hex.ToString(), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out value) )
                            {
                                sb.Append((char)value);
                                i += 5;
                                continue;
                            }
                        }
                    }

                    throw new FormatException().Store("name", name).Store("i", i);
                }
                else
                {
                    if( stringBuilderUsed )
                        sb.Append(name[i]);
                    ++i;
                }
            }

            return stringBuilderUsed;
        }

        /// <summary>
        /// Converts an escaped name to it's original format.
        /// </summary>
        /// <param name="name">The string to decode.</param>
        /// <returns>The original string contents.</returns>
        public static string UnescapeName( string name )
        {
            StringBuilder sb = null;
            var stringBuilderUsed = UnescapeName(name, ref sb);
            if( stringBuilderUsed )
                return sb.ToString();
            else
                return name;
        }

        /// <summary>
        /// Converts an escaped path to it's original format.
        /// </summary>
        /// <param name="path">The string to decode.</param>
        /// <returns>The original string contents.</returns>
        public static string UnescapePath( string path )
        {
            if( path.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( path.Length == 0 )
                return string.Empty;

            if( path.IndexOf(PathSeparator) == -1 )
                return UnescapeName(path);

            var sb = new StringBuilder();
            bool stringBuilderUsed;
            Substring remainingPath = path;
            Substring name;
            do
            {
                if( sb.Length != 0 )
                    sb.Append(PathSeparator);

                name = Substring.SplitFirst(ref remainingPath, PathSeparatorArray, StringSplitOptions.None);
                if( !DataStore.IsValidName(name) )
                    throw new ArgumentException("Not a valid data store path!").Store("path", path);

                stringBuilderUsed = UnescapeName(name, ref sb);
                if( !stringBuilderUsed )
                    sb.Append(name);
            }
            while( !remainingPath.NullOrEmpty );

            return sb.ToString();
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

        // not public, because only the array reference is read-only, and not the actual items.
        internal static readonly char[] PathSeparatorArray = new char[] { PathSeparator };

        #endregion

        #region NameAndPathComparer, Comparer

        //// NOTE: Comparer needs to be able to handle invalid data store names and paths!
        //// NOTE: carefully review code, if Comparer sensitivity changes,
        ////       since Mechanical.IO.FileSystem depends on it as well.

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
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>The text-based data store value representation.</returns>
        public static string ToString<T>( T value, IDataStoreValueSerializer<T> serializer )
        {
            if( serializer.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var writer = StringWriter.Value;
            serializer.Serialize(value, writer);

            var result = writer.ToString();
            writer.Clear();
            return result;
        }

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
                magicBag = AppCore.MagicBag;

            var serializer = magicBag.Pull<IDataStoreValueSerializer<T>>();
            return ToString<T>(value, serializer);
        }

        /// <summary>
        /// Deserializes the specified text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="value">The text-based data store value representation.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The data store name of the value. Ignored by most during deserialization.</param>
        /// <returns>The deserialized value.</returns>
        public static T Parse<T>( Substring value, IDataStoreValueDeserializer<T> deserializer, string name = "a" )
        {
            if( deserializer.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            var reader = StringReader.Value;
            reader.Set(value);

            var result = deserializer.Deserialize(name, reader);
            reader.Set(Substring.Empty);
            return result;
        }

        /// <summary>
        /// Deserializes the specified text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="value">The text-based data store value representation.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <param name="name">The data store name of the value. Ignored by most during deserialization.</param>
        /// <returns>The deserialized value.</returns>
        public static T Parse<T>( Substring value, IMagicBag magicBag = null, string name = "a" )
        {
            if( magicBag.NullReference() )
                magicBag = AppCore.MagicBag;

            var deserializer = magicBag.Pull<IDataStoreValueDeserializer<T>>();
            return Parse<T>(value, deserializer, name);
        }

        /// <summary>
        /// Tries to deserialize the specified text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="value">The text-based data store value representation.</param>
        /// <param name="result">The deserialized value; or the default value of the type, if deserialization failed.</param>
        /// <param name="deserializer">The object handling deserialization.</param>
        /// <param name="name">The data store name of the value. Ignored by most during deserialization.</param>
        /// <returns><c>true</c> if the deserialization succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse<T>( Substring value, out T result, IDataStoreValueDeserializer<T> deserializer, string name = "a" )
        {
            try
            {
                result = Parse<T>(value, deserializer, name);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Tries to deserialize the specified text-based data store value representation.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="value">The text-based data store value representation.</param>
        /// <param name="result">The deserialized value; or the default value of the type, if deserialization failed.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for deserialization; or <c>null</c> for the default magic bag.</param>
        /// <param name="name">The data store name of the value. Ignored by most during deserialization.</param>
        /// <returns><c>true</c> if the deserialization succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse<T>( Substring value, out T result, IMagicBag magicBag = null, string name = "a" )
        {
            try
            {
                result = Parse<T>(value, magicBag, name);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        #endregion

        #region Combine, GetNodeName, GetParentPath, GetPathNames, IsValidPath

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
            if( path.NullOrEmpty )
                throw new ArgumentException().Store("path", path);

            int index = path.LastIndexOf(PathSeparator);
            if( index != -1 )
                result = path.Substr(startIndex: 0, length: index);
            else
                result = Substring.Empty;
        }

        /// <summary>
        /// Gets the name of the first node in the specified path.
        /// </summary>
        /// <param name="path">The data store path to look at, and the remaining part of the path. Empty, if there was </param>
        /// <param name="result">The data store name found.</param>
        public static void GetRootName( ref Substring path, out Substring result )
        {
            if( path.Origin.NullReference() )
                throw new ArgumentException("Not a valid data store path!").Store("path", path);

            if( path.Length == 0 )
                result = Substring.Empty;
            else
                result = Substring.SplitFirst(ref path, PathSeparatorArray, StringSplitOptions.None);
        }

        /// <summary>
        /// Gets the names of the path, from left to right.
        /// </summary>
        /// <param name="path">The data store path to look at.</param>
        /// <returns>The names of the specified path.</returns>
        public static IEnumerable<Substring> GetPathNames( Substring path )
        {
            Substring remainingPath = path;
            Substring name;
            do
            {
                name = Substring.SplitFirst(ref remainingPath, PathSeparatorArray, StringSplitOptions.None);
                if( !DataStore.IsValidName(name) )
                    throw new ArgumentException("Not a valid data store path!").Store("path", path);

                yield return name;
            }
            while( !remainingPath.NullOrEmpty );
        }

        /// <summary>
        /// Determines whether the specified string is a valid data store path.
        /// </summary>
        /// <param name="path">The string to examine.</param>
        /// <returns><c>true</c> if the specified string is a valid data store path; otherwise, <c>false</c>.</returns>
        public static bool IsValidPath( Substring path )
        {
            if( path.Origin.NullReference() )
                return false;

            if( path.Length == 0 )
                return true;

            if( path.Length > MaxPathLength )
                return false;

            Substring remainingPath = path;
            Substring name;
            do
            {
                name = Substring.SplitFirst(ref remainingPath, PathSeparatorArray, StringSplitOptions.None);
                if( !DataStore.IsValidName(name) )
                    return false;
            }
            while( !remainingPath.NullOrEmpty );

            return true;
        }

        #endregion

        //// TODO: unit tests
        //// TODO: binary data store (seekable?! - what about network streams?)
        //// TODO: (unit) test datastores for attempting to read or write multiple roots
    }
}
