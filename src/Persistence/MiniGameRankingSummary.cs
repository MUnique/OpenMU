// <copyright file="MiniGameRankingSummary.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// The summarized mini game results of a character, e.g. for the leaderboards of the admin panel.
/// </summary>
/// <param name="CharacterId">The identifier of the character.</param>
/// <param name="CharacterName">The name of the character.</param>
/// <param name="CharacterClassId">The identifier of the character class.</param>
/// <param name="TotalScore">The sum of all scores of the character.</param>
/// <param name="BestScore">The best score which the character reached in one game.</param>
/// <param name="GameCount">The number of played games.</param>
public sealed record MiniGameRankingSummary(Guid CharacterId, string CharacterName, Guid? CharacterClassId, long TotalScore, int BestScore, int GameCount);
