// <copyright file="FriendDeletedPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IFriendDeletedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("FriendDeletedPlugIn", "The default implementation of the IFriendDeletedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("3f6f86ec-ffe8-4fa7-82c3-4a743fab7157")]
    public class FriendDeletedPlugIn : IFriendDeletedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendDeletedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public FriendDeletedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void FriendDeleted(string deletingFriend)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0E))
            {
                var packet = writer.Span;
                packet[2] = 0xC3;
                packet[3] = 0x01;
                packet.Slice(4).WriteString(deletingFriend, Encoding.UTF8);
                writer.Commit();
            }
        }
    }
}