// <copyright file="ValueListWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel;

/// <summary>
/// A list of wrappers for <typeparamref name="TValue"/>s.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class ValueListWrapper<TValue> : List<ValueWrapper<TValue>>, IList<TValue>
    where TValue : struct
{
    private readonly IList<TValue> _innerList;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueListWrapper{TValue}"/> class.
    /// </summary>
    /// <param name="innerList">The inner list.</param>
    public ValueListWrapper(IList<TValue> innerList)
    {
        this._innerList = innerList;
        this.AddRange(this._innerList.Select(this.CreateWrapper));
    }

    /// <inheritdoc />
    TValue IList<TValue>.this[int index]
    {
        get => this._innerList[index];
        set
        {
            this._innerList[index] = value;
            this[index].Value = value;
        }
    }

    /// <inheritdoc />
    public new IEnumerator<TValue> GetEnumerator()
    {
        return this._innerList.GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(TValue item)
    {
        this._innerList.Add(item);
        this.Add(this.CreateWrapper(item, this.Count));
    }

    /// <inheritdoc />
    public bool Contains(TValue item)
    {
        return this._innerList.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(TValue[] array, int arrayIndex)
    {
        this._innerList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(TValue item)
    {
        if (this._innerList.Remove(item))
        {
            var wrapperIndex = this.FindIndex(v => Equals(v.Value, item));
            if (wrapperIndex >= 0)
            {
                this[wrapperIndex].PropertyChanged -= this.OnValueChanged;
                this.RemoveAt(wrapperIndex);
                for (int i = wrapperIndex; i < this._innerList.Count; i++)
                {
                    this[i].Index = i;
                }
            }

            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool IsReadOnly => this._innerList.IsReadOnly;

    /// <inheritdoc />
    public int IndexOf(TValue item)
    {
        return this._innerList.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, TValue item)
    {
        this._innerList.Insert(index, item);
        this.Insert(index, this.CreateWrapper(item, index));
        for (int i = index + 1; i < this._innerList.Count; i++)
        {
            this[i].Index = i;
        }
    }

    private ValueWrapper<TValue> CreateWrapper(TValue item, int index)
    {
        var wrapper = new ValueWrapper<TValue>(item, index);
        wrapper.PropertyChanged += this.OnValueChanged;
        return wrapper;
    }

    private void OnValueChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ValueWrapper<TValue> wrapper)
        {
            this._innerList[wrapper.Index] = wrapper.Value;
        }
    }
}