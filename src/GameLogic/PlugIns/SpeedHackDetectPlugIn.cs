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
[Display(Name = "Speedhack Anti-Cheat", Description = "Detects and prevents player walk and attack speedhacking.")]
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
        if (steps.IsEmpty)
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
                state.RecentWalks.Clear();
                state.LastWalkStartTime = DateTime.MinValue;
            }
            else
            {
                var now = DateTime.UtcNow;
                if (state.LastWalkStartTime > DateTime.MinValue)
                {
                    if (now - state.LastWalkStartTime > TimeSpan.FromSeconds(2))
                    {
                        state.RecentWalks.Clear();
                    }
                }

                state.RecentWalks.Enqueue(new WalkHistoryEntry { Time = now, StartPoint = startPoint });
                state.LastWalkStartTime = now;

                while (state.RecentWalks.Count > 5)
                {
                    state.RecentWalks.Dequeue();
                }

                if (state.RecentWalks.Count >= 3)
                {
                    var first = state.RecentWalks.Peek();
                    var elapsed = now - first.Time;

                    // Compute cumulative Chebyshev path length between consecutive start positions.
                    var cumulativeTiles = 0;
                    Point? prevPoint = null;
                    foreach (var entry in state.RecentWalks)
                    {
                        if (prevPoint.HasValue)
                        {
                            cumulativeTiles += Math.Max(
                                Math.Abs((int)entry.StartPoint.X - prevPoint.Value.X),
                                Math.Abs((int)entry.StartPoint.Y - prevPoint.Value.Y));
                        }

                        prevPoint = entry.StartPoint;
                    }

                    if (cumulativeTiles > 0)
                    {
                        double stepDelayMs = player.StepDelay.TotalMilliseconds;
                        double scalingFactor = stepDelayMs / NormalStepDelayMs;

                        const double BaseStepDelayMarginMs = 50.0;
                        const double MinStepDelayMs = 50.0;
                        double stepDelayMarginMs = BaseStepDelayMarginMs * scalingFactor;
                        double checkStepDelayMs = Math.Max(Math.Min(MinStepDelayMs, stepDelayMs), stepDelayMs - stepDelayMarginMs);
                        var expectedTime = TimeSpan.FromMilliseconds(cumulativeTiles * checkStepDelayMs);
                        var deficit = expectedTime - elapsed;
                        var tolerance = TimeSpan.FromMilliseconds(config.WalkSpeedToleranceMs * scalingFactor);

                        if (deficit > tolerance)
                        {
                            player.Logger.LogWarning(
                                "Speedhack detected on walk for player {0}: traveled {1} tiles in {2}ms (expected at least {3}ms). Deficit: {4}ms.",
                                player.Name,
                                cumulativeTiles,
                                elapsed.TotalMilliseconds,
                                expectedTime.TotalMilliseconds,
                                deficit.TotalMilliseconds);
                            shouldRecordViolation = true;
                            state.RecentWalks.Clear(); // Clear to avoid double triggers
                            state.LastWalkStartTime = DateTime.MinValue; // Reset tracker
                        }
                    }
                }
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
        if (player.Attributes is not { } attributes)
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
            state.LastWalkStartTime = DateTime.MinValue;
            state.RecentWalks.Clear();
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
            await player.ShowBlueMessageAsync("Warning: Unusual activity detected (speed check). Repeated violations will result in account restriction.").ConfigureAwait(false);
        }
        else
        {
            // Do nothing if no actions are required.
        }
    }

    private readonly record struct WalkHistoryEntry
    {
        public DateTime Time { get; init; }

        public Point StartPoint { get; init; }
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

        public Queue<WalkHistoryEntry> RecentWalks { get; } = new();

        public DateTime LastWalkStartTime { get; set; } = DateTime.MinValue;
    }
}
