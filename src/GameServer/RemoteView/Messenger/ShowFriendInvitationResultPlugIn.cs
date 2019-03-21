﻿// <copyright file="ShowFriendInvitationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowFriendInvitationResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowFriendInvitationResultPlugIn", "The default implementation of the IShowFriendInvitationResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("8df329bd-88da-423c-8f85-173180ab8601")]
    public class ShowFriendInvitationResultPlugIn : IShowFriendInvitationResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowFriendInvitationResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowFriendInvitationResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowFriendInvitationResult(bool success, uint requestId)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 8))
            {
                var packet = writer.Span;
                packet[2] = 0xCB;
                packet[3] = success ? (byte)1 : (byte)0;
                packet.Slice(4, 4).SetIntegerSmallEndian(requestId);
                writer.Commit();
            }
        }
    }
}