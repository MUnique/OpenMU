// <copyright file="InventoryProductDelivery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// The standard delivery of a cash shop product, which puts its items into the inventory.
/// </summary>
/// <remarks>
/// It's not a plugin and can't be deactivated. It keeps the delivered items until they're shown,
/// so a new instance is used for every delivery.
/// </remarks>
internal sealed class InventoryProductDelivery : ICashShopProductDeliveryPlugIn
{
    private readonly List<Item> _addedItems = [];

    /// <inheritdoc />
    public ItemIdentifier Key => default;

    /// <inheritdoc />
    public bool CanDeliver(CashShopProduct product) => product.CanBeDelivered();

    /// <inheritdoc />
    public async ValueTask<CashShopUseResult> DeliverAsync(Player player, CashShopProduct product)
    {
        if (player.Inventory is not { } inventory || product.ItemDefinition is not { } definition)
        {
            return CashShopUseResult.CannotUse;
        }

        foreach (var item in CreateItems(player, definition, product.ItemLevel, product.Quantity))
        {
            if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
            {
                await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
                foreach (var addedItem in this._addedItems)
                {
                    await inventory.RemoveItemAsync(addedItem).ConfigureAwait(false);
                    await player.PersistenceContext.DeleteAsync(addedItem).ConfigureAwait(false);
                }

                this._addedItems.Clear();
                return CashShopUseResult.InventoryFull;
            }

            this._addedItems.Add(item);
        }

        return CashShopUseResult.Success;
    }

    /// <inheritdoc />
    public async ValueTask DeliveredAsync(Player player, CashShopProduct product)
    {
        foreach (var item in this._addedItems)
        {
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates the items of a product. The quantity of a stackable item is its durability,
    /// so it's split into stacks of the maximum durability.
    /// </summary>
    private static IEnumerable<Item> CreateItems(Player player, ItemDefinition definition, byte level, int quantity)
    {
        var remaining = Math.Max(quantity, 1);
        while (remaining > 0)
        {
            var item = player.PersistenceContext.CreateNew<Item>();
            item.Definition = definition;
            item.Level = level;
            var count = item.IsStackable() ? Math.Min(remaining, (int)definition.Durability) : 1;
            item.Durability = item.IsStackable() ? count : definition.Durability;
            remaining -= count;
            yield return item;
        }
    }
}
