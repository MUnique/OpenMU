// <copyright file="OfflineLevelingManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.Interfaces;

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

        // Charge initial Zen cost (similar to MuHelper start)
        var helperConfig = realPlayer.GameContext.FeaturePlugIns.GetPlugIn<MuHelperFeaturePlugIn>()?.Configuration ??
                           new MuHelperServerConfiguration();
        var totalLevel = (int)(realPlayer.Level + (realPlayer.Attributes?[Stats.MasterLevel] ?? 0));
        var initialCost = helperConfig.CostPerStage.FirstOrDefault() * totalLevel;

        if (initialCost > 0 && !realPlayer.TryRemoveMoney(initialCost))
        {
            this._activePlayers.TryRemove(loginName, out _);
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

            var party = realPlayer.Party;

            // Suppress the disconnection event to prevent double-save or redundant log-offs.
            realPlayer.SuppressDisconnectedEvent();

            // Clear the player's party reference before disconnecting so that DisconnectAsync
            // does NOT call LeaveTemporarilyAsync and create a stale OfflinePartyMember snapshot.
            // The ghost will be properly inserted into the party after it is fully initialized.
            if (party is not null)
            {
                realPlayer.Party = null;
            }

            // Perform disconnection: removes player from map and saves progress.
            await realPlayer.DisconnectAsync().ConfigureAwait(false);

            // Dispose the old context so entities can be attached to the ghost's new context.
            realPlayer.PersistenceContext.Dispose();

            if (!await sentinel.InitializeAsync(account, character).ConfigureAwait(false))
            {
                this._activePlayers.TryRemove(loginName, out _);
                return false;
            }

            // Now that the offline player is fully initialized, swap it into the party.
            if (party is not null)
            {
                await party.ReplaceMemberAsync(realPlayer, sentinel).ConfigureAwait(false);
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

    /// <summary>
    /// Returns a snapshot of all currently active offline leveling players.
    /// </summary>
    public IReadOnlyCollection<OfflineLevelingPlayer> GetOfflineLevelingPlayers()
        => this._activePlayers.Values.ToList();
}