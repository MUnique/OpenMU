// <copyright file="CashShopCategoriesInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initializer for cash shop categories.
/// </summary>
public class CashShopCategoriesInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopCategoriesInitializer"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CashShopCategoriesInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes the cash shop categories.
    /// </summary>
    public override void Initialize()
    {
        this.CreateCategory(1, "Consumables", "Potions, scrolls, and other consumable items", "icon_consumables", 10);
        this.CreateCategory(2, "Jewels", "Enhancement jewels and stones", "icon_jewels", 20);
        this.CreateCategory(3, "Event Items", "Limited-time event products", "icon_events", 30);
        this.CreateCategory(4, "Buffs & Boosts", "Experience boosters and buff items", "icon_buffs", 40);
        this.CreateCategory(5, "Special", "Unique and special products", "icon_special", 50);
    }

    private void CreateCategory(
        int categoryId,
        string name,
        string description,
        string iconId,
        int displayOrder)
    {
        var category = this.Context.CreateNew<CashShopCategory>();
        category.CategoryId = categoryId;
        category.Name = name;
        category.Description = description;
        category.IconId = iconId;
        category.DisplayOrder = displayOrder;
        category.IsVisible = true;

        this.GameConfiguration.CashShopCategories.Add(category);
    }
}
