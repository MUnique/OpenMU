// <copyright file="ItemPriceDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Defines how the price of an item is calculated, if it differs from the automatic price calculation of equipment.
/// It's referenced by <see cref="ItemDefinition.PriceDefinition"/> and can be shared by several item definitions.
/// </summary>
/// <remarks>
/// The buying price of an item is calculated in these steps:
/// <list type="number">
///   <item>The base price is taken from <see cref="PricePerLevel"/>, if it has an entry for the level of the item.
///         Otherwise, it's the result of the <see cref="BasePriceFormula"/>. Without a formula, it's the automatic equipment price.</item>
///   <item>It's scaled by the quantity of the item, see <see cref="QuantityScaling"/>.</item>
///   <item>The price modifiers of the options are applied, see <see cref="Modifiers"/>.</item>
///   <item>The guardian option increases the price by 16 percent, and the price is limited to 3,000,000,000.</item>
/// </list>
/// The selling price is a third of the buying price, reduced by the missing durability and rounded, see <see cref="SellingPriceRounding"/>.
/// </remarks>
[Cloneable]
public partial class ItemPriceDefinition
{
    /// <summary>
    /// Gets or sets the name, e.g. "Jewel of Bless" or "Halloween items".
    /// </summary>
    public LocalizedString Name { get; set; }

    /// <summary>
    /// Gets or sets the formula for the base price of one unit of the item.
    /// When it's <c>null</c>, the automatic equipment price is used.
    /// </summary>
    /// <remarks>
    /// The formula is evaluated by mXparser. The result is truncated to an integer.
    /// These variables are available:
    /// <list type="bullet">
    ///   <item><c>level</c>: the item level.</item>
    ///   <item><c>durability</c>: the current durability, which is the number of pieces for stackable items.</item>
    ///   <item><c>maxDurability</c>: the <see cref="ItemDefinition.Durability"/>.</item>
    ///   <item><c>value</c>: the <see cref="ItemDefinition.Value"/>.</item>
    ///   <item><c>dropLevel</c>: the <see cref="ItemDefinition.DropLevel"/> plus 3 per item level, plus 25 for excellent items.</item>
    ///   <item><c>optionLevel</c>: the level of the normal option (<see cref="ItemOptionTypes.Option"/>), or 0.</item>
    ///   <item><c>optionCount</c>: the number of normal options.</item>
    ///   <item><c>healthRecoveryOptionLevel</c>: the level of the normal option, if it's a health recovery option; otherwise 0.</item>
    ///   <item><c>automaticPrice</c>: the automatic equipment price, before any modifiers.</item>
    /// </list>
    /// </remarks>
    public string? BasePriceFormula { get; set; }

    /// <summary>
    /// Gets or sets fixed base prices for specific item levels.
    /// When there is an entry for the level of the item, it takes precedence over the <see cref="BasePriceFormula"/>.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemLevelPrice> PricePerLevel { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets how the quantity (durability) of the item scales the base price.
    /// </summary>
    public ItemPriceQuantityScaling QuantityScaling { get; set; }

    /// <summary>
    /// Gets or sets the price modifiers of the item options, which are applied on top of the base price.
    /// </summary>
    public ItemPriceModifiers Modifiers { get; set; }

    /// <summary>
    /// Gets or sets how the selling price is rounded.
    /// </summary>
    public ItemPriceRounding SellingPriceRounding { get; set; }

    /// <summary>
    /// Gets or sets the price which is used to calculate the success rate of crafting, instead of the buying price.
    /// </summary>
    /// <remarks>
    /// Supposedly, earlier versions of the game had different prices for jewels, and the success rates of
    /// some craftings, e.g. the Chaos Weapon and the first wings, are still based on them.
    /// </remarks>
    public long? CraftingReferencePrice { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}
