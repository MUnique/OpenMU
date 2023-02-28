// <copyright file="ConfigurationTypeRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Collections.Concurrent;
using System.IO;

using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A repository which gets its data from the <see cref="EntityDataContext.CurrentGameConfiguration"/>, without additionally touching the database.
/// </summary>
/// <typeparam name="T">The data object type.</typeparam>
internal class ConfigurationTypeRepository<T> : IRepository<T>, IConfigurationTypeRepository
    where T : class
{
    private readonly RepositoryManager _repositoryManager;

    private readonly Func<GameConfiguration, ICollection<T>> _collectionSelector;

    /// <summary>
    /// A cache which holds each <typeparamref name="T"/> in a dictionary to be able to access it by faster by id.
    /// There is one cache for each <see cref="GameConfiguration"/>, because it could be possible that more than one
    /// <see cref="GameConfiguration"/> could be hosted by one server.
    /// </summary>
    private readonly IDictionary<GameConfiguration, IDictionary<Guid, T>> _cache = new ConcurrentDictionary<GameConfiguration, IDictionary<Guid, T>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationTypeRepository{T}" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="collectionSelector">The collection selector which returns the collection of <typeparamref name="T" /> of a <see cref="GameConfiguration" />.</param>
    public ConfigurationTypeRepository(RepositoryManager repositoryManager, Func<GameConfiguration, ICollection<T>> collectionSelector)
    {
        this._repositoryManager = repositoryManager;
        this._collectionSelector = collectionSelector;
    }

    /// <summary>
    /// Gets all objects by using the <see cref="_collectionSelector"/> to the current <see cref="GameConfiguration"/>.
    /// </summary>
    /// <returns>All objects of the repository.</returns>
    public ValueTask<IEnumerable<T>> GetAllAsync()
    {
        return ValueTask.FromResult<IEnumerable<T>>(this._collectionSelector(this.GetCurrentGameConfiguration()));
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IRepository.GetAllAsync()
    {
        return await this.GetAllAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<T?> GetByIdAsync(Guid id)
    {
        this.EnsureCacheForCurrentConfiguration();
        var dictionary = this._cache[this.GetCurrentGameConfiguration()];
        if (dictionary.TryGetValue(id, out var result))
        {
            return ValueTask.FromResult<T?>(result);
        }

        throw new InvalidDataException($"The object of {typeof(T).Name} with the specified id {id} could not be found in the game configuration");
    }

    /// <inheritdoc />
    public async ValueTask<bool> DeleteAsync(object obj)
    {
        if (obj is not T item)
        {
            return false;
        }

        var gameConfiguration = this.GetCurrentGameConfiguration();
        var collection = this._collectionSelector(gameConfiguration);
        return collection.Remove(item);
    }

    /// <inheritdoc />
    public async ValueTask<bool> DeleteAsync(Guid id)
    {
        var obj = await this.GetByIdAsync(id).ConfigureAwait(false);
        if (obj is null)
        {
            return false;
        }

        return await this.DeleteAsync(obj).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id)
    {
        return await this.GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <summary>
    /// Ensures the cache for the current configuration.
    /// </summary>
    public void EnsureCacheForCurrentConfiguration()
    {
        var configuration = this.GetCurrentGameConfiguration();

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

    private GameConfiguration GetCurrentGameConfiguration()
    {
        var context = (this._repositoryManager.ContextStack.GetCurrentContext() as CachingEntityFrameworkContext)?.Context as EntityDataContext;
        if (context is null)
        {
            throw new InvalidOperationException("This repository can only be used within an account context.");
        }

        return context.CurrentGameConfiguration ?? throw new InvalidOperationException("There is no current configuration.");
    }
}