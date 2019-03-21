// <copyright file="IShowGuildMasterDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Interface of a view whose implementation informs about to show the guild master dialog.
    /// </summary>
    public interface IShowGuildMasterDialogPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the guild master dialog which is shown after talking with the guild master npc, if the player is allowed to create a guild.
        /// </summary>
        void ShowGuildMasterDialog();
    }
}