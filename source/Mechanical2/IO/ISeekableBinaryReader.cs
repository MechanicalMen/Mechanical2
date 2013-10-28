using System;
using System.IO;

namespace Mechanical.IO
{
    /// <summary>
    /// An readable and seekable abstract stream.
    /// </summary>
    public interface ISeekableBinaryReader : IBinaryReader
    {
        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>A long value representing the length of the stream in bytes.</value>
        long Length { get; }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>The current position within the stream.</value>
        long Position { get; set; }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter. </param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position. </param>
        /// <returns>The new position within the current stream.</returns>
        long Seek( long offset, SeekOrigin origin );
    }
}
