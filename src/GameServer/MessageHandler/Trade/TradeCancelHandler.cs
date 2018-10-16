// <copyright file="TradeCancelHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;

    /// <summary>
    /// Handles the trade cancel packets.
    /// </summary>
    internal class TradeCancelHandler : IPacketHandler
    {
        private readonly TradeCancelAction cancelHandler = new TradeCancelAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.cancelHandler.CancelTrade(player);
        }
    }
}
