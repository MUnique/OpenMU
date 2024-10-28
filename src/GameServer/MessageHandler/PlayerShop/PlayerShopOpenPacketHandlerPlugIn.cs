// <copyright file="PlayerShopOpenPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler which opens the player shop (3F 02).
/// </summary>
[PlugIn("Player Shop - Open", "Packet handler which opens the player shop (3F 02).")]
[Guid("6997E1D3-B722-40AE-8F71-A65BB4377529")]
[BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
internal class PlayerShopOpenPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly OpenStoreAction _openStoreAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => PlayerShopOpen.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PlayerShopOpen message = packet;
        await this._openStoreAction.OpenStoreAsync(player, message.StoreName).ConfigureAwait(false);
    }
}