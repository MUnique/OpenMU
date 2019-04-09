// <copyright file="PartyRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for party request packets.
    /// </summary>
    [PlugIn("PartyRequestHandlerPlugIn", "Handler for party request packets.")]
    [Guid("759d5b1a-a2f9-4de8-a03e-023a4810111d")]
    internal class PartyRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly PartyRequestAction action = new PartyRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.PartyRequest;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
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
