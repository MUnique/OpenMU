// <copyright file="EntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A implementation of <see cref="EntityFrameworkContextBase"/> which doesn't cache and always asks the database for objects.
    /// </summary>
    public class EntityFrameworkContext : EntityFrameworkContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EntityFrameworkContext(DbContext context)
            : base (context, new RepositoryManager(), true)
        {
            this.RepositoryManager.RegisterRepositories();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkContext" /> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="isOwner">If set to <c>true</c>, this instance owns the <see cref="EntityFrameworkContextBase.Context" />. That means it will be disposed when this instance will be disposed.</param>
        public EntityFrameworkContext(DbContext context, RepositoryManager repositoryManager, bool isOwner)
            : base(context, repositoryManager, isOwner)
        {
        }
    }
}
