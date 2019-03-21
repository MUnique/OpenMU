﻿// <copyright file="ShowGuildCreateResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowGuildCreateResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowGuildCreateResultPlugIn", "The default implementation of the IShowGuildCreateResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ff6c5a06-4699-461b-9004-756269393c40")]
    public class ShowGuildCreateResultPlugIn : IShowGuildCreateResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowGuildCreateResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowGuildCreateResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowGuildCreateResult(GuildCreateErrorDetail errorDetail)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x56;
                packet[3] = (byte)(errorDetail == GuildCreateErrorDetail.None ? 1 : 0);
                packet[4] = (byte)errorDetail;
                writer.Commit();
            }
        }
    }
}