// <copyright file="GuildRoleAssignAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using System.Collections.Immutable;
using MUnique.OpenMU.GameLogic.Attributes;
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
    /// <param name="nickname">The nickname of the target guild member. The target may be online on any game server or offline.</param>
    /// <param name="newPosition">The new position. Only <see cref="GuildPosition.NormalMember"/>, <see cref="GuildPosition.BattleMaster"/> and <see cref="GuildPosition.AssistantMaster"/> are accepted.</param>
    /// <param name="enforceLimits">If set to <c>true</c> (default), the role limits of the official server are enforced:
    /// at most one assistant master, and battle masters limited by the guild master's level. Demotions are never limited.</param>
    /// <remarks>
    /// Failures are only logged; no dedicated client response packet exists for role assignment.
    /// On success, the guild server publishes the change which updates the target's guild status and views.
    /// </remarks>
    public async ValueTask AssignRoleAsync(Player player, string nickname, GuildPosition newPosition, bool enforceLimits = true)
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

        if (string.Equals(player.SelectedCharacter?.Name, targetName, StringComparison.OrdinalIgnoreCase))
        {
            player.Logger.LogWarning("Rejected guild role assignment: guild master {PlayerName} cannot change its own role.", player.Name);
            return;
        }

        var guildServer = (player.GameContext as IGameServerContext)?.GuildServer;
        if (guildServer is null)
        {
            player.Logger.LogWarning("No guild server available");
            return;
        }

        // Resolve through the guild list, which covers members on other game servers and offline members.
        var members = await guildServer.GetGuildListAsync(guildStatus.GuildId).ConfigureAwait(false);
        var target = members.FirstOrDefault(m => string.Equals(m.PlayerName, targetName, StringComparison.OrdinalIgnoreCase));
        if (target?.PlayerName is null)
        {
            player.Logger.LogWarning("Rejected guild role assignment: target {TargetName} is not in the same guild.", targetName);
            return;
        }

        if (target.PlayerPosition == GuildPosition.GuildMaster)
        {
            player.Logger.LogWarning("Rejected guild role assignment: target {TargetName} is the guild master, leadership transfer is not supported.", targetName);
            return;
        }

        if (target.PlayerPosition == newPosition)
        {
            player.Logger.LogDebug("Guild role assignment skipped: target {TargetName} already has position {Position}.", targetName, newPosition);
            return;
        }

        if (enforceLimits && !this.ValidateRoleLimits(player, members, target, targetName, newPosition))
        {
            return;
        }

        if (!await guildServer.ChangeGuildMemberPositionByNameAsync(guildStatus.GuildId, target.PlayerName, newPosition).ConfigureAwait(false))
        {
            player.Logger.LogWarning("Rejected guild role assignment: target {TargetName} could not be updated.", targetName);
        }
    }

    /// <summary>
    /// Gets the maximum number of battle masters for the given combined level of the guild master.
    /// Mirrors the official server: <c>(level / 200) + 1</c> with integer division.
    /// </summary>
    /// <param name="masterTotalLevel">The combined normal and master level of the guild master.</param>
    /// <returns>The maximum number of battle masters.</returns>
    private static int GetMaxBattleMasterCount(int masterTotalLevel)
    {
        return (masterTotalLevel / 200) + 1;
    }

    /// <summary>
    /// Validates the role limits of the official server: at most one assistant master, and a
    /// battle master count below the limit derived from the guild master's level.
    /// Demotions are never limited.
    /// </summary>
    /// <param name="player">The requesting guild master.</param>
    /// <param name="members">The current guild member list.</param>
    /// <param name="target">The targeted member entry.</param>
    /// <param name="targetName">The nickname used in log messages.</param>
    /// <param name="newPosition">The requested position.</param>
    /// <returns><c>true</c> if the limits allow the assignment; otherwise, <c>false</c>.</returns>
    private bool ValidateRoleLimits(Player player, IImmutableList<GuildListEntry> members, GuildListEntry target, string targetName, GuildPosition newPosition)
    {
        if (newPosition == GuildPosition.AssistantMaster
            && members.Any(m => m.PlayerPosition == GuildPosition.AssistantMaster && !string.Equals(m.PlayerName, target.PlayerName, StringComparison.OrdinalIgnoreCase)))
        {
            player.Logger.LogWarning("Rejected guild role assignment: guild already has an assistant master, target {TargetName}.", targetName);
            return false;
        }

        if (newPosition == GuildPosition.BattleMaster)
        {
            var battleMasterCount = members.Count(m => m.PlayerPosition == GuildPosition.BattleMaster);
            var maxBattleMasters = GetMaxBattleMasterCount(player.Level + (int)(player.Attributes?[Stats.MasterLevel] ?? 0));
            if (battleMasterCount >= maxBattleMasters)
            {
                player.Logger.LogWarning("Rejected guild role assignment: guild already has {Count} of {Max} battle masters, target {TargetName}.", battleMasterCount, maxBattleMasters, targetName);
                return false;
            }
        }

        return true;
    }
}
