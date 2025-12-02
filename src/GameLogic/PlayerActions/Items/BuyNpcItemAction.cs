// <copyright file="BuyNpcItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to buy items from a Monster merchant.
/// </summary>
public class BuyNpcItemAction
{
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
        if (storeItem is null)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Item Unknown", MessageType.BlueNormal)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
            return;
        }

        // Inventory Update:
        if (storeItem.IsStackable() && player.Inventory!.Items.FirstOrDefault(item => storeItem.CanCompletelyStackOn(item)) is { } targetItem)
        {
            if (!this.CheckMoney(player, storeItem))
            {
                return;
            }

            targetItem.Durability += storeItem.Durability;
            await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(targetItem, false)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
        }
        else
        {
            var toSlot = player.Inventory!.CheckInvSpace(storeItem);
            if (toSlot is null)
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Inventory Full", MessageType.BlueNormal)).ConfigureAwait(false);
                await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
                return;
            }

            if (!this.CheckMoney(player, storeItem))
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You don't have enough Money", MessageType.BlueNormal)).ConfigureAwait(false);
                await player.InvokeViewPlugInAsync<IBuyNpcItemFailedPlugIn>(p => p.BuyNpcItemFailedAsync()).ConfigureAwait(false);
                return;
            }

            var newItem = player.PersistenceContext.CreateNew<Item>();
            newItem.AssignValues(storeItem);
            newItem.ItemSlot = (byte)toSlot;
            await player.InvokeViewPlugInAsync<INpcItemBoughtPlugIn>(p => p.NpcItemBoughtAsync(newItem)).ConfigureAwait(false);
            await player.Inventory.AddItemAsync(newItem).ConfigureAwait(false);
            player.GameContext.PlugInManager.GetPlugInPoint<IItemBoughtFromMerchantPlugIn>()?.ItemBought(player, newItem, storeItem, player.OpenedNpc);
        }

        await player.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync()).ConfigureAwait(false);
    }

    private bool CheckMoney(Player player, Item item)
    {
        var price = this._priceCalculator.CalculateFinalBuyingPrice(item);
        if (!player.TryRemoveMoney((int)price))
        {
            return false;
        }

        return true;
    }
}