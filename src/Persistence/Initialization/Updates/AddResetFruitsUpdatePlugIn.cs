// <copyright file="AddResetFruitsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the reset fruits of strength, agility, vitality, energy and command,
/// and puts the packages of the in-game shop which contain them on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddResetFruitsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddResetFruitsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("3B7FDCD4-7CCC-462A-899C-0770BD3DBC15")]
public class AddResetFruitsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add reset fruits";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the reset fruits of strength, agility, vitality, energy and command, which take back invested points of a stat and return them as level up points. The packages of the in-game shop which contain them are put on sale.";

    /// <summary>
    /// The item numbers (of group 13) of the products of the in-game shop, by their product sequence.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, short> ProductItemNumbers = new Dictionary<int, short>
    {
        { 14, 54 }, // Reset Fruit (Strength)
        { 15, 55 }, // Reset Fruit (Agility)
        { 16, 56 }, // Reset Fruit (Vitality)
        { 17, 57 }, // Reset Fruit (Energy)
        { 18, 58 }, // Reset Fruit (Command)
    };

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 14, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The fruits get the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new CashShopItems(context, gameConfiguration).CreateResetFruits();

        // The fruits may already exist, e.g. when the cash shop update was applied in the same run
        // and created them without their rules. So, the final state is applied to all of them.
        var fruits = gameConfiguration.Items.Where(IsResetFruit).ToList();
        fruits.ForEach(fruit => fruit.Durability = CashShopInitializer.ResetFruitDurability);
        ItemRules.Apply(fruits);

        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    private static bool IsResetFruit(ItemDefinition item) => item.Group == 13 && ProductItemNumbers.Values.Contains(item.Number);

    /// <summary>
    /// Assigns the fruits to the products of the in-game shop, which were created without them,
    /// and sets the quantity of all fruit products.
    /// Their packages were initialized as not for sale then, so they're put on sale.
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
            foreach (var product in package.Products)
            {
                if (!ProductItemNumbers.TryGetValue(product.ProductSequence, out var number))
                {
                    continue;
                }

                if (product.ItemDefinition is null
                    && gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Number == number) is { } item)
                {
                    product.ItemDefinition = item;
                    assigned = true;
                }

                if (product.ItemDefinition is { Group: 13 } definition && definition.Number == number)
                {
                    product.Quantity = CashShopInitializer.ResetFruitDurability;
                }
            }

            if (assigned && package.Products.All(product => product.CanBeDelivered()))
            {
                package.IsForSale = true;
            }
        }
    }
}
