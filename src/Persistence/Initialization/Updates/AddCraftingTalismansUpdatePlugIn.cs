// <copyright file="AddCraftingTalismansUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Talisman of Luck and the Talisman of Chaos Assembly, adds them to the craftings
/// which accept them, and puts the packages of the in-game shop which contain them on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddCraftingTalismansUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddCraftingTalismansUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("40A2D4CF-7D09-4504-A723-B58F71990EF9")]
public class AddCraftingTalismansUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add crafting talismans";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Talisman of Luck, which increases the success rate of a crafting by its durability in percent, and the Talisman of Chaos Assembly, which keeps the item of a failed +10 to +15 item upgrade. They are added to the craftings which accept them, and the packages of the in-game shop which contain them are put on sale.";

    /// <summary>
    /// The quantities, which are the durability of the delivered Talisman of Luck, by the price sequence of the products of the in-game shop.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, int> TalismanOfLuckQuantities = new Dictionary<int, int>
    {
        { 37, 1 }, // Talisman of Luck, 1 %
        { 38, 5 }, // Talisman of Luck, 5 %
        { 583, 10 }, // Talisman of Luck 10%
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
    public override DateTime CreatedAt => new(2026, 11, 02, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The talismans get the item rule flags, so these have to exist first.
    /// The chaos mix fix resets the added percentage of all required items of the first wings crafting,
    /// so it has to run before.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID, typeof(FixChaosMixesPlugInSeason6).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var talismans = new CraftingTalismans(context, gameConfiguration).CreateTalismans();
        ItemRules.Apply(talismans);
        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the Talisman of Luck to the products of the in-game shop, which were created without it.
    /// Their packages were initialized as not for sale then, so they're put on sale.
    /// The price sequences only identify these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration)
            || gameConfiguration.Items.FirstOrDefault(item => item.Group == CraftingTalismans.Group && item.Number == CraftingTalismans.TalismanOfLuckNumber) is not { } talismanOfLuck)
        {
            return;
        }

        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null))
            {
                if (TalismanOfLuckQuantities.TryGetValue(product.PriceSequence, out var quantity))
                {
                    product.ItemDefinition = talismanOfLuck;
                    product.Quantity = quantity;
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
