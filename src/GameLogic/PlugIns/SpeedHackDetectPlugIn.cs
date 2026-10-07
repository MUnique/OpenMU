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
    private const int NormalStepDelayMs = 300;
    private readonly ConditionalWeakTable<Player, SpeedHackState> _playerStates = new();

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

        bool isSafezone = player.IsAtSafezone();
        bool shouldRecordViolation = false;
        var startPoint = steps.Span[0].From;
        var state = this.GetState(player);

        lock (state.Lock)
        {
            if (isSafezone)
            {
                state.ResetWalk();
            }
            else
            {
                var now = DateTime.UtcNow;
                double stepDelayMs = player.StepDelay.TotalMilliseconds;
                double scalingFactor = stepDelayMs / NormalStepDelayMs;
                double maxCreditMs = config.WalkSpeedToleranceMs * scalingFactor;

                // The walk check works like a token bucket of walking time: passed time adds credit,
                // walked tiles consume it. This way, only the average speed matters. When walk packets
                // are delayed by the network and then arrive in a burst, the time of the delay is
                // credited (up to the tolerance) and covers the tiles which arrive in the burst.
                // A speed hack, however, consumes more than it earns and drains the credit over time.
                var elapsed = now - (state.LastWalkTime ?? now);
                state.WalkCreditMs = state.LastWalkTime is null
                    ? maxCreditMs
                    : Math.Min(maxCreditMs, state.WalkCreditMs + elapsed.TotalMilliseconds);

                if (state.LastWalkStartPoint is { } lastStartPoint)
                {
                    var tiles = Math.Max(
                        Math.Abs(startPoint.X - lastStartPoint.X),
                        Math.Abs(startPoint.Y - lastStartPoint.Y));

                    const double BaseStepDelayMarginMs = 50.0;
                    const double MinStepDelayMs = 50.0;
                    double stepDelayMarginMs = BaseStepDelayMarginMs * scalingFactor;
                    double checkStepDelayMs = Math.Max(Math.Min(MinStepDelayMs, stepDelayMs), stepDelayMs - stepDelayMarginMs);
                    var expectedTimeMs = tiles * checkStepDelayMs;

                    state.WalkCreditMs -= expectedTimeMs;
                    if (state.WalkCreditMs < 0)
                    {
                        player.Logger.LogWarning(
                            "Speedhack detected on walk for player {0}: traveled {1} tiles in {2}ms (expected at least {3}ms), exceeding the tolerance of {4}ms.",
                            player.Name,
                            tiles,
                            elapsed.TotalMilliseconds,
                            expectedTimeMs,
                            maxCreditMs);
                        shouldRecordViolation = true;

                        // The deficit is not carried over, so a single violation doesn't cause a chain of follow-up violations.
                        state.WalkCreditMs = 0;
                    }
                }

                state.LastWalkTime = now;
                state.LastWalkStartPoint = startPoint;
            }
        }

        if (shouldRecordViolation)
        {
            eventArgs.IsCheatDetected = true;
            await this.RecordViolationAsync(player, state, config).ConfigureAwait(false);
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
        var now = DateTime.UtcNow;

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

            shouldRecordViolation = true;
        }

        if (shouldRecordViolation)
        {
            eventArgs.IsCheatDetected = true;
            await this.RecordViolationAsync(player, state, config).ConfigureAwait(false);
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

    private SpeedHackState GetState(Player player)
    {
        return this._playerStates.GetValue(player, p => new SpeedHackState(this.Configuration?.MaxAttackTokens ?? 5.0));
    }

    private async ValueTask RecordViolationAsync(Player player, SpeedHackState state, SpeedHackDetectConfiguration config)
    {
        var now = DateTime.UtcNow;
        bool shouldBan = false;
        bool shouldWarn = false;
        bool shouldDisconnect = false;

        lock (state.Lock)
        {
            if (now - state.LastAlertTime < TimeSpan.FromSeconds(config.AlertDebounceSeconds))
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

            if (state.AlertTimes.Count > config.MaxWarnings)
            {
                if (config.AutoBan)
                {
                    player.Logger.LogError("Player {0} exceeded speedhack warning limit. Banning account {1} and disconnecting.", player.Name, player.Account?.LoginName);
                    if (player.Account is { } account)
                    {
                        account.State = AccountState.Banned;
                        shouldBan = true;
                    }
                }

                if (config.DisconnectOnViolation)
                {
                    shouldDisconnect = true;
                }

                if (!shouldBan && !shouldDisconnect)
                {
                    shouldWarn = true;
                }
            }
            else
            {
                shouldWarn = true;
            }
        }

        if (shouldBan)
        {
            await player.SaveProgressAsync().ConfigureAwait(false);
        }

        if (shouldBan || shouldDisconnect)
        {
            await player.DisconnectAsync().ConfigureAwait(false);
        }
        else if (shouldWarn)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SpeedHackWarning)).ConfigureAwait(false);
        }
        else
        {
            // Do nothing if no actions are required.
        }
    }

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

        public void ResetWalk()
        {
            this.LastWalkTime = null;
            this.LastWalkStartPoint = null;
            this.WalkCreditMs = 0;
        }
    }
}
