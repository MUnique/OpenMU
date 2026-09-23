// <copyright file="KanturuNightmarePhaseSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Selects the active health phase of the Nightmare boss fight
/// from its current health percentage.
/// </summary>
internal static class KanturuNightmarePhaseSelector
{
    /// <summary>
    /// Gets the target phase index for the given health percentage.
    /// </summary>
    /// <param name="hpPhases">The configured health phases, in order.</param>
    /// <param name="healthPercentage">The current health percentage (0-100).</param>
    /// <returns>The index of the phase which should be active (0 = none yet).</returns>
    public static int GetTargetPhaseIndex(IList<KanturuNightmareHpPhase> hpPhases, float healthPercentage)
    {
        var targetPhaseIndex = 0;
        for (var i = 0; i < hpPhases.Count; i++)
        {
            if (healthPercentage < hpPhases[i].HealthPercentage)
            {
                targetPhaseIndex = i + 1;
            }
        }

        return targetPhaseIndex;
    }

    /// <summary>
    /// Determines whether the monitor should advance to the target phase.
    /// </summary>
    /// <param name="targetPhaseIndex">The index of the phase matching the current health.</param>
    /// <param name="currentPhaseIndex">The index of the currently active phase.</param>
    /// <returns><c>true</c> if the monitor should advance; otherwise, <c>false</c>.</returns>
    public static bool ShouldAdvance(int targetPhaseIndex, int currentPhaseIndex)
    {
        return targetPhaseIndex > currentPhaseIndex;
    }
}
