// <copyright file="ItemPriceRounding.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Defines how a selling price is rounded.
/// </summary>
/// <remarks>
/// The values are persisted, so they must not be renumbered.
/// </remarks>
public enum ItemPriceRounding
{
    /// <summary>
    /// Prices of at least 1,000 are rounded down to a multiple of 100, prices of at least 100 to a multiple of 10.
    /// The durability of the item reduces the price.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The price is rounded down to a multiple of 10, and the durability doesn't reduce it, e.g. for potions.
    /// </summary>
    Tens = 1,
}
