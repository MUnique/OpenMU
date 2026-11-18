// <copyright file="CashShopItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initializer for the items of the cash shop which aren't put into the inventory. A delivery
/// plugin applies them when a player uses them from the cash shop storage, e.g. the
/// <see cref="GameLogic.CashShop.VaultExtensionDeliveryPlugIn"/>.
/// </summary>
/// <remarks>
/// Existing items aren't changed, so it can be used by an update, too.
/// </remarks>
internal class CashShopItems : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CashShopItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateItem(14, 91, LocalizedString.FromResource(() => ItemNames.SummonerCharacterCard));
        this.CreateItem(14, 162, LocalizedString.FromResource(() => ItemNames.MagicBackpack));
        this.CreateItem(14, 163, LocalizedString.FromResource(() => ItemNames.VaultExpansionCertificate));
        this.CreateItem(14, 169, LocalizedString.FromResource(() => ItemNames.RageFighterCharacterCard));
        this.CreateResetFruits();
        this.CreateTalismanOfResurrection();
        this.CreateTalismanOfMobility();
    }

    /// <summary>
    /// Creates the Talisman of Mobility, if it doesn't exist yet. Like in the client, it has a durability of 2:
    /// the first usage saves the place of the character, the second one warps it back,
    /// see <see cref="GameLogic.PlayerActions.ItemConsumeActions.TalismanOfMobilityConsumeHandlerPlugIn"/>.
    /// It gets the rules of the <see cref="ItemRules"/> which block trading and selling it, and the attributes
    /// in which a character keeps the saved place are added, so that every update which creates it is complete.
    /// </summary>
    /// <returns>The created item, or <c>null</c>, if it already existed.</returns>
    internal ItemDefinition? CreateTalismanOfMobility()
    {
        this.AddMobilityTalismanAttributes();
        var talisman = this.CreateItem(13, 70, LocalizedString.FromResource(() => ItemNames.TalismanOfMobility));
        if (talisman is not null)
        {
            talisman.Durability = CashShopInitializer.TalismanOfMobilityDurability;
            ItemRules.Apply([talisman]);
        }

        return talisman;
    }

    /// <summary>
    /// Creates the Talisman of Resurrection, if it doesn't exist yet. When a player dies while it carries one
    /// in its inventory, it's respawned at the place of its death and the talisman is used up,
    /// see <see cref="GameLogic.PlugIns.TalismanOfResurrectionSpawnGatePlugIn"/>.
    /// The rules which block trading and selling it are applied by the <see cref="ItemRules"/>.
    /// </summary>
    /// <returns>The created item, or <c>null</c>, if it already existed.</returns>
    internal ItemDefinition? CreateTalismanOfResurrection()
    {
        return this.CreateItem(13, 69, LocalizedString.FromResource(() => ItemNames.TalismanOfResurrection));
    }

    /// <summary>
    /// Creates the reset fruits, which don't exist yet. A reset fruit takes back invested points of one stat
    /// and returns them as level up points. Each durability takes back ten points, and like in the client,
    /// a fruit has a durability of up to 100.
    /// The rules which block trading and selling them are applied by the <see cref="ItemRules"/>.
    /// </summary>
    /// <returns>The created items.</returns>
    internal IReadOnlyList<ItemDefinition> CreateResetFruits()
    {
        const byte group = 13;
        var fruits = new[]
            {
                this.CreateItem(group, 54, LocalizedString.FromResource(() => ItemNames.ResetFruitStrength)),
                this.CreateItem(group, 55, LocalizedString.FromResource(() => ItemNames.ResetFruitAgility)),
                this.CreateItem(group, 56, LocalizedString.FromResource(() => ItemNames.ResetFruitVitality)),
                this.CreateItem(group, 57, LocalizedString.FromResource(() => ItemNames.ResetFruitEnergy)),
                this.CreateItem(group, 58, LocalizedString.FromResource(() => ItemNames.ResetFruitCommand)),
            }
            .OfType<ItemDefinition>()
            .ToList();
        fruits.ForEach(fruit => fruit.Durability = CashShopInitializer.ResetFruitDurability);
        return fruits;
    }

    /// <summary>
    /// Adds the attributes in which a character keeps the place which it saved with the Talisman of Mobility.
    /// </summary>
    private void AddMobilityTalismanAttributes()
    {
        foreach (var attribute in new[] { Stats.MobilityTalismanMap, Stats.MobilityTalismanX, Stats.MobilityTalismanY })
        {
            if (this.GameConfiguration.Attributes.All(a => a.Id != attribute.Id))
            {
                this.GameConfiguration.Attributes.Add(this.Context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description));
            }
        }
    }

    private ItemDefinition? CreateItem(byte group, short number, LocalizedString name)
    {
        if (this.GameConfiguration.Items.Any(item => item.Group == group && item.Number == number))
        {
            return null;
        }

        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
        return item;
    }
}
