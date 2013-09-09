using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    /// <summary>
    /// Converts basic data types to and from byte arrays, in little endian format.
    /// </summary>
    public static class BitConverterLE
    {
        //// NOTE: byte is trivial
        ////       sbyte is simple casting

        //// NOTE: char is not handled, as it should be encoding specific
        ////       (but converting to short should work as well)

        #region Private Members

        //// NOTE: these structures help us avoid unsafe code

        [StructLayout(LayoutKind.Explicit)]
        private struct IntFloat
        {
            [FieldOffset(0)]
            public float FloatValue;

            [FieldOffset(0)]
            public int IntValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct LongDouble
        {
            [FieldOffset(0)]
            public double DoubleValue;

            [FieldOffset(0)]
            public long LongValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct IntDecimal
        {
            [FieldOffset(0)]
            public decimal DecimalValue;

            [FieldOffset(0)]
            public int Lo;

            [FieldOffset(4)]
            public int Mid;

            [FieldOffset(8)]
            public int Hi;

            [FieldOffset(12)]
            public int Flags;
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void CheckArgs( byte[] array, int startIndex, int count )
        {
            if( array.NullReference() )
                throw new ArgumentNullException().Store("startIndex", startIndex);

            if( count <= 0
             || startIndex < 0
             || startIndex + count > array.Length )
                throw new ArgumentOutOfRangeException().Store("startIndex", startIndex).Store("count", count).Store("array.Length", array.Length);
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void ConditionalReverse( byte[] array, int startIndex, int count )
        {
            if( !BitConverter.IsLittleEndian )
            {
                int i = startIndex;
                int j = startIndex + count - 1;
                byte tmp;
                while( i < j )
                {
                    tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                    i++;
                    j--;
                }
            }
        }

        #endregion

        #region GetBytes

        /// <summary>
        /// Gets the binary representation of the specified 16-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( short value, byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 2);

            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            ConditionalReverse(array, startIndex, count: 2);
        }

        /// <summary>
        /// Gets the binary representation of the specified 16-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( short value )
        {
            var array = new byte[2];
            GetBytes(value, array, startIndex: 0);
            return array;
        }

        /// <summary>
        /// Gets the binary representation of the specified 16-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( ushort value, byte[] array, int startIndex )
        {
            GetBytes((short)value, array, startIndex);
        }

        /// <summary>
        /// Gets the binary representation of the specified 16-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( ushort value )
        {
            return GetBytes((short)value);
        }

        /// <summary>
        /// Gets the binary representation of the specified 32-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( int value, byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 4);

            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            array[startIndex + 2] = (byte)(value >> 16);
            array[startIndex + 3] = (byte)(value >> 24);
            ConditionalReverse(array, startIndex, count: 4);
        }

        /// <summary>
        /// Gets the binary representation of the specified 32-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( int value )
        {
            var array = new byte[4];
            GetBytes(value, array, startIndex: 0);
            return array;
        }

        /// <summary>
        /// Gets the binary representation of the specified 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( uint value, byte[] array, int startIndex )
        {
            GetBytes((int)value, array, startIndex);
        }

        /// <summary>
        /// Gets the binary representation of the specified 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( uint value )
        {
            return GetBytes((int)value);
        }

        /// <summary>
        /// Gets the binary representation of the specified 64-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( long value, byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 8);

            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            array[startIndex + 2] = (byte)(value >> 16);
            array[startIndex + 3] = (byte)(value >> 24);
            array[startIndex + 4] = (byte)(value >> 32);
            array[startIndex + 5] = (byte)(value >> 40);
            array[startIndex + 6] = (byte)(value >> 48);
            array[startIndex + 7] = (byte)(value >> 56);
            ConditionalReverse(array, startIndex, count: 8);
        }

        /// <summary>
        /// Gets the binary representation of the specified 64-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( long value )
        {
            var array = new byte[8];
            GetBytes(value, array, startIndex: 0);
            return array;
        }

        /// <summary>
        /// Gets the binary representation of the specified 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( ulong value, byte[] array, int startIndex )
        {
            GetBytes((long)value, array, startIndex);
        }

        /// <summary>
        /// Gets the binary representation of the specified 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( ulong value )
        {
            return GetBytes((long)value);
        }

        /// <summary>
        /// Gets the binary representation of the specified single-precision floating point value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( float value, byte[] array, int startIndex )
        {
            var converter = new IntFloat();
            converter.FloatValue = value;
            GetBytes(converter.IntValue, array, startIndex);
        }

        /// <summary>
        /// Gets the binary representation of the specified single-precision floating point value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( float value )
        {
            var converter = new IntFloat();
            converter.FloatValue = value;
            return GetBytes(converter.IntValue);
        }

        /// <summary>
        /// Gets the binary representation of the specified double-precision floating point value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( double value, byte[] array, int startIndex )
        {
            var converter = new LongDouble();
            converter.DoubleValue = value;
            GetBytes(converter.LongValue, array, startIndex);
        }

        /// <summary>
        /// Gets the binary representation of the specified double-precision floating point value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( double value )
        {
            var converter = new LongDouble();
            converter.DoubleValue = value;
            return GetBytes(converter.LongValue);
        }

        /// <summary>
        /// Gets the binary representation of the specified <see cref="Decimal"/> number.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( decimal value, byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 16);

            var d = new IntDecimal();
            d.DecimalValue = value;
            array[0] = (byte)d.Lo;
            array[1] = (byte)(d.Lo >> 8);
            array[2] = (byte)(d.Lo >> 16);
            array[3] = (byte)(d.Lo >> 24);

            array[4] = (byte)d.Mid;
            array[5] = (byte)(d.Mid >> 8);
            array[6] = (byte)(d.Mid >> 16);
            array[7] = (byte)(d.Mid >> 24);

            array[8] = (byte)d.Hi;
            array[9] = (byte)(d.Hi >> 8);
            array[10] = (byte)(d.Hi >> 16);
            array[11] = (byte)(d.Hi >> 24);

            array[12] = (byte)d.Flags;
            array[13] = (byte)(d.Flags >> 8);
            array[14] = (byte)(d.Flags >> 16);
            array[15] = (byte)(d.Flags >> 24);
            ConditionalReverse(array, startIndex, count: 16);
        }

        /// <summary>
        /// Gets the binary representation of the specified <see cref="Decimal"/> number.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( decimal value )
        {
            var array = new byte[16];
            GetBytes(value, array, startIndex: 0);
            return array;
        }

        /// <summary>
        /// Gets the binary representation of the specified <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( bool value, byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 1);

            array[startIndex] = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Gets the binary representation of the specified <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( bool value )
        {
            var array = new byte[1];
            GetBytes(value, array, startIndex: 0);
            return array;
        }

        /// <summary>
        /// Gets the binary representation of the specified <see cref="Char"/> value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        public static void GetBytes( char value, byte[] array, int startIndex )
        {
            GetBytes((short)value, array, startIndex);
        }

        /// <summary>
        /// Gets the binary representation of the specified <see cref="Char"/> value.
        /// </summary>
        /// <param name="value">The value to convert to bytes.</param>
        /// <returns>The array that stores the binary representation.</returns>
        public static byte[] GetBytes( char value )
        {
            return GetBytes((short)value);
        }

        #endregion

        #region To*

        /// <summary>
        /// Gets the 16-bit signed integer from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static short ToInt16( byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 2);

            ConditionalReverse(array, startIndex, count: 2);
            return (short)(array[startIndex] | array[startIndex + 1] << 8);
        }

        /// <summary>
        /// Gets the 16-bit unsigned integer from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static ushort ToUInt16( byte[] array, int startIndex )
        {
            return (ushort)ToInt16(array, startIndex);
        }

        /// <summary>
        /// Gets the 32-bit signed integer from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static int ToInt32( byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 4);

            ConditionalReverse(array, startIndex, count: 4);
            return (int)(array[startIndex] | array[startIndex + 1] << 8 | array[startIndex + 2] << 16 | array[startIndex + 3] << 24);
        }

        /// <summary>
        /// Gets the 32-bit unsigned integer from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static uint ToUInt32( byte[] array, int startIndex )
        {
            return (uint)ToInt32(array, startIndex);
        }

        /// <summary>
        /// Gets the 64-bit signed integer from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static long ToInt64( byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 8);

            ConditionalReverse(array, startIndex, count: 8);
            uint lo = (uint)(array[startIndex] | array[startIndex + 1] << 8 | array[startIndex + 2] << 16 | array[startIndex + 3] << 24);
            uint hi = (uint)(array[startIndex + 4] | array[startIndex + 5] << 8 | array[startIndex + 6] << 16 | array[startIndex + 7] << 24);
            return lo | ((long)hi << 32);
        }

        /// <summary>
        /// Gets the 64-bit unsigned integer from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static ulong ToUInt64( byte[] array, int startIndex )
        {
            return (ulong)ToInt64(array, startIndex);
        }

        /// <summary>
        /// Gets the single-precision floating point value from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static float ToSingle( byte[] array, int startIndex )
        {
            var converter = new IntFloat();
            converter.IntValue = ToInt32(array, startIndex);
            return converter.FloatValue;
        }

        /// <summary>
        /// Gets the double-precision floating point value from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static double ToDouble( byte[] array, int startIndex )
        {
            var converter = new LongDouble();
            converter.LongValue = ToInt64(array, startIndex);
            return converter.DoubleValue;
        }

        /// <summary>
        /// Gets the <see cref="Decimal"/> number from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static decimal ToDecimal( byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 16);

            ConditionalReverse(array, startIndex, count: 16);
            var converter = new IntDecimal();
            converter.Lo = ((int)array[0]) | ((int)array[1] << 8) | ((int)array[2] << 16) | ((int)array[3] << 24);
            converter.Mid = ((int)array[4]) | ((int)array[5] << 8) | ((int)array[6] << 16) | ((int)array[7] << 24);
            converter.Hi = ((int)array[8]) | ((int)array[9] << 8) | ((int)array[10] << 16) | ((int)array[11] << 24);
            converter.Flags = ((int)array[12]) | ((int)array[13] << 8) | ((int)array[14] << 16) | ((int)array[15] << 24);
            return converter.DecimalValue;
        }

        /// <summary>
        /// Gets the <see cref="Boolean"/> value from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static bool ToBoolean( byte[] array, int startIndex )
        {
            CheckArgs(array, startIndex, count: 8);

            return array[startIndex] != 0;
        }

        /// <summary>
        /// Gets the <see cref="Char"/> value from the binary representation.
        /// </summary>
        /// <param name="array">The array that stores the binary representation.</param>
        /// <param name="startIndex">The starting position within <paramref name="array"/>.</param>
        /// <returns>The value found.</returns>
        public static char ToChar( byte[] array, int startIndex )
        {
            return (char)ToInt16(array, startIndex);
        }

        #endregion
    }
}
