// <copyright file="IPlayerShopBuyRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// The kind of result.
/// </summary>
public enum ItemBuyResult
{
    /// <summary>
    /// Undefined result.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The item has been bought successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The seller is not available.
    /// </summary>
    NotAvailable = 2,

    /// <summary>
    /// The requested player has no open shop.
    /// </summary>
    ShopNotOpened = 3,

    /// <summary>
    /// The requested player is already in a transaction with another player.
    /// </summary>
    InTransaction = 4,

    /// <summary>
    /// The requested item slot is invalid.
    /// </summary>
    InvalidShopSlot = 5,

    /// <summary>
    /// The requested player with the specified id has a different name or price is missing.
    /// </summary>
    NameMismatchOrPriceMissing = 6,

    /// <summary>
    /// The player has not enough money to buy the item from the seller.
    /// </summary>
    LackOfMoney = 7,

    /// <summary>
    /// The selling player cannot sell the item, because the sale would overflow his money amount in the inventory. Another possibility is that the inventory of the buyer cannot take the item.
    /// </summary>
    MoneyOverflowOrNotEnoughSpace = 8,

    /// <summary>
    /// The requested player has item block active.
    /// </summary>
    ItemBlock = 9,
}

/// <summary>
/// Interface of a view whose implementation informs about an item which has been bought
/// from another players shop.
/// </summary>
public interface IPlayerShopBuyRequestResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Notifies the client if an item has been bought from another players shop.
    /// </summary>
    /// <param name="seller">The seller.</param>
    /// <param name="result">The result.</param>
    /// <param name="item">The item.</param>
    ValueTask ShowResultAsync(IIdentifiable? seller, ItemBuyResult result, Item? item);
}