// <copyright file="LogoutAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.GameLogic.Views.Login;

    /// <summary>
    /// Action to log the player out of the game.
    /// </summary>
    public class LogoutAction
    {
        /// <summary>
        /// Logs out the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="logoutType">Type of the logout.</param>
        public void Logout(Player player, LogoutType logoutType)
        {
            player.CurrentMap?.Remove(player);
            player.SelectedCharacter = null;
            player.MagicEffectList.ClearAllEffects();
            player.Party?.KickMySelf(player);
            player.PersistenceContext.SaveChanges();
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

                player.ViewPlugIns.GetPlugIn<ILogoutPlugIn>()?.Logout(logoutType);
            }
        }
    }
}
