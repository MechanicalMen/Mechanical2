using System;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Reads from a(n abstract) character stream.
    /// </summary>
    public interface ITextReader
    {
        /// <summary>
        /// Returns the next available character, without actually reading it.
        /// </summary>
        /// <returns>An integer representing the next character to be read, or <c>-1</c> if no more characters are available or the reader does not support seeking.</returns>
        int Peek();

        /// <summary>
        /// Reads the next character.
        /// </summary>
        /// <returns>The next character, or <c>-1</c> if no more characters are available.</returns>
        int Read();

        /// <summary>
        /// Reads a specified maximum number of characters, and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> - 1) replaced by the characters read.</param>
        /// <param name="index">The position in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the stream is reached before the specified number of characters is read into the buffer, the method returns.</param>
        /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether the data is available within the stream. This method returns <c>0</c> (zero) if it is called when no more characters are left to read.</returns>
        int Read( char[] buffer, int index, int count );

        /// <summary>
        /// Reads a specified maximum number of characters.
        /// </summary>
        /// <param name="count">The maximum number of characters to read. If the end of the stream is reached before the specified number of characters is read, the method returns.</param>
        /// <returns>The characters read. The number of characters that have been read will be less than or equal to <paramref name="count"/>, depending on whether the data is available within the stream. This method returns <see cref="Substring.Empty"/> if it is called when no more characters are left to read.</returns>
        Substring Read( int count );

        /// <summary>
        /// Reads a line of characters.
        /// </summary>
        /// <returns>The next line from the stream, or <c>null</c> if all characters have been read.</returns>
        string ReadLine();

        /// <summary>
        /// Reads a line of characters.
        /// </summary>
        /// <param name="substr">The next line from the stream, or <see cref="Substring.Null"/> if all characters have been read.</param>
        void ReadLine( out Substring substr );

        /// <summary>
        /// Reads all characters from the current position to the end of the stream.
        /// </summary>
        /// <returns>All characters from the current position to the end of the stream.</returns>
        string ReadToEnd();

        /// <summary>
        /// Reads all characters from the current position to the end of the stream.
        /// </summary>
        /// <param name="substr">All characters from the current position to the end of the stream.</param>
        void ReadToEnd( out Substring substr );
    }
}
