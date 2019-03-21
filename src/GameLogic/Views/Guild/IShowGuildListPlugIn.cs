// <copyright file="IShowGuildListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Interface of a view whose implementation informs about the previously requested list of guild players.
    /// </summary>
    public interface IShowGuildListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild list.
        /// </summary>
        /// <param name="players">The players of the guild.</param>
        void ShowGuildList(IEnumerable<GuildListEntry> players);
    }
}