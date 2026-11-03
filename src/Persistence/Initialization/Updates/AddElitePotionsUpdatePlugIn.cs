// <copyright file="AddElitePotionsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the elite potions of the in-game shop (elite healing potion, elite mana potion,
/// medium elite healing potion and elite SD potion), and puts the packages of the in-game shop
/// which contain them on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddElitePotionsUpdatePlugIn_Name), Description = nameof(PlugInResources.AddElitePotionsUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("EFDD272F-C359-4953-86FB-9703C3067A8D")]
public class AddElitePotionsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The item numbers (of group 14) of the products of the in-game shop, by their product sequence.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, short> ProductItemNumbers = new Dictionary<int, short>
    {
        { 23, 70 }, // Elite Healing Potion
        { 24, 71 }, // Elite Mana Potion
        { 76, 133 }, // Elite SD Potion
        { 116, 133 }, // Elite SD Potion of the Gladiator Package
    };

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddElitePotionsUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddElitePotionsUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 17, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The potions get the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new Potions(context, gameConfiguration).CreateElitePotions();
        ItemRules.Apply(addedItems);
        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the potions to the products of the in-game shop, which were created without them.
    /// Their packages were initialized as not for sale then, so they're put on sale, if all of
    /// their products can be delivered.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration)
        {
            return;
        }

        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null))
            {
                if (ProductItemNumbers.TryGetValue(product.ProductSequence, out var number)
                    && gameConfiguration.Items.FirstOrDefault(item => item.Group == 14 && item.Number == number) is { } item)
                {
                    product.ItemDefinition = item;
                    assigned = true;
                }
            }

            if (assigned && package.Products.All(product => product.CanBeDelivered()))
            {
                package.IsForSale = true;
            }
        }
    }
}
