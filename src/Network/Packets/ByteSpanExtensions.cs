// <copyright file="ByteSpanExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets
{
    using System;
    using System.Buffers;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extensions for byte spans to read and write data.
    /// These may be called by the generated code, so please don't delete unused ones - they could be used in the future.
    /// </summary>
    public static class ByteSpanExtensions
    {
        /// <summary>
        /// Gets the short value by using little endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The short value.</returns>
        public static ushort GetShortLittleEndian(this Span<byte> data)
        {
            return unchecked((ushort)(data[1] | (data[0] << 8)));
        }

        /// <summary>
        /// Gets the short value by using big endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The short value.</returns>
        public static ushort GetShortBigEndian(this Span<byte> data)
        {
            return unchecked((ushort)(data[0] | (data[1] << 8)));
        }

        /// <summary>
        /// Sets the short value by using little endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        public static void SetShortLittleEndian(this Span<byte> data, ushort value)
        {
            data[0] = (byte)((value >> 8) & 0xFF);
            data[1] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the short value by using big endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        public static void SetShortBigEndian(this Span<byte> data, ushort value)
        {
            data[0] = (byte)(value & 0xFF);
            data[1] = (byte)((value >> 8) & 0xFF);
        }

        /// <summary>
        /// Gets the integer value by using little endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The integer value.</returns>
        public static uint GetIntegerLittleEndian(this Span<byte> data)
        {
            return unchecked((uint)(data[3] | (data[2] << 8) | (data[1] << 16) | (data[0] << 24)));
        }

        /// <summary>
        /// Gets the integer value by using big endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The integer value.</returns>
        public static uint GetIntegerBigEndian(this Span<byte> data)
        {
            return unchecked((uint)(data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24)));
        }

        /// <summary>
        /// Sets the integer value by using little endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        public static void SetIntegerLittleEndian(this Span<byte> data, uint value)
        {
            data[0] = (byte)((value >> 24) & 0xFF);
            data[1] = (byte)((value >> 16) & 0xFF);
            data[2] = (byte)((value >> 8) & 0xFF);
            data[3] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the integer value by using big endian byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        public static void SetIntegerBigEndian(this Span<byte> data, uint value)
        {
            data[0] = (byte)(value & 0xFF);
            data[1] = (byte)((value >> 8) & 0xFF);
            data[2] = (byte)((value >> 16) & 0xFF);
            data[3] = (byte)((value >> 24) & 0xFF);
        }

        /// <summary>
        /// Converting bytes of a byte span to an 64bit unsigned Integer, little endian.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns>The value as long.</returns>
        public static ulong GetLongLittleEndian(this Span<byte> span)
        {
            ulong result = 0;
            ulong multiplier = 0x100000000000000;
            for (int i = 0; i < 8; i++)
            {
                result += span[i] * multiplier; // byte shifting is not possible here, because a byte shift operation always returns an integer.
                multiplier /= 0x100;
            }

            return result;
        }

        /// <summary>
        /// Converting bytes of a byte span to an 64bit unsigned Integer, big endian.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns>The value as long.</returns>
        public static ulong GetLongBigEndian(this Span<byte> span)
        {
            ulong result = 0;
            ulong multiplier = 0x100000000000000;
            for (int i = 7; i >= 0; i--)
            {
                result += span[i] * multiplier; // byte shifting is not possible here, because a byte shift operation always returns an integer.
                multiplier /= 0x100;
            }

            return result;
        }

        /// <summary>
        /// Sets the bytes of a long value to an byte array at the specified span in big endian.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The long value.</param>
        public static void SetLongBigEndian(this Span<byte> array, ulong value)
        {
            if (array.Length < 8)
            {
                throw new ArgumentException("span is too small.", nameof(array));
            }

            array[7] = (byte)((value / 0x100000000000000) & 0xFF);
            array[6] = (byte)((value / 0x1000000000000) & 0xFF);
            array[5] = (byte)((value / 0x10000000000) & 0xFF);
            array[4] = (byte)((value / 0x100000000) & 0xFF);
            array[3] = (byte)((value >> 24) & 0xFF);
            array[2] = (byte)((value >> 16) & 0xFF);
            array[1] = (byte)((value >> 8) & 0xFF);
            array[0] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the bytes of a long value to an byte array at the specified span in little endian.
        /// </summary>
        /// <param name="span">The target span.</param>
        /// <param name="value">The long value.</param>
        public static void SetLongLittleEndian(this Span<byte> span, ulong value)
        {
            if (span.Length < 8)
            {
                throw new ArgumentException("span is too small.", nameof(span));
            }

            span[0] = (byte)((value / 0x100000000000000) & 0xFF);
            span[1] = (byte)((value / 0x1000000000000) & 0xFF);
            span[2] = (byte)((value / 0x10000000000) & 0xFF);
            span[3] = (byte)((value / 0x100000000) & 0xFF);
            span[4] = (byte)((value >> 24) & 0xFF);
            span[5] = (byte)((value >> 16) & 0xFF);
            span[6] = (byte)((value >> 8) & 0xFF);
            span[7] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Gets the boolean flag of the first byte of the span, considering bit shifting.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        /// <returns>The boolean flag.</returns>
        public static bool GetBoolean(this Span<byte> span, int leftShifted = 0)
        {
            return ((span[0] >> leftShifted) & 1) == 1;
        }

        /// <summary>
        /// Sets the boolean flag to the first byte of the span, considering bit shifting.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="value">If set to <c>true</c>, the flag is set, otherwise it's cleared.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        public static void SetBoolean(this Span<byte> span, bool value, int leftShifted = 0)
        {
            var mask = (byte)(1 << leftShifted);
            var clearMask = (byte)(0xFF - (1 << leftShifted));
            span[0] &= clearMask;
            if (value)
            {
                span[0] |= mask;
            }
        }

        /// <summary>
        /// Gets the byte value of the first byte of the span, by using only a defined number of bits and considering bit shifting.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="bits">The number of bits which are relevant to get this value.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        /// <returns>The byte value.</returns>
        public static byte GetByteValue(this Span<byte> span, int bits = 8, int leftShifted = 0)
        {
            var andMask = (byte)(Math.Pow(2, bits) - 1);
            var numericalValue = (byte)((span[0] >> leftShifted) & andMask);

            // Unfortunately, it's not possible without boxing it into an object :-(
            return numericalValue;
        }

        /// <summary>
        /// Sets the byte value of the first byte of the span, by using only a defined number of bits and considering bit shifting.
        /// It will not modify the other bits, when the <paramref name="bits"/> is smaller than 8.
        /// </summary>
        /// <param name="span">The span with at least one byte.</param>
        /// <param name="value">The value which should be set.</param>
        /// <param name="bits">The number of bits which are relevant to set this value in the target byte.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        public static void SetByteValue(this Span<byte> span, byte value, int bits = 8, int leftShifted = 0)
        {
            var bitMask = (byte)(Math.Pow(2, bits) - 1) << leftShifted;
            var clearMask = (byte)(0xFF - bitMask);

            span[0] &= clearMask;

            var numericalValue = Convert.ToByte(value);
            span[0] |= (byte)((numericalValue << leftShifted) & bitMask);
        }

        /// <summary>
        /// Extracts a string of a byte span with the specified encoding.
        /// </summary>
        /// <param name="span">The byte span.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="maximumBytes">The maximum length.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The resulting string.</returns>
        /// <remarks>This is not optimal yet, since it creates a new byte array. We might wait until encoding works on spans.</remarks>
        public static string ExtractString(this Span<byte> span, int startIndex, int maximumBytes, Encoding encoding)
        {
            var content = span.Slice(startIndex, maximumBytes).ToArray();
            int count = content.TakeWhile(b => b != 0).Count();
            return encoding.GetString(content, 0, count);
        }

        /// <summary>
        /// Writes the string into the given span.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="text">The text.</param>
        /// <param name="encoding">The encoding.</param>
        public static void WriteString(this Span<byte> target, string text, Encoding encoding)
        {
            target.Clear();
            var array = ArrayPool<byte>.Shared.Rent(encoding.GetByteCount(text));
            try
            {
                var size = encoding.GetBytes(text, 0, text.Length, array, 0);
                array.AsSpan(0, size).CopyTo(target);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array, true);
            }
        }
    }
}
