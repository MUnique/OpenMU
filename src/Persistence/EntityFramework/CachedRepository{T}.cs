// <copyright file="CachedRepository{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;

/// <summary>
/// A repository which caches all of its data in memory.
/// </summary>
/// <typeparam name="T">The type of the business object.</typeparam>
public class CachedRepository<T> : IRepository<T>
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
    public IEnumerable<T> GetAll()
    {
        if (this._allLoaded)
        {
            return this._cache.Values;
        }

        if (this._loading)
        {
            while (this._loading)
            {
                // TODO: When making this async, then do a Task.Delay...
                Thread.Sleep(10);
            }

            return this._cache.Values;
        }

        this._loading = true;
        try
        {
            IEnumerable<T> values = this.BaseRepository.GetAll();

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
    public T? GetById(Guid id)
    {
        this.GetAll();
        this._cache.TryGetValue(id, out var result);
        return result;
    }

    /// <inheritdoc/>
    object? IRepository.GetById(Guid id)
    {
        return this.GetById(id);
    }

    /// <inheritdoc/>
    public bool Delete(object obj)
    {
        if (obj is IIdentifiable identifiable)
        {
            return this.Delete(identifiable.Id);
        }

        return false;
    }

    /// <inheritdoc/>
    public bool Delete(Guid id)
    {
        if (this.BaseRepository.Delete(id))
        {
            this.RemoveFromCache(id);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the object to the cache.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="obj">The object.</param>
    protected virtual void AddToCache(Guid id, T obj)
    {
        if (this._cache.ContainsKey(id))
        {
            if (object.Equals(this._cache[id], obj))
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