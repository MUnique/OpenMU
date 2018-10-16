// <copyright file="GuildRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for guild requests.
    /// </summary>
    internal class GuildRequestHandler : IPacketHandler
    {
        private readonly GuildRequestAction requestAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildRequestHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildRequestHandler(IGameServerContext gameContext)
        {
            this.requestAction = new GuildRequestAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var guildMasterId = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            this.requestAction.RequestGuild(player, guildMasterId);
        }
    }
}
