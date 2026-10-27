// <copyright file="SpeedHackDetectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A feature plugin that provides configuration and acts as a trigger control for speedhack anti-cheat checks.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectPlugIn_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
[Guid("A95A8D2F-A0C3-442E-995C-005B5C1B42D2")]
public class SpeedHackDetectPlugIn : IFeaturePlugIn, ISupportCustomConfiguration<SpeedHackDetectConfiguration>, ISupportDefaultCustomConfiguration, ISpeedHackCheatCheckPlugIn, IAttackSpeedCheckPlugIn, IPingCheckPlugIn, IWalkRequestValidationPlugIn, IWalkStepPlugIn, IMovementStateResetPlugIn
{
    private readonly ConditionalWeakTable<Player, SpeedHackState> _playerStates = new();
    private readonly TimeProvider _timeProvider;
    private readonly SpeedHackViolationRecorder _violationRecorder;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedHackDetectPlugIn"/> class.
    /// </summary>
    public SpeedHackDetectPlugIn()
        : this(TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedHackDetectPlugIn"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    internal SpeedHackDetectPlugIn(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
        this._violationRecorder = new SpeedHackViolationRecorder(timeProvider);
    }

    /// <inheritdoc/>
    public SpeedHackDetectConfiguration? Configuration { get; set; }

    /// <inheritdoc/>
    public object CreateDefaultConfig() => new SpeedHackDetectConfiguration();

    /// <inheritdoc/>
    public async ValueTask ValidateWalkRequestAsync(Player player, Memory<WalkingStep> steps, WalkRequestValidationArgs eventArgs)
    {
        var speedHackArgs = new SpeedHackCheckEventArgs();
        await this.WalkCheatCheckAsync(player, steps, speedHackArgs).ConfigureAwait(false);
        eventArgs.IsWalkAllowed = !speedHackArgs.IsCheatDetected;
    }

