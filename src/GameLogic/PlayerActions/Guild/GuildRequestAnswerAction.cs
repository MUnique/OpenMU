// <copyright file="GuildRequestAnswerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action for a guild master player to answer the guild membership request.
    /// </summary>
    public class GuildRequestAnswerAction
    {
        /// <summary>
        /// Answers the request.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="accept">If set to <c>true</c>, the membership has been accepted. Otherwise, not.</param>
        public void AnswerRequest(Player player, bool accept)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            var guildServer = (player.GameContext as IGameServerContext)?.GuildServer;
            if (guildServer is null)
            {
                return;
            }

            var lastGuildRequester = player.LastGuildRequester;
            if (lastGuildRequester?.SelectedCharacter is null)
            {
                return;
            }

            if (lastGuildRequester.GuildStatus is not null)
            {
                lastGuildRequester.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.AlreadyHaveGuild);
                return;
            }

            if (player.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                player.Logger.LogWarning("Suspicious request for player with name: {0} (player is not a guild master), could be hack attempt.", player.Name);
                lastGuildRequester.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.NotTheGuildMaster);
                return;
            }

            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
                || lastGuildRequester.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                lastGuildRequester.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.GuildMasterOrRequesterIsBusy);
            }

            if (accept)
            {
                var guildStatus = guildServer.CreateGuildMember(player.GuildStatus.GuildId, lastGuildRequester.SelectedCharacter.Id, lastGuildRequester.SelectedCharacter.Name, GuildPosition.NormalMember, ((IGameServerContext)player.GameContext).Id);
                lastGuildRequester.GuildStatus = guildStatus;

                lastGuildRequester.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayerToGuild(lastGuildRequester, false), true);
            }

            lastGuildRequester.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(accept ? GuildRequestAnswerResult.Accepted : GuildRequestAnswerResult.Refused);
            player.LastGuildRequester = null;
        }
    }
}
