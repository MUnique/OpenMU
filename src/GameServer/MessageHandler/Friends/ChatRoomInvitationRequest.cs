// <copyright file="ChatRoomInvitationRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Friends
{
    using System;
    using System.Text;
    using GameLogic.Interfaces;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Packet handler which handles chat room invitation requests,
    /// where a player invites additional players to an existing chat room.
    /// </summary>
    public class ChatRoomInvitationRequest : IPacketHandler
    {
        private readonly ChatRequestAction chatRequestAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRoomInvitationRequest"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChatRoomInvitationRequest(IGameServerContext gameContext)
        {
            this.chatRequestAction = new ChatRequestAction(gameContext);
        }

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
