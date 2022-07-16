// <copyright file="GuildKickPlayerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to kick a player out of a guild.
/// </summary>
public class GuildKickPlayerAction
{
    /// <summary>
    /// Kicks the player out of the guild.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="nickname">The nickname.</param>
    /// <param name="securityCode">The security code.</param>
    public async ValueTask KickPlayerAsync(Player player, string nickname, string securityCode)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            player.Logger.LogError($"Account {player.Account?.LoginName} not in the right state, but {player.PlayerState.CurrentState}.");
            return;
        }

        if (player.GuildStatus is null)
        {
            player.Logger.LogError($"Player {player} not in a guild.");
            return;
        }

        var guildServer = (player.GameContext as IGameServerContext)?.GuildServer;
        if (guildServer is null)
        {
            player.Logger.LogWarning("No guild server available");
            return;
        }

        if (player.Account!.SecurityCode != null && player.Account.SecurityCode != securityCode)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Wrong Security Code.", MessageType.BlueNormal)).ConfigureAwait(false);
            player.Logger.LogDebug("Wrong Security Code: [{0}] <> [{1}], Player: {2}", securityCode, player.Account.SecurityCode, player.SelectedCharacter?.Name);

            await player.InvokeViewPlugInAsync<IGuildKickResultPlugIn>(p => p.GuildKickResultAsync(GuildKickSuccess.Failed)).ConfigureAwait(false);
            return;
        }

        var isKickingHimself = player.SelectedCharacter!.Name == nickname;
        if (!isKickingHimself && player.GuildStatus?.Position != GuildPosition.GuildMaster)
        {
            player.Logger.LogWarning("Suspicious kick request for player with name: {0} (player is not a guild master) to kick {1}, could be hack attempt.", player.Name, nickname);
            await player.InvokeViewPlugInAsync<IGuildKickResultPlugIn>(p => p.GuildKickResultAsync(GuildKickSuccess.FailedBecausePlayerIsNotGuildMaster)).ConfigureAwait(false);
            return;
        }

        if (isKickingHimself && player.GuildStatus?.Position == GuildPosition.GuildMaster)
        {
            var guildId = player.GuildStatus.GuildId;
            await player.InvokeViewPlugInAsync<IGuildKickResultPlugIn>(p => p.GuildKickResultAsync(GuildKickSuccess.GuildDisband)).ConfigureAwait(false);
            await guildServer.KickMemberAsync(guildId, nickname).ConfigureAwait(false);
            return;
        }

        await guildServer.KickMemberAsync(player.GuildStatus!.GuildId, nickname).ConfigureAwait(false);
    }
}