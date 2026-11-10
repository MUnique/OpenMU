// <copyright file="AddFigurinesAndCharmsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the figurines and charms, which are used inside the inventory.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFigurinesAndCharmsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFigurinesAndCharmsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("7EABF432-BC91-48E6-BDD0-5F1B7D7EFBC9")]
public class AddFigurinesAndCharmsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The items of the products of the cash shop script of the game client, by their product sequence.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, (byte Group, short Number)> ProductItems = new Dictionary<int, (byte Group, short Number)>
    {
        { 459, (13, 128) },
        { 460, (13, 129) },
        { 461, (13, 130) },
        { 462, (13, 132) },
    };

    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add the figurines and charms";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Hawk and Goat Figurines and the (Golden) Oak and Maple Charms, which give their bonuses while they're used inside the inventory.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 09, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new CashShopItems(context, gameConfiguration).CreateFigurinesAndCharms();
        LinkCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Links the products of the cash shop to the figurines and charms. The cash shop was created before these items existed,
    /// so its products of them have no item definition. Like other packages with products which can't be delivered,
    /// their packages are not for sale; the administrator decides when to sell them.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void LinkCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } cashShop
            || !CashShopInitializer.UsesClientScript(cashShop))
        {
            return;
        }

        var products = cashShop.Packages
            .SelectMany(package => package.Products)
            .Where(product => product.ItemDefinition is null && ProductItems.ContainsKey(product.ProductSequence));
        foreach (var product in products)
        {
            var (group, number) = ProductItems[product.ProductSequence];
            product.ItemDefinition = gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == number);
        }
    }
}
