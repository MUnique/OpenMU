// <copyright file="ReferenceResolvingTypeInfoResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// A json type info resolver for the types which are read by the generated reference resolving converters,
/// including the persistent types themselves.
/// </summary>
/// <remarks>
/// The types are registered by the generated code, so that their metadata doesn't have to be built
/// by the reflection based <see cref="DefaultJsonTypeInfoResolver"/>.
/// The converters are chosen like the <see cref="JsonSerializer"/> does it: A converter of the
/// <see cref="JsonSerializerOptions.Converters"/> is preferred over the built-in converter.
/// For types which are not registered, this resolver returns <c>null</c>, so that it can be combined with other resolvers.
/// </remarks>
public sealed class ReferenceResolvingTypeInfoResolver : IJsonTypeInfoResolver
{
    private static readonly ConcurrentDictionary<Type, Func<JsonSerializerOptions, JsonTypeInfo?>> Factories = new();

    private ReferenceResolvingTypeInfoResolver()
    {
    }

    /// <summary>
    /// Gets the instance of the resolver.
    /// </summary>
    public static ReferenceResolvingTypeInfoResolver Instance { get; } = new();

    /// <summary>
    /// Registers the type, so that this resolver provides its metadata.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="builtInConverterFactory">
    /// The factory for the built-in converter of the type, which is used when the options don't contain a converter for it.
    /// If it's <c>null</c>, the type is only resolved when the options contain a converter for it.
    /// </param>
    public static void Register<T>(Func<JsonSerializerOptions, JsonConverter<T>>? builtInConverterFactory)
    {
        Factories.TryAdd(typeof(T), options => CreateTypeInfo(options, builtInConverterFactory));
    }

    /// <inheritdoc />
    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (!Factories.TryGetValue(type, out var factory))
        {
            // The module initializer, which registers the types, runs before the first access to the module.
            // We make sure here that it ran, because the runtime is free to postpone it until then.
            RuntimeHelpers.RunModuleConstructor(type.Module.ModuleHandle);
            if (!Factories.TryGetValue(type, out factory))
            {
                return null;
            }
        }

        return factory(options);
    }

    private static JsonTypeInfo? CreateTypeInfo<T>(JsonSerializerOptions options, Func<JsonSerializerOptions, JsonConverter<T>>? builtInConverterFactory)
    {
        var converter = GetConverterOfOptions(options, typeof(T)) ?? builtInConverterFactory?.Invoke(options);
        return converter is null ? null : JsonMetadataServices.CreateValueInfo<T>(options, converter);
    }

    private static JsonConverter? GetConverterOfOptions(JsonSerializerOptions options, Type type)
    {
        foreach (var converter in options.Converters)
        {
            if (converter.CanConvert(type))
            {
                return converter is JsonConverterFactory factory ? factory.CreateConverter(type, options) : converter;
            }
        }

        return null;
    }
}
