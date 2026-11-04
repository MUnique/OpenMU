// <copyright file="CachedRepository{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using Nito.AsyncEx;

/// <summary>
/// A repository which caches all of its data in memory.
/// </summary>
/// <typeparam name="T">The type of the business object.</typeparam>
public class CachedRepository<T> : IRepository<T>
    where T : class, IIdentifiable
{
    private readonly ConcurrentDictionary<Guid, T> _cache = new();

    private readonly AsyncLock _loadLock = new();

    private volatile bool _allLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedRepository{T}"/> class.
    /// </summary>
    /// <param name="baseRepository">The base repository.</param>
    public CachedRepository(IRepository<T> baseRepository)
    {
        this.BaseRepository = baseRepository;
    }

    /// <summary>
    /// Gets the underlying base repository.
    /// </summary>
    protected IRepository<T> BaseRepository { get; }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IRepository.GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await this.EnsureLoadedAsync(cancellationToken).ConfigureAwait(false);
        return this._cache.Values;
    }

    /// <inheritdoc/>
    public async ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await this.EnsureLoadedAsync(cancellationToken).ConfigureAwait(false);
        this._cache.TryGetValue(id, out var result);
        return result;
    }

    /// <inheritdoc/>
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> DeleteAsync(object obj)
    {
        if (obj is not IIdentifiable identifiable)
        {
            return false;
        }

        return await this.DeleteAsync(identifiable.Id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> DeleteAsync(Guid id)
    {
        if (!await this.BaseRepository.DeleteAsync(id).ConfigureAwait(false))
        {
            return false;
        }

        this.RemoveFromCache(id);
        return true;
    }

    /// <summary>
    /// Adds the object to the cache.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="obj">The object.</param>
    protected virtual void AddToCache(Guid id, T obj)
    {
        if (!this._cache.TryAdd(id, obj) && !ReferenceEquals(this._cache[id], obj))
        {
            throw new ArgumentException("Other object with same id is already in cache.");
        }
    }

    /// <summary>
    /// Removes the object from cache.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected virtual void RemoveFromCache(Guid id)
    {
        this._cache.TryRemove(id, out _);
    }

    private async ValueTask EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if (this._allLoaded)
        {
            return;
        }

        using (await this._loadLock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            // Concurrent callers wait for the first one. If its loading failed, the next one tries again.
            if (this._allLoaded)
            {
                return;
            }

            var values = await this.BaseRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            foreach (var obj in values)
            {
                if (!this._cache.ContainsKey(obj.Id))
                {
                    this.AddToCache(obj.Id, obj);
                }
            }

            this._allLoaded = true;
        }
    }
}