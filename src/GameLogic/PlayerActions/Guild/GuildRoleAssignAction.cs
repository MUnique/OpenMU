// <copyright file="GuildRoleAssignAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to assign a role (assistant master, battle master, normal member) to a guild member.
/// Only the guild master may assign roles. Leadership transfer is not supported here.
/// </summary>
public class GuildRoleAssignAction
{
    /// <summary>
    /// Assigns the specified role to the guild member with the specified nickname.
    /// </summary>
    /// <param name="player">The requesting player. Must be the guild master.</param>
    /// <param name="nickname">The nickname of the target guild member. Must be online on the same game server and in the same guild.</param>
    /// <param name="newPosition">The new position. Only <see cref="GuildPosition.NormalMember"/>, <see cref="GuildPosition.BattleMaster"/> and <see cref="GuildPosition.AssistantMaster"/> are accepted.</param>
    /// <remarks>
    /// Failures are only logged; no dedicated client response packet exists for role assignment.
    /// On success, the guild server publishes the change which updates the target's guild status and views.
    /// </remarks>
    public async ValueTask AssignRoleAsync(Player player, string nickname, GuildPosition newPosition)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            player.Logger.LogError($"Account {player.Account?.LoginName} not in the right state, but {player.PlayerState.CurrentState}.");
            return;
        }

        var guildStatus = player.GuildStatus;
        if (guildStatus is null)
        {
            player.Logger.LogError($"Player {player} not in a guild.");
            return;
        }

        // The fixed-size name field may be space-padded by the client.
        var targetName = nickname.Trim();
        if (string.IsNullOrEmpty(targetName))
        {
            player.Logger.LogWarning("Rejected guild role assignment of player {PlayerName}: empty target name.", player.Name);
            return;
        }

        if (newPosition is not (GuildPosition.NormalMember or GuildPosition.BattleMaster or GuildPosition.AssistantMaster))
        {
            player.Logger.LogWarning("Rejected guild role assignment of player {PlayerName} to {TargetName}: invalid position {Position}.", player.Name, targetName, newPosition);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            player.Logger.LogWarning("Suspicious role assign request for player with name: {PlayerName} (player is not a guild master) to assign {TargetName}.", player.Name, targetName);
            return;
        }

        var guildServer = (player.GameContext as IGameServerContext)?.GuildServer;
        if (guildServer is null)
        {
            player.Logger.LogWarning("No guild server available");
            return;
        }

        var target = player.GameContext.GetPlayerByCharacterName(targetName);
        if (target?.SelectedCharacter is null)
        {
            player.Logger.LogWarning("Rejected guild role assignment: target {TargetName} is not online on this server.", targetName);
            return;
        }

        if (target.SelectedCharacter.Id == player.SelectedCharacter?.Id)
        {
            player.Logger.LogWarning("Rejected guild role assignment: guild master {PlayerName} cannot change its own role.", player.Name);
            return;
        }

        if (target.GuildStatus?.GuildId != guildStatus.GuildId)
        {
            player.Logger.LogWarning("Rejected guild role assignment: target {TargetName} is not in the same guild.", targetName);
            return;
        }

        if (target.GuildStatus.Position == GuildPosition.GuildMaster)
        {
            player.Logger.LogWarning("Rejected guild role assignment: target {TargetName} is the guild master, leadership transfer is not supported.", targetName);
            return;
        }

        if (target.GuildStatus.Position == newPosition)
        {
            player.Logger.LogDebug("Guild role assignment skipped: target {TargetName} already has position {Position}.", targetName, newPosition);
            return;
        }

        await guildServer.ChangeGuildMemberPositionAsync(guildStatus.GuildId, target.SelectedCharacter.Id, newPosition).ConfigureAwait(false);
    }
}
