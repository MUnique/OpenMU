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
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handles the trade request packets.
    /// </summary>
    [PlugIn("TradeRequestHandlerPlugIn", "Handles the trade request packets.")]
    [Guid("f2b8c4c0-2e9d-4f1f-8c42-76b0312e4021")]
    internal class TradeRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly TradeRequestAction requestAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => TradeRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            TradeRequest message = packet;
            var partner = player.GetObservingPlayerWithId(message.PlayerId);
            if (partner is null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Trade partner not found.", MessageType.BlueNormal);
                return;
            }

            this.requestAction.RequestTrade(player, partner);
        }
    }
}
