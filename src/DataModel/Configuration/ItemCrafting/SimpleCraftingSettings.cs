// <copyright file="SimpleCraftingSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Crafting settings for the simple item crafting handler.
/// </summary>
[Cloneable]
public partial class SimpleCraftingSettings
{
    /// <summary>
    /// Gets or sets the price to do the crafting.
    /// </summary>
    public int Money { get; set; }

    /// <summary>
    /// Gets or sets the price to do the crafting.
    /// </summary>
    public int MoneyPerFinalSuccessPercentage { get; set; }

    /// <summary>
    /// Gets or sets the NPC price divisor for the sum of crafting items' prices. For each full division, the percentage gets increased by 1 percent, and the mix price rises.
    /// </summary>
    /// <remarks>Used for Chaos Weapon and 1st Level Wing craftings.</remarks>
    public int NpcPriceDivisor { get; set; }

    /// <summary>
    /// Gets or sets the success percent.
    /// </summary>
    public byte SuccessPercent { get; set; }

    /// <summary>
    /// Gets or sets the maximum success percent.
    /// </summary>
    public byte MaximumSuccessPercent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether multiple crafting at the same time are allowed for this crafting.
    /// </summary>
    public bool MultipleAllowed { get; set; }

    /// <summary>
    /// Gets or sets the required items.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemCraftingRequiredItem> RequiredItems { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the result items, which are generated when the crafting succeeded.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemCraftingResultItem> ResultItems { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the result item selection.
    /// </summary>
    public ResultItemSelection ResultItemSelect { get; set; }

    /// <summary>
    /// Gets or sets the success percentage addition for a item with luck option which gets modified.
    /// </summary>
    public int SuccessPercentageAdditionForLuck { get; set; }

    /// <summary>
    /// Gets or sets the success percentage addition for an excellent item which gets modified.
    /// </summary>
    public int SuccessPercentageAdditionForExcellentItem { get; set; }

    /// <summary>
    /// Gets or sets the success percentage addition for an ancient item which gets modified.
    /// </summary>
    public int SuccessPercentageAdditionForAncientItem { get; set; }

    /// <summary>
    /// Gets or sets the success percentage addition for a "380 item" which gets modified.
    /// </summary>
    public int SuccessPercentageAdditionForGuardianItem { get; set; }

    /// <summary>
    /// Gets or sets the success percentage addition for a socket item which gets modified.
    /// </summary>
    public int SuccessPercentageAdditionForSocketItem { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent of getting the luck option in the random result item.
    /// </summary>
    public byte ResultItemLuckOptionChance { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent of getting the skill in the random result item.
    /// </summary>
    public byte ResultItemSkillChance { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent of getting an excellent option in the random result item.
    /// </summary>
    public byte ResultItemExcellentOptionChance { get; set; }

    /// <summary>
    /// Gets or sets the maximum excellent options in the random result item.
    /// </summary>
    public byte ResultItemMaxExcOptionCount { get; set; }
}