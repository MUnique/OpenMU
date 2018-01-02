// <copyright file="ArrayExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extensions for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Sets the values of an array. Starting at the first element, values are copied into the array.
        /// </summary>
        /// <typeparam name="T">The generic type of the array.</typeparam>
        /// <param name="array">The array which values should be set.</param>
        /// <param name="values">The values which should be set.</param>
        /// <returns>The <paramref name="array"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">args.Length > array.Length</exception>
        public static T[] SetValues<T>(this T[] array, params T[] values)
        {
            if (values.Length > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(values), $"{nameof(values)}.Length > {nameof(array)}.Length");
            }

            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i];
            }

            return array;
        }

        /// <summary>
        /// Extracts a string of an byte array with the specified encoding.
        /// </summary>
        /// <param name="array">The byte array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="maximumBytes">The maximum length.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The resulting string.</returns>
        public static string ExtractString(this byte[] array, int startIndex, int maximumBytes, Encoding encoding)
        {
            int count = array.Skip(startIndex).Take(maximumBytes).TakeWhile(b => b != 0).Count();
            return encoding.GetString(array, startIndex, count);
        }

        /// <summary>
        /// Determines whether the specified other array is equal.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="otherArray">The other array.</param>
        /// <returns>True, if all elements of the other array match with this array.</returns>
        public static bool IsEqual(this byte[] array, byte[] otherArray)
        {
            if (array.Length != otherArray.Length)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != otherArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the byte array into a readable HEX string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The HEX string.</returns>
        public static string AsString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        /// <summary>
        /// Converting bytes of an array to an 64bit unsigned Integer, small endian.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>The qword.</returns>
        public static ulong MakeQword(this byte[] array, int startIndex)
        {
            ulong result = 0;
            ulong multiplier = 0x100000000000000;
            for (int i = 0; i < 8; i++)
            {
                result += array[startIndex + i] * multiplier; // byte shifting is not possible here, because a byte shift operation always returns an integer.
                multiplier /= 0x100;
            }

            return result;
        }

        /// <summary>
        /// Converts bytes of an array to an 32bit unsigned Integer, big endian.
        /// </summary>
        /// <param name="array">Byte array.</param>
        /// <param name="startIndex">Starting index. The array needs 3 more elements.</param>
        /// <returns>An unsigned integer.</returns>
        public static uint MakeDwordBigEndian(this byte[] array, int startIndex)
        {
            return unchecked(((uint)array[startIndex + 3] << 24) | (uint)(array[startIndex + 2] << 16) | (uint)(array[startIndex + 1] << 8) | array[startIndex]);
        }

        /// <summary>
        /// Converts bytes of an array to an 32bit unsigned Integer, small endian.
        /// </summary>
        /// <param name="array">Byte array.</param>
        /// <param name="startIndex">Starting index. The array needs 3 more elements.</param>
        /// <returns>An unsigned integer.</returns>
        public static uint MakeDwordSmallEndian(this byte[] array, int startIndex)
        {
            return unchecked(((uint)array[startIndex] << 24) | (uint)(array[startIndex + 1] << 16) | (uint)(array[startIndex + 2] << 8) | array[startIndex + 3]);
        }

        /// <summary>
        /// Converts bytes of an array to an unsigned short, small endian.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>An unsigned short.</returns>
        public static ushort MakeWordSmallEndian(this byte[] array, int startIndex)
        {
            return (ushort)((array[startIndex] << 8) | array[startIndex + 1]);
        }

        /// <summary>
        /// Converts bytes of an array to an unsigned short, small endian.
        /// If the array is not long enough, it returns 0.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>An unsigned short.</returns>
        public static ushort TryMakeWordSmallEndian(this byte[] array, int startIndex)
        {
            return array.Length > startIndex + 1 ? array.MakeWordSmallEndian(startIndex) : default(ushort);
        }

        /// <summary>
        /// Converts bytes of an array to an unsigned short, big endian.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>An unsigned short.</returns>
        public static ushort MakeWordBigEndian(this byte[] array, int startIndex)
        {
            return (ushort)((array[startIndex + 1] << 8) | array[startIndex]);
        }

        /// <summary>
        /// Converts bytes of an array to an unsigned short, big endian.
        /// If the array is not long enough, it returns 0.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>An unsigned short.</returns>
        public static ushort TryMakeWordBigEndian(this byte[] array, int startIndex)
        {
            return array.Length > startIndex + 1 ? array.MakeWordBigEndian(startIndex) : default(ushort);
        }

        /// <summary>
        /// Sets the bytes of an integer (small endian) to an byte array at the specified start index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The integer value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static void SetIntegerSmallEndian(this byte[] array, uint value, int startIndex)
        {
            if (array.Length < startIndex + 4)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            array[startIndex] = (byte)((value >> 24) & 0xFF);
            array[startIndex + 1] = (byte)((value >> 16) & 0xFF);
            array[startIndex + 2] = (byte)((value >> 8) & 0xFF);
            array[startIndex + 3] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the bytes of an integer (big endian) to an byte array at the specified start index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The integer value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static void SetIntegerBigEndian(this byte[] array, uint value, int startIndex)
        {
            if (array.Length < startIndex + 4)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            array[startIndex + 3] = (byte)((value >> 24) & 0xFF);
            array[startIndex + 2] = (byte)((value >> 16) & 0xFF);
            array[startIndex + 1] = (byte)((value >> 8) & 0xFF);
            array[startIndex] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the bytes of a long (big endian) to an byte array at the specified start index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The long value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static void SetLongBigEndian(this byte[] array, long value, int startIndex)
        {
            if (array.Length < startIndex + 8)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            array[startIndex + 7] = (byte)((value / 0x100000000000000) & 0xFF);
            array[startIndex + 6] = (byte)((value / 0x1000000000000) & 0xFF);
            array[startIndex + 5] = (byte)((value / 0x10000000000) & 0xFF);
            array[startIndex + 4] = (byte)((value / 0x100000000) & 0xFF);
            array[startIndex + 3] = (byte)((value >> 24) & 0xFF);
            array[startIndex + 2] = (byte)((value >> 16) & 0xFF);
            array[startIndex + 1] = (byte)((value >> 8) & 0xFF);
            array[startIndex] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the bytes of a long (small endian) to an byte array at the specified start index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The long value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static void SetLongSmallEndian(this byte[] array, long value, int startIndex)
        {
            if (array.Length < startIndex + 8)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            array[startIndex] = (byte)((value / 0x100000000000000) & 0xFF);
            array[startIndex + 1] = (byte)((value / 0x1000000000000) & 0xFF);
            array[startIndex + 2] = (byte)((value / 0x10000000000) & 0xFF);
            array[startIndex + 3] = (byte)((value / 0x100000000) & 0xFF);
            array[startIndex + 4] = (byte)((value >> 24) & 0xFF);
            array[startIndex + 5] = (byte)((value >> 16) & 0xFF);
            array[startIndex + 6] = (byte)((value >> 8) & 0xFF);
            array[startIndex + 7] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Sets the bytes of a short (small endian) to an byte array at the specified start index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The short value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static void SetShortSmallEndian(this byte[] array, ushort value, int startIndex)
        {
            if (array.Length < startIndex + 2)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            array[startIndex] = value.GetHighByte();
            array[startIndex + 1] = value.GetLowByte();
        }

        /// <summary>
        /// Sets the bytes of a short (big endian) to an byte array at the specified start index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="value">The short value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static void SetShortBigEndian(this byte[] array, ushort value, int startIndex)
        {
            if (array.Length < startIndex + 2)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            array[startIndex + 1] = value.GetHighByte();
            array[startIndex] = value.GetLowByte();
        }

        /// <summary>
        /// Gets the size of the packet header.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The size of the header.</returns>
        public static int GetPacketHeaderSize(this byte[] packet)
        {
            switch (packet[0])
            {
                case 0xC1:
                case 0xC3:
                    return 2;
                case 0xC2:
                case 0xC4:
                    return 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the type of the packet. This only works when the packet type is not encrypted.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The type of the packet.</returns>
        public static byte GetPacketType(this byte[] packet)
        {
            return packet[packet.GetPacketHeaderSize()];
        }

        /// <summary>
        /// Gets the sub type of the packet. This only works when the packet type is not encrypted.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The sub type of the packet.</returns>
        public static byte GetPacketSubType(this byte[] packet)
        {
            return packet[packet.GetPacketHeaderSize() + 1];
        }

        /// <summary>
        /// Gets the size of a packet from its header.
        /// C1 and C3 packets have a maximum length of 255, and the length defined in the second byte.
        /// C2 and C4 packets have a maximum length of 65535, and the length defined in the second and third byte.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The size of a packet.</returns>
        public static int GetPacketSize(this byte[] packet)
        {
            switch (packet[0])
            {
                case 0xC1:
                case 0xC3:
                    return packet[1];
                case 0xC2:
                case 0xC4:
                    return packet[1] << 8 | packet[2];
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Sets the size of the byte array as packet length in the corresponding indexes of the byte array.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public static void SetPacketSize(this byte[] packet)
        {
            var size = packet.Length;
            switch (packet[0])
            {
                case 0xC1:
                case 0xC3:
                    packet[1] = (byte)size;
                    break;
                case 0xC2:
                case 0xC4:
                    packet[1] = (byte)((size & 0xFF00) >> 8);
                    packet[2] = (byte)(size & 0x00FF);
                    break;
            }
        }
    }
}
