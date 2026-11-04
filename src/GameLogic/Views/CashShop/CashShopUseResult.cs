// <copyright file="CashShopUseResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// The result of a request to use an item of the cash shop storage.
/// </summary>
public enum CashShopUseResult
{
    /// <summary>
    /// The item was used and its items were added to the inventory.
    /// </summary>
    Success,

    /// <summary>
    /// The item doesn't exist in the storage.
    /// </summary>
    ItemNotFound,

    /// <summary>
    /// The inventory doesn't have enough space for the items.
    /// </summary>
    InventoryFull,

    /// <summary>
    /// The item can't be used, e.g. because its product isn't configured anymore.
    /// </summary>
    CannotUse,

    /// <summary>
    /// The usage couldn't be saved.
    /// </summary>
    SaveFailed,
}
