// <copyright file="ChatRoomCreatedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
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
            var chatServerIp = this.player.GameServerContext.FriendServer.ChatServerIp;
            this.player.Connection?.SendChatRoomConnectionInfo(chatServerIp, authenticationInfo.RoomId, uint.Parse(authenticationInfo.AuthenticationToken), friendName, success);
        }
    }
}