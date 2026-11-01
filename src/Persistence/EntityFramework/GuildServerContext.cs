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
    public async ValueTask<IReadOnlyDictionary<Guid, string>> GetGuildMasterNamesAsync(CancellationToken cancellationToken = default)
    {
        var masters = await (from member in this.Context.Set<GuildMember>()
                             join character in this.Context.Set<CharacterName>() on member.Id equals character.Id
                             where member.Status == MUnique.OpenMU.Interfaces.GuildPosition.GuildMaster
                             select new { member.GuildId, character.Name })
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        // A guild has one master, but the data isn't constrained to it.
        return masters
            .GroupBy(master => master.GuildId)
            .ToDictionary(group => group.Key, group => group.First().Name);
    }

    /// <inheritdoc/>
    public async ValueTask<GuildMembership?> GetGuildMembershipAsync(string characterName)
    {
        return await CreateGuildMembershipQuery(this.Context, characterName).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the query for the guild membership of a character.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="characterName">The name of the character.</param>
    /// <returns>The query.</returns>
    internal static IQueryable<GuildMembership> CreateGuildMembershipQuery(DbContext context, string characterName)
    {
        return from member in context.Set<GuildMember>()
               join character in context.Set<CharacterName>() on member.Id equals character.Id
               join guild in context.Set<Guild>() on member.GuildId equals guild.Id
               where character.Name == characterName
               select new GuildMembership(member.Id, member.GuildId, member.Status, guild.AllianceGuildId);
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
            .Include(g => g.RawHostility)
            .Where(g => g.Name != null && g.Name.ToLower().Contains(term))
            .OrderBy(g => g.Name)
            .Skip(skip)
            .Take(count)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<int> CountGuildsAsync(string text, CancellationToken cancellationToken = default)
    {
        var term = text.ToLowerInvariant();
        return await this.Context.Set<Guild>().AsNoTracking()
            .CountAsync(g => g.Name != null && g.Name.ToLower().Contains(term), cancellationToken).ConfigureAwait(false);
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

    /// <inheritdoc/>
    public async ValueTask<GuildStatistics> GetGuildStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var guilds = this.Context.Set<Guild>().AsNoTracking();
        var guildCount = await guilds.CountAsync(cancellationToken).ConfigureAwait(false);
        var allianceCount = await guilds
            .Where(g => g.AllianceGuildId != null)
            .Select(g => g.AllianceGuildId)
            .Distinct()
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);
        return new GuildStatistics(guildCount, allianceCount);
    }
}
