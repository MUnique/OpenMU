// <copyright file="WalkSpeedCheck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Records server-executed walk steps and evaluates their pace over a sliding window.
/// </summary>
internal sealed class WalkSpeedCheck
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkSpeedCheck"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    public WalkSpeedCheck(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Records a step which the server actually executed.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="state">The player state.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="step">The executed step.</param>
    /// <param name="stepDelay">The delay which was used for pacing the step.</param>
    public void RecordStep(
        Player player,
        SpeedHackState state,
        SpeedHackDetectConfiguration config,
        WalkingStep step,
        TimeSpan stepDelay)
    {
        if (player.IsAtSafezone())
        {
            this.Reset(state);
            return;
        }

        double distance = step.From.EuclideanDistanceTo(step.To);
        if (distance <= 0)
        {
            return;
        }

        double costMs = WalkSpeedValidator.GetExpectedTimeMs(distance, stepDelay);
        double windowMs = WalkSpeedValidator.ResolveWindowSeconds(config.WalkSpeedWindowSeconds) * 1000.0;
        long now = this._timeProvider.GetTimestamp();
        double nowMs = this._timeProvider.GetElapsedTime(0, now).TotalMilliseconds;

        lock (state.Lock)
        {
            state.WalkWindow.Add(nowMs, costMs, windowMs);
        }
    }

    /// <summary>
    /// Resets the recorded walk steps.
    /// </summary>
    /// <param name="state">The player state.</param>
    public void Reset(SpeedHackState state)
    {
        lock (state.Lock)
        {
            state.ResetWalk();
        }
    }

    /// <summary>
    /// Evaluates the walk step rate of the sliding window and updates the player's walk state.
    /// The caller must hold <see cref="SpeedHackState.Lock"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="state">The state of the player.</param>
    /// <param name="now">The current monotonic timestamp.</param>
    /// <returns>
    /// The violation severity, and whether the walk rate violated the speed budget. The severity is
    /// <see cref="SpeedHackViolationSeverity.None"/> when the walk rate is clean or only observed.
    /// </returns>
    /// <remarks>
    /// Only steps which the server actually executed are recorded, with the time at which they were executed.
    /// A walk request therefore cannot inflate the measured distance by running ahead of the character, and
    /// the time between two packets is irrelevant. The cost of the executed steps is summed up over a long
    /// window and compared to the length of that window. Time which passes without walking earns nothing
    /// beyond the window, and a speed hack, which walks more than the window allows, exceeds its budget
    /// however its packets are spread over time.
    /// </remarks>
    public (SpeedHackViolationSeverity Severity, bool IsViolation) Evaluate(
        Player player,
        SpeedHackDetectConfiguration config,
        SpeedHackState state,
        long now)
    {
        double halfLifeMinutes = ViolationPolicy.ResolveHalfLifeMinutes(config.ViolationScoreHalfLifeMinutes);
        var scoreElapsed = state.LastScoreUpdateTimestamp is { } lastScoreUpdateTimestamp
            ? this._timeProvider.GetElapsedTime(lastScoreUpdateTimestamp, now)
            : TimeSpan.Zero;
        state.ViolationScore = ViolationPolicy.DecayScore(state.ViolationScore, scoreElapsed, halfLifeMinutes);
        state.LastScoreUpdateTimestamp = now;

        double stepDelayMs = player.StepDelay.TotalMilliseconds;
        double windowMs = WalkSpeedValidator.ResolveWindowSeconds(config.WalkSpeedWindowSeconds) * 1000.0;
        double toleranceMs = GetToleranceMs(config, stepDelayMs);

        // The timestamps of the time provider are monotonic, so they are immune to wall-clock adjustments.
        double nowMs = this._timeProvider.GetElapsedTime(0, now).TotalMilliseconds;
        double windowCostMs = state.WalkWindow.GetCost(nowMs, windowMs);

        var verdict = WalkSpeedValidator.Evaluate(windowCostMs, windowMs, toleranceMs);
        if (!verdict.IsViolation)
        {
            return (SpeedHackViolationSeverity.None, false);
        }

        // The window starts again after a violation. Otherwise, every following walk would still see the
        // same excess and be flagged again until it leaves the window, although it's the same incident.
        // A speed hack which keeps going exceeds the budget of the new window again, and is flagged again.
        state.WalkWindow.Clear();

        if (!config.EnforceWalkSpeed)
        {
            // The client is authoritative for walking, so the result is diagnostic only
            // and never blocks the walk or contributes to punishments.
            player.Logger.LogInformation(
                "Walk pace observation for player {0}: server-executed walk steps take at least {1:F0}ms within the last {2:F0}ms, exceeding the budget of {3:F0}ms (including a tolerance of {4:F0}ms), overshoot {5:F0}ms. Walk speed enforcement is disabled; movement is accepted.",
                player.Name,
                verdict.ExpectedTimeMs,
                windowMs,
                verdict.BudgetMs,
                toleranceMs,
                verdict.OvershootMs);
            return (SpeedHackViolationSeverity.None, true);
        }

        return (this.PenalizeViolation(player, config, state, windowMs, toleranceMs, verdict), true);
    }

    /// <summary>
    /// Scores a large difference between the client-reported walk start and the server position.
    /// The caller must hold <see cref="SpeedHackState.Lock"/>.
    /// PlayerMovement bounds the offset independently; the severity contributes to the warning and
    /// punishment history.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="state">The state of the player.</param>
    /// <param name="offset">The distance between client and server position.</param>
    /// <returns>The violation severity, or <see cref="SpeedHackViolationSeverity.None"/> when only observed.</returns>
    public SpeedHackViolationSeverity PenalizePositionMismatch(
        Player player,
        SpeedHackDetectConfiguration config,
        SpeedHackState state,
        double offset)
    {
        var maxOffset = Math.Max(0, config.MaxAllowedWalkStartOffset);
        if (offset <= maxOffset)
        {
            return SpeedHackViolationSeverity.None;
        }

        state.ViolationScore += ViolationPolicy.WeighViolation(offset - maxOffset, maxOffset);
        player.Logger.LogWarning(
            "Speedhack detected on walk for player {0}: client position is {1:F1} tiles away from the server position, exceeding the allowed offset of {2}. Score: {3:F2}.",
            player.Name,
            offset,
            maxOffset,
            state.ViolationScore);
        if (config.ObserveOnly)
        {
            return SpeedHackViolationSeverity.None;
        }

        return GetSeverity(config, state);
    }

    private static double GetToleranceMs(SpeedHackDetectConfiguration config, double stepDelayMs)
    {
        return config.WalkSpeedToleranceMs * (stepDelayMs / WalkSpeedValidator.NormalStepDelayMs);
    }

    private static SpeedHackViolationSeverity GetSeverity(SpeedHackDetectConfiguration config, SpeedHackState state)
    {
        double rejectThreshold = ViolationPolicy.ResolveRejectThreshold(config.ViolationScoreRejectThreshold);
        return ViolationPolicy.ShouldEscalate(state.ViolationScore, rejectThreshold)
            ? SpeedHackViolationSeverity.Reject
            : SpeedHackViolationSeverity.Soft;
    }

    /// <summary>
    /// Scores a detected walk violation, logs it and decides its severity.
    /// The caller must hold <see cref="SpeedHackState.Lock"/>.
    /// Recording the warning and punishing stay in <see cref="SpeedHackViolationRecorder"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="state">The state of the player.</param>
    /// <param name="windowMs">The length of the window in milliseconds.</param>
    /// <param name="toleranceMs">The tolerance which was added to the window.</param>
    /// <param name="verdict">The time-budget verdict.</param>
    /// <returns>The violation severity, or <see cref="SpeedHackViolationSeverity.None"/> when only observed.</returns>
    private SpeedHackViolationSeverity PenalizeViolation(
        Player player,
        SpeedHackDetectConfiguration config,
        SpeedHackState state,
        double windowMs,
        double toleranceMs,
        WalkSpeedValidator.Verdict verdict)
    {
        // Each violation adds to the score proportionally to its overshoot, so isolated
        // blips stay soft while sustained cheating escalates to a hard rejection.
        state.ViolationScore += ViolationPolicy.WeighViolation(verdict.OvershootMs, verdict.BudgetMs);
        player.Logger.LogWarning(
            "Speedhack detected on walk for player {0}: server-executed walk steps take at least {1:F0}ms within the last {2:F0}ms, exceeding the budget of {3:F0}ms (including a tolerance of {4:F0}ms). Score: {5:F2}, overshoot: {6:F0}ms.",
            player.Name,
            verdict.ExpectedTimeMs,
            windowMs,
            verdict.BudgetMs,
            toleranceMs,
            state.ViolationScore,
            verdict.OvershootMs);
        if (config.ObserveOnly)
        {
            return SpeedHackViolationSeverity.None;
        }

        return GetSeverity(config, state);
    }
}
