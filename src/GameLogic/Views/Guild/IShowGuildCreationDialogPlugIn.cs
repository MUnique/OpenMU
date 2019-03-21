// <copyright file="IShowGuildCreationDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about to show the guild creation dialog.
    /// </summary>
    public interface IShowGuildCreationDialogPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild creation dialog.
        /// </summary>
        void ShowGuildCreationDialog();
    }
}