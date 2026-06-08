// <copyright file="GenericRepositoryBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

/// <summary>
/// Base class for a generic repository which wraps the access to the DBSet of the <see cref="EntityDataContext"/>.
/// Entities are getting eagerly (=completely) loaded automatically.
/// </summary>
/// <typeparam name="T">The type which this repository should manage.</typeparam>
internal abstract class GenericRepositoryBase<T> : IRepository<T>, ILoadByProperty, IContextAwareRepository
    where T : class
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepositoryBase{T}" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="logger">The logger.</param>
    protected GenericRepositoryBase(IContextAwareRepositoryProvider repositoryProvider, ILogger logger)
    {
        this._logger = logger;
        this.RepositoryProvider = repositoryProvider;
        using var completeContext = new EntityDataContext();
        this.FullEntityType = completeContext.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"{typeof(T)} is not included in the model");
    }

    /// <summary>
    /// Gets the repository provider.
    /// </summary>
    protected IContextAwareRepositoryProvider RepositoryProvider { get; }

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
        using var context = this.GetContext(null);
        return context.Context.Remove(obj) is not null;
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return this.GetAllAsync((EntityFrameworkContextBase?)null, cancellationToken);
    }

    /// <inheritdoc/>
    public ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return this.GetByIdAsync(id, (EntityFrameworkContextBase?)null, cancellationToken);
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        return await this.GetAllAsync((EntityFrameworkContextBase?)null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await this.GetByIdAsync(id, (EntityFrameworkContextBase?)null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IContextAwareRepository.GetAllAsync(EntityFrameworkContextBase? context, CancellationToken cancellationToken)
    {
        return await this.GetAllAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    async ValueTask<object?> IContextAwareRepository.GetByIdAsync(Guid id, EntityFrameworkContextBase? context, CancellationToken cancellationToken)
    {
        return await this.GetByIdAsync(id, context, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all objects, using the given originating context.
    /// </summary>
    /// <param name="context">The originating context, or <c>null</c> to use a temporary context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All objects of the repository.</returns>
    public virtual async ValueTask<IEnumerable<T>> GetAllAsync(EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var ownedContext = context is null ? this.GetContext(null) : null;
        var origin = context ?? ownedContext!;

        var result = await origin.Context.Set<T>().ToListAsync(cancellationToken).ConfigureAwait(false);
        await this.LoadDependentDataAsync(result, origin, cancellationToken).ConfigureAwait(false);
        var newItems = origin.Context.ChangeTracker.Entries<T>().Where(e => e.State == EntityState.Added).Select(e => e.Entity);
        result.AddRange(newItems);
        return result;
    }

    /// <summary>
    /// Gets an object by identifier, using the given originating context.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="context">The originating context, or <c>null</c> to use a temporary context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The object with the identifier.</returns>
    public virtual async ValueTask<T?> GetByIdAsync(Guid id, EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        using var ownedContext = context is null ? this.GetContext(null) : null;
        var origin = context ?? ownedContext!;

        var result = await origin.Context.Set<T>().FindAsync(id, cancellationToken).ConfigureAwait(false);
        if (result is null)
        {
            this._logger.LogDebug("Object with id {Id} could not be found.", id);
        }
        else
        {
            await this.LoadDependentDataAsync(result, origin, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable> LoadByPropertyAsync(IProperty property, object propertyValue, EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        using var ownedContext = context is null ? this.GetContext(null) : null;
        var origin = context ?? ownedContext!;

        var result = (await this.LoadByPropertyInternalAsync(property, propertyValue, origin.Context, cancellationToken).ConfigureAwait(false)).OfType<T>().ToList();
        await this.LoadDependentDataAsync(result, origin, cancellationToken).ConfigureAwait(false);
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
        if (entityEntry.Context is ITypedContext)
        {
            return entityEntry.Metadata.GetNavigations();
        }

        return this.FullEntityType.GetNavigations();
    }

    /// <summary>
    /// Loads the dependent data of the object from the corresponding repositories.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="origin">The originating context with which the object got loaded. It is necessary to retrieve the foreign key ids.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual async ValueTask LoadDependentDataAsync(object obj, EntityFrameworkContextBase origin, CancellationToken cancellationToken)
    {
        var currentContext = origin.Context;
        var entityEntry = currentContext.Entry(obj);

        foreach (var navigation in this.GetNavigations(entityEntry).OfType<INavigation>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!navigation.IsCollection && navigation.GetClrValue(obj) is null)
            {
                if (currentContext is ITypedContext editContext
                    && editContext.RootType != navigation.DeclaringEntityType
                    && editContext.IsBackReference(entityEntry.Metadata.ClrType))
                {
                    // prevents endless loop.
                    continue;
                }

                if (this.FullEntityType.FindPrimaryKey()?.Properties[0] == navigation.ForeignKey.Properties[0])
                {
                    // The entity type is a many-to-many join entity and the property of the navigation is the "owner" of it.
                    // Therefore, we don't need to load it. It would be nice to set the object reference somehow, though.
                }
                else
                {
                    await this.LoadNavigationPropertyAsync(entityEntry, navigation, origin, cancellationToken).ConfigureAwait(false);
                    navigation.SetIsLoadedWhenNoTracking(obj);
                }
            }
        }

        foreach (var collection in entityEntry.Collections.Where(c => !c.IsLoaded))
        {
            if (collection.Metadata is INavigation metadata)
            {
                if (currentContext is ITypedContext editContext
                    && editContext.RootType != metadata.DeclaringEntityType
                    && editContext.IsBackReference(entityEntry.Metadata.ClrType))
                {
                    // prevents endless loop.
                    continue;
                }

                await this.LoadCollectionAsync(entityEntry, metadata, origin, cancellationToken).ConfigureAwait(false);
                collection.IsLoaded = true;
            }
        }
    }

    /// <summary>
    /// Loads the dependent data of the objects from the corresponding repositories.
    /// </summary>
    /// <param name="loadedObjects">The loaded objects.</param>
    /// <param name="origin">The originating context with which the objects got loaded. It is necessary to retrieve the foreign key ids.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    protected virtual async ValueTask LoadDependentDataAsync(IEnumerable loadedObjects, EntityFrameworkContextBase origin, CancellationToken cancellationToken)
    {
        foreach (var obj in loadedObjects)
        {
            await this.LoadDependentDataAsync(obj, origin, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Loads the navigation collection.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    /// <param name="navigation">The navigation.</param>
    /// <param name="origin">The originating context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual async ValueTask LoadCollectionAsync(EntityEntry entityEntry, INavigation navigation, EntityFrameworkContextBase origin, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

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

        if (this.RepositoryProvider.GetRepository(foreignKeyProperty.DeclaringType.ClrType, origin) is ILoadByProperty repository)
        {
            var foreignKeyValue = entityEntry.Property(navigation.ForeignKey.PrincipalKey.Properties[0].Name).CurrentValue;
            if (foreignKeyValue is { })
            {
                var items = await repository.LoadByPropertyAsync(foreignKeyProperty, foreignKeyValue, origin, cancellationToken).ConfigureAwait(false);
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
            this._logger.LogWarning("No repository found which supports loading by foreign key for type {ClrType}.", foreignKeyProperty.DeclaringType.ClrType);
            loadStatusAware.LoadingStatus = LoadingStatus.Failed;
        }
    }

    /// <summary>
    /// Loads the data navigation property and sets it in the entity.
    /// </summary>
    /// <param name="entityEntry">The entity entry from the context.</param>
    /// <param name="navigation">The navigation property.</param>
    /// <param name="origin">The originating context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual async ValueTask LoadNavigationPropertyAsync(EntityEntry entityEntry, IReadOnlyNavigation navigation, EntityFrameworkContextBase origin, CancellationToken cancellationToken)
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
                repository = this.RepositoryProvider.GetRepository(navigation.TargetEntityType.ClrType, origin);
            }
            catch (RepositoryNotFoundException ex)
            {
                this._logger.LogError(ex, "Repository not found: {Message}", ex.Message);
            }

            if (repository != null)
            {
                var loaded = repository is IContextAwareRepository contextAware
                    ? await contextAware.GetByIdAsync(id, origin, cancellationToken).ConfigureAwait(false)
                    : await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                if (!navigation.TrySetClrValue(entityEntry.Entity, loaded))
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
    /// Gets a context to work with. If no originating context is given, a new temporary one is getting created.
    /// </summary>
    /// <param name="origin">The originating context, or <c>null</c> to create a temporary context.</param>
    /// <returns>The context.</returns>
    protected abstract EntityFrameworkContextBase GetContext(EntityFrameworkContextBase? origin);

    private async ValueTask<IEnumerable> LoadByPropertyInternalAsync(IProperty property, object propertyValue, DbContext context, CancellationToken cancellationToken)
    {
        if (property.ClrType == typeof(Guid))
        {
            return await context.Set<T>().Where(o => EF.Property<Guid>(o, property.Name) == (Guid)propertyValue).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        if (property.ClrType == typeof(Guid?))
        {
            return await context.Set<T>().Where(o => EF.Property<Guid?>(o, property.Name) == (Guid?)propertyValue).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        return Enumerable.Empty<object>();
    }
}
