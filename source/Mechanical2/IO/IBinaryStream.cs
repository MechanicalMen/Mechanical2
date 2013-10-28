using System;

namespace Mechanical.IO
{
    /// <summary>
    /// An abstract readable, writable and seekable stream.
    /// </summary>
    public interface IBinaryStream : ISeekableBinaryReader, IBinaryWriter
    {
        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        void SetLength( long value );
    }
}
