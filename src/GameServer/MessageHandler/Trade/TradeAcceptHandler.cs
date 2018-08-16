// <copyright file="TradeAcceptHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;

    /// <summary>
    /// Packet Handler which is called when a trade request gets answered by the player.
    /// </summary>
    internal class TradeAcceptHandler : IPacketHandler
    {
        private readonly TradeAcceptAction acceptAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAcceptHandler"/> class.
        /// </summary>
        public TradeAcceptHandler()
        {
            this.acceptAction = new TradeAcceptAction();
        }

        /// <inheritdoc/>
        /// <summary>The packet looks like: 0xC1, 0x04, 0x37, 0x01.</summary>
        public void HandlePacket(Player player, byte[] packet)
        {
            if (packet.Length < 4)
            {
                return;
            }

            this.acceptAction.HandleTradeAccept(player, packet[3] == 1);
        }
    }
}
