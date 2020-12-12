// <copyright file="CachedRepository{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A repository which caches all of its data in memory.
    /// </summary>
    /// <typeparam name="T">The type of the business object.</typeparam>
    public class CachedRepository<T> : IRepository<T>
        where T : class, IIdentifiable
    {
        private readonly IDictionary<Guid, T> cache;

        private bool allLoaded;
        private bool loading;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRepository{T}"/> class.
        /// </summary>
        /// <param name="baseRepository">The base repository.</param>
        public CachedRepository(IRepository<T> baseRepository)
        {
            this.BaseRepository = baseRepository;

            this.cache = new Dictionary<Guid, T>();
        }

        /// <summary>
        /// Gets the underlying base repository.
        /// </summary>
        protected IRepository<T> BaseRepository { get; }

        /// <inheritdoc/>
        public IEnumerable<T> GetAll()
        {
            if (this.allLoaded || this.loading)
            {
                return this.cache.Values;
            }

            this.loading = true;
            IEnumerable<T> values = this.BaseRepository.GetAll();

            foreach (var obj in values)
            {
                if (!this.cache.ContainsKey(obj.Id))
                {
                    this.AddToCache(obj.Id, obj);
                }
            }

            this.loading = false;
            this.allLoaded = true;

            return this.cache.Values;
        }

        /// <inheritdoc/>
        public T? GetById(Guid id)
        {
            this.GetAll();
            this.cache.TryGetValue(id, out var result);
            return result;
        }

        /// <inheritdoc/>
        object? IRepository.GetById(Guid id)
        {
            return this.GetById(id);
        }

        /// <inheritdoc/>
        public bool Delete(object obj)
        {
            if (obj is IIdentifiable identifiable)
            {
                return this.Delete(identifiable.Id);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            if (this.BaseRepository.Delete(id))
            {
                this.RemoveFromCache(id);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the object to the cache.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="obj">The object.</param>
        protected virtual void AddToCache(Guid id, T obj)
        {
            if (this.cache.ContainsKey(id))
            {
                if (object.Equals(this.cache[id], obj))
                {
                     throw new ArgumentException("Other object with same id is already in cache.");
                }
            }
            else
            {
                this.cache.Add(id, obj);
            }
        }

        /// <summary>
        /// Removes the object from cache.
        /// </summary>
        /// <param name="id">The identifier.</param>
        protected virtual void RemoveFromCache(Guid id)
        {
            this.cache.Remove(id);
        }
    }
}
