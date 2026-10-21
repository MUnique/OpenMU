// <copyright file="ItemStorePriceExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

/// <summary>
/// Extensions to get and set the price of an item in a player shop.
/// </summary>
public static class ItemStorePriceExtensions
{
    /// <summary>
    /// Gets the price of the item in the player shop.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The price; <c>null</c>, if the item has no price.</returns>
    /// <remarks>
    /// An amount of 0 or less is treated as no price, so that such an item can't be bought for nothing.
    /// Setting a price never stores it, but a modified database could contain it.
    /// </remarks>
    public static ShopPrice? GetStorePrice(this Item item)
    {
        return item.StorePrice is { } amount and > 0
            ? new ShopPrice(amount, item.StorePriceItemDefinition, item.StorePriceCoinType)
            : null;
    }

    /// <summary>
    /// Sets or removes the price of the item in the player shop.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="price">The price; <c>null</c> removes the price.</param>
    public static void SetStorePrice(this Item item, ShopPrice? price)
    {
        item.StorePrice = price?.Amount;
        item.StorePriceItemDefinition = price?.PriceItem;
        item.StorePriceCoinType = price?.CoinType;
    }
}
