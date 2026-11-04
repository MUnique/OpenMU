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
/// Analyzer that analyzes data packets by considering the configuration files.
/// </summary>
public sealed class PacketAnalyzer : IDisposable
{
    private const string CommonFile = "CommonEnums.xml";
    private const int DefaultVersionValue = 100;
    private const int ExtendedVersionValue = (106 * 100) + 3;

    private readonly IList<IDisposable> _watchers = new List<IDisposable>();

    /// <summary>
    /// The loaded packet definitions of the <see cref="DefinitionSet"/>. The array has one
    /// slot per file, so that a reloaded file can simply replace its previous content.
    /// </summary>
    private readonly PacketDefinitions?[] _packetDefinitions;

    private PacketDefinitions? _commonDefinitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketAnalyzer" /> class.
    /// The definitions are automatically loaded from the configuration files.
    /// </summary>
    /// <param name="definitionSet">The set of packet definitions which should be used.</param>
    /// <param name="watchFiles">If set to <c>true</c>, the definition files are watched and
    /// automatically reloaded when they change. That's useful when new packet definitions are
    /// developed, but usually not required when just analyzing the traffic.</param>
    public PacketAnalyzer(PacketDefinitionSet definitionSet = PacketDefinitionSet.GameServer, bool watchFiles = false)
    {
        this.DefinitionSet = definitionSet;

        var files = GetDefinitionFiles(definitionSet);
        this._packetDefinitions = new PacketDefinitions?[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            var index = i;
            this.LoadConfiguration(def => this._packetDefinitions[index] = def, files[index], watchFiles);
        }

        this.LoadConfiguration(def => this._commonDefinitions = def, CommonFile, watchFiles);
    }

    /// <summary>
    /// Gets the set of packet definitions which is used by this instance.
    /// </summary>
    public PacketDefinitionSet DefinitionSet { get; }

