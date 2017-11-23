// <copyright file="GuildRequestAnswerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Entities;
    using Views;

    /// <summary>
    /// Action for a guild master player to answer the guild membership request.
    /// </summary>
    public class GuildRequestAnswerAction
    {
        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildRequestAnswerAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildRequestAnswerAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Answers the request.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="accept">If set to <c>true</c>, the membership has been accepted. Otherwise, not.</param>
        public void AnswerRequest(Player player, bool accept)
        {
            var lastGuildRequester = player.LastGuildRequester;
            if (lastGuildRequester == null)
            {
                return;
            }

            if (lastGuildRequester.SelectedCharacter.GuildMemberInfo != null)
            {
                lastGuildRequester.PlayerView.GuildView.GuildJoinResponse(GuildRequestAnswerResult.AlreadyHaveGuild);
                return;
            }

            if (accept)
            {
                var guildMember = this.gameContext.GuildServer.CreateGuildMember(player.SelectedCharacter.GuildMemberInfo.GuildId, player.LastGuildRequester.SelectedCharacter.Id, player.LastGuildRequester.SelectedCharacter.Name, GuildPosition.NormalMember);
                player.LastGuildRequester.SelectedCharacter.GuildMemberInfo = guildMember;

                player.LastGuildRequester.ShortGuildID = this.gameContext.GuildServer.GuildMemberEnterGame(guildMember.GuildId, guildMember.Name, this.gameContext.Id);
                var playerList = new List<Player>(1) { player };
                player.ForEachObservingPlayer(p => p.PlayerView.GuildView.AssignPlayersToGuild(playerList, false), true);
            }

            lastGuildRequester.PlayerView.GuildView.GuildJoinResponse(accept ? GuildRequestAnswerResult.Accepted : GuildRequestAnswerResult.Refused);
            player.LastGuildRequester = null;
        }
    }
}
