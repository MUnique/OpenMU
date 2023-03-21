// <copyright file="GuildServerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

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
}