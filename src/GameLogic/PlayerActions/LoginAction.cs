// <copyright file="LoginAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Login;

    /// <summary>
    /// Action to log in a player to the game.
    /// </summary>
    public class LoginAction
    {
        /// <summary>
        /// Logins the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void Login(Player player, string username, string password)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            Account? account;
            try
            {
                account = player.PersistenceContext.GetAccountByLoginName(username, password);
            }
            catch (Exception ex)
            {
                player.Logger.LogError("Login Failed.", ex);
                player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()?.ShowLoginResult(LoginResult.ConnectionError);
                return;
            }

            if (account is null)
            {
                player.Logger.LogInformation($"Account not found or invalid password, username: [{username}]");
                player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()?.ShowLoginResult(LoginResult.InvalidPassword);
            }
            else if (account.State == AccountState.Banned)
            {
                player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()?.ShowLoginResult(LoginResult.AccountBlocked);
            }
            else if (account.State == AccountState.TemporarilyBanned)
            {
                player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()
                    ?.ShowLoginResult(LoginResult.TemporaryBlocked);
            }
            else
            {
                using var context = player.PlayerState.TryBeginAdvanceTo(PlayerState.Authenticated);
                if (context.Allowed && player.GameContext is IGameServerContext gameServerContext &&
                    gameServerContext.LoginServer.TryLogin(username, gameServerContext.Id))
                {
                    player.Account = account;
                    player.Logger.LogDebug("Login successful, username: [{0}]", username);
                    player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()?.ShowLoginResult(LoginResult.OK);
                }
                else
                {
                    context.Allowed = false;
                    player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()
                        ?.ShowLoginResult(LoginResult.AccountAlreadyConnected);
                }
            }
        }
    }
}
