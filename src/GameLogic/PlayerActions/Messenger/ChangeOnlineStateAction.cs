// <copyright file="ChangeOnlineStateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    /// <summary>
    /// Action to change the online state for the messenger.
    /// </summary>
    public class ChangeOnlineStateAction
    {
        private const byte InvisibleState = 0xFE;

        private IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeOnlineStateAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChangeOnlineStateAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Sets the online state.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="online">If set to <c>true</c> the player is visible as online with its server id to his friends. Otherwise, not.</param>
        public void SetOnlineState(Player player, bool online)
        {
            player.OnlineAsFriend = online;
            this.gameContext.FriendServer.SetOnlineState(player.SelectedCharacter.Id, player.SelectedCharacter.Name, player.OnlineAsFriend ? this.gameContext.Id : InvisibleState);
        }
    }
}
