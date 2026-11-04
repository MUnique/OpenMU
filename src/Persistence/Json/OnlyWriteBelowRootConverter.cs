// <copyright file="OnlyWriteBelowRootConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A <see cref="JsonConverter{T}"/> which only writes the id of an object if it's not the root object.
/// Works around the fact that the <see cref="JsonSerializer"/> does not support circular references.
/// </summary>
/// <typeparam name="T">The type which should be converted.</typeparam>
public class OnlyWriteBelowRootConverter<T> : JsonConverter<T>
    where T : class, IIdentifiable
{
    /// <inheritdoc />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (writer.CurrentDepth > 3)
        {
            writer.WriteStartObject();
            writer.WriteString("$id", value.Id);
            writer.WriteEndObject();
        }
        else
        {
            var defaultOptions = new JsonSerializerOptions(options);
            defaultOptions.Converters.Remove(this);
            JsonSerializer.Serialize(writer, value, defaultOptions);
        }
    }
}