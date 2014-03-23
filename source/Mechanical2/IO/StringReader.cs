using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// A reusable <see cref="Substring"/> wrapper.
    /// </summary>
    public class StringReader : StringReaderBase
    {
        #region Private Fields

        private Substring substr;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringReader"/> class.
        /// </summary>
        public StringReader()
        {
            this.Set(Substring.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringReader"/> class.
        /// </summary>
        /// <param name="substr">The substring to read from.</param>
        public StringReader( Substring substr )
        {
            this.Set(substr);
        }

        #endregion

        #region StringReaderBase

        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        /// Fills the internal buffer of the reader.
        /// </summary>
        /// <returns>Characters from the stream, or a null or empty <see cref="Substring"/> if all characters have been read.</returns>
        protected override Substring RequestBuffer()
        {
            var newBuffer = this.substr;
            this.substr = Substring.Null;
            return newBuffer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Specifies the <see cref="Substring"/> to read from.
        /// </summary>
        /// <param name="substr">The substring to read from.</param>
        public void Set( Substring substr )
        {
            if( substr.Origin.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            this.substr = substr;
        }

        #endregion
    }
}
