// <copyright file="CachedRepository{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Linq;
using System.Threading;

/// <summary>
/// A repository which caches all of its data in memory.
/// </summary>
/// <typeparam name="T">The type of the business object.</typeparam>
public class CachedRepository<T> : IRepository<T>, IContextAwareRepository
    where T : class, IIdentifiable
{
    private readonly IDictionary<Guid, T> _cache;

    private bool _allLoaded;
    private bool _loading;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedRepository{T}"/> class.
    /// </summary>
    /// <param name="baseRepository">The base repository.</param>
    public CachedRepository(IRepository<T> baseRepository)
    {
        this.BaseRepository = baseRepository;

        this._cache = new Dictionary<Guid, T>();
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
    public ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return this.GetAllAsync(null, cancellationToken);
    }

    /// <summary>
    /// Gets all objects, using the given originating context to load them from the base repository.
    /// </summary>
    /// <param name="context">The originating context, or <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All objects of the repository.</returns>
    public async ValueTask<IEnumerable<T>> GetAllAsync(EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        if (this._allLoaded)
        {
            return this._cache.Values;
        }

        if (this._loading)
        {
            while (this._loading)
            {
                await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            }

            return this._cache.Values;
        }

        this._loading = true;
        try
        {
            IEnumerable<T> values = this.BaseRepository is IContextAwareRepository contextAware
                ? (await contextAware.GetAllAsync(context, cancellationToken).ConfigureAwait(false)).Cast<T>()
                : await this.BaseRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            foreach (var obj in values)
            {
                if (!this._cache.ContainsKey(obj.Id))
                {
                    this.AddToCache(obj.Id, obj);
                }
            }
        }
        finally
        {
            this._loading = false;
        }

        this._allLoaded = true;

        return this._cache.Values;
    }

    /// <inheritdoc/>
    public ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return this.GetByIdAsync(id, null, cancellationToken);
    }

    /// <summary>
    /// Gets an object by identifier, using the given originating context to load the data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="context">The originating context, or <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The object with the identifier.</returns>
    public async ValueTask<T?> GetByIdAsync(Guid id, EntityFrameworkContextBase? context, CancellationToken cancellationToken = default)
    {
        await this.GetAllAsync(context, cancellationToken).ConfigureAwait(false);
        this._cache.TryGetValue(id, out var result);
        return result;
    }

    /// <inheritdoc/>
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
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
        if (this._cache.TryGetValue(id, out var value))
        {
            if (Equals(value, obj))
            {
                throw new ArgumentException("Other object with same id is already in cache.");
            }
        }
        else
        {
            this._cache.Add(id, obj);
        }
    }

    /// <summary>
    /// Removes the object from cache.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected virtual void RemoveFromCache(Guid id)
    {
        this._cache.Remove(id);
    }
}