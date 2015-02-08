using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.IO;
using Mechanical.IO.FileSystem;

namespace Mechanical.Common.Tests
{
    /// <summary>
    /// Thinly wraps an <see cref="IFileSystem"/>, though some members may instead throw exceptions.
    /// </summary>
    public class ErrorFileSystem : IFileSystem
    {
        #region ThrowOptions

        /// <summary>
        /// Details when exceptions may be thrown by the <see cref="ErrorFileSystem"/>.
        /// </summary>
        [Flags]
        public enum ThrowOptions
        {
            /// <summary>
            /// Do not throw exceptions
            /// </summary>
            None = 0,

            /// <summary>
            /// Throw an exception, instead of opening a file for reading.
            /// </summary>
            Read = 1,

            /// <summary>
            /// Throw an exception, instead of opening a file for writing.
            /// </summary>
            Write = 2,

            /// <summary>
            /// Throw an exception, instead of listing files or directories.
            /// </summary>
            List = 4,

            /// <summary>
            /// Throw an exception, instead of allowing to open a second stream for the same file.
            /// </summary>
            FileShare = 8,

            /// <summary>
            /// Throw exceptions whenever possible.
            /// </summary>
            All = Read | Write | List | FileShare
        }

        #endregion

        #region WrappedStreamBase

        private class WrappedStreamBase : IStreamBase
        {
            private readonly ErrorFileSystem parent;
            private readonly string dataStorePath;
            private readonly IStreamBase stream;

            public WrappedStreamBase( ErrorFileSystem parent, string dataStorePath, IStreamBase stream )
            {
                Ensure.That("parent", parent).NotNull();
                Ensure.That("stream", stream).NotNull();

                if( !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException().Store("dataStorePath", dataStorePath);

                this.parent = parent;
                this.dataStorePath = dataStorePath;
                this.stream = stream;

                try
                {
                    this.parent.MarkFileOpen(this.dataStorePath);
                }
                catch
                {
                    this.stream.Close();
                    throw;
                }
            }

            public void Close()
            {
                try
                {
                    this.stream.Close();
                }
                finally
                {
                    this.parent.MarkFileClosed(this.dataStorePath);
                }
            }
        }

        #endregion

        #region BinaryReaderWrapper

        private class BinaryReaderWrapper : WrappedStreamBase, IBinaryReader
        {
            private readonly IBinaryReader stream;

            public BinaryReaderWrapper( ErrorFileSystem parent, string dataStorePath, IBinaryReader stream )
                : base(parent, dataStorePath, stream)
            {
                this.stream = stream;
            }

            public int Read( byte[] buffer, int index, int count )
            {
                return this.stream.Read(buffer, index, count);
            }

            public byte[] Read( int count )
            {
                return this.stream.Read(count);
            }

            public void Read( int count, out ArraySegment<byte> bytes )
            {
                this.stream.Read(count, out bytes);
            }

            public byte[] ReadToEnd()
            {
                return this.stream.ReadToEnd();
            }

            public void ReadToEnd( out ArraySegment<byte> bytes )
            {
                this.stream.ReadToEnd(out bytes);
            }

            public sbyte ReadSByte()
            {
                return this.stream.ReadSByte();
            }

            public byte ReadByte()
            {
                return this.stream.ReadByte();
            }

            public short ReadInt16()
            {
                return this.stream.ReadInt16();
            }

            public ushort ReadUInt16()
            {
                return this.stream.ReadUInt16();
            }

            public int ReadInt32()
            {
                return this.stream.ReadInt32();
            }

            public uint ReadUInt32()
            {
                return this.stream.ReadUInt32();
            }

            public long ReadInt64()
            {
                return this.stream.ReadInt64();
            }

            public ulong ReadUInt64()
            {
                return this.stream.ReadUInt64();
            }

            public float ReadSingle()
            {
                return this.stream.ReadSingle();
            }

            public double ReadDouble()
            {
                return this.stream.ReadDouble();
            }

            public decimal ReadDecimal()
            {
                return this.stream.ReadDecimal();
            }

            public bool ReadBoolean()
            {
                return this.stream.ReadBoolean();
            }

            public char ReadChar()
            {
                return this.stream.ReadChar();
            }

            public string ReadString()
            {
                return this.stream.ReadString();
            }
        }

        #endregion

        #region TextReaderWrapper

