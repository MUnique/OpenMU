// <copyright file="GuildServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        public GuildServerContext(GuildContext guildContext, RepositoryManager repositoryManager)
            : base(guildContext, repositoryManager)
        {
        }

        /// <inheritdoc/>
        public bool GuildWithNameExists(string name)
        {
            return this.Context.Set<Guild>().Any(guild => guild.Name == name);
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<Guid, string> GetMemberNames(Guid guildId)
        {
            return (from member in this.Context.Set<GuildMember>()
                join character in this.Context.Set<CharacterName>() on member.Id equals character.Id
                where member.GuildId == guildId
                select new { character.Id, character.Name }).ToDictionary(member => member.Id, member => member.Name);
        }
    }
}