// <copyright file="TradeButtonHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Handles the trade button packets.
    /// </summary>
    internal class TradeButtonHandler : IPacketHandler
    {
        private readonly TradeButtonAction buttonAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeButtonHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public TradeButtonHandler(IGameContext gameContext)
        {
            this.buttonAction = new TradeButtonAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 4)
            {
                return;
            }

            this.buttonAction.TradeButtonChanged(player, (TradeButtonState)packet[3]);
        }
    }
}