        private class TextReaderWrapper : WrappedStreamBase, ITextReader
        {
            private readonly ITextReader stream;

            public TextReaderWrapper( ErrorFileSystem parent, string dataStorePath, ITextReader stream )
                : base(parent, dataStorePath, stream)
            {
                this.stream = stream;
            }

            public int Peek()
            {
                return this.stream.Peek();
            }

            public int Read()
            {
                return this.stream.Read();
            }

            public int Read( char[] buffer, int index, int count )
            {
                return this.stream.Read(buffer, index, count);
            }

            public Substring Read( int count )
            {
                return this.stream.Read(count);
            }

            public string ReadLine()
            {
                return this.stream.ReadLine();
            }

            public void ReadLine( out Substring substr )
            {
                this.stream.ReadLine(out substr);
            }

            public string ReadToEnd()
            {
                return this.stream.ReadToEnd();
            }

            public void ReadToEnd( out Substring substr )
            {
                this.stream.ReadToEnd(out substr);
            }
        }

        #endregion

        #region BinaryWriterWrapper

        private class BinaryWriterWrapper : WrappedStreamBase, IBinaryWriter
        {
            private readonly IBinaryWriter stream;

            public BinaryWriterWrapper( ErrorFileSystem parent, string dataStorePath, IBinaryWriter stream )
                : base(parent, dataStorePath, stream)
            {
                this.stream = stream;
            }

            public void Flush()
            {
                this.stream.Flush();
            }

            public void Write( byte[] array, int offset, int count )
            {
                this.stream.Write(array, offset, count);
            }

            public void Write( sbyte value )
            {
                this.stream.Write(value);
            }

            public void Write( byte value )
            {
                this.stream.Write(value);
            }

            public void Write( short value )
            {
                this.stream.Write(value);
            }

            public void Write( ushort value )
            {
                this.stream.Write(value);
            }

            public void Write( int value )
            {
                this.stream.Write(value);
            }

            public void Write( uint value )
            {
                this.stream.Write(value);
            }

            public void Write( long value )
            {
                this.stream.Write(value);
            }

            public void Write( ulong value )
            {
                this.stream.Write(value);
            }

            public void Write( float value )
            {
                this.stream.Write(value);
            }

            public void Write( double value )
            {
                this.stream.Write(value);
            }

            public void Write( decimal value )
            {
                this.stream.Write(value);
            }

            public void Write( bool value )
            {
                this.stream.Write(value);
            }

            public void Write( char value )
            {
                this.stream.Write(value);
            }

            public void Write( string value )
            {
                this.stream.Write(value);
            }

            public void Write( Substring value )
            {
                this.stream.Write(value);
            }
        }

        #endregion

        #region TextWriterWrapper

        private class TextWriterWrapper : WrappedStreamBase, ITextWriter
        {
            private readonly ITextWriter stream;

            public TextWriterWrapper( ErrorFileSystem parent, string dataStorePath, ITextWriter stream )
                : base(parent, dataStorePath, stream)
            {
                this.stream = stream;
            }

            public void Flush()
            {
                this.stream.Flush();
            }

            public void Write( char character )
            {
                this.stream.Write(character);
            }

            public void Write( char[] array, int offset, int count )
            {
                this.stream.Write(array, offset, count);
            }

            public void Write( string str )
            {
                this.stream.Write(str);
            }

            public void Write( Substring substr )
            {
                this.stream.Write(substr);
            }

            public void WriteLine()
            {
                this.stream.WriteLine();
            }
        }

        #endregion

        #region BinaryReaderWriterWrapper

        private class BinaryReaderWriterWrapper : WrappedStreamBase, IBinaryStream
        {
            private readonly IBinaryStream stream;

            public BinaryReaderWriterWrapper( ErrorFileSystem parent, string dataStorePath, IBinaryStream stream )
                : base(parent, dataStorePath, stream)
            {
                this.stream = stream;
            }

            #region IBinaryReader

            public int Read( byte[] buffer, int index, int count )
            {
                return this.stream.Read(buffer, index, count);
            }

            public byte[] Read( int count )
            {
                return this.stream.Read(count);
            }

            public void Read( int count, out ArraySegment<byte> bytes )
            {
                this.stream.Read(count, out bytes);
            }

            public byte[] ReadToEnd()
            {
                return this.stream.ReadToEnd();
            }

