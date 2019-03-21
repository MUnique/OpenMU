// <copyright file="IShowGuildInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about the guild information of a previously requested guild.
    /// </summary>
    public interface IShowGuildInfoPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild information of a previously requested guild.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        void ShowGuildInfo(uint guildId);
    }
}