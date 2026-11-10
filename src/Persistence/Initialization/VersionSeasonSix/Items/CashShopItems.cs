// <copyright file="CashShopItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
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
        this.CreateFigurinesAndCharms();
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
    /// Creates the figurines and charms, which don't exist yet. They're used inside the inventory
    /// with a right click, see <see cref="GameLogic.PlayerActions.Items.UseInventoryItemAction"/>.
    /// Their durability is 1, so that they're not stackable; the game logic sets the durability of an item
    /// to 255 when it's added to the inventory, which tells the client that it's not in use.
    /// Their bonuses are the ones which the client shows. The worn horseshoe (13/134) increases the item drop rate, for which there's no attribute yet.
    /// </summary>
    /// <returns>The created items.</returns>
    internal IReadOnlyList<ItemDefinition> CreateFigurinesAndCharms()
    {
        const byte group = 13;
        var items = new List<ItemDefinition>();
        void Create(short number, LocalizedString name, byte height, params (AttributeDefinition Attribute, float Value)[] bonuses)
        {
            if (this.CreateItem(group, number, name) is not { } item)
            {
                return;
            }

            item.Height = height;
            item.Durability = 1;
            foreach (var (attribute, value) in bonuses)
            {
                item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(attribute, value, AggregateType.AddRaw));
            }

            items.Add(item);
        }

        Create(128, LocalizedString.FromResource(() => ItemNames.HawkFigurine), 1, (Stats.CriticalDamageBonus, 10));
        Create(129, LocalizedString.FromResource(() => ItemNames.GoatFigurine), 1, (Stats.ExcellentDamageBonus, 10));
        Create(130, LocalizedString.FromResource(() => ItemNames.OakCharm), 1, (Stats.MaximumHealth, 50));
        Create(131, LocalizedString.FromResource(() => ItemNames.MapleCharm), 1, (Stats.MaximumMana, 50));
        Create(132, LocalizedString.FromResource(() => ItemNames.GoldenOakCharm), 2, (Stats.MaximumHealth, 150), (Stats.MaximumShield, 50));
        Create(133, LocalizedString.FromResource(() => ItemNames.GoldenMapleCharm), 2, (Stats.MaximumMana, 150), (Stats.MaximumAbility, 50));
        return items;
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
