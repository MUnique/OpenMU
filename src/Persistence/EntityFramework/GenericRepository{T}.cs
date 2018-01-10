// <copyright file="GenericRepository{T}.cs" company="MUnique">
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
    /// A generic repository which wraps the access to the dbset of the <see cref="EntityDataContext"/>.
    /// Entities are getting eagerly (=completely) loaded automatically.
    /// </summary>
    /// <typeparam name="T">The type which this repository should manage.</typeparam>
    internal class GenericRepository<T> : IRepository<T>, ILoadByProperty
        where T : class
    {
        private readonly IRepositoryManager repositoryManager;

        /// <summary>
        /// The complete meta model of the entity type in <see cref="EntityDataContext"/>.
        /// </summary>
        private readonly IEntityType fullEntityType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public GenericRepository(IRepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
            using (var completeContext = new EntityDataContext())
            {
                this.fullEntityType = completeContext.Model.FindEntityType(typeof(T));
            }
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        protected IRepositoryManager RepositoryManager => this.repositoryManager;

        private static ILog Log { get; } = LogManager.GetLogger(typeof(GenericRepository<>).MakeGenericType(typeof(T)));

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            return this.Delete(this.GetById(id));
        }

        /// <inheritdoc/>
        public bool Delete(object obj)
        {
            using (var context = this.GetContext())
            {
                return context.Context.Remove(obj) != null;
            }
        }

        /// <inheritdoc/>
        public virtual IEnumerable<T> GetAll()
        {
            using (var context = this.GetContext())
            {
                var result = context.Context.Set<T>().ToList();
                this.LoadDependentData(result, context.Context);
                var newItems = context.Context.ChangeTracker.Entries<T>().Where(e => e.State == EntityState.Added).Select(e => e.Entity);
                return result.Concat(newItems);
            }
        }

        /// <inheritdoc/>
        public virtual T GetById(Guid id)
        {
            using (var context = this.GetContext())
            {
                var result = context.Context.Set<T>().Find(id);
                if (result == null)
                {
                    Log.Warn($"Object with id {id} could not be found.");
                }
                else
                {
                    this.LoadDependentData(result, context.Context);
                }

                return result;
            }
        }

        /// <inheritdoc/>
        object IRepository.GetById(Guid id)
        {
            return this.GetById(id);
        }

        /// <inheritdoc/>
        public IEnumerable LoadByProperty(IProperty property, object propertyValue)
        {
            using (var context = this.GetContext())
            {
                var result = this.LoadByPropertyInternal(property, propertyValue, context.Context).OfType<T>().ToList();
                this.LoadDependentData(result, context.Context);
                return result;
            }
        }

        /// <summary>
        /// Loads the dependent data of the object from the corresponding repositories.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="currentContext">The current context with which the object got loaded. It is neccessary to retrieve the foreign key ids.</param>
        protected virtual void LoadDependentData(object obj, DbContext currentContext)
        {
            var entityEntry = currentContext.Entry(obj);

            foreach (var navigation in this.fullEntityType.GetNavigations())
            {
                if (!navigation.IsCollection())
                {
                    if (this.fullEntityType.FindPrimaryKey().Properties[0] == navigation.ForeignKey.Properties[0])
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

            if (this.repositoryManager.GetRepository(foreignKeyProperty.DeclaringEntityType.ClrType) is ILoadByProperty repository)
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
                    repository = this.repositoryManager.GetRepository(navigation.GetTargetType().ClrType);
                }
                catch (RepositoryNotFoundException ex)
                {
                    Log.Error($"Repository not found: {ex.Message}");
                }

                if (repository != null)
                {
                    var setter = navigation.GetSetter();
                    setter.SetClrValue(entityEntry.Entity, repository.GetById(id));
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
        protected virtual EntityFrameworkContext GetContext()
        {
            var context = this.repositoryManager.GetCurrentContext() as EntityFrameworkContext;

            return new EntityFrameworkContext(context?.Context ?? new EntityDataContext(), context == null);
        }

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