    /// <inheritdoc/>
    public ValueTask ResetMovementStateAsync(Player player)
    {
        var state = this.GetState(player);
        lock (state.Lock)
        {
            state.ResetWalk();
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public void WalkStepPerformed(Player player, WalkingStep step, TimeSpan stepDelay)
    {
        if (IsServerControlled(player))
        {
            return;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return;
        }

        if (player.IsAtSafezone())
        {
            var safezoneState = this.GetState(player);
            lock (safezoneState.Lock)
            {
                safezoneState.ResetWalk();
            }

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

        var state = this.GetState(player);
        lock (state.Lock)
        {
            state.WalkWindow.Add(nowMs, costMs, windowMs);
        }
    }

    /// <summary>
    /// Checks the walk speed and records a violation, if the walk has to be rejected.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="steps">The steps requested by the player.</param>
    /// <param name="eventArgs">The event args to mark a violation.</param>
    public async ValueTask WalkCheatCheckAsync(Player player, Memory<WalkingStep> steps, SpeedHackCheckEventArgs eventArgs)
    {
        if (steps.IsEmpty || IsServerControlled(player))
        {
            return;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return;
        }

        var state = this.GetState(player);
        var violationSeverity = SpeedHackViolationSeverity.None;
        var positionMismatchSeverity = SpeedHackViolationSeverity.None;
        var startPoint = steps.Span[0].From;
        lock (state.Lock)
        {
            if (player.IsAtSafezone())
            {
                state.ResetWalk();
            }
            else
            {
                var walkEvaluation = this.EvaluateWalkLocked(player, config, state, this._timeProvider.GetTimestamp());
                violationSeverity = walkEvaluation.Severity;
                if (!walkEvaluation.IsViolation)
                {
                    positionMismatchSeverity = this.PenalizePositionMismatchLocked(player, config, state, startPoint.EuclideanDistanceTo(player.Position));
                }
            }
        }

        if (violationSeverity is not SpeedHackViolationSeverity.None)
        {
            eventArgs.Severity = violationSeverity;
            await this._violationRecorder.RecordAsync(player, state, config, violationSeverity).ConfigureAwait(false);
        }
        else if (positionMismatchSeverity is not SpeedHackViolationSeverity.None)
        {
            // PlayerMovement validates the offset independently and resynchronizes the client
            // when it's too large. The mismatch still contributes to warnings and punishments.
            await this._violationRecorder.RecordAsync(player, state, config, positionMismatchSeverity).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask AttackCheatCheckAsync(Player player, Skill? skill, SpeedHackCheckEventArgs eventArgs)
    {
        if (player.Attributes is null || IsServerControlled(player))
        {
            return;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return;
        }

        var attackLimit = AttackSpeedModel.Resolve(config, player, skill);
        var now = this._timeProvider.GetTimestamp();

        var minIntervalMs = attackLimit.IntervalMs;
        var state = this.GetState(player);
        bool shouldRecordViolation = false;
        double elapsedMs = 0;
        double remainingTokens = 0;

        lock (state.Lock)
        {
            if (state.LastAttackTokenUpdateTimestamp is null)
            {
                state.LastAttackTokenUpdateTimestamp = now;
                state.AttackTokens = config.MaxAttackTokens - 1.0;
                return;
            }

            elapsedMs = Math.Max(0, this._timeProvider.GetElapsedTime(state.LastAttackTokenUpdateTimestamp.Value, now).TotalMilliseconds);
            state.LastAttackTokenUpdateTimestamp = now;

            var regen = elapsedMs / minIntervalMs;
            state.AttackTokens = Math.Min(config.MaxAttackTokens, state.AttackTokens + regen);

            if (state.AttackTokens >= 1.0)
            {
                state.AttackTokens -= 1.0;
                return;
            }

            remainingTokens = state.AttackTokens;
            shouldRecordViolation = !config.ObserveOnly;
        }

        player.Logger.LogDebug(
            "Speedhack detected on attack for player {0}: {1:F0}ms since the last attack, expected at least {2:F0}ms ({3} {4:F0}, skill {5}), {6:F2} attack tokens left.{7}",
            player.Name,
            elapsedMs,
            minIntervalMs,
            attackLimit.SpeedSource,
            attackLimit.Speed,
            skill?.Number.ToString() ?? "melee",
            remainingTokens,
            config.ObserveOnly ? " Observe-only, the attack is not blocked." : string.Empty);

        if (shouldRecordViolation)
        {
            // The attack path has no resync response and no score model, so its violations
            // stay soft and debounced, exactly as before: only sustained violations spread
            // over time can accumulate toward a punishment, never a single burst.
            eventArgs.Severity = SpeedHackViolationSeverity.Soft;
            await this._violationRecorder.RecordAsync(player, state, config, eventArgs.Severity).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public ValueTask AttackCheatCheckAsync(Player player, SpeedHackCheckEventArgs eventArgs) =>
        this.AttackCheatCheckAsync(player, null, eventArgs);

    /// <inheritdoc/>
    public ValueTask PingCheatCheckAsync(Player player, uint tickCount, ushort attackSpeed)
    {
        if (IsServerControlled(player))
        {
            return ValueTask.CompletedTask;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return ValueTask.CompletedTask;
        }

        var state = this.GetState(player);
        var now = this._timeProvider.GetTimestamp();

        double serverElapsedMs = 0;
        PingClockValidator.Result clockResult;
        lock (state.Lock)
        {
            if (state.LastPingTimestamp is not { } lastPingTimestamp
                || state.LastClientTickCount is not { } lastClientTickCount)
            {
                state.LastPingTimestamp = now;
                state.LastClientTickCount = tickCount;
                return ValueTask.CompletedTask;
            }

            serverElapsedMs = this._timeProvider.GetElapsedTime(lastPingTimestamp, now).TotalMilliseconds;
            clockResult = PingClockValidator.Evaluate(lastClientTickCount, tickCount, serverElapsedMs, config.PingClockToleranceMs);
            state.LastPingTimestamp = now;
            state.LastClientTickCount = tickCount;
        }

        if (clockResult.IsClockReset)
        {
            player.Logger.LogDebug(
                "Client tick count reset for player {0}: client advanced {1:F0}ms, server advanced {2:F0}ms. Resetting the ping baseline.",
                player.Name,
                clockResult.ClientElapsedMs,
                serverElapsedMs);
        }
        else if (clockResult.IsClientAhead)
        {
            player.Logger.LogDebug(
                "Client clock inconsistency for player {0}: client advanced {1:F0}ms while the server advanced {2:F0}ms (tolerance {3}ms). Observe-only, no action taken.",
                player.Name,
                clockResult.ClientElapsedMs,
                serverElapsedMs,
                config.PingClockToleranceMs);
        }

        if (player.Attributes is { } attributes)
        {
            double serverAttackSpeed = attributes[Stats.AttackSpeed];
            double difference = Math.Abs(attackSpeed - serverAttackSpeed);
            if (difference > config.PingAttackSpeedTolerance)
            {
                player.Logger.LogDebug(
                    "Client attack speed mismatch for player {0}: client reported {1}, server calculated {2:F0} (tolerance {3}). Observe-only, no action taken.",
                    player.Name,
                    attackSpeed,
                    serverAttackSpeed,
                    config.PingAttackSpeedTolerance);
            }
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Gets the warning count for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The warning count.</returns>
    public int GetWarningCount(Player player) => this._violationRecorder.GetWarningCount(this.GetState(player));

    /// <summary>
    /// Clears the alert debounce for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    public void ResetAlertDebounce(Player player) => this._violationRecorder.ResetDebounce(this.GetState(player));

    /// <summary>
    /// Gets the current violation score for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The current violation score.</returns>
    internal double GetViolationScore(Player player)
    {
        var state = this.GetState(player);
        lock (state.Lock)
        {
            return state.ViolationScore;
        }
    }

    /// <summary>
    /// Determines whether the player's actions originate on the server instead of a game client.
    /// These checks validate what a client claims about its own timing, so there is nothing to
    /// validate for an offline player: the server itself paces its walks and attacks. Its MU Helper
    /// tick performs the same work cycle as the original client helper (recover, then attack), which
    /// draws two attack tokens from one 500 ms tick and eventually empties the bucket - so leaving
    /// these players in would only ever produce false positives, and with the default configuration
    /// those are answered with a persisted account ban.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c> if the player is controlled by the server; otherwise, <c>false</c>.</returns>
    private static bool IsServerControlled(Player player) => player is Offline.OfflinePlayer;

    /// <summary>
    /// Gets the tolerance of the walk speed check, which defines how much the walked distance
    /// may exceed the time which actually passed within the window.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="stepDelayMs">The current step delay of the player.</param>
    /// <returns>The tolerance in milliseconds.</returns>
    private static double GetWalkToleranceMs(SpeedHackDetectConfiguration config, double stepDelayMs)
    {
        return config.WalkSpeedToleranceMs * (stepDelayMs / WalkSpeedValidator.NormalStepDelayMs);
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
    private (SpeedHackViolationSeverity Severity, bool IsViolation) EvaluateWalkLocked(Player player, SpeedHackDetectConfiguration config, SpeedHackState state, long now)
    {
        double halfLifeMinutes = ViolationPolicy.ResolveHalfLifeMinutes(config.ViolationScoreHalfLifeMinutes);
        var scoreElapsed = state.LastScoreUpdateTimestamp is { } lastScoreUpdateTimestamp
            ? this._timeProvider.GetElapsedTime(lastScoreUpdateTimestamp, now)
            : TimeSpan.Zero;
        state.ViolationScore = ViolationPolicy.DecayScore(state.ViolationScore, scoreElapsed, halfLifeMinutes);
        state.LastScoreUpdateTimestamp = now;

        double stepDelayMs = player.StepDelay.TotalMilliseconds;
        double windowMs = WalkSpeedValidator.ResolveWindowSeconds(config.WalkSpeedWindowSeconds) * 1000.0;
        double toleranceMs = GetWalkToleranceMs(config, stepDelayMs);

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

        var violation = new WalkViolation(windowMs, toleranceMs, verdict);
        if (!config.EnforceWalkSpeed)
        {
            // The client is authoritative for walking, so the result is diagnostic only
            // and never blocks the walk or contributes to punishments.
            player.Logger.LogInformation(
                "Walk pace observation for player {0}: server-executed walk steps take at least {1:F0}ms within the last {2:F0}ms, exceeding the budget of {3:F0}ms (including a tolerance of {4:F0}ms), overshoot {5:F0}ms. Walk speed enforcement is disabled; movement is accepted.",
                player.Name,
                violation.Verdict.ExpectedTimeMs,
                violation.WindowMs,
                violation.Verdict.BudgetMs,
                violation.ToleranceMs,
                violation.Verdict.OvershootMs);
            return (SpeedHackViolationSeverity.None, true);
        }

        return (this.PenalizeWalkViolationLocked(player, config, state, violation), true);
    }

    /// <summary>
    /// Scores a detected walk violation, logs it and decides its severity.
    /// The caller must hold <see cref="SpeedHackState.Lock"/>.
    /// Recording the warning and punishing stay in <see cref="SpeedHackViolationRecorder"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="state">The state of the player.</param>
    /// <param name="violation">The detected violation.</param>
    /// <returns>The violation severity, or <see cref="SpeedHackViolationSeverity.None"/> when only observed.</returns>
    private SpeedHackViolationSeverity PenalizeWalkViolationLocked(Player player, SpeedHackDetectConfiguration config, SpeedHackState state, WalkViolation violation)
    {
        // Each violation adds to the score proportionally to its overshoot, so isolated
        // blips stay soft while sustained cheating escalates to a hard rejection.
        state.ViolationScore += ViolationPolicy.WeighViolation(violation.Verdict.OvershootMs, violation.Verdict.BudgetMs);
        player.Logger.LogWarning(
            "Speedhack detected on walk for player {0}: server-executed walk steps take at least {1:F0}ms within the last {2:F0}ms, exceeding the budget of {3:F0}ms (including a tolerance of {4:F0}ms). Score: {5:F2}, overshoot: {6:F0}ms.",
            player.Name,
            violation.Verdict.ExpectedTimeMs,
            violation.WindowMs,
            violation.Verdict.BudgetMs,
            violation.ToleranceMs,
            state.ViolationScore,
            violation.Verdict.OvershootMs);
        if (config.ObserveOnly)
        {
            return SpeedHackViolationSeverity.None;
        }

        double rejectThreshold = ViolationPolicy.ResolveRejectThreshold(config.ViolationScoreRejectThreshold);
        return ViolationPolicy.ShouldEscalate(state.ViolationScore, rejectThreshold)
            ? SpeedHackViolationSeverity.Reject
            : SpeedHackViolationSeverity.Soft;
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
    private SpeedHackViolationSeverity PenalizePositionMismatchLocked(Player player, SpeedHackDetectConfiguration config, SpeedHackState state, double offset)
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

        double rejectThreshold = ViolationPolicy.ResolveRejectThreshold(config.ViolationScoreRejectThreshold);
        return ViolationPolicy.ShouldEscalate(state.ViolationScore, rejectThreshold)
            ? SpeedHackViolationSeverity.Reject
            : SpeedHackViolationSeverity.Soft;
    }

    private SpeedHackState GetState(Player player)
    {
        return this._playerStates.GetValue(player, p => new SpeedHackState(this.Configuration?.MaxAttackTokens ?? 5.0));
    }

    /// <summary>
    /// A detected walk violation with everything needed to score and log it.
    /// </summary>
    /// <param name="WindowMs">The length of the window in milliseconds.</param>
    /// <param name="ToleranceMs">The tolerance which was added to the window.</param>
    /// <param name="Verdict">The time-budget verdict.</param>
    private readonly record struct WalkViolation(double WindowMs, double ToleranceMs, WalkSpeedValidator.Verdict Verdict);
}
