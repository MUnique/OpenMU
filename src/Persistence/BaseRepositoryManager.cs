// <copyright file="BaseRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The base repository manager.
    /// </summary>
    public class BaseRepositoryManager
    {
        /// <summary>
        /// Gets the repositories for each entity type.
        /// </summary>
        protected IDictionary<Type, object> Repositories { get; } = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the repository of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <returns>The repository of the specified generic type.</returns>
        public virtual IRepository<T> GetRepository<T>()
        {
            return this.GetRepository(typeof(T)) as IRepository<T>;
        }

        /// <summary>
        /// Gets the repository of the specified type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The repository of the specified type.</returns>
        public virtual IRepository GetRepository(Type objectType)
        {
            var repository = this.InternalGetRepository(objectType);
            if (repository == null)
            {
                throw new RepositoryNotFoundException(objectType);
            }

            return repository;
        }

        /// <summary>
        /// Gets the repository of the specified type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The repository of the specified type.</returns>
        protected IRepository InternalGetRepository(Type objectType)
        {
            Type currentSearchType = objectType;
            do
            {
                if (currentSearchType == null)
                {
                    break;
                }

                if (this.Repositories.TryGetValue(currentSearchType, out object repository))
                {
                    return repository as IRepository;
                }

                currentSearchType = currentSearchType.BaseType;
            }
            while (currentSearchType != typeof(object));

            return null;
        }

        /// <summary>
        /// Registers the repository.
        /// </summary>
        /// <typeparam name="T">The generic type which the repository handles.</typeparam>
        /// <param name="repository">The repository.</param>
        protected void RegisterRepository<T>(IRepository<T> repository)
        {
            this.RegisterRepository(typeof(T), repository);
        }

        /// <summary>
        /// Registers the repository.
        /// </summary>
        /// <param name="type">The generic type which the repository handles.</param>
        /// <param name="repository">The repository.</param>
        protected virtual void RegisterRepository(Type type, IRepository repository)
        {
            this.Repositories.Add(type, repository);
        }
    }
}
