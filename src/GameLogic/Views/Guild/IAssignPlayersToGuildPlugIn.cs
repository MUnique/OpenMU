// <copyright file="IAssignPlayersToGuildPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface of a view whose implementation informs about players which are assigned to a guild.
    /// </summary>
    public interface IAssignPlayersToGuildPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Assigns the specified player to its guild.
        /// </summary>
        /// <param name="guildPlayer">The player of a guild.</param>
        /// <param name="appearsNew">If set to <c>true</c>,the player just appeared in the view. Otherwise, the players were already in the view, and freshly assigned to the guild.</param>
        void AssignPlayerToGuild(Player guildPlayer, bool appearsNew);

        /// <summary>
        /// Assigns the players to their guilds.
        /// </summary>
        /// <param name="guildPlayers">The players of a guild.</param>
        /// <param name="appearsNew">If set to <c>true</c>,the player just appeared in the view. Otherwise, the players were already in the view, and freshly assigned to the guild.</param>
        void AssignPlayersToGuild(ICollection<Player> guildPlayers, bool appearsNew);
    }
}