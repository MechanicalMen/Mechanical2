using System;
using Mechanical.Core;

namespace Mechanical.IO
{
    /// <summary>
    /// Writes to a(n abstract) byte stream.
    /// </summary>
    public interface IBinaryWriter : IStreamBase
    {
        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes the byte array to the data store value.
        /// </summary>
        /// <param name="array">The byte array to write data from.</param>
        /// <param name="offset">The position at which to start retrieving data.</param>
        /// <param name="count">The number of bytes to write.</param>
        void Write( byte[] array, int offset, int count );

        /// <summary>
        /// Writes a(n) <see cref="SByte"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( sbyte value );

        /// <summary>
        /// Writes a(n) <see cref="Byte"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( byte value );

        /// <summary>
        /// Writes a(n) <see cref="Int16"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( short value );

        /// <summary>
        /// Writes a(n) <see cref="UInt16"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( ushort value );

        /// <summary>
        /// Writes a(n) <see cref="Int32"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( int value );

        /// <summary>
        /// Writes a(n) <see cref="UInt32"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( uint value );

        /// <summary>
        /// Writes a(n) <see cref="Int64"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( long value );

        /// <summary>
        /// Writes a(n) <see cref="UInt64"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( ulong value );

        /// <summary>
        /// Writes a(n) <see cref="Single"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( float value );

        /// <summary>
        /// Writes a(n) <see cref="Double"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( double value );

        /// <summary>
        /// Writes a(n) <see cref="Decimal"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( decimal value );

        /// <summary>
        /// Writes a(n) <see cref="Boolean"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( bool value );

        /// <summary>
        /// Writes a(n) <see cref="Char"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( char value );

        /// <summary>
        /// Writes a(n) <see cref="String"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( string value );

        /// <summary>
        /// Writes a(n) <see cref="Substring"/> to the data store value.
        /// </summary>
        /// <param name="value">The object to write to the data store value.</param>
        void Write( Substring value );
    }
}
