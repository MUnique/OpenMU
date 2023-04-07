// <copyright file="ItemOptionList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

using System.Collections;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// A list of applied item options.
/// It comes to action when multiple options of an <see cref="ItemOptionType"/>
/// can be applied to an item, e.g. excellent options.
/// </summary>
public class ItemOptionList : IList<IncreasableItemOption>
{
    private readonly ItemOptionType _optionType;
    private readonly Item _item;
    private readonly IContext _persistenceContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemOptionList"/> class.
    /// </summary>
    /// <param name="optionType">Type of the option.</param>
    /// <param name="item">The item.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    public ItemOptionList(ItemOptionType optionType, Item item, IContext persistenceContext)
    {
        this._optionType = optionType;
        this._item = item;
        this._persistenceContext = persistenceContext;
    }

    /// <summary>
    /// Occurs when the list has changed.
    /// </summary>
    public event EventHandler? ListChanged;

    /// <inheritdoc />
    public int Count => this.OptionLinks.Count();

    /// <inheritdoc />
    public bool IsReadOnly => false;

    private IEnumerable<ItemOptionLink> OptionLinks => this._item.ItemOptions
        .Where(link => link.ItemOption?.OptionType == this._optionType);

    /// <inheritdoc />
    public IncreasableItemOption this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerator<IncreasableItemOption> GetEnumerator()
    {
        return this.OptionLinks.Select(link => link.ItemOption!).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(IncreasableItemOption option)
    {
        var optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = option;
        this._item.ItemOptions.Add(optionLink);
        this.ListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Clear()
    {
        var currentOptions = this.ToList();
        foreach (var option in currentOptions)
        {
            this.Remove(option);
        }
    }

    /// <inheritdoc />
    public bool Contains(IncreasableItemOption item)
    {
        return this.OptionLinks.Any(o => o.ItemOption == item);
    }

    /// <inheritdoc />
    public void CopyTo(IncreasableItemOption[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Remove(IncreasableItemOption item)
    {
        if (this._item.ItemOptions.FirstOrDefault(o => o.ItemOption == item) is { } optionLink)
        {
            this._item.ItemOptions.Remove(optionLink);
            this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
            this.ListChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public int IndexOf(IncreasableItemOption item)
    {
        var index = this.OptionLinks.TakeWhile(o => o.ItemOption != item).Count();
        return index == this.Count ? -1 : index;
    }

    /// <inheritdoc />
    public void Insert(int index, IncreasableItemOption item)
    {
        this.Add(item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        if (this.OptionLinks.Take(index + 1).LastOrDefault()?.ItemOption is { } item)
        {
            this.Remove(item);
        }
    }
}