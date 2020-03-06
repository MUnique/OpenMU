// <copyright file="PacketAnalyzer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MUnique.OpenMU.Network.Packets;
    using static System.Buffers.Binary.BinaryPrimitives;

    /// <summary>
    /// Analyzer which analyzes data packets by considering the configuration files.
    /// </summary>
    public sealed class PacketAnalyzer : IDisposable
    {
        private const string ClientToServerPacketsFile = "ClientToServerPackets.xml";
        private const string ServerToClientPacketsFile = "ServerToClientPackets.xml";
        private const string CommonFile = "CommonEnums.xml";
        private readonly IList<IDisposable> watchers = new List<IDisposable>();
        private PacketDefinitions clientPacketDefinitions;
        private PacketDefinitions serverPacketDefinitions;
        private PacketDefinitions commonDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketAnalyzer"/> class.
        /// The configuration is automatically loaded from the configuration files.
        /// </summary>
        public PacketAnalyzer()
        {
            this.LoadAndWatchConfiguration(def => this.serverPacketDefinitions = def, ServerToClientPacketsFile);
            this.LoadAndWatchConfiguration(def => this.clientPacketDefinitions = def, ClientToServerPacketsFile);
            this.LoadAndWatchConfiguration(def => this.commonDefinitions = def, CommonFile);
        }

        /// <summary>
        /// Extracts the information of the packet and returns it as a formatted string.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The formatted string with the extracted information.</returns>
        public string ExtractInformation(Packet packet)
        {
            var definitions = packet.ToServer ? this.clientPacketDefinitions : this.serverPacketDefinitions;
            var definition = definitions.Packets.FirstOrDefault(p => (byte)p.Type == packet.Type && p.Code == packet.Code && (!p.SubCodeSpecified || p.SubCode == packet.SubCode));
            if (definition != null)
            {
                var stringBuilder = new StringBuilder()
                    .Append(definition.Caption ?? definition.Name);
                foreach (var field in definition.Fields)
                {
                    stringBuilder.Append(Environment.NewLine)
                        .Append(field.Name).Append(": ").Append(this.ExtractFieldValueOrGetError(packet.Data.AsSpan(), field, definition, definitions));
                }

                return stringBuilder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var watcher in this.watchers)
            {
                watcher.Dispose();
            }

            this.watchers.Clear();
        }

        private void LoadAndWatchConfiguration(Action<PacketDefinitions> assignAction, string fileName)
        {
            assignAction(PacketDefinitions.Load(fileName));
            var watcher = new FileSystemWatcher(Environment.CurrentDirectory, fileName);

            watcher.Changed += (sender, args) =>
            {
                PacketDefinitions definitions;
                try
                {
                    definitions = PacketDefinitions.Load(fileName);
                }
                catch
                {
                    // I know, bad practice... but when it fails, because of some invalid xml file, we just don't assign it.
                    return;
                }

                if (definitions != null)
                {
                    assignAction(definitions);
                }
            };

            watcher.EnableRaisingEvents = true;

            this.watchers.Add(watcher);
        }

        /// <summary>
        /// Extracts the field value from this packet or returns an error message, if it couldn't be extracted.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="field">The field definition.</param>
        /// <param name="packet">The packet.</param>
        /// <param name="definitions">The definitions.</param>
        /// <returns>
        /// The value of the field or the error message.
        /// </returns>
        private string ExtractFieldValueOrGetError(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions)
        {
            try
            {
                return this.ExtractFieldValue(data, field, packet, definitions);
            }
            catch (Exception e)
            {
                return $"{e.GetType().Name}: {e.Message}";
            }
        }

        /// <summary>
        /// Extracts the field value from this packet.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="field">The field definition.</param>
        /// <param name="packet">The packet.</param>
        /// <param name="definitions">The definitions.</param>
        /// <returns>
        /// The value of the field.
        /// </returns>
        private string ExtractFieldValue(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions)
        {
            var fieldSize = field.GetFieldSizeInBytes();
            if (field.Type == FieldType.String && field.Index < data.Length)
            {
                return data.ExtractString(field.Index, fieldSize ?? int.MaxValue, Encoding.UTF8);
            }

            if (field.Type == FieldType.Binary && field.Index < data.Length)
            {
                return fieldSize.HasValue
                    ? data.Slice(field.Index, fieldSize.Value).AsString()
                    : data.Slice(field.Index).AsString();
            }

            if (data.Length < field.Index + fieldSize)
            {
                return string.Empty;
            }

            switch (field.Type)
            {
                case FieldType.Byte:
                    return data.Slice(field.Index).GetByteValue(field.LengthSpecified ? field.Length : 8, field.LeftShifted).ToString();
                case FieldType.Boolean:
                    return data.Slice(field.Index).GetBoolean(field.LeftShifted).ToString();
                case FieldType.IntegerLittleEndian:
                    return ReadUInt32LittleEndian(data.Slice(field.Index)).ToString();
                case FieldType.IntegerBigEndian:
                    return ReadUInt32BigEndian(data.Slice(field.Index)).ToString();
                case FieldType.ShortLittleEndian:
                    return ReadUInt16LittleEndian(data.Slice(field.Index)).ToString();
                case FieldType.ShortBigEndian:
                    return ReadUInt16BigEndian(data.Slice(field.Index)).ToString();
                case FieldType.LongLittleEndian:
                    return ReadUInt64LittleEndian(data.Slice(field.Index)).ToString();
                case FieldType.LongBigEndian:
                    return ReadUInt64BigEndian(data.Slice(field.Index)).ToString();
                case FieldType.Enum:
                    return this.ExtractEnumValue(data, field, packet, definitions);
                case FieldType.StructureArray:
                    return this.ExtractStructureArrayValues(data, field, packet, definitions);
                case FieldType.Float:
                    return BitConverter.ToSingle(data.Slice(field.Index)).ToString(CultureInfo.InvariantCulture);
                default:
                    return string.Empty;
            }
        }

        private string ExtractStructureArrayValues(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions)
        {
            var type = packet.Structures?.FirstOrDefault(s => s.Name == field.TypeName)
                       ?? definitions.Structures?.FirstOrDefault(s => s.Name == field.TypeName)
                       ?? this.commonDefinitions.Structures?.FirstOrDefault(s => s.Name == field.TypeName);
            if (type == null)
            {
                return data.Slice(field.Index).AsString();
            }

            var countField = packet.Fields.FirstOrDefault(f => f.Name == field.ItemCountField)
                             ?? packet.Structures?.SelectMany(s => s.Fields).FirstOrDefault(f => f.Name == field.ItemCountField);
            int count = int.Parse(this.ExtractFieldValue(data, countField, packet, definitions));
            if (count == 0)
            {
                return string.Empty;
            }

            var typeLength = type.Length > 0 ? type.Length : this.DetermineFixedStructLength(data, field, type, count);
            var stringBuilder = new StringBuilder();
            var restData = data.Slice(field.Index);

            for (int i = 0; i < count; i++)
            {
                var currentLength = typeLength ?? this.DetermineDynamicStructLength(restData, type, packet);
                var elementData = restData.Slice(0, currentLength);
                restData = restData.Slice(currentLength);

                stringBuilder.Append(Environment.NewLine)
                    .Append(field.Name + $"[{i}]:");
                stringBuilder.Append(Environment.NewLine)
                    .Append("  Raw: ").Append(elementData.AsString());
                foreach (var structField in type.Fields)
                {
                    stringBuilder.Append(Environment.NewLine)
                        .Append("  ").Append(structField.Name).Append(": ").Append(this.ExtractFieldValue(elementData, structField, packet, definitions));
                }
            }

            return stringBuilder.ToString();
        }

        private string ExtractEnumValue(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions)
        {
            var byteValue = data.Slice(field.Index).GetByteValue(field.LengthSpecified ? field.Length : 8, field.LeftShifted);
            var enumDefinition = packet.Enums?.FirstOrDefault(e => e.Name == field.TypeName)
                                 ?? definitions.Enums?.FirstOrDefault(e => e.Name == field.TypeName)
                                 ?? this.commonDefinitions?.Enums?.FirstOrDefault(e => e.Name == field.TypeName);
            var enumValue = enumDefinition?.Values.FirstOrDefault(v => v.Value == byteValue);
            return $"{data[field.Index]} ({enumValue?.Name ?? "unknown"})";
        }

        private int? DetermineFixedStructLength(Span<byte> data, Field field, Structure type, int count)
        {
            if (type.Length > 0)
            {
                return type.Length;
            }

            if (type.Fields.All(f => f.Type != FieldType.StructureArray))
            {
                return (data.Length - field.Index) / count;
            }

            return null;
        }

        /// <summary>
        /// Determines the length of the dynamic structure.
        /// We assume that a nested struct type has a fixed length specified in <see cref="Structure.Length"/>.
        /// </summary>
        /// <param name="restData">The rest data.</param>
        /// <param name="type">The type.</param>
        /// <param name="packetType">Type of the packet.</param>
        /// <returns>The dynamic length of a struct with a nested structure array.</returns>
        private int DetermineDynamicStructLength(Span<byte> restData, Structure type, PacketDefinition packetType)
        {
            var nestedStructField = type.Fields.First(f => f.Type == FieldType.StructureArray);
            var countField = type.Fields.First(f => f.Name == nestedStructField.ItemCountField);
            var count = restData[countField.Index];
            var nestedStructType = packetType.Structures.First(s => s.Name == nestedStructField.TypeName);
            return nestedStructField.Index + (count * nestedStructType.Length);
        }
    }
}
