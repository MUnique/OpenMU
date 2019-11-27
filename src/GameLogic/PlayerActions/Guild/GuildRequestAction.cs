// <copyright file="GuildRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to request guild membership from a guild master player.
    /// </summary>
    public class GuildRequestAction
    {
        /// <summary>
        /// Requests the guild from the guild master player with the specified id.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="guildMasterId">The guild master identifier.</param>
        public void RequestGuild(Player player, ushort guildMasterId)
        {
            if (player.Level < 6)
            {
                player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.MinimumLevel6);
                return;
            }

            if (player.GuildStatus != null)
            {
                player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.AlreadyHaveGuild);
                return;
            }

            Player guildMaster = player.CurrentMap.GetObject(guildMasterId) as Player;

            if (guildMaster?.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.NotTheGuildMaster);
                return; // targeted player not in a guild or not the guild master
            }

            if (guildMaster.LastGuildRequester != null || player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()?.ShowGuildJoinResponse(GuildRequestAnswerResult.GuildMasterOrRequesterIsBusy);
                return;
            }

            guildMaster.LastGuildRequester = player;
            guildMaster.ViewPlugIns.GetPlugIn<IShowGuildJoinRequestPlugIn>()?.ShowGuildJoinRequest(player);
        }
    }
}
