﻿// <copyright file="BackupItemStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// A storage which keeps items backed up - to be able to revert them after cancellation of an action.
/// </summary>
public class BackupItemStorage
{
    private readonly IList<(Item Item, byte Slot)> _initialItemStates;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupItemStorage"/> class.
    /// </summary>
    /// <param name="itemStorage">The item storage.</param>
    public BackupItemStorage(ItemStorage itemStorage)
    {
        var items = new List<Item>();
        items.AddRange(itemStorage.Items);
        this.Items = items;
        this.Money = itemStorage.Money;
        this._initialItemStates = this.Items.Select(item => (item, item.ItemSlot)).ToList();
    }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    public ICollection<Item> Items { get; set; }

    /// <summary>
    /// Gets or sets the money.
    /// </summary>
    public int Money { get; set; }

    /// <summary>
    /// Restores the initial item states.
    /// </summary>
    public void RestoreItemStates()
    {
        this._initialItemStates.ForEach(state => state.Item.ItemSlot = state.Slot);
    }
}