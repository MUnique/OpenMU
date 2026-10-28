// <copyright file="PingCheck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Compares the timing and attack speed reported by client ping packets with the server.
/// </summary>
internal sealed class PingCheck
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingCheck"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    public PingCheck(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Evaluates a ping packet. Mismatches are only logged, never sanctioned.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="state">The player state.</param>
    /// <param name="tickCount">The client tick count.</param>
    /// <param name="attackSpeed">The attack speed reported by the client.</param>
    public void Evaluate(
        Player player,
        SpeedHackDetectConfiguration config,
        SpeedHackState state,
        uint tickCount,
        ushort attackSpeed)
    {
        var now = this._timeProvider.GetTimestamp();

        double serverElapsedMs;
        PingClockValidator.Result clockResult;
        lock (state.Lock)
        {
            if (state.LastPingTimestamp is not { } lastPingTimestamp
                || state.LastClientTickCount is not { } lastClientTickCount)
            {
                state.LastPingTimestamp = now;
                state.LastClientTickCount = tickCount;
                return;
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
        else
        {
            // Client clock is within expected tolerance; no action taken.
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
    }
}
