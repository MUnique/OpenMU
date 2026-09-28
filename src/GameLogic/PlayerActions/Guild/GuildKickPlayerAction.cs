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

        // Same fallback as DeleteCharacterAction: an account with no security code ever set (the
        // default, "" - there is no in-game or web flow to configure one) is checked against the
        // account password instead. Without this fallback, "" != null is always true, so the check
        // below would always run against an empty string that no legitimate input can match, and a
        // guild could never be kicked from - or disbanded, since a self-kick by the guild master is
        // how disbanding works below.
        var checkAsPassword = string.IsNullOrEmpty(player.Account!.SecurityCode);
        var securityCodeIsWrong = checkAsPassword
            ? !BCrypt.Net.BCrypt.Verify(securityCode, player.Account.PasswordHash)
            : player.Account.SecurityCode != securityCode;
        if (securityCodeIsWrong)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.WrongSecurityCode)).ConfigureAwait(false);
            player.Logger.LogDebug("Wrong Security Code, Player: {0}", player.SelectedCharacter?.Name);

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