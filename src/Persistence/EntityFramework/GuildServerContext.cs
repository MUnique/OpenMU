// <copyright file="GuildServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// The EF Core implementation of a context which is used by the guild server.
/// </summary>
internal class GuildServerContext : CachingEntityFrameworkContext, IGuildServerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuildServerContext" /> class.
    /// </summary>
    /// <param name="guildContext">The guild context.</param>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="logger">The logger.</param>
    public GuildServerContext(GuildContext guildContext, IContextAwareRepositoryProvider repositoryProvider, ILogger<GuildServerContext> logger)
        : base(guildContext, repositoryProvider, null, logger)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<bool> GuildWithNameExistsAsync(string name)
    {
        return await this.Context.Set<Guild>().AnyAsync(guild => guild.Name == name).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<Guid?> GetPersistentGuildIdByNameAsync(string name)
    {
        return await this.Context.Set<Guild>()
            .Where(guild => guild.Name == name)
            .Select(guild => (Guid?)guild.Id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyDictionary<Guid, string>> GetPersistentGuildNamesAsync(IReadOnlyCollection<Guid> guildIds)
    {
        return await this.Context.Set<Guild>()
            .Where(guild => guildIds.Contains(guild.Id) && guild.Name != null)
            .Select(guild => new { guild.Id, guild.Name })
            .ToDictionaryAsync(guild => guild.Id, guild => guild.Name!)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyDictionary<Guid, string>> GetMemberNamesAsync(Guid guildId)
    {
        return await (from member in this.Context.Set<GuildMember>()
                      join character in this.Context.Set<CharacterName>() on member.Id equals character.Id
                      where member.GuildId == guildId
                      select new { character.Id, character.Name })
            .ToDictionaryAsync(member => member.Id, member => member.Name).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetAlliancesAsync(Guid allianceMasterId)
    {
        return await this.Context.Set<Guild>()
            .Where(g => g.AllianceGuildId == allianceMasterId)
            .Include(g => g.RawMembers)
            .ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetGuildsOrderedByNameAsync(int skip, int count, CancellationToken cancellationToken = default)
    {
        return await this.Context.Set<Guild>()
            .AsNoTracking()
            .Include(g => g.RawMembers)
            .Include(g => g.RawAllianceGuild)
            .OrderBy(g => g.Name)
            .Skip(skip)
            .Take(count)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DataModel.Entities.Guild>> SearchGuildsAsync(string searchTerm, int skip, int count, CancellationToken cancellationToken = default)
    {
        // Invariant: this runs in .NET, so it must not depend on the server's locale (see the
        // equivalent remark in PlayerContext.SearchAccountsAsync). The ToLower() calls below are
        // translated to the database's own lower(), which is why they cannot take a culture.
        var term = searchTerm.ToLowerInvariant();
        return await this.Context.Set<Guild>()
            .AsNoTracking()
            .Include(g => g.RawMembers)
            .Include(g => g.RawAllianceGuild)
            .Where(g => g.Name != null && g.Name.ToLower().Contains(term))
            .OrderBy(g => g.Name)
            .Skip(skip)
            .Take(count)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<Guid>> GetAllianceMasterIdsAsync(IReadOnlyCollection<Guid> guildIds)
    {
        if (guildIds.Count == 0)
        {
            return [];
        }

        return await this.Context.Set<Guild>()
            .AsNoTracking()
            .Where(g => g.AllianceGuildId != null && guildIds.Contains(g.AllianceGuildId!.Value))
            .Select(g => g.AllianceGuildId!.Value)
            .Distinct()
            .ToListAsync().ConfigureAwait(false);
    }
}
