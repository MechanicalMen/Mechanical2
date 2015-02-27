using System;
using System.Collections.Generic;
using System.Globalization;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    /// <summary>
    /// An immutable substring. Generally faster than a String.Substring calls.
    /// </summary>
    public struct Substring
    {
        #region Public Fields

        /// <summary>
        /// The original string that this instance is a substring of.
        /// </summary>
        public readonly string Origin;

        /// <summary>
        /// The zero-based starting index.
        /// </summary>
        public readonly int StartIndex;

        /// <summary>
        /// The number of characters in the substring.
        /// </summary>
        public readonly int Length;

        #endregion

        #region Constructors, Substr

        private Substring( int startIndex, int length, string origin )
        {
            //// NOTE: fast costructor without validation

            this.Origin = origin;
            this.StartIndex = startIndex;
            this.Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Substring"/> struct.
        /// </summary>
        /// <param name="origin">The original string that this instance is a substring of.</param>
        /// <param name="startIndex">The zero-based starting character position of the substring.</param>
        /// <param name="length">The number of characters in the substring.</param>
        public Substring( string origin, int startIndex, int length )
            : this(startIndex, length, origin)
        {
            if( origin.NullReference() )
            {
                if( startIndex != Substring.Null.StartIndex
                 || length != Substring.Null.Length )
                    throw new ArgumentException().Store("origin", origin).Store("startIndex", startIndex).Store("length", length);
            }
            else
            {
                if( startIndex < 0
                 || length < 0
                 || startIndex + length > origin.Length )
                    throw new ArgumentException().Store("origin", origin).Store("startIndex", startIndex).Store("length", length);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Substring"/> struct.
        /// </summary>
        /// <param name="origin">The original string to be wrapped in a substring.</param>
        public Substring( string origin )
        {
            this.Origin = origin;
            if( this.Origin.NullReference() )
            {
                this.StartIndex = Substring.Null.StartIndex;
                this.Length = Substring.Null.Length;
            }
            else
            {
                this.StartIndex = 0;
                this.Length = origin.Length;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Substring"/> struct.
        /// </summary>
        /// <param name="origin">The original string that this instance is a substring of.</param>
        /// <param name="startIndex">The zero-based starting character position of the substring.</param>
        public Substring( string origin, int startIndex )
        {
            this.Origin = origin;
            if( this.Origin.NullReference() )
            {
                if( startIndex != Substring.Null.StartIndex )
                    throw new ArgumentOutOfRangeException("startIndex").Store("origin", origin).Store("startIndex", startIndex);

                this.StartIndex = Substring.Null.StartIndex;
                this.Length = Substring.Null.Length;
            }
            else
            {
                if( startIndex < 0
                 || startIndex > origin.Length )
                    throw new ArgumentOutOfRangeException("startIndex").Store("origin", origin).Store("startIndex", startIndex);

                this.StartIndex = startIndex;
                this.Length = this.Origin.Length - startIndex;
            }
        }

        #region Substr

        //// NOTE: member names cannot be the same as that of their enclosing types.

        /// <summary>
        /// Retrieves a substring of this instance.
        /// </summary>
        /// <param name="startIndex">The zero-based starting character position of the substring.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <returns>The <see cref="Substring"/> created.</returns>
        public Substring Substr( int startIndex, int length )
        {
            if( this.Origin.NullReference() )
            {
                if( startIndex != Substring.Null.StartIndex
                 || length != Substring.Null.Length )
                    throw new ArgumentException().Store("startIndex", startIndex).Store("length", length);
            }
            else
            {
                if( startIndex < 0
                 || length < 0
                 || startIndex + length > this.Length )
                    throw new ArgumentException().Store("startIndex", startIndex).Store("length", length).Store("Substring.Length", this.Length);
            }

            return new Substring(this.StartIndex + startIndex, length, this.Origin);
        }

        /// <summary>
        /// Retrieves a substring of this instance.
        /// </summary>
        /// <param name="startIndex">The zero-based starting character position of the substring.</param>
        /// <returns>The <see cref="Substring"/> created.</returns>
        public Substring Substr( int startIndex )
        {
            int length;
            if( this.Origin.NullReference() )
            {
                if( startIndex != Substring.Null.StartIndex )
                    throw new ArgumentOutOfRangeException("startIndex").Store("startIndex", startIndex);

                length = Substring.Null.Length;
            }
            else
            {
                if( startIndex < 0
                 || startIndex > this.Length )
                    throw new ArgumentOutOfRangeException("startIndex").Store("startIndex", startIndex);

                length = this.Length - startIndex;
            }

            return new Substring(this.StartIndex + startIndex, length, this.Origin);
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this object represents a <c>null</c> or an empty string.
        /// </summary>
        /// <value><c>true</c> if this object represents a <c>null</c> or an empty string; otherwise <c>false</c>.</value>
        public bool NullOrEmpty
        {
            get
            {
                return this.Origin.NullReference()
                    || this.Length == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object represents a <c>null</c> or an empty string, or consists only of white-space characters.
        /// </summary>
        /// <value><c>true</c> if this object represents a <c>null</c> or an empty string, or consists only of white-space characters; otherwise <c>false</c>.</value>
        public bool NullOrWhiteSpace
        {
            get
            {
                if( this.Origin.NullReference() )
                    return true;

                int till = this.StartIndex + this.Length;
                for( int i = this.StartIndex; i < till; ++i )
                {
                    if( !char.IsWhiteSpace(this.Origin[i]) )
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object represents a <c>null</c> or an empty string, or contains leading or trailing white-space characters.
        /// </summary>
        /// <value><c>true</c> if this object represents a <c>null</c> or an empty string, or contains leading or trailing white-space characters; otherwise <c>false</c>.</value>
        public bool NullOrLengthy
        {
            get
            {
                return this.NullOrEmpty
                    || char.IsWhiteSpace(this.Origin, this.StartIndex)
                    || char.IsWhiteSpace(this.Origin, this.StartIndex + this.Length - 1);
            }
        }

        /// <summary>
        /// Gets the character at the specified position.
        /// </summary>
        /// <param name="index">The position to get the character from.</param>
        /// <returns>The character found.</returns>
        public char this[int index]
        {
            get
            {
                if( index < 0
                 || index >= this.Length )
                    throw new ArgumentOutOfRangeException("index").Store("Origin", this.Origin).Store("StartIndex", this.StartIndex).Store("Length", this.Length).Store("index", index);

                return this.Origin[this.StartIndex + index];
            }
        }

        #endregion

        #region Public Methods

        #region CompareTo

        /// <summary>
        /// Compares this instance to the <paramref name="comparand"/> using the specified comparison options and culture-specific information to influence the comparison, and returns an integer that indicates the relationship of the two strings to each other in the sort order.
        /// </summary>
        /// <param name="comparand">The string to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two substrings.</returns>
        public int CompareTo( string comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            if( comparand.NullReference() )
            {
                if( this.Origin.NullReference() )
                    return 0;
                else
                    return 1;
            }
            else
            {
                if( culture.NullReference() )
                    culture = CultureInfo.CurrentCulture;

                return culture.CompareInfo.Compare(this.Origin, this.StartIndex, this.Length, comparand, 0, comparand.Length, options);
            }
        }

        /// <summary>
        /// Compares this instance to the <paramref name="comparand"/> using the specified comparison options and culture-specific information to influence the comparison, and returns an integer that indicates the relationship of the two strings to each other in the sort order.
        /// </summary>
        /// <param name="comparand">The substring to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two substrings.</returns>
        public int CompareTo( Substring comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            if( culture.NullReference() )
                culture = CultureInfo.CurrentCulture;

            return culture.CompareInfo.Compare(this.Origin, this.StartIndex, this.Length, comparand.Origin, comparand.StartIndex, comparand.Length, options);
        }

        #endregion

        #region Equals

        /// <summary>
        /// Determines whether this instance and the <paramref name="comparand"/> have the same value, using the specified comparison options and culture-specific information to influence the comparison.
        /// </summary>
        /// <param name="comparand">The string to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns><c>true</c> if this instance and the <paramref name="comparand"/> have the same value; otherwise <c>false</c>.</returns>
        public bool Equals( string comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            return this.CompareTo(comparand, options, culture) == 0;
        }

        /// <summary>
        /// Determines whether this instance and the <paramref name="comparand"/> have the same value, using the specified comparison options and culture-specific information to influence the comparison.
        /// </summary>
        /// <param name="comparand">The substring to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns><c>true</c> if this instance and the <paramref name="comparand"/> have the same value; otherwise <c>false</c>.</returns>
        public bool Equals( Substring comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            return this.CompareTo(comparand, options, culture) == 0;
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// Determines whether the beginning of this instance matches the <paramref name="comparand"/>, using the specified comparison options and culture-specific information to influence the comparison.
        /// </summary>
        /// <param name="comparand">The string to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns><c>true</c> if the beginning of this instance matches the <paramref name="comparand"/>; otherwise <c>false</c>.</returns>
        public bool StartsWith( string comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            // the original implementation throws too
            if( comparand.NullReference() )
                throw new ArgumentNullException("comparand").StoreFileLine();

            if( this.Origin.NullReference() )
                return false;

            if( comparand.Length > this.Length )
                return false;
            else if( comparand.Length == this.Length )
                return this.CompareTo(comparand, options, culture) == 0;
            else
                return new Substring(this.StartIndex, comparand.Length, this.Origin).CompareTo(comparand, options, culture) == 0;
        }

        /// <summary>
        /// Determines whether the beginning of this instance matches the <paramref name="comparand"/>, using the specified comparison options and culture-specific information to influence the comparison.
        /// </summary>
        /// <param name="comparand">The substring to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns><c>true</c> if the beginning of this instance matches the <paramref name="comparand"/>; otherwise <c>false</c>.</returns>
        public bool StartsWith( Substring comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            // the original implementation throws too
            if( comparand.Origin.NullReference() )
                throw new ArgumentNullException("comparand").StoreFileLine();

            if( this.Origin.NullReference() )
                return false;

            if( comparand.Length > this.Length )
                return false;
            else if( comparand.Length == this.Length )
                return this.CompareTo(comparand, options, culture) == 0;
            else
                return new Substring(this.StartIndex, comparand.Length, this.Origin).CompareTo(comparand, options, culture) == 0;
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// Determines whether the end of this instance matches the <paramref name="comparand"/>, using the specified comparison options and culture-specific information to influence the comparison.
        /// </summary>
        /// <param name="comparand">The string to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns><c>true</c> if the end of this instance matches the <paramref name="comparand"/>; otherwise <c>false</c>.</returns>
        public bool EndsWith( string comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            // the original implementation throws too
            if( comparand.NullReference() )
                throw new ArgumentNullException("comparand").StoreFileLine();

            if( this.Origin.NullReference() )
                return false;

            if( comparand.Length > this.Length )
                return false;
            else if( comparand.Length == this.Length )
                return this.CompareTo(comparand, options, culture) == 0;
            else
                return new Substring(this.StartIndex + this.Length - comparand.Length, comparand.Length, this.Origin).CompareTo(comparand, options, culture) == 0;
        }

        /// <summary>
        /// Determines whether the end of this instance matches the <paramref name="comparand"/>, using the specified comparison options and culture-specific information to influence the comparison.
        /// </summary>
        /// <param name="comparand">The substring to compare with this instance.</param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. <c>null</c> is interpreted as the current culture.</param>
        /// <returns><c>true</c> if the end of this instance matches the <paramref name="comparand"/>; otherwise <c>false</c>.</returns>
        public bool EndsWith( Substring comparand, CompareOptions options = CompareOptions.None, CultureInfo culture = null )
        {
            // the original implementation throws too
            if( comparand.Origin.NullReference() )
                throw new ArgumentNullException("comparand").StoreFileLine();

            if( this.Origin.NullReference() )
                return false;

            if( comparand.Length > this.Length )
                return false;
            else if( comparand.Length == this.Length )
                return this.CompareTo(comparand, options, culture) == 0;
            else
                return new Substring(this.StartIndex + this.Length - comparand.Length, comparand.Length, this.Origin).CompareTo(comparand, options, culture) == 0;
        }

        #endregion

        #region IndexOf

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current String object. Parameters specify the starting search position in the current string and the type of search to use for the specified string.
        /// </summary>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of the value parameter if that string is found, or <c>-1</c> if it is not. If <paramref name="value"/> is <see cref="string.Empty"/>, the return value is <paramref name="startIndex"/>.</returns>
        public int IndexOf( string value, int startIndex, StringComparison comparisonType )
        {
            if( this.Origin.NullReference() )
                return -1;

            int result = this.Origin.IndexOf(value, this.StartIndex + startIndex, this.Length - startIndex, comparisonType);
            if( result == -1 )
                return result;
            else
                return result - this.StartIndex;
        }

        #endregion

        #region LastIndexOf

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified Unicode character within this instance.
        /// </summary>
        /// <param name="value">The Unicode character to seek.</param>
        /// <returns>The zero-based index position of value if that character is found, or <c>-1</c> if it is not.</returns>
        public int LastIndexOf( char value )
        {
            if( this.Origin.NullReference() )
                return -1;

            for( int i = this.StartIndex + this.Length - 1; i >= this.StartIndex; --i )
            {
                if( this.Origin[i] == value )
                    return i - this.StartIndex;
            }

            return -1;
        }

        #endregion

        #region TrimStart

        /// <summary>
        /// Removes all leading occurrences of a set of characters from this instance.
        /// </summary>
        /// <returns>The substring that remains after all leading occurrences of white-space characters are removed from this instance.</returns>
        public Substring TrimStart()
        {
            return this.TrimStart(null);
        }

        /// <summary>
        /// Removes all leading occurrences of a set of characters from this instance.
        /// </summary>
        /// <param name="trimChars">An array of characters to remove, or <c>null</c>.</param>
        /// <returns>The substring that remains after all leading occurrences of white-space characters are removed from this instance.</returns>
        public Substring TrimStart( params char[] trimChars )
        {
            if( this.NullOrEmpty )
                return this;

            int startIndex = this.StartIndex;
            int till = this.StartIndex + this.Length;

            if( trimChars.NullReference()
             || trimChars.Length == 0 )
            {
                for( ; startIndex < till; ++startIndex )
                {
                    if( !char.IsWhiteSpace(this.Origin[startIndex]) )
                        break;
                }
            }
            else
            {
                int i;
                char ch;

                for( ; startIndex < till; ++startIndex )
                {
                    ch = this.Origin[startIndex];

                    for( i = 0; i < trimChars.Length; ++i )
                    {
                        if( trimChars[i] == ch )
                            break;
                    }

                    if( i == trimChars.Length )
                        break;
                }
            }

            return new Substring(this.Origin, startIndex, till - startIndex);
        }

        #endregion

        #region TrimEnd

        /// <summary>
        /// Removes all trailing occurrences of a set of characters from this instance.
        /// </summary>
        /// <returns>The substring that remains after all trailing occurrences of white-space characters are removed from this instance.</returns>
        public Substring TrimEnd()
        {
            return this.TrimEnd(null);
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of characters from this instance.
        /// </summary>
        /// <param name="trimChars">An array of Unicode characters to remove, or <c>null</c>.</param>
        /// <returns>The substring that remains after all trailing occurrences of white-space characters are removed from this instance.</returns>
        public Substring TrimEnd( params char[] trimChars )
        {
            if( this.NullOrEmpty )
                return this;

            int till = this.StartIndex + this.Length;
            int endIndex = till - 1;

            if( trimChars.NullReference()
             || trimChars.Length == 0 )
            {
                for( ; endIndex >= this.StartIndex; --endIndex )
                {
                    if( !char.IsWhiteSpace(this.Origin[endIndex]) )
                        break;
                }
            }
            else
            {
                int i;
                char ch;

                for( ; endIndex >= this.StartIndex; --endIndex )
                {
                    ch = this.Origin[endIndex];

                    for( i = 0; i < trimChars.Length; ++i )
                    {
                        if( trimChars[i] == ch )
                            break;
                    }

                    if( i == trimChars.Length )
                        break;
                }
            }

            return new Substring(this.Origin, this.StartIndex, endIndex - this.StartIndex + 1);
        }

        #endregion

        #region Trim

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters from this instance.
        /// </summary>
        /// <returns>The substring that remains after all leading and trailing occurrences of white-space characters are removed from this instance.</returns>
        public Substring Trim()
        {
            return this.Trim(null);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters from this instance.
        /// </summary>
        /// <param name="trimChars">An array of Unicode characters to remove, or <c>null</c>.</param>
        /// <returns>The substring that remains after all leading and trailing occurrences of the characters in the <paramref name="trimChars"/> parameter are removed from this instance. If <paramref name="trimChars"/> is <c>null</c> or an empty array, white-space characters are removed instead.</returns>
        public Substring Trim( params char[] trimChars )
        {
            return this.TrimStart(trimChars).TrimEnd(trimChars);
        }

        #endregion

        #region SplitFirst, Split

        /// <summary>
        /// Returns the first substring in <paramref name="substr"/> that is delimited by elements of <paramref name="separator"/>.
        /// </summary>
        /// <param name="substr">The substring to search in, and the rest of the substring not searched yet.</param>
        /// <param name="separator">An array of Unicode characters that delimit the substrings in <paramref name="substr"/>, an empty array that contains no delimiters, or <c>null</c>.</param>
        /// <param name="options"><see cref="StringSplitOptions.RemoveEmptyEntries"/> to omit empty substrings; or <see cref="StringSplitOptions.None"/> to include empty substrings.</param>
        /// <returns>The first substring in <paramref name="substr"/> that is delimited by elements of <paramref name="separator"/>.</returns>
        public static Substring SplitFirst( ref Substring substr, char[] separator = null, StringSplitOptions options = StringSplitOptions.None )
        {
            if( substr.Origin.NullReference() )
                return Substring.Null;

            // should we return empty substrings?
            if( options == StringSplitOptions.None )
            {
                // find the first delimeter
                int delimeterAt = -1;
                int till = substr.StartIndex + substr.Length;
                if( separator.NullReference()
                 || separator.Length == 0 )
                {
                    // delimeters are whitespaces
                    for( int i = substr.StartIndex; i < till; ++i )
                    {
                        if( char.IsWhiteSpace(substr.Origin[i]) )
                        {
                            delimeterAt = i;
                            break;
                        }
                    }
                }
                else
                {
                    for( int i = substr.StartIndex; i < till; ++i )
                    {
                        foreach( char ch in separator )
                        {
                            if( substr.Origin[i] == ch )
                            {
                                delimeterAt = i;
                                i = till;
                                break;
                            }
                        }
                    }
                }

                // any delimeters?
                if( delimeterAt == -1 )
                {
                    var lastPart = substr;
                    substr = Substring.Null;
                    return lastPart;
                }
                else
                {
                    var part = new Substring(substr.StartIndex, delimeterAt - substr.StartIndex, substr.Origin);
                    substr = new Substring(delimeterAt + 1, till - delimeterAt - 1, substr.Origin);
                    return part;
                }
            }
            else if( options == StringSplitOptions.RemoveEmptyEntries )
            {
                Substring part;
                do
                {
                    part = SplitFirst(ref substr, separator, StringSplitOptions.None);
                }
                while( part.Origin.NotNullReference() && part.Length == 0 );

                return part; // null or substring
            }
            else
            {
                throw new ArgumentException().Store("substr", substr).Store("separator", separator).Store("options", options);
            }
        }

        /// <summary>
        /// Returns a <see cref="Substring"/> array that contains the substrings in this instance that are delimited by elements of a specified Unicode character array.
        /// </summary>
        /// <param name="separator">An array of Unicode characters that delimit the substrings in this string, an empty array that contains no delimiters, or <c>null</c>.</param>
        /// <param name="options"><see cref="StringSplitOptions.RemoveEmptyEntries"/> to omit empty substrings; or <see cref="StringSplitOptions.None"/> to include empty substrings.</param>
        /// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more characters in <paramref name="separator"/>.</returns>
        public Substring[] Split( char[] separator = null, StringSplitOptions options = StringSplitOptions.None )
        {
            if( this.Origin.NullReference() )
                throw new NullReferenceException().StoreFileLine();

            var list = new List<Substring>();
            var rest = this;
            var name = SplitFirst(ref rest, separator, options);
            while( name.Origin.NotNullReference() )
            {
                list.Add(name);
                name = SplitFirst(ref rest, separator, options);
            }

            return list.ToArray();
        }

        #endregion

        #region ToString, GetHashCode

        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents this instance.</returns>
        public override string ToString()
        {
            if( this.Origin.NullReference() )
                return null;
            else
                return this.Origin.Substring(this.StartIndex, this.Length);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Substring"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            //// NOTE: This will have a quite bad performance (in performance critical scenarios).
            ////       Unfortunately, the only alternative I could find was CultureInfo.CompareInfo.GetHashCode( string, CompareOptions )
            ////       which is still not good enough for us.
            ////       When dictionaries are in play, it is either recommended to use strings, or a special IEqualityComparer<Substring>

            if( this.Origin.NullReference() )
                return 0;
            else
                return this.Origin.Substring(this.StartIndex, this.Length).GetHashCode();
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Substring"/>.
        /// </summary>
        /// <param name="str">The string to wrap in a <see cref="Substring"/>.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Substring( string str )
        {
            if( str.NullReference() )
                return Substring.Null;
            else
                return new Substring(str);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Substring"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="substr">The substring to convert to a string.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator string( Substring substr )
        {
            // NOTE: explicit, because this type exists to limit Substring(...) calls.
            return substr.ToString();
        }

        #endregion

        #region Public Static Fields

        /// <summary>
        /// A substring of <c>null</c>.
        /// </summary>
        public static readonly Substring Null = new Substring();

        /// <summary>
        /// An empty substring.
        /// </summary>
        public static readonly Substring Empty = new Substring(string.Empty, 0, 0);

        #endregion
    }
}
