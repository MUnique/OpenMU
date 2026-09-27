// <copyright file="WeeklyQuestOverview.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The overview of the quests of a player.
/// </summary>
/// <param name="Entries">The entries of the available quests.</param>
/// <param name="NextResetUtc">The point in time (UTC) when the next week starts.</param>
/// <param name="NextDailyResetUtc">The point in time (UTC) when the next day starts.</param>
public record WeeklyQuestOverview(IReadOnlyList<WeeklyQuestOverviewEntry> Entries, DateTime NextResetUtc, DateTime NextDailyResetUtc);
