// <copyright file="UpdateInventoryListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateInventoryListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateInventoryListPlugIn", "The default implementation of the IUpdateInventoryListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("ba8ca7c7-a497-497e-b2f7-9f9366ff6ac5")]
public class UpdateInventoryListPlugIn : IUpdateInventoryListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateInventoryListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateInventoryListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateInventoryListAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.SelectedCharacter?.Inventory is null)
        {
            return;
        }

        // C4 00 00 00 F3 10 ...
        var items = this._player.SelectedCharacter.Inventory.Items.OrderBy(item => item.ItemSlot).ToList();
        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var lengthPerItem = StoredItemRef.GetRequiredSize(itemSerializer.NeededSpace);
            var size = CharacterInventoryRef.GetRequiredSize(items.Count, lengthPerItem);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CharacterInventoryRef(span)
            {
                ItemCount = (byte)items.Count,
            };

            int headerSize = CharacterInventoryRef.GetRequiredSize(0, 0);
            int actualSize = headerSize;
            int i = 0;
            foreach (var item in items)
            {
                if (item.Definition is null)
                {
                    this._player.Logger.LogWarning("Item {0} has no definition.", item);
                    packet.ItemCount--;
                    continue;
                }

                var storedItem = new StoredItemRef(span[actualSize..]);
                storedItem.ItemSlot = item.ItemSlot;
                var itemSize = itemSerializer.SerializeItem(storedItem.ItemData, item);
                actualSize += StoredItemRef.GetRequiredSize(itemSize);
                i++;
            }

            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}