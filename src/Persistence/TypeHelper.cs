// <copyright file="TypeHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections.Concurrent;
using System.Reflection;

/// <summary>
/// Helper class which offers functions related to the extended data model types.
/// </summary>
public static class TypeHelper
{
    private static readonly string ConfigurationNamespace = "MUnique.OpenMU.DataModel.Configuration";

    /// <summary>
    /// A cache which holds extended types (Value) for their originating assembly and corresponding base type (Key).
    /// The assembly is part of the key, because there are multiple persistence models (e.g. Entity Framework and in-memory)
    /// which can be used in the same process.
    /// </summary>
    private static readonly ConcurrentDictionary<(Assembly Origin, Type BaseType), Type> BaseToPersistentTypes = new();

    /// <summary>
    /// The generated persistent type registries of the assemblies, prepared for lookups; <c>null</c> for assemblies without one.
    /// </summary>
    private static readonly ConcurrentDictionary<Assembly, RegistryLookup?> Registries = new();

    /// <summary>
    /// Gets the ef core type of <typeparamref name="TBase"/>.
    /// </summary>
    /// <typeparam name="TBase">Base type of the data model.</typeparam>
    /// <param name="origin">The originating assembly of the persistent type of <typeparamref name="TBase"/>.</param>
    /// <returns>Extended ef core type of <typeparamref name="TBase"/>.</returns>
    public static Type GetPersistentTypeOf<TBase>(this Assembly origin)
    {
        return origin.GetPersistentTypeOf(typeof(TBase));
    }

    /// <summary>
    /// Gets the ef core type of the given base type.
    /// </summary>
    /// <param name="origin">The originating assembly of the persistent type of the base type.</param>
    /// /// <param name="baseType">Base type of the data model.</param>
    /// <returns>Extended ef core type of the base type.</returns>
    public static Type GetPersistentTypeOf(this Assembly origin, Type baseType)
    {
        if (baseType.Assembly == origin)
        {
            // TBase is already the persistent type
            return baseType;
        }

        if (GetRegistry(origin)?.ByBaseType.TryGetValue(baseType, out var persistentType) ?? false)
        {
            return persistentType.Type;
        }

        return BaseToPersistentTypes.GetOrAdd((origin, baseType), static key => key.Origin.GetTypes().First(t => t.BaseType == key.BaseType));
    }

    /// <summary>
    /// Finds the persistent type of the given type in the generated <see cref="IPersistentTypeRegistry"/> of the originating assembly.
    /// </summary>
    /// <param name="origin">The originating assembly of the persistent type.</param>
    /// <param name="type">The persistent type, or its base type of the data model.</param>
    /// <returns>The persistent type; <c>null</c>, if the assembly has no registry, or if it doesn't contain the type.</returns>
    public static PersistentType? FindPersistentType(this Assembly origin, Type type)
    {
        if (GetRegistry(origin) is not { } registry)
        {
            return null;
        }

        var types = type.Assembly == origin ? registry.ByType : registry.ByBaseType;
        return types.GetValueOrDefault(type);
    }

    /// <summary>
    /// Creates a new object of the extended ef core type of the <typeparamref name="TBase" />.
    /// </summary>
    /// <typeparam name="TBase">The base type of the data model.</typeparam>
    /// <param name="origin">The originating assembly of the persistent type of <typeparamref name="TBase"/>.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// A new object of the extended ef core type of the <typeparamref name="TBase" />.
    /// </returns>
    public static TBase CreateNew<TBase>(this Assembly origin, params object?[] args)
        where TBase : class
    {
        if (args.Length == 0 && origin.FindPersistentType(typeof(TBase))?.CreateInstance() is TBase instance)
        {
            return instance;
        }

        var persistentType = origin.GetPersistentTypeOf<TBase>();
        if (args.Length == 0)
        {
            return (TBase)Activator.CreateInstance(persistentType)!;
        }

        return (TBase)Activator.CreateInstance(persistentType, args)!;
    }

    /// <summary>
    /// Creates a new object of the extended ef core type of the given type.
    /// </summary>
    /// <param name="origin">The originating assembly of the persistent type of <paramref name="type"/>.</param>
    /// <param name="type">The type which should get created.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// A new object of the extended ef core type of the <paramref name="type" />.
    /// </returns>
    public static object CreateNew(this Assembly origin, Type type, params object?[] args)
    {
        if (args.Length == 0 && origin.FindPersistentType(type)?.CreateInstance() is { } instance)
        {
            return instance;
        }

        var persistentType = origin.GetPersistentTypeOf(type);
        if (args.Length == 0)
        {
            return Activator.CreateInstance(persistentType)!;
        }

        return Activator.CreateInstance(persistentType, args)!;
    }

    /// <summary>
    /// Determines whether the given type is a is configuration type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if the given type is a configuration type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsConfigurationType(this Type type)
    {
        if (type.Namespace != null
            && type.Namespace.StartsWith(ConfigurationNamespace, StringComparison.InvariantCulture))
        {
            return true;
        }

        if (type.BaseType is { Namespace: { } }
            && type.BaseType.Namespace.StartsWith(ConfigurationNamespace, StringComparison.InvariantCulture))
        {
            return true;
        }

        if (type.Name.Contains("Definition", StringComparison.InvariantCulture))
        {
            return true;
        }

        if (type.Name is "AttributeRelationship" or "PlugInConfiguration" or "ConstValueAttribute")
        {
            return true;
        }

        return false;
    }

    private static RegistryLookup? GetRegistry(Assembly origin)
    {
        return Registries.GetOrAdd(
            origin,
            static assembly => assembly.GetCustomAttribute<PersistentTypeRegistryAttribute>() is { } attribute
                               && Activator.CreateInstance(attribute.RegistryType) is IPersistentTypeRegistry registry
                ? new RegistryLookup(registry)
                : null);
    }

    /// <summary>
    /// The persistent types of a <see cref="IPersistentTypeRegistry"/>, by their type and by their base type.
    /// </summary>
    private sealed class RegistryLookup
    {
        public RegistryLookup(IPersistentTypeRegistry registry)
        {
            foreach (var persistentType in registry.Types)
            {
                this.ByType.TryAdd(persistentType.Type, persistentType);
                if (persistentType.BaseType is { } baseType && baseType.Assembly != persistentType.Type.Assembly)
                {
                    // Like Assembly.GetTypes().First(t => t.BaseType == baseType), the first type wins.
                    this.ByBaseType.TryAdd(baseType, persistentType);
                }
            }
        }

        public Dictionary<Type, PersistentType> ByType { get; } = new();

        public Dictionary<Type, PersistentType> ByBaseType { get; } = new();
    }
}