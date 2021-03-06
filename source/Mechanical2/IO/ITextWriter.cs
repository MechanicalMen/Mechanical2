﻿using System;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Writes to a(n abstract) character stream.
    /// </summary>
    public interface ITextWriter
    {
        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        void Close();

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        void Flush();

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
