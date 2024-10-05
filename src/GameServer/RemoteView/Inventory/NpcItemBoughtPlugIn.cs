// <copyright file="NpcItemBoughtPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="INpcItemBoughtPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("NpcItemBoughtPlugIn", "The default implementation of the INpcItemBoughtPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("cf45b5e2-158a-4998-bc73-fed4d4d31c0c")]
public class NpcItemBoughtPlugIn : INpcItemBoughtPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpcItemBoughtPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NpcItemBoughtPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask NpcItemBoughtAsync(Item newItem)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var itemSerializer = this._player.ItemSerializer;

        int Write()
        {
            var size = ItemBoughtRef.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemBoughtRef(span)
            {
                InventorySlot = newItem.ItemSlot,
            };
            var itemSize = itemSerializer.SerializeItem(packet.ItemData, newItem);
            var actualSize = ItemBoughtRef.GetRequiredSize(itemSize);
            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}