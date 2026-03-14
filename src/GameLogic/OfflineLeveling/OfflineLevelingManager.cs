// <copyright file="OfflineLevelingManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;


/// <summary>
/// Manages active <see cref="OfflineLevelingPlayer"/> sessions.
/// </summary>
public sealed class OfflineLevelingManager
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, OfflineLevelingPlayer> _activePlayers =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Starts an offline leveling session by replacing the real player with a ghost.
    /// </summary>
    /// <param name="realPlayer">The real player who typed the command.</param>
    /// <param name="loginName">The pre-validated account login name.</param>
    /// <returns><c>true</c> if the offline session was started successfully.</returns>
    public async ValueTask<bool> StartAsync(Player realPlayer, string loginName)
    {
        // Capture references before disconnect clears them from the player instance.
        var account = realPlayer.Account;
        var character = realPlayer.SelectedCharacter;

        if (account is null || character is null)
        {
            return false;
        }

        // Atomically claim the slot to prevent racing during initialization.
        var sentinel = new OfflineLevelingPlayer(realPlayer.GameContext);
        if (!this._activePlayers.TryAdd(loginName, sentinel))
        {
            // Another session is already active or being started for this account.
            await sentinel.DisposeAsync().ConfigureAwait(false);
            return false;
        }

        try
        {
            // Log off from the login server to allow reconnecting while the ghost runs.
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

            // Suppress the disconnection event to prevent double-save or redundant log-offs.
            realPlayer.SuppressDisconnectedEvent();

            // Perform disconnection: removes player from map and saves progress.
            await realPlayer.DisconnectAsync().ConfigureAwait(false);

            // Dispose the old context so entities can be attached to the ghost's new context.
            realPlayer.PersistenceContext.Dispose();

            // Now the name slot is free, initialize the ghost in-place.
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