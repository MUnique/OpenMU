// <copyright file="IGuildKickResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about the result of a previously requested kick request.
    /// </summary>
    public interface IGuildKickResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the result of the kick request.
        /// </summary>
        /// <param name="successCode">The success code.</param>
        void GuildKickResult(GuildKickSuccess successCode);
    }
}