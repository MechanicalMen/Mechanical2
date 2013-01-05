using System;

namespace Mechanical.Core
{
    /// <summary>
    /// Converts basic data types to and from byte arrays, in little endian format.
    /// </summary>
    public static class BitConverterLE
    {
        #region GetBytes

        /// <summary>
        /// Returns the specified 16-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public static byte[] GetBytes( short value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public static byte[] GetBytes( ushort value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public static byte[] GetBytes( int value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public static byte[] GetBytes( uint value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 64-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public static byte[] GetBytes( long value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public static byte[] GetBytes( ulong value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified single-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public static byte[] GetBytes( float value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified double-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public static byte[] GetBytes( double value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Returns the specified decimal number as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 16.</returns>
        public static byte[] GetBytes( decimal value )
        {
            int[] bits = decimal.GetBits(value);

            var bytes = new byte[16];
            Array.Copy(GetBytes(bits[0]), 0, bytes, 0, 4);
            Array.Copy(GetBytes(bits[1]), 0, bytes, 4, 4);
            Array.Copy(GetBytes(bits[2]), 0, bytes, 8, 4);
            Array.Copy(GetBytes(bits[3]), 0, bytes, 12, 4);
            return bytes;
        }

        /// <summary>
        /// Returns the specified <see cref="Boolean"/> value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 1.</returns>
        public static byte[] GetBytes( bool value )
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Returns the specified Unicode character value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public static byte[] GetBytes( char value )
        {
            var bytes = BitConverter.GetBytes(value);
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(bytes);
            return bytes;
        }

        #endregion

        #region To*

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        public static short ToInt16( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 2);
            return BitConverter.ToInt16(value, startIndex);
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        public static ushort ToUInt16( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 2);
            return BitConverter.ToUInt16(value, startIndex);
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
        public static int ToInt32( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 4);
            return BitConverter.ToInt32(value, startIndex);
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
        public static uint ToUInt32( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 4);
            return BitConverter.ToUInt32(value, startIndex);
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
        public static long ToInt64( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 8);
            return BitConverter.ToInt64(value, startIndex);
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
        public static ulong ToUInt64( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 8);
            return BitConverter.ToUInt64(value, startIndex);
        }

        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A single-precision floating point number formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
        public static float ToSingle( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 4);
            return BitConverter.ToSingle(value, startIndex);
        }

        /// <summary>
        /// Returns a double-precision floating point number converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A double precision floating point number formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
        public static double ToDouble( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 8);
            return BitConverter.ToDouble(value, startIndex);
        }

        /// <summary>
        /// Returns a decimal number converted from sixteen bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A decimal number formed by sixteen bytes beginning at <paramref name="startIndex"/>.</returns>
        public static decimal ToDecimal( byte[] value, int startIndex )
        {
            var bits = new int[]
            {
                ToInt32(value, startIndex),
                ToInt32(value, startIndex + 4),
                ToInt32(value, startIndex + 8),
                ToInt32(value, startIndex + 12)
            };
            return new decimal(bits);
        }

        /// <summary>
        /// Returns a Boolean value converted from one byte at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns><c>true</c> if the byte at <paramref name="startIndex"/> in <paramref name="value"/> is nonzero; otherwise, <c>false</c>.</returns>
        public static bool ToBoolean( byte[] value, int startIndex )
        {
            return BitConverter.ToBoolean(value, startIndex);
        }

        /// <summary>
        /// Returns a Unicode character converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A character formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        public static char ToChar( byte[] value, int startIndex )
        {
            if( !BitConverter.IsLittleEndian )
                Array.Reverse(value, startIndex, 2);
            return BitConverter.ToChar(value, startIndex);
        }

        #endregion
    }
}
