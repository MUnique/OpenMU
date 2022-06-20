// <copyright file="BloodCastleItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Item-related extension methods for the blood castle event.
/// </summary>
public static class BloodCastleItemExtensions
{
    private static readonly (short Group, short Number) ArchangelQuestItemId = (13, 19);

    /// <summary>
    /// Determines whether an item definition is that of the archangel quest item.
    /// </summary>
    /// <param name="itemDefinition">The item definition.</param>
    /// <returns>
    ///   <c>true</c> if the specified item definition is the archangel quest item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsArchangelQuestItem(this ItemDefinition? itemDefinition)
    {
        return itemDefinition?.Group == ArchangelQuestItemId.Group && itemDefinition.Number == ArchangelQuestItemId.Number;
    }

    /// <summary>
    /// Determines whether the item is the archangel quest item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is the archangel quest item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsArchangelQuestItem(this Item item)
    {
        return item.Definition?.IsArchangelQuestItem() ?? false;
    }

    /// <summary>
    /// Tries to get the quest item from the players inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The quest item.</param>
    /// <returns>The success.</returns>
    public static bool TryGetQuestItem(this Player player, [MaybeNullWhen(false)] out Item item)
    {
        item = player.Inventory!.Items.FirstOrDefault(i => i.IsArchangelQuestItem());
        return item is not null;
    }
}