// <copyright file="IGuildJoinResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about the response of the previously sent guild join request.
    /// </summary>
    public interface IGuildJoinResponsePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild join response from the guild master.
        /// </summary>
        /// <param name="response">The response.</param>
        void ShowGuildJoinResponse(GuildRequestAnswerResult response);
    }
}