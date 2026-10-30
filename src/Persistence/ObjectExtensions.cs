// <copyright file="ObjectExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;

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
        if (item is AttributeDefinition attribute)
        {
            return attribute.GetDisplayName() ?? string.Empty;
        }

        if (item is ConstValueAttribute constant)
        {
            return $"{constant.Definition.GetDisplayName()}: {constant.Value} ({constant.AggregateType.GetDisplayName()})";
        }

        if (item is CharacterQuestState questState)
        {
            var culture = CultureInfo.CurrentUICulture;
            var lastFinishedCaption = typeof(CharacterQuestState).GetPropertyCaption(nameof(CharacterQuestState.LastFinishedQuest), culture);
            var activeCaption = typeof(CharacterQuestState).GetPropertyCaption(nameof(CharacterQuestState.ActiveQuest), culture);
            var lastFinishedName = questState.LastFinishedQuest?.Name.GetTranslation(culture);
            var activeName = questState.ActiveQuest?.Name.GetTranslation(culture);
            return $"#{questState.Group}, {lastFinishedCaption}: {(string.IsNullOrWhiteSpace(lastFinishedName) ? "—" : lastFinishedName)}, {activeCaption}: {(string.IsNullOrWhiteSpace(activeName) ? "—" : activeName)}";
        }

        if (!NameProperties.TryGetValue(item.GetType(), out var nameProperty))
        {
            var properties = item.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(LocalizedString))
                .ToList();
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

        var result = nameProperty.GetValue(item) switch
        {
            LocalizedString localizedString => localizedString.ToString(),
            string str => str!,
            null => string.Empty,
            _ => item.ToString(),
        };

        return result ?? string.Empty;
    }
}