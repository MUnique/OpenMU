// <copyright file="PartyKickHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;

    /// <summary>
    /// Handler for party kick packets.
    /// </summary>
    internal class PartyKickHandler : IPacketHandler
    {
        private readonly PartyKickAction action = new PartyKickAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            byte index = packet[3];
            this.action.KickPlayer(player, index);
        }
    }
}
