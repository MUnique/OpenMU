// <copyright file="OfflineLevelingManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Collections.Concurrent;

/// <summary>
/// Tracks active <see cref="OfflineLevelingPlayer"/> instances across the game context.
/// One shared instance is stored on the <see cref="GameContext"/> so both the
/// chat command and the re-login plug-in can reach it.
/// </summary>
public sealed class OfflineLevelingManager
{
    private readonly ConcurrentDictionary<string, OfflineLevelingPlayer> _activePlayers =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Captures the real player's state, disconnects the real client, then spawns a ghost
    /// that continues leveling in the background.
    /// </summary>
    /// <param name="realPlayer">The real player who typed the command.</param>
    /// <param name="loginName">The pre-validated account login name.</param>
    /// <returns><c>true</c> if the offline session was started successfully.</returns>
    public async ValueTask<bool> StartAsync(Player realPlayer, string loginName)
    {
        // Capture all needed references before the real player disconnects,
        // since disconnect clears SelectedCharacter and Account from the Player instance.
        var account = realPlayer.Account;
        var character = realPlayer.SelectedCharacter;

        if (account is null || character is null)
        {
            return false;
        }

        // Use a placeholder sentinel to atomically claim the slot before any async work.
        // This prevents a second call for the same loginName from racing through while
        // the real player is being disconnected and the ghost is being initialized.
        var sentinel = new OfflineLevelingPlayer(realPlayer.GameContext);
        if (!this._activePlayers.TryAdd(loginName, sentinel))
        {
            // Another session is already active or being started for this account.
            await sentinel.DisposeAsync().ConfigureAwait(false);
            return false;
        }

        try
        {
            // Log off the login server so the player can reconnect once the ghost is running.
            if (realPlayer.GameContext is IGameServerContext gsCtx)
            {
                try
                {
                    await gsCtx.LoginServer.LogOffAsync(loginName, gsCtx.Id).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    realPlayer.Logger.LogWarning(ex, "Could not log off from login server during offlevel.");
                }
            }

            // Suppress the PlayerDisconnected event so GameServer.OnPlayerDisconnectedAsync
            // does not try to log off again or double-save.
            realPlayer.SuppressDisconnectedEvent();

            // DisconnectAsync: removes real player from map, fires PlayerLeftWorld
            // (freeing the name slot in PlayersByCharacterName), and saves progress.
            await realPlayer.DisconnectAsync().ConfigureAwait(false);

            // SuppressDisconnectedEvent() prevents OnPlayerDisconnectedAsync from firing,
            // so realPlayer.DisposeAsync() is never called by the game server — which means
            // realPlayer.PersistenceContext is never disposed and its EF DbContext is still
            // alive and tracking the account/character entities.
            // We must dispose it explicitly NOW so the entities become fully detached,
            // allowing the ghost's fresh PersistenceContext to Attach and track them.
            realPlayer.PersistenceContext.Dispose();

            // Now the name slot is free — initialize the ghost in-place.
            if (!await sentinel.InitializeAsync(account, character).ConfigureAwait(false))
            {
                this._activePlayers.TryRemove(loginName, out _);
                return false;
            }

            return true;
        }
        catch
        {
            this._activePlayers.TryRemove(loginName, out _);
            throw;
        }
    }

    /// <summary>
    /// Stops and removes the offline player for the given account, if one exists.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    public async ValueTask StopAsync(string loginName)
    {
        if (this._activePlayers.TryRemove(loginName, out var offlinePlayer))
        {
            await offlinePlayer.StopAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Returns whether an offline leveling session is currently active for <paramref name="loginName"/>.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    public bool IsActive(string loginName) => this._activePlayers.ContainsKey(loginName);
}