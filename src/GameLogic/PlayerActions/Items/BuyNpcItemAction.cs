// <copyright file="BuyNpcItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Action to buy items from a Monster merchant.
/// </summary>
public class BuyNpcItemAction
{
    private readonly CastleSiegeTaxProvider _castleSiegeTaxProvider = new();
    private readonly ItemPriceCalculator _priceCalculator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyNpcItemAction"/> class.
    /// </summary>
    public BuyNpcItemAction()
    {
        this._priceCalculator = new ItemPriceCalculator();
    }

    /// <summary>
    /// Buys the item of the specified slot from the <see cref="Player.OpenedNpc"/> merchant store.
    /// </summary>
    /// <param name="player">The player who buys the item.</param>
    /// <param name="slot">The slot of the item.</param>
    public async ValueTask BuyItemAsync(Player player, byte slot)
    {
        if (player.OpenedNpc is null)
        {
            await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
            return;
        }

        var npcDefinition = player.OpenedNpc.Definition;
        if (npcDefinition?.MerchantStore is null || npcDefinition.MerchantStore.Items.Count == 0)
        {
            await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
            return;
        }

        var storeItem = npcDefinition.MerchantStore.Items.FirstOrDefault(i => i.ItemSlot == slot);
        if (storeItem is null || storeItem.Definition is { IsActive: false })
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemUnknown)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
            return;
        }

        var merchant = player.OpenedNpc;
        var purchase = new NpcItemPurchaseArgs(storeItem);
        var purchasePlugIns = player.GameContext.PlugInManager.GetPlugInPoint<INpcItemPurchasePlugIn>();
        if (purchasePlugIns is not null)
        {
            await purchasePlugIns.BeforePurchaseAsync(player, merchant, purchase).ConfigureAwait(false);
            if (purchase.Cancel)
            {
                await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
                return;
            }
        }

        var itemTemplate = purchase.ReplacementItem ?? storeItem;
        var price = purchase.Price ?? this._priceCalculator.CalculateFinalBuyingPrice(storeItem);

        // Inventory Update:
        if (purchase.ReplacementItem is null && storeItem.IsStackable() && player.Inventory!.Items.FirstOrDefault(item => storeItem.CanCompletelyStackOn(item)) is { } targetItem)
        {
            if (!await this.TryPayAsync(player, price).ConfigureAwait(false))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NotEnoughMoney)).ConfigureAwait(false);
                await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
                return;
            }

            targetItem.Durability += storeItem.Durability;

            // A stacked purchase is still a successful purchase: report it with ItemBought (C1 32),
            // which releases the client from its waiting state. NpcItemBuyFailed (C1 32 FF) must only
            // be sent on actual failures; the durability update tells the client the new stack size.
            await player.InvokeViewPlugInAsync<INpcItemBoughtPlugIn>(p => p.NpcItemBoughtAsync(targetItem)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(targetItem, false)).ConfigureAwait(false);
            if (purchasePlugIns is not null)
            {
                await purchasePlugIns.AfterPurchaseAsync(player, merchant, purchase, targetItem).ConfigureAwait(false);
            }
        }
        else
        {
            var toSlot = player.Inventory!.CheckInvSpace(itemTemplate);
            if (toSlot is null)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InventoryFull)).ConfigureAwait(false);
                await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
                return;
            }

            if (!await this.TryPayAsync(player, price).ConfigureAwait(false))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NotEnoughMoney)).ConfigureAwait(false);
                await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
                return;
            }

            var newItem = player.PersistenceContext.CreateNew<Item>();
            newItem.AssignValues(itemTemplate);
            newItem.ItemSlot = (byte)toSlot;
            await player.InvokeViewPlugInAsync<INpcItemBoughtPlugIn>(p => p.NpcItemBoughtAsync(newItem)).ConfigureAwait(false);
            await player.Inventory.AddItemAsync(newItem).ConfigureAwait(false);
            player.GameContext.PlugInManager.GetPlugInPoint<IItemBoughtFromMerchantPlugIn>()?.ItemBought(player, newItem, storeItem, merchant);
            if (purchasePlugIns is not null)
            {
                await purchasePlugIns.AfterPurchaseAsync(player, merchant, purchase, newItem).ConfigureAwait(false);
            }
        }

        await player.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync()).ConfigureAwait(false);
    }

    private ValueTask<bool> TryPayAsync(Player player, long price)
    {
        return this._castleSiegeTaxProvider.TryPayStoreCostAsync(player, price);
    }
}
