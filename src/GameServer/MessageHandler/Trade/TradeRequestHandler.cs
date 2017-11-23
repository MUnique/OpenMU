// <copyright file="TradeRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handles the trade request packets.
    /// </summary>
    internal class TradeRequestHandler : IPacketHandler
    {
        private readonly TradeRequestAction requestAction = new TradeRequestAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            ////0xC3, 0x05, 0x36, idtotrade[0], idtotrade[1]
            ushort pid = NumberConversionExtensions.MakeWord(packet[4], packet[3]);

            Player partner = player.GetObservingPlayerWithId(pid);
            if (partner == null)
            {
                player.PlayerView.ShowMessage("Trade partner not found.", MessageType.BlueNormal);
                return;
            }

            this.requestAction.RequestTrade(player, partner);
        }
    }
}
