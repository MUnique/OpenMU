// <copyright file="GenericRepositoryBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// Base class for a generic repository which wraps the access to the DBSet of the <see cref="EntityDataContext"/>.
    /// Entities are getting eagerly (=completely) loaded automatically.
    /// </summary>
    /// <typeparam name="T">The type which this repository should manage.</typeparam>
    internal abstract class GenericRepositoryBase<T> : IRepository<T>, ILoadByProperty
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepositoryBase{T}"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        protected GenericRepositoryBase(RepositoryManager repositoryManager)
        {
            this.RepositoryManager = repositoryManager;
            using var completeContext = new EntityDataContext();
            this.FullEntityType = completeContext.Model.FindEntityType(typeof(T));
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        protected RepositoryManager RepositoryManager { get; }

        /// <summary>
        /// Gets the complete meta model of the entity type in <see cref="EntityDataContext"/>.
        /// </summary>
        protected IEntityType FullEntityType { get; }

        private static ILog Log { get; } = LogManager.GetLogger(typeof(CachingGenericRepository<>).MakeGenericType(typeof(T)));

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            return this.Delete(this.GetById(id));
        }

        /// <inheritdoc/>
        public bool Delete(object obj)
        {
            using var context = this.GetContext();
            return context.Context.Remove(obj) != null;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<T> GetAll()
        {
            using var context = this.GetContext();
            var result = context.Context.Set<T>().ToList();
            this.LoadDependentData(result, context.Context);
            var newItems = context.Context.ChangeTracker.Entries<T>().Where(e => e.State == EntityState.Added).Select(e => e.Entity);
            return result.Concat(newItems);
        }

        /// <inheritdoc/>
        public virtual T GetById(Guid id)
        {
            using var context = this.GetContext();
            var result = context.Context.Set<T>().Find(id);
            if (result == null)
            {
                Log.Debug($"Object with id {id} could not be found.");
            }
            else
            {
                this.LoadDependentData(result, context.Context);
            }

            return result;
        }

        /// <inheritdoc/>
        object IRepository.GetById(Guid id)
        {
            return this.GetById(id);
        }

        /// <inheritdoc/>
        public IEnumerable LoadByProperty(IProperty property, object propertyValue)
        {
            using var context = this.GetContext();
            var result = this.LoadByPropertyInternal(property, propertyValue, context.Context).OfType<T>().ToList();
            this.LoadDependentData(result, context.Context);
            return result;
        }

        /// <summary>
        /// Gets the navigations which should be considered when loading the data.
        /// By default, we just load what the meta model of the current context contains.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        /// <returns>The navigations which should be considered when loading the data.</returns>
        protected virtual IEnumerable<INavigation> GetNavigations(EntityEntry entityEntry)
        {
            return entityEntry.Navigations.Select(n => n.Metadata);
        }

        /// <summary>
        /// Loads the dependent data of the object from the corresponding repositories.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="currentContext">The current context with which the object got loaded. It is neccessary to retrieve the foreign key ids.</param>
        protected virtual void LoadDependentData(object obj, DbContext currentContext)
        {
            var entityEntry = currentContext.Entry(obj);

            foreach (var navigation in this.GetNavigations(entityEntry))
            {
                if (!navigation.IsCollection())
                {
                    if (this.FullEntityType.FindPrimaryKey().Properties[0] == navigation.ForeignKey.Properties[0])
                    {
                        // The entity type is a many-to-many join entity and the property of the navigation is the "owner" of it.
                        // Therefore, we don't need to load it. It would be nice to set the object reference somehow, though.
                    }
                    else
                    {
                        this.LoadNavigationProperty(entityEntry, navigation);
                    }
                }
            }

            foreach (var collection in entityEntry.Collections.Where(c => !c.IsLoaded))
            {
                this.LoadCollection(entityEntry, collection.Metadata, currentContext);
                collection.IsLoaded = true;
            }
        }

        /// <summary>
        /// Loads the navigation collection.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="navigation">The navigation.</param>
        /// <param name="context">The context.</param>
        protected virtual void LoadCollection(EntityEntry entityEntry, INavigation navigation, DbContext context)
        {
            var foreignKeyProperty = navigation.ForeignKey.Properties.First();
            var loadStatusAware = navigation.GetGetter().GetClrValue(entityEntry.Entity) as ILoadingStatusAwareList;
            if (loadStatusAware?.LoadingStatus == LoadingStatus.Loaded || loadStatusAware?.LoadingStatus == LoadingStatus.Loading)
            {
                // already loaded or loading
                return;
            }

            if (loadStatusAware == null && navigation.GetGetter().GetClrValue(entityEntry.Entity) != null)
            {
                // already loaded or loading
                return;
            }

            if (loadStatusAware == null)
            {
                throw new InvalidOperationException($"The collection is not implementing {nameof(ILoadingStatusAware)}");
            }

            loadStatusAware.LoadingStatus = LoadingStatus.Loading;

            if (this.RepositoryManager.GetRepository(foreignKeyProperty.DeclaringEntityType.ClrType) is ILoadByProperty repository)
            {
                var foreignKeyValue = entityEntry.Property(navigation.ForeignKey.PrincipalKey.Properties.First().Name).CurrentValue;
                var items = repository.LoadByProperty(foreignKeyProperty, foreignKeyValue);
                foreach (var obj in items)
                {
                    if (!loadStatusAware.Contains(obj))
                    {
                        loadStatusAware.Add(obj);
                    }
                }

                loadStatusAware.LoadingStatus = LoadingStatus.Loaded;
            }
            else
            {
                Log.Warn($"No repository found which supports loading by foreign key for type ${foreignKeyProperty.DeclaringEntityType.ClrType}.");
                loadStatusAware.LoadingStatus = LoadingStatus.Failed;
            }
        }

        /// <summary>
        /// Loads the data navigation property and sets it in the entity.
        /// </summary>
        /// <param name="entityEntry">The entity entry from the context.</param>
        /// <param name="navigation">The navigation property.</param>
        protected virtual void LoadNavigationProperty(EntityEntry entityEntry, INavigation navigation)
        {
            if (navigation.ForeignKey.DeclaringEntityType != navigation.DeclaringEntityType)
            {
                // inverse property
                return;
            }

            var keyProperty = navigation.ForeignKey.Properties.First();
            var idValue = entityEntry.Property(keyProperty.Name).CurrentValue;

            Guid id = (idValue as Guid?) ?? Guid.Empty;
            if (id != Guid.Empty)
            {
                var getter = navigation.GetGetter();
                var currentValue = getter.GetClrValue(entityEntry.Entity);
                if ((currentValue is IIdentifiable) && (currentValue as IIdentifiable).Id == id)
                {
                    // loaded already
                    return;
                }

                IRepository repository = null;
                try
                {
                    repository = this.RepositoryManager.GetRepository(navigation.GetTargetType().ClrType);
                }
                catch (RepositoryNotFoundException ex)
                {
                    Log.Error($"Repository not found: {ex.Message}");
                }

                if (repository != null)
                {
                    if (navigation is Navigation concreteNavigation
                        && concreteNavigation.Setter != null)
                    {
                        concreteNavigation.Setter.SetClrValue(entityEntry.Entity, repository.GetById(id));
                    }
                    else
                    {
                        Log.Error($"Could not find setter for navigation {navigation}");
                    }
                }
                else
                {
                    Log.Error($"Repository not found for navigation target type {navigation.GetTargetType()}.");
                }
            }
        }

        /// <summary>
        /// Loads the dependent data of the objects from the corresponding repositories.
        /// </summary>
        /// <param name="loadedObjects">The loaded objects.</param>
        /// <param name="currentContext">The current context with which the objects got loaded. It is neccessary to retrieve the foreign key ids.</param>
        protected virtual void LoadDependentData(IEnumerable loadedObjects, DbContext currentContext)
        {
            foreach (var obj in loadedObjects)
            {
                this.LoadDependentData(obj, currentContext);
            }
        }

        /// <summary>
        /// Gets a context to work with. If no context is currently registered at the repository manager, a new one is getting created.
        /// </summary>
        /// <returns>The context.</returns>
        protected abstract EntityFrameworkContextBase GetContext();

        private IEnumerable LoadByPropertyInternal(IProperty property, object propertyValue, DbContext context)
        {
            if (property.ClrType == typeof(Guid))
            {
                return context.Set<T>().Where(o => EF.Property<Guid>(o, property.Name) == (Guid)propertyValue);
            }

            if (property.ClrType == typeof(Guid?))
            {
                return context.Set<T>().Where(o => EF.Property<Guid?>(o, property.Name) == (Guid?)propertyValue);
            }

            return Enumerable.Empty<object>();
        }
    }
}