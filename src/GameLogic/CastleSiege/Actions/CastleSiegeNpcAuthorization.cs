// <copyright file="CastleSiegeNpcAuthorization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Authorization checks shared by Castle Siege NPC management actions.
/// </summary>
internal static class CastleSiegeNpcAuthorization
{
    /// <summary>
    /// Checks whether a player is a guild master or assistant master of the castle-owner guild.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the player is an authorized owner-guild officer.</returns>
    public static async ValueTask<bool> IsOwnerOfficerAsync(Player player, CastleSiegeContext context)
    {
        if (player.GuildStatus is not { } guildStatus
            || guildStatus.Position is not (GuildPosition.GuildMaster or GuildPosition.AssistantMaster)
            || player.GameContext is not IGameServerContext gameServerContext
            || await gameServerContext.GuildServer
                .GetPersistentGuildIdAsync(guildStatus.GuildId)
                .ConfigureAwait(false) is not { } persistentGuildId)
        {
            return false;
        }

        return context.SiegeData.OwnerGuildId == persistentGuildId;
    }

    /// <summary>
    /// Checks whether a player belongs to the castle-owner guild or one of its alliance guilds.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the player belongs to the owner alliance.</returns>
    public static async ValueTask<bool> IsOwnerAllianceMemberAsync(Player player, CastleSiegeContext context)
    {
        var guildId = await CastleSiegeGuildResolver.ResolveRegistrationGuildIdAsync(player).ConfigureAwait(false);
        return guildId is not null && context.SiegeData.OwnerGuildId == guildId;
    }
}
