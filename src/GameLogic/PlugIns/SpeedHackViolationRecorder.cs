// <copyright file="SpeedHackViolationRecorder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Records speed hack warnings and applies the configured sanction.
/// </summary>
internal sealed class SpeedHackViolationRecorder
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedHackViolationRecorder"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    public SpeedHackViolationRecorder(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Gets the warning count for the player.
    /// </summary>
    /// <param name="state">The player state.</param>
    /// <returns>The warning count.</returns>
    public int GetWarningCount(SpeedHackState state)
    {
        lock (state.Lock)
        {
            return state.AlertTimes.Count;
        }
    }

    /// <summary>
    /// Clears the alert debounce for the player.
    /// </summary>
    /// <param name="state">The player state.</param>
    public void ResetDebounce(SpeedHackState state)
    {
        lock (state.Lock)
        {
            state.LastAlertTimestamp = null;
        }
    }

    /// <summary>
    /// Records a warning and applies the configured sanction.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="state">The player state.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="severity">The severity which was detected.</param>
    /// <returns>The task.</returns>
    public async ValueTask RecordAsync(Player player, SpeedHackState state, SpeedHackDetectConfiguration config, SpeedHackViolationSeverity severity)
    {
        var now = this._timeProvider.GetTimestamp();
        ViolationPolicy.Sanction sanction;
        lock (state.Lock)
        {
            // Isolated blips within the debounce window are dropped for every severity, so a
            // single burst can neither flood the log nor fill the warning history.
            var sinceLastAlert = state.LastAlertTimestamp is { } lastAlertTimestamp
                ? this._timeProvider.GetElapsedTime(lastAlertTimestamp, now)
                : TimeSpan.MaxValue;
            if (sinceLastAlert < TimeSpan.FromSeconds(config.AlertDebounceSeconds))
            {
                return;
            }

            state.LastAlertTimestamp = now;
            state.AlertTimes.Enqueue(now);

            while (state.AlertTimes.Count > 0
                   && this._timeProvider.GetElapsedTime(state.AlertTimes.Peek(), now) > TimeSpan.FromHours(config.WarningHistoryHours))
            {
                state.AlertTimes.Dequeue();
            }

            player.Logger.LogWarning("Speedhack warning issued for player {0}. Total warnings in last hour: {1}", player.Name, state.AlertTimes.Count);
            sanction = ViolationPolicy.DecideSanction(config, state.AlertTimes.Count, CanPunish(player), player.Account is not null);
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

    private static bool CanPunish(Player player) =>
        player.Account?.State is not (AccountState.Spectator or AccountState.GameMaster or AccountState.GameMasterInvisible)
        && player.SelectedCharacter?.CharacterStatus is not CharacterStatus.GameMaster;
}
