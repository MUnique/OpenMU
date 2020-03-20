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
        /// Gets the boolean flag of the first byte of the span, considering bit shifting.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        /// <returns>The boolean flag.</returns>
        public static bool GetBoolean(this Span<byte> span, int leftShifted)
        {
            return ((span[0] >> leftShifted) & 1) == 1;
        }

        /// <summary>
        /// Gets the boolean flag of the first byte of the span.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <returns>The boolean flag.</returns>
        public static bool GetBoolean(this Span<byte> span)
        {
            return (span[0] & 1) == 1;
        }

        /// <summary>
        /// Sets the boolean flag to the first byte of the span, considering bit shifting.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="value">If set to <c>true</c>, the flag is set, otherwise it's cleared.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        public static void SetBoolean(this Span<byte> span, bool value, int leftShifted)
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
        /// Sets the boolean flag to the first byte of the span.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="value">If set to <c>true</c>, the flag is set, otherwise it's cleared.</param>
        public static void SetBoolean(this Span<byte> span, bool value)
        {
            const byte clearMask = 0b1111_1110;
            span[0] &= clearMask;
            if (value)
            {
                span[0] |= 1;
            }
        }

        /// <summary>
        /// Gets the byte value of the first byte of the span, by using only a defined number of bits and considering bit shifting.
        /// </summary>
        /// <param name="span">The span with at least 1 byte.</param>
        /// <param name="bits">The number of bits which are relevant to get this value.</param>
        /// <param name="leftShifted">The number of left shifted bits which were required to write the value into the byte.</param>
        /// <returns>The byte value.</returns>
        public static byte GetByteValue(this Span<byte> span, int bits, int leftShifted)
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
        public static void SetByteValue(this Span<byte> span, byte value, int bits, int leftShifted)
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

        /// <summary>
        /// Converts the byte span into a readable hexadecimal string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The hexadecimal string.</returns>
        public static string AsString(this Span<byte> bytes)
        {
            return BitConverter.ToString(bytes.ToArray()).Replace('-', ' ');
        }
    }
}
