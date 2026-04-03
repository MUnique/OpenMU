// <copyright file="LoginAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using System.Threading;
using MUnique.OpenMU.GameLogic.Views.Login;

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
                if (!account.IsTemplate && player.GameContext is IGameServerContext gameServerContext)
                {
                    await gameServerContext.LoginServer.LogOffAsync(username, gameServerContext.Id).ConfigureAwait(false);
                }

                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            }
        }
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

            if (player.GameContext.OfflineLevelingManager.TryGetPlayer(username, out var offlinePlayer))
            {
                var isTemplateOffline = offlinePlayer!.IsTemplatePlayer;
                if (!isTemplateOffline && !await gameServerContext.LoginServer.TryLoginAsync(username, gameServerContext.Id).ConfigureAwait(false))
                {
                    context.Allowed = false;
                    await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountAlreadyConnected)).ConfigureAwait(false);
                    await gameServerContext.EventPublisher.PlayerAlreadyLoggedInAsync(gameServerContext.Id, username).ConfigureAwait(false);
                    return (false, null);
                }

                var offlineAccount = await this.HandleOfflineSessionHandoverAsync(player, username).ConfigureAwait(false);
                if (offlineAccount is null)
                {
                    if (!isTemplateOffline)
                    {
                        await gameServerContext.LoginServer.LogOffAsync(username, gameServerContext.Id).ConfigureAwait(false);
                    }

                    context.Allowed = false;
                    return (false, null);
                }

                return (true, offlineAccount);
            }

            var loadedAccount = await player.PersistenceContext.GetAccountByLoginNameAsync(username).ConfigureAwait(false);
            if (loadedAccount is null)
            {
                player.Logger.LogError("Failed to load account {Username} after authentication.", username);
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
                context.Allowed = false;
                return (false, null);
            }

            if (!loadedAccount.IsTemplate && !await gameServerContext.LoginServer.TryLoginAsync(username, gameServerContext.Id).ConfigureAwait(false))
            {
                context.Allowed = false;
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountAlreadyConnected)).ConfigureAwait(false);
                await gameServerContext.EventPublisher.PlayerAlreadyLoggedInAsync(gameServerContext.Id, username).ConfigureAwait(false);
                return (false, null);
            }

            return (true, loadedAccount);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error during login through login server.");
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            return (false, null);
        }
    }

    private async ValueTask<Account?> HandleOfflineSessionHandoverAsync(Player player, string username)
    {
        player.Logger.LogInformation("Account {Username} has an active offline session. Stopping it and reloading account data.", username);

        try
        {
            await player.GameContext.OfflineLevelingManager.StopAsync(username).ConfigureAwait(false);
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

    private async ValueTask HandleAlreadyConnectedAsync(Player player, string username)
    {
        await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountAlreadyConnected)).ConfigureAwait(false);
        if (player.GameContext is IGameServerContext gameServerContext)
        {
            await gameServerContext.EventPublisher.PlayerAlreadyLoggedInAsync(gameServerContext.Id, username).ConfigureAwait(false);
        }
    }

    private async ValueTask FinishLoginAsync(Player player, string username, Account account)
    {
        player.Account = account;
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