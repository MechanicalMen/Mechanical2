using System;
using System.IO;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Reads from a <see cref="Stream"/>.
    /// The <see cref="Stream"/> can be changed at any time, and will not be disposed of.
    /// Uses the little endian format.
    /// </summary>
    public class BinaryArrayReaderLE : IBinaryReader
    {
        #region Private Fields

        private static readonly byte[] EmptyBytes = new byte[0];
        private readonly byte[] buffer = new byte[16];
        private ArraySegment<byte> arraySegment;
        private int offset = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryArrayReaderLE"/> class.
        /// </summary>
        public BinaryArrayReaderLE()
            : this(new ArraySegment<byte>(EmptyBytes))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryArrayReaderLE"/> class.
        /// </summary>
        /// <param name="bytes">The byte array to read from.</param>
        public BinaryArrayReaderLE( ArraySegment<byte> bytes )
        {
            this.Bytes = bytes;
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void FillBuffer( int count )
        {
            if( count > this.arraySegment.Count - this.offset )
                throw new EndOfStreamException().StoreDefault();

            if( count == 1 )
            {
                this.buffer[0] = this.arraySegment.Array[this.arraySegment.Offset + this.offset];
                ++this.offset;
            }
            else
            {
                Buffer.BlockCopy(src: this.arraySegment.Array, srcOffset: this.arraySegment.Offset + this.offset, dst: this.buffer, dstOffset: 0, count: count);
                this.offset += count;
            }
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ArraySegment<byte> ReadBytes( int count )
        {
            if( count < 0 )
                count = this.arraySegment.Count - this.offset; // count < 0  ==  ReadToEnd
            else if( count == 0 )
                return new ArraySegment<byte>(EmptyBytes);

            var result = new ArraySegment<byte>(this.arraySegment.Array, this.arraySegment.Offset + this.offset, this.arraySegment.Count - this.offset);
            this.offset += count;
            return result;
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private byte[] ToArray( ArraySegment<byte> segment )
        {
            if( segment.Count == 0 )
            {
                return EmptyBytes;
            }
            else
            {
                var result = new byte[segment.Count];
                Buffer.BlockCopy(src: segment.Array, srcOffset: segment.Offset, dst: result, dstOffset: 0, count: segment.Count);
                return result;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the byte array to read from.
        /// </summary>
        /// <value>The byte array to read from.</value>
        public ArraySegment<byte> Bytes
        {
            get
            {
                return this.arraySegment;
            }
            set
            {
                Ensure.That(value.Array).NotNull();

                if( !object.ReferenceEquals(this.arraySegment.Array, value.Array)
                 || this.arraySegment.Offset != value.Offset
                 || this.arraySegment.Count != value.Count )
                {
                    this.arraySegment = value;
                    this.offset = 0;
                }
            }
        }

        #endregion

        #region IBinaryReader

        /// <summary>
        /// Reads the specified number of bytes, starting from a specified point in the byte array.
        /// </summary>
        /// <param name="buffer">The buffer to read data into.</param>
        /// <param name="index">The starting point in the buffer at which to begin reading into the buffer.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes read into <paramref name="buffer"/>. This might be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</returns>
        public int Read( byte[] buffer, int index, int count )
        {
            if( buffer.NullReference() )
                throw new ArgumentNullException().StoreDefault();

            if( index < 0
             || count < 0
             || index + count > buffer.Length )
                throw new ArgumentOutOfRangeException().Store("index", index).Store("count", count).Store("buffer.Length", buffer.Length);

            Buffer.BlockCopy(src: this.arraySegment.Array, srcOffset: this.arraySegment.Offset + this.offset, dst: buffer, dstOffset: index, count: count);
            this.offset += count;
            return count;
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The maximum number of bytes to read. If the end of the data store value is reached before the specified number of bytes is read, the method returns.</param>
        /// <returns>The bytes read. The length may be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</returns>
        public byte[] Read( int count )
        {
            ArraySegment<byte> segment;
            this.Read(count, out segment);
            return this.ToArray(segment);
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The maximum number of bytes to read. If the end of the data store value is reached before the specified number of bytes is read, the method returns.</param>
        /// <param name="bytes">The bytes read. The length may be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</param>
        public void Read( int count, out ArraySegment<byte> bytes )
        {
            if( count < 0 )
                throw new ArgumentOutOfRangeException("count").Store("count", count);

            bytes = this.ReadBytes(count);
        }

        /// <summary>
        /// Reads all bytes starting from the current position.
        /// </summary>
        /// <returns>All bytes starting from the current position.</returns>
        public byte[] ReadToEnd()
        {
            ArraySegment<byte> segment;
            this.ReadToEnd(out segment);
            return this.ToArray(segment);
        }

        /// <summary>
        /// Reads all bytes starting from the current position.
        /// </summary>
        /// <param name="bytes">All bytes starting from the current position.</param>
        public void ReadToEnd( out ArraySegment<byte> bytes )
        {
            bytes = this.ReadBytes(-1);
        }

        /// <summary>
        /// Reads a(n) <see cref="SByte"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public sbyte ReadSByte()
        {
            this.FillBuffer(1);
            return (sbyte)this.buffer[0];
        }

        /// <summary>
        /// Reads a(n) <see cref="Byte"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public byte ReadByte()
        {
            this.FillBuffer(1);
            return this.buffer[0];
        }

        /// <summary>
        /// Reads a(n) <see cref="Int16"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public short ReadInt16()
        {
            this.FillBuffer(2);
            return BitConverterLE.ToInt16(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="UInt16"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public ushort ReadUInt16()
        {
            this.FillBuffer(2);
            return BitConverterLE.ToUInt16(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Int32"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public int ReadInt32()
        {
            this.FillBuffer(4);
            return BitConverterLE.ToInt32(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="UInt32"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public uint ReadUInt32()
        {
            this.FillBuffer(4);
            return BitConverterLE.ToUInt32(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Int64"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public long ReadInt64()
        {
            this.FillBuffer(8);
            return BitConverterLE.ToInt64(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="UInt64"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public ulong ReadUInt64()
        {
            this.FillBuffer(8);
            return BitConverterLE.ToUInt64(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Single"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public float ReadSingle()
        {
            this.FillBuffer(4);
            return BitConverterLE.ToSingle(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Double"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public double ReadDouble()
        {
            this.FillBuffer(8);
            return BitConverterLE.ToDouble(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Decimal"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public decimal ReadDecimal()
        {
            this.FillBuffer(16);
            return BitConverterLE.ToDecimal(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Boolean"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public bool ReadBoolean()
        {
            this.FillBuffer(1);
            return BitConverterLE.ToBoolean(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="Char"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public char ReadChar()
        {
            this.FillBuffer(2);
            return BitConverterLE.ToChar(this.buffer, startIndex: 0);
        }

        /// <summary>
        /// Reads a(n) <see cref="String"/>.
        /// </summary>
        /// <returns>The value read.</returns>
        public string ReadString()
        {
            var length = this.ReadInt32();
            var chars = new char[length];

            for( int i = 0; i < chars.Length; ++i )
                chars[i] = this.ReadChar();

            return new string(chars);
        }

        #endregion
    }
}
