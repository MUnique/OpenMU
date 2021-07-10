// <copyright file="ShowTradeRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowTradeRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowTradeRequestPlugIn", "The default implementation of the IShowTradeRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("2e6e8c0e-8220-46e3-931a-630d596178ca")]
    public class ShowTradeRequestPlugIn : IShowTradeRequestPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowTradeRequestPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowTradeRequestPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowTradeRequest(ITrader requester)
        {
            this.player.Connection?.SendTradeRequest(requester.Name);
        }
    }
}