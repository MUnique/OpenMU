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

        var account = await this.AuthenticateAccountAsync(player, username, password).ConfigureAwait(false);
        if (account is null || !await this.ValidateAccountStateAsync(player, account).ConfigureAwait(false))
        {
            return;
        }

        var (success, finalAccount) = await this.TryEstablishSessionAsync(player, username, password, account).ConfigureAwait(false);
        if (success && finalAccount is { })
        {
            await this.FinishLoginAsync(player, username, finalAccount).ConfigureAwait(false);
        }
    }

    private async ValueTask<Account?> AuthenticateAccountAsync(Player player, string username, string password)
    {
        try
        {
            var account = await player.PersistenceContext.GetAccountByLoginNameAsync(username, password).ConfigureAwait(false);
            if (account is null)
            {
                player.Logger.LogInformation($"Account not found or invalid password, username: [{username}].");
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.InvalidPassword)).ConfigureAwait(false);
            }

            return account;
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Login Failed.");
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            return null;
        }
    }

    private async ValueTask<bool> ValidateAccountStateAsync(Player player, Account account)
    {
        if (account.State == AccountState.Banned)
        {
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountBlocked)).ConfigureAwait(false);
            return false;
        }

        if (account.State == AccountState.TemporarilyBanned)
        {
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.TemporaryBlocked)).ConfigureAwait(false);
            return false;
        }

        return true;
    }

    private async ValueTask<(bool Success, Account? Account)> TryEstablishSessionAsync(Player player, string username, string password, Account account)
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
                return (false, null);
            }

            if (!account.IsTemplate && !await gameServerContext.LoginServer.TryLoginAsync(username, gameServerContext.Id).ConfigureAwait(false))
            {
                context.Allowed = false;
                await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.AccountAlreadyConnected)).ConfigureAwait(false);
                await gameServerContext.EventPublisher.PlayerAlreadyLoggedInAsync(gameServerContext.Id, username).ConfigureAwait(false);
                return (false, null);
            }

            var finalAccount = account;
            if (player.GameContext.OfflineLevelingManager.IsActive(username))
            {
                finalAccount = await this.HandleOfflineSessionHandoverAsync(player, username, password, account).ConfigureAwait(false);
                if (finalAccount is null)
                {
                    context.Allowed = false;
                    return (false, null);
                }
            }

            return (true, finalAccount);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error during login through login server.");
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
            return (false, null);
        }
    }

    private async ValueTask<Account?> HandleOfflineSessionHandoverAsync(Player player, string username, string password, Account account)
    {
        player.Logger.LogInformation("Account {username} has an active offline session. Stopping it and reloading account data.", username);
        await player.GameContext.OfflineLevelingManager.StopAsync(username).ConfigureAwait(false);
        player.PersistenceContext.Detach(account);
        var reloadedAccount = await player.PersistenceContext.GetAccountByLoginNameAsync(username, password).ConfigureAwait(false);
        if (reloadedAccount is null)
        {
            player.Logger.LogError("Failed to reload account {username} after stopping offline session.", username);
            await player.InvokeViewPlugInAsync<IShowLoginResultPlugIn>(p => p.ShowLoginResultAsync(LoginResult.ConnectionError)).ConfigureAwait(false);
        }

        return reloadedAccount;
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
        player.Logger.LogDebug($"Login successful, username: [{username}].");

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