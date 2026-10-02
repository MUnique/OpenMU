// <copyright file="ShopItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// A purchasable item in the web shop, priced in Crimson coins.
///
/// This lives in the OpenMU Postgres (not Strapi) because stock and
/// availability are transactional business state — the ledger
/// (CrimsonCoinLedger) that backs purchases is in the same database,
/// so a single transaction can read balance, decrement stock, and write
/// the ledger entry. Strapi is editorial CMS only; splitting the
/// purchase flow across two systems would break atomicity.
///
/// Stock semantics: <c>-1</c> means unlimited (default for digital
/// goods like name changes, donation tiers). For finite items the
/// operator seeds the stock; the purchase endpoint decrements it
/// atomically alongside the ledger entry.
///
/// When <see cref="ItemDefinitionId"/> is set, the row wraps an actual
/// in-game item from <c>config."ItemDefinition"</c> — the sprite, name,
/// and in-game behaviour all come from OpenMU's data. The price and
/// shop-specific fields (category, available, featured, sort order)
/// stay operator-editable. When null, the row is a "pure" shop product
/// (bundle, donation tier, name change) with no in-game item
/// counterpart.
/// </summary>
[AggregateRoot]
public class ShopItem
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display name. Defaults to the linked
    /// <see cref="ItemDefinition"/>'s name when seeded.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL slug (unique). Defaults to
    /// <c>"{Group}-{Number}"</c> for ItemDefinition-backed rows.
    /// </summary>
    [Required]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the short description shown on the catalog card (max 200 chars).
    /// </summary>
    [Required]
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the long description rendered on the detail page. Markdown.
    /// </summary>
    public string LongDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price in whole Crimson coins (integer, no decimals).
    /// </summary>
    [Required]
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the category. One of: <c>"cosmético"</c>, <c>"utilidad"</c>,
    /// <c>"donación"</c>, <c>"paquete"</c>. String (not enum) because EF
    /// doesn't enumerate over arbitrary string sets; the API layer validates
    /// the value before persisting.
    /// </summary>
    [Required]
    public string Category { get; set; } = "cosmético";

    /// <summary>
    /// Gets or sets the OpenMU sprite identifier (filename stem of an imported
    /// sprite in <c>public/assets/items/</c>, e.g. <c>"item_0_0_15"</c>).
    /// Null for items without a matching sprite (bundles, donations).
    /// </summary>
    public string? GameItemId { get; set; }

    /// <summary>
    /// Gets or sets the FK to <c>config."ItemDefinition"</c> when this
    /// shop row wraps an actual in-game item. Null for pure shop
    /// products (bundles, donations).
    /// </summary>
    public Guid? ItemDefinitionId { get; set; }

    /// <summary>
    /// Gets or sets the navigation back to the wrapped item definition.
    /// Marked <c>[MemberOfAggregate]</c> so EF treats the relationship
    /// as a join column rather than loading the aggregate.
    /// </summary>
    [MemberOfAggregate]
    public virtual ItemDefinition? ItemDefinition { get; set; }

    /// <summary>
    /// Gets or sets the stock count. <c>-1</c> = unlimited.
    /// </summary>
    public int Stock { get; set; } = -1;

    /// <summary>
    /// Gets or sets a value indicating whether this item is visible on the shop.
    /// Toggled off when sold out or temporarily retired; not deleted so purchase
    /// history remains queryable.
    /// </summary>
    public bool Available { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this item surfaces in the
    /// "featured" carousel on the shop landing.
    /// </summary>
    public bool Featured { get; set; }

    /// <summary>
    /// Gets or sets the sort order within a category (lower = first).
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
