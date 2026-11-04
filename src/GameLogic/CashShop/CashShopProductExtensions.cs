// <copyright file="CashShopProductExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

/// <summary>
/// Extensions for the products of the cash shop.
/// </summary>
public static class CashShopProductExtensions
{
    /// <summary>
    /// Determines whether the product can be delivered into the inventory when a player uses it.
    /// </summary>
    /// <remarks>
    /// A product can't be delivered without an item definition. Time-limited products can't be delivered
    /// either, because items which expire aren't supported yet; they would be delivered as permanent items.
    /// On a game server, <see cref="CashShopProductDelivery.CanDeliver"/> decides, because an
    /// <see cref="PlugIns.ICashShopProductDeliveryPlugIn"/> may deliver the product instead. Without
    /// the plugins, e.g. when initializing the catalog, this is the best approximation.
    /// </remarks>
    /// <param name="product">The product.</param>
    /// <returns><c>true</c>, if the product can be delivered; otherwise, <c>false</c>.</returns>
    public static bool CanBeDelivered(this CashShopProduct product)
    {
        return product.ItemDefinition is not null && product.Duration == TimeSpan.Zero;
    }
}
