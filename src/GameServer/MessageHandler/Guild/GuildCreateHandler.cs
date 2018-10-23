// <copyright file="GuildCreateHandler.cs" company="MUnique">
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
    /// Handler for guild create packets.
    /// </summary>
    internal class GuildCreateHandler : IPacketHandler
    {
        private readonly GuildCreateAction createAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildCreateHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildCreateHandler(IGameServerContext gameContext)
        {
            this.createAction = new GuildCreateAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var guildName = packet.ExtractString(4, 9, Encoding.UTF8);
            var guildEmblem = packet.Slice(12, 32).ToArray();
            this.createAction.CreateGuild(player, guildName, guildEmblem);
        }
    }
}
