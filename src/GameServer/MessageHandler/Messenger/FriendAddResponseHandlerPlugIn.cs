// <copyright file="FriendAddResponseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for friend add response packets.
    /// </summary>
    [PlugIn("FriendAddResponseHandlerPlugIn", "Handler for friend add response packets.")]
    [Guid("171b8f75-3927-4325-b694-54130365e4a2")]
    internal class FriendAddResponseHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly AddResponseAction responseAction = new AddResponseAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.FriendAddReponse;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 0x0E)
            {
                // log
                return;
            }

            var requesterName = packet.ExtractString(4, 10, Encoding.UTF8);
            var accepted = packet[3] == 1;
            this.responseAction.ProceedResponse(player, requesterName, accepted);
        }
    }
}
