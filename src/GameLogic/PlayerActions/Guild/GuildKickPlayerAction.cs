// <copyright file="GuildKickPlayerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to kick a player out of a guild.
    /// </summary>
    public class GuildKickPlayerAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(GuildKickPlayerAction));

        /// <summary>
        /// Kicks the player out of the guild.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="nickname">The nickname.</param>
        /// <param name="securityCode">The security code.</param>
        public void KickPlayer(Player player, string nickname, string securityCode)
        {
            var guildServer = (player.GameContext as IGameServerContext)?.GuildServer;
            if (guildServer == null)
            {
                Log.Warn("No guild server available");
                return;
            }

            if (player.Account.SecurityCode != null && player.Account.SecurityCode != securityCode)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Wrong Security Code.", MessageType.BlueNormal);
                Log.DebugFormat("Wrong Security Code: [{0}] <> [{1}], Player: {2}", securityCode, player.Account.SecurityCode, player.SelectedCharacter.Name);

                player.ViewPlugIns.GetPlugIn<IGuildKickResultPlugIn>()?.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            var isKickingHimself = player.SelectedCharacter.Name == nickname;
            if (!isKickingHimself && player.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                Log.WarnFormat("Suspicious kick request for player with name: {0} (player is not a guild master) to kick {1}, could be hack attempt.", player.Name, nickname);
                player.ViewPlugIns.GetPlugIn<IGuildKickResultPlugIn>()?.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            if (isKickingHimself && player.GuildStatus?.Position == GuildPosition.GuildMaster)
            {
                var guildId = player.GuildStatus.GuildId;
                guildServer.KickMember(guildId, nickname);

                player.GuildStatus = null;
                player.ViewPlugIns.GetPlugIn<IGuildKickResultPlugIn>()?.GuildKickResult(GuildKickSuccess.GuildDisband);
                return;
            }

            guildServer.KickMember(player.GuildStatus.GuildId, nickname);
        }
    }
}
