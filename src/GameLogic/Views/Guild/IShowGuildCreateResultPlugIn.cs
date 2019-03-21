// <copyright file="IShowGuildCreateResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about the guild creation result.
    /// </summary>
    public interface IShowGuildCreateResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild create result.
        /// </summary>
        /// <param name="errorDetail">The error detail.</param>
        void ShowGuildCreateResult(GuildCreateErrorDetail errorDetail);
    }
}