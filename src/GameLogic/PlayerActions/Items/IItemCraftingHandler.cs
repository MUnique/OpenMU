// <copyright file="IItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Interface for an item crafting handler.
/// </summary>
public interface IItemCraftingHandler
{
    /// <summary>
    /// Mixes the items of the <see cref="Player.TemporaryStorage"/> with this crafting handler.
    /// </summary>
    /// <param name="player">The mixing player.</param>
    /// <param name="socketSlot">The socket slot index for the <see cref="MountSeedSphereCrafting"/> and <see cref="RemoveSeedSphereCrafting"/>. It's a 0-based index.</param>
    /// <returns>The crafting result and the resulting item; if there are multiple, only the last one is returned.</returns>
    ValueTask<(CraftingResult Result, Item? Item)> DoMixAsync(Player player, byte socketSlot);

    /// <summary>
    /// Tries to get the required items for this crafting.
    /// If they can't be get or something is wrong, a <see cref="CraftingResult"/> with the
    /// corresponding error is returned. Otherwise, it's <c>null</c>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="items">The items.</param>
    /// <param name="successRateByItems">The success rate by items.</param>
    /// <returns><c>null</c>, if the required items could be get; Otherwise, the corresponding error is returned.</returns>
    CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems);
}