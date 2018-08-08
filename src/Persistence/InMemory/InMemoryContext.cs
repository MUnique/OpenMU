// <copyright file="InMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Persistence.EntityFramework;

    /// <summary>
    /// An in-memory context which get it's data from the repositories of the <see cref="InMemoryPersistenceContextProvider"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.IContext" />
    public class InMemoryContext : IContext
    {
        /// <summary>
        /// The manager which holds the memory repositories.
        /// </summary>
        private readonly InMemoryRepositoryManager manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryContext"/> class.
        /// </summary>
        /// <param name="manager">The manager which holds the memory repositories.</param>
        public InMemoryContext(InMemoryRepositoryManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Gets the manager which holds the memory repositories.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        protected InMemoryRepositoryManager Manager => this.manager;

        /// <inheritdoc/>
        public void Dispose()
        {
            // nothing to do here
        }

        /// <inheritdoc/>
        public bool SaveChanges()
        {
            return true;
        }

        /// <inheritdoc/>
        public void Detach(object item)
        {
            if (item is IIdentifiable identifiable)
            {
                var repository = this.manager.GetRepository(item.GetType()) as IMemoryRepository;
                repository?.Remove(identifiable.Id);
            }
        }

        /// <inheritdoc/>
        public void Attach(object item)
        {
            if (item is IIdentifiable identifiable)
            {
                var repository = this.manager.GetRepository(item.GetType()) as IMemoryRepository;
                repository?.Add(identifiable.Id, item);
            }
        }

        /// <inheritdoc/>
        public T CreateNew<T>(params object[] args)
            where T : class
        {
            var newObject = TypeHelper.CreateNew<T>(args);
            if (newObject is IIdentifiable identifiable)
            {
                if (identifiable.Id == Guid.Empty)
                {
                    identifiable.Id = Guid.NewGuid();
                }

                var repository = this.manager.GetRepository<T>();
                repository?.Add(identifiable.Id, newObject);
            }

            return newObject;
        }

        /// <inheritdoc/>
        public bool Delete<T>(T obj)
            where T : class
        {
            return this.manager.GetRepository<T>()?.Delete(obj) ?? false;
        }

        /// <inheritdoc/>
        public T GetById<T>(Guid id)
            where T : class
        {
            return this.manager.GetRepository<T>().GetById(id);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Get<T>()
            where T : class
        {
            return this.manager.GetRepository<T>().GetAll();
        }
    }
}