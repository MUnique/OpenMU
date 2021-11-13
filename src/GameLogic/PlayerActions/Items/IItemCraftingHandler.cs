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
    (CraftingResult, Item?) DoMix(Player player, byte socketSlot);
}