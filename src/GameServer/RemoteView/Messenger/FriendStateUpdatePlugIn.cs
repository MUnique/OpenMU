﻿// <copyright file="FriendStateUpdatePlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IFriendStateUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("FriendStateUpdatePlugIn", "The default implementation of the IFriendStateUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("1ed98d6a-7917-4209-bd03-e8a966b99688")]
    public class FriendStateUpdatePlugIn : IFriendStateUpdatePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendStateUpdatePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public FriendStateUpdatePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void FriendStateUpdate(string friend, int serverId)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0E))
            {
                var packet = writer.Span;
                packet[2] = 0xC4;
                packet.Slice(3).WriteString(friend, Encoding.UTF8);
                packet[packet.Length - 1] = (byte)serverId;
                writer.Commit();
            }
        }
    }
}