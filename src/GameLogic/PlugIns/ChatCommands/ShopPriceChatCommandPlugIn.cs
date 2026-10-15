// <copyright file="ShopPriceChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets the prices of the items in the own player store, in any of the enabled
/// currencies (see <see cref="PlayerShopCurrenciesFeaturePlugIn"/>). Without arguments, it lists the prices.
/// The currency is "zen", an account coin ("wcc", "wcp", "gp"), a jewel ("bless", "soul", ...) or the group and
/// number of an item, e.g. "14/13".
/// </summary>
/// <remarks>
/// It works with every client, so also players of the original client can set prices in other currencies than zen.
/// Their client doesn't show these prices, though.
/// </remarks>
[Guid("A5A3B2CE-7D58-43D6-81F3-804F078F0990")]
[PlugIn]
[Display(Name = nameof(PlugInResources.ShopPriceChatCommandPlugIn_Name), Description = nameof(PlugInResources.ShopPriceChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(ShopPriceChatCommandArgs), CharacterStatus.Normal)]
public class ShopPriceChatCommandPlugIn : ChatCommandPlugInBase<ShopPriceChatCommandArgs>, IDisabledByDefault
{
    private const string Command = "/shopprice";

    private readonly SetItemPriceAction _setItemPriceAction = new();

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, ShopPriceChatCommandArgs arguments)
    {
        if (player.ShopStorage is not { } shopStorage)
        {
            return;
        }

        if (arguments.Slot == 0)
        {
            await ShowPricesAsync(player, shopStorage).ConfigureAwait(false);
            return;
        }

        if (GetStoreSlot(arguments.Slot) is not { } slot || arguments.Amount < 0)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceCommandUsage), Command).ConfigureAwait(false);
            return;
        }

        if (!ShopPrices.TryParse(player.GameContext, arguments.Currency, arguments.Amount, out var price))
        {
            var allowedCurrencies = ShopPrices.GetAllowedChatNames(player.GameContext);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopUnknownCurrency), arguments.Currency, string.Join(", ", allowedCurrencies)).ConfigureAwait(false);
            return;
        }

        if (shopStorage.StoreOpen)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopCloseToChangePrices)).ConfigureAwait(false);
            return;
        }

        var position = slot - InventoryConstants.FirstStoreItemSlotIndex + 1;
        var result = await this._setItemPriceAction.TrySetPriceAsync(player, slot, price).ConfigureAwait(false);
        switch (result)
        {
            case ItemPriceResult.Success when arguments.Amount > 0:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceSet), position, arguments.Amount, player.GetCurrencyName(price)).ConfigureAwait(false);
                break;
            case ItemPriceResult.Success:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceRemoved), position).ConfigureAwait(false);
                break;
            case ItemPriceResult.ItemNotFound:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceItemNotFound), position).ConfigureAwait(false);
                break;
            case ItemPriceResult.CharacterLevelTooLow:
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceLevelTooLow)).ConfigureAwait(false);
                break;
            default:
                // The action already showed a message about the reason.
                break;
        }
    }

    private static byte? GetStoreSlot(int slotArgument)
    {
        if (slotArgument >= 1 && slotArgument <= InventoryConstants.StoreSize)
        {
            return (byte)(InventoryConstants.FirstStoreItemSlotIndex + slotArgument - 1);
        }

        if (slotArgument >= InventoryConstants.FirstStoreItemSlotIndex
            && slotArgument < InventoryConstants.FirstStoreItemSlotIndex + InventoryConstants.StoreSize)
        {
            return (byte)slotArgument;
        }

        return null;
    }

    private static async ValueTask ShowPricesAsync(Player player, IShopStorage shopStorage)
    {
        var items = shopStorage.Items.OrderBy(i => i.ItemSlot).ToList();
        if (items.Count == 0)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopIsEmpty)).ConfigureAwait(false);
            return;
        }

        foreach (var item in items)
        {
            var position = item.ItemSlot - InventoryConstants.FirstStoreItemSlotIndex + 1;
            if (item.GetStorePrice() is { } price)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceListEntry), position, item.ToString(player.Culture), price.Amount, player.GetCurrencyName(price)).ConfigureAwait(false);
            }
            else
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopPriceListEntryWithoutPrice), position, item.ToString(player.Culture)).ConfigureAwait(false);
            }
        }
    }
}
