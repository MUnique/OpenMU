// <copyright file="MiniGameLeaderboard.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The leaderboard of a mini game type, shared by the admin panel and agents.
/// </summary>
/// <param name="Summary">The scope of the leaderboard.</param>
/// <param name="AvailableTypes">The mini game types which save their ranking statistics, so that they have a leaderboard.</param>
/// <param name="MiniGameType">The type of the mini game of this leaderboard; <c>null</c>, if no mini game has a leaderboard.</param>
/// <param name="Days">The number of past days of which finished games are counted; <c>null</c> for all time.</param>
/// <param name="Entries">The best characters, ordered by their total score.</param>
public sealed record MiniGameLeaderboard(string Summary, IReadOnlyList<MiniGameType> AvailableTypes, MiniGameType? MiniGameType, int? Days, IReadOnlyList<MiniGameLeaderboardEntry> Entries);
