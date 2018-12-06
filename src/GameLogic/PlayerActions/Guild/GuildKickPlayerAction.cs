// <copyright file="GuildKickPlayerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using MUnique.OpenMU.Interfaces;
    using Views;

    /// <summary>
    /// Action to kick a player out of a guild.
    /// </summary>
    public class GuildKickPlayerAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(GuildKickPlayerAction));

        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildKickPlayerAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildKickPlayerAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Kicks the player out of the guild.
        /// </summary>
        /// <param name="guildMaster">The guild master.</param>
        /// <param name="nickname">The nickname.</param>
        /// <param name="securityCode">The security code.</param>
        public void KickPlayer(Player guildMaster, string nickname, string securityCode)
        {
            if (guildMaster.Account.SecurityCode != null && guildMaster.Account.SecurityCode != securityCode)
            {
                guildMaster.PlayerView.ShowMessage("Wrong Security Code.", MessageType.BlueNormal);
                Log.DebugFormat("Wrong Security Code: [{0}] <> [{1}], Player: {2}", securityCode, guildMaster.Account.SecurityCode, guildMaster.SelectedCharacter.Name);

                guildMaster.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            if (guildMaster.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                Log.WarnFormat("Suspicious request for player with name: {0} (player is not a guild master), could be hack attempt.", guildMaster.Name);
                guildMaster.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            if (nickname == guildMaster.SelectedCharacter.Name)
            {
                var guildId = guildMaster.GuildStatus.GuildId;
                this.gameContext.GuildServer.KickMember(guildId, nickname);
                this.gameContext.GuildCache.Invalidate(guildId);
                guildMaster.GuildStatus = null;
                guildMaster.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.GuildDisband);
                return;
            }

            this.gameContext.GuildServer.KickMember(guildMaster.GuildStatus.GuildId, nickname);
        }
    }
}
