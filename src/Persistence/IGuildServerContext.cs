// <copyright file="IGuildServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Threading;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A context which is used by the <see cref="IGuildServer"/>.
/// </summary>
public interface IGuildServerContext : IContext
{
    /// <summary>
    /// Returns if the guild with the specified name exists.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>True, if the guild with the specified name exists.</returns>
    ValueTask<bool> GuildWithNameExistsAsync(string name);

    /// <summary>
    /// Gets the persistent identifier of the guild with the specified name.
    /// </summary>
    /// <param name="name">The guild name.</param>
    /// <returns>The guild identifier, or <see langword="null"/> when no guild has the name.</returns>
    ValueTask<Guid?> GetPersistentGuildIdByNameAsync(string name);

    /// <summary>
    /// Gets the canonical names of the guilds with the specified persistent identifiers.
    /// </summary>
    /// <param name="guildIds">The persistent guild identifiers.</param>
    /// <returns>The names keyed by persistent guild identifier. Missing guilds are omitted.</returns>
    ValueTask<IReadOnlyDictionary<Guid, string>> GetPersistentGuildNamesAsync(IReadOnlyCollection<Guid> guildIds);

    /// <summary>
    /// Gets the member names of a guild.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The member names of a guild.</returns>
    /// <remarks>Since names are stored in Character.Name and not duplicated.</remarks>
    ValueTask<IReadOnlyDictionary<Guid, string>> GetMemberNamesAsync(Guid guildId);

    /// <summary>
    /// Gets the alliances of a guild.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The ids of the alliances of a guild.</returns>
    ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetAlliancesAsync(Guid guildId);

    /// <summary>
    /// Gets a page of guilds, ordered by name, without loading the whole guild table into memory.
    /// </summary>
    /// <param name="skip">The number of guilds to skip.</param>
    /// <param name="count">The maximum number of guilds to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested page of guilds, including alliance and member information.</returns>
    ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetGuildsOrderedByNameAsync(int skip, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches guilds by name and returns a page of the matching results, without loading the whole guild table into memory.
    /// </summary>
    /// <param name="searchTerm">The case-insensitive search term which is matched against the guild name.</param>
    /// <param name="skip">The number of matching guilds to skip.</param>
    /// <param name="count">The maximum number of guilds to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested page of matching guilds, including alliance and member information.</returns>
    ValueTask<IReadOnlyList<DataModel.Entities.Guild>> SearchGuildsAsync(string searchTerm, int skip, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Of the given guild identifiers, returns the ones which are the master of an alliance
    /// (i.e. at least one other guild points to them as their <see cref="Interfaces.Guild.AllianceGuild"/>).
    /// </summary>
    /// <param name="guildIds">The guild identifiers to check. Kept small (e.g. one page) to avoid a full table scan.</param>
    /// <returns>The subset of <paramref name="guildIds"/> which are alliance masters.</returns>
    ValueTask<IReadOnlyCollection<Guid>> GetAllianceMasterIdsAsync(IReadOnlyCollection<Guid> guildIds);
}
