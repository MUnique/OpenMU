// <copyright file="ReferenceResolvingConverterFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Factory for converters which are able to resolve (circular) references.
/// </summary>
/// <remarks>
/// It prefers the converters which were generated at compile time (see <see cref="ReferenceResolvingConverterRegistry"/>),
/// and falls back to the <see cref="ReferenceResolvingConverter{T}"/>, which uses reflection.
/// </remarks>
public class ReferenceResolvingConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// A cache for previously created converters.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, JsonConverter> ConvertersCache = new();

    /// <summary>
    /// Gets or sets the types which are ignored during deserialization.
    /// </summary>
    public Type[] IgnoredTypes { get; set; } = [];

    /// <inheritdoc />
    public override bool CanConvert(Type objectType) =>
        !objectType.IsEnum
        && !objectType.IsGenericType
        && objectType.IsAssignableTo(typeof(IIdentifiable))
        && (objectType.Namespace?.StartsWith("MUnique.OpenMU.", StringComparison.InvariantCulture) ?? false);

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return ConvertersCache.GetOrAdd(typeToConvert, type =>
        {
            if (ReferenceResolvingConverterRegistry.TryCreate(type, this.IgnoredTypes) is { } generatedConverter)
            {
                return generatedConverter;
            }

            var converterType = typeof(ReferenceResolvingConverter<>).MakeGenericType(type);
            return (JsonConverter)Activator.CreateInstance(converterType, new object[] { this.IgnoredTypes })!;
        });
    }
}