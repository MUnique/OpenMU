// <copyright file="CashShopProductsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initializer for cash shop products.
/// </summary>
public class CashShopProductsInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopProductsInitializer"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CashShopProductsInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes the cash shop products.
    /// </summary>
    public override void Initialize()
    {
        // Create sample cash shop products
        this.CreateProduct(1, "Health Potion Pack", 14, 3, 10, 0, 0, 10, 1, 3); // Large Healing Potion x10
        this.CreateProduct(2, "Mana Potion Pack", 14, 6, 10, 0, 0, 10, 1, 3); // Large Mana Potion x10
        this.CreateProduct(3, "Mixed Potion Pack", 14, 3, 20, 10, 0, 5, 1, 3); // 5 Large Healing Potions
        this.CreateProduct(4, "Bless of Light (Pack)", 14, 13, 50, 0, 0, 10, 1, 1); // Jewel of Bless x10
        this.CreateProduct(5, "Soul of Wizardry (Pack)", 14, 14, 50, 0, 0, 10, 1, 1); // Jewel of Soul x10
        this.CreateProduct(6, "Town Portal Scroll", 14, 11, 5, 0, 0, 1, 1, 1); // Town Portal Scroll
        this.CreateProduct(7, "Apple Bundle", 14, 0, 3, 3, 0, 20, 1, 1); // Apple x20
        this.CreateProduct(8, "Premium Health Pack", 14, 3, 0, 20, 0, 20, 1, 3, isEventItem: true); // Event: Large Healing Potion x20
        this.CreateProduct(9, "Premium Mana Pack", 14, 6, 0, 20, 0, 20, 1, 3, isEventItem: true); // Event: Large Mana Potion x20
        this.CreateProduct(10, "Goblin's Fortune Pack", 14, 13, 0, 0, 100, 5, 1, 1); // GoblinPoints: Jewel of Bless x5
    }

    private void CreateProduct(
        int productId,
        string displayName,
        byte itemGroup,
        byte itemNumber,
        int priceWCoinC,
        int priceWCoinP,
        int priceGoblinPoints,
        byte quantity,
        byte itemLevel = 0,
        byte durability = 1,
        bool isEventItem = false)
    {
        var product = this.Context.CreateNew<CashShopProduct>();
        product.ProductId = productId;
        product.DisplayName = displayName;
        product.PriceWCoinC = priceWCoinC;
        product.PriceWCoinP = priceWCoinP;
        product.PriceGoblinPoints = priceGoblinPoints;
        product.Quantity = quantity;
        product.ItemLevel = itemLevel;
        product.Durability = durability;
        product.ItemOptionLevel = 0;
        product.IsAvailable = true;
        product.IsEventItem = isEventItem;
        product.Category = isEventItem ? "Event Items" : "Consumables";

        // Find the item definition
        product.Item = this.GameConfiguration.Items.FirstOrDefault(i => i.Group == itemGroup && i.Number == itemNumber);

        if (product.Item != null)
        {
            this.GameConfiguration.CashShopProducts.Add(product);
        }
    }
}
