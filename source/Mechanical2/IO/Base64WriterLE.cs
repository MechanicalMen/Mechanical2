using System;
using Mechanical.Conditions;

namespace Mechanical.IO
{
    /// <summary>
    /// Writes to an <see cref="ITextWriter"/>.
    /// The <see cref="ITextWriter"/> can be changed at any time, and will not be disposed of.
    /// The base64 code produced will NOT be valid, unless the writer is flushed!
    /// Uses the little endian format.
    /// </summary>
    public class Base64WriterLE : BinaryWriterBaseLE
    {
        #region Private Fields

        //// for every 3 bytes, four characters are written
        //// if there are less then 3 bytes, they are saved, or padded

        private readonly byte[] leftOverBytes = new byte[3];
        private readonly char[] characters = new char[4];
        private int numBytesLeftOver;
        private ITextWriter textWriter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Base64WriterLE"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="ITextWriter"/> being written to.</param>
        public Base64WriterLE( ITextWriter textWriter )
            : base()
        {
            this.TextWriter = textWriter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="ITextWriter"/> being written to.
        /// </summary>
        /// <value>The <see cref="ITextWriter"/> being read written to.</value>
        public ITextWriter TextWriter
        {
            get
            {
                return this.textWriter;
            }
            set
            {
                Ensure.That(value).NotNull();

                if( !object.ReferenceEquals(this.textWriter, value) )
                    this.textWriter = value;
            }
        }

        #endregion

        #region IBinaryWriter

        /// <summary>
        /// Writes the byte array to the data store value.
        /// </summary>
        /// <param name="array">The byte array to write data from.</param>
        /// <param name="offset">The position at which to start retrieving data.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write( byte[] array, int offset, int count )
        {
            //// TODO: test arguments!

            // save characters that would either have to be padded, or written later
            this.numBytesLeftOver = count % 3;
            if( this.numBytesLeftOver > 0 )
            {
                count -= this.numBytesLeftOver;
                for( int i = 0; i < this.numBytesLeftOver; i++ )
                    this.leftOverBytes[i] = array[offset + count + i];
            }

            // encode bytes in chunks
            int endAt = offset + count;
            int byteChunkLength = this.leftOverBytes.Length;
            int numChars;
            while( offset < endAt )
            {
                if( offset + byteChunkLength > endAt )
                    byteChunkLength = endAt - offset;

                numChars = Convert.ToBase64CharArray(array, offset, byteChunkLength, this.characters, 0);
                this.textWriter.Write(this.characters, 0, numChars);

                offset += byteChunkLength;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes out any remaining bytes and adds padding, if necessary.
        /// </summary>
        public void Flush()
        {
            if( this.numBytesLeftOver > 0 )
            {
                int numChars = Convert.ToBase64CharArray(this.leftOverBytes, 0, this.numBytesLeftOver, this.characters, 0);
                this.textWriter.Write(this.characters, 0, numChars);
                this.numBytesLeftOver = 0;
            }
        }

        #endregion
    }

    //// TODO: base64 reader (decompile Base64Decoder); base64 data store
}
