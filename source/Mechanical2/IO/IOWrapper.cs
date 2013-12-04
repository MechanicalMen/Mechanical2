using System;
using System.IO;
using System.Text;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Helps to switch between <see cref="System.IO"/> and <see cref="Mechanical.IO"/>.
    /// </summary>
    public static class IOWrapper
    {
        #region Stream -> IBinaryStream

        internal class StreamAsIBinaryStream : DisposableObject, IBinaryStream
        {
            #region Non-Public Fields

            private Stream stream;
            private BinaryStreamReaderLE reader;
            private BinaryStreamWriterLE writer;

            #endregion

            #region Constructor

            internal StreamAsIBinaryStream( Stream stream )
            {
                if( stream.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                if( !stream.CanRead )
                    throw new ArgumentException("Stream not readable!").StoreFileLine();

                if( !stream.CanWrite )
                    throw new ArgumentException("Stream not writable!").StoreFileLine();

                if( !stream.CanSeek )
                    throw new ArgumentException("Stream not seekable!").StoreFileLine();

                this.stream = stream;
                this.reader = new BinaryStreamReaderLE(this.stream);
                this.writer = new BinaryStreamWriterLE(this.stream);
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected override void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)

                    if( this.stream.NotNullReference() )
                    {
                        this.stream.Dispose();
                        this.stream = null;
                    }
                }

                //// shared cleanup logic
                //// (unmanaged resources)
                this.reader = null;
                this.writer = null;

                base.OnDispose(disposing);
            }

            #endregion

            #region IBinaryReader

            void IBinaryReader.Close()
            {
                this.Close();
            }

            public int Read( byte[] buffer, int index, int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.Read(buffer, index, count);
            }

            public byte[] Read( int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.Read(count);
            }

            public void Read( int count, out ArraySegment<byte> bytes )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.reader.Read(count, out bytes);
            }

            public byte[] ReadToEnd()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadToEnd();
            }

            public void ReadToEnd( out ArraySegment<byte> bytes )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.reader.ReadToEnd(out bytes);
            }

            public sbyte ReadSByte()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadSByte();
            }

            public byte ReadByte()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadByte();
            }

            public short ReadInt16()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadInt16();
            }

            public ushort ReadUInt16()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadUInt16();
            }

            public int ReadInt32()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadInt32();
            }

            public uint ReadUInt32()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadUInt32();
            }

            public long ReadInt64()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadInt64();
            }

            public ulong ReadUInt64()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadUInt64();
            }

            public float ReadSingle()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadSingle();
            }

            public double ReadDouble()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadDouble();
            }

            public decimal ReadDecimal()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadDecimal();
            }

            public bool ReadBoolean()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadBoolean();
            }

            public char ReadChar()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadChar();
            }

            public string ReadString()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadString();
            }

            #endregion

            #region ISeekableBinaryReader

            public long Length
            {
                get
                {
                    if( this.IsDisposed )
                        throw new ObjectDisposedException(null).StoreFileLine();

                    return this.stream.Length;
                }
            }

            public long Position
            {
                get
                {
                    if( this.IsDisposed )
                        throw new ObjectDisposedException(null).StoreFileLine();

                    return this.stream.Position;
                }
                set
                {
                    if( this.IsDisposed )
                        throw new ObjectDisposedException(null).StoreFileLine();

                    this.stream.Position = value;
                }
            }

            public long Seek( long offset, SeekOrigin origin )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.stream.Seek(offset, origin);
            }

            #endregion

            #region IBinaryWriter

            void IBinaryWriter.Close()
            {
                this.Close();
            }

            public void Flush()
            {
                this.writer.Flush();
            }

            public void Write( byte[] array, int offset, int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(array, offset, count);
            }

            public void Write( sbyte value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( byte value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( short value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( ushort value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( int value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( uint value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( long value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( ulong value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( float value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( double value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( decimal value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( bool value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( char value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( string value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( Substring value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            #endregion

            #region IBinaryStream

            public void Close()
            {
                this.Dispose();
            }

            public void SetLength( long value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.stream.SetLength(value);
            }

            #endregion

            protected Stream BaseStream
            {
                get { return this.stream; }
            }
        }

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>, in a disposable <see cref="IBinaryStream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static IBinaryStream ToBinaryStream( Stream stream )
        {
            return new StreamAsIBinaryStream(stream);
        }

        #endregion

        #region IBinaryStream -> Stream

        private class IBinaryStreamAsStream : Stream
        {
            #region Private Fields

            private IBinaryStream binaryStream;

            #endregion

            #region Constructors

            internal IBinaryStreamAsStream( IBinaryStream stream )
            {
                if( stream.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.binaryStream = stream;
            }

            #endregion

            #region Stream

            protected override void Dispose( bool disposing )
            {
                if( disposing )
                {
                    if( this.binaryStream.NotNullReference() )
                    {
                        this.binaryStream.Close();
                        this.binaryStream = null;
                    }
                }

                base.Dispose(disposing);
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                this.binaryStream.Flush();
            }

            public override long Length
            {
                get { return this.binaryStream.Length; }
            }

            public override long Position
            {
                get { return this.binaryStream.Position; }
                set { this.binaryStream.Position = value; }
            }

            public override int Read( byte[] buffer, int offset, int count )
            {
                return this.binaryStream.Read(buffer, offset, count);
            }

            public override long Seek( long offset, SeekOrigin origin )
            {
                return this.binaryStream.Seek(offset, origin);
            }

            public override void SetLength( long value )
            {
                this.binaryStream.SetLength(value);
            }

            public override void Write( byte[] buffer, int offset, int count )
            {
                this.binaryStream.Write(buffer, offset, count);
            }

            public override void WriteByte( byte value )
            {
                this.binaryStream.Write(value);
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="IBinaryStream"/>, in <see cref="Stream"/>.
        /// </summary>
        /// <param name="binaryStream">The <see cref="IBinaryStream"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static Stream Wrap( IBinaryStream binaryStream )
        {
            return new IBinaryStreamAsStream(binaryStream);
        }

        #endregion

        #region Stream -> IBinaryReader, ISeekableBinaryReader

        private class StreamAsIBinaryReader : DisposableObject, IBinaryReader
        {
            #region Seekable

            internal class Seekable : StreamAsIBinaryReader, ISeekableBinaryReader
            {
                #region Constructor

                internal Seekable( Stream stream )
                    : base(stream)
                {
                    if( !stream.CanSeek )
                        throw new ArgumentException("Stream not seekable!").StoreFileLine();
                }

                #endregion

                #region ISeekableBinaryReader

                public long Length
                {
                    get
                    {
                        if( this.IsDisposed )
                            throw new ObjectDisposedException(null).StoreFileLine();

                        return this.BaseStream.Length;
                    }
                }

                public long Position
                {
                    get
                    {
                        if( this.IsDisposed )
                            throw new ObjectDisposedException(null).StoreFileLine();

                        return this.BaseStream.Position;
                    }
                    set
                    {
                        if( this.IsDisposed )
                            throw new ObjectDisposedException(null).StoreFileLine();

                        this.BaseStream.Position = value;
                    }
                }

                public long Seek( long offset, SeekOrigin origin )
                {
                    if( this.IsDisposed )
                        throw new ObjectDisposedException(null).StoreFileLine();

                    return this.BaseStream.Seek(offset, origin);
                }

                #endregion
            }

            #endregion

            #region Private Fields

            private Stream stream;
            private BinaryStreamReaderLE reader;

            #endregion

            #region Constructor

            internal StreamAsIBinaryReader( Stream stream )
            {
                if( stream.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                if( !stream.CanRead )
                    throw new ArgumentException("Stream not readable!").StoreFileLine();

                this.stream = stream;
                this.reader = new BinaryStreamReaderLE(this.stream);
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected override void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)

                    if( this.stream.NotNullReference() )
                    {
                        this.stream.Dispose();
                        this.stream = null;
                    }
                }

                //// shared cleanup logic
                //// (unmanaged resources)
                this.reader = null;

                base.OnDispose(disposing);
            }

            #endregion

            #region IBinaryReader

            public void Close()
            {
                this.Dispose();
            }

            public int Read( byte[] buffer, int index, int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.Read(buffer, index, count);
            }

            public byte[] Read( int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.Read(count);
            }

            public void Read( int count, out ArraySegment<byte> bytes )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.reader.Read(count, out bytes);
            }

            public byte[] ReadToEnd()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadToEnd();
            }

            public void ReadToEnd( out ArraySegment<byte> bytes )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.reader.ReadToEnd(out bytes);
            }

            public sbyte ReadSByte()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadSByte();
            }

            public byte ReadByte()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadByte();
            }

            public short ReadInt16()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadInt16();
            }

            public ushort ReadUInt16()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadUInt16();
            }

            public int ReadInt32()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadInt32();
            }

            public uint ReadUInt32()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadUInt32();
            }

            public long ReadInt64()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadInt64();
            }

            public ulong ReadUInt64()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadUInt64();
            }

            public float ReadSingle()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadSingle();
            }

            public double ReadDouble()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadDouble();
            }

            public decimal ReadDecimal()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadDecimal();
            }

            public bool ReadBoolean()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadBoolean();
            }

            public char ReadChar()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadChar();
            }

            public string ReadString()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.reader.ReadString();
            }

            #endregion

            protected Stream BaseStream
            {
                get { return this.stream; }
            }
        }

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>, in a disposable <see cref="IBinaryReader"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static IBinaryReader ToBinaryReader( Stream stream )
        {
            if( stream.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            if( stream.CanSeek )
                return new StreamAsIBinaryReader.Seekable(stream);
            else
                return new StreamAsIBinaryReader(stream);
        }

        #endregion

        #region IBinaryReader, ISeekableBinaryReader -> Stream

        private class IBinaryReaderAsStream : Stream
        {
            #region Private Fields

            private IBinaryReader binaryReader;
            private ISeekableBinaryReader binarySeekable;

            #endregion

            #region Constructors

            internal IBinaryReaderAsStream( IBinaryReader reader, bool allowSeeking )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.binaryReader = reader;
                this.binarySeekable = allowSeeking ? reader as ISeekableBinaryReader : null;
            }

            #endregion

            #region Stream

            protected override void Dispose( bool disposing )
            {
                if( disposing )
                {
                    if( this.binaryReader.NotNullReference() )
                    {
                        this.binaryReader.Close();
                        this.binaryReader = null;
                        this.binarySeekable = null;
                    }
                }

                base.Dispose(disposing);
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return this.binarySeekable.NotNullReference(); } // technically never true, since we don't support SetLength!
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void Flush()
            {
            }

            public override long Length
            {
                get
                {
                    if( !this.CanSeek )
                        throw new NotSupportedException().StoreFileLine();

                    return this.binarySeekable.Length;
                }
            }

            public override long Position
            {
                get
                {
                    if( !this.CanSeek )
                        throw new NotSupportedException().StoreFileLine();

                    return this.binarySeekable.Position;
                }
                set
                {
                    if( !this.CanSeek )
                        throw new NotSupportedException().StoreFileLine();

                    this.binarySeekable.Position = value;
                }
            }

            public override int Read( byte[] buffer, int offset, int count )
            {
                return this.binaryReader.Read(buffer, offset, count);
            }

            public override long Seek( long offset, SeekOrigin origin )
            {
                if( !this.CanSeek )
                    throw new NotSupportedException().StoreFileLine();

                return this.binarySeekable.Seek(offset, origin);
            }

            public override void SetLength( long value )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            public override void Write( byte[] buffer, int offset, int count )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="IBinaryReader"/>, in <see cref="Stream"/>.
        /// </summary>
        /// <param name="binaryReader">The <see cref="IBinaryReader"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static Stream Wrap( IBinaryReader binaryReader )
        {
            return new IBinaryReaderAsStream(binaryReader, allowSeeking: false);
        }

        /// <summary>
        /// Wraps the specified <see cref="ISeekableBinaryReader"/>, in <see cref="Stream"/>.
        /// </summary>
        /// <param name="binaryReader">The <see cref="ISeekableBinaryReader"/> to wrap.</param>
        /// <param name="canSeek">Seekable streams require this explicit permission, because SetLength always throw a <see cref="NotSupportedException"/>! Use at your own risk.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static Stream Wrap( ISeekableBinaryReader binaryReader, bool canSeek )
        {
            return new IBinaryReaderAsStream(binaryReader, canSeek);
        }

        #endregion

        #region Stream -> IBinaryWriter

        internal class StreamAsIBinaryWriter : DisposableObject, IBinaryWriter
        {
            #region Private Fields

            private Stream stream;
            private BinaryStreamWriterLE writer;

            #endregion

            #region Constructor

            internal StreamAsIBinaryWriter( Stream stream )
            {
                if( stream.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                if( !stream.CanWrite )
                    throw new ArgumentException("Stream not writable!").StoreFileLine();

                this.stream = stream;
                this.writer = new BinaryStreamWriterLE(this.stream);
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected override void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)

                    if( this.stream.NotNullReference() )
                    {
                        this.stream.Dispose();
                        this.stream = null;
                    }
                }

                //// shared cleanup logic
                //// (unmanaged resources)
                this.writer = null;

                base.OnDispose(disposing);
            }

            #endregion

            #region IBinaryWriter

            public void Close()
            {
                this.Dispose();
            }

            public void Flush()
            {
                this.writer.Flush();
            }

            public void Write( byte[] array, int offset, int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(array, offset, count);
            }

            public void Write( sbyte value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( byte value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( short value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( ushort value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( int value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( uint value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( long value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( ulong value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( float value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( double value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( decimal value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( bool value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( char value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( string value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            public void Write( Substring value )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.writer.Write(value);
            }

            #endregion

            protected Stream BaseStream
            {
                get { return this.stream; }
            }
        }

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>, in a disposable <see cref="IBinaryWriter"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static IBinaryWriter ToBinaryWriter( Stream stream )
        {
            return new StreamAsIBinaryWriter(stream);
        }

        #endregion

        #region IBinaryWriter -> Stream

        private class IBinaryWriterAsStream : Stream
        {
            #region Private Fields

            private IBinaryWriter binaryWriter;

            #endregion

            #region Constructors

            internal IBinaryWriterAsStream( IBinaryWriter writer )
            {
                if( writer.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.binaryWriter = writer;
            }

            #endregion

            #region Stream

            protected override void Dispose( bool disposing )
            {
                if( disposing )
                {
                    if( this.binaryWriter.NotNullReference() )
                    {
                        this.binaryWriter.Close();
                        this.binaryWriter = null;
                    }
                }

                base.Dispose(disposing);
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                this.binaryWriter.Flush();
            }

            public override long Length
            {
                get { throw new NotSupportedException().StoreFileLine(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotSupportedException().StoreFileLine();
                }
                set
                {
                    throw new NotSupportedException().StoreFileLine();
                }
            }

            public override int Read( byte[] buffer, int offset, int count )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            public override long Seek( long offset, SeekOrigin origin )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            public override void SetLength( long value )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            public override void Write( byte[] buffer, int offset, int count )
            {
                this.binaryWriter.Write(buffer, offset, count);
            }

            public override void WriteByte( byte value )
            {
                this.binaryWriter.Write(value);
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="IBinaryWriter"/>, in <see cref="Stream"/>.
        /// </summary>
        /// <param name="binaryWriter">The <see cref="IBinaryWriter"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static Stream Wrap( IBinaryWriter binaryWriter )
        {
            return new IBinaryWriterAsStream(binaryWriter);
        }

        #endregion


        #region Stream -> ITextReader

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>, in a disposable <see cref="ITextReader"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static ITextReader ToTextReader( Stream stream )
        {
            return Wrap(new System.IO.StreamReader(stream));
        }

        #endregion

        #region Stream -> ITextWriter

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>, in a disposable <see cref="ITextWriter"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="newLine">The line terminator string to use.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static ITextWriter ToTextWriter( Stream stream, Encoding encoding, string newLine )
        {
            return Wrap(new System.IO.StreamWriter(stream, encoding) { NewLine = newLine });
        }

        #endregion

        #region TextReader -> ITextReader

        private class TexReaderAsITextReader : DisposableObject, ITextReader
        {
            #region Private Fields

            private TextReader textReader;

            #endregion

            #region Constructor

            internal TexReaderAsITextReader( TextReader textReader )
            {
                if( textReader.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.textReader = textReader;
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected override void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)

                    if( this.textReader.NotNullReference() )
                    {
                        this.textReader.Dispose();
                        this.textReader = null;
                    }
                }

                //// shared cleanup logic
                //// (unmanaged resources)

                base.OnDispose(disposing);
            }

            #endregion

            #region ITextReader

            public void Close()
            {
                this.Dispose();
            }

            public int Peek()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.textReader.Peek();
            }

            public int Read()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.textReader.Read();
            }

            public int Read( char[] buffer, int index, int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.textReader.Read(buffer, index, count);
            }

            public Substring Read( int count )
            {
                var buffer = new char[count];
                var numCharsRead = this.Read(buffer, 0, count);
                return new string(buffer, 0, numCharsRead);
            }

            public string ReadLine()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.textReader.ReadLine();
            }

            public void ReadLine( out Substring substr )
            {
                substr = this.ReadLine();
            }

            public string ReadToEnd()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                return this.textReader.ReadToEnd();
            }

            public void ReadToEnd( out Substring substr )
            {
                substr = this.ReadToEnd();
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="TextReader"/>, in a disposable <see cref="ITextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static ITextReader Wrap( TextReader textReader )
        {
            return new TexReaderAsITextReader(textReader);
        }

        #endregion

        #region ITextReader -> TextReader

        private class ITexReaderAsTextReader : TextReader
        {
            #region Private Fields

            private ITextReader textReader;

            #endregion

            #region Constructor

            internal ITexReaderAsTextReader( ITextReader textReader )
            {
                if( textReader.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.textReader = textReader;
            }

            #endregion

            #region TextReader

            protected override void Dispose( bool disposing )
            {
                if( disposing )
                {
                    if( this.textReader.NotNullReference() )
                    {
                        this.textReader.Close();
                        this.textReader = null;
                    }
                }

                base.Dispose(disposing);
            }

            public override int Peek()
            {
                return this.textReader.Peek();
            }

            public override int Read()
            {
                return this.textReader.Read();
            }

            public override int Read( char[] buffer, int index, int count )
            {
                return this.textReader.Read(buffer, index, count);
            }

            public override string ReadLine()
            {
                return this.textReader.ReadLine();
            }

            public override string ReadToEnd()
            {
                return this.textReader.ReadToEnd();
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="ITextReader"/>, in <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static TextReader Wrap( ITextReader textReader )
        {
            return new ITexReaderAsTextReader(textReader);
        }

        #endregion

        #region TextWriter -> ITextWriter

        private class TexWriterAsITextWriter : DisposableObject, ITextWriter
        {
            #region Private Fields

            private TextWriter textWriter;

            #endregion

            #region Constructor

            internal TexWriterAsITextWriter( TextWriter textWriter )
            {
                if( textWriter.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.textWriter = textWriter;
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected override void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)

                    if( this.textWriter.NotNullReference() )
                    {
                        this.textWriter.Dispose();
                        this.textWriter = null;
                    }
                }

                //// shared cleanup logic
                //// (unmanaged resources)

                base.OnDispose(disposing);
            }

            #endregion

            #region ITextWriter

            public void Close()
            {
                this.Dispose();
            }

            public void Flush()
            {
                this.textWriter.Flush();
            }

            public void Write( char character )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.textWriter.Write(character);
            }

            public void Write( char[] array, int offset, int count )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.textWriter.Write(array, offset, count);
            }

            public void Write( string str )
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                if( str.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.textWriter.Write(str);
            }

            public void Write( Substring substr )
            {
                this.Write(substr.ToString());
            }

            public void WriteLine()
            {
                if( this.IsDisposed )
                    throw new ObjectDisposedException(null).StoreFileLine();

                this.textWriter.WriteLine();
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="TextWriter"/>, in a disposable <see cref="ITextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static ITextWriter Wrap( TextWriter textWriter )
        {
            return new TexWriterAsITextWriter(textWriter);
        }

        #endregion

        #region ITextWriter -> TextWriter

        private class ITexWriterAsTextWriter : TextWriter
        {
            #region Private Fields

            private readonly Encoding encoding;
            private ITextWriter textWriter;

            #endregion

            #region Constructor

            internal ITexWriterAsTextWriter( ITextWriter textWriter, Encoding encoding )
            {
                if( textWriter.NullReference() )
                    throw new ArgumentNullException("textWriter").StoreFileLine();

                if( encoding.NullReference() )
                    throw new ArgumentNullException("encoding").StoreFileLine();

                this.encoding = encoding;
                this.textWriter = textWriter;
            }

            #endregion

            #region TextWriter

            protected override void Dispose( bool disposing )
            {
                if( disposing )
                {
                    if( this.textWriter.NotNullReference() )
                    {
                        this.textWriter.Close();
                        this.textWriter = null;
                    }
                }

                base.Dispose(disposing);
            }

            public override void Flush()
            {
                base.Flush();
                this.textWriter.Flush();
            }

            public override Encoding Encoding
            {
                get { return this.encoding; }
            }

            public override void Write( char value )
            {
                this.textWriter.Write(value);
            }

            public override void Write( char[] buffer, int index, int count )
            {
                this.textWriter.Write(buffer, index, count);
            }

            public override void Write( string value )
            {
                this.textWriter.Write(value);
            }

            public override void WriteLine()
            {
                this.textWriter.WriteLine();
            }

            #endregion
        }

        /// <summary>
        /// Wraps the specified <see cref="ITextWriter"/>, in <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="ITextWriter"/> to wrap.</param>
        /// <param name="encoding">The encoding to report through the <see cref="TextWriter"/>.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static TextWriter Wrap( ITextWriter textWriter, Encoding encoding )
        {
            return new ITexWriterAsTextWriter(textWriter, encoding);
        }

        #endregion


        #region Extension Methods

        private static readonly ThreadLocal<byte[]> CopyBuffer = new ThreadLocal<byte[]>(() => new byte[4 * 1024]); // the same buffer size as System.IO.Stream.CopyTo

        /// <summary>
        /// Copies the contents of one stream to another.
        /// </summary>
        /// <param name="reader">The source, to read data from.</param>
        /// <param name="writer">The destination, to write data to.</param>
        public static void CopyTo( this IBinaryReader reader, IBinaryWriter writer )
        {
            Ensure.Debug(reader, r => r.NotNull());
            Ensure.That(writer).NotNull();

            int bytesRead;
            var buffer = CopyBuffer.Value;
            while( (bytesRead = reader.Read(buffer, 0, buffer.Length)) != 0 )
                writer.Write(buffer, 0, bytesRead);
        }

        #endregion
    }
}
