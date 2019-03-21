// <copyright file="IShowGuildJoinRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about a guild join request.
    /// </summary>
    public interface IShowGuildJoinRequestPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild join request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowGuildJoinRequest(Player requester);
    }
}