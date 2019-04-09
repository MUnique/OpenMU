// <copyright file="TradeButtonHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handles the trade button packets.
    /// </summary>
    [PlugIn("TradeButtonHandlerPlugIn", "Handles the trade button packets.")]
    [Guid("4e70bdec-c890-4e7d-93a9-1801f821f322")]
    internal class TradeButtonHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly TradeButtonAction buttonAction = new TradeButtonAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.TradeButton;

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
