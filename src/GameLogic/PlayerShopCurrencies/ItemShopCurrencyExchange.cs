// <copyright file="ItemShopCurrencyExchange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Pays prices in items, e.g. jewels, by moving them from the inventory of the buyer to the one of the seller.
/// </summary>
/// <remarks>
/// Only whole items are moved: a single item counts one piece, bundles of the jewel mix count their pieces.
/// Bundles are never split, and there is no change, so the buyer needs a combination of items which
/// matches the price exactly. No items are created or deleted: they are moved within the persistence
/// context of the <see cref="ShopPurchase.TradeContext"/>, which also moves the sold item, so that
/// everything is saved together.
/// </remarks>
public sealed class ItemShopCurrencyExchange : IShopCurrencyExchange
{
    /// <summary>
    /// The number of pieces of a bundle per bundle level, which matches the jewel mix (<c>ItemStackAction</c>).
    /// </summary>
    private const int PiecesPerBundleLevel = 10;

    /// <summary>
    /// The highest level of a bundle which the jewel mix creates: bundles of 10, 20 or 30 pieces have the levels 0 to 2.
    /// </summary>
    /// <remarks>
    /// Bundles with a higher level can't be created by players, and unstacking them doesn't give their pieces back,
    /// so they're not accepted as payment.
    /// </remarks>
    private const byte MaximumBundleLevel = 2;

    /// <inheritdoc />
    public ShopPaymentResult CheckFunds(Player buyer, ShopPrice price)
    {
        var rows = GetPaymentRows(buyer, price.PriceItem);
        return rows.Sum(r => r.Pieces) < price.Amount ? ShopPaymentResult.LackOfCurrency : ShopPaymentResult.Success;
    }

    /// <inheritdoc />
    public async ValueTask<ShopPaymentResult> TryPayAsync(ShopPurchase purchase)
    {
        var rows = GetPaymentRows(purchase.Buyer, purchase.Price.PriceItem);
        if (rows.Sum(r => r.Pieces) < purchase.Price.Amount)
        {
            return ShopPaymentResult.LackOfCurrency;
        }

        if (SelectRows(rows, purchase.Price.Amount) is not { } selectedRows)
        {
            return ShopPaymentResult.ExactAmountNotPayable;
        }

        if (purchase.Buyer.Inventory is not { } buyerInventory || purchase.Seller.Inventory is not { } sellerInventory)
        {
            return ShopPaymentResult.Failed;
        }

        // The price items and bundles take one slot each.
        var sellerSlots = sellerInventory.FreeSlots.Take(selectedRows.Count).ToList();
        if (sellerSlots.Count < selectedRows.Count)
        {
            return ShopPaymentResult.SellerInventoryFull;
        }

        for (var i = 0; i < selectedRows.Count; i++)
        {
            var item = selectedRows[i].Item;
            var buyerSlot = item.ItemSlot;
            purchase.TradeContext.Attach(item);
            await buyerInventory.RemoveItemAsync(item).ConfigureAwait(false);
            item.ItemSlot = sellerSlots[i];
            if (!await sellerInventory.AddItemAsync(sellerSlots[i], item).ConfigureAwait(false))
            {
                // Shouldn't happen, because the slot was free. Undo everything.
                item.ItemSlot = buyerSlot;
                await buyerInventory.AddItemAsync(buyerSlot, item).ConfigureAwait(false);
                await this.UndoAsync(purchase, buyerInventory, sellerInventory).ConfigureAwait(false);
                purchase.TradeContext.Detach(item);
                purchase.Buyer.Logger.LogError("Couldn't move {item} to the free slot {slot} of the inventory of {seller}.", item, sellerSlots[i], purchase.Seller);
                return ShopPaymentResult.Failed;
            }

            purchase.PaymentItems.Add((item, buyerSlot));
        }

        return ShopPaymentResult.Success;
    }

