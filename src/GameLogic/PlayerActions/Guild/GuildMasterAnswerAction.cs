// <copyright file="GuildMasterAnswerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Action to answer the dialog of the guild master npc.
/// </summary>
public class GuildMasterAnswerAction
{
    /// <summary>
    /// Type of the answer.
    /// </summary>
    public enum Answer
    {
        /// <summary>
        /// Cancels the guild master npc dialog.
        /// </summary>
        Cancel = 0,

        /// <summary>
        /// The guild master npc dialog should be shown.
        /// </summary>
        ShowDialog = 1,
    }

    /// <summary>
    /// Processes the answer.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="answer">The answer.</param>
    public async ValueTask ProcessAnswerAsync(Player player, Answer answer)
    {
        if (player.PlayerState.CurrentState == PlayerState.EnteredWorld && answer == Answer.ShowDialog)
        {
            await player.InvokeViewPlugInAsync<IShowGuildCreationDialogPlugIn>(p => p.ShowGuildCreationDialogAsync()).ConfigureAwait(false);
        }
        else if (player.OpenedNpc?.Definition.NpcWindow == NpcWindow.GuildMaster && await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            player.OpenedNpc = null;
        }
        else
        {
            // nothing to do.
        }
    }
}