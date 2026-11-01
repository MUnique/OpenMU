// <copyright file="KanturuMonsterComparer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Compares monster definitions by their number, because the configured definition
/// may be a different instance than the one of the spawned monster.
/// </summary>
internal static class KanturuMonsterComparer
{
    /// <summary>
    /// Determines whether the monster definitions describe the same monster.
    /// </summary>
    /// <param name="first">The first monster definition.</param>
    /// <param name="second">The second monster definition.</param>
    /// <returns><c>true</c> if both definitions describe the same monster; otherwise, <c>false</c>.</returns>
    public static bool IsSameMonster(MonsterDefinition? first, MonsterDefinition? second)
    {
        return first is not null && second is not null && first.Number == second.Number;
    }

    /// <summary>
    /// Determines whether the killed monster counts towards the phase kill target.
    /// </summary>
    /// <param name="killed">The definition of the killed monster.</param>
    /// <param name="phase">The current phase.</param>
    /// <returns><c>true</c> if the kill counts towards the kill target; otherwise, <c>false</c>.</returns>
    public static bool IsCountedMonster(MonsterDefinition? killed, KanturuPhaseDefinition? phase)
    {
        return phase is not null
            && killed is not null
            && phase.CountedMonsters.Any(counted => IsSameMonster(counted, killed));
    }
}
