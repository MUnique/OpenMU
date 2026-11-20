// <copyright file="IInventoryItemUseChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A view plugin which shows whether an item which is used inside the inventory is in use,
/// after the player requested to use it or to stop using it.
/// </summary>
public interface IInventoryItemUseChangedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows whether the item is in use. Its durability tells it, see <see cref="InventoryUsedItemExtensions"/>.
    /// </summary>
    /// <param name="item">The item.</param>
    ValueTask InventoryItemUseChangedAsync(Item item);
}
