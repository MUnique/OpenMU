// <copyright file="WebsiteMiniGameRanking.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The leaderboard of a mini game type, as a website shows it.
/// </summary>
/// <param name="MiniGameType">The invariant name of the mini game type, e.g. <c>BloodCastle</c>; <c>null</c>, if no mini game has a leaderboard.</param>
/// <param name="AvailableTypes">The invariant names of the mini game types which have a leaderboard.</param>
/// <param name="Days">The number of past days of which finished games are counted; <c>null</c> for all time.</param>
/// <param name="Entries">The best characters, ordered by their total score.</param>
public sealed record WebsiteMiniGameRanking(string? MiniGameType, IReadOnlyList<string> AvailableTypes, int? Days, IReadOnlyList<MiniGameLeaderboardEntry> Entries);
