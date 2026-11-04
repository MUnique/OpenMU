// <copyright file="INpcItemPurchasePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which can change what a player gets when buying an item from a merchant NPC,
/// e.g. a gambling NPC which hands out a randomly generated item instead of the shown one.
/// </summary>
[Guid("8C3F5A27-6E1D-4B90-A4C2-7D9E0B1F3A65")]
[PlugInPoint("NPC item purchase", "Plugins which can replace the item and the price of a purchase from a merchant NPC, or refuse it.")]
public interface INpcItemPurchasePlugIn
{
    /// <summary>
    /// Is called before the inventory space and the money of the player are checked.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="merchant">The merchant NPC.</param>
    /// <param name="eventArgs">The purchase. A plugin can set a replacement item and price, or cancel it.</param>
    /// <returns>The task.</returns>
    ValueTask BeforePurchaseAsync(Player player, NonPlayerCharacter merchant, NpcItemPurchaseArgs eventArgs);

    /// <summary>
    /// Is called after the item was paid and added to the inventory of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="merchant">The merchant NPC.</param>
    /// <param name="eventArgs">The purchase.</param>
    /// <param name="boughtItem">The item which was added to the inventory.</param>
    /// <returns>The task.</returns>
    ValueTask AfterPurchaseAsync(Player player, NonPlayerCharacter merchant, NpcItemPurchaseArgs eventArgs, Item boughtItem);
}
