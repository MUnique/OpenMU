// <copyright file="GuildKickPlayerHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for guild player kick packets.
    /// </summary>
    internal class GuildKickPlayerHandler : IPacketHandler
    {
        private readonly GuildKickPlayerAction kickAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildKickPlayerHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildKickPlayerHandler(IGameServerContext gameContext)
        {
            this.kickAction = new GuildKickPlayerAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player guildMaster, Span<byte> packet)
        {
            var nickname = packet.ExtractString(3, 10, Encoding.UTF8);
            var securityCode = packet.ExtractString(13, packet.Length - 13, Encoding.UTF8);

            this.kickAction.KickPlayer(guildMaster, nickname, securityCode);
        }
    }
}
