// <copyright file="PacketAnalyzer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.PlugIns;
using static System.Buffers.Binary.BinaryPrimitives;

/// <summary>
/// Analyzer which analyzes data packets by considering the configuration files.
/// </summary>
public sealed class PacketAnalyzer : IDisposable
{
    private const string ClientToServerPacketsFile = "ClientToServerPackets.xml";
    private const string ServerToClientPacketsFile = "ServerToClientPackets.xml";
    private const string CommonFile = "CommonEnums.xml";
    private const int DefaultVersionValue = 100;
    private const int ExtendedVersionValue = 106 * 100 + 3;

    private readonly IList<IDisposable> _watchers = new List<IDisposable>();
    private PacketDefinitions? _clientPacketDefinitions;
    private PacketDefinitions? _serverPacketDefinitions;
    private PacketDefinitions? _commonDefinitions;
    private ClientVersion _clientVersion;
    private int _clientVersionValue = DefaultVersionValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketAnalyzer"/> class.
    /// The configuration is automatically loaded from the configuration files.
    /// </summary>
    public PacketAnalyzer()
    {
        this.LoadAndWatchConfiguration(def => this._serverPacketDefinitions = def, ServerToClientPacketsFile);
        this.LoadAndWatchConfiguration(def => this._clientPacketDefinitions = def, ClientToServerPacketsFile);
        this.LoadAndWatchConfiguration(def => this._commonDefinitions = def, CommonFile);
    }

    /// <summary>
    /// Gets or sets the client version.
    /// </summary>
    public ClientVersion ClientVersion
    {
        get => this._clientVersion;
        set
        {
            this._clientVersion = value;
            this._clientVersionValue = value.Season * 100 + value.Episode;
        }
    }

