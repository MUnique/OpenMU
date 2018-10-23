// <copyright file="GuildListRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;

    /// <summary>
    /// Handler for guild list request packets.
    /// </summary>
    internal class GuildListRequestHandler : IPacketHandler
    {
        private readonly GuildListRequestAction requestAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildListRequestHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildListRequestHandler(IGameServerContext gameContext)
        {
            this.requestAction = new GuildListRequestAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.requestAction.RequestGuildList(player);
        }
    }
}
