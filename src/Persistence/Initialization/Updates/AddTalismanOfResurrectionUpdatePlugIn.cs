// <copyright file="AddTalismanOfResurrectionUpdatePlugIn.cs" company="MUnique">
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
/// This update adds the Talisman of Resurrection and puts the packages of the in-game shop which contain it on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddTalismanOfResurrectionUpdatePlugIn_Name), Description = nameof(PlugInResources.AddTalismanOfResurrectionUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("363C62F6-E039-42C3-B5EC-1990C4C71A16")]
public class AddTalismanOfResurrectionUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The product sequence of the products of the in-game shop which contain the Talisman of Resurrection.
    /// </summary>
    private const int TalismanOfResurrectionProductSequence = 40;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddTalismanOfResurrectionUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddTalismanOfResurrectionUpdatePlugIn_Description;

    /// <inheritdoc />
    /// <remarks>
    /// Like the update of the kalima ticket, it puts packages of the in-game shop on sale,
    /// which a server owner may want to keep off sale, so it can be deselected.
    /// </remarks>
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 09, 10, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The talisman gets the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (new CashShopItems(context, gameConfiguration).CreateTalismanOfResurrection() is { } talisman)
        {
            ItemRules.Apply([talisman]);
        }

        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
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
            || gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 69 }) is not { } talisman)
        {
            return;
        }

        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null && p.ProductSequence == TalismanOfResurrectionProductSequence))
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
