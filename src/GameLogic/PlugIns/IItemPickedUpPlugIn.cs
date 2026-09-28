// <copyright file="IItemPickedUpPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a player picked up an item from the ground.
/// </summary>
[Guid("7B3E9C14-2D6A-4F81-A5C0-8E1B4D7F2A93")]
[PlugInPoint("Item picked up", "Plugins which are called when a player picked up an item from the ground.")]
public interface IItemPickedUpPlugIn
{
    /// <summary>
    /// Is called when a player picked up an item from the ground.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The picked up item. If it has been stacked onto another item, it's the item which was on the ground.</param>
    /// <param name="fromPlayerInventory">If set to <c>true</c>, the item was dropped from the inventory of a player, instead of by a monster or an event.</param>
    ValueTask ItemPickedUpAsync(Player player, Item item, bool fromPlayerInventory);
}
