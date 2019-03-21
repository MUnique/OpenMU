﻿// <copyright file="GuildJoinResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IGuildJoinResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("GuildJoinResponsePlugIn", "The default implementation of the IGuildJoinResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("4a8bd97c-a544-4cac-b8cd-2e73945bcfdc")]
    public class GuildJoinResponsePlugIn : IGuildJoinResponsePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildJoinResponsePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public GuildJoinResponsePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void GuildJoinResponse(GuildRequestAnswerResult result)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x51;
                packet[3] = (byte)result;
                writer.Commit();
            }
        }
    }
}