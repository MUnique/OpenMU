// <copyright file="CollectionListAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemDrops;

using System.Collections;

/// <summary>
/// Adapts a <see cref="ICollection{T}"/> to a <see cref="IList{T}"/>, so that it can be used
/// with components which require a list, e.g. the <see cref="MUnique.OpenMU.Web.Shared.Components.Form.MultiLookupField{TObject}"/>.
/// All modifications are passed through to the underlying collection.
/// </summary>
/// <typeparam name="T">The type of the elements.</typeparam>
public sealed class CollectionListAdapter<T> : IList<T>
{
    private readonly ICollection<T> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionListAdapter{T}"/> class.
    /// </summary>
    /// <param name="collection">The adapted collection.</param>
    public CollectionListAdapter(ICollection<T> collection)
    {
        this._collection = collection;
    }

    /// <inheritdoc />
    public int Count => this._collection.Count;

    /// <inheritdoc />
    public bool IsReadOnly => this._collection.IsReadOnly;

    /// <inheritdoc />
    public T this[int index]
    {
        get => this._collection.ElementAt(index);
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this._collection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    public void Add(T item)
    {
        if (!this._collection.Contains(item))
        {
            this._collection.Add(item);
        }
    }

    /// <inheritdoc />
    public void Clear() => this._collection.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => this._collection.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => this._collection.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(T item) => this._collection.Remove(item);

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        var index = 0;
        foreach (var element in this._collection)
        {
            if (Equals(element, item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    /// <inheritdoc />
    public void Insert(int index, T item) => this.Add(item);

    /// <inheritdoc />
    public void RemoveAt(int index) => this._collection.Remove(this[index]);
}
