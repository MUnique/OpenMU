// <copyright file="CashShopProductExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Extensions for the products of the cash shop.
/// </summary>
public static class CashShopProductExtensions
{
    /// <summary>
    /// Determines whether the product can be delivered into the inventory when a player uses it.
    /// </summary>
    /// <remarks>
    /// A product can't be delivered without an item definition. A time-limited product is delivered as items
    /// which expire (see <see cref="Item.ExpiresAt"/>), if its item can be one, see <see cref="CanBeTimeLimited"/>.
    /// On a game server, <see cref="CashShopProductDelivery.CanDeliver"/> decides, because an
    /// <see cref="PlugIns.ICashShopProductDeliveryPlugIn"/> may deliver the product instead. Without
    /// the plugins, e.g. when initializing the catalog, this is the best approximation.
    /// </remarks>
    /// <param name="product">The product.</param>
    /// <returns><c>true</c>, if the product can be delivered; otherwise, <c>false</c>.</returns>
    public static bool CanBeDelivered(this CashShopProduct product)
    {
        return product.ItemDefinition is { } definition
               && (product.Duration == TimeSpan.Zero || definition.CanBeTimeLimited());
    }

    /// <summary>
    /// Determines whether items of this kind can be delivered as time-limited items.
    /// </summary>
    /// <remarks>
    /// Only items which are worn are time-limited, like the pets and rings of the cash shop: their effect lasts
    /// while they are equipped and valid. A consumable item, like a seal, has an effect with its own duration
    /// after it's used, which a time-limited product of the cash shop would have to extend; that isn't supported.
    /// An item which a player can't drop or sell once it expired must not become time-limited either, because it
    /// would stay in the inventory forever.
    /// </remarks>
    /// <param name="definition">The item definition.</param>
    /// <returns><c>true</c>, if items of this kind can be time-limited; otherwise, <c>false</c>.</returns>
    public static bool CanBeTimeLimited(this ItemDefinition definition)
    {
        return definition.ItemSlot is not null
               && definition.ConsumeEffect is null
               && definition.CanBeDisposedOfWhenExpired();
    }
}
