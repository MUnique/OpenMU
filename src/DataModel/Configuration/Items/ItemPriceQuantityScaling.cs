// <copyright file="ItemPriceQuantityScaling.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Defines how the quantity (durability) of an item scales its base price.
/// </summary>
/// <remarks>
/// The values are persisted, so they must not be renumbered.
/// </remarks>
public enum ItemPriceQuantityScaling
{
    /// <summary>
    /// The quantity doesn't affect the price.
    /// </summary>
    None = 0,

    /// <summary>
    /// The base price is the price of one piece, so it's multiplied by the durability (= number of pieces), e.g. for potions.
    /// </summary>
    PerPiece = 1,

    /// <summary>
    /// The base price is the price of a full item, so it's multiplied by the durability and divided by the
    /// maximum durability of the item definition, e.g. for arrows and bolts.
    /// </summary>
    ByFillRatio = 2,
}
