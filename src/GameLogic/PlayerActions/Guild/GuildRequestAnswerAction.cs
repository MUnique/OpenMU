﻿// <copyright file="GuildRequestAnswerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;
    using Views;

    /// <summary>
    /// Action for a guild master player to answer the guild membership request.
    /// </summary>
    public class GuildRequestAnswerAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(GuildRequestAnswerAction));

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

            if (lastGuildRequester.GuildStatus != null)
            {
                lastGuildRequester.PlayerView.GuildView.GuildJoinResponse(GuildRequestAnswerResult.AlreadyHaveGuild);
                return;
            }

            if (player.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                Log.WarnFormat("Suspicious request for player with name: {0} (player is not a guild master), could be hack attempt.", player.Name);
                return;
            }

            if (accept)
            {
                var guildStatus = this.gameContext.GuildServer.CreateGuildMember(player.GuildStatus.GuildId, player.LastGuildRequester.SelectedCharacter.Id, lastGuildRequester.SelectedCharacter.Name, GuildPosition.NormalMember, this.gameContext.Id);
                lastGuildRequester.GuildStatus = guildStatus;

                var playerList = new List<Player>(1) { player };
                player.ForEachObservingPlayer(p => p.PlayerView.GuildView.AssignPlayersToGuild(playerList, false), true);
            }

            lastGuildRequester.PlayerView.GuildView.GuildJoinResponse(accept ? GuildRequestAnswerResult.Accepted : GuildRequestAnswerResult.Refused);
            player.LastGuildRequester = null;
        }
    }
}
