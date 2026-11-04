// <copyright file="ManyToManyCollectionAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;

/// <summary>
/// A many-to-many collection adapter which adapts beween <typeparamref name="T"/> and <typeparamref name="TJoin"/>.
/// </summary>
/// <remarks>
/// Usually in our object model we don't define collections of join entities and their types.
/// This is done automatically by our T4 templates.
/// </remarks>
/// <typeparam name="T">The type which is in a many to many relationship.</typeparam>
/// <typeparam name="TJoin">The type of the join entity. It contains a property of <typeparamref name="T"/>.</typeparam>
/// <seealso cref="System.Collections.Generic.ICollection{T}" />
internal class ManyToManyCollectionAdapter<T, TJoin> : ICollection<T>
{
    /// <summary>
    /// The raw collection, which is usually the collection which is mapped by entity framework.
    /// </summary>
    private readonly ICollection<TJoin> _rawCollection;

    /// <summary>
    /// The function to create a new join entity.
    /// </summary>
    private readonly Func<T, TJoin> _createJoinEntityFunction;

    /// <summary>
    /// The function to extract the instance of <typeparamref name="T"/> out of <typeparamref name="TJoin"/>.
    /// </summary>
    private readonly Func<TJoin, T> _extractFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManyToManyCollectionAdapter{T, TJoin}"/> class.
    /// </summary>
    /// <param name="rawCollection">The raw collection, which is usually the collection which is mapped by entity framework.</param>
    /// <param name="extractFunction">The function to extract the instance of <typeparamref name="T"/> out of <typeparamref name="TJoin"/>.</param>
    /// <param name="createJoinEntityFunction">The function to create a new join entity.</param>
    public ManyToManyCollectionAdapter(ICollection<TJoin> rawCollection, Func<TJoin, T> extractFunction, Func<T, TJoin> createJoinEntityFunction)
    {
        this._rawCollection = rawCollection;
        this._createJoinEntityFunction = createJoinEntityFunction;
        this._extractFunction = extractFunction;
    }

    /// <inheritdoc />
    public int Count => this._rawCollection.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => this._rawCollection.IsReadOnly;

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return this._rawCollection.Select(this._extractFunction).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        if (item != null)
        {
            this._rawCollection.Add(this._createJoinEntityFunction(item));
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this._rawCollection.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return this._rawCollection.Any(i => object.Equals(this._extractFunction(i), item));
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        this._rawCollection.Select(this._extractFunction).ToList().CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var joinItem = this._rawCollection.FirstOrDefault(i => object.Equals(this._extractFunction(i), item));
        if (joinItem != null)
        {
            return this._rawCollection.Remove(joinItem);
        }

        return false;
    }
}