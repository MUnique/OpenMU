// <copyright file="AddFreeTicketItemsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the free tickets of Devil Square, Blood Castle, Illusion Temple and Chaos Castle,
/// and puts the packages of the in-game shop which contain them on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFreeTicketItemsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFreeTicketItemsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("F42C59B2-B0F7-463C-B000-9805AA584341")]
public class AddFreeTicketItemsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add free event tickets";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the free tickets of Devil Square, Blood Castle, Illusion Temple and Chaos Castle, which allow to enter the event of any level. The packages of the in-game shop which contain them are put on sale.";

    /// <summary>
    /// The item numbers (of group 13) of the products of the in-game shop, by their product sequence.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, short> ProductItemNumbers = new Dictionary<int, short>
    {
        { 12, 46 }, // Devil Square Ticket
        { 13, 47 }, // Blood Castle Ticket
        { 19, 61 }, // Illusion Temple Ticket
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
    public override DateTime CreatedAt => new(2026, 11, 02, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new EventTicketItems(context, gameConfiguration).CreateFreeTicketItems();
        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the tickets to the products of the in-game shop, which were created without them.
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
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null))
            {
                if (ProductItemNumbers.TryGetValue(product.ProductSequence, out var number)
                    && gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Number == number) is { } item)
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
