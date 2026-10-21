// <copyright file="JsonObjectDeserializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A json deserializer which is able to resolve circular references.
/// </summary>
public class JsonObjectDeserializer
{
    private static readonly Type[] IgnoredTypes = { typeof(ConstantElement) };

    /// <summary>
    /// The serializer options per type of deserializer and version of the <see cref="JsonConverterRegistry"/>.
    /// </summary>
    /// <remarks>
    /// The serializer caches the metadata of the types per options instance. Creating new options for
    /// each deserialization would build this metadata again every time, so the options are reused.
    /// Because the reference handler differs per deserialization, the options get a
    /// <see cref="DelegatingReferenceHandler"/>, which uses the reference handler of the current deserialization.
    /// </remarks>
    private static readonly ConcurrentDictionary<(Type DeserializerType, int RegistryVersion), JsonSerializerOptions> OptionsCache = new();

    /// <summary>
    /// The reference handler of the deserialization which is currently running on this thread.
    /// The deserialization is synchronous, so it's not shared between different deserializations.
    /// </summary>
    [ThreadStatic]
    private static ReferenceHandler? _currentReferenceHandler;

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
        var options = OptionsCache.GetOrAdd((this.GetType(), JsonConverterRegistry.Version), _ => this.CreateOptions());
        var previousReferenceHandler = _currentReferenceHandler;
        _currentReferenceHandler = referenceHandler;
        try
        {
            return JsonSerializer.Deserialize(textReader, (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T)));
        }
        finally
        {
            _currentReferenceHandler = previousReferenceHandler;
        }
    }

    /// <summary>
    /// Called before the deserialization happens. Can be overwritten to apply additional settings.
    /// </summary>
    /// <remarks>
    /// The options are reused for all deserializations of this type of deserializer,
    /// as long as the <see cref="JsonConverterRegistry"/> doesn't change.
    /// </remarks>
    /// <param name="options">The serializer options.</param>
    protected virtual void BeforeDeserialize(JsonSerializerOptions options)
    {
        // can be overwritten to apply additional settings.
    }

    /// <summary>
    /// Creates the type info resolver, which provides the metadata of the types which are registered by the generated code,
    /// and of the types of the <see cref="PersistenceJsonSerializerContext"/>.
    /// The reflection based resolver is only used for other types, and only if reflection is enabled for the serializer.
    /// </summary>
    /// <returns>The type info resolver.</returns>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The reflection based resolver is only used when reflection is enabled for the serializer.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "The reflection based resolver is only used when reflection is enabled for the serializer.")]
    private static IJsonTypeInfoResolver CreateTypeInfoResolver()
    {
        return JsonSerializer.IsReflectionEnabledByDefault
            ? JsonTypeInfoResolver.Combine(ReferenceResolvingTypeInfoResolver.Instance, PersistenceJsonSerializerContext.Default, new DefaultJsonTypeInfoResolver())
            : JsonTypeInfoResolver.Combine(ReferenceResolvingTypeInfoResolver.Instance, PersistenceJsonSerializerContext.Default);
    }

    private JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = new DelegatingReferenceHandler(),
            TypeInfoResolver = CreateTypeInfoResolver(),
            Converters =
            {
                new LocalizedStringJsonConverter(),
                new ReferenceResolvingConverterFactory { IgnoredTypes = IgnoredTypes },
            },
        };

        this.BeforeDeserialize(options);
        return options;
    }

    /// <summary>
    /// A reference handler which delegates to the reference handler of the current deserialization.
    /// </summary>
    private sealed class DelegatingReferenceHandler : ReferenceHandler, IIdReferenceHandler
    {
        /// <inheritdoc />
        public ReferenceResolver? Current => (_currentReferenceHandler as IIdReferenceHandler)?.Current;

        /// <inheritdoc />
        public override ReferenceResolver CreateResolver()
        {
            return (_currentReferenceHandler ?? throw new InvalidOperationException("No deserialization is running.")).CreateResolver();
        }
    }
}
