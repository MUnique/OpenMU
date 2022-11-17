// <copyright file="BinaryAsHexJsonConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// This converter converts a hex string to a byte array.
/// Newtonsoft.Json expects a base64 string, but postgres delivers us a hex string,
/// so this converter is needed.
/// </summary>
public class BinaryAsHexJsonConverter : JsonConverter<byte[]>
{
    /// <summary>
    /// The prefix of a byte array string, provided by postgres.
    /// </summary>
    private const string ByteArrayPrefix = @"\x";

    /// <summary>
    /// A character to value mapping table for faster hex string parsing. Each character (0-9a-fA-F)
    /// has it's corresponding value of 0-15. All other characters are initialized with <see cref="int.MaxValue"/>.
    /// </summary>
    private readonly int[] _characterToValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryAsHexJsonConverter"/> class.
    /// </summary>
    public BinaryAsHexJsonConverter()
    {
        this._characterToValue = new int[byte.MaxValue + 1];
        for (int i = 0; i < this._characterToValue.Length; i++)
        {
            this._characterToValue[i] = int.MaxValue;
        }

        for (byte i = 0; i < 10; i++)
        {
            this._characterToValue['0' + i] = i;
        }

        for (byte i = 0xA; i < 16; i++)
        {
            this._characterToValue['a' + i - 0xA] = i;
            this._characterToValue['A' + i - 0xA] = i;
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        var arrayString = ByteArrayPrefix + BitConverter.ToString(value)
            .Replace("-", string.Empty, StringComparison.InvariantCulture)
            .ToLowerInvariant();
        writer.WriteStringValue(arrayString);
    }

    /// <inheritdoc />
    public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var prefixSize = ByteArrayPrefix.Length + 1; // +1 for escaping
            var hexData = reader.ValueSpan.Slice(prefixSize);
            var data = new byte[(hexData.Length - prefixSize) / 2];
            for (var i = 0; i < data.Length; i++)
            {
                var index = i * 2;
                int highNibble = this.ParseCharacter(hexData[index]);
                int lowNibble = this.ParseCharacter(hexData[index + 1]);
                data[i] = (byte)((highNibble << 4) | lowNibble);
            }

            return data;
        }

        throw new Exception($"Unexpected token parsing binary. Expected String, got {reader.TokenType}.");
    }

    /// <inheritdoc />
    public override bool CanConvert(Type objectType) => objectType == typeof(byte[]);

    private int ParseCharacter(byte character)
    {
        var value = this._characterToValue[character];
        if (value == int.MaxValue)
        {
            throw new ArgumentException($"invalid character: {character}", nameof(character));
        }

        return value;
    }
}