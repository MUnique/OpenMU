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
}
