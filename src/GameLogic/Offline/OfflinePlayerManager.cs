// <copyright file="OfflinePlayerManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// Manages active <see cref="OfflinePlayer"/> sessions.
/// </summary>
public sealed class OfflinePlayerManager
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, OfflinePlayer> _activePlayers =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a snapshot of all currently active offline players.
    /// </summary>
    public IReadOnlyCollection<OfflinePlayer> OfflinePlayers
        => this._activePlayers.Values.ToList();

    /// <summary>
    /// Starts an offline player session by replacing the real player with a copy.
    /// </summary>
    /// <param name="realPlayer">The real player who typed the command.</param>
    /// <param name="loginName">The pre-validated account login name.</param>
    /// <returns><c>true</c> if the offline session was started successfully.</returns>
    public async ValueTask<bool> StartAsync(Player realPlayer, string loginName)
    {
        var account = realPlayer.Account;
        var character = realPlayer.SelectedCharacter;

        if (account is null || character is null)
        {
            return false;
        }

        var sentinel = new OfflinePlayer(realPlayer.GameContext);

        // Atomically claim the slot to prevent racing during initialization.
        if (!this._activePlayers.TryAdd(loginName, sentinel))
        {
            await sentinel.DisposeAsync().ConfigureAwait(false);
            return false;
        }

        if (!this.TryChargeInitialZenCost(realPlayer))
        {
            await this.RemoveAndDisposeAsync(loginName, sentinel).ConfigureAwait(false);
            return false;
        }

        try
        {
            await this.TransitionToOfflineAsync(realPlayer, loginName).ConfigureAwait(false);

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
    /// Stops and removes the offline session for the given account, if one exists.
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
    /// Returns whether an offline player session is currently active for <paramref name="loginName"/>.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    public bool IsActive(string loginName) => this._activePlayers.ContainsKey(loginName);

    /// <summary>
    /// Tries to get the active offline player for the given account login name.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    /// <param name="player">The offline player, if found.</param>
    /// <returns><c>true</c> if an active session exists; otherwise <c>false</c>.</returns>
    public bool TryGetPlayer(string loginName, out OfflinePlayer? player)
        => this._activePlayers.TryGetValue(loginName, out player);

    private async ValueTask TransitionToOfflineAsync(Player realPlayer, string loginName)
    {
        await this.LogOffFromLoginServerAsync(realPlayer, loginName).ConfigureAwait(false);

        realPlayer.SuppressDisconnectedEvent();
        await realPlayer.DisconnectAsync().ConfigureAwait(false);
        realPlayer.PersistenceContext.Dispose();
    }

    /// <summary>
    /// Calculates and deducts the initial Zen cost for starting an offline player session.
    /// The cost is based on the first MuHelper cost stage multiplied by the player's total level.
    /// </summary>
    /// <param name="player">The player to charge.</param>
    /// <returns><c>true</c> if the cost was successfully charged or no cost applies; otherwise <c>false</c>.</returns>
    private bool TryChargeInitialZenCost(Player player)
    {
        var initialCost = this.CalculateInitialZenCost(player);

        return initialCost <= 0 || player.TryRemoveMoney(initialCost);
    }

    /// <summary>
    /// Calculates the initial Zen cost for the given player based on the MuHelper configuration
    /// and the player's combined normal and master level.
    /// </summary>
    /// <param name="player">The player for whom to calculate the cost.</param>
    /// <returns>The Zen amount to charge; 0 if no cost applies.</returns>
    private int CalculateInitialZenCost(Player player)
    {
        var config = player.GameContext.FeaturePlugIns.GetPlugIn<MuHelperFeaturePlugIn>()?.Configuration
                     ?? new MuHelperConfiguration();

        var costPerStage = config.CostPerStage.FirstOrDefault();
        if (costPerStage <= 0)
        {
            return 0;
        }

        var totalLevel = player.Level + (int)(player.Attributes?[Stats.MasterLevel] ?? 0);

        return costPerStage * totalLevel;
    }

    /// <summary>
    /// Attempts to log the player off from the login server so the account slot is freed
    /// for reconnection while the ghost session is running. Failures are logged and swallowed
    /// because the offline session can still proceed without this step.
    /// </summary>
    /// <param name="player">The player being transitioned to offline.</param>
    /// <param name="loginName">The account login name.</param>
    private async ValueTask LogOffFromLoginServerAsync(Player player, string loginName)
    {
        if (player.GameContext is not IGameServerContext gsCtx)
        {
            return;
        }

        try
        {
            await gsCtx.LoginServer.LogOffAsync(loginName, gsCtx.Id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "Could not log off from login server during offline player start.");
        }
    }

    /// <summary>
    /// Removes the sentinel entry from the active players dictionary and disposes the ghost.
    /// Used when startup fails before the ghost is fully initialized.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    /// <param name="sentinel">The ghost player to dispose.</param>
    private async ValueTask RemoveAndDisposeAsync(string loginName, OfflinePlayer sentinel)
    {
        this._activePlayers.TryRemove(loginName, out _);
        await sentinel.DisposeAsync().ConfigureAwait(false);
    }
}