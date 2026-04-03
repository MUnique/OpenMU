// <copyright file="ResetProgressionCalculator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Calculates costs and rewards for the next character reset.
/// </summary>
public static class ResetProgressionCalculator
{
    /// <summary>
    /// Calculates the reset progression for the next reset.
    /// </summary>
    /// <param name="currentResetCount">The current reset count.</param>
    /// <param name="pointsPerResetOverride">The player specific points per reset override (0 means not configured).</param>
    /// <param name="configuration">The reset configuration.</param>
    /// <returns>The calculated progression.</returns>
    public static ResetProgression Calculate(int currentResetCount, int pointsPerResetOverride, ResetConfiguration configuration)
    {
        var nextResetCount = currentResetCount + 1;
        var requiredZen = Math.Max(0, configuration.RequiredMoney);
        if (configuration.MultiplyRequiredMoneyByResetCount)
        {
            requiredZen *= nextResetCount;
        }

        var pointsForReset = GetPointsForReset(configuration, pointsPerResetOverride, nextResetCount);
        var totalPointsAfterReset = GetTotalPointsAfterReset(configuration, pointsPerResetOverride, nextResetCount, pointsForReset);
        var requiredItemAmount = GetRequiredItemAmount(configuration, nextResetCount);

        return new ResetProgression(nextResetCount, requiredZen, requiredItemAmount, pointsForReset, totalPointsAfterReset);
    }

    private static int GetPointsForReset(ResetConfiguration configuration, int pointsPerResetOverride, int nextResetCount)
    {
        if (GetMatchingTier(configuration.PointsTiers, nextResetCount, tier => tier.MinimumResetCount) is { } tier)
        {
            return Math.Max(0, tier.PointsGranted);
        }

        var pointsPerReset = pointsPerResetOverride == 0 ? configuration.PointsPerReset : pointsPerResetOverride;
        if (configuration.MultiplyPointsByResetCount)
        {
            pointsPerReset *= nextResetCount;
        }

        return Math.Max(0, pointsPerReset);
    }

    private static int GetRequiredItemAmount(ResetConfiguration configuration, int nextResetCount)
    {
        if (configuration.RequiredResetItem is null)
        {
            return 0;
        }

        if (GetMatchingTier(configuration.ItemCostTiers, nextResetCount, tier => tier.MinimumResetCount) is not { } tier)
        {
            return 0;
        }

        return Math.Max(0, tier.RequiredItemAmount);
    }

    private static int GetTotalPointsAfterReset(ResetConfiguration configuration, int pointsPerResetOverride, int nextResetCount, int pointsForReset)
    {
        if (configuration.PointsTiers.Count == 0)
        {
            return pointsForReset;
        }

        long total = 0;
        for (var resetCount = 1; resetCount <= nextResetCount; resetCount++)
        {
            total += GetPointsForReset(configuration, pointsPerResetOverride, resetCount);
            if (total >= int.MaxValue)
            {
                return int.MaxValue;
            }
        }

        return (int)total;
    }

    private static TTier? GetMatchingTier<TTier>(IEnumerable<TTier> tiers, int resetCount, Func<TTier, int> getMinimumResetCount)
        where TTier : class
    {
        return tiers
            .OrderByDescending(getMinimumResetCount)
            .FirstOrDefault(tier => getMinimumResetCount(tier) <= resetCount);
    }
}

/// <summary>
/// A value object which contains costs and rewards for the next reset.
/// </summary>
/// <param name="NextResetCount">The resulting reset count after a successful reset.</param>
/// <param name="RequiredZen">The required zen for the reset.</param>
/// <param name="RequiredItemAmount">The required amount of configured reset items.</param>
/// <param name="PointsForReset">The amount of points granted for the reset.</param>
/// <param name="TotalPointsAfterReset">The total amount of points after the reset when replacement mode is active.</param>
public readonly record struct ResetProgression(
    int NextResetCount,
    int RequiredZen,
    int RequiredItemAmount,
    int PointsForReset,
    int TotalPointsAfterReset);
