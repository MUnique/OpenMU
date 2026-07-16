// <copyright file="BotJewelHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests the jewel usage policy of <see cref="BotJewelHandler"/> - which jewel a bot picks for which
/// equipped item; the actual consumption goes through the regular consume handlers and is covered by
/// <see cref="ItemConsumptionTest"/>.
/// </summary>
[TestFixture]
public class BotJewelHandlerTest
{
    private const byte FirstBackpackSlot = 12;

    /// <summary>
    /// A Bless in stock and an equipped piece below +6: the weakest piece is chosen for the Bless.
    /// </summary>
    [Test]
    public async ValueTask PrefersBlessOnWeakestUpgradeableItemAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        // The stronger piece is the one the bot must NOT pick, so it only has to be there.
        await AddEquippedItemAsync(player, InventoryConstants.LeftHandSlot, 4).ConfigureAwait(false);
        var weakPiece = await AddEquippedItemAsync(player, InventoryConstants.RightHandSlot, 2).ConfigureAwait(false);
        var bless = await AddJewelAsync(player, FirstBackpackSlot, ItemConstants.JewelOfBless).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot + 1, ItemConstants.JewelOfSoul).ConfigureAwait(false);

        var plan = BotJewelHandler.PlanNextUse(player, false);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.Value.Jewel, Is.SameAs(bless));
        Assert.That(plan.Value.Target, Is.SameAs(weakPiece));
        Assert.That(plan.Value.IsLife, Is.False);
    }

    /// <summary>
    /// All gear at +6 or above: a Soul is only risked with a spare in stock.
    /// </summary>
    /// <param name="soulCount">The number of souls in the backpack.</param>
    /// <param name="expectsUse">Whether a jewel use is expected.</param>
    [TestCase(1, false)]
    [TestCase(2, true)]
    public async ValueTask RisksSoulOnlyWithSpareStockAsync(int soulCount, bool expectsUse)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddEquippedItemAsync(player, InventoryConstants.LeftHandSlot, 6).ConfigureAwait(false);
        for (var i = 0; i < soulCount; i++)
        {
            await AddJewelAsync(player, (byte)(FirstBackpackSlot + i), ItemConstants.JewelOfSoul).ConfigureAwait(false);
        }

        var plan = BotJewelHandler.PlanNextUse(player, false);

        Assert.That(plan.HasValue, Is.EqualTo(expectsUse));
    }

    /// <summary>
    /// The Soul prefers a lucky target (its success bonus) over a lower-level one without luck.
    /// </summary>
    [Test]
    public async ValueTask SoulPrefersLuckyItemAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddEquippedItemAsync(player, InventoryConstants.LeftHandSlot, 6).ConfigureAwait(false);
        var luckyPiece = await AddEquippedItemAsync(player, InventoryConstants.RightHandSlot, 7, withLuck: true).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot, ItemConstants.JewelOfSoul).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot + 1, ItemConstants.JewelOfSoul).ConfigureAwait(false);

        var plan = BotJewelHandler.PlanNextUse(player, false);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.Value.Target, Is.SameAs(luckyPiece));
    }

    /// <summary>
    /// Without luck the Soul risk stops at +6 (a failure from +7 on resets the item to +0), so only
    /// lucky items may be pushed further - up to the jewel ceiling of +9.
    /// </summary>
    /// <param name="itemLevel">The level of the equipped item.</param>
    /// <param name="withLuck">Whether the equipped item has luck.</param>
    /// <param name="expectsUse">Whether a jewel use is expected.</param>
    [TestCase(7, false, false)]
    [TestCase(7, true, true)]
    [TestCase(8, true, true)]
    [TestCase(9, true, false)]
    public async ValueTask RisksSoulAbovePlusSixOnlyWithLuckAsync(byte itemLevel, bool withLuck, bool expectsUse)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddEquippedItemAsync(player, InventoryConstants.LeftHandSlot, itemLevel, withLuck).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot, ItemConstants.JewelOfSoul).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot + 1, ItemConstants.JewelOfSoul).ConfigureAwait(false);

        var plan = BotJewelHandler.PlanNextUse(player, false);

        Assert.That(plan.HasValue, Is.EqualTo(expectsUse));
    }

    /// <summary>
    /// Life is the last resort and is planned at most once per trip.
    /// </summary>
    /// <param name="lifeAlreadyUsed">Whether a life was already used within the trip.</param>
    /// <param name="expectsUse">Whether a jewel use is expected.</param>
    [TestCase(false, true)]
    [TestCase(true, false)]
    public async ValueTask UsesLifeAtMostOncePerTripAsync(bool lifeAlreadyUsed, bool expectsUse)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = await AddEquippedItemAsync(player, InventoryConstants.LeftHandSlot, 9).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot, ItemConstants.JewelOfLife).ConfigureAwait(false);
        await AddJewelAsync(player, FirstBackpackSlot + 1, ItemConstants.JewelOfLife).ConfigureAwait(false);

        var plan = BotJewelHandler.PlanNextUse(player, lifeAlreadyUsed);

        Assert.That(plan.HasValue, Is.EqualTo(expectsUse));
        if (expectsUse)
        {
            Assert.That(plan!.Value.Target, Is.SameAs(target));
            Assert.That(plan.Value.IsLife, Is.True);
        }
    }

    /// <summary>
    /// Without any applicable jewel or target, nothing is planned.
    /// </summary>
    [Test]
    public async ValueTask PlansNothingWithoutJewelsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddEquippedItemAsync(player, InventoryConstants.LeftHandSlot, 2).ConfigureAwait(false);

        var plan = BotJewelHandler.PlanNextUse(player, false);

        Assert.That(plan, Is.Null);
    }

    private static async ValueTask<Item> AddEquippedItemAsync(Player player, byte slot, byte level, bool withLuck = false)
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        var itemOptions = new List<ItemOptionLink>();
        item.Setup(i => i.ItemOptions).Returns(itemOptions);
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var definition = new Mock<ItemDefinition>();
        definition.SetupAllProperties();
        var itemSlot = new Mock<ItemSlotType>();
        itemSlot.Setup(s => s.ItemSlots).Returns(new List<int> { slot });
        definition.Setup(d => d.ItemSlot).Returns(itemSlot.Object);
        item.Object.Definition = definition.Object;
        item.Object.Definition.Width = 1;
        item.Object.Definition.Height = 1;
        item.Object.Definition.MaximumItemLevel = 15;
        item.Object.Definition.Durability = 1;
        item.Object.Durability = 1;
        item.Object.Level = level;

        if (withLuck)
        {
            var optionLink = new Mock<ItemOptionLink>();
            optionLink.SetupAllProperties();
            var option = new Mock<IncreasableItemOption>();
            option.SetupAllProperties();
            option.Object.OptionType = ItemOptionTypes.Luck;
            optionLink.Object.ItemOption = option.Object;
            itemOptions.Add(optionLink.Object);
        }

        await player.Inventory!.AddItemAsync(slot, item.Object).ConfigureAwait(false);
        return item.Object;
    }

    private static async ValueTask<Item> AddJewelAsync(Player player, int slot, ItemIdentifier identifier)
    {
        var jewel = new Item
        {
            Definition = new ItemDefinition
            {
                Number = identifier.Number ?? 0,
                Group = identifier.Group,
                Width = 1,
                Height = 1,
            },
            Durability = 1,
        };
        await player.Inventory!.AddItemAsync((byte)slot, jewel).ConfigureAwait(false);
        return jewel;
    }
}
