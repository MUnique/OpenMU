// <copyright file="MuHelperZenCostCalculator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuHelper;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Calculates the Zen cost for the MU Helper and offline leveling based on the
/// player's total level and the elapsed stage derived from the server configuration.
/// </summary>
public static class MuHelperZenCostCalculator
{
    /// <summary>
    /// Calculates the Zen amount to charge for the current pay interval.
    /// </summary>
    /// <param name="player">The player being charged.</param>
    /// <param name="configuration">The MU helper server configuration.</param>
    /// <param name="startTimestamp">The timestamp when the session started, used to determine the current cost stage.</param>
    /// <returns>The Zen amount to deduct; 0 if the configuration has no cost entries.</returns>
    public static int Calculate(Player player, MuHelperConfiguration configuration, DateTime startTimestamp)
    {
        if (configuration.CostPerStage.Count == 0 || configuration.StageInterval <= TimeSpan.Zero)
        {
            return 0;
        }

        var elapsed = DateTime.UtcNow - startTimestamp;
        var currentStage = (int)(elapsed / configuration.StageInterval);
        currentStage = Math.Clamp(currentStage, 0, configuration.CostPerStage.Count - 1);

        var costMultiplier = configuration.CostPerStage[currentStage];
        var totalLevel = player.Level + (int)(player.Attributes?[Stats.MasterLevel] ?? 0);

        return costMultiplier * totalLevel;
    }
}