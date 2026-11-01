// <copyright file="LoginAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using System.Threading;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to log in a player to the game.
/// </summary>
public class LoginAction
{
    private static int _templateCounter;

    /// <summary>
    /// Logins the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    public async ValueTask LoginAsync(Player player, string username, string password)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());

        var state = await this.AuthenticateAsync(player, username, password).ConfigureAwait(false);
        if (state is null)
        {
            return;
        }

        if (!await this.ValidateAccountStateAsync(player, state.Value).ConfigureAwait(false))
        {
            return;
        }

        var (success, account) = await this.TryEstablishSessionAsync(player, username).ConfigureAwait(false);
        if (success && account is { })
        {
            try
            {
                await this.FinishLoginAsync(player, username, account).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Failed to finish login for [{Username}].", username);
                await ReleaseLoginServerRegistrationAsync(player).ConfigureAwait(false);

                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask ReleaseLoginServerRegistrationAsync(Player player)
    {
        if (player.ReleaseLoginServerRegistration() is not { } loginName
            || player.GameContext is not IGameServerContext gameServerContext)
        {
            return;
        }

        try
        {
            await gameServerContext.LoginServer.LogOffAsync(loginName, gameServerContext.Id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Couldn't log off [{Username}] at the login server after the failed login.", loginName);
        }
    }

    /// <summary>
    /// Reserves a place for the player on the game server, see <see cref="IGameContext.TryReserveLoginPlace"/>.
    /// </summary>
    /// <param name="player">The player which logs in. It's not logged in yet, so it isn't counted.</param>
    /// <param name="gameServerContext">The game server context.</param>
    /// <param name="username">The username.</param>
    /// <param name="replacedPlayer">The logged in player whose place the player takes over, e.g. its offline player.</param>
    /// <returns>The reservation, or <c>null</c> when the game server is full.</returns>
    /// <remarks>
    /// Only logged in players and logins in progress take a place, so connections which stay at the
    /// login screen don't.
    /// </remarks>
    private static IDisposable? TryReservePlace(Player player, IGameServerContext gameServerContext, string username, Player? replacedPlayer)
    {
        var maximumPlayers = gameServerContext.ServerConfiguration.MaximumPlayers;
        var (reservation, occupiedPlaces) = gameServerContext.TryReserveLoginPlace(maximumPlayers, replacedPlayer);
        if (reservation is not null)
        {
            return reservation;
        }

        player.Logger.LogInformation(
            "Login of [{Username}] refused, because the game server is full: {OccupiedPlaces} of {MaximumPlayers} players are logged in or logging in.",
            username,
            occupiedPlaces,
            maximumPlayers);
        return null;
    }

    /// <summary>
    /// Refuses the login, because the game server is full. The connection stays at the login screen.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="context">The context of the state change to <see cref="PlayerState.Authenticated"/>, which doesn't happen.</param>
    /// <returns>The result of the session establishment.</returns>
    private static async ValueTask<(bool Success, Account? Account)> RefuseBecauseFullAsync(Player player, StateMachine.StateChangeContext context)
    {
        context.Allowed = false;
        await ReleaseLoginServerRegistrationAsync(player).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ServerIsFull)).ConfigureAwait(false);
        return (false, null);
    }

    private async ValueTask<AccountState?> AuthenticateAsync(Player player, string username, string password)
    {
        try
        {
            var state = await player.PersistenceContext.AuthenticateAsync(username, password).ConfigureAwait(false);
            if (state is null)
            {
                player.Logger.LogInformation("Account not found or invalid password, username: [{Username}].", username);
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.InvalidPassword)).ConfigureAwait(false);
            }

            return state;
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Login Failed.");
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            return null;
        }
    }

    private async ValueTask<bool> ValidateAccountStateAsync(Player player, AccountState state)
    {
        if (state == AccountState.Banned)
        {
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountBlocked)).ConfigureAwait(false);
            return false;
        }

        if (state == AccountState.TemporarilyBanned)
        {
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.TemporaryBlocked)).ConfigureAwait(false);
            return false;
        }

        return true;
    }

    private async ValueTask<(bool Success, Account? Account)> TryEstablishSessionAsync(Player player, string username)
    {
        // Kept until the state change of the context below is done, see IGameContext.TryReserveLoginPlace.
        IDisposable? reservedPlace = null;
        try
        {
            await using var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
            if (!context.Allowed)
            {
                await this.HandleAlreadyConnectedAsync(player, username).ConfigureAwait(false);
                return (false, null);
            }

            if (player.GameContext is not IGameServerContext gameServerContext)
            {
                context.Allowed = false;
                return (false, null);
            }

            if (player.GameContext.OfflinePlayerManager.TryGetPlayer(username, out var offlinePlayer))
            {
                // The player takes over the place of its offline player. It needs a free place only when the
                // offline player isn't logged in anymore, e.g. because its session ended in the meantime.
                // The place is reserved before the offline session gets stopped, so it's kept when it fails.
                reservedPlace = TryReservePlace(player, gameServerContext, username, offlinePlayer);
                if (reservedPlace is null)
                {
                    return await RefuseBecauseFullAsync(player, context).ConfigureAwait(false);
                }

                var isTemplateOffline = offlinePlayer!.IsTemplatePlayer;
                if (!isTemplateOffline)
                {
                    if (!await gameServerContext.LoginServer.TryLoginAsync(username, gameServerContext.Id).ConfigureAwait(false))
                    {
                        context.Allowed = false;
                        await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountAlreadyConnected)).ConfigureAwait(false);
                        await NotifyAlreadyLoggedInAsync(gameServerContext, username).ConfigureAwait(false);
                        return (false, null);
                    }

                    // Remembered while the state change is in progress, which a disconnect waits for.
                    player.SetLoginServerRegistration(username);
                }

                var offlineAccount = await this.HandleOfflineSessionHandoverAsync(player, username).ConfigureAwait(false);
                if (offlineAccount is null)
                {
                    await ReleaseLoginServerRegistrationAsync(player).ConfigureAwait(false);
                    context.Allowed = false;
                    return (false, null);
                }

                return (true, offlineAccount);
            }

            // The account is reserved at the login server BEFORE it's loaded. The old session of an
            // account saves its progress and only then logs off (see Player.DisconnectAsync), so a
            // reservation which succeeds guarantees that the load below sees that save. Loading first
            // let a quick reconnect (the client resumes about 2 seconds after a lost connection) load
            // the account while the old session was still saving, and reserve it right after the old
            // session logged off: two diverging copies of the character, and the database ended up
            // with both of them - duplicated items, and items on top of each other.
            var reserved = await gameServerContext.LoginServer.TryLoginAsync(username, gameServerContext.Id).ConfigureAwait(false);
            if (reserved)
            {
                // Remembered while the state change is in progress, which a disconnect waits for.
                // Set before the load, so that every failure below releases it exactly once.
                player.SetLoginServerRegistration(username);

                // A full game server refuses the login before the account gets loaded. When the account
                // couldn't be reserved, it's loaded first, so that an account which is logged in
                // somewhere else is still reported as such (unless it's a template account).
                reservedPlace = TryReservePlace(player, gameServerContext, username, null);
                if (reservedPlace is null)
                {
                    return await RefuseBecauseFullAsync(player, context).ConfigureAwait(false);
                }
            }

            var loadedAccount = await player.PersistenceContext.GetAccountByLoginNameAsync(username).ConfigureAwait(false);
            if (loadedAccount is null)
            {
                await ReleaseLoginServerRegistrationAsync(player).ConfigureAwait(false);
                player.Logger.LogError("Failed to load account {Username} after authentication.", username);
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
                context.Allowed = false;
                return (false, null);
            }

            if (loadedAccount.IsTemplate)
            {
                // Template accounts may be logged in several times at once, so they never hold the reservation.
                await ReleaseLoginServerRegistrationAsync(player).ConfigureAwait(false);
            }
            else if (!reserved)
            {
                context.Allowed = false;
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountAlreadyConnected)).ConfigureAwait(false);
                await NotifyAlreadyLoggedInAsync(gameServerContext, username).ConfigureAwait(false);
                return (false, null);
            }

            reservedPlace ??= TryReservePlace(player, gameServerContext, username, null);
            if (reservedPlace is null)
            {
                return await RefuseBecauseFullAsync(player, context).ConfigureAwait(false);
            }

            return (true, loadedAccount);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error during login through login server.");
            await ReleaseLoginServerRegistrationAsync(player).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            return (false, null);
        }
        finally
        {
            // After the state change: the context is disposed when the try block is left.
            reservedPlace?.Dispose();
        }
    }

    private async ValueTask<Account?> HandleOfflineSessionHandoverAsync(Player player, string username)
    {
        player.Logger.LogInformation("Account {Username} has an active offline session. Stopping it and reloading account data.", username);

        try
        {
            await player.GameContext.OfflinePlayerManager.StopAsync(username).ConfigureAwait(false);
            var account = await player.PersistenceContext.GetAccountByLoginNameAsync(username).ConfigureAwait(false);
            if (account is null)
            {
                player.Logger.LogError("Failed to reload account {Username} after stopping offline session.", username);
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            }

            return account;
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Failed to reload account {Username} after stopping offline session.", username);
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            return null;
        }
    }

    /// <summary>
    /// Notifies the game servers and external systems, that somebody tried to log into an account which is already logged in.
    /// </summary>
    private static async ValueTask NotifyAlreadyLoggedInAsync(IGameServerContext gameServerContext, string username)
    {
        await gameServerContext.EventPublisher.PlayerAlreadyLoggedInAsync(gameServerContext.Id, username).ConfigureAwait(false);
        await gameServerContext.EventPublisher.GameEventAsync(new AccountLoginBlockedEvent(gameServerContext.Id, DateTime.UtcNow, username)).ConfigureAwait(false);
    }

    private async ValueTask HandleAlreadyConnectedAsync(Player player, string username)
    {
        var result = player.LoginResultOverride ?? LoginResult.AccountAlreadyConnected;
        player.LoginResultOverride = null;
        await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(result)).ConfigureAwait(false);
        if (player.GameContext is IGameServerContext gameServerContext)
        {
            await NotifyAlreadyLoggedInAsync(gameServerContext, username).ConfigureAwait(false);
        }
    }

    private async ValueTask FinishLoginAsync(Player player, string username, Account account)
    {
        await player.SetAccountAsync(account).ConfigureAwait(false);
        player.Logger.LogDebug("Login successful, username: [{Username}].", username);

        if (player.IsTemplatePlayer)
        {
            foreach (var character in account.Characters)
            {
                var counter = Interlocked.Increment(ref _templateCounter);
                character.Name = $"_{counter}";
            }
        }

        await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.Ok)).ConfigureAwait(false);
    }
}