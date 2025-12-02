// <copyright file="PlayerShopBuyRequestPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler which handles buy requests to a player shop (3F 06).
/// </summary>
[PlugIn("Player Shop - Buy request", "Packet handler which handles buy requests to a player shop (3F 06).")]
[Guid("F5B72F91-9651-433D-AC23-5898B950A09B")]
[BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
internal class PlayerShopBuyRequestPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly BuyRequestAction _buyAction = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => PlayerShopItemBuyRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        PlayerShopItemBuyRequest message = packet;

        if (player.CurrentMap?.GetObject(message.PlayerId) is not Player requestedPlayer)
        {
            player.Logger.LogDebug("Player not found: {0}", message.PlayerId);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(null, ItemBuyResult.NotAvailable, null)).ConfigureAwait(false);
            return;
        }

        if (message.PlayerName != requestedPlayer.SelectedCharacter?.Name)
        {
            player.Logger.LogDebug("Player Names don't match: {0} != {1}", message.PlayerName, requestedPlayer.SelectedCharacter?.Name);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(null, ItemBuyResult.NameMismatchOrPriceMissing, null)).ConfigureAwait(false);
            return;
        }

        await this._buyAction.BuyItemAsync(player, requestedPlayer, message.ItemSlot).ConfigureAwait(false);
    }
}