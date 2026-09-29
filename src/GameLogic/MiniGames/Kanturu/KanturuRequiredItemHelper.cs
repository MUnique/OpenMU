// <copyright file="KanturuRequiredItemHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Filters equipped items down to the ones which satisfy event map requirements.
/// </summary>
internal static class KanturuRequiredItemHelper
{
    /// <summary>
    /// Gets the equipped items which provide one of the required attributes.
    /// </summary>
    /// <param name="equippedItems">The equipped items to filter.</param>
    /// <param name="requirements">The requirements of the event map.</param>
    /// <returns>The equipped items which satisfy one of the requirements.</returns>
    public static IList<Item> GetRequiredItems(IEnumerable<Item>? equippedItems, ICollection<AttributeRequirement> requirements)
    {
        if (equippedItems is null)
        {
            return [];
        }

        return equippedItems
            .Where(item => item.Definition?.BasePowerUpAttributes
                .Any(powerUp => requirements.Any(requirement => requirement.Attribute == powerUp.TargetAttribute)) is true)
            .ToList();
    }

    /// <summary>
    /// Gets the equipped items of the player which provide one of the required attributes.
    /// </summary>
    /// <param name="player">The player whose equipped items are searched.</param>
    /// <param name="requirements">The requirements of the event map.</param>
    /// <returns>The equipped items which satisfy one of the requirements.</returns>
    public static IList<Item> GetRequiredItems(Player player, ICollection<AttributeRequirement> requirements)
    {
        return GetRequiredItems(player.Inventory?.EquippedItems, requirements);
    }
}
