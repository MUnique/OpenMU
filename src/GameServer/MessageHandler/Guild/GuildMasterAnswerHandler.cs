// <copyright file="GuildMasterAnswerHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;

    /// <summary>
    /// Handler for guild master answer packets.
    /// </summary>
    internal class GuildMasterAnswerHandler : IPacketHandler
    {
        private readonly GuildMasterAnswerAction answerAction = new GuildMasterAnswerAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            var answer = (GuildMasterAnswerAction.Answer)packet[3];
            this.answerAction.ProcessAnswer(player, answer);
        }
    }
}
