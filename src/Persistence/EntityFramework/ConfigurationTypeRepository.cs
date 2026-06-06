// <copyright file="ConfigurationTypeRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A repository which gets its data from the <see cref="EntityDataContext.CurrentGameConfiguration"/>, without additionally touching the database.
/// </summary>
/// <typeparam name="T">The data object type.</typeparam>
internal class ConfigurationTypeRepository<T> : IRepository<T>, IConfigurationTypeRepository, IContextAwareRepository
    where T : class
{
    private readonly IContextAwareRepositoryProvider _repositoryProvider;

    private readonly Func<GameConfiguration, ICollection<T>> _collectionSelector;

    /// <summary>
    /// A cache which holds each <typeparamref name="T"/> in a dictionary to be able to access it by faster by id.
    /// There is one cache for each <see cref="GameConfiguration"/>, because it could be possible that more than one
    /// <see cref="GameConfiguration"/> could be hosted by one server.
    /// </summary>
    private readonly IDictionary<GameConfiguration, IDictionary<Guid, T>> _cache = new ConcurrentDictionary<GameConfiguration, IDictionary<Guid, T>>();

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationTypeRepository{T}" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="collectionSelector">The collection selector which returns the collection of <typeparamref name="T" /> of a <see cref="GameConfiguration" />.</param>
    public ConfigurationTypeRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory, Func<GameConfiguration, ICollection<T>> collectionSelector)
    {
        this._repositoryProvider = repositoryProvider;
        this._collectionSelector = collectionSelector;
        this._logger = loggerFactory.CreateLogger(this.GetType());
    }

    /// <summary>
    /// Gets all objects by using the <see cref="_collectionSelector"/> to the current <see cref="GameConfiguration"/>.
    /// </summary>
    /// <param name="context">The originating context which holds the current game configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All objects of the repository.</returns>
    public ValueTask<IEnumerable<T>> GetAllAsync(EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<IEnumerable<T>>(this._collectionSelector(this.GetCurrentGameConfiguration(context)));
    }

    /// <inheritdoc />
    public ValueTask<T?> GetByIdAsync(Guid id, EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this.EnsureCacheForCurrentConfiguration(context);

        var dictionary = this._cache[this.GetCurrentGameConfiguration(context)];
        if (dictionary.TryGetValue(id, out var result))
        {
            return ValueTask.FromResult<T?>(result);
        }

        return ValueTask.FromResult<T?>(null);
    }

    /// <inheritdoc />
    public ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return this.GetAllAsync((EntityFrameworkContextBase?)null, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return this.GetByIdAsync(id, (EntityFrameworkContextBase?)null, cancellationToken);
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.GetAllAsync((EntityFrameworkContextBase?)null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await this.GetByIdAsync(id, (EntityFrameworkContextBase?)null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IContextAwareRepository.GetAllAsync(EntityFrameworkContextBase? context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.GetAllAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async ValueTask<object?> IContextAwareRepository.GetByIdAsync(Guid id, EntityFrameworkContextBase? context, CancellationToken cancellationToken)
    {
        return await this.GetByIdAsync(id, context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<bool> DeleteAsync(object obj)
    {
        if (obj is not T item)
        {
            return false;
        }

        var gameConfiguration = this.GetCurrentGameConfiguration(null);
        var collection = this._collectionSelector(gameConfiguration);
        return collection.Remove(item);
    }

    /// <inheritdoc />
    public async ValueTask<bool> DeleteAsync(Guid id)
    {
        if (await this.GetByIdAsync(id).ConfigureAwait(false) is { } obj)
        {
            return await this.DeleteAsync(obj).ConfigureAwait(false);
        }

        return false;
    }

    /// <summary>
    /// Ensures the cache for the configuration of the given originating context.
    /// TODO: Call this at a better place and time - so that we can remove this check before every GetById
    /// </summary>
    /// <param name="context">The originating context which holds the current game configuration.</param>
    public void EnsureCacheForCurrentConfiguration(EntityFrameworkContextBase? context)
    {
        var configuration = this.GetCurrentGameConfiguration(context);
        if (this._cache.ContainsKey(configuration))
        {
            return;
        }

        lock (this._cache)
        {
            if (this._cache.ContainsKey(configuration))
            {
                return;
            }

            var dictionary = this._collectionSelector(configuration)
                .Where(item => item is IIdentifiable)
                .ToDictionary(item => ((IIdentifiable)item).Id, item => item);
            this._cache.Add(configuration, dictionary);
            foreach (var item in dictionary.Values)
            {
                ConfigurationIdReferenceResolver.Instance.AddReference((IIdentifiable)item);
            }
        }
    }

    /// <inheritdoc />
    public void UpdateCachedInstances(object changedInstance)
    {
        foreach (var (gameConfiguration, cache) in this._cache)
        {
            if (!cache.TryGetValue(changedInstance.GetId(), out var cachedInstance))
            {
                this._logger.LogDebug("Cached instance '{cachedInstance}' couldn't be updated because it wasn't found.", cachedInstance);
                return;
            }

            if (cachedInstance is not IAssignable<T> assignable)
            {
                // todo: implement this for all types
                this._logger.LogWarning("Cached instance '{cachedInstance}' couldn't be updated because it doesn't implement {IAssignable}.", cachedInstance, typeof(IAssignable<T>));
                return;
            }

            assignable.AssignValuesOf((T)changedInstance, gameConfiguration);
            this._logger.LogInformation("Updated cached instance '{cachedInstance}'.", cachedInstance);
        }
    }

    private GameConfiguration GetCurrentGameConfiguration(EntityFrameworkContextBase? context)
    {
        var dbContext = (context as CachingEntityFrameworkContext)?.Context as EntityDataContext;
        if (dbContext is null)
        {
            throw new InvalidOperationException("This repository can only be used within an account context.");
        }

        return dbContext.CurrentGameConfiguration ?? throw new InvalidOperationException("There is no current configuration.");
    }
}
