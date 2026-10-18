// <copyright file="SpeedHackDetectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.Collections.Generic;
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
public class SpeedHackDetectPlugIn : IFeaturePlugIn, ISupportCustomConfiguration<SpeedHackDetectConfiguration>, ISupportDefaultCustomConfiguration, ISpeedHackCheatCheckPlugIn
{
    private readonly ConditionalWeakTable<Player, SpeedHackState> _playerStates = new();
    private readonly TimeProvider _timeProvider;

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
    }

    /// <inheritdoc/>
    public SpeedHackDetectConfiguration? Configuration { get; set; }

    /// <inheritdoc/>
    public object CreateDefaultConfig() => new SpeedHackDetectConfiguration();

    /// <inheritdoc/>
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
        lock (state.Lock)
        {
            if (player.IsAtSafezone())
            {
                state.ResetWalk();
            }
            else
            {
                violationSeverity = this.EvaluateWalkLocked(player, config, state, steps.Span[0].From, this._timeProvider.GetUtcNow().UtcDateTime);
            }
        }

        if (violationSeverity is not SpeedHackViolationSeverity.None)
        {
            eventArgs.Severity = violationSeverity;
            await this.RecordViolationAsync(player, state, config, violationSeverity).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask AttackCheatCheckAsync(Player player, SpeedHackCheckEventArgs eventArgs)
    {
        if (player.Attributes is not { } attributes || IsServerControlled(player))
        {
            return;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return;
        }

        var attackSpeed = attributes[Stats.AttackSpeed];
        var now = this._timeProvider.GetUtcNow().UtcDateTime;

        var minIntervalMs = Math.Max(config.AttackSpeedMinIntervalMs, config.AttackSpeedBaseDelayMs - (attackSpeed * config.AttackSpeedScalingFactor));
        var state = this.GetState(player);
        bool shouldRecordViolation = false;

        lock (state.Lock)
        {
            if (state.LastAttackTokenUpdateTime == DateTime.MinValue)
            {
                state.LastAttackTokenUpdateTime = now;
                state.AttackTokens = config.MaxAttackTokens - 1.0;
                return;
            }

            var elapsedMs = (now - state.LastAttackTokenUpdateTime).TotalMilliseconds;
            state.LastAttackTokenUpdateTime = now;

            var regen = elapsedMs / minIntervalMs;
            state.AttackTokens = Math.Min(config.MaxAttackTokens, state.AttackTokens + regen);

            if (state.AttackTokens >= 1.0)
            {
                state.AttackTokens -= 1.0;
                return;
            }

            shouldRecordViolation = !config.ObserveOnly;
        }

        if (shouldRecordViolation)
        {
            // The attack path has no resync response and no score model, so its violations
            // stay soft and debounced, exactly as before: only sustained violations spread
            // over time can accumulate toward a punishment, never a single burst.
            eventArgs.Severity = SpeedHackViolationSeverity.Resync;
            await this.RecordViolationAsync(player, state, config, eventArgs.Severity).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public ValueTask ResetMovementStateAsync(Player player)
    {
        var state = this.GetState(player);
        lock (state.Lock)
        {
            // The position changed without walking, so the next walk can't be compared to the last one.
            // The credit is kept on purpose, so that a forced position resync doesn't refill it.
            state.LastWalkStartPoint = null;
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Gets the warning count for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The warning count.</returns>
    public int GetWarningCount(Player player)
    {
        var state = this.GetState(player);
        lock (state.Lock)
        {
            return state.AlertTimes.Count;
        }
    }

    /// <summary>
    /// Sets the last alert time for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="time">The time to set.</param>
    public void SetLastAlertTime(Player player, DateTime time)
    {
        var state = this.GetState(player);
        lock (state.Lock)
        {
            state.LastAlertTime = time;
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
    /// Determines whether the player is a game master. Game masters are trusted, so their
    /// violations are still logged and answered, but never with a ban of their account.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c> if the player is a game master; otherwise, <c>false</c>.</returns>
    private static bool IsGameMaster(Player player) =>
        player.Account?.State is AccountState.GameMaster or AccountState.GameMasterInvisible
        || player.SelectedCharacter?.CharacterStatus is CharacterStatus.GameMaster;

    /// <summary>
    /// Gets the maximum walk credit, which defines how far the walk start points
    /// may run ahead of the time which actually passed.
    /// </summary>
    /// <remarks>
    /// The game client reports the tile it's about to enter as its position, as soon as it starts
    /// a step. When the player clicks new targets in quick succession, every click starts a new
    /// step, so every walk request claims one more tile than the character actually walked yet.
    /// This lead is legitimate and bounded: when it exceeds <see cref="SpeedHackDetectConfiguration.MaxAllowedWalkStartOffset"/>,
    /// the walk is resynchronized anyway. So, the credit covers at least that many steps, even when the
    /// configured tolerance is lower - especially with fast mounts (e.g. a horse), which scale the
    /// tolerance down by their shorter step delay.
    /// </remarks>
    /// <param name="config">The configuration.</param>
    /// <param name="stepDelayMs">The current step delay of the player.</param>
    /// <returns>The maximum walk credit in milliseconds.</returns>
    private static double GetMaxWalkCreditMs(SpeedHackDetectConfiguration config, double stepDelayMs)
    {
        double configuredCreditMs = config.WalkSpeedToleranceMs * (stepDelayMs / WalkSpeedValidator.NormalStepDelayMs);
        double walkStartLeadCreditMs = Math.Max(0, config.MaxAllowedWalkStartOffset) * stepDelayMs;
        return Math.Max(configuredCreditMs, walkStartLeadCreditMs);
    }

    /// <summary>
    /// Evaluates one walk against the earned time budget and updates the player's walk state.
    /// The caller must hold <see cref="SpeedHackState.Lock"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="state">The state of the player.</param>
    /// <param name="startPoint">The start of the requested walk.</param>
    /// <param name="now">The current time.</param>
    /// <returns>The violation severity, or <see cref="SpeedHackViolationSeverity.None"/> when the walk is clean or only observed.</returns>
    private SpeedHackViolationSeverity EvaluateWalkLocked(Player player, SpeedHackDetectConfiguration config, SpeedHackState state, Point startPoint, DateTime now)
    {
        double stepDelayMs = player.StepDelay.TotalMilliseconds;
        double maxCreditMs = GetMaxWalkCreditMs(config, stepDelayMs);

        // The walk check works like a token bucket of walking time: passed time adds credit,
        // walked tiles consume it. This way, only the average speed matters. When walk packets
        // are delayed by the network and then arrive in a burst, the time of the delay is
        // credited (up to the tolerance) and covers the tiles which arrive in the burst.
        // A speed hack, however, consumes more than it earns and drains the credit over time.
        var elapsed = now - (state.LastWalkTime ?? now);
        if (state.LastWalkTime is null)
        {
            state.WalkCreditMs = maxCreditMs;
        }

        double halfLifeMinutes = ViolationPolicy.ResolveHalfLifeMinutes(config.ViolationScoreHalfLifeMinutes);
        state.ViolationScore = ViolationPolicy.DecayScore(state.ViolationScore, now - (state.LastScoreUpdateTime ?? now), halfLifeMinutes);
        state.LastScoreUpdateTime = now;

        if (state.LastWalkStartPoint is not { } lastStartPoint)
        {
            state.WalkCreditMs = Math.Min(maxCreditMs, state.WalkCreditMs + elapsed.TotalMilliseconds);
            state.LastWalkTime = now;
            state.LastWalkStartPoint = startPoint;
            return SpeedHackViolationSeverity.None;
        }

        var tiles = Math.Max(
            Math.Abs(startPoint.X - lastStartPoint.X),
            Math.Abs(startPoint.Y - lastStartPoint.Y));

        var verdict = WalkSpeedValidator.Evaluate(state.WalkCreditMs, maxCreditMs, elapsed.TotalMilliseconds, tiles, stepDelayMs);
        state.WalkCreditMs = verdict.NewCreditMs;
        state.LastWalkTime = now;
        state.LastWalkStartPoint = startPoint;
        if (!verdict.IsViolation)
        {
            return SpeedHackViolationSeverity.None;
        }

        return this.PenalizeWalkViolationLocked(player, config, state, new WalkViolation(tiles, elapsed, maxCreditMs, verdict));
    }

    /// <summary>
    /// Scores a detected walk violation, logs it and decides its severity.
    /// The caller must hold <see cref="SpeedHackState.Lock"/>.
    /// Recording the warning and punishing stay in <see cref="RecordViolationAsync"/>.
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
        state.ViolationScore += ViolationPolicy.WeighViolation(violation.Verdict.DeficitMs, violation.Verdict.ExpectedTimeMs);
        player.Logger.LogWarning(
            "Speedhack detected on walk for player {0}: traveled {1} tiles in {2}ms (expected at least {3}ms), exceeding the tolerance of {4}ms. Score: {5:F2}, overshoot: {6:F0}ms.",
            player.Name,
            violation.Tiles,
            violation.Elapsed.TotalMilliseconds,
            violation.Verdict.ExpectedTimeMs,
            violation.MaxCreditMs,
            state.ViolationScore,
            violation.Verdict.DeficitMs);
        if (config.ObserveOnly)
        {
            return SpeedHackViolationSeverity.None;
        }

        double rejectThreshold = ViolationPolicy.ResolveRejectThreshold(config.ViolationScoreRejectThreshold);
        return ViolationPolicy.ShouldEscalate(state.ViolationScore, rejectThreshold)
            ? SpeedHackViolationSeverity.Reject
            : SpeedHackViolationSeverity.Resync;
    }

    private SpeedHackState GetState(Player player)
    {
        return this._playerStates.GetValue(player, p => new SpeedHackState(this.Configuration?.MaxAttackTokens ?? 5.0));
    }

    private async ValueTask RecordViolationAsync(Player player, SpeedHackState state, SpeedHackDetectConfiguration config, SpeedHackViolationSeverity severity)
    {
        var now = this._timeProvider.GetUtcNow().UtcDateTime;
        ViolationPolicy.Sanction sanction;
        lock (state.Lock)
        {
            // Sustained cheating escalates past the debounce: isolated blips within the
            // debounce window are still dropped, but an escalated violation always counts.
            if (severity is not SpeedHackViolationSeverity.Reject && now - state.LastAlertTime < TimeSpan.FromSeconds(config.AlertDebounceSeconds))
            {
                return;
            }

            state.LastAlertTime = now;
            state.AlertTimes.Enqueue(now);

            while (state.AlertTimes.Count > 0 && now - state.AlertTimes.Peek() > TimeSpan.FromHours(config.WarningHistoryHours))
            {
                state.AlertTimes.Dequeue();
            }

            player.Logger.LogWarning("Speedhack warning issued for player {0}. Total warnings in last hour: {1}", player.Name, state.AlertTimes.Count);
            sanction = ViolationPolicy.DecideSanction(config, state.AlertTimes.Count, player.Account is not null && !IsGameMaster(player));
            if (sanction.BanAccount && player.Account is { } account)
            {
                player.Logger.LogError("Player {0} exceeded speedhack warning limit. Banning account {1} and disconnecting.", player.Name, account.LoginName);
                account.State = AccountState.Banned;
            }
        }

        if (sanction.BanAccount)
        {
            await player.SaveProgressAsync().ConfigureAwait(false);
        }

        if (sanction.BanAccount || sanction.DisconnectPlayer)
        {
            await player.DisconnectAsync().ConfigureAwait(false);
        }
        else if (sanction.WarnPlayer)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SpeedHackWarning)).ConfigureAwait(false);
        }
        else
        {
            // Do nothing if no actions are required.
        }
    }

    /// <summary>
    /// A detected walk violation with everything needed to score and log it.
    /// </summary>
    /// <param name="Tiles">The tiles charged for the walk.</param>
    /// <param name="Elapsed">The time passed since the previous walk.</param>
    /// <param name="MaxCreditMs">The maximum credit the walk started with.</param>
    /// <param name="Verdict">The time-budget verdict.</param>
    private readonly record struct WalkViolation(int Tiles, TimeSpan Elapsed, double MaxCreditMs, WalkSpeedValidator.Verdict Verdict);

    private class SpeedHackState
    {
        public SpeedHackState(double maxAttackTokens)
        {
            this.AttackTokens = maxAttackTokens;
        }

        public object Lock { get; } = new();

        public double AttackTokens { get; set; }

        public DateTime LastAttackTokenUpdateTime { get; set; } = DateTime.MinValue;

        public DateTime LastAlertTime { get; set; } = DateTime.MinValue;

        public Queue<DateTime> AlertTimes { get; } = new();

        public DateTime? LastWalkTime { get; set; }

        public Point? LastWalkStartPoint { get; set; }

        public double WalkCreditMs { get; set; }

        public double ViolationScore { get; set; }

        public DateTime? LastScoreUpdateTime { get; set; }

        public void ResetWalk()
        {
            this.LastWalkTime = null;
            this.LastWalkStartPoint = null;
            this.WalkCreditMs = 0;
        }
    }
}