    /// <summary>
    /// Extracts the information of the packet and returns it as a formatted string.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <param name="clientVersion">The client version of the connection, which decides which
    /// packet definition applies when more than one matches.</param>
    /// <returns>The formatted string with the extracted information.</returns>
    public string ExtractInformation(Packet packet, ClientVersion clientVersion)
    {
        if (this.DeterminePacketDefinition(packet, clientVersion) is not { } match)
        {
            return string.Empty;
        }

        var (definition, definitions) = match;
        var clientVersionValue = GetVersionValue(clientVersion);
        var stringBuilder = new StringBuilder()
            .Append(definition.Caption ?? definition.Name);
        foreach (var field in definition.Fields ?? Enumerable.Empty<Field>())
        {
            stringBuilder.Append(Environment.NewLine)
                .Append(field.Name).Append(": ").Append(this.ExtractFieldValueOrGetError(packet.Data.AsSpan(), field, definition, definitions, clientVersionValue));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Extracts the information of the packet and returns it as a short, formatted string.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <param name="clientVersion">The client version of the connection, which decides which
    /// packet definition applies when more than one matches.</param>
    /// <returns>The formatted string with the extracted information.</returns>
    public (string Data, PacketDefinition? Definition) ExtractShortInformation(Packet packet, ClientVersion clientVersion)
    {
        if (this.DeterminePacketDefinition(packet, clientVersion) is not { } match)
        {
            return (packet.PacketData, null);
        }

        var (definition, definitions) = match;
        var clientVersionValue = GetVersionValue(clientVersion);
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
                    .Append(this.ExtractFieldValueOrGetError(packet.Data.AsSpan(), field, definition, definitions, clientVersionValue));
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

    private static int GetVersionValue(ClientVersion clientVersion)
    {
        return (clientVersion.Season * 100) + clientVersion.Episode;
    }

    private static string[] GetDefinitionFiles(PacketDefinitionSet definitionSet)
    {
        return definitionSet switch
        {
            PacketDefinitionSet.GameServer => ["ClientToServerPackets.xml", "ServerToClientPackets.xml"],
            PacketDefinitionSet.ConnectServer => ["ConnectServerPackets.xml"],
            PacketDefinitionSet.ChatServer => ["ChatServerPackets.xml"],
            _ => throw new ArgumentOutOfRangeException(nameof(definitionSet), definitionSet, "Unknown packet definition set."),
        };
    }

    private (PacketDefinition Definition, PacketDefinitions Owner)? DeterminePacketDefinition(Packet packet, ClientVersion clientVersion)
    {
        var direction = packet.ToServer ? Direction.ClientToServer : Direction.ServerToClient;
        var clientVersionValue = GetVersionValue(clientVersion);

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

        var filteredDefinitions = this._packetDefinitions
            .Where(definitions => definitions is not null)
            .SelectMany(definitions => (definitions!.Packets ?? Enumerable.Empty<PacketDefinition>())
                .Select(p => (Definition: p, Owner: definitions)))
            .Where(pair => pair.Definition.Direction == direction || pair.Definition.Direction == Direction.Bidirectional)
            .Where(pair => (byte)pair.Definition.Type == packet.Type && pair.Definition.Code == packet.Code && (!pair.Definition.SubCodeSpecified || pair.Definition.SubCode == packet.SubCode))
            .Select(pair => (Version: GetVersion(pair.Definition.Name ?? string.Empty), pair.Definition, pair.Owner))
            .OrderBy(pair => pair.Version)
            .ToList();

        if (filteredDefinitions.Count == 0)
        {
            return null;
        }

        if (filteredDefinitions.Count == 1)
        {
            return (filteredDefinitions[0].Definition, filteredDefinitions[0].Owner);
        }

        if (filteredDefinitions.FirstOrDefault(d => d.Version == clientVersionValue) is { Definition: { Name: { } } } exactMatch)
        {
            return (exactMatch.Definition, exactMatch.Owner);
        }

        var sameLengthPackets = filteredDefinitions.Where(d => d.Definition.Length == packet.Size).ToList();
        if (sameLengthPackets.Count > 0)
        {
            if (sameLengthPackets.Count == 1 && sameLengthPackets[0] is { Definition.Name: { } } sameLengthMatch)
            {
                return (sameLengthMatch.Definition, sameLengthMatch.Owner);
            }

            var filteredByDefaults = this.GetPacketDefinitionsFilteredByDefaultValues(packet, sameLengthPackets, clientVersionValue).ToList();
            if (filteredByDefaults.Count == 1)
            {
                return (filteredByDefaults[0].Definition, filteredByDefaults[0].Owner);
            }

            if (filteredByDefaults.Count > 0)
            {
                filteredDefinitions.RemoveAll(def => !filteredByDefaults.Any(f => ReferenceEquals(f.Definition, def.Definition)));
            }
        }

        var current = filteredDefinitions[0];
        foreach (var def in filteredDefinitions.Skip(1))
        {
            if (def.Version > clientVersionValue)
            {
                break;
            }

            current = def;
        }

        return (current.Definition, current.Owner);
    }

    private IEnumerable<(int Version, PacketDefinition Definition, PacketDefinitions Owner)> GetPacketDefinitionsFilteredByDefaultValues(Packet packet, IEnumerable<(int Version, PacketDefinition Definition, PacketDefinitions Owner)> definitions, int clientVersionValue)
    {
        foreach (var candidate in definitions)
        {
            var def = candidate.Definition;
            var defaultFields = def.Fields?.TakeWhile(f => !string.IsNullOrWhiteSpace(f.DefaultValue)).ToList();
            if (defaultFields is null or { Count: 0 })
            {
                break;
            }

            if (defaultFields.TrueForAll(field => int.TryParse(this.ExtractFieldValueOrGetError(packet.Data, field, def, candidate.Owner, clientVersionValue), out var actual)
                                                  && (int.TryParse(field.DefaultValue, out var target) || int.TryParse(field.DefaultValue!.Replace("0x", string.Empty), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out target))
                                                  && actual == target))
            {
                yield return candidate;
            }
        }
    }

    private void LoadConfiguration(Action<PacketDefinitions?> assignAction, string fileName, bool watchFile)
    {
        // The definition files are copied next to the binaries, which is not necessarily the
        // working directory of the process - a server is usually not started from its own
        // folder.
        var directory = AppContext.BaseDirectory;
        var filePath = Path.Combine(directory, fileName);
        if (File.Exists(filePath))
        {
            assignAction(PacketDefinitions.Load(filePath));
        }

        if (!watchFile)
        {
            return;
        }

        var watcher = new FileSystemWatcher(directory, fileName);

        watcher.Changed += (_, _) =>
        {
            PacketDefinitions? definitions;
            try
            {
                definitions = PacketDefinitions.Load(filePath);
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
    /// <param name="clientVersionValue">The numeric client version of the connection.</param>
    /// <returns>
    /// The value of the field or the error message.
    /// </returns>
    private string ExtractFieldValueOrGetError(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions, int clientVersionValue)
    {
        try
        {
            return this.ExtractFieldValue(data, field, packet, definitions, clientVersionValue);
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
    /// <param name="clientVersionValue">The numeric client version of the connection.</param>
    /// <returns>
    /// The value of the field.
    /// </returns>
    private string ExtractFieldValue(Span<byte> data, Field field, PacketDefinition packet, PacketDefinitions definitions, int clientVersionValue)
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
            FieldType.StructureArray => this.ExtractStructureArrayValues(data, field, packet, definitions, clientVersionValue),
            FieldType.Float => ReadSingleLittleEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            FieldType.Double => ReadDoubleBigEndian(data[field.Index..]).ToString(CultureInfo.InvariantCulture),
            _ => string.Empty,
        };
    }

    private string ExtractStructureArrayValues(Span<byte> data, Field arrayField, PacketDefinition packet, PacketDefinitions definitions, int clientVersionValue)
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
        int count = countField is null ? 0 : int.Parse(this.ExtractFieldValue(data, countField, packet, definitions, clientVersionValue), CultureInfo.InvariantCulture);
        if (count == 0)
        {
            return string.Empty;
        }

        var typeLength = elementType.Length > 0 ? elementType.Length : this.DetermineFixedStructLength(data, arrayField, elementType, count);
        var fixedLengthByCount = this.CalcFixStructLengthBySizeAndCount(data, arrayField, elementType, count);
        var stringBuilder = new StringBuilder();
        var restData = data[arrayField.Index..];

        for (int i = 0; i < count; i++)
        {
            var currentLength = typeLength ?? this.DetermineDynamicStructLength(restData, elementType, packet, clientVersionValue) ?? fixedLengthByCount;
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
                    .Append("  ").Append(structField.Name).Append(": ").Append(this.ExtractFieldValue(elementData, structField, packet, definitions, clientVersionValue));
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
    /// <param name="clientVersionValue">The numeric client version of the connection.</param>
    /// <returns>The dynamic length of a struct with a nested structure array.</returns>
    private int? DetermineDynamicStructLength(Span<byte> restData, Structure type, PacketDefinition packetType, int clientVersionValue)
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

        if (clientVersionValue == ExtendedVersionValue
            && type.Fields.FirstOrDefault(f => f.Type == FieldType.Binary) is { } binaryField
            && binaryField.Name?.EndsWith("ItemData") is true)
        {
            return binaryField.Index + this.DetermineItemSize(restData, binaryField);
        }

        if (type.Fields.MaxBy(f => f.Index) is { Type: not (FieldType.Binary or FieldType.StructureArray) } lastField)
        {
            return lastField.Index + this.FieldSize(lastField.Type);
        }

        return null;
    }

    private int DetermineItemSize(Span<byte> restData, Field binaryField)
    {
        var itemData = restData.Slice(binaryField.Index);
        var size = 5;
        var options = itemData[4];

        // Option
        if ((options & 1) == 1)
        {
            size++;
        }

        // Excellent
        if ((options & 8) == 8)
        {
            size++;
        }

        // Ancient
        if ((options & 0x10) == 0x10)
        {
            size++;
        }

        // Harmony
        if ((options & 0x20) == 0x20)
        {
            size++;
        }

        // Sockets
        if ((options & 0x80) == 0x80)
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