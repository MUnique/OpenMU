// <copyright file="RepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Linq;

    /// <summary>
    /// A repository manager which does not use caching.
    /// </summary>
    public class RepositoryManager : BaseRepositoryManager
    {
        /// <summary>
        /// Gets the context stack. When loading an object, the current context should be pushed onto the stack.
        /// </summary>
        internal IContextStack ContextStack { get; } = new ContextStack();

        /// <summary>
        /// Registers the repositories.
        /// </summary>
        public virtual void RegisterRepositories()
        {
            this.RegisterRepository(new GameConfigurationRepository(this));
            this.RegisterMissingRepositoriesAsGeneric();
        }

        /// <summary>
        /// Registers generic repositories for all types of the data model which are not registered otherwise.
        /// </summary>
        protected void RegisterMissingRepositoriesAsGeneric()
        {
            var registeredTypes = this.Repositories.Keys.ToList();
            using var entityContext = new EntityDataContext();
            var modelTypes = entityContext.Model.GetEntityTypes().Select(e => e.ClrType);
            var missingTypes = modelTypes.Where(t => !registeredTypes.Contains(t.BaseType));
            foreach (var type in missingTypes)
            {
                var repository = this.CreateGenericRepository(type);
                this.RegisterRepository(type, repository);
            }
        }

        /// <summary>
        /// Creates the generic repository for the specified type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The created repository.</returns>
        protected virtual IRepository CreateGenericRepository(Type entityType)
        {
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(entityType);
            return (IRepository)Activator.CreateInstance(repositoryType, this);
        }

        /// <summary>
        /// Registers the repository. Adapts the type, so that the base type gets registered.
        /// </summary>
        /// <param name="type">The generic type which the repository handles.</param>
        /// <param name="repository">The repository.</param>
        protected override void RegisterRepository(Type type, IRepository repository)
        {
            if (type.Namespace == this.GetType().Namespace && type.BaseType != typeof(object))
            {
                base.RegisterRepository(type.BaseType, repository);
            }
            else
            {
                base.RegisterRepository(type, repository);
            }
        }
    }
}