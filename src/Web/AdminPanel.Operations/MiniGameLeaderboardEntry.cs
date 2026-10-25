// <copyright file="MiniGameLeaderboardEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// An entry of a mini game leaderboard.
/// </summary>
/// <param name="Rank">The rank, starting at 1.</param>
/// <param name="CharacterName">The name of the character.</param>
/// <param name="ClassName">The name of the character class.</param>
/// <param name="TotalScore">The sum of all scores of the character.</param>
/// <param name="BestScore">The best score which the character reached in one game.</param>
/// <param name="GameCount">The number of played games.</param>
public sealed record MiniGameLeaderboardEntry(int Rank, string CharacterName, string ClassName, long TotalScore, int BestScore, int GameCount);
