// <copyright file="ItemPriceModifiers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The price modifiers of item options, which can be applied on top of the base price of an item.
/// They are applied in the order of their values.
/// </summary>
/// <remarks>
/// The values are persisted, so they must not be renumbered.
/// </remarks>
[Flags]
public enum ItemPriceModifiers
{
    /// <summary>
    /// No modifiers are applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// The skill of the item increases the price by 150 percent.
    /// </summary>
    Skill = 1,

    /// <summary>
    /// The luck option increases the price by 25 percent.
    /// </summary>
    Luck = 1 << 1,

    /// <summary>
    /// The normal option increases the price by 60 percent at level 1, and by 0.7 * 2^(level - 1) above.
    /// </summary>
    Option = 1 << 2,

    /// <summary>
    /// Each wing option increases the price by 25 percent.
    /// </summary>
    WingOption = 1 << 3,

    /// <summary>
    /// Each excellent option doubles the price.
    /// </summary>
    Excellent = 1 << 4,

    /// <summary>
    /// All modifiers are applied, like for the automatic equipment price.
    /// </summary>
    All = Skill | Luck | Option | WingOption | Excellent,
}
