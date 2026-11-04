// <copyright file="ViolationPolicy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The policy math of speed hack punishment: how violations score, decay and escalate.
/// Pure functions without player, clock, lock or configuration dependencies, so they can
/// be unit-tested deterministically. <see cref="SpeedHackDetectPlugIn"/> orchestrates
/// player state around them and stays the only place which reads the configuration.
/// </summary>
internal static class ViolationPolicy
{
    /// <summary>
    /// The violation score at which a walk violation escalates to a hard rejection.
    /// Also the default of <see cref="SpeedHackDetectConfiguration.ViolationScoreRejectThreshold"/>.
    /// </summary>
    internal const double DefaultRejectThreshold = 6.0;

    /// <summary>
    /// The half-life in minutes for the violation score decay.
    /// Also the default of <see cref="SpeedHackDetectConfiguration.ViolationScoreHalfLifeMinutes"/>.
    /// </summary>
    internal const double DefaultHalfLifeMinutes = 5.0;

    /// <summary>
    /// Resolves the configured reject threshold, falling back to the default for
    /// existing databases which predate the setting and deserialize it as 0.
    /// </summary>
    /// <param name="configuredThreshold">The configured threshold.</param>
    /// <returns>The threshold to escalate at.</returns>
    internal static double ResolveRejectThreshold(double configuredThreshold)
    {
        if (configuredThreshold > 0)
        {
            return configuredThreshold;
        }

        return DefaultRejectThreshold;
    }

    /// <summary>
    /// Resolves the configured score half-life, falling back to the default for
    /// existing databases which predate the setting and deserialize it as 0.
    /// </summary>
    /// <param name="configuredHalfLifeMinutes">The configured half-life in minutes.</param>
    /// <returns>The half-life to decay with.</returns>
    internal static double ResolveHalfLifeMinutes(double configuredHalfLifeMinutes)
    {
        if (configuredHalfLifeMinutes > 0)
        {
            return configuredHalfLifeMinutes;
        }

        return DefaultHalfLifeMinutes;
    }

    /// <summary>
    /// Decays a violation score over the passed time with the given half-life.
    /// </summary>
    /// <param name="score">The score to decay.</param>
    /// <param name="elapsed">The time passed since the score was last updated.</param>
    /// <param name="halfLifeMinutes">The half-life in minutes.</param>
    /// <returns>The decayed score.</returns>
    internal static double DecayScore(double score, TimeSpan elapsed, double halfLifeMinutes)
    {
        if (elapsed <= TimeSpan.Zero)
        {
            return score;
        }

        return score * Math.Pow(0.5, elapsed.TotalMinutes / halfLifeMinutes);
    }

    /// <summary>
    /// Weighs one violation for the score: every violation counts at least one,
    /// larger overshoots count proportionally more.
    /// </summary>
    /// <param name="overshoot">By how much the action exceeded its budget.</param>
    /// <param name="expected">The budget the action had, in the same unit as <paramref name="overshoot"/>.</param>
    /// <returns>The weight to add to the score.</returns>
    internal static double WeighViolation(double overshoot, double expected)
    {
        return 1.0 + (overshoot / Math.Max(1.0, expected));
    }

    /// <summary>
    /// Decides whether a violation score escalates to a hard rejection.
    /// </summary>
    /// <param name="score">The current violation score.</param>
    /// <param name="rejectThreshold">The threshold to escalate at.</param>
    /// <returns><c>true</c> if the violation is hard; otherwise, soft.</returns>
    internal static bool ShouldEscalate(double score, double rejectThreshold)
    {
        return score >= rejectThreshold;
    }

    /// <summary>
    /// Decides the sanction for a recorded violation from the warning history.
    /// Pure decision; the caller executes it (message, disconnect, ban).
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="warningsInWindow">The warnings in the history window, including the current one.</param>
    /// <param name="canPunish">Whether the player may be punished at all, e.g. not a game master or spectator.</param>
    /// <param name="hasAccount">Whether the player has an account which can be banned.</param>
    /// <returns>The sanction to execute.</returns>
    internal static Sanction DecideSanction(SpeedHackDetectConfiguration config, int warningsInWindow, bool canPunish, bool hasAccount)
    {
        if (!canPunish)
        {
            return default;
        }

        if (warningsInWindow <= config.MaxWarnings)
        {
            return new Sanction(WarnPlayer: true, DisconnectPlayer: false, BanAccount: false);
        }

        bool banAccount = config.AutoBan && hasAccount;
        bool disconnectPlayer = config.DisconnectOnViolation;
        return new Sanction(!banAccount && !disconnectPlayer, disconnectPlayer, banAccount);
    }

    /// <summary>
    /// The sanction to execute for a recorded violation.
    /// </summary>
    /// <param name="WarnPlayer">Whether to show the player a warning message.</param>
    /// <param name="DisconnectPlayer">Whether to disconnect the player.</param>
    /// <param name="BanAccount">Whether to ban the account.</param>
    internal readonly record struct Sanction(bool WarnPlayer, bool DisconnectPlayer, bool BanAccount);
}
