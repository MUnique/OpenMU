// <copyright file="JsonObjectSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Class to serialize an object to a json string or stream.
/// </summary>
public class JsonObjectSerializer
{
    /// <summary>
    /// Serializes the specified object into a stream.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    public void Serialize<T>(T obj, Stream stream)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true,
        };

        JsonSerializer.Serialize(stream, obj, options); // todo: async
    }

    /// <summary>
    /// Serializes the specified object into a string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>The serialized object as string.</returns>
    public string Serialize<T>(T obj)
    {
        using var stream = new MemoryStream();
        this.Serialize(obj, stream);

        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }
}