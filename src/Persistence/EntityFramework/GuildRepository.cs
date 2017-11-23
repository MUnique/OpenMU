// <copyright file="GuildRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Linq;

    /// <summary>
    /// Repository for instances of <see cref="Guild"/>.
    /// </summary>
    internal class GuildRepository : GenericRepository<Guild>, IGuildRepository<Guild>
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
            using (var context = new EntityDataContext())
            {
                return context.Set<Guild>().Any(guild => guild.Name == name);
            }
        }
    }
}
