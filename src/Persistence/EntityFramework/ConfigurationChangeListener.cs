// <copyright file="ConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using System.IO;

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
            && (parent?.GetId() ?? parent as Guid?) is { } parentId)
        {
            using var configContext = this._contextProvider.Value.CreateNewConfigurationContext();
            var gameConfiguration = (await configContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
            if (gameConfiguration is null)
            {
                return;
            }

            using var context = this._contextProvider.Value.CreateNewContext(gameConfiguration);
            object? cachedParent = null;
            try
            {
                cachedParent = await context.GetByIdAsync(parentId, parentCollectionNavigation.DeclaringEntityType.ClrType).ConfigureAwait(false);
            }
            catch (InvalidDataException)
            {
                // It can happen when the object and the parent object were created at the same time.
                // In this case, we just ignore it.
            }

            if (cachedParent is not null)
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
            && (parent?.GetId() ?? parent as Guid?) is { } parentId
            && await context.GetByIdAsync(parentId, parentCollectionNavigation.DeclaringEntityType.ClrType).ConfigureAwait(false) is { } cachedParent
            && await context.GetByIdAsync(id, type).ConfigureAwait(false) is { } cachedEntity)
        {
            collectionAccessor.Remove(cachedParent, cachedEntity);
        }

        await this._changePublisher.ConfigurationRemovedAsync(type, id).ConfigureAwait(false);
    }
}