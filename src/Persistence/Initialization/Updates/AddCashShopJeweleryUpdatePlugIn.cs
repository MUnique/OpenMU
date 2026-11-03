// <copyright file="AddCashShopJeweleryUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the rings and necklaces of the in-game shop: the Lethal Wizard's Ring, the Sapphire, Ruby,
/// Topaz and Amethyst Rings and the Ruby, Emerald and Sapphire Necklaces.
/// </summary>
/// <remarks>
/// The products of the in-game shop which contain them get them as item. Their packages stay not for sale,
/// because all of these products are time-limited, which isn't supported yet.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddCashShopJeweleryUpdatePlugIn_Name), Description = nameof(PlugInResources.AddCashShopJeweleryUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8F56AB54-3DC1-41A1-BA4A-D0446A69A502")]
public class AddCashShopJeweleryUpdatePlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddCashShopJeweleryUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddCashShopJeweleryUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The items get the item rule flags and the item stat profile of their kind, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn =>
    [
        typeof(AddItemRuleFlagsPlugIn).GUID,
        typeof(AddItemStatProfilesUpdatePlugInSeasonSix).GUID,
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new Jewelery(context, gameConfiguration).CreateCashShopJewelery();
        ItemRules.Apply(addedItems);
        foreach (var item in addedItems)
        {
            var profileName = ItemStatProfiles.GetProfileName(item);
            item.StatProfile = gameConfiguration.ItemStatProfiles.FirstOrDefault(profile => profile.Name == profileName);
        }

        AssignCashShopProducts(gameConfiguration, addedItems);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the added items to the products of the in-game shop, which were created without them.
    /// The packages stay as they are: their products are time-limited, so they can't be delivered yet.
    /// The product sequences only identify these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration, IReadOnlyList<ItemDefinition> addedItems)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration)
            || addedItems.Count == 0)
        {
            return;
        }

        foreach (var product in configuration.Packages.SelectMany(package => package.Products).Where(product => product.ItemDefinition is null))
        {
            product.ItemDefinition = addedItems.FirstOrDefault(item => item.Number == ProductItemNumber(product.ProductSequence));
        }
    }

    /// <summary>
    /// Gets the item number (of group 13) of a product of the in-game shop, by its product sequence.
    /// </summary>
    private static short? ProductItemNumber(int productSequence) => productSequence switch
    {
        71 or 118 => 107, // Lethal Wizard's Ring
        62 => 109, // Sapphire Ring
        63 => 110, // Ruby Ring
        64 => 111, // Topaz Ring
        65 => 112, // Amethyst Ring
        66 or 122 => 113, // Ruby Necklace
        67 => 114, // Emerald Necklace
        68 => 115, // Sapphire Necklace
        _ => null,
    };
}
