// <copyright file="GuildServerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Threading;
using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// In-memory context implementation for <see cref="IGuildServerContext"/>.
/// </summary>
public class GuildServerInMemoryContext : InMemoryContext, IGuildServerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuildServerInMemoryContext"/> class.
    /// </summary>
    /// <param name="provider">The manager which holds the memory repositories.</param>
    public GuildServerInMemoryContext(InMemoryRepositoryProvider provider)
        : base(provider)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<bool> GuildWithNameExistsAsync(string name)
    {
        return (await this.Provider.GetRepository<DataModel.Entities.Guild>().GetAllAsync().ConfigureAwait(false)).Any(g => g.Name == name);
    }

    /// <inheritdoc/>
    public async ValueTask<Guid?> GetPersistentGuildIdByNameAsync(string name)
    {
        return (await this.Provider.GetRepository<DataModel.Entities.Guild>().GetAllAsync().ConfigureAwait(false))
            .FirstOrDefault(guild => guild.Name == name)
            ?.Id;
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyDictionary<Guid, string>> GetPersistentGuildNamesAsync(IReadOnlyCollection<Guid> guildIds)
    {
        var guildIdSet = guildIds.ToHashSet();
        return (await this.Provider.GetRepository<DataModel.Entities.Guild>().GetAllAsync().ConfigureAwait(false))
            .Where(guild => guildIdSet.Contains(guild.Id) && guild.Name is not null)
            .ToDictionary(guild => guild.Id, guild => guild.Name!);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyDictionary<Guid, string>> GetMemberNamesAsync(Guid guildId)
    {
        var members = (await this.Provider.GetRepository<GuildMember>().GetAllAsync().ConfigureAwait(false))
                                            .Where(member => member.GuildId == guildId);
        var characters = await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false);
        return members
            .Select(m => (m.Id, Name: characters.FirstOrDefault(c => c.Id == m.Id)?.Name!))
            .Where(m => m.Name is not null)
            .ToDictionary(m => m.Id, m => m.Name);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetAlliancesAsync(Guid guildId)
    {
        return (await this.Provider.GetRepository<DataModel.Entities.Guild>()
            .GetAllAsync()
            .ConfigureAwait(false))
            .Where(g => g.AllianceGuild?.GetId() == guildId)
            .ToList();
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetGuildsOrderedByNameAsync(int skip, int count, CancellationToken cancellationToken = default)
    {
        var allGuilds = await this.Provider.GetRepository<DataModel.Entities.Guild>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allGuilds.OrderBy(g => g.Name).Skip(skip).Take(count).ToList();
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DataModel.Entities.Guild>> SearchGuildsAsync(string searchTerm, int skip, int count, CancellationToken cancellationToken = default)
    {
        var allGuilds = await this.Provider.GetRepository<DataModel.Entities.Guild>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allGuilds
            .Where(g => g.Name?.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) == true)
            .OrderBy(g => g.Name)
            .Skip(skip)
            .Take(count)
            .ToList();
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<Guid>> GetAllianceMasterIdsAsync(IReadOnlyCollection<Guid> guildIds)
    {
        if (guildIds.Count == 0)
        {
            return [];
        }

        var guildIdSet = guildIds.ToHashSet();
        var allGuilds = await this.Provider.GetRepository<DataModel.Entities.Guild>().GetAllAsync().ConfigureAwait(false);
        return allGuilds
            .Where(g => g.AllianceGuild is { } master && guildIdSet.Contains(master.GetId()))
            .Select(g => g.AllianceGuild!.GetId())
            .Distinct()
            .ToList();
    }
}
