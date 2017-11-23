// <copyright file="GuildInfoRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    /// <summary>
    /// Action to request the information (name, symbol) of a guild.
    /// </summary>
    public class GuildInfoRequestAction
    {
        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildInfoRequestAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildInfoRequestAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Requests the guild information.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="shortGuildId">The short guild identifier.</param>
        public void RequestGuildInfo(Player player, ushort shortGuildId)
        {
            byte[] guildInfo = this.gameContext.GuildCache.GetGuildData(shortGuildId);
            if (guildInfo != null)
            {
                player.PlayerView.GuildView.ShowGuildInfo(guildInfo);
            }
        }
    }
}
