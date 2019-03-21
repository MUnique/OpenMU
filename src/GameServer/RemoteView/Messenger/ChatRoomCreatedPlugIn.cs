// <copyright file="ChatRoomCreatedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IChatRoomCreatedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ChatRoomCreatedPlugIn", "The default implementation of the IChatRoomCreatedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("a7c99cb5-94f6-42ea-b6e2-2272a9a81e12")]
    public class ChatRoomCreatedPlugIn : IChatRoomCreatedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRoomCreatedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ChatRoomCreatedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string friendName, bool success)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 0x24))
            {
                var chatServerIp = this.player.GameServerContext.FriendServer.GetChatserverIP();
                var packet = writer.Span;
                packet[2] = 0xCA;
                packet.Slice(3, 15).WriteString(chatServerIp, Encoding.UTF8);
                var chatRoomId = authenticationInfo.RoomId;
                packet[18] = chatRoomId.GetLowByte();
                packet[19] = chatRoomId.GetHighByte();
                packet.Slice(20, 4).SetIntegerBigEndian(uint.Parse(authenticationInfo.AuthenticationToken));

                packet[24] = 0x01; // type
                packet.Slice(25, 10).WriteString(friendName, Encoding.UTF8);
                packet[35] = success ? (byte)1 : (byte)0;
                writer.Commit();
            }
        }
    }
}