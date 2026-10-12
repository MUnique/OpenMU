// <copyright file="PlayerShopSetItemPriceExtendedPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.GameServer.RemoteView.PlayerShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler which sets prices in zen, items or account coins for an item in the player shop (3F 09).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.PlayerShopSetItemPriceExtendedPacketHandlerPlugIn_Name), Description = nameof(PlugInResources.PlayerShopSetItemPriceExtendedPacketHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E7528764-B648-43D1-A8B4-FD167CD3EF6F")]
[BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
internal class PlayerShopSetItemPriceExtendedPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly SetItemPriceAction _setPriceAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => PlayerShopSetItemPriceExtended.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < PlayerShopSetItemPriceExtended.Length)
        {
            player.Logger.LogWarning("Ignoring truncated player shop set item price packet of length {Length}.", packet.Length);
            return;
        }

        PlayerShopSetItemPriceExtended message = packet;
        player.Logger.LogDebug(
            "Player [{0}] sets price of slot {1} to {2}/{3}/{4}",
            player.SelectedCharacter?.Name,
            message.ItemSlot,
            message.MoneyPrice,
            message.PriceItemType,
            message.RequiredItemAmount);

        if (PlayerShopPriceEncoding.Decode(message.MoneyPrice, message.PriceItemType, message.RequiredItemAmount, player.GameContext.Configuration) is not { } price)
        {
            player.Logger.LogWarning("Player [{0}] sent a price with the unknown item type {1}.", player.SelectedCharacter?.Name, message.PriceItemType);
            await player.InvokeViewPlugInAsync<IItemPriceSetResponsePlugIn>(p => p.ItemPriceSetResponseAsync(message.ItemSlot, ItemPriceResult.Failed)).ConfigureAwait(false);
            return;
        }

        await this._setPriceAction.SetPriceAsync(player, message.ItemSlot, price).ConfigureAwait(false);
    }
}
