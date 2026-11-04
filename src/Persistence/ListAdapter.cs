// <copyright file="ListAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// A collection adapter which adapts a <see cref="ICollection{TEfCore}"/> to an <see cref="ICollection{TClass}"/>,
/// if <typeparamref name="TEfCore" /> inherits from <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass">The type of the class.</typeparam>
/// <typeparam name="TEfCore">The type of the ef core.</typeparam>
/// <seealso cref="System.Collections.Generic.ICollection{TClass}" />
public class ListAdapter<TClass, TEfCore> : CollectionAdapter<TClass, TEfCore>, IList<TClass>
    where TClass : class
    where TEfCore : TClass
{
    private readonly IList<TEfCore> _rawList;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListAdapter{TClass, TEfCore}"/> class.
    /// </summary>
    /// <param name="rawList">The raw list.</param>
    public ListAdapter(IList<TEfCore> rawList)
        : base(rawList)
    {
        this._rawList = rawList;
    }

    /// <inheritdoc />
    public TClass this[int index]
    {
        get => this._rawList[index];

        set => this._rawList[index] = (TEfCore)value;
    }

    /// <inheritdoc />
    public int IndexOf(TClass item)
    {
        return this._rawList.IndexOf((TEfCore)item);
    }

    /// <inheritdoc />
    public void Insert(int index, TClass item)
    {
        this._rawList.Insert(index, (TEfCore)item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        this._rawList.RemoveAt(index);
    }
}