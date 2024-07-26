// <copyright file="ObjectJsonExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

/// <summary>
/// Generic extension methods for the (de)serialization of objects.
/// </summary>
public static class ObjectJsonExtensions
{
    /// <summary>
    /// Parses a json document string into a <typeparamref name="T" /> instance.
    /// </summary>
    /// <typeparam name="T">The type of the resulting object.</typeparam>
    /// <param name="stream">A text reader with the content of the json document.</param>
    /// <returns>
    /// The resulting <typeparamref name="T" />.
    /// </returns>
    [return: MaybeNull]
    public static T FromJson<T>(this Stream stream)
    {
        var deserializer = new JsonObjectDeserializer();
        var result = deserializer.Deserialize<T>(stream, new IdReferenceHandler());
        return result;
    }

    /// <summary>
    /// Exports this instance into a json document string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The json document string which contains all data of this instance.
    /// </returns>
    public static async ValueTask<string> ToJsonAsync<T>(this T obj, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await obj.ToJsonAsync(stream, cancellationToken).ConfigureAwait(false);

        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }

    /// <summary>
    /// Exports this instance into a json document by writing into the passed text writer.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="textWriter">The target text writer, e.g. a <see cref="StringWriter" />.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async ValueTask ToJsonAsync<T>(this T obj, Stream textWriter, CancellationToken cancellationToken = default)
    {
        var serializer = new JsonObjectSerializer();
        await serializer.SerializeAsync(obj, textWriter, cancellationToken).ConfigureAwait(false);
    }
}