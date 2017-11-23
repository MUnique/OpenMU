// <copyright file="GuildInfoRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for guild info request packets.
    /// </summary>
    internal class GuildInfoRequestHandler : IPacketHandler
    {
        private readonly GuildInfoRequestAction requestAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildInfoRequestHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildInfoRequestHandler(IGameServerContext gameContext)
        {
            this.requestAction = new GuildInfoRequestAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            var shortGuildId = NumberConversionExtensions.MakeWord(packet[5], packet[4]);
            this.requestAction.RequestGuildInfo(player, shortGuildId);
        }
    }
}
