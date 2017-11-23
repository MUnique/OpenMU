// <copyright file="NumberConversionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    /// <summary>
    /// Number conversion extensions.
    /// </summary>
    public static class NumberConversionExtensions
    {
        /// <summary>
        /// Converts 2 shorts to an 32bit unsigned Integer.
        /// </summary>
        /// <param name="lowWord">The low word.</param>
        /// <param name="highWord">The high word.</param>
        /// <returns>The unsigned integer.</returns>
        public static uint MakeDword(ushort lowWord, ushort highWord)
        {
            return (uint)(lowWord + (highWord << 0x10));
        }

        /// <summary>
        /// Converts 2 bytes to one 16bit unsigned short
        /// </summary>
        /// <param name="lowByte">The low byte.</param>
        /// <param name="highByte">The high byte.</param>
        /// <returns>The unsigned short.</returns>
        public static ushort MakeWord(byte lowByte, byte highByte)
        {
            return (ushort)(lowByte + ((highByte << 8) & 0xFF00));
        }

        /// <summary>
        /// Converts an Integer to an byte array, small endian.
        /// </summary>
        /// <param name="integer">The integer.</param>
        /// <returns>The bytes of the integer.</returns>
        public static byte[] ToBytesSmallEndian(this uint integer)
        {
            return new[] { (byte)(integer >> 24 & 0xFF), (byte)(integer >> 16 & 0xFF), (byte)(integer >> 8 & 0xFF), (byte)(integer & 0xFF) };
        }

        /// <summary>
        /// Converts an Integer to an byte array, big endian.
        /// </summary>
        /// <param name="integer">The integer.</param>
        /// <returns>The bytes of the integer.</returns>
        public static byte[] ToBytesBigEndian(this uint integer)
        {
            return new[] { (byte)(integer & 0xFF), (byte)(integer >> 8 & 0xFF), (byte)(integer >> 16 & 0xFF), (byte)(integer >> 24 & 0xFF) };
        }

        /// <summary>
        /// Converts an unsigned long to an byte array (small endian).
        /// </summary>
        /// <param name="value">The long value.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToBytesSmallEndian(this ulong value)
        {
            return new[]
            {
                (byte)(value / 0x100000000000000),
                (byte)(value / 0x1000000000000),
                (byte)(value / 0x10000000000),
                (byte)(value / 0x100000000),
                (byte)(value >> 24 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value & 0xFF)
            };
        }

        /// <summary>
        /// Swaps the bytes of an unsigned short value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value with swapped bytes</returns>
        public static ushort SwapBytes(this ushort value)
        {
            return (ushort)(((value & 0xFF) << 8) + ((value >> 8) & 0xFF));
        }

        /// <summary>
        /// Swaps the bytes of an unsigned integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value with swapped bytes.</returns>
        public static uint SwapBytes(this uint value)
        {
            return ((value << 24) & 0xFF000000) | ((value << 8) & 0xFF0000) | ((value >> 8) & 0xFF00) | ((value >> 24) & 0xFF);
        }

        /// <summary>
        /// Gets the high byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The high byte.</returns>
        public static byte GetHighByte(this ushort value)
        {
            return (byte)(value >> 8 & 0xFF);
        }

        /// <summary>
        /// Gets the low byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The low byte.</returns>
        public static byte GetLowByte(this ushort value)
        {
            return (byte)(value & 0xFF);
        }

        /// <summary>
        /// Converts the signed short to an unsigned short.
        /// </summary>
        /// <param name="value">The signed value.</param>
        /// <returns>The unsigned value.</returns>
        public static ushort ToUnsigned(this short value)
        {
            return unchecked((ushort)value);
        }

        /// <summary>
        /// Converts the unsigned short to a signed short.
        /// </summary>
        /// <param name="value">The usigned value.</param>
        /// <returns>The signed value.</returns>
        public static short ToSigned(this ushort value)
        {
            return unchecked((short)value);
        }

        /// <summary>
        /// Gets the byte of an integer at a specific index position.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns>The byte of the integer at the specific index position.</returns>
        public static byte GetByte(this uint value, int index)
        {
            return (byte)((value >> (8 * index)) & 0xFF);
        }
    }
}