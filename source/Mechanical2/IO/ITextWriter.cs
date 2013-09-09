using System;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Writes to a(n abstract) character stream.
    /// </summary>
    public interface ITextWriter
    {
        /// <summary>
        /// Writes the character.
        /// </summary>
        /// <param name="character">The character to write.</param>
        void Write( char character );

        /// <summary>
        /// Writes the character array.
        /// </summary>
        /// <param name="array">The character array to write data from.</param>
        /// <param name="offset">The character position at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        void Write( char[] array, int offset, int count );

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="str">The string to write.</param>
        void Write( string str );

        /// <summary>
        /// Writes the substring.
        /// </summary>
        /// <param name="substr">The substring to write.</param>
        void Write( Substring substr );

        /// <summary>
        /// Writes a line terminator.
        /// </summary>
        void WriteLine();
    }
}
