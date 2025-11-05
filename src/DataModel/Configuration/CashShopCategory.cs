// <copyright file="CashShopCategory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines a category for organizing cash shop products.
/// </summary>
public class CashShopCategory
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon identifier for the category.
    /// </summary>
    public string? IconId { get; set; }

    /// <summary>
    /// Gets or sets the display order for sorting categories.
    /// Lower numbers appear first.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this category is visible to players.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Name} (ID: {this.CategoryId})";
    }
}
