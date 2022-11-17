// <copyright file="JsonObjectDeserializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// A json deserializer which is able to resolve circular references.
/// </summary>
public class JsonObjectDeserializer
{
    private static readonly Type[] IgnoredTypes = { typeof(ConstantElement) };

    /// <summary>
    /// Deserializes the json string to an object of <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of an object to which the json string should be serialized to.</typeparam>
    /// <param name="textReader">The text reader with the json result string.</param>
    /// <param name="referenceHandler">The reference resolver.</param>
    /// <returns>
    /// The resulting object which has been deserialized from the <paramref name="textReader" />.
    /// </returns>
    public T? Deserialize<T>(Stream textReader, ReferenceHandler referenceHandler)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = referenceHandler,
            Converters = { new ReferenceResolvingConverterFactory { IgnoredTypes = IgnoredTypes } },
        };

        this.BeforeDeserialize(options);

        var result = JsonSerializer.Deserialize<T>(textReader, options);
        return result;
    }

    /// <summary>
    /// Called before the deserialization happens. Can be overwritten to apply additional settings.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    protected virtual void BeforeDeserialize(JsonSerializerOptions options)
    {
        // can be overwritten to apply additional settings.
    }
}