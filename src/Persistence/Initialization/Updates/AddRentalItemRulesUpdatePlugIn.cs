// <copyright file="AddRentalItemRulesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update prepares the items which are rented in the cash shop for their expiration:
/// it sets <see cref="ItemDefinition.IsSellableToNpcWhenExpired"/> (see <see cref="ItemRules.SellableWhenExpired"/>),
/// and assigns the small fixed prices for which the client sells them once they expired
/// (see <see cref="ItemPriceDefinitions.AssignRentalPrices"/>).
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
        return ValueTask.CompletedTask;
    }
}
