// <copyright file="PlayerShopRequestItemListPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using PlugInResources = MUnique.OpenMU.GameServer.Properties.PlugInResources;

/// <summary>
/// Packet handler which handles requests for the item list of another player (3F 05).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.PlayerShopRequestItemListPacketHandlerPlugIn_Name), Description = nameof(PlugInResources.PlayerShopRequestItemListPacketHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
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
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OpenStorePlayerNotFound)).ConfigureAwait(false);
            return;
        }

        if (message.PlayerName != requestedPlayer.SelectedCharacter?.Name)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerNamesDontMatch), message.PlayerName, requestedPlayer.SelectedCharacter?.Name).ConfigureAwait(false);
            return;
        }

        await this._requestListAction.RequestStoreItemListAsync(player, requestedPlayer).ConfigureAwait(false);
    }
}