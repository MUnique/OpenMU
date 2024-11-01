// <copyright file="PlayerShopSetItemPricePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler which sets prices for an item in the player shop (3F 01).
/// </summary>
[PlugIn("Player Shop - Set Price", "Packet handler which sets prices for an item in the player shop (3F 01).")]
[Guid("0E78ADBD-4B3D-4D3E-B5B1-34FA66BFC854")]
[BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
internal class PlayerShopSetItemPricePacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly SetItemPriceAction _setPriceAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => PlayerShopSetItemPrice.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PlayerShopSetItemPrice message = packet;
        player.Logger.LogDebug("Player [{0}] sets price of slot {1} to {2}", player.SelectedCharacter?.Name, message.ItemSlot, message.Price);
        await this._setPriceAction.SetPriceAsync(player, message.ItemSlot, (int)message.Price).ConfigureAwait(false);
    }
}