// <copyright file="ShowMoneyDrop097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Money drop packet format for 0.97 clients (packed group/number byte).
/// </summary>
[PlugIn("ShowMoneyDrop 0.97", "Uses the 0.75-style money drop packet for 0.97 clients.")]
[Guid("5C6B0D46-4A42-4D24-8A44-1F64C532E7F5")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ShowMoneyDrop097 : IShowMoneyDropPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMoneyDrop097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMoneyDrop097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowMoneyAsync(ushort itemId, bool isFreshDrop, uint amount, Point point)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return ValueTask.CompletedTask;
        }

        int Write()
        {
            const int itemDataLength = 5;
            var droppedItemLength = ItemsDroppedRef.DroppedItemRef.GetRequiredSize(itemDataLength);
            var size = ItemsDroppedRef.GetRequiredSize(1, droppedItemLength);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemsDroppedRef(span)
            {
                ItemCount = 1,
            };

            int headerSize = ItemsDroppedRef.GetRequiredSize(0, 0);
            var itemBlock = new ItemsDroppedRef.DroppedItemRef(span[headerSize..]);
            itemBlock.Id = itemId;
            itemBlock.IsFreshDrop = isFreshDrop;
            itemBlock.PositionX = point.X;
            itemBlock.PositionY = point.Y;

            var itemData = itemBlock.ItemData[..itemDataLength];
            itemData.Clear();
            EncodeMoneyItemData(itemData, amount);

            span.Slice(0, size).SetPacketSize();
            return size;
        }

        return connection.SendAsync(Write);
    }

    private static void EncodeMoneyItemData(Span<byte> itemData, uint amount)
    {
        const byte moneyGroup = 14;
        const byte moneyNumber = 15;
        var itemIndex = (ushort)((moneyNumber & 0x1F) | (moneyGroup << 5));

        itemData[0] = (byte)(itemIndex & 0xFF);
        itemData[1] = (byte)((amount >> 16) & 0xFF);
        itemData[2] = (byte)((amount >> 8) & 0xFF);
        itemData[3] = (byte)((itemIndex & 0x100) >> 1);
        itemData[4] = (byte)(amount & 0xFF);
    }
}
