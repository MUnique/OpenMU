﻿// <copyright file="GenericRepositoryBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

/// <summary>
/// Base class for a generic repository which wraps the access to the DBSet of the <see cref="EntityDataContext"/>.
/// Entities are getting eagerly (=completely) loaded automatically.
/// </summary>
/// <typeparam name="T">The type which this repository should manage.</typeparam>
internal abstract class GenericRepositoryBase<T> : IRepository<T>, ILoadByProperty
    where T : class
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepositoryBase{T}" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="logger">The logger.</param>
    protected GenericRepositoryBase(RepositoryManager repositoryManager, ILogger logger)
    {
        this._logger = logger;
        this.RepositoryManager = repositoryManager;
        using var completeContext = new EntityDataContext();
        this.FullEntityType = completeContext.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"{typeof(T)} is not included in the model");
    }

    /// <summary>
    /// Gets the repository manager.
    /// </summary>
    protected RepositoryManager RepositoryManager { get; }

    /// <summary>
    /// Gets the complete meta model of the entity type in <see cref="EntityDataContext"/>.
    /// </summary>
    protected IEntityType FullEntityType { get; }

    /// <inheritdoc/>
    public async ValueTask<bool> DeleteAsync(Guid id)
    {
        if (await this.GetByIdAsync(id).ConfigureAwait(false) is { } item)
        {
            return await this.DeleteAsync(item).ConfigureAwait(false);
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> DeleteAsync(object obj)
    {
        using var context = this.GetContext();
        return context.Context.Remove(obj) is not null;
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IRepository.GetAllAsync()
    {
        return await this.GetAllAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async ValueTask<IEnumerable<T>> GetAllAsync()
    {
        using var context = this.GetContext();
        var result = await context.Context.Set<T>().ToListAsync().ConfigureAwait(false);
        await this.LoadDependentDataAsync(result, context.Context).ConfigureAwait(false);
        var newItems = context.Context.ChangeTracker.Entries<T>().Where(e => e.State == EntityState.Added).Select(e => e.Entity);
        result.AddRange(newItems);
        return result;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<T?> GetByIdAsync(Guid id)
    {
        using var context = this.GetContext();
        var result = await context.Context.Set<T>().FindAsync(id).ConfigureAwait(false);
        if (result is null)
        {
            this._logger.LogDebug("Object with id {Id} could not be found.", id);
        }
        else
        {
            await this.LoadDependentDataAsync(result, context.Context).ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc/>
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id)
    {
        return await this.GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable> LoadByPropertyAsync(IProperty property, object propertyValue)
    {
        using var context = this.GetContext();
        var result = (await this.LoadByPropertyInternalAsync(property, propertyValue, context.Context).ConfigureAwait(false)).OfType<T>().ToList();
        await this.LoadDependentDataAsync(result, context.Context).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Gets the navigations which should be considered when loading the data.
    /// By default, we just load what the meta model of the current context contains.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    /// <returns>The navigations which should be considered when loading the data.</returns>
    protected virtual IEnumerable<INavigationBase> GetNavigations(EntityEntry entityEntry)
    {
        return entityEntry.Navigations.Select(n => n.Metadata);
    }

    /// <summary>
    /// Loads the dependent data of the object from the corresponding repositories.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="currentContext">The current context with which the object got loaded. It is necessary to retrieve the foreign key ids.</param>
    protected virtual async ValueTask LoadDependentDataAsync(object obj, DbContext currentContext)
    {
        var entityEntry = currentContext.Entry(obj);

        foreach (var navigation in this.GetNavigations(entityEntry).OfType<INavigation>())
        {
            if (!navigation.IsCollection)
            {
                if (this.FullEntityType.FindPrimaryKey()?.Properties[0] == navigation.ForeignKey.Properties[0])
                {
                    // The entity type is a many-to-many join entity and the property of the navigation is the "owner" of it.
                    // Therefore, we don't need to load it. It would be nice to set the object reference somehow, though.
                }
                else
                {
                    await this.LoadNavigationPropertyAsync(entityEntry, navigation).ConfigureAwait(false);
                }
            }
        }

        foreach (var collection in entityEntry.Collections.Where(c => !c.IsLoaded))
        {
            if (collection.Metadata is INavigation metadata)
            {
                await this.LoadCollectionAsync(entityEntry, metadata, currentContext).ConfigureAwait(false);
                collection.IsLoaded = true;
            }
        }
    }

    /// <summary>
    /// Loads the dependent data of the objects from the corresponding repositories.
    /// </summary>
    /// <param name="loadedObjects">The loaded objects.</param>
    /// <param name="currentContext">The current context with which the objects got loaded. It is necessary to retrieve the foreign key ids.</param>
    protected virtual async ValueTask LoadDependentDataAsync(IEnumerable loadedObjects, DbContext currentContext)
    {
        foreach (var obj in loadedObjects)
        {
            await this.LoadDependentDataAsync(obj, currentContext).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Loads the navigation collection.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    /// <param name="navigation">The navigation.</param>
    /// <param name="context">The context.</param>
    protected virtual async ValueTask LoadCollectionAsync(EntityEntry entityEntry, INavigation navigation, DbContext context)
    {
        var foreignKeyProperty = navigation.ForeignKey.Properties[0];
        var loadStatusAware = navigation.GetClrValue<ILoadingStatusAwareList>(entityEntry.Entity);
        if (loadStatusAware?.LoadingStatus == LoadingStatus.Loaded || loadStatusAware?.LoadingStatus == LoadingStatus.Loading)
        {
            // already loaded or loading
            return;
        }

        if (loadStatusAware is null && navigation.GetClrValue(entityEntry.Entity) != null)
        {
            // already loaded or loading
            return;
        }

        if (loadStatusAware is null)
        {
            throw new InvalidOperationException($"The collection is not implementing {nameof(ILoadingStatusAware)}");
        }

        loadStatusAware.LoadingStatus = LoadingStatus.Loading;

        if (this.RepositoryManager.GetRepository(foreignKeyProperty.DeclaringEntityType.ClrType) is ILoadByProperty repository)
        {
            var foreignKeyValue = entityEntry.Property(navigation.ForeignKey.PrincipalKey.Properties[0].Name).CurrentValue;
            if (foreignKeyValue is { })
            {
                var items = await repository.LoadByPropertyAsync(foreignKeyProperty, foreignKeyValue).ConfigureAwait(false);
                foreach (var obj in items)
                {
                    if (!loadStatusAware.Contains(obj))
                    {
                        loadStatusAware.Add(obj);
                    }
                }
            }

            loadStatusAware.LoadingStatus = LoadingStatus.Loaded;
        }
        else
        {
            this._logger.LogWarning("No repository found which supports loading by foreign key for type {ClrType}.", foreignKeyProperty.DeclaringEntityType.ClrType);
            loadStatusAware.LoadingStatus = LoadingStatus.Failed;
        }
    }

    /// <summary>
    /// Loads the data navigation property and sets it in the entity.
    /// </summary>
    /// <param name="entityEntry">The entity entry from the context.</param>
    /// <param name="navigation">The navigation property.</param>
    protected virtual async ValueTask LoadNavigationPropertyAsync(EntityEntry entityEntry, IReadOnlyNavigation navigation)
    {
        if (navigation.ForeignKey.DeclaringEntityType != navigation.DeclaringEntityType)
        {
            // inverse property
            return;
        }

        var keyProperty = navigation.ForeignKey.Properties[0];
        var idValue = entityEntry.Property(keyProperty.Name).CurrentValue;

        Guid id = (idValue as Guid?) ?? Guid.Empty;
        if (id != Guid.Empty)
        {
            var currentValue = navigation.GetClrValue(entityEntry.Entity);
            if (currentValue is IIdentifiable identifiable && identifiable.Id == id)
            {
                // loaded already
                return;
            }

            IRepository? repository = null;
            try
            {
                repository = this.RepositoryManager.GetRepository(navigation.TargetEntityType.ClrType);
            }
            catch (RepositoryNotFoundException ex)
            {
                this._logger.LogError(ex, "Repository not found: {Message}", ex.Message);
            }

            if (repository != null)
            {
                if (!navigation.TrySetClrValue(entityEntry.Entity, await repository.GetByIdAsync(id).ConfigureAwait(false)))
                {
                    this._logger.LogError("Could not find setter for navigation {Navigation}", navigation);
                }
            }
            else
            {
                this._logger.LogError("Repository not found for navigation target type {TargetEntityType}.", navigation.TargetEntityType);
            }
        }
    }

    /// <summary>
    /// Gets a context to work with. If no context is currently registered at the repository manager, a new one is getting created.
    /// </summary>
    /// <returns>The context.</returns>
    protected abstract EntityFrameworkContextBase GetContext();

    private async ValueTask<IEnumerable> LoadByPropertyInternalAsync(IProperty property, object propertyValue, DbContext context)
    {
        if (property.ClrType == typeof(Guid))
        {
            return await context.Set<T>().Where(o => EF.Property<Guid>(o, property.Name) == (Guid)propertyValue).ToListAsync().ConfigureAwait(false);
        }

        if (property.ClrType == typeof(Guid?))
        {
            return await context.Set<T>().Where(o => EF.Property<Guid?>(o, property.Name) == (Guid?)propertyValue).ToListAsync().ConfigureAwait(false);
        }

        return Enumerable.Empty<object>();
    }
}