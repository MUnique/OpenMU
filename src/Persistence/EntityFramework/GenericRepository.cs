// <copyright file="GenericRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    /// <summary>
    /// A generic repository which wraps the access to the DBSet of the <see cref="EntityDataContext"/>.
    /// Entities are getting eagerly (=completely) loaded automatically.
    /// </summary>
    /// <typeparam name="T">The type which this repository should manage.</typeparam>
    internal class GenericRepository<T> : GenericRepositoryBase<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public GenericRepository(RepositoryManager repositoryManager)
            : base(repositoryManager)
        {
        }

        /// <summary>
        /// Gets a context to work with. If no context is currently registered at the repository manager, a new one is getting created.
        /// </summary>
        /// <returns>The context.</returns>
        protected override EntityFrameworkContextBase GetContext()
        {
            var context = this.RepositoryManager.ContextStack.GetCurrentContext() as EntityFrameworkContext;

            return new EntityFrameworkContext(context?.Context ?? new TypedContext<T>(), this.RepositoryManager, context == null);
        }
    }
}
