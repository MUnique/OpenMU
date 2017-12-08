// <copyright file="LoginAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to log in a player to the game.
    /// </summary>
    public class LoginAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LoginAction));

        private readonly IGameServerContext gameServerContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public LoginAction(IGameServerContext gameContext)
        {
            this.gameServerContext = gameContext;
        }

        /// <summary>
        /// Logins the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void Login(Player player, string username, string password)
        {
            Account account = null;
            try
            {
                using (this.gameServerContext.RepositoryManager.UseContext(player.PersistenceContext))
                {
                    var repository = this.gameServerContext.RepositoryManager.GetRepository<Account, IAccountRepository<Account>>();
                    account = repository.GetAccountByLoginName(username, password);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Login Failed.", ex);
            }

            if (account != null)
            {
                if (account.State == AccountState.Banned)
                {
                    player.PlayerView.ShowLoginResult(LoginResult.AccountBlocked);
                }
                else if (account.State == AccountState.TemporarilyBanned)
                {
                    player.PlayerView.ShowLoginResult(LoginResult.TemporaryBlocked);
                }
                else
                {
                    using (var context = player.PlayerState.TryBeginAdvanceTo(PlayerState.Authenticated))
                    {
                        if (this.gameServerContext.LoginServer.TryLogin(username, this.gameServerContext.Id))
                        {
                            context.Allowed = true;
                            player.Account = account;
                            Log.DebugFormat("Login successful, username: [{0}]", username);
                            player.PlayerView.ShowLoginResult(LoginResult.OK);
                        }
                        else
                        {
                            context.Allowed = false;
                            player.PlayerView.ShowLoginResult(LoginResult.AccountAlreadyConnected);
                        }
                    }
                }
            }
            else
            {
                Log.InfoFormat($"Account not found or invalid password, username: [{username}]");
                player.PlayerView.ShowLoginResult(LoginResult.InvalidPassword);
            }
        }
    }
}
