using System;
using System.IO;
using Mechanical.Conditions;

namespace Mechanical.IO
{
    /// <summary>
    /// Writes to a <see cref="Stream"/>.
    /// The <see cref="Stream"/> can be changed at any time, and will not be disposed of.
    /// Uses the little endian format.
    /// </summary>
    public class BinaryStreamWriterLE : BinaryWriterBaseLE
    {
        #region Private Fields

        private Stream stream;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryStreamWriterLE"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> being written to.</param>
        public BinaryStreamWriterLE( Stream stream )
            : base()
        {
            this.Stream = stream;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="Stream"/> being written to.
        /// </summary>
        /// <value>The <see cref="Stream"/> being read written to.</value>
        public Stream Stream
        {
            get
            {
                return this.stream;
            }
            set
            {
                Ensure.That(value).NotNull();
                Ensure.That(value.CanWrite).IsTrue(() => new InvalidOperationException("Stream is not writeable!"));

                if( !object.ReferenceEquals(this.stream, value) )
                    this.stream = value;
            }
        }

        #endregion

        #region IBinaryWriter

        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        public override void Close()
        {
            //// this class does NOT dispose of the Stream it - currently - wraps
            //// for that, use the IOWrapper class
        }

        /// <summary>
        /// Writes the byte array to the data store value.
        /// </summary>
        /// <param name="array">The byte array to write data from.</param>
        /// <param name="offset">The position at which to start retrieving data.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write( byte[] array, int offset, int count )
        {
            this.stream.Write(array, offset, count);
        }

        #endregion
    }
}
