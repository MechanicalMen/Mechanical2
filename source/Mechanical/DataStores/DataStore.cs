using System;
using System.Text;

namespace Mechanical.DataStores
{
    /// <summary>
    /// Encapsulates generic, data store related functionality.
    /// </summary>
    public static class DataStore
    {
        #region ValidName

        /// <summary>
        /// Determines whether the specified string is a valid data store name.
        /// </summary>
        /// <param name="name">The name to examine.</param>
        /// <returns><c>true</c> if the specified string is a valid data store name; otherwise, <c>false</c>.</returns>
        public static bool ValidName( string name )
        {
            if( string.IsNullOrEmpty(name) )
                return false;

            if( name.Length > 256 )
                return false;

            if( !ValidFirstCharacter(name[0]) )
                return false;

            for( int i = 1; i < name.Length; ++i )
            {
                if( !ValidNonFirstCharacter(name[i]) )
                    return false;
            }

            return true;
        }

        private static bool ValidFirstCharacter( char ch )
        {
            return ('a' <= ch && ch <= 'z')
                || ('A' <= ch && ch <= 'Z')
                || ch == '_';
        }

        private static bool ValidNonFirstCharacter( char ch )
        {
            return ValidFirstCharacter(ch)
                || ('0' <= ch && ch <= '9');
        }

        #endregion

        #region ValidTypeAlias

        /// <summary>
        /// Determines whether the specified string is a valid data store type alias.
        /// </summary>
        /// <param name="typeAlias">The type alias to examine.</param>
        /// <returns><c>true</c> if the specified string is a valid data store type alias; otherwise, <c>false</c>.</returns>
        public static bool ValidTypeAlias( string typeAlias )
        {
            if( string.IsNullOrEmpty(typeAlias) )
                return false;

            foreach( char ch in typeAlias )
            {
                if( (int)ch < 32 || 126 < (int)ch )
                    return false; // non-ASCII, non-printable character
            }

            return true;
        }

        #endregion

        #region NameEquals

        /// <summary>
        /// Determines whether the two data store names have the same value.
        /// For non data store names, the result is undetermined.
        /// </summary>
        /// <param name="name1">The first data store name.</param>
        /// <param name="name2">The second data store name.</param>
        /// <returns><c>true</c>, if the two data store names have the same value; otherwise <c>false</c>.</returns>
        public static bool NameEquals( string name1, string name2 )
        {
            return string.CompareOrdinal(name1, name2) == 0;
        }

        #endregion

        #region TypeAliasEquals

        /// <summary>
        /// Determines whether the two data store type aliases have the same value.
        /// For non data store type aliases, the result is undetermined.
        /// </summary>
        /// <param name="typeAlias1">The first data store type alias.</param>
        /// <param name="typeAlias2">The second data store type alias.</param>
        /// <returns><c>true</c>, if the two data store type aliases have the same value; otherwise <c>false</c>.</returns>
        public static bool TypeAliasEquals( string typeAlias1, string typeAlias2 )
        {
            return string.CompareOrdinal(typeAlias1, typeAlias2) == 0;
        }

        #endregion

        #region Encoding, NewLine, PathSeparator

        /// <summary>
        /// The <see cref="Encoding"/> data stores use.
        /// </summary>
        public static readonly Encoding Encoding = Encoding.UTF8;

        /// <summary>
        /// The line terminator string data stores use.
        /// </summary>
        public const string NewLine = "\n";

        /// <summary>
        /// The character separating data store names in a data store path.
        /// </summary>
        public const char PathSeparator = '/';

        #endregion
    }
}
