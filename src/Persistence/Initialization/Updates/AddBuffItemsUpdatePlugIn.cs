// <copyright file="AddBuffItemsUpdatePlugIn.cs" company="MUnique">
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
/// This update adds the buff items of the in-game shop (seals, scrolls, elixirs and auras) with their effects,
/// and assigns them to the products of the in-game shop which contain them.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddBuffItemsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddBuffItemsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("B887EECA-56F2-4AC1-951B-78ED12041DF5")]
public class AddBuffItemsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add buff items";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the seals, scrolls, elixirs and auras of the in-game shop, which apply a timed effect when they are used. Existing items with the same numbers are left unchanged.";

    /// <summary>
    /// The items (group, number) of the products of the in-game shop, by their product sequence.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, (byte Group, short Number)> ProductItems = new Dictionary<int, (byte Group, short Number)>
    {
        { 9, (13, 43) }, // Seal of Ascension
        { 10, (13, 44) }, // Seal of Wealth
        { 11, (13, 45) }, // Seal of Sustenance
        { 25, (14, 72) }, // Scroll of Quickness
        { 26, (14, 73) }, // Scroll of Defense
        { 27, (14, 74) }, // Scroll of Wrath
        { 28, (14, 75) }, // Scroll of Wizardry
        { 29, (14, 76) }, // Scroll of Health
        { 30, (14, 77) }, // Scroll of Mana
        { 36, (14, 97) }, // Scroll of Battle
        { 37, (14, 98) }, // Scroll of Strength
        { 38, (13, 62) }, // Seal of Healing
        { 39, (13, 63) }, // Seal of Divinity
        { 50, (13, 93) }, // Master Seal of Ascension
        { 51, (13, 94) }, // Master Seal of Wealth
        { 73, (13, 104) }, // Max AG Boost Aura
        { 74, (13, 105) }, // Max SD Boost Aura
        { 94, (13, 44) }, // Seal of Wealth
        { 95, (13, 94) }, // Master Seal of Wealth
        { 97, (14, 73) }, // Scroll of Defense
        { 115, (14, 72) }, // Scroll of Quickness
        { 120, (14, 98) }, // Scroll of Strength
        { 121, (13, 105) }, // Max SD Boost Aura
        { 156, (14, 77) }, // Scroll of Mana
        { 157, (14, 76) }, // Scroll of Health
        { 158, (14, 75) }, // Scroll of Wizardry
        { 159, (14, 74) }, // Scroll of Wrath
        { 160, (14, 73) }, // Scroll of Defense
        { 161, (14, 72) }, // Scroll of Quickness
        { 162, (14, 97) }, // Scroll of Battle
        { 432, (13, 62) }, // Seal of Healing
        { 491, (14, 73) }, // Scroll of Defense
        { 492, (14, 76) }, // Scroll of Health
        { 493, (14, 77) }, // Scroll of Mana
        { 494, (14, 97) }, // Scroll of Battle
        { 495, (14, 98) }, // Scroll of Strength
        { 496, (14, 72) }, // Scroll of Quickness
        { 497, (14, 74) }, // Scroll of Wrath
        { 498, (14, 75) }, // Scroll of Wizardry
        { 518, (13, 44) }, // Seal of Wealth
        { 519, (13, 44) }, // Seal of Wealth
        { 520, (13, 94) }, // Master Seal of Wealth
        { 521, (13, 94) }, // Master Seal of Wealth
        { 522, (13, 43) }, // Seal of Ascension
        { 524, (13, 93) }, // Master Seal of Ascension
        { 525, (13, 93) }, // Master Seal of Ascension
        { 529, (13, 43) }, // Seal of Ascension
        { 1353, (13, 44) }, // Seal of Wealth
        { 1354, (13, 62) }, // Seal of Healing
        { 1356, (13, 94) }, // Master Seal of Wealth
    };

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The items get the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new BuffItems(context, gameConfiguration).CreateItems();
        ItemRules.Apply(addedItems);
        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the items to the products of the in-game shop, which were created without them.
    /// </summary>
    /// <remarks>
    /// These products are limited in time, which the in-game shop can't deliver yet. So their packages stay
    /// not for sale, unless all of their products can be delivered.
    /// The product sequences only identify these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </remarks>
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
                if (ProductItems.TryGetValue(product.ProductSequence, out var key)
                    && gameConfiguration.Items.FirstOrDefault(item => item.Group == key.Group && item.Number == key.Number) is { } item)
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
