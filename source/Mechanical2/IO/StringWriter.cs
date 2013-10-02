using System;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO
{
    /// <summary>
    /// A reusable <see cref="StringBuilder"/> wrapper.
    /// </summary>
    public class StringWriter : ITextWriter
    {
        #region Private Fields

        private readonly StringBuilder sb;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringWriter"/> class.
        /// </summary>
        public StringWriter()
            : this(new StringBuilder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringWriter"/> class.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        public StringWriter( StringBuilder sb )
        {
            Ensure.That(sb).NotNull();

            this.sb = sb;
        }

        #endregion

        #region ITextWriter

        /// <summary>
        /// Writes the character.
        /// </summary>
        /// <param name="character">The character to write.</param>
        public void Write( char character )
        {
            this.sb.Append(character);
        }

        /// <summary>
        /// Writes the character array.
        /// </summary>
        /// <param name="array">The character array to write data from.</param>
        /// <param name="offset">The character position at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write( char[] array, int offset, int count )
        {
            this.sb.Append(array, offset, count);
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="str">The string to write.</param>
        public void Write( string str )
        {
            this.sb.Append(str);
        }

        /// <summary>
        /// Writes the substring.
        /// </summary>
        /// <param name="substr">The substring to write.</param>
        public void Write( Substring substr )
        {
            if( substr.Origin.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            this.sb.Append(substr.Origin, substr.StartIndex, substr.Length);
        }

        /// <summary>
        /// Writes a line terminator.
        /// </summary>
        public void WriteLine()
        {
            this.sb.Append(DataStore.DefaultNewLine);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes any previously written content.
        /// </summary>
        public void Clear()
        {
            this.sb.Clear();
        }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return this.sb.ToString();
        }

        #endregion
    }
}
