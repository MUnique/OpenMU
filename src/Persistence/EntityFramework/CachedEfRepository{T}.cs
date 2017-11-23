// <copyright file="CachedEfRepository{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// A repository which caches data to minimize database queries.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    internal class CachedEfRepository<T> : CachedRepository<T>, ILoadByProperty, INotifyAddedObject
            where T : class, IIdentifiable
    {
        private readonly IDictionary<ForeignCacheKey, ISet<T>> byForeignKeyCache = new Dictionary<ForeignCacheKey, ISet<T>>();
        private readonly RepositoryManager repositoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedEfRepository{T}" /> class.
        /// </summary>
        /// <param name="baseRepository">The base repository.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        public CachedEfRepository(GenericRepository<T> baseRepository, RepositoryManager repositoryManager)
            : base(baseRepository)
        {
            this.repositoryManager = repositoryManager;
        }

        /// <inheritdoc/>
        public IEnumerable LoadByProperty(IProperty property, object propertyValue)
        {
            this.GetAll();
            Guid resultKey = (Guid)propertyValue;
            if (this.byForeignKeyCache.TryGetValue(this.GetForeignKeyCacheKey(property.Name, resultKey), out ISet<T> objList))
            {
                return objList;
            }

            return Enumerable.Empty<T>();
        }

        /// <inheritdoc />
        public void ObjectAdded(object addedObject)
        {
            var objectAsT = addedObject as T;
            if (objectAsT == null)
            {
                throw new ArgumentException($"addedObject is not of type {typeof(T)}");
            }

            this.AddToCache(objectAsT.Id, objectAsT);
        }

        /// <inheritdoc/>
        protected override void AddToCache(Guid id, T obj)
        {
            using (var context = this.GetContext())
            {
                var entityType = context.Context.Model.FindEntityType(typeof(T).FullName);
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    this.AddToCacheByForeignKey(obj, context, foreignKey);
                }
            }

            base.AddToCache(id, obj);
        }

        /// <summary>
        /// Gets a context to work with. If no context is currently registered at the repository manager, a new one is getting created.
        /// </summary>
        /// <returns>The context.</returns>
        protected virtual EntityFrameworkContext GetContext()
        {
            var context = this.repositoryManager.GetCurrentContext() as EntityFrameworkContext;

            return new EntityFrameworkContext(context?.Context ?? new EntityDataContext(), context == null);
        }

        /// <inheritdoc/>
        protected override void RemoveFromCache(Guid id)
        {
            var obj = this.GetFromCache(id);
            if (obj == null)
            {
                return;
            }

            if (this.repositoryManager.GetCurrentContext() is EntityFrameworkContext context)
            {
                var entityType = context.Context.Model.FindEntityType(typeof(T).FullName);
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    this.RemoveFromCacheByForeignKey(obj, context, foreignKey);
                }
            }

            base.RemoveFromCache(id);
        }

        private void AddToCacheByForeignKey(T obj, EntityFrameworkContext context, IForeignKey foreignKey)
        {
            var foreignKeyProperty = foreignKey.Properties.First();
            var foreignKeyValue = this.GetPropertyValue(obj, context, foreignKeyProperty);
            if (foreignKeyValue.HasValue)
            {
                var cacheKey = this.GetForeignKeyCacheKey(foreignKeyProperty.Name, foreignKeyValue.Value);
                if (!this.byForeignKeyCache.TryGetValue(cacheKey, out ISet<T> objList))
                {
                    objList = new HashSet<T>();
                    this.byForeignKeyCache.Add(cacheKey, objList);
                }

                if (!objList.Contains(obj))
                {
                    objList.Add(obj);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ForeignCacheKey GetForeignKeyCacheKey(string propertyName, Guid foreignKeyValue)
        {
            return new ForeignCacheKey(propertyName, foreignKeyValue);
        }

        private void RemoveFromCacheByForeignKey(T obj, EntityFrameworkContext context, IForeignKey foreignKey)
        {
            var foreignKeyProperty = foreignKey.Properties.First();
            var foreignKeyValue = this.GetPropertyValue(obj, context, foreignKeyProperty);
            if (foreignKeyValue.HasValue)
            {
                if (this.byForeignKeyCache.TryGetValue(this.GetForeignKeyCacheKey(foreignKeyProperty.Name, foreignKeyValue.Value), out ISet<T> objList))
                {
                    objList.Remove(obj);
                }
            }
        }

        private Guid? GetPropertyValue(T obj, EntityFrameworkContext context, IProperty property)
        {
            var entry = context.Context.Entry(obj);
            return (Guid?)entry.Property(property.Name).CurrentValue;
        }

        private struct ForeignCacheKey
        {
            private readonly string propertyName;

            private readonly Guid id;

            public ForeignCacheKey(string propertyName, Guid id)
            {
                this.propertyName = propertyName;
                this.id = id;
            }

            public override int GetHashCode()
            {
                return this.propertyName.GetHashCode() + this.id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ForeignCacheKey))
                {
                    return false;
                }

                var other = (ForeignCacheKey)obj;
                return this.propertyName == other.propertyName && this.id == other.id;
            }
        }
    }
}
