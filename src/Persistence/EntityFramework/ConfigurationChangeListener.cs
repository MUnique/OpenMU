// <copyright file="ConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Class which listens to changes within the <see cref="GameConfiguration"/>,
/// updates the cached instances and publishes them to the <see cref="IConfigurationChangePublisher"/>.
/// </summary>
public class ConfigurationChangeListener : IConfigurationChangeListener
{
    private readonly Lazy<IPersistenceContextProvider> _contextProvider;
    private readonly IConfigurationChangePublisher _changePublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationChangeListener"/> class.
    /// </summary>
    /// <param name="contextProvider">The context provider.</param>
    /// <param name="changePublisher">The change publisher.</param>
    public ConfigurationChangeListener(Lazy<IPersistenceContextProvider> contextProvider, IConfigurationChangePublisher changePublisher)
    {
        this._contextProvider = contextProvider;
        this._changePublisher = changePublisher;
    }

    /// <inheritdoc />
    public async ValueTask ConfigurationChangedAsync(Type type, Guid id, object configuration, object? parent)
    {
        var repositoryProvider = this._contextProvider.Value.RepositoryProvider;
        if (repositoryProvider is ICacheAwareRepositoryProvider cacheAwareRepositoryProvider)
        {
            await cacheAwareRepositoryProvider.UpdateCachedInstanceAsync(configuration).ConfigureAwait(false);
        }

        await this._changePublisher.ConfigurationChangedAsync(type, id, configuration).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ConfigurationAddedAsync(Type type, Guid id, object configuration, object? parent, INavigationBase? parentCollectionNavigation)
    {
        if (parentCollectionNavigation?.GetCollectionAccessor() is { } collectionAccessor
            && (parent as Guid? ?? parent?.GetId()) is { } parentId)
        {
            using var configContext = this._contextProvider.Value.CreateNewConfigurationContext();
            var gameConfiguration = (await configContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
            if (gameConfiguration is null)
            {
                return;
            }

            using var context = this._contextProvider.Value.CreateNewContext(gameConfiguration);
            var cachedParent = await context.GetByIdAsync(parentId, parentCollectionNavigation.DeclaringEntityType.ClrType).ConfigureAwait(false);

            // The cached parent may contain it already, when it was added together with its new parent,
            // because the values of the parent are assigned including its children.
            if (cachedParent is not null && !collectionAccessor.Contains(cachedParent, configuration))
            {
                var cachedConfiguration = configContext.CreateNew(type);
                if (cachedConfiguration is IAssignable assignable)
                {
                    assignable.AssignValuesOf(configuration, gameConfiguration);
                }
                else
                {
                    throw new InvalidOperationException($"Configuration type {type} is not assignable.");
                }

                collectionAccessor.Add(cachedParent, cachedConfiguration, false);
                this.RefreshConfigurationCaches();
            }
        }

        await this._changePublisher.ConfigurationAddedAsync(type, id, configuration).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ConfigurationRemovedAsync(Type type, Guid id, object? parent, INavigationBase? parentCollectionNavigation)
    {
        using var configContext = this._contextProvider.Value.CreateNewConfigurationContext();
        var gameConfiguration = (await configContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        using var context = this._contextProvider.Value.CreateNewContext(gameConfiguration);

        if (parentCollectionNavigation?.GetCollectionAccessor() is { } collectionAccessor
            && (parent as Guid? ?? parent?.GetId()) is { } parentId
            && await context.GetByIdAsync(parentId, parentCollectionNavigation.DeclaringEntityType.ClrType).ConfigureAwait(false) is { } cachedParent
            && collectionAccessor.GetOrCreate(cachedParent, false) is IEnumerable cachedCollection
            && cachedCollection.Cast<object>().FirstOrDefault(cachedObject => cachedObject.GetId() == id) is { } cachedEntity)
        {
            collectionAccessor.Remove(cachedParent, cachedEntity);
            this.RefreshConfigurationCaches();
        }

        await this._changePublisher.ConfigurationRemovedAsync(type, id).ConfigureAwait(false);
    }

    /// <summary>
    /// Refreshes the caches of the configuration types, so that the objects which were added to or removed from
    /// the cached configuration are found, e.g. when accounts which reference them are loaded.
    /// </summary>
    private void RefreshConfigurationCaches()
    {
        (this._contextProvider.Value.RepositoryProvider as CacheAwareRepositoryProvider)?.RefreshConfigurationCaches();
    }
}