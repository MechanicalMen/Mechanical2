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
    public class BinaryStreamReaderLE : IBinaryReader
    {
        #region Private Fields

        private static readonly byte[] EmptyBytes = new byte[0];
        private readonly byte[] buffer = new byte[16];
        private Stream stream;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryStreamReaderLE"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> being read from.</param>
        public BinaryStreamReaderLE( Stream stream )
        {
            this.Stream = stream;
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void FillBuffer( int count )
        {
            int bytesReadPreviously = 0;
            int bytesReadNow = 0;

            if( count == 1 )
            {
                bytesReadNow = this.stream.ReadByte();
                if( bytesReadNow == -1 )
                    goto endOfStream;
                this.buffer[0] = (byte)bytesReadNow;
                return;
            }

            do
            {
                bytesReadNow = this.stream.Read(this.buffer, bytesReadPreviously, count - bytesReadPreviously);
                if( bytesReadNow == 0 )
                    goto endOfStream;
                bytesReadPreviously += bytesReadNow;
            }
            while( bytesReadPreviously < count );
            return;

        endOfStream:
            throw new EndOfStreamException().StoreFileLine();
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private byte[] ReadBytes( int count )
        {
            //// NOTE: count < 0  ==  ReadToEnd

            if( count == 0 )
                return EmptyBytes;

            using( var ms = new MemoryStream() )
            {
                var buffer = new byte[1024];
                int numRead;
                int numWritten = 0;
                while( (numRead = this.Read(buffer, index: 0, count: buffer.Length)) != 0 )
                {
                    if( count >= 0
                     && numWritten + numRead > count )
                        numRead = count - numWritten; // bytes left

                    ms.Write(buffer, offset: 0, count: numRead);
                    numWritten += numRead;

                    if( numWritten == count )
                        break;
                }

                return ms.ToArray();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="Stream"/> being read from.
        /// </summary>
        /// <value>The <see cref="Stream"/> being read from.</value>
        public Stream Stream
        {
            get
            {
                return this.stream;
            }
            set
            {
                Ensure.That(value).NotNull();
                Ensure.That(value.CanRead).IsTrue(() => new InvalidOperationException("Stream is not readable!"));

                if( !object.ReferenceEquals(this.stream, value) )
                    this.stream = value;
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
                throw new ArgumentNullException().StoreFileLine();

            if( index < 0
             || count < 0
             || index + count > buffer.Length )
                throw new ArgumentOutOfRangeException().Store("index", index).Store("count", count).Store("buffer.Length", buffer.Length);

            return this.stream.Read(buffer, index, count);
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The maximum number of bytes to read. If the end of the data store value is reached before the specified number of bytes is read, the method returns.</param>
        /// <returns>The bytes read. The length may be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</returns>
        public byte[] Read( int count )
        {
            if( count < 0 )
                throw new ArgumentOutOfRangeException("count").Store("count", count);

            return this.ReadBytes(count);
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The maximum number of bytes to read. If the end of the data store value is reached before the specified number of bytes is read, the method returns.</param>
        /// <param name="bytes">The bytes read. The length may be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.</param>
        public void Read( int count, out ArraySegment<byte> bytes )
        {
            bytes = new ArraySegment<byte>(this.Read(count));
        }

        /// <summary>
        /// Reads all bytes starting from the current position.
        /// </summary>
        /// <returns>All bytes starting from the current position.</returns>
        public byte[] ReadToEnd()
        {
            return this.ReadBytes(-1);
        }

        /// <summary>
        /// Reads all bytes starting from the current position.
        /// </summary>
        /// <param name="bytes">All bytes starting from the current position.</param>
        public void ReadToEnd( out ArraySegment<byte> bytes )
        {
            bytes = new ArraySegment<byte>(this.ReadToEnd());
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
