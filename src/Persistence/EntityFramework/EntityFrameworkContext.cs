// <copyright file="EntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Implementation of <see cref="IContext"/> for the entity framework <see cref="PersistenceContextProvider"/>.
    /// </summary>
    public class EntityFrameworkContext : IContext
    {
        [Obsolete]
        private readonly bool isOwner;
        private readonly PersistenceContextProvider contextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contextProvider">The contextProvider.</param>
        /// <param name="isOwner">if set to <c>true</c> this instance owns the <paramref name="context" />.</param>
        public EntityFrameworkContext(DbContext context, PersistenceContextProvider contextProvider, bool isOwner = true)
        {
            this.Context = context;
            this.contextProvider = contextProvider;
            this.isOwner = isOwner;
        }

        /// <summary>
        /// Gets the entity framework context.
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        /// Gets the context provider.
        /// </summary>
        /// <value>
        /// The context provider.
        /// </value>
        protected PersistenceContextProvider ContextProvider => this.contextProvider;

        /// <inheritdoc/>
        public bool SaveChanges()
        {
            this.Context.SaveChanges();
            return true;
        }

        /// <inheritdoc />
        public void Detach(object item)
        {
            var entry = this.Context.Entry(item);
            if (entry == null)
            {
                return;
            }

            entry.State = EntityState.Detached;
        }

        /// <inheritdoc />
        public void Attach(object item)
        {
            this.Context.Attach(item);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.Context != null && this.isOwner)
            {
                this.Context.Dispose();
            }

            this.Context = null;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type which should get created.</typeparam>
        /// <param name="args">The arguments which are handed 1-to-1 to the constructor. If no arguments are given, the default constructor will be called.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T" />.
        /// </returns>
        public T CreateNew<T>(params object[] args)
            where T : class
        {
            var instance = TypeHelper.CreateNew<T>(args);
            if (instance != null)
            {
                this.Context.Add(instance);
            }

            return instance;
        }

        /// <inheritdoc/>
        public bool Delete<T>(T obj)
            where T : class
        {
            return this.Context.Remove(obj) != null;
        }

        /// <inheritdoc/>
        public T GetById<T>(Guid id)
            where T : class
        {
            using (this.contextProvider.UseContext(this))
            {
                var repository = this.contextProvider.RepositoryManager.GetRepository<T>();
                return repository.GetById(id);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<T> Get<T>()
            where T : class
        {
            using (this.contextProvider.UseContext(this))
            {
                var repository = this.contextProvider.RepositoryManager.GetRepository<T>();
                return repository.GetAll();
            }
        }
    }
}
