// <copyright file="CashShopProduct.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using System.ComponentModel.DataAnnotations;
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
    [Range(0, 1000000)]
    public int PriceWCoinC { get; set; }

    /// <summary>
    /// Gets or sets the price in WCoinP.
    /// </summary>
    [Range(0, 1000000)]
    public int PriceWCoinP { get; set; }

    /// <summary>
    /// Gets or sets the price in Goblin Points.
    /// </summary>
    [Range(0, 1000000)]
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
    /// Gets or sets the date and time from which this product is available.
    /// If null, there is no start date restriction.
    /// </summary>
    public DateTime? AvailableFrom { get; set; }

    /// <summary>
    /// Gets or sets the date and time until which this product is available.
    /// If null, there is no end date restriction.
    /// </summary>
    public DateTime? AvailableUntil { get; set; }

    /// <summary>
    /// Gets a value indicating whether this product is currently available for purchase,
    /// considering both the IsAvailable flag and the date range restrictions.
    /// </summary>
    public bool IsCurrentlyAvailable
    {
        get
        {
            if (!this.IsAvailable)
            {
                return false;
            }

            var now = DateTime.UtcNow;

            if (this.AvailableFrom.HasValue && now < this.AvailableFrom.Value)
            {
                return false;
            }

            if (this.AvailableUntil.HasValue && now > this.AvailableUntil.Value)
            {
                return false;
            }

            return true;
        }
    }

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
