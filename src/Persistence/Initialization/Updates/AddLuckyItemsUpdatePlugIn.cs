// <copyright file="AddLuckyItemsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the lucky items: the lucky armor sets, the lucky item tickets and the jewels of extension
/// and elevation. David in Elveland gets the lucky item dialog, in which a ticket is exchanged for a lucky armor.
/// The packages of the in-game shop which contain the tickets are put on sale.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLuckyItemsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLuckyItemsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("7637B9EC-B034-436F-A622-9963E78F1898")]
public class AddLuckyItemsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add lucky items";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the lucky armor sets, the lucky item tickets and the jewels of extension and elevation. David in Elveland exchanges a ticket for a lucky armor which fits the character class. The packages of the in-game shop which contain the tickets are put on sale.";

    /// <summary>
    /// The number of the npc David, who exchanges the lucky item tickets.
    /// </summary>
    private const short DavidNumber = 579;

    /// <summary>
    /// The first product sequence of the lucky item tickets in the in-game shop. The products are in the order of the ticket numbers.
    /// </summary>
    private const int FirstTicketProductSequence = 465;

    /// <summary>
    /// The number (of group 13) of the first lucky item ticket.
    /// </summary>
    private const short FirstTicketNumber = 135;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 14, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The items get the item rule flags and the item stat profiles, so these have to exist first.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID, typeof(AddItemStatProfilesUpdatePlugInSeasonSix).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var luckyItems = new LuckyItems(context, gameConfiguration);
        var addedItems = luckyItems.CreateItems();
        ItemRules.Apply(addedItems);
        ItemStatProfiles.Apply(context, gameConfiguration);

        if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == DavidNumber) is { } david)
        {
            david.NpcWindow = NpcWindow.CombineLuckyItem;
            if (david.ItemCraftings.All(c => c.Number != LuckyItems.TicketExchangeCraftingNumber))
            {
                david.ItemCraftings.Add(luckyItems.CreateTicketExchangeCrafting());
            }
        }

        AssignCashShopProducts(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Assigns the tickets to the products of the in-game shop, which were created without them.
    /// Their packages were initialized as not for sale then, so they're put on sale.
    /// The product sequences only identify these products in the script which comes with the game client,
    /// so a configuration which uses a custom script stays untouched.
    /// </summary>
    private static void AssignCashShopProducts(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.CashShopConfiguration is not { } configuration
            || !CashShopInitializer.UsesClientScript(configuration))
        {
            return;
        }

        var ticketCount = LuckyItems.TicketNumbers.Count();
        foreach (var package in configuration.Packages)
        {
            var assigned = false;
            foreach (var product in package.Products.Where(p => p.ItemDefinition is null))
            {
                var ticketIndex = product.ProductSequence - FirstTicketProductSequence;
                if (ticketIndex >= 0 && ticketIndex < ticketCount
                    && gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Number == FirstTicketNumber + ticketIndex) is { } item)
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
