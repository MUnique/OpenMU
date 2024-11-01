// <copyright file="ItemMoveHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Inventory;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for item move packets.
/// </summary>
[PlugIn("ItemMoveHandlerPlugIn", "Handler for item move packets.")]
[Guid("c499c596-7711-4971-bc83-7abd9e6b5553")]
internal class ItemMoveHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly MoveItemAction _moveAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ItemMoveRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ItemMoveRequest message = packet;

        // to make it compatible with multiple versions, we just handle the data which is coming after that manually
        var itemSize = 12;
        if (player is RemotePlayer remotePlayer)
        {
            itemSize = remotePlayer.ItemSerializer.NeededSpace;
        }

        var toStorage = (ItemStorageKind)packet.Span[5 + itemSize];
        byte toSlot = packet.Span[6 + itemSize];

        await this._moveAction.MoveItemAsync(player, message.FromSlot, message.FromStorage.Convert(), toSlot, toStorage.Convert()).ConfigureAwait(false);
    }
}