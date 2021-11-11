﻿// <copyright file="GuildServerInMemoryContext.cs" company="MUnique">
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
    /// <param name="manager">The manager which holds the memory repositories.</param>
    public GuildServerInMemoryContext(InMemoryRepositoryManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc/>
    public bool GuildWithNameExists(string name)
    {
        return this.Manager.GetRepository<DataModel.Entities.Guild>().GetAll().Any(g => g.Name == name);
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<Guid, string> GetMemberNames(Guid guildId)
    {
        var members = this.Manager.GetRepository<GuildMember>().GetAll().Where(member => member.GuildId == guildId);
        var characters = this.Manager.GetRepository<Character>().GetAll();
        return members
            .Select(m => (m.Id, Name: characters.FirstOrDefault(c => c.Id == m.Id)?.Name!))
            .Where(m => m.Name is not null)
            .ToDictionary(m => m.Id, m => m.Name);
    }
}