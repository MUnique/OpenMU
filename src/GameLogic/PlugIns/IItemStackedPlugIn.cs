// <copyright file="IItemStackedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when an item got moved by the player.
/// </summary>
[Guid("FCDE60E9-6183-4833-BF59-CE907F9EECCD")]
[PlugInPoint("Item stacked", "Plugins which are called when an item has been stacked by the player.")]
public interface IItemStackedPlugIn
{
    /// <summary>
    /// Is called when an item has been moved by the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="sourceItem">The source item.</param>
    /// <param name="targetItem">The target item.</param>
    ValueTask ItemStackedAsync(Player player, Item sourceItem, Item targetItem);
}