// <copyright file="GuildRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
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
            Player guildMaster = player.CurrentMap.GetObject(guildMasterId) as Player;

            if (guildMaster?.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                return; // targeted player not in a guild or not the guild master
            }

            if (guildMaster.LastGuildRequester != null)
            {
                player.PlayerView.ShowMessage("The Guild Master is busy.", MessageType.BlueNormal);
                return;
            }

            guildMaster.LastGuildRequester = player;
            guildMaster.PlayerView.GuildView.ShowGuildJoinRequest(player);
        }
    }
}
