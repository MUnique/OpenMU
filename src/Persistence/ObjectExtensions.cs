// <copyright file="ObjectExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections.Concurrent;
using System.Reflection;

/// <summary>
/// Extensions for objects.
/// </summary>
public static class ObjectExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> IdProperties = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> NameProperties = new();

    /// <summary>
    /// Gets the guid identifier of an object, which has the name "Id".
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The guid identifier of an object, which has the name "Id".</returns>
    public static Guid GetId(this object item)
    {
        if (item is IIdentifiable identifiable)
        {
            return identifiable.Id;
        }

        if (!IdProperties.TryGetValue(item.GetType(), out var idProperty))
        {
            idProperty = item.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("Id") && p.PropertyType == typeof(Guid));
            IdProperties.TryAdd(item.GetType(), idProperty);
        }

        if (idProperty is null)
        {
            return Guid.Empty;
        }

        return (Guid)idProperty.GetValue(item)!;
    }

    /// <summary>
    /// Gets the name of an object.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The name of an object.</returns>
    public static string GetName(this object item)
    {
        if (!NameProperties.TryGetValue(item.GetType(), out var nameProperty))
        {
            var properties = item.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)).ToList();
            nameProperty = properties.FirstOrDefault(p => p.Name.Equals("Name"))
                           ?? properties.FirstOrDefault(p => p.Name.Equals("Caption"))
                           ?? properties.FirstOrDefault(p => p.Name.Equals("Designation"))
                           ?? properties.FirstOrDefault(p => p.Name.Equals("Description"));
            NameProperties.TryAdd(item.GetType(), nameProperty);
        }

        if (nameProperty is null)
        {
            return item.ToString() ?? string.Empty;
        }

        return (string?)nameProperty.GetValue(item) ?? string.Empty;
    }
}