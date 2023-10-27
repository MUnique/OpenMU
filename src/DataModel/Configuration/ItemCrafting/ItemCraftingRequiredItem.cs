// <copyright file="ItemCraftingRequiredItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

using System.Globalization;
using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Describes an required item for a crafting.
/// </summary>
[Cloneable]
public partial class ItemCraftingRequiredItem
{
    /// <summary>
    /// Gets or sets the collection of possible items which are valid for this requirement.
    /// </summary>
    public virtual ICollection<ItemDefinition> PossibleItems { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the minimum item level.
    /// </summary>
    public byte MinimumItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum item level.
    /// </summary>
    public byte MaximumItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the required item options.
    /// </summary>
    public virtual ICollection<ItemOptionType> RequiredItemOptions { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the minimum amount.
    /// </summary>
    public byte MinimumAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum amount.
    /// </summary>
    public byte MaximumAmount { get; set; }

    /// <summary>
    /// Gets or sets the success result.
    /// </summary>
    public MixResult SuccessResult { get; set; }

    /// <summary>
    /// Gets or sets the fail result.
    /// </summary>
    public MixResult FailResult { get; set; }

    /// <summary>
    /// Gets or sets the NPC price divisor. For each full division, the percentage gets increased by 1 percent, and the mix price rises.
    /// </summary>
    public int NpcPriceDivisor { get; set; }

    /// <summary>
    /// Gets or sets the add percentage per item.
    /// </summary>
    public byte AddPercentage { get; set; }

    /// <summary>
    /// Gets or sets the reference identifier to the corresponding <see cref="ItemCraftingResultItem.Reference"/>.
    /// If <c>0</c>, no reference exists.
    /// </summary>
    public byte Reference { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        string itemName;
        if (!this.PossibleItems.Any())
        {
            itemName = "Random Item";
        }
        else
        {
            itemName = string.Join(", ", this.PossibleItems.Select(p => p.Name));
        }

        string amount = this.MinimumAmount == this.MaximumAmount
            ? this.MinimumAmount.ToString(CultureInfo.InvariantCulture)
            : $"{this.MinimumAmount}~{this.MaximumAmount}";

        string level;
        if (this.MinimumItemLevel == this.MaximumItemLevel && this.MinimumItemLevel == 0)
        {
            level = string.Empty;
        }
        else if (this.MinimumItemLevel == this.MaximumItemLevel)
        {
            level = $"+{this.MinimumItemLevel}";
        }
        else
        {
            level = $"+{this.MinimumItemLevel}~{this.MaximumItemLevel}";
        }

        string options;
        if (this.RequiredItemOptions.Any())
        {
            options = "+" + string.Join("+", this.RequiredItemOptions.Select(o => o.Name));
        }
        else
        {
            options = string.Empty;
        }

        return $"{amount} x {itemName}{level}{options}";
    }
}