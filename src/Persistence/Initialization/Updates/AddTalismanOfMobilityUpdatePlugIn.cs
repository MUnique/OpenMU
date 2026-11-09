// <copyright file="AddTalismanOfMobilityUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Talisman of Mobility and puts the packages of the in-game shop which contain it on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddTalismanOfMobilityUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddTalismanOfMobilityUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("123277D4-E0F0-4F92-9F94-F64F15F840CD")]
public class AddTalismanOfMobilityUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add the Talisman of Mobility";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Talisman of Mobility, which saves a place and warps the character back to it. The packages of the in-game shop which contain it are put on sale.";

    /// <summary>
    /// The product sequence of the products of the in-game shop which contain the Talisman of Mobility.
    /// </summary>
    private const int TalismanOfMobilityProductSequence = 41;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    /// <remarks>
    /// Like the update of the kalima ticket, it puts packages of the in-game shop on sale,
    /// which a server owner may want to keep off sale, so it can be deselected.
    /// </remarks>
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 09, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The talisman gets the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        AddAttributes(context, gameConfiguration);
        if (new CashShopItems(context, gameConfiguration).CreateTalismanOfMobility() is { } talisman)
        {
            ItemRules.Apply([talisman]);
        }

        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Adds the attributes in which a character keeps the place which it saved with the talisman.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void AddAttributes(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var attribute in new[] { Stats.MobilityTalismanMap, Stats.MobilityTalismanX, Stats.MobilityTalismanY })
        {
            if (gameConfiguration.Attributes.All(a => a.Id != attribute.Id))
            {
                gameConfiguration.Attributes.Add(context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description));
            }
        }
    }

    /// <summary>
    /// Assigns the talisman to the products of the in-game shop, which were created without it.
    /// Their packages were initialized as not for sale then, so they're put on sale.
    /// The product sequence only identifies these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration)
            || gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 70 }) is not { } talisman)
        {
            return;
        }

        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null && p.ProductSequence == TalismanOfMobilityProductSequence))
            {
                product.ItemDefinition = talisman;
                assigned = true;
            }

            if (assigned && package.Products.All(product => product.CanBeDelivered()))
            {
                package.IsForSale = true;
            }
        }
    }
}