    /// <inheritdoc />
    public async ValueTask NotifySellerAsync(ShopPurchase purchase)
    {
        foreach (var (item, _) in purchase.PaymentItems)
        {
            await purchase.Seller.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask NotifyBuyerAsync(ShopPurchase purchase)
    {
        foreach (var (_, buyerSlot) in purchase.PaymentItems)
        {
            await purchase.Buyer.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(buyerSlot)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Selects inventory rows whose pieces sum up exactly to the price.
    /// </summary>
    /// <param name="rows">The available rows.</param>
    /// <param name="price">The price.</param>
    /// <returns>The selected rows; <c>null</c>, if no combination matches the price exactly.</returns>
    /// <remarks>
    /// The work and memory grow with the price, so it's only called when the rows sum up to at least the price.
    /// That bounds the price by the items which the buyer actually has, not by a configured maximum.
    /// </remarks>
    internal static IReadOnlyList<(Item Item, int Pieces)>? SelectRows(IReadOnlyList<(Item Item, int Pieces)> rows, int price)
    {
        // A subset sum, because bundles can't be split: reachedBy[sum] is the index of the row which
        // first reached the sum. The sums are iterated descending, so that each row is used at most once.
        // Bigger rows come first, so that bundles are preferred over many single jewels.
        var ordered = rows.OrderByDescending(r => r.Pieces).ToList();
        var reachedBy = new int[price + 1];
        Array.Fill(reachedBy, -1);
        reachedBy[0] = int.MaxValue;
        for (var rowIndex = 0; rowIndex < ordered.Count && reachedBy[price] < 0; rowIndex++)
        {
            var pieces = ordered[rowIndex].Pieces;
            for (var sum = price; sum >= pieces; sum--)
            {
                if (reachedBy[sum] < 0 && reachedBy[sum - pieces] >= 0)
                {
                    reachedBy[sum] = rowIndex;
                }
            }
        }

        if (reachedBy[price] < 0)
        {
            return null;
        }

        var result = new List<(Item Item, int Pieces)>();
        var remaining = price;
        while (remaining > 0)
        {
            var row = ordered[reachedBy[remaining]];
            result.Add(row);
            remaining -= row.Pieces;
        }

        return result;
    }

    /// <summary>
    /// Gets the items of the inventory of the buyer with which the price can be paid.
    /// </summary>
    /// <param name="buyer">The buyer.</param>
    /// <param name="priceItem">The definition of the items with which the price is paid.</param>
    /// <returns>The items with their number of pieces.</returns>
    internal static IReadOnlyList<(Item Item, int Pieces)> GetPaymentRows(Player buyer, ItemDefinition? priceItem)
    {
        if (priceItem is null || buyer.Inventory is not { } inventory)
        {
            return [];
        }

        var bundleDefinitions = buyer.GameContext.Configuration.JewelMixes
            .Where(mix => ShopPrice.IsSameItem(mix.SingleJewel, priceItem))
            .Select(mix => mix.MixedJewel)
            .OfType<ItemDefinition>()
            .ToList();

        var rows = new List<(Item Item, int Pieces)>();
        foreach (var item in inventory.Items)
        {
            if (item.ItemSlot < InventoryConstants.EquippableSlotsCount
                || item.Definition is not { IsTradable: true } definition
                || item.ItemOptions.Any()
                || item.ItemSetGroups.Any())
            {
                continue;
            }

            if (ShopPrice.IsSameItem(definition, priceItem) && item.Level == 0)
            {
                rows.Add((item, 1));
            }
            else if (item.Level <= MaximumBundleLevel && bundleDefinitions.Any(bundle => ShopPrice.IsSameItem(bundle, definition)))
            {
                rows.Add((item, (item.Level + 1) * PiecesPerBundleLevel));
            }
            else
            {
                // not an item of the price
            }
        }

        return rows;
    }

    private async ValueTask UndoAsync(ShopPurchase purchase, IStorage buyerInventory, IStorage sellerInventory)
    {
        foreach (var (item, buyerSlot) in purchase.PaymentItems)
        {
            await sellerInventory.RemoveItemAsync(item).ConfigureAwait(false);
            item.ItemSlot = buyerSlot;
            await buyerInventory.AddItemAsync(buyerSlot, item).ConfigureAwait(false);
            purchase.TradeContext.Detach(item);
        }

        purchase.PaymentItems.Clear();
    }
}