            public void ReadToEnd( out ArraySegment<byte> bytes )
            {
                this.stream.ReadToEnd(out bytes);
            }

            public sbyte ReadSByte()
            {
                return this.stream.ReadSByte();
            }

            public byte ReadByte()
            {
                return this.stream.ReadByte();
            }

            public short ReadInt16()
            {
                return this.stream.ReadInt16();
            }

            public ushort ReadUInt16()
            {
                return this.stream.ReadUInt16();
            }

            public int ReadInt32()
            {
                return this.stream.ReadInt32();
            }

            public uint ReadUInt32()
            {
                return this.stream.ReadUInt32();
            }

            public long ReadInt64()
            {
                return this.stream.ReadInt64();
            }

            public ulong ReadUInt64()
            {
                return this.stream.ReadUInt64();
            }

            public float ReadSingle()
            {
                return this.stream.ReadSingle();
            }

            public double ReadDouble()
            {
                return this.stream.ReadDouble();
            }

            public decimal ReadDecimal()
            {
                return this.stream.ReadDecimal();
            }

            public bool ReadBoolean()
            {
                return this.stream.ReadBoolean();
            }

            public char ReadChar()
            {
                return this.stream.ReadChar();
            }

            public string ReadString()
            {
                return this.stream.ReadString();
            }

            #endregion

            #region IBinaryWriter

            public void Flush()
            {
                this.stream.Flush();
            }

            public void Write( byte[] array, int offset, int count )
            {
                this.stream.Write(array, offset, count);
            }

            public void Write( sbyte value )
            {
                this.stream.Write(value);
            }

            public void Write( byte value )
            {
                this.stream.Write(value);
            }

            public void Write( short value )
            {
                this.stream.Write(value);
            }

            public void Write( ushort value )
            {
                this.stream.Write(value);
            }

            public void Write( int value )
            {
                this.stream.Write(value);
            }

            public void Write( uint value )
            {
                this.stream.Write(value);
            }

            public void Write( long value )
            {
                this.stream.Write(value);
            }

            public void Write( ulong value )
            {
                this.stream.Write(value);
            }

            public void Write( float value )
            {
                this.stream.Write(value);
            }

            public void Write( double value )
            {
                this.stream.Write(value);
            }

            public void Write( decimal value )
            {
                this.stream.Write(value);
            }

            public void Write( bool value )
            {
                this.stream.Write(value);
            }

            public void Write( char value )
            {
                this.stream.Write(value);
            }

            public void Write( string value )
            {
                this.stream.Write(value);
            }

            public void Write( Substring value )
            {
                this.stream.Write(value);
            }

            #endregion

            #region IBinaryStream, ISeekableBinaryReader

            public void SetLength( long value )
            {
                this.stream.SetLength(value);
            }

            public long Length
            {
                get { return this.stream.Length; }
            }

            public long Position
            {
                get { return this.stream.Position; }
                set { this.stream.Position = value; }
            }

            public long Seek( long offset, System.IO.SeekOrigin origin )
            {
                return this.stream.Seek(offset, origin);
            }

            #endregion
        }

        #endregion


        #region Private Fields

        private readonly IFileSystem fileSystem;
        private readonly ThrowOptions throwOptions;
        private readonly HashSet<string> filesOpen;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFileSystem"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system to wrap.</param>
        /// <param name="options">When to throw exceptions.</param>
        public ErrorFileSystem( IFileSystem fileSystem, ThrowOptions options )
        {
            Ensure.That("fileSystem", fileSystem).NotNull();
            Ensure.That(options).IsValid();

            this.fileSystem = fileSystem;
            this.throwOptions = options;
            this.filesOpen = new HashSet<string>(DataStore.Comparer);
        }

        #endregion

        #region Private Methods

        private void Throw(
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            throw new System.IO.IOException("Operation not supported").StoreFileLine(filePath, memberName, lineNumber);
        }

        private void MarkFileOpen( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.FileShare) )
            {
                lock( this.filesOpen )
                {
                    if( this.filesOpen.Contains(dataStorePath) )
                        throw new InvalidOperationException("File already open!").Store("dataStorePath", dataStorePath);

                    this.filesOpen.Add(dataStorePath);
                }
            }
        }

        private void MarkFileClosed( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.FileShare) )
            {
                lock( this.filesOpen )
                {
                    if( !this.filesOpen.Contains(dataStorePath) )
                        throw new InvalidOperationException("File already closed!").Store("dataStorePath", dataStorePath);

                    this.filesOpen.Remove(dataStorePath);
                }
            }
        }

        #endregion

        #region IFileSystemBase

        /// <summary>
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        public bool EscapesNames
        {
            get { return this.fileSystem.EscapesNames; }
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostFilePath
        {
            get { return this.fileSystem.SupportsToHostFilePath; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        public string ToHostFilePath( string dataStorePath )
        {
            return this.fileSystem.ToHostFilePath(dataStorePath);
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostDirectoryPath
        {
            get { return this.fileSystem.SupportsToHostDirectoryPath; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        public string ToHostDirectoryPath( string dataStorePath )
        {
            return this.fileSystem.ToHostDirectoryPath(dataStorePath);
        }

        #endregion

        #region IFileSystemReader

        /// <summary>
        /// Gets the names of the files found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files.</param>
        /// <returns>The names of the files found.</returns>
        public string[] GetFileNames( string dataStorePath = "" )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.List) )
                this.Throw();

            return this.fileSystem.GetFileNames(dataStorePath);
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.List) )
                this.Throw();

            return this.fileSystem.GetDirectoryNames(dataStorePath);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        public ITextReader ReadText( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Read) )
                this.Throw();

            return new TextReaderWrapper(this, dataStorePath, this.fileSystem.ReadText(dataStorePath));
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        public IBinaryReader ReadBinary( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Read) )
                this.Throw();

            return new BinaryReaderWrapper(this, dataStorePath, this.fileSystem.ReadBinary(dataStorePath));
        }


        /// <summary>
        /// Gets a value indicating whether the GetFileSize method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsGetFileSize
        {
            get { return this.fileSystem.SupportsGetFileSize; }
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        public long GetFileSize( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.List) ) // List seems more appropriate than Read
                this.Throw();

            return this.fileSystem.GetFileSize(dataStorePath);
        }

        #endregion

        #region IFileSystemWriter

        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        public void CreateDirectory( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            this.fileSystem.CreateDirectory(dataStorePath);
        }

        /// <summary>
        /// Deletes the specified file. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to delete.</param>
        public void DeleteFile( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            this.fileSystem.DeleteFile(dataStorePath);
        }

        /// <summary>
        /// Deletes the specified directory. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to delete.</param>
        public void DeleteDirectory( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            this.fileSystem.DeleteDirectory(dataStorePath);
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        public ITextWriter CreateNewText( string dataStorePath, bool overwriteIfExists )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            return new TextWriterWrapper(this, dataStorePath, this.fileSystem.CreateNewText(dataStorePath, overwriteIfExists));
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateNewBinary( string dataStorePath, bool overwriteIfExists )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            return new BinaryWriterWrapper(this, dataStorePath, this.fileSystem.CreateNewBinary(dataStorePath, overwriteIfExists));
        }


        /// <summary>
        /// Gets a value indicating whether the CreateWriteThroughBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsCreateWriteThroughBinary
        {
            get { return this.fileSystem.SupportsCreateWriteThroughBinary; }
        }

        /// <summary>
        /// Creates a new empty file, and opens it for writing.
        /// No intermediate buffers are kept: all operations access the file directly.
        /// This hurts performance, but is important for log files (less is lost in case of a crash).
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <param name="overwriteIfExists"><c>true</c> to overwrite the file in case it already exists (like <see cref="System.IO.FileMode.Create"/>); or <c>false</c> to throw an exception (like <see cref="System.IO.FileMode.CreateNew"/>).</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateWriteThroughBinary( string dataStorePath, bool overwriteIfExists )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            return new BinaryWriterWrapper(this, dataStorePath, this.fileSystem.CreateWriteThroughBinary(dataStorePath, overwriteIfExists));
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsReadWriteBinary
        {
            get { return this.fileSystem.SupportsReadWriteBinary; }
        }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream ReadWriteBinary( string dataStorePath )
        {
            if( this.throwOptions.HasFlag(ThrowOptions.Write) )
                this.Throw();

            return new BinaryReaderWriterWrapper(this, dataStorePath, this.fileSystem.ReadWriteBinary(dataStorePath));
        }

        #endregion
    }
}
