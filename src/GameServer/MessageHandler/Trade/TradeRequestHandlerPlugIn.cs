// <copyright file="TradeRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handles the trade request packets.
    /// </summary>
    [PlugIn("TradeRequestHandlerPlugIn", "Handles the trade request packets.")]
    [Guid("f2b8c4c0-2e9d-4f1f-8c42-76b0312e4021")]
    internal class TradeRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly TradeRequestAction requestAction = new TradeRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.TradeRequest;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ////0xC3, 0x05, 0x36, idtotrade[0], idtotrade[1]
            ushort pid = NumberConversionExtensions.MakeWord(packet[4], packet[3]);

            Player partner = player.GetObservingPlayerWithId(pid);
            if (partner == null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Trade partner not found.", MessageType.BlueNormal);
                return;
            }

            this.requestAction.RequestTrade(player, partner);
        }
    }
}
