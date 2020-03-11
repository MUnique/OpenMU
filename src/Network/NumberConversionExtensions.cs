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
        /// Converts 4 bytes to an 32bit unsigned Integer.
        /// </summary>
        /// <param name="lowest">The lowest.</param>
        /// <param name="lower">The lower.</param>
        /// <param name="higher">The higher.</param>
        /// <param name="highest">The highest.</param>
        /// <returns>
        /// The unsigned integer.
        /// </returns>
        public static uint MakeDword(byte lowest, byte lower, byte higher, byte highest)
        {
            return (uint)(MakeWord(lowest, lower) + (MakeWord(higher, highest) << 0x10));
        }

        /// <summary>
        /// Converts 2 bytes to one 16bit unsigned short.
        /// </summary>
        /// <param name="lowByte">The low byte.</param>
        /// <param name="highByte">The high byte.</param>
        /// <returns>The unsigned short.</returns>
        public static ushort MakeWord(byte lowByte, byte highByte)
        {
            return (ushort)(lowByte + ((highByte << 8) & 0xFF00));
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