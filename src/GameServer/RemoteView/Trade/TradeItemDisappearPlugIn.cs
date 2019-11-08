// <copyright file="TradeItemDisappearPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="ITradeItemDisappearPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("TradeItemDisappearPlugIn", "The default implementation of the ITradeItemDisappearPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("5ac9569b-3c5a-468e-bf48-1e67f81281c5")]
    public class TradeItemDisappearPlugIn : ITradeItemDisappearPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeItemDisappearPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public TradeItemDisappearPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void TradeItemDisappear(byte slot, Item item)
        {
            using var writer = this.player.Connection.StartSafeWrite(TradeItemRemoved.HeaderType, TradeItemRemoved.Length);
            _ = new TradeItemRemoved(writer.Span)
            {
                Slot = slot,
            };

            writer.Commit();
        }
    }
}
