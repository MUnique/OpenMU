// <copyright file="PlayerLeftGuildPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IPlayerLeftGuildPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("PlayerLeftGuildPlugIn", "The default implementation of the IPlayerLeftGuildPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("e37868e8-6fdf-41fb-aa7f-01e6f569f5c0")]
    public class PlayerLeftGuildPlugIn : IPlayerLeftGuildPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLeftGuildPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public PlayerLeftGuildPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void PlayerLeftGuild(Player player)
        {
            var writer = this.player.Connection.StartSafeWrite(GuildMemberLeftGuild.HeaderType, GuildMemberLeftGuild.Length);
            _ = new GuildMemberLeftGuild(writer.Span)
            {
                PlayerId = this.player.GetId(player),
                IsGuildMaster = player.GuildStatus?.Position == GuildPosition.GuildMaster,
            };
            writer.Commit();
        }
    }
}
