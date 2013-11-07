using System;

namespace Mechanical.IO
{
    /// <summary>
    /// Reads from a(n abstract) byte stream.
    /// </summary>
    public interface IBinaryReader
    {
        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        void Close();

        /// <summary>
        /// Reads the specified number of bytes, starting from a specified point in the byte array.
        /// </summary>
        /// <param name="buffer">The buffer to read data into.</param>
        /// <param name="index">The starting point in the buffer at which to begin reading into the buffer.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes read into <paramref name="buffer"/>. This might be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</returns>
        int Read( byte[] buffer, int index, int count );

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The maximum number of bytes to read. If the end of the data store value is reached before the specified number of bytes is read, the method returns.</param>
        /// <returns>The bytes read. The length may be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</returns>
        byte[] Read( int count );

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The maximum number of bytes to read. If the end of the data store value is reached before the specified number of bytes is read, the method returns.</param>
        /// <param name="bytes">The bytes read. The length may be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</param>
        void Read( int count, out ArraySegment<byte> bytes );

        /// <summary>
        /// Reads all bytes starting from the current position.
        /// </summary>
        /// <returns>All bytes starting from the current position.</returns>
        byte[] ReadToEnd();

        /// <summary>
        /// Reads all bytes starting from the current position.
        /// </summary>
        /// <param name="bytes">All bytes starting from the current position.</param>
        void ReadToEnd( out ArraySegment<byte> bytes );

        /// <summary>
        /// Reads a(n) <see cref="SByte"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        sbyte ReadSByte();

        /// <summary>
        /// Reads a(n) <see cref="Byte"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        byte ReadByte();

        /// <summary>
        /// Reads a(n) <see cref="Int16"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        short ReadInt16();

        /// <summary>
        /// Reads a(n) <see cref="UInt16"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads a(n) <see cref="Int32"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        int ReadInt32();

        /// <summary>
        /// Reads a(n) <see cref="UInt32"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads a(n) <see cref="Int64"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        long ReadInt64();

        /// <summary>
        /// Reads a(n) <see cref="UInt64"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Reads a(n) <see cref="Single"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        float ReadSingle();

        /// <summary>
        /// Reads a(n) <see cref="Double"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        double ReadDouble();

        /// <summary>
        /// Reads a(n) <see cref="Decimal"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        decimal ReadDecimal();

        /// <summary>
        /// Reads a(n) <see cref="Boolean"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        bool ReadBoolean();

        /// <summary>
        /// Reads a(n) <see cref="Char"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        char ReadChar();

        /// <summary>
        /// Reads a(n) <see cref="String"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        string ReadString();
    }
}