    /// <summary>
    /// Extracts the information of the packet and returns it as a formatted string.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <returns>The formatted string with the extracted information.</returns>
    public string ExtractInformation(Packet packet)
    {
        var definitions = packet.ToServer ? this._clientPacketDefinitions : this._serverPacketDefinitions;
        var definition = this.DeterminePacketDefinition(packet);
        if (definition != null)
        {
            var stringBuilder = new StringBuilder()
                .Append(definition.Caption ?? definition.Name);
            foreach (var field in definition.Fields ?? Enumerable.Empty<Field>())
            {
                stringBuilder.Append(Environment.NewLine)
                    .Append(field.Name).Append(": ").Append(this.ExtractFieldValueOrGetError(packet.Data.AsSpan(), field, definition, definitions!));
            }

            return stringBuilder.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// Extracts the information of the packet and returns it as a short, formatted string.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <returns>The formatted string with the extracted information.</returns>
    public (string, PacketDefinition?) ExtractShortInformation(Packet packet)
    {
        var definitions = packet.ToServer ? this._clientPacketDefinitions : this._serverPacketDefinitions;
        var definition = this.DeterminePacketDefinition(packet);
        if (definition is null)
        {
            return (packet.PacketData, null);
        }

        var stringBuilder = new StringBuilder(definition.Caption ?? definition.Name ?? string.Empty);
        var relevantFields = definition.Fields?
            .Where(f => f.Type != FieldType.Binary && f.Type != FieldType.StructureArray)
            .Where(f => f.Name != "HeaderCode")
            ?? [];
        if (relevantFields.Any())
        {
            stringBuilder.Append(" (");
            var isFirst = true;
            foreach (var field in relevantFields)
            {
                if (!isFirst)
                {
                    stringBuilder.Append("; ");
                }

                isFirst = false;

                stringBuilder.Append(field.Name)
                    .Append(": ")
                    .Append(this.ExtractFieldValueOrGetError(packet.Data.AsSpan(), field, definition, definitions!));
            }

            stringBuilder.Append(")");
        }

        return (stringBuilder.ToString(), definition);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        foreach (var watcher in this._watchers)
        {
            watcher.Dispose();
        }

        this._watchers.Clear();
    }

    private PacketDefinition? DeterminePacketDefinition(Packet packet)
    {
        var allDefinitions = packet.ToServer ? this._clientPacketDefinitions : this._serverPacketDefinitions;
        if (allDefinitions is null)
        {
            return null;
        }

        int GetVersion(string name)
        {
            if (name.EndsWith("Extended", StringComparison.InvariantCulture))
            {
                return ExtendedVersionValue;
            }

            var match = Regex.Match(name, "^[A-Za-z]+?([0-9]{3})$");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return DefaultVersionValue;
        }

        var filteredDefinitions = allDefinitions.Packets?
            .Where(p => (byte)p.Type == packet.Type && p.Code == packet.Code && (!p.SubCodeSpecified || p.SubCode == packet.SubCode))
            .Select(p => (Version: GetVersion(p.Name ?? string.Empty), Definition: p))
            .OrderBy(pair => pair.Version)
            .ToList();

        if (filteredDefinitions is null || !filteredDefinitions.Any())
        {
            return null;
        }

        if (filteredDefinitions.Count == 1)
        {
            return filteredDefinitions[0].Definition;
        }

        if (filteredDefinitions.FirstOrDefault(d => d.Version == this._clientVersionValue) is { Definition: { Name: { } } } exactMatch)
        {
            return exactMatch.Definition;
        }

        var sameLengthPackets = filteredDefinitions.Where(d => d.Definition.Length == packet.Size).Select(d => d.Definition).ToList();
        if (sameLengthPackets.Count > 0)
        {
            if (sameLengthPackets.Count == 1 && sameLengthPackets.First() is { Name: { } } sameLengthMatch)
            {
                return sameLengthMatch;
            }

            var filteredByDefaults = this.GetPacketDefinitionsFilteredByDefaultValues(packet, sameLengthPackets, allDefinitions).ToList();
            if (filteredByDefaults.Count == 1)
            {
                return filteredByDefaults.First();
            }

            if (filteredByDefaults.Count > 0)
            {
                filteredDefinitions.RemoveAll(def => !filteredByDefaults.Contains(def.Definition));
            }
        }

        var current = filteredDefinitions.First();
        foreach (var def in filteredDefinitions.Skip(1))
        {
            if (def.Version > this._clientVersionValue)
            {
                break;
            }

            current = def;
        }

        return current.Definition;
    }

    private IEnumerable<PacketDefinition> GetPacketDefinitionsFilteredByDefaultValues(Packet packet, IEnumerable<PacketDefinition> definitions, PacketDefinitions allDefinitions)
    {
        foreach (var def in definitions)
        {
            var defaultFields = def.Fields?.TakeWhile(f => !string.IsNullOrWhiteSpace(f.DefaultValue)).ToList();
            if (defaultFields is null or { Count: 0 })
            {
                break;
            }

            if (defaultFields.TrueForAll(field => int.TryParse(this.ExtractFieldValueOrGetError(packet.Data, field, def, allDefinitions), out var actual)
                                                  && (int.TryParse(field.DefaultValue, out var target) || int.TryParse(field.DefaultValue!.Replace("0x", string.Empty), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out target))
                                                  && actual == target))
            {
                yield return def;
            }
        }
    }

    private void LoadAndWatchConfiguration(Action<PacketDefinitions?> assignAction, string fileName)
    {
        assignAction(PacketDefinitions.Load(fileName));
        var watcher = new FileSystemWatcher(Environment.CurrentDirectory, fileName);

        watcher.Changed += (_, _) =>
        {
            PacketDefinitions? definitions;
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

        this._watchers.Add(watcher);
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
                : data[field.Index..].AsString();
        }

        if (data.Length < field.Index + fieldSize)
        {
            return string.Empty;
        }

        return field.Type switch
        {
            FieldType.Byte => data[field.Index..]
                .GetByteValue(field.LengthSpecified ? field.Length : 8, field.LeftShifted)
                .ToString(),
            FieldType.Boolean => data[field.Index..].GetBoolean(field.LeftShifted).ToString(),
            FieldType.IntegerLittleEndian => ReadUInt32LittleEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.IntegerBigEndian => ReadUInt32BigEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.ShortLittleEndian => ReadUInt16LittleEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.ShortBigEndian => ReadUInt16BigEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.LongLittleEndian => ReadUInt64LittleEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.LongBigEndian => ReadUInt64BigEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.Enum => this.ExtractEnumValue(data, field, packet, definitions),
            FieldType.StructureArray => this.ExtractStructureArrayValues(data, field, packet, definitions),
            FieldType.Float => ReadSingleLittleEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.Double => ReadDoubleBigEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            _ => string.Empty,
        };
    }

    private string ExtractStructureArrayValues(Span<byte> data, Field arrayField, PacketDefinition packet, PacketDefinitions definitions)
    {
        var elementType = packet.Structures?.FirstOrDefault(s => s.Name == arrayField.TypeName)
                   ?? definitions.Structures?.FirstOrDefault(s => s.Name == arrayField.TypeName)
                   ?? this._commonDefinitions?.Structures?.FirstOrDefault(s => s.Name == arrayField.TypeName);
        if (elementType is null)
        {
            return data[arrayField.Index..].AsString();
        }

        var countField = packet.Fields?.FirstOrDefault(f => f.Name == arrayField.ItemCountField)
                         ?? packet.Structures?.SelectMany(s => s.Fields ?? Enumerable.Empty<Field>()).FirstOrDefault(f => f.Name == arrayField.ItemCountField);
        int count = countField is null ? 0 : int.Parse(this.ExtractFieldValue(data, countField, packet, definitions), CultureInfo.InvariantCulture);
        if (count == 0)
        {
            return string.Empty;
        }

        var typeLength = elementType.Length > 0 ? elementType.Length : this.DetermineFixedStructLength(data, arrayField, elementType, count);
        var fixedLengthByCount = CalcFixStructLengthBySizeAndCount(data, arrayField, elementType, count);
        var stringBuilder = new StringBuilder();
        var restData = data[arrayField.Index..];

        for (int i = 0; i < count; i++)
        {
            var currentLength = typeLength ?? this.DetermineDynamicStructLength(restData, elementType, packet) ?? fixedLengthByCount;
            if (currentLength is null)
            {
                break;
            }

            var elementData = restData[..currentLength.Value];
            restData = restData[currentLength.Value..];

            stringBuilder.Append(Environment.NewLine)
                .Append(arrayField.Name + $"[{i}]:");
            stringBuilder.Append(Environment.NewLine)
                .Append("  Raw: ").Append(elementData.AsString());
            foreach (var structField in elementType.Fields ?? Enumerable.Empty<Field>())
            {
                stringBuilder.Append(Environment.NewLine)
                    .Append("  ").Append(structField.Name).Append(": ").Append(this.ExtractFieldValue(elementData, structField, packet, definitions));
            }
        }

        return stringBuilder.ToString();
    }

    private string ExtractEnumValue(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions)
    {
        var byteValue = data[field.Index..].GetByteValue(field.LengthSpecified ? field.Length : 8, field.LeftShifted);
        var enumDefinition = packet.Enums?.FirstOrDefault(e => e.Name == field.TypeName)
                             ?? definitions.Enums?.FirstOrDefault(e => e.Name == field.TypeName)
                             ?? this._commonDefinitions?.Enums?.FirstOrDefault(e => e.Name == field.TypeName);
        var enumValue = enumDefinition?.Values?.FirstOrDefault(v => v.Value == byteValue);
        return $"{data[field.Index]} ({enumValue?.Name ?? "unknown"})";
    }

    private int? DetermineFixedStructLength(Span<byte> data, Field field, Structure type, int count)
    {
        if (type.Length > 0)
        {
            return type.Length;
        }
        return null;
    }

    private int? CalcFixStructLengthBySizeAndCount(Span<byte> data, Field field, Structure type, int count)
    {
        if (type.Fields?.All(f => f.Type != FieldType.StructureArray) ?? false)
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
    private int? DetermineDynamicStructLength(Span<byte> restData, Structure type, PacketDefinition packetType)
    {
        if (type.Fields is null)
        {
            return null;
        }

        if (packetType.Structures is not null
            && type.Fields.FirstOrDefault(f => f.Type == FieldType.StructureArray) is { } nestedStructField)
        {

            var countField = type.Fields.First(f => f.Name == nestedStructField.ItemCountField);
            var count = restData[countField.Index];
            var nestedStructType = packetType.Structures.First(s => s.Name == nestedStructField.TypeName);
            return nestedStructField.Index + (count * nestedStructType.Length);

        }
        if (this._clientVersionValue == ExtendedVersionValue
            && type.Fields.FirstOrDefault(f => f.Type == FieldType.Binary) is { } binaryField
            && binaryField.Name?.EndsWith("ItemData") is true)
        {
            return binaryField.Index + DetermineItemSize(restData, binaryField);
        }

        if (type.Fields.MaxBy(f => f.Index) is { Type: not (FieldType.Binary or FieldType.StructureArray)} lastField)
        {
            return lastField.Index + FieldSize(lastField.Type);
        }

        return null;
    }

    private int DetermineItemSize(Span<byte> restData, Field binaryField)
    {
        var itemData = restData.Slice(binaryField.Index);
        var size = 5;
        var options = itemData[4];
        if ((options & 1) == 1) // Option
        {
            size++;
        }

        if ((options & 8) == 8) // Excellent
        {
            size++;
        }

        if ((options & 0x10) == 0x10) // Ancient
        {
            size++;
        }

        if ((options & 0x20) == 0x20) // Harmony
        {
            size++;
        }

        if ((options & 0x80) == 0x80) // Sockets
        {
            size++;
            var socketCount = itemData[size] & 0xF;
            size += socketCount;
        }

        return size;
    }

    private int FieldSize(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.Byte => 1,
            FieldType.Boolean => 1,
            FieldType.IntegerLittleEndian => 4,
            FieldType.IntegerBigEndian => 4,
            FieldType.ShortLittleEndian => 2,
            FieldType.ShortBigEndian => 2,
            FieldType.LongLittleEndian => 8,
            FieldType.LongBigEndian => 8,
            FieldType.Enum => 1,
            FieldType.StructureArray => 1,
            FieldType.Float => 4,
            FieldType.Double => 8,
            _ => 1,
        };
    }
}