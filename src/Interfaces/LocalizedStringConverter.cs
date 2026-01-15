// <copyright file="LocalizedStringConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A JSON converter for the <see cref="LocalizedString"/> type which serializes it as a simple JSON string.
/// </summary>
public class LocalizedStringConverter : JsonConverter<LocalizedString>
{
    /// <inheritdoc />
    public override LocalizedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return new LocalizedString(reader.GetString());
        }

        throw new Exception($"Unexpected token parsing binary. Expected String, got {reader.TokenType}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, LocalizedString value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}