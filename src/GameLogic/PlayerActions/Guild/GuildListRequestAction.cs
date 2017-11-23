// <copyright file="GuildListRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    /// <summary>
    /// Action to request the guild list.
    /// </summary>
    public class GuildListRequestAction
    {
        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildListRequestAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildListRequestAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Requests the guild list of the guild the player is currently part of.
        /// </summary>
        /// <param name="player">The player.</param>
        public void RequestGuildList(Player player)
        {
            if (player.SelectedCharacter.GuildMemberInfo == null)
            {
                return;
            }

            var players = this.gameContext.GuildServer.GetGuildList(player.SelectedCharacter.GuildMemberInfo.GuildId);
            player.PlayerView.GuildView.ShowGuildList(players);
        }
    }
}
