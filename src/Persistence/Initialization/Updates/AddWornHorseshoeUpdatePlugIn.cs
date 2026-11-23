// <copyright file="AddWornHorseshoeUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the worn horseshoe (13/134), which increases the item drop rate while it's used inside the inventory.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddWornHorseshoeUpdatePlugIn_Name), Description = nameof(PlugInResources.AddWornHorseshoeUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5C0F3B1E-8A6D-4F27-9E43-2B7D19C6A8E5")]
public class AddWornHorseshoeUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The product sequence of the worn horseshoe in the cash shop script of the game client.
    /// </summary>
    internal const int ProductSequence = 463;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddWornHorseshoeUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddWornHorseshoeUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 16, 13, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The item uses the attribute of the item drop rate, which is added by the <see cref="AddSealItemDropRateUpdatePlugIn"/>,
    /// and the use inside the inventory, which comes with the <see cref="AddFigurinesAndCharmsUpdatePlugIn"/>.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddSealItemDropRateUpdatePlugIn).GUID, typeof(AddFigurinesAndCharmsUpdatePlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new CashShopItems(context, gameConfiguration).CreateWornHorseshoe();
        LinkCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Links the products of the cash shop to the worn horseshoe. The cash shop was created before the item existed,
    /// so its products have no item definition. Their packages stay not for sale, because the products are time-limited.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void LinkCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } cashShop
            || !CashShopInitializer.UsesClientScript(cashShop)
            || gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 134 }) is not { } horseshoe)
        {
            return;
        }

        var products = cashShop.Packages
            .SelectMany(package => package.Products)
            .Where(product => product.ItemDefinition is null && product.ProductSequence == ProductSequence);
        foreach (var product in products)
        {
            product.ItemDefinition = horseshoe;
        }
    }
}
