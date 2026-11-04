// <copyright file="EquatableArray.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Collections;
using System.Collections.Immutable;

/// <summary>
/// An immutable array with value equality.
/// </summary>
/// <remarks>
/// Incremental generators compare the models of their pipeline steps to decide if the following steps have to run again.
/// <see cref="ImmutableArray{T}"/> has reference equality, so it's wrapped by this type.
/// </remarks>
/// <typeparam name="T">The type of the items.</typeparam>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquatableArray{T}"/> struct.
    /// </summary>
    /// <param name="items">The items.</param>
    public EquatableArray(ImmutableArray<T> items)
    {
        this._items = items;
    }

    private ImmutableArray<T> Items => this._items.IsDefault ? ImmutableArray<T>.Empty : this._items;

    /// <inheritdoc />
    public bool Equals(EquatableArray<T> other) => this.Items.SequenceEqual(other.Items);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is EquatableArray<T> other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 17;
        foreach (var item in this.Items)
        {
            hashCode = (hashCode * 31) + (item?.GetHashCode() ?? 0);
        }

        return hashCode;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)this.Items).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
