using System;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Helps to switch between <see cref="System.IO"/> and <see cref="Mechanical.IO"/>.
    /// </summary>
    public static class IOWrapper
    {
        #region Stream -> IBinaryReader

        private class StreamAsIBinaryReader : DisposableObject, IBinaryReader
        {
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
        }

        /// <summary>
        /// Wraps the specified <see cref="Stream"/>, in a disposable <see cref="IBinaryReader"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to wrap.</param>
        /// <returns>The disposable wrapper created.</returns>
        public static IBinaryReader ToBinaryReader( Stream stream )
        {
            return new StreamAsIBinaryReader(stream);
        }

        #endregion

        #region Stream -> ITextReader

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
    }
}
