// -----------------------------------------------------------------------
// <copyright file="IItemConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler to modify items which are specified by the target slot.
/// </summary>
[Guid("420FB26E-CE78-4942-8589-3A416EF4E31F")]
[PlugInPoint("Item consume handlers", "Plugins which will be executed to consume an item.")]
internal interface IItemConsumeHandlerPlugIn : IStrategyPlugIn<ItemIdentifier>
{
    /// <summary>
    /// Consumes the item at the specified slot, and reduces its durability by one.
    /// If the durability has reached 0, the item is getting destroyed.
    /// If a target slot is specified, the consumption targets the item on this slot (e.g. upgrade of an item by a jewel).
    /// </summary>
    /// <param name="player">The player which is consuming.</param>
    /// <param name="item">The item which gets consumed.</param>
    /// <param name="targetItem">The item which is the target of the consumption (e.g. upgrade target of a jewel).</param>
    /// <param name="fruitUsage">In case the item is a fruit, this parameter defines how the fruit should be used.</param>
    /// <returns>The success of the consumption.</returns>
    ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage);
}