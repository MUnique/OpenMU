// <copyright file="LocalizedCaption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

using System.Reflection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A <see cref="LocalizedString"/> property value of an object of the configuration.
/// </summary>
/// <param name="Owner">The object which owns the property.</param>
/// <param name="OwnerId">The identifier of the owner.</param>
/// <param name="Property">The property.</param>
/// <param name="Value">The current value.</param>
internal sealed record LocalizedCaption(object Owner, Guid OwnerId, PropertyInfo Property, LocalizedString Value)
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    /// <summary>
    /// Gets the key which identifies the property of the owner, independent of the context.
    /// </summary>
    public (Guid OwnerId, string PropertyName) Key => (this.OwnerId, this.Property.Name);

    /// <summary>
    /// Sets a new value to the property of the owner.
    /// </summary>
    /// <param name="value">The new value.</param>
    public void SetValue(LocalizedString value)
    {
        this.Property.SetValue(this.Owner, value);
    }

    /// <summary>
    /// Finds all localized captions which are reachable from the game configuration.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The captions of all identifiable objects of the configuration.</returns>
    public static IEnumerable<LocalizedCaption> FindAll(GameConfiguration gameConfiguration)
    {
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var pending = new Stack<object>();
        pending.Push(gameConfiguration);
        while (pending.TryPop(out var current))
        {
            if (!visited.Add(current))
            {
                continue;
            }

            foreach (var property in GetDataModelProperties(current.GetType()))
            {
                object? value;
                try
                {
                    value = property.GetValue(current);
                }
                catch (TargetInvocationException)
                {
                    continue;
                }

                switch (value)
                {
                    case null:
                        break;
                    case LocalizedString localizedString:
                        if (current is IIdentifiable identifiable && property.CanWrite)
                        {
                            yield return new LocalizedCaption(current, identifiable.Id, property, localizedString);
                        }

                        break;
                    case string:
                        break;
                    case System.Collections.IEnumerable enumerable:
                        foreach (var item in enumerable)
                        {
                            if (item is not null && IsDataModelObject(item))
                            {
                                pending.Push(item);
                            }
                        }

                        break;
                    default:
                        if (IsDataModelObject(value))
                        {
                            pending.Push(value);
                        }

                        break;
                }
            }
        }
    }

    private static bool IsDataModelObject(object value)
    {
        var type = value.GetType();
        return !type.IsValueType && GetDataModelType(type) is not null;
    }

    private static Type? GetDataModelType(Type type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.Assembly == typeof(GameConfiguration).Assembly)
            {
                return current;
            }
        }

        return null;
    }

    private static IEnumerable<PropertyInfo> GetDataModelProperties(Type type)
    {
        // Only properties which are defined by the data model. Persistence specific properties
        // (e.g. "Raw" collections or join entities of Entity Framework) are skipped.
        return PropertyCache.GetOrAdd(type, static t => t
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetIndexParameters().Length == 0 && p.GetMethod is not null)
            .Where(p => p.GetMethod!.GetBaseDefinition().DeclaringType?.Assembly == typeof(GameConfiguration).Assembly)
            .Where(p => !p.PropertyType.IsPrimitive && !p.PropertyType.IsEnum)
            .ToArray());
    }
}
