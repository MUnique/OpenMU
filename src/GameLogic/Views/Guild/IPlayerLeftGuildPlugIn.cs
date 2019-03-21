// <copyright file="IPlayerLeftGuildPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about a player which left its guild.
    /// </summary>
    public interface IPlayerLeftGuildPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A Player the left his guild.
        /// </summary>
        /// <param name="player">The player who left his guild.</param>
        void PlayerLeftGuild(Player player);
    }
}
