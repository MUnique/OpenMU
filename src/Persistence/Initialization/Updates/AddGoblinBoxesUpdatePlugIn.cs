// <copyright file="AddGoblinBoxesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Gold Key, Silver Key, the sealed and opened Golden Box and Silver Box, the goblin
/// combinations which open the sealed boxes with the keys, and puts the packages of the in-game shop
/// which contain the keys on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddGoblinBoxesUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddGoblinBoxesUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("6926D991-53CF-47E5-8A5C-FA55D3D56D75")]
public class AddGoblinBoxesUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add golden and silver boxes";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Gold Key and Silver Key, the Sealed Golden Box and Sealed Silver Box, which the keys open at the chaos goblin, and the Golden Box and Silver Box, which give an item or money when they're dropped. The packages of the in-game shop which contain the keys are put on sale.";

    /// <summary>
    /// The item numbers (of group 14) of the products of the in-game shop, by their product sequence.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, short> ProductItemNumbers = new Dictionary<int, short>
    {
        { 69, GoblinBoxItems.SilverKeyNumber },
        { 70, GoblinBoxItems.GoldKeyNumber },
    };

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    /// <remarks>
    /// The update puts packages of the in-game shop on sale, which a server owner may want to keep off sale,
    /// so it can be deselected.
    /// </remarks>
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 21, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The items get the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var initializer = new GoblinBoxItems(context, gameConfiguration);
        var addedItems = initializer.CreateItems();
        ItemRules.Apply(addedItems);
        initializer.AddCraftings();
        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the keys to the products of the in-game shop, which were created without them.
    /// Their packages were initialized as not for sale then, so they're put on sale.
    /// The product sequences only identify these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration))
        {
            return;
        }

        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null))
            {
                if (ProductItemNumbers.TryGetValue(product.ProductSequence, out var number)
                    && gameConfiguration.Items.FirstOrDefault(item => item.Group == GoblinBoxItems.ItemGroup && item.Number == number) is { } item)
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
