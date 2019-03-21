// <copyright file="ShowFriendRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowFriendRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowFriendRequestPlugIn", "The default implementation of the IShowFriendRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("efb8b51b-181a-4a7d-b5a4-34c0dbd41748")]
    public class ShowFriendRequestPlugIn : IShowFriendRequestPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowFriendRequestPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowFriendRequestPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowFriendRequest(string requester)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0D))
            {
                var packet = writer.Span;
                packet[2] = 0xC2;
                packet.Slice(3).WriteString(requester, Encoding.UTF8);
                writer.Commit();
            }
        }
    }
}