// <copyright file="GuildCreateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Action to create a guild.
    /// </summary>
    public class GuildCreateAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(GuildCreateAction));

        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildCreateAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildCreateAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Creates the guild.
        /// </summary>
        /// <param name="creator">The creator.</param>
        /// <param name="guildName">Name of the guild.</param>
        /// <param name="guildEmblem">The guild emblem.</param>
        public void CreateGuild(Player creator, string guildName, byte[] guildEmblem)
        {
            if (creator.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                Log.Error($"Account {creator.Account.LoginName} not in the right state, but {creator.PlayerState.CurrentState}.");
                return;
            }

            if (this.gameContext.GuildServer.GuildExists(guildName))
            {
                creator.PlayerView.GuildView.ShowGuildCreateResult(GuildCreateErrorDetail.GuildAlreadyExist);
                return;
            }

            creator.GuildStatus = this.gameContext.GuildServer.CreateGuild(guildName, creator.SelectedCharacter.Name, creator.SelectedCharacter.Id, guildEmblem, this.gameContext.Id);
            if (creator.GuildStatus == null)
            {
                creator.PlayerView.GuildView.ShowGuildCreateResult(GuildCreateErrorDetail.GuildAlreadyExist);
                return;
            }

            creator.PlayerView.GuildView.ShowGuildCreateResult(GuildCreateErrorDetail.None);
            creator.ForEachObservingPlayer(p => p.PlayerView.GuildView.AssignPlayerToGuild(creator, false), true);

            Log.InfoFormat("Guild created: [{0}], Master: [{1}]", guildName, creator.SelectedCharacter.Name);
        }
    }
}
