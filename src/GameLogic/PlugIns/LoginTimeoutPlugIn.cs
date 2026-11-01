// <copyright file="LoginTimeoutPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Disconnects the clients which don't log in within the configured time after they connected.
/// </summary>
/// <remarks>
/// The connections which wait for their login are limited by the maximum players of the game server.
/// Without this plugin, a connection can stay at the login screen as long as the client keeps it
/// open, so connections which never log in could keep new clients out.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.LoginTimeoutPlugIn_Name), Description = nameof(PlugInResources.LoginTimeoutPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7CCAB352-EB98-48E4-A6D1-751F3E45EBFD")]
public class LoginTimeoutPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<LoginTimeoutPlugInConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <summary>
    /// The time when this plugin saw a player at the login screen the first time. Its entries go away with the players.
    /// </summary>
    private readonly ConditionalWeakTable<Player, StrongBox<DateTime>> _waitingSince = new();

    /// <inheritdoc />
    public LoginTimeoutPlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        this.Configuration ??= CreateDefaultConfiguration();
        var timeout = this.Configuration.Timeout;
        var now = DateTime.UtcNow;
        foreach (var player in await gameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (player.PlayerState.CurrentState != PlayerState.LoginScreen)
            {
                continue;
            }

            var waitingSince = this._waitingSince.GetValue(player, _ => new StrongBox<DateTime>(now)).Value;
            if (now - waitingSince < timeout)
            {
                continue;
            }

            player.Logger.LogInformation("Disconnecting {Player}, because it didn't log in within {Timeout}.", player, timeout);
            try
            {
                await player.DisconnectAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Error when disconnecting {Player} after the login timeout.", player);
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        // Nothing to start; the timeout is checked on every run.
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return CreateDefaultConfiguration();
    }

    private static LoginTimeoutPlugInConfiguration CreateDefaultConfiguration()
    {
        return new LoginTimeoutPlugInConfiguration();
    }
}
