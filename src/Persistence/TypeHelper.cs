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
    /// A cache which holds extended types (Value) for their corresponding base type (Key).
    /// </summary>
    private static readonly IDictionary<Type, Type> BaseToPersistentTypes = new ConcurrentDictionary<Type, Type>();

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

        if (!BaseToPersistentTypes.TryGetValue(baseType, out var persistentType))
        {
            persistentType = origin.GetTypes().First(t => t.BaseType == baseType);
            BaseToPersistentTypes.Add(baseType, persistentType);
        }

        return persistentType;
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
}