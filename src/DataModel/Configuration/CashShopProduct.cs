// <copyright file="CashShopProduct.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Defines a product available in the cash shop.
/// </summary>
public class CashShopProduct
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the item definition.
    /// </summary>
    public virtual ItemDefinition? Item { get; set; }

    /// <summary>
    /// Gets or sets the price in WCoinC.
    /// </summary>
    public int PriceWCoinC { get; set; }

    /// <summary>
    /// Gets or sets the price in WCoinP.
    /// </summary>
    public int PriceWCoinP { get; set; }

    /// <summary>
    /// Gets or sets the price in Goblin Points.
    /// </summary>
    public int PriceGoblinPoints { get; set; }

    /// <summary>
    /// Gets or sets the quantity of items in this product.
    /// </summary>
    public byte Quantity { get; set; }

    /// <summary>
    /// Gets or sets the item level.
    /// </summary>
    public byte ItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the item option level.
    /// </summary>
    public byte ItemOptionLevel { get; set; }

    /// <summary>
    /// Gets or sets the durability.
    /// </summary>
    public byte Durability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this product is available for purchase.
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this product is a featured/event item.
    /// </summary>
    public bool IsEventItem { get; set; }

    /// <summary>
    /// Gets or sets the category of the product.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name for display.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.DisplayName} (ID: {this.ProductId})";
    }
}
