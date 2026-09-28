// <copyright file="IWeeklyQuestListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// Interface of a view whose client can show the weekly quests and their progress in a window.
/// </summary>
public interface IWeeklyQuestListViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the active weekly quests and the progress of the player. It replaces the quests which the client knew before.
    /// </summary>
    /// <param name="overview">The overview of the weekly quests.</param>
    ValueTask ShowWeeklyQuestsAsync(WeeklyQuestOverview overview);

    /// <summary>
    /// Updates the progress of one quest which the client already knows.
    /// </summary>
    /// <param name="entry">The entry of the quest.</param>
    /// <param name="nextResetUtc">The point in time (UTC) when the next week starts.</param>
    /// <param name="nextDailyResetUtc">The point in time (UTC) when the next day starts.</param>
    ValueTask UpdateWeeklyQuestAsync(WeeklyQuestOverviewEntry entry, DateTime nextResetUtc, DateTime nextDailyResetUtc);
}
