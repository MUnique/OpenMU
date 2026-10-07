// <copyright file="LostConnectionDetectionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Disconnects players whose client stopped to report that it's alive.
/// </summary>
/// <remarks>
/// A connection can get lost without being closed, e.g. when the network of a mobile client drops or a
/// browser tab is frozen. The server doesn't notice that by itself, so the player would stay connected, and its
/// account would stay registered at the login server - every further login of the account would be rejected.
/// The game client reports periodically that it's alive (the ping, every 20 seconds), even when the player
/// doesn't do anything. When these reports stop for longer than the configured timeout while the player is in
/// the world, the connection is considered lost and the player is disconnected.
/// Players whose client didn't report it since entering the world, e.g. because it doesn't support it, are not
/// affected. Neither are players outside of the world, because not every client reports it there.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.LostConnectionDetectionPlugIn_Name), Description = nameof(PlugInResources.LostConnectionDetectionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("18632228-3E40-4C75-839D-AF7843089E82")]
public class LostConnectionDetectionPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<LostConnectionDetectionConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// The interval in which the players are checked. It's shorter than any sensible timeout,
    /// so that a lost connection is detected shortly after the timeout elapsed.
    /// </summary>
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);

    private DateTime _nextCheckUtc = DateTime.UtcNow;

    /// <inheritdoc />
    public LostConnectionDetectionConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        var timeout = (this.Configuration ??= new LostConnectionDetectionConfiguration()).Timeout;
        if (now < this._nextCheckUtc || timeout <= TimeSpan.Zero)
        {
            return;
        }

        this._nextCheckUtc = now + CheckInterval;
        var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players)
        {
            if (player.LastAliveReport is not { } lastAliveReport
                || now - lastAliveReport <= timeout
                || !player.IsConnected)
            {
                continue;
            }

            player.Logger.LogInformation("Disconnecting {Player}, because its client didn't report since {LastAliveReport} that it's alive.", player, lastAliveReport);
            await player.DisconnectAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextCheckUtc = DateTime.MinValue;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new LostConnectionDetectionConfiguration();
    }
}
