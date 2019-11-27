// <copyright file="TradeItemAppearPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ITradeItemAppearPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("TradeItemAppearPlugIn", "The default implementation of the ITradeItemAppearPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("560d18e7-7d36-47bf-9298-8017454fa7bf")]
    public class TradeItemAppearPlugIn : ITradeItemAppearPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeItemAppearPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public TradeItemAppearPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void TradeItemAppear(byte toSlot, Item item)
        {
            var itemSerializer = this.player.ItemSerializer;
            using var writer = this.player.Connection.StartSafeWrite(TradeItemAdded.HeaderType, TradeItemAdded.GetRequiredSize(itemSerializer.NeededSpace));
            var packet = new TradeItemAdded(writer.Span)
            {
                ToSlot = toSlot,
            };
            itemSerializer.SerializeItem(packet.ItemData, item);
            writer.Commit();
        }
    }
}