// <copyright file="AttributeDisplayNames.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

using System.Globalization;
using System.Resources;

/// <summary>
/// Localizes built-in attribute text without changing names used by configuration updates.
/// </summary>
public static class AttributeDisplayNames
{
    private static readonly ResourceManager Resources = new("MUnique.OpenMU.AttributeSystem.Properties.AttributeTexts", typeof(AttributeDisplayNames).Assembly);

    /// <summary>Gets a localized name while preserving unknown and customized names.</summary>
    /// <param name="attribute">The attribute definition.</param>
    /// <returns>The localized name or the original name.</returns>
    public static string? GetDisplayName(this AttributeDefinition attribute)
    {
        var key = GetKey(attribute.Designation);
        return Resources.GetString("Name_" + key, CultureInfo.InvariantCulture) == attribute.Designation
            ? Resources.GetString("Name_" + key, CultureInfo.CurrentUICulture) ?? attribute.Designation
            : attribute.Designation;
    }

    /// <summary>Gets a localized label for an attribute aggregation operation.</summary>
    /// <param name="aggregateType">The aggregation operation.</param>
    /// <returns>The localized label or enum value.</returns>
    public static string GetDisplayName(this AggregateType aggregateType) =>
        Resources.GetString("AggregateType_" + aggregateType, CultureInfo.CurrentUICulture) ?? aggregateType.ToString();

    /// <summary>Gets a localized description without replacing a customized description.</summary>
    /// <param name="attribute">The attribute definition.</param>
    /// <returns>The localized description or the original description.</returns>
    public static string? GetDisplayDescription(this AttributeDefinition attribute)
    {
        var key = GetKey(attribute.Designation);
        return Resources.GetString("Name_" + key, CultureInfo.InvariantCulture) == attribute.Designation
            && Resources.GetString("Description_" + key, CultureInfo.InvariantCulture) == attribute.Description
                ? Resources.GetString("Description_" + key, CultureInfo.CurrentUICulture) ?? attribute.Description
                : attribute.Description;
    }

    private static string GetKey(string? designation) => new((designation ?? string.Empty).Where(char.IsAsciiLetterOrDigit).ToArray());
}
