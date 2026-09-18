// <copyright file="CastleSiegeGuildResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Resolves runtime guild information to its persistent Castle Siege identity.
/// </summary>
internal static class CastleSiegeGuildResolver
{
    /// <summary>
    /// Resolves a guild which is allowed to mutate a Castle Siege registration.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <returns>The resolved guild and validation result.</returns>
    public static async ValueTask<(CastleSiegeGuildReference? Guild, CastleSiegeRegistrationResult Result)> ResolveAuthorizedGuildAsync(
        Player player)
    {
        if (player.GuildStatus is not { } guildStatus)
        {
            return (null, CastleSiegeRegistrationResult.NoGuild);
        }

        if (guildStatus.Position != GuildPosition.GuildMaster
            || player.GameContext is not IGameServerContext gameServerContext)
        {
            return (null, CastleSiegeRegistrationResult.InvalidGuild);
        }

        if (await gameServerContext.GuildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } guild)
        {
            return (null, CastleSiegeRegistrationResult.InvalidGuild);
        }

        if (guild.AllianceGuild is not null
            && !await gameServerContext.GuildServer.IsAllianceMasterAsync(guildStatus.GuildId).ConfigureAwait(false))
        {
            return (null, CastleSiegeRegistrationResult.InvalidGuild);
        }

        if (await gameServerContext.GuildServer.GetPersistentGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false) is not { } persistentGuildId)
        {
            return (null, CastleSiegeRegistrationResult.InvalidGuild);
        }

        return (new(guildStatus.GuildId, persistentGuildId, guild.Name), CastleSiegeRegistrationResult.Success);
    }

    /// <summary>
    /// Resolves the registration identity visible to any member of a guild or alliance.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <returns>The persistent registration guild identifier, or <see langword="null"/>.</returns>
    public static async ValueTask<Guid?> ResolveRegistrationGuildIdAsync(Player player)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext gameServerContext)
        {
            return null;
        }

        return await gameServerContext.GuildServer
            .GetPersistentAllianceMasterGuildIdAsync(guildStatus.GuildId)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Resolves the participating guild of a player who is the alliance master of their own guild.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>The participant entry, or <see langword="null"/> when the player is not an authorized alliance master of a currently participating guild.</returns>
    public static CastleSiegeGuildParticipant? ResolveParticipatingAllianceMaster(Player player, CastleSiegeContext context)
    {
        if (player.GuildStatus is not { Position: GuildPosition.GuildMaster } guildStatus
            || !context.FinalGuildList.TryGetValue(guildStatus.GuildId, out var participant)
            || !participant.IsAllianceMaster)
        {
            return null;
        }

        return participant;
    }
}
