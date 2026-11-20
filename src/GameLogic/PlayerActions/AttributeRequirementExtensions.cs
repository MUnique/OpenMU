// <copyright file="AttributeRequirementExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Extension methods for <see cref="AttributeRequirement"/>s of maps and warps.
/// </summary>
public static class AttributeRequirementExtensions
{
    /// <summary>
    /// Gets the first of the requirements which the player doesn't fulfill.
    /// </summary>
    /// <param name="requirements">The requirements. <c>null</c> stands for no requirements.</param>
    /// <param name="player">The player.</param>
    /// <returns>The missed requirement, or <c>null</c> if the player fulfills all of them.</returns>
    public static AttributeRequirement? GetMissingRequirement(this IEnumerable<AttributeRequirement>? requirements, Player player)
    {
        return requirements?.FirstOrDefault(requirement => player.Attributes is null || player.Attributes[requirement.Attribute] < requirement.MinimumValue);
    }

    /// <summary>
    /// Gets the requirement as the player reads it in a message: the name of the attribute
    /// in the language of the player, and the minimum value, e.g. "Master Level 50".
    /// </summary>
    /// <param name="requirement">The requirement.</param>
    /// <param name="player">The player.</param>
    /// <returns>The requirement as text.</returns>
    public static string GetDisplayText(this AttributeRequirement requirement, Player player)
    {
        var name = new LocalizedString(requirement.Attribute?.Designation).GetTranslation(player.Culture);
        return $"{name} {requirement.MinimumValue}";
    }
}
