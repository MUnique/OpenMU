// <copyright file="CashShopProductDelivery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Selects how a cash shop product is delivered when a player uses it from the cash shop storage.
/// </summary>
public static class CashShopProductDelivery
{
    /// <summary>
    /// Determines whether the product can be delivered, which is a requirement to sell it.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="product">The product.</param>
    /// <returns><c>true</c>, if the product can be delivered; otherwise, <c>false</c>.</returns>
    public static bool CanDeliver(IGameContext gameContext, CashShopProduct product)
    {
        return GetDelivery(gameContext, product) is not null;
    }

    /// <summary>
    /// Gets the delivery of the product: the active <see cref="ICashShopProductDeliveryPlugIn"/> of its
    /// item, or the delivery into the inventory if there is none.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="product">The product.</param>
    /// <returns>
    /// The delivery; <c>null</c>, if the product can't be delivered. That's the case if the delivery
    /// can't deliver this product, e.g. a time-limited one, or if the strategy of its item is inactive.
    /// </returns>
    public static ICashShopProductDeliveryPlugIn? GetDelivery(IGameContext gameContext, CashShopProduct product)
    {
        if (product.ItemDefinition is not { } definition)
        {
            return null;
        }

        var plugInManager = gameContext.PlugInManager;
        var itemKey = new ItemIdentifier(definition.Number, definition.Group);
        var groupKey = new ItemIdentifier(null, definition.Group);
        ICashShopProductDeliveryPlugIn? delivery = plugInManager.GetStrategy<ItemIdentifier, ICashShopProductDeliveryPlugIn>(itemKey)
                       ?? plugInManager.GetStrategy<ItemIdentifier, ICashShopProductDeliveryPlugIn>(groupKey);
        if (delivery is null)
        {
            // Without its strategy, an item like a character card would be put into the inventory,
            // where it's of no use. It stays in the storage instead, until the strategy is active again.
            var knownKeys = GetKeysOfKnownStrategies(plugInManager);
            if (knownKeys.Contains(itemKey) || knownKeys.Contains(groupKey))
            {
                return null;
            }

            delivery = new InventoryProductDelivery();
        }

        return delivery.CanDeliver(product) ? delivery : null;
    }

    /// <summary>
    /// Gets the keys of all known delivery strategies, including inactive ones. The plugin manager only
    /// provides the types of inactive plugins, so their keys are read from new instances.
    /// </summary>
    private static HashSet<ItemIdentifier> GetKeysOfKnownStrategies(PlugInManager plugInManager)
    {
        return plugInManager.GetKnownPlugInsOf<ICashShopProductDeliveryPlugIn>()
            .Where(type => !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) is not null)
            .Select(type => ((ICashShopProductDeliveryPlugIn)Activator.CreateInstance(type)!).Key)
            .ToHashSet();
    }
}
