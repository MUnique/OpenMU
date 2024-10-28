// <copyright file="PlayerShopRequestItemListPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler which handles requests for the item list of another player (3F 05).
/// </summary>
[PlugIn("Player Shop - Request Item List", "Packet handler which handles requests for the item list of another player (3F 05).")]
[Guid("5A87AD36-1778-4CA4-AE08-B7BC12135C1B")]
[BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
internal class PlayerShopRequestItemListPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly StoreItemListRequestAction _requestListAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => PlayerShopItemListRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PlayerShopItemListRequest message = packet;
        if (player.CurrentMap?.GetObject(message.PlayerId) is not Player requestedPlayer)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Open Store: Player not found.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        if (message.PlayerName != requestedPlayer.SelectedCharacter?.Name)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Player Names don't match." + message.PlayerName + "<>" + requestedPlayer.SelectedCharacter?.Name, MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        await this._requestListAction.RequestStoreItemListAsync(player, requestedPlayer).ConfigureAwait(false);
    }
}