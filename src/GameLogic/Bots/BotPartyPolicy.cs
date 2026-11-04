// <copyright file="BotPartyPolicy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Shared tuning for bot party formation.
/// </summary>
internal static class BotPartyPolicy
{
    /// <summary>Smallest party size.</summary>
    internal const int MinPartySize = 2;

    /// <summary>Largest party size.</summary>
    internal const int MaxPartySize = 5;

    /// <summary>Widest effective level gap inside a party.</summary>
    internal const int MaxLevelGap = 12;

    /// <summary>Share of candidates grouped into parties.</summary>
    internal const int PartiedSharePercent = 80;

    /// <summary>
    /// Planning divisor for population sizing. Below the 3.5 target mean:
    /// build key dedupe and level gaps truncate real parties.
    /// </summary>
    internal const double ExpectedPartySize = 3.0;

    /// <summary>
    /// Estimates the party count for a population.
    /// One buffer per party, so this is also the buffer quota.
    /// </summary>
    /// <param name="botCount">The bot count.</param>
    internal static int EstimatePartyCount(int botCount)
    {
        return (int)Math.Ceiling(botCount * PartiedSharePercent / 100.0 / ExpectedPartySize);
    }
}
