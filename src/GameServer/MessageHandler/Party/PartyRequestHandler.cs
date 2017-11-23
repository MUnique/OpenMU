// <copyright file="PartyRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for party request packets.
    /// </summary>
    internal class PartyRequestHandler : IPacketHandler
    {
        private readonly PartyRequestAction action = new PartyRequestAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            ushort toRequestId = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            var toRequest = player.GetObservingPlayerWithId(toRequestId);
            if (toRequest == null)
            {
                return;
            }

            this.action.HandlePartyRequest(player, toRequest);
        }
    }
}
