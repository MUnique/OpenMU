// <copyright file="AddKalimaTicketUpdatePlugIn.cs" company="MUnique">
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
/// This update adds the kalima ticket and puts the packages of the in-game shop which contain it on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddKalimaTicketUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddKalimaTicketUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("4FD0B2EB-2DCF-4066-B75E-0E13236F728B")]
public class AddKalimaTicketUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add kalima ticket";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the kalima ticket, which opens a gate to the level of Kalima which fits the level of the character. The packages of the in-game shop which contain it are put on sale.";

    /// <summary>
    /// The product sequence of the products of the in-game shop which contain the kalima ticket.
    /// </summary>
    private const int KalimaTicketProductSequence = 47;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    /// <remarks>
    /// Like the update of the free tickets, it puts packages of the in-game shop on sale,
    /// which a server owner may want to keep off sale, so it can be deselected.
    /// </remarks>
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The ticket gets the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (new EventTicketItems(context, gameConfiguration).CreateKalimaTicket() is { } ticket)
        {
            ItemRules.Apply([ticket]);
        }

        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the ticket to the products of the in-game shop, which were created without it.
    /// Their packages were initialized as not for sale then, so they're put on sale.
    /// The product sequence only identifies these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration)
            || gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 48 }) is not { } ticket)
        {
            return;
        }

        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null && p.ProductSequence == KalimaTicketProductSequence))
            {
                product.ItemDefinition = ticket;
                assigned = true;
            }

            if (assigned && package.Products.All(product => product.CanBeDelivered()))
            {
                package.IsForSale = true;
            }
        }
    }
}
