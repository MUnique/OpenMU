// <copyright file="LogoutAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Action to log the player out of the game.
    /// </summary>
    public class LogoutAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LogoutAction));

        private readonly IGameServerContext gameServerContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public LogoutAction(IGameServerContext gameContext)
        {
            this.gameServerContext = gameContext;
        }

        /// <summary>
        /// Logs out the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="logoutType">Type of the logout.</param>
        public void Logout(Player player, LogoutType logoutType)
        {
            if (player.CurrentMap != null)
            {
                player.CurrentMap.Remove(player);
            }

            if (player.SelectedCharacter != null)
            {
                this.gameServerContext.FriendServer.SetOnlineState(player.SelectedCharacter.Id, player.SelectedCharacter.Name, 0xFF);
                player.SelectedCharacter = null;
            }

            player.MagicEffectList.ClearAllEffects();
            if (logoutType == LogoutType.CloseGame)
            {
                player.Disconnect();
            }
            else
            {
                if (logoutType == LogoutType.BackToCharacterSelection)
                {
                    player.PlayerState.TryAdvanceTo(PlayerState.Authenticated);
                }

                player.PlayerView.Logout(logoutType);
            }
        }
    }
}
