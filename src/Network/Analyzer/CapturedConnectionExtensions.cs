// <copyright file="CapturedConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Extensions for <see cref="ICapturedConnection"/>.
    /// </summary>
    public static class CapturedConnectionExtensions
    {
        private static char fieldSeparator = ';';

        /// <summary>
        /// Saves the captured packets to the file in a CSV format.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="path">The path.</param>
        public static void SaveToFile(this ICapturedConnection connection, string path)
        {
            using var file = File.OpenWrite(path);
            using var writer = new StreamWriter(file);
            writer.WriteLine(connection.StartTimestamp);
            foreach (var packet in connection.PacketList)
            {
                writer.Write(packet.Timestamp.Ticks);
                writer.Write(fieldSeparator);
                writer.Write(packet.ToServer);
                writer.Write(fieldSeparator);
                writer.Write(packet.Size);
                writer.Write(fieldSeparator);
                writer.WriteLine(packet.PacketData);
            }
        }

        /// <summary>
        /// Loads the packets from the file.
        /// </summary>
        /// <param name="packetList">The packet list.</param>
        /// <param name="path">The path.</param>
        /// <returns>The <see cref="ICapturedConnection.StartTimestamp"/>.</returns>
        public static DateTime LoadFromFile(this BindingList<Packet> packetList, string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);
            DateTime.TryParse(reader.ReadLine(), out var start);

            while (!reader.EndOfStream)
            {
                var currentLine = reader.ReadLine();
                var splittedLine = currentLine?.Split(fieldSeparator);
                if (splittedLine is { Length: > 3 }
                    && long.TryParse(splittedLine[0], out var ticks)
                    && bool.TryParse(splittedLine[1], out var toServer)
                    && int.TryParse(splittedLine[2], out var size)
                    && TryParseArray(splittedLine[3], size, out var data))
                {
                    packetList.Add(new Packet(new TimeSpan(ticks), data, toServer));
                }
                else
                {
                    Debug.Fail($"Invalid line: {currentLine}");
                }
            }

            return start;
        }

        /// <summary>
        /// Tries to parse the byte array string.
        /// </summary>
        /// <param name="arrayString">The array string.</param>
        /// <param name="data">The resulting byte array.</param>
        /// <returns>The success of the parsing.</returns>
        public static bool TryParseArray(string arrayString, out byte[] data)
        {
            return TryParseArray(arrayString, 0, out data);
        }

        private static bool TryParseArray(string arrayString, int specifiedLength, out byte[] data)
        {
            var bytesAsString = arrayString.Split(' ');
            var arrayLength = specifiedLength == 0 ? bytesAsString.Length : specifiedLength;
            data = new byte[arrayLength];

            if (bytesAsString.Length != arrayLength)
            {
                return false;
            }

            for (int i = 0; i < bytesAsString.Length; i++)
            {
                data[i] = byte.Parse(bytesAsString[i], System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return true;
        }
    }
}