// <copyright file="GuildRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using log4net;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to request guild membership from a guild master player.
    /// </summary>
    public class GuildRequestAction
    {
        private static ILog log = LogManager.GetLogger(typeof(GuildRequestAction));

        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildRequestAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildRequestAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Requests the guild from the guild master player with the specified id.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="guildMasterId">The guild master identifier.</param>
        public void RequestGuild(Player player, ushort guildMasterId)
        {
            Player guildmaster = player.CurrentMap.GetObject(guildMasterId) as Player;

            if (guildmaster?.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                return; // targetted player not in a guild or not the guild master
            }

            if (guildmaster.LastGuildRequester != null)
            {
                player.PlayerView.ShowMessage("The Guild Master is busy.", MessageType.BlueNormal);
                return;
            }

            guildmaster.LastGuildRequester = player;
            guildmaster.PlayerView.GuildView.ShowGuildJoinRequest(player);
        }
    }
}
