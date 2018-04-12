// <copyright file="GuildRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Repository for instances of <see cref="Guild"/>.
    /// </summary>
    internal class GuildRepository : GenericRepository<Guild>, IGuildRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuildRepository"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public GuildRepository(IRepositoryManager manager)
            : base(manager)
        {
        }

        /// <inheritdoc/>
        public bool GuildWithNameExists(string name)
        {
            using (var context = new GuildContext())
            {
                return context.Set<Guild>().Any(guild => guild.Name == name);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<Guid, string> GetMemberNames(Guid guildId)
        {
            using (var context = new GuildContext())
            {
                return (from member in context.Set<GuildMember>()
                    join character in context.Set<Character>() on member.Id equals character.Id
                    where member.GuildId == guildId
                    select new { character.Id, character.Name }).ToDictionary(member => member.Id, member => member.Name);
            }
        }
    }
}
