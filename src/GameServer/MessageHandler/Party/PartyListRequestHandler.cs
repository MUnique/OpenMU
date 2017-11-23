// <copyright file="PartyListRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;

    /// <summary>
    /// Handler for party list request packets.
    /// </summary>
    internal class PartyListRequestHandler : IPacketHandler
    {
        private readonly PartyListRequestAction action = new PartyListRequestAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            this.action.RequestPartyList(player);
        }
    }
}
