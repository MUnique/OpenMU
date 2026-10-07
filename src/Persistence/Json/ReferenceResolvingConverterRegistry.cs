// <copyright file="ReferenceResolvingConverterRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

/// <summary>
/// A registry for the reference resolving converters which are generated at compile time.
/// </summary>
/// <remarks>
/// The generated converters register themselves by a module initializer of their assembly.
/// When no converter was generated for a type, the <see cref="ReferenceResolvingConverterFactory"/>
/// falls back to the <see cref="ReferenceResolvingConverter{T}"/>, which uses reflection.
/// </remarks>
public static class ReferenceResolvingConverterRegistry
{
    private static readonly ConcurrentDictionary<Type, Func<Type[], JsonConverter>> Factories = new();

    /// <summary>
    /// Registers a factory for the converter of the specified type.
    /// </summary>
    /// <typeparam name="T">The type which is converted.</typeparam>
    /// <param name="factory">The factory which creates the converter for the given ignored types.</param>
    public static void Register<T>(Func<Type[], ReferenceResolvingConverterBase<T>> factory)
        where T : class, IIdentifiable, new()
    {
        Factories[typeof(T)] = factory;
    }

    /// <summary>
    /// Tries to create the registered converter for the specified type.
    /// </summary>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="ignoredTypes">The ignored types.</param>
    /// <returns>The created converter, or <c>null</c>, if no converter is registered for the type.</returns>
    internal static JsonConverter? TryCreate(Type typeToConvert, Type[] ignoredTypes)
    {
        if (!Factories.TryGetValue(typeToConvert, out var factory))
        {
            // The module initializer, which registers the converters, runs before the first access to the module.
            // We make sure here that it ran, because the runtime is free to postpone it until then.
            RuntimeHelpers.RunModuleConstructor(typeToConvert.Module.ModuleHandle);
            Factories.TryGetValue(typeToConvert, out factory);
        }

        return factory?.Invoke(ignoredTypes);
    }
}
