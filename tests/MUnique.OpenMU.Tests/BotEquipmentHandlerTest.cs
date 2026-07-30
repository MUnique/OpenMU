// <copyright file="BotEquipmentHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests which gear a bot considers an upgrade (<see cref="BotEquipmentHandler.IsUpgradeFor"/>, also
/// the pickup filter of the offline <see cref="MUnique.OpenMU.GameLogic.Offline.ItemPickupHandler"/>)
/// and the hand rule it shares with the engine (<see cref="ItemExtensions.ConflictsWithEquippedHands"/>).
/// </summary>
[TestFixture]
public class BotEquipmentHandlerTest
{
    private const byte StaffGroup = 5;
    private const byte ShieldGroup = 6;
    private const byte ArmorGroup = 8;

    /// <summary>
    /// A better weapon of the bot's own fighting style is an upgrade worth picking up. The test player's
    /// class is energy-based, so its style is the staff (see <see cref="BotProgression.IsPreferredWeaponGroup"/>).
    /// </summary>
    [Test]
    public async ValueTask BetterWeaponIsAnUpgradeAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, StaffGroup, 1, dropLevel: 10), InventoryConstants.LeftHandSlot).ConfigureAwait(false);
        var better = CreateItem(CreateDefinition(player, StaffGroup, 2, dropLevel: 40));

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, better), Is.True);
    }

    /// <summary>
    /// A two-handed weapon needs the other hand: while a shield is worn, it is only worth it if it beats
    /// the weapon AND the shield it displaces. Without counting the shield, the bot took its gear off,
    /// had the equip refused by the engine (a two-hander needs the hand free), put the old weapon back on
    /// and started over - hundreds of swaps an hour, unarmed half of the time.
    /// </summary>
    [Test]
    public async ValueTask TwoHandedWeaponIsNoUpgradeWhenItLosesToWeaponAndShieldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, StaffGroup, 1, dropLevel: 50), InventoryConstants.LeftHandSlot).ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, ShieldGroup, 1, dropLevel: 40, slot: InventoryConstants.RightHandSlot), InventoryConstants.RightHandSlot).ConfigureAwait(false);

        var twoHanded = CreateItem(CreateDefinition(player, StaffGroup, 3, dropLevel: 60, width: 2));

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, twoHanded), Is.False);
    }

    /// <summary>
    /// A two-handed weapon which beats the worn weapon and shield together is worth the swap - the bot
    /// frees the hand for it, like a player would.
    /// </summary>
    [Test]
    public async ValueTask TwoHandedWeaponIsAnUpgradeWhenItBeatsWeaponAndShieldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, StaffGroup, 1, dropLevel: 50), InventoryConstants.LeftHandSlot).ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, ShieldGroup, 1, dropLevel: 40, slot: InventoryConstants.RightHandSlot), InventoryConstants.RightHandSlot).ConfigureAwait(false);

        var twoHanded = CreateItem(CreateDefinition(player, StaffGroup, 3, dropLevel: 120, width: 2));

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, twoHanded), Is.True);
    }

    /// <summary>
    /// Without a shield in the way, the same two-handed weapon is a welcome upgrade.
    /// </summary>
    [Test]
    public async ValueTask TwoHandedWeaponIsAnUpgradeWithFreeOffHandAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, StaffGroup, 1, dropLevel: 10), InventoryConstants.LeftHandSlot).ConfigureAwait(false);

        var twoHanded = CreateItem(CreateDefinition(player, StaffGroup, 3, dropLevel: 60, width: 2));

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, twoHanded), Is.True);
    }

    /// <summary>
    /// A weapon never goes into the free off-hand: a bot dual-wielding the junk weapons it happens to be
    /// qualified for is neither useful nor a sight a real character offers.
    /// </summary>
    [Test]
    public async ValueTask JunkWeaponIsNoUpgradeForTheFreeOffHandAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await WearAsync(player, CreateDefinition(player, StaffGroup, 1, dropLevel: 50), InventoryConstants.LeftHandSlot).ConfigureAwait(false);

        var junk = CreateItem(CreateDefinition(player, StaffGroup, 2, dropLevel: 5));

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, junk), Is.False);
    }

    /// <summary>
    /// Gear the bot's class cannot wear is no upgrade, however good it is.
    /// </summary>
    [Test]
    public async ValueTask UnqualifiedGearIsNoUpgradeAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var definition = CreateDefinition(player, ArmorGroup, 1, dropLevel: 80, slot: InventoryConstants.ArmorSlot);
        definition.QualifiedCharacters.Clear();

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, CreateItem(definition)), Is.False);
    }

    /// <summary>
    /// An empty armor slot takes any qualified piece - that is what makes a naked bot dress itself.
    /// </summary>
    [Test]
    public async ValueTask ArmorForAnEmptySlotIsAnUpgradeAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var armor = CreateItem(CreateDefinition(player, ArmorGroup, 1, dropLevel: 20, slot: InventoryConstants.ArmorSlot));

        Assert.That(BotEquipmentHandler.IsUpgradeFor(player, armor), Is.True);
    }

    private static ItemDefinition CreateDefinition(Player player, byte group, short number, byte dropLevel, byte width = 1, int? slot = null)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.QualifiedCharacters).Returns(new List<CharacterClass>());
        definitionMock.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definitionMock.Setup(d => d.Requirements).Returns(new List<AttributeRequirement>());

        var slotTypeMock = new Mock<ItemSlotType>();
        var targetSlot = slot ?? InventoryConstants.LeftHandSlot;
        var slots = targetSlot == InventoryConstants.LeftHandSlot && group <= ShieldGroup && width < 2
            ? new List<int> { InventoryConstants.LeftHandSlot, InventoryConstants.RightHandSlot }
            : new List<int> { targetSlot };
        slotTypeMock.Setup(s => s.ItemSlots).Returns(slots);
        definitionMock.Setup(d => d.ItemSlot).Returns(slotTypeMock.Object);

        var definition = definitionMock.Object;
        definition.Group = group;
        definition.Number = number;
        definition.Width = width;
        definition.Height = 2;
        definition.Durability = 100;
        definition.DropLevel = dropLevel;
        definition.QualifiedCharacters.Add(player.SelectedCharacter!.CharacterClass!);
        player.GameContext.Configuration.Items.Add(definition);
        return definition;
    }

    private static Item CreateItem(ItemDefinition definition)
    {
        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        itemMock.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var item = itemMock.Object;
        item.Definition = definition;
        item.Durability = definition.Durability;
        return item;
    }

    private static async ValueTask<Item> WearAsync(Player player, ItemDefinition definition, byte slot)
    {
        var item = CreateItem(definition);
        await player.Inventory!.AddItemAsync(slot, item).ConfigureAwait(false);
        return item;
    }
}
