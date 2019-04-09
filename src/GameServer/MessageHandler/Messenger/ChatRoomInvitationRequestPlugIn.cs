// <copyright file="ChatRoomInvitationRequestPlugIn.cs" company="MUnique">
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
    /// Packet handler which handles chat room invitation requests,
    /// where a player invites additional players to an existing chat room.
    /// </summary>
    [PlugIn("ChatRoomInvitationRequestPlugIn", "Packet handler which handles chat room invitation requests, where a player invites additional players to an existing chat room.")]
    [Guid("fc779867-7d2d-4409-83b4-b6616bb9234e")]
    public class ChatRoomInvitationRequestPlugIn : IPacketHandlerPlugIn
    {
        private readonly ChatRequestAction chatRequestAction = new ChatRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.ChatRoomInvitationReq;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // C1 13 CB [FriendName] [RoomNumber] [RequestId]
            if (packet.Length < 0x13)
            {
                // Log?
                return;
            }

            var friendName = packet.ExtractString(3, 10, Encoding.UTF8);
            var roomId = packet.MakeWordSmallEndian(13);
            var requestId = packet.MakeDwordSmallEndian(15);
            this.chatRequestAction.InviteFriendToChat(player, friendName, roomId, requestId);
        }
    }
}
