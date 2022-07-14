// <copyright file="GuildServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
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
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="logger">The logger.</param>
    public GuildServerContext(GuildContext guildContext, RepositoryManager repositoryManager, ILogger<GuildServerContext> logger)
        : base(guildContext, repositoryManager, logger)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<bool> GuildWithNameExistsAsync(string name)
    {
        return await this.Context.Set<Guild>().AnyAsync(guild => guild.Name == name);
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyDictionary<Guid, string>> GetMemberNamesAsync(Guid guildId)
    {
        return await (from member in this.Context.Set<GuildMember>()
            join character in this.Context.Set<CharacterName>() on member.Id equals character.Id
            where member.GuildId == guildId
            select new { character.Id, character.Name })
            .ToDictionaryAsync(member => member.Id, member => member.Name);
    }
}