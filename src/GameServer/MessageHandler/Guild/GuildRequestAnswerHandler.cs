// <copyright file="GuildRequestAnswerHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using GameLogic.Interfaces;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;

    /// <summary>
    /// Handler for guild request answer packets.
    /// </summary>
    internal class GuildRequestAnswerHandler : IPacketHandler
    {
        private readonly GuildRequestAnswerAction answerAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildRequestAnswerHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildRequestAnswerHandler(IGameServerContext gameContext)
        {
            this.answerAction = new GuildRequestAnswerAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            bool accept = packet[3] == 1;
            this.answerAction.AnswerRequest(player, accept);
        }
    }
}
