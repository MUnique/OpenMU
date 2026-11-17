// <copyright file="AddRentalItemRulesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update prepares the items which are rented in the cash shop for their expiration:
/// it sets <see cref="ItemDefinition.IsSellableToNpcWhenExpired"/> (see <see cref="ItemRules.SellableWhenExpired"/>),
/// and assigns the small fixed prices for which the client sells them once they expired
/// (see <see cref="ItemPriceDefinitions.AssignRentalPrices"/>).
/// It also puts the packages of the cash shop on sale which couldn't be sold before only because their
/// products are time-limited, see <see cref="CashShopProductExtensions.CanBeTimeLimited"/>.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddRentalItemRulesUpdatePlugIn_Name), Description = nameof(PlugInResources.AddRentalItemRulesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("BEA2ABF3-9EC2-4EDC-88E3-6C13B1E6F6F4")]
public class AddRentalItemRulesUpdatePlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddRentalItemRulesUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddRentalItemRulesUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 16, 23, 20, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The rings and necklaces and the small wings are added by updates, and the price definitions, too.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn =>
    [
        typeof(AddCashShopUpdatePlugIn).GUID,
        typeof(AddItemRuleFlagsPlugIn).GUID,
        typeof(AddItemPriceDefinitionsPlugInSeason6).GUID,
        typeof(AddCashShopJeweleryUpdatePlugIn).GUID,
        typeof(AddSmallWingsUpdatePlugIn).GUID,
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ItemRules.ApplySellableWhenExpired(gameConfiguration.Items);
        new ItemPriceDefinitions(context, gameConfiguration).AssignRentalPrices();
        PutTimeLimitedPackagesOnSale(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Puts the packages on sale, which have a time-limited product that can be delivered now, and whose
    /// products can all be delivered. Only the catalog of the client's script is changed: the initialization
    /// created all of these packages for sale, but not for sale because of the time-limited products.
    /// </summary>
    private static void PutTimeLimitedPackagesOnSale(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration))
        {
            return;
        }

        foreach (var package in configuration.Packages.Where(package => !package.IsForSale))
        {
            if (package.Products.Any(product => product.Duration > TimeSpan.Zero && product.ItemDefinition?.CanBeTimeLimited() == true)
                && package.Products.All(product => product.CanBeDelivered()))
            {
                package.IsForSale = true;
            }
        }
    }
}
