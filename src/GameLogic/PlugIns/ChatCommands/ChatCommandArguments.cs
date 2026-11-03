// <copyright file="ChatCommandArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Collections.Concurrent;
using System.Reflection;

/// <summary>
/// Provides the writable properties of the arguments classes of the chat commands.
/// </summary>
/// <remarks>
/// The properties of the arguments classes which are derived from <see cref="ArgumentsBase"/> are registered by
/// code which is generated at compile time. For other classes, they are determined by reflection.
/// </remarks>
public static class ChatCommandArguments
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<ChatCommandArgumentProperty>> Properties = new();

    /// <summary>
    /// Registers the writable properties of an arguments class.
    /// </summary>
    /// <param name="argumentsType">The type of the arguments class.</param>
    /// <param name="properties">The writable properties, in the order of <see cref="Type.GetProperties()"/>.</param>
    public static void Register(Type argumentsType, IReadOnlyList<ChatCommandArgumentProperty> properties)
    {
        Properties[argumentsType] = properties;
    }

    /// <summary>
    /// Gets the writable properties of an arguments class, in the order in which they are expected
    /// when they are passed without their short names.
    /// </summary>
    /// <param name="argumentsType">The type of the arguments class.</param>
    /// <returns>The writable properties.</returns>
    public static IReadOnlyList<ChatCommandArgumentProperty> GetProperties(Type argumentsType)
    {
        return Properties.GetOrAdd(argumentsType, static type => CreatePropertiesByReflection(type));
    }

    /// <summary>
    /// Determines the writable properties of an arguments class by reflection.
    /// </summary>
    /// <param name="argumentsType">The type of the arguments class.</param>
    /// <returns>The writable properties.</returns>
    internal static IReadOnlyList<ChatCommandArgumentProperty> CreatePropertiesByReflection(Type argumentsType)
    {
        return argumentsType.GetProperties()
            .Where(property => property.SetMethod is not null)
            .Select(property => new ChatCommandArgumentProperty(
                property.Name,
                property.PropertyType,
                property.GetCustomAttribute<ArgumentAttribute>(inherit: true),
                property.GetCustomAttribute<ValidValuesAttribute>(inherit: true),
                property.GetCustomAttribute<RangeAttribute>(inherit: true),
                property.GetCustomAttribute<ValueReferenceAttribute>(inherit: true),
                property.SetValue))
            .ToList();
    }
}
