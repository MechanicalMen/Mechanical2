using System;
using System.Runtime.CompilerServices;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Helps implementing <see cref="ITextReader"/>.
    /// </summary>
    public abstract class StringReaderBase : ITextReader
    {
        #region Private Fields

        private Substring buffer;
        private StringBuilder sb;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringReaderBase"/> class.
        /// </summary>
        public StringReaderBase()
        {
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void AppendToBuilder( Substring substr )
        {
            if( !substr.NullOrEmpty )
            {
                if( this.sb.NullReference() )
                    this.sb = new StringBuilder();

                this.sb.Append(substr.Origin, substr.StartIndex, substr.Length);
            }
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private string ClearBuilder()
        {
            if( this.sb.NullReference() )
            {
                return null;
            }
            else
            {
                var str = this.sb.ToString();
                this.sb.Clear();
                return str;
            }
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int IndexOfNewLineInBuffer()
        {
            if( this.buffer.NullOrEmpty )
                return -1;

            for( int i = this.buffer.StartIndex; i < this.buffer.Origin.Length; ++i )
            {
                if( this.buffer.Origin[i] == '\r'
                 || this.buffer.Origin[i] == '\n' )
                    return i - this.buffer.StartIndex;
            }

            return -1;
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void SetBufferPastNewLine( int newLineAt )
        {
            this.buffer = this.buffer.Substr(newLineAt);

            int ch = this.Read();
            if( (char)ch == '\r' )
            {
                ch = this.Peek();
                if( ch != -1
                 && (char)ch == '\n' )
                    this.Read();
            }
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Fills the internal buffer of the reader.
        /// </summary>
        /// <returns>Characters from the stream, or a null or empty <see cref="Substring"/> if all characters have been read.</returns>
        protected abstract Substring RequestBuffer();

        #endregion

        #region ITextReader

        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Returns the next available character, without actually reading it.
        /// </summary>
        /// <returns>An integer representing the next character to be read, or <c>-1</c> if no more characters are available or the reader does not support seeking.</returns>
        public int Peek()
        {
            if( this.buffer.NullOrEmpty )
            {
                this.buffer = this.RequestBuffer();
                if( this.buffer.NullOrEmpty )
                    return -1;
            }

            return this.buffer[0];
        }

        /// <summary>
        /// Reads the next character.
        /// </summary>
        /// <returns>The next character, or <c>-1</c> if no more characters are available.</returns>
        public int Read()
        {
            int result = this.Peek();
            if( result != -1 )
                this.buffer = this.buffer.Substr(1);
            return result;
        }

        /// <summary>
        /// Reads a specified maximum number of characters, and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> - 1) replaced by the characters read.</param>
        /// <param name="index">The position in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the stream is reached before the specified number of characters is read into the buffer, the method returns.</param>
        /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether the data is available within the stream. This method returns <c>0</c> (zero) if it is called when no more characters are left to read.</returns>
        public int Read( char[] buffer, int index, int count )
        {
            var substr = this.Read(count);
            if( substr.NullOrEmpty )
                return 0;

            substr.Origin.CopyTo(
                sourceIndex: substr.StartIndex,
                destination: buffer,
                destinationIndex: index,
                count: substr.Length);

            return substr.Length;
        }

        /// <summary>
        /// Reads a specified maximum number of characters.
        /// </summary>
        /// <param name="count">The maximum number of characters to read. If the end of the stream is reached before the specified number of characters is read, the method returns.</param>
        /// <returns>The characters read. The number of characters that have been read will be less than or equal to <paramref name="count"/>, depending on whether the data is available within the stream. This method returns <see cref="Substring.Empty"/> if it is called when no more characters are left to read.</returns>
        public Substring Read( int count )
        {
            if( count < 0 )
                throw new ArgumentException().Store("count", count);

            if( count == 0 )
                return Substring.Empty;

            // anything to read?
            if( this.buffer.NullOrEmpty )
            {
                this.buffer = this.RequestBuffer();
                if( this.buffer.NullOrEmpty )
                    return Substring.Empty;
            }

            // buffer not empty
            if( count <= this.buffer.Length )
            {
                var result = this.buffer.Substr(startIndex: 0, length: count);
                this.buffer = this.buffer.Substr(count);
                return result;
            }
            else
            {
                while( true )
                {
                    int len = Math.Min(count, this.buffer.Length);
                    this.AppendToBuilder(this.buffer.Substr(startIndex: 0, length: len));
                    this.buffer = this.buffer.Substr(len);

                    count -= len;
                    if( count == 0 )
                        break;

                    // if count != 0; then buffer.Length == 0
                    this.buffer = this.RequestBuffer();
                    if( this.buffer.NullOrEmpty )
                        break;
                }

                // we either read the required number of characters,
                // or we've run out things to read
                return this.ClearBuilder();
            }
        }

        /// <summary>
        /// Reads a line of characters.
        /// </summary>
        /// <returns>The next line from the stream, or <c>null</c> if all characters have been read.</returns>
        public string ReadLine()
        {
            Substring substr;
            this.ReadLine(out substr);
            return substr.ToString();
        }

        /// <summary>
        /// Reads a line of characters.
        /// </summary>
        /// <param name="substr">The next line from the stream, or <see cref="Substring.Null"/> if all characters have been read.</param>
        public void ReadLine( out Substring substr )
        {
            if( this.buffer.NullOrEmpty )
            {
                this.buffer = this.RequestBuffer();
                if( this.buffer.NullOrEmpty )
                {
                    substr = Substring.Null;
                    return;
                }
            }

            int newLineAt = this.IndexOfNewLineInBuffer();
            if( newLineAt != -1 )
            {
                substr = this.buffer.Substr(startIndex: 0, length: newLineAt);
                this.SetBufferPastNewLine(newLineAt);
            }
            else
            {
                while( newLineAt == -1 )
                {
                    this.AppendToBuilder(this.buffer);

                    this.buffer = this.RequestBuffer();
                    if( this.buffer.NullOrEmpty )
                        break;

                    newLineAt = this.IndexOfNewLineInBuffer();
                }

                if( newLineAt != -1 )
                {
                    this.AppendToBuilder(this.buffer.Substr(startIndex: 0, length: newLineAt));
                    this.SetBufferPastNewLine(newLineAt);
                }

                substr = this.ClearBuilder();
            }
        }

        /// <summary>
        /// Reads all characters from the current position to the end of the stream.
        /// </summary>
        /// <returns>All characters from the current position to the end of the stream.</returns>
        public string ReadToEnd()
        {
            Substring tmp;
            this.ReadToEnd(out tmp);
            return tmp.ToString();
        }

        /// <summary>
        /// Reads all characters from the current position to the end of the stream.
        /// </summary>
        /// <param name="substr">All characters from the current position to the end of the stream.</param>
        public void ReadToEnd( out Substring substr )
        {
            var nextBuffer = this.RequestBuffer();
            if( nextBuffer.NullOrEmpty )
            {
                if( this.buffer.NullOrEmpty )
                    substr = Substring.Empty;
                else
                    substr = this.buffer;
            }
            else
            {
                this.AppendToBuilder(this.buffer);
                this.buffer = nextBuffer;

                while( !this.buffer.NullOrEmpty )
                {
                    this.AppendToBuilder(this.buffer);
                    this.buffer = this.RequestBuffer();
                }

                substr = this.ClearBuilder();
            }

            this.buffer = Substring.Null;
        }

        #endregion
    }
}
