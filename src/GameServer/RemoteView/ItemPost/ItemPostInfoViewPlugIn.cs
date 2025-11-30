// <copyright file="ItemPostInfoViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.ItemPost;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.ItemPost;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Forwards posted item details to the client.
/// </summary>
[PlugIn(nameof(ItemPostInfoViewPlugIn), "Forwards posted item details to the client.")]
[Guid("E2C0AA04-FAAF-42CB-9CA3-14EBED971F3B")]
public class ItemPostInfoViewPlugIn : IItemPostInfoViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPostInfoViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemPostInfoViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowItemPostInfoAsync(uint postId, Item item)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        _ = postId;

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var size = ItemPostInfo.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemPostInfo(span);
            var itemSize = itemSerializer.SerializeItem(packet.ItemData, item);
            var actualSize = ItemPostInfo.GetRequiredSize(itemSize);
            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
