using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Helps implementing <see cref="IBinaryWriter"/>.
    /// Uses the little endian format.
    /// </summary>
    public abstract class BinaryWriterBaseLE : IBinaryWriter
    {
        #region Private Fields

        private readonly byte[] buffer = new byte[16];

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWriterBaseLE"/> class.
        /// </summary>
        protected BinaryWriterBaseLE()
        {
        }

        #endregion

        #region IBinaryWriter

        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Writes the byte array to the data store value.
        /// </summary>
        /// <param name="array">The byte array to write data from.</param>
        /// <param name="offset">The position at which to start retrieving data.</param>
        /// <param name="count">The number of bytes to write.</param>
        public abstract void Write( byte[] array, int offset, int count );

        /// <summary>
        /// Writes an <see cref="SByte"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( sbyte value )
        {
            this.buffer[0] = (byte)value;
            this.Write(this.buffer, offset: 0, count: 1);
        }

        /// <summary>
        /// Writes an <see cref="Byte"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( byte value )
        {
            this.buffer[0] = value;
            this.Write(this.buffer, offset: 0, count: 1);
        }

        /// <summary>
        /// Writes an <see cref="Int16"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( short value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 2);
        }

        /// <summary>
        /// Writes an <see cref="UInt16"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( ushort value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 2);
        }

        /// <summary>
        /// Writes an <see cref="Int32"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( int value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 4);
        }

        /// <summary>
        /// Writes an <see cref="UInt32"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( uint value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 4);
        }

        /// <summary>
        /// Writes an <see cref="Int64"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( long value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 8);
        }

        /// <summary>
        /// Writes an <see cref="UInt64"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( ulong value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 8);
        }

        /// <summary>
        /// Writes an <see cref="Single"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( float value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 4);
        }

        /// <summary>
        /// Writes an <see cref="Double"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( double value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 8);
        }

        /// <summary>
        /// Writes an <see cref="Decimal"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( decimal value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 16);
        }

        /// <summary>
        /// Writes an <see cref="Boolean"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( bool value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 1);
        }

        /// <summary>
        /// Writes an <see cref="Char"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( char value )
        {
            BitConverterLE.GetBytes(value, this.buffer, startIndex: 0);
            this.Write(this.buffer, offset: 0, count: 2);
        }

        /// <summary>
        /// Writes an <see cref="String"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( string value )
        {
            if( value.NullReference() )
                throw new ArgumentNullException("value").StoreFileLine();

            this.Write(value.Length);
            for( int i = 0; i < value.Length; ++i )
                this.Write(value[i]);
        }

        /// <summary>
        /// Writes a(n) <see cref="Substring"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        public void Write( Substring value )
        {
            if( value.Origin.NullReference() )
                throw new ArgumentNullException("value").StoreFileLine();

            this.Write(value.Length);
            for( int i = 0; i < value.Length; ++i )
                this.Write(value[i]);
        }

        #endregion
    }
}
