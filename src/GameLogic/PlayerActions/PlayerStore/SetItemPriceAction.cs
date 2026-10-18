// <copyright file="SetItemPriceAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;

using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;

/// <summary>
/// Action to set the price of an item of the player store.
/// </summary>
public class SetItemPriceAction
{
    /// <summary>
    /// Sets the price of an item in zen.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The slot of the item in the store (0 to 31).</param>
    /// <param name="price">The price.</param>
    public ValueTask SetPriceAsync(Player player, byte slot, int price)
    {
        return this.SetPriceAsync(player, slot, new ShopPrice(price));
    }

    /// <summary>
    /// Sets the price of an item, which may be paid with zen, items or account coins.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The slot of the item in the store (0 to 31).</param>
    /// <param name="price">The price.</param>
    public async ValueTask SetPriceAsync(Player player, byte slot, ShopPrice price)
    {
        var result = await this.TrySetPriceAsync(player, slot, price).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemPriceSetResponsePlugIn>(p => p.ItemPriceSetResponseAsync(slot, result)).ConfigureAwait(false);
    }

    /// <summary>
    /// Tries to set the price of an item, without sending the response.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The slot of the item in the inventory.</param>
    /// <param name="price">The price. An amount of 0 removes the price.</param>
    /// <returns>The result.</returns>
    /// <remarks>
    /// When a price isn't allowed, or is too high, a message is shown to the player,
    /// because the result can't express it.
    /// </remarks>
    public async ValueTask<ItemPriceResult> TrySetPriceAsync(Player player, byte slot, ShopPrice price)
    {
        if (player.ShopStorage?.StoreOpen ?? false)
        {
            return ItemPriceResult.Failed;
        }

        if (player.Level < 6)
        {
            return ItemPriceResult.CharacterLevelTooLow;
        }

        var item = player.SelectedCharacter?.Inventory?.Items?.FirstOrDefault(i => i.ItemSlot == slot);
        if (item is null)
        {
            return ItemPriceResult.ItemNotFound;
        }

        if (price.Amount < 0)
        {
            // Like before the other prices were introduced, a negative price removes the price.
            item.SetStorePrice(null);
            return ItemPriceResult.PriceNegative;
        }

        if (price.Amount == 0)
        {
            item.SetStorePrice(null);
            return ItemPriceResult.Success;
        }

        if (price.Kind != ShopPriceKind.Zen)
        {
            if (!ShopPrices.IsAllowed(player.GameContext, price))
            {
                player.Logger.LogWarning("Player {player} tried to set a price in {currency}, which isn't allowed.", player, price.PriceItem?.ToString() ?? price.CoinType?.ToString());
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopCurrencyNotAllowed), player.GetCurrencyName(price)).ConfigureAwait(false);
                return ItemPriceResult.Failed;
            }

            var maximumAmount = ShopPrices.GetMaximumAmount(player.GameContext, price.Kind);
            if (price.Amount > maximumAmount)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceTooHigh), maximumAmount, player.GetCurrencyName(price)).ConfigureAwait(false);
                return ItemPriceResult.Failed;
            }
        }

        item.SetStorePrice(price);
        return ItemPriceResult.Success;
    }
}
