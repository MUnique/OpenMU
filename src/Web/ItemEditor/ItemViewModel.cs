// <copyright file="ItemViewModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// View-Model for an <see cref="Item"/>.
/// </summary>
public class ItemViewModel
{
    /// <summary>
    /// The level mapping. Not each item level has its own effects. We save some
    /// traffic and storage space of item pictures with this trick.
    /// </summary>
    private static readonly int[] LevelMapping = { 0, 0, 0, 3, 3, 5, 5, 7, 7, 9, 9, 11, 11, 13, 13, 15, 15 };

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemViewModel"/> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="item">The item.</param>
    public ItemViewModel(StorageViewModel parent, Item item)
    {
        this.Parent = parent;
        this.Item = item;
    }

    /// <summary>
    /// Gets the parent storage.
    /// </summary>
    public StorageViewModel Parent { get; }

    /// <summary>
    /// Gets the item.
    /// </summary>
    public Item Item { get; }

    /// <summary>
    /// Gets the effect level.
    /// </summary>
    public int EffectLevel
    {
        get
        {
            if (this.Item.IsTrainablePet())
            {
                return 0;
            }

            if (this.Item.IsWearable())
            {
                return LevelMapping[this.Item.Level];
            }

            return this.Item.Level;
        }
    }

    /// <summary>
    /// Gets the option suffix.
    /// </summary>
    public string OptionSuffix
    {
        get
        {
            var isAncient = this.Item.ItemSetGroups.Any(s => s.AncientSetDiscriminator > 0);
            if (isAncient)
            {
                return "_a";
            }

            var isExcellent = this.Item.ItemOptions.Any(o => o.ItemOption!.OptionType == ItemOptionTypes.Excellent);
            if (isExcellent && this.Item.Definition?.Group < 12)
            {
                return "_e";
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    public int Column => this.EffectiveIndex % InventoryConstants.RowSize;

    /// <summary>
    /// Gets the row.
    /// </summary>
    public int Row => this.EffectiveIndex / InventoryConstants.RowSize;

    /// <summary>
    /// Gets the effective index of the item in the box.
    /// </summary>
    private int EffectiveIndex => this.Item.ItemSlot - this.Parent.StartIndex;
}