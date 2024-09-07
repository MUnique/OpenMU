// <copyright file="MemoryRepository{TValue}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Collections;
using System.Threading;

/// <summary>
/// A repository which lives on memory only.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class MemoryRepository<TValue> : IRepository<TValue>, IMemoryRepository
    where TValue : class
{
    private readonly List<Guid> _createdObjects = new();
    private readonly IDictionary<Guid, TValue> _values = new Dictionary<Guid, TValue>();

    /// <summary>
    /// Adds an item to the repository.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="obj">The item.</param>
    public void Add(Guid key, TValue obj)
    {
        this._values.TryAdd(key, obj);
        this._createdObjects.Add(key);
    }

    /// <inheritdoc />
    public void Add(Guid key, object obj)
    {
        if (obj is TValue value)
        {
            this.Add(key, value);
        }
        else
        {
            throw new ArgumentException($"Given object is not of type {typeof(TValue)}", nameof(obj));
        }
    }

    /// <inheritdoc />
    public async ValueTask RemoveAsync(Guid key)
    {
        await this.DeleteAsync(key).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<TValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        this._values.TryGetValue(id, out var obj);
        return ValueTask.FromResult(obj);
    }

    /// <inheritdoc/>
    public void OnSaveChanges()
    {
        foreach (var oldObjId in this._createdObjects)
        {
            if (this._values.TryGetValue(oldObjId, out var obj)
                && obj.GetId() is var currentId
                && currentId != oldObjId)
            {
                this._values.Remove(oldObjId);
                if (!this._values.TryAdd(currentId, obj))
                {
                    throw new InvalidOperationException($"Duplicate ID {currentId}. Existing: {this._values[currentId]}, Wanted to add: {obj}");
                }
            }
        }

        this._createdObjects.Clear();
    }

    /// <inheritdoc/>
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<bool> DeleteAsync(object obj)
    {
        var key = this._values.Where(kvp => kvp.Value.Equals(obj)).Select(kvp => kvp.Key).FirstOrDefault();
        return ValueTask.FromResult(this._values.Remove(key));
    }

    /// <inheritdoc/>
    public ValueTask<bool> DeleteAsync(Guid id)
    {
        return ValueTask.FromResult(this._values.Remove(id));
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TValue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return this._values.Values.Cast<TValue>();
    }

    /// <inheritdoc/>
    async ValueTask<IEnumerable> IRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return this._values.Values;
    }
}