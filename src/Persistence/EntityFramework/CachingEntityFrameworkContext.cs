// <copyright file="CachingEntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Implementation of <see cref="IContext"/> for the entity framework <see cref="PersistenceContextProvider"/>.
    /// </summary>
    internal class CachingEntityFrameworkContext : EntityFrameworkContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingEntityFrameworkContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        public CachingEntityFrameworkContext(DbContext context, RepositoryManager repositoryManager)
            : base(context, repositoryManager, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingEntityFrameworkContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryManager">The repsoitory manager.</param>
        /// <param name="isOwner">if set to <c>true</c> this instance owns the <paramref name="context" />.</param>
        public CachingEntityFrameworkContext(DbContext context, RepositoryManager repositoryManager, bool isOwner)
            : base(context, repositoryManager, isOwner)
        {
        }
    }
}
