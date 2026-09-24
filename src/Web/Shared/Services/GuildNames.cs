// <copyright file="GuildNames.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Bulk resolution of guild names for the online-accounts tables: one lookup per distinct
/// guild instead of one per row. A display column must never fail the whole table because
/// a single guild lookup failed, so per-guild failures resolve to "no guild".
/// </summary>
public static class GuildNames
{
    /// <summary>
    /// Finds the guild server of the first in-process game server, if any.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    public static IGuildServer? FindServer(IServerProvider serverProvider)
        => serverProvider.Servers
            .OfType<IGameServerContextProvider>()
            .Select(s => s.Context.GuildServer)
            .FirstOrDefault(g => g is not null);

    /// <summary>
    /// Resolves the names of the given guilds.
    /// </summary>
    /// <param name="guildServer">The guild server, if available (all-in-one deployment only).</param>
    /// <param name="guildIds">The guild identifiers to resolve.</param>
    /// <returns>The names by guild identifier; unknown or failed lookups are absent.</returns>
    public static async Task<Dictionary<uint, string>> ResolveAsync(IGuildServer? guildServer, IEnumerable<uint> guildIds)
    {
        var result = new Dictionary<uint, string>();
        if (guildServer is null)
        {
            return result;
        }

        foreach (var guildId in guildIds.Distinct())
        {
            try
            {
                if (await guildServer.GetGuildAsync(guildId).ConfigureAwait(false) is { Name: { } name })
                {
                    result[guildId] = name;
                }
            }
            catch (Exception)
            {
                // A single failed lookup resolves to "no guild" (see above).
            }
        }

        return result;
    }

    /// <summary>
    /// Resolves the persistent identifiers of the given guilds. These are required to link
    /// to the guild detail page, which is addressed by persistent identifier.
    /// </summary>
    /// <param name="guildServer">The guild server, if available (all-in-one deployment only).</param>
    /// <param name="guildIds">The guild identifiers to resolve.</param>
    /// <returns>The persistent identifiers by guild identifier; unknown or failed lookups are absent.</returns>
    public static async Task<Dictionary<uint, Guid>> ResolvePersistentIdsAsync(IGuildServer? guildServer, IEnumerable<uint> guildIds)
    {
        var result = new Dictionary<uint, Guid>();
        if (guildServer is null)
        {
            return result;
        }

        foreach (var guildId in guildIds.Distinct())
        {
            try
            {
                if (await guildServer.GetPersistentGuildIdAsync(guildId).ConfigureAwait(false) is { } persistentId
                    && persistentId != Guid.Empty)
                {
                    result[guildId] = persistentId;
                }
            }
            catch (Exception)
            {
                // A single failed lookup resolves to "no guild" (see above).
            }
        }

        return result;
    }
}
