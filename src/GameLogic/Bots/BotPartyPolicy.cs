// <copyright file="BotPartyPolicy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Shared tuning for bot party formation.
/// </summary>
internal static class BotPartyPolicy
{
    internal const int MinPartySize = 2;
    internal const int MaxPartySize = 5;
    internal const int MaxLevelGap = 12;
    internal const int PartiedSharePercent = 80;

    /// <summary>
    /// Mean party size used for population planning.
    /// </summary>
    internal static double AveragePartySize => (MinPartySize + MaxPartySize) / 2.0;

    /// <summary>
    /// Estimates the party count for a population.
    /// One buffer per party, so this is also the buffer quota.
    /// </summary>
    /// <param name="botCount">The bot count.</param>
    internal static int EstimatePartyCount(int botCount)
    {
        return (int)Math.Ceiling(botCount * PartiedSharePercent / 100.0 / AveragePartySize);
    }
}
