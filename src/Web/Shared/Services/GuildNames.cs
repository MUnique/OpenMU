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
    /// Resolves the display data of the given guilds: one lookup per distinct
    /// guild instead of one per row.
    /// </summary>
    /// <param name="guildServer">The guild server, if available (all-in-one deployment only).</param>
    /// <param name="guildIds">The guild identifiers to resolve.</param>
    /// <returns>The display data by guild identifier; unknown or failed lookups are absent.</returns>
    public static async Task<Dictionary<uint, GuildInfo>> ResolveAsync(IGuildServer? guildServer, IEnumerable<uint> guildIds)
    {
        var result = new Dictionary<uint, GuildInfo>();
        if (guildServer is null)
        {
            return result;
        }

        foreach (var guildId in guildIds.Distinct())
        {
            string? name = null;
            Guid? persistentId = null;
            try
            {
                name = (await guildServer.GetGuildAsync(guildId).ConfigureAwait(false))?.Name;
            }
            catch (Exception)
            {
                // A single failed lookup resolves to "no guild" (see above).
            }

            try
            {
                var resolvedId = await guildServer.GetPersistentGuildIdAsync(guildId).ConfigureAwait(false);
                persistentId = resolvedId is { } guid && guid != Guid.Empty ? guid : null;
            }
            catch (Exception)
            {
                // A single failed lookup resolves to "no guild" (see above).
            }

            if (name is not null || persistentId is not null)
            {
                result[guildId] = new GuildInfo(name, persistentId);
            }
        }

        return result;
    }

    /// <summary>
    /// Display data of a guild for the online-accounts tables.
    /// </summary>
    /// <param name="Name">The name of the guild, if it could be resolved.</param>
    /// <param name="PersistentId">The persistent identifier of the guild, for linking to the guild page. Null when unknown.</param>
    public sealed record GuildInfo(string? Name, Guid? PersistentId);
}
