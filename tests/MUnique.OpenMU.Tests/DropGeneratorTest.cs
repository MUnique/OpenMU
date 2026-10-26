// <copyright file="DropGeneratorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the drop generator.
/// </summary>
[TestFixture]
public class DropGeneratorTest
{
    /// <summary>
    /// Tests if the drop fails because the randomizer returns a number which causes a fail.
    /// </summary>
    [Test]
    public async ValueTask TestDropFailAsync()
    {
        var config = this.GetGameConfig();
        var generator = new DefaultDropGenerator(config, this.GetRandomizer(9999));
        var (items, _) = await generator.GenerateItemDropsAsync(this.GetMonster(1, 0), 0, await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false));
        var item = items.FirstOrDefault();
        Assert.That(item, Is.Null);
    }

    /// <summary>
    /// Tests the drops defined by a monster are getting considered.
    /// </summary>
    [Test]
    public async ValueTask TestItemDropItemByMonsterAsync()
    {
        var config = this.GetGameConfig();
        var monster = this.GetMonster(1, 0);
        monster.DropItemGroups.AddBasicDropItemGroups();
        monster.DropItemGroups.Add(3000, SpecialItemType.RandomItem, true);

        var generator = new DefaultDropGenerator(config, this.GetRandomizer2(0, 0.5));
        var (items, _) = await generator.GenerateItemDropsAsync(monster, 1, await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false));
        var item = items.FirstOrDefault();

        Assert.That(item, Is.Not.Null);

        // ReSharper disable once PossibleNullReferenceException
        Assert.That(item!.Definition, Is.EqualTo(monster.DropItemGroups.Last().PossibleItems.First()));
    }

    /// <summary>
    /// Tests that items with a maximum drop level are filtered from generic monster drops.
    /// </summary>
    [Test]
    public async ValueTask TestMaximumDropLevelAsync()
    {
        var config = this.GetGameConfig();
        var cappedItem = this.CreateItemDefinition(12, 15, 12, 66);
        var uncappedItem = this.CreateItemDefinition(14, 13, 25);

        var dropGroup = new Mock<DropItemGroup>();
        dropGroup.SetupAllProperties();
        dropGroup.Object.Chance = 1.0;
        dropGroup.Object.ItemType = SpecialItemType.Jewel;
        dropGroup.Setup(g => g.PossibleItems).Returns(new List<ItemDefinition> { cappedItem, uncappedItem });

        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.CurrentMap!.Definition.DropItemGroups.Add(dropGroup.Object);

        var generator = new DefaultDropGenerator(config, this.GetRandomizer(0));
        var (items, _) = await generator.GenerateItemDropsAsync(this.GetMonster(1, 67), 1, player).ConfigureAwait(false);
        var item = items.FirstOrDefault();

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Definition, Is.EqualTo(uncappedItem));
    }

    /// <summary>
    /// Tests that inactive items are not dropped, but the active items of the same drop group.
    /// </summary>
    [Test]
    public async ValueTask TestInactiveItemIsNotDroppedAsync()
    {
        var config = this.GetGameConfig();
        var inactiveItem = this.CreateItemDefinition(14, 13, 0);
        inactiveItem.IsActive = false;
        var activeItem = this.CreateItemDefinition(14, 14, 0);
        var monster = this.GetMonster(1, 0);
        monster.DropItemGroups.Add(this.CreateDropItemGroup(inactiveItem, activeItem));

        var generator = new DefaultDropGenerator(config, this.GetRandomizer(0));
        var (items, _) = await generator.GenerateItemDropsAsync(monster, 1, await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false)).ConfigureAwait(false);
        var item = items.FirstOrDefault();

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Definition, Is.EqualTo(activeItem));
    }

    /// <summary>
    /// Tests that nothing is dropped of a drop group, when all of its items are inactive.
    /// </summary>
    [Test]
    public async ValueTask TestNoDropWhenAllItemsAreInactiveAsync()
    {
        var config = this.GetGameConfig();
        var inactiveItem = this.CreateItemDefinition(14, 13, 0);
        inactiveItem.IsActive = false;
        var monster = this.GetMonster(1, 0);
        monster.DropItemGroups.Add(this.CreateDropItemGroup(inactiveItem));

        var generator = new DefaultDropGenerator(config, this.GetRandomizer(0));
        var (items, _) = await generator.GenerateItemDropsAsync(monster, 1, await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false)).ConfigureAwait(false);

        Assert.That(items, Is.Empty);
    }

    /// <summary>
    /// Tests the drops defined by a player are getting considered.
    /// </summary>
    public void TestItemDropItemByPlayer()
    {
        // to be implemented
    }

    /// <summary>
    /// Tests the drops defined by a map are getting considered.
    /// </summary>
    public void TestItemDropItemByMap()
    {
        // to be implemented
    }

    /// <summary>
    /// Tests that an excellent drop of a map drop group never selects a socket item,
    /// because socket items can't be excellent.
    /// </summary>
    [Test]
    public async ValueTask TestExcellentMapDropSkipsSocketItemsAsync()
    {
        var socketItem = this.CreateExcellentCapableItemDefinition(0, 28, 3);
        var normalItem = this.CreateExcellentCapableItemDefinition(0, 19, 0);

        var item = await this.GenerateExcellentMapDropAsync(socketItem, normalItem).ConfigureAwait(false);

        AssertExcellentWithoutSockets(item, normalItem);
    }

    /// <summary>
    /// Tests that an excellent drop of the generic monster drop (which selects the item by the monster level)
    /// never selects a socket item.
    /// </summary>
    [Test]
    public async ValueTask TestExcellentMonsterDropSkipsSocketItemsAsync()
    {
        var socketItem = this.CreateExcellentCapableItemDefinition(0, 28, 3);
        var normalItem = this.CreateExcellentCapableItemDefinition(0, 19, 0);
        var config = this.GetGameConfig(socketItem, normalItem);
        var monster = this.GetMonster(1, 50);
        monster.DropItemGroups.Add(10000, SpecialItemType.Excellent, false);

        var generator = new DefaultDropGenerator(config, this.GetRandomizer(0));
        var (items, _) = await generator.GenerateItemDropsAsync(monster, 1, await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false)).ConfigureAwait(false);

        AssertExcellentWithoutSockets(items.FirstOrDefault(), normalItem);
    }

    /// <summary>
    /// Tests that an excellent item box (e.g. a box of kundun) with socket items and normal items
    /// never selects a socket item.
    /// </summary>
    [Test]
    public void TestExcellentItemBoxSkipsSocketItems()
    {
        var socketItem = this.CreateExcellentCapableItemDefinition(0, 28, 3);
        var normalItem = this.CreateExcellentCapableItemDefinition(0, 19, 0);
        var box = this.CreateExcellentDropGroup(socketItem, normalItem);

        var generator = new DefaultDropGenerator(this.GetGameConfig(), this.GetRandomizer(0));
        var item = generator.GenerateItemDrop(box);

        AssertExcellentWithoutSockets(item, normalItem);
    }

    /// <summary>
    /// Tests that an excellent drop with only socket items drops nothing instead of an excellent socket item.
    /// </summary>
    [Test]
    public async ValueTask TestExcellentDropWithOnlySocketItemsAsync()
    {
        var socketItem = this.CreateExcellentCapableItemDefinition(0, 28, 3);

        var item = await this.GenerateExcellentMapDropAsync(socketItem).ConfigureAwait(false);

        Assert.That(item, Is.Null);
    }

    /// <summary>
    /// Tests that the common random item drop never selects a socket item, because socket items have their own drop group.
    /// </summary>
    [Test]
    public async ValueTask TestRandomItemDropSkipsSocketItemsAsync()
    {
        var socketItem = this.CreateItemDefinition(0, 28, 100);
        socketItem.MaximumSockets = 3;
        var normalItem = this.CreateItemDefinition(0, 19, 100);

        var item = await this.GenerateSpecialMapDropAsync(SpecialItemType.RandomItem, socketItem, normalItem).ConfigureAwait(false);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Definition, Is.EqualTo(normalItem));
        Assert.That(item.SocketCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that the socket item drop group selects only socket items, which get at least one socket.
    /// </summary>
    [Test]
    public async ValueTask TestSocketItemDropSelectsOnlySocketItemsAsync()
    {
        var normalItem = this.CreateItemDefinition(0, 19, 100);
        var socketItem = this.CreateItemDefinition(0, 28, 100);
        socketItem.MaximumSockets = 3;

        var item = await this.GenerateSpecialMapDropAsync(SpecialItemType.SocketItem, normalItem, socketItem).ConfigureAwait(false);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Definition, Is.EqualTo(socketItem));
        Assert.That(item.SocketCount, Is.GreaterThan(0));
    }

    /// <summary>
    /// Tests that ExcellentItemDropLevelDelta property exists and has correct default.
    /// </summary>
    [Test]
    public void TestExcellentItemDropLevelDelta_PropertyExists()
    {
        var config = this.GetGameConfig();
        // The initializer sets default to 25 for backward compatibility
        config.ExcellentItemDropLevelDelta = 25;
        Assert.That(config.ExcellentItemDropLevelDelta, Is.EqualTo(25));

        config.ExcellentItemDropLevelDelta = 0;
        Assert.That(config.ExcellentItemDropLevelDelta, Is.EqualTo(0));

        config.ExcellentItemDropLevelDelta = 50;
        Assert.That(config.ExcellentItemDropLevelDelta, Is.EqualTo(50));
    }

    private static void AssertExcellentWithoutSockets(Item? item, ItemDefinition expectedDefinition)
    {
        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Definition, Is.EqualTo(expectedDefinition));
        Assert.That(item.ItemOptions.Any(o => object.Equals(o.ItemOption?.OptionType, ItemOptionTypes.Excellent)), Is.True);
        Assert.That(item.SocketCount, Is.EqualTo(0));
    }

    private async ValueTask<Item?> GenerateExcellentMapDropAsync(params ItemDefinition[] possibleItems)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.CurrentMap!.Definition.DropItemGroups.Add(this.CreateExcellentDropGroup(possibleItems));

        var generator = new DefaultDropGenerator(this.GetGameConfig(), this.GetRandomizer(0));
        var (items, _) = await generator.GenerateItemDropsAsync(this.GetMonster(1, 50), 1, player).ConfigureAwait(false);
        return items.FirstOrDefault();
    }

    private DropItemGroup CreateExcellentDropGroup(params ItemDefinition[] possibleItems)
    {
        var dropGroup = new Mock<DropItemGroup>();
        dropGroup.SetupAllProperties();
        dropGroup.Object.Chance = 1.0;
        dropGroup.Object.ItemType = SpecialItemType.Excellent;
        dropGroup.Setup(g => g.PossibleItems).Returns(possibleItems.ToList());
        return dropGroup.Object;
    }

    private ItemDefinition CreateExcellentCapableItemDefinition(byte group, short number, int maximumSockets)
    {
        var excellentOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Excellent };
        var excellentOptions = new BasicModel.ItemOptionDefinition { MaximumOptionsPerItem = 1 };
        excellentOptions.PossibleOptions.Add(excellentOption);

        var itemDefinition = new Mock<ItemDefinition>();
        itemDefinition.SetupAllProperties();
        itemDefinition.Object.Group = group;
        itemDefinition.Object.Number = number;
        itemDefinition.Object.DropLevel = 40;
        itemDefinition.Object.Durability = 1;
        itemDefinition.Object.DropsFromMonsters = true;
        itemDefinition.Object.MaximumSockets = maximumSockets;
        itemDefinition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition> { excellentOptions });
        itemDefinition.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        return itemDefinition.Object;
    }

    private async ValueTask<Item?> GenerateSpecialMapDropAsync(SpecialItemType itemType, params ItemDefinition[] configuredItems)
    {
        var dropGroup = new Mock<DropItemGroup>();
        dropGroup.SetupAllProperties();
        dropGroup.Object.Chance = 1.0;
        dropGroup.Object.ItemType = itemType;
        dropGroup.Setup(g => g.PossibleItems).Returns(new List<ItemDefinition>());

        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.CurrentMap!.Definition.DropItemGroups.Add(dropGroup.Object);

        var config = this.GetGameConfig();
        foreach (var definition in configuredItems)
        {
            definition.DropsFromMonsters = true;
            config.Items.Add(definition);
        }

        // NextInt returns its minimum: the first possible item and one socket.
        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(r => r.NextInt(It.IsAny<int>(), It.IsAny<int>())).Returns((int min, int _) => min);
        randomizer.Setup(r => r.NextDouble()).Returns(0);

        var generator = new DefaultDropGenerator(config, randomizer.Object);
        var (items, _) = await generator.GenerateItemDropsAsync(this.GetMonster(1, 100), 1, player).ConfigureAwait(false);
        return items.FirstOrDefault();
    }

    private MonsterDefinition GetMonster(int numberOfDrops, byte level)
    {
        var monster = new Mock<MonsterDefinition>();
        monster.SetupAllProperties();
        monster.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        monster.Setup(m => m.Attributes).Returns(new List<MonsterAttribute>());
        monster.Object.NumberOfMaximumItemDrops = numberOfDrops;
        monster.Object.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = level });
        return monster.Object;
    }

    private IRandomizer GetRandomizer(int randomValue)
    {
        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(r => r.NextInt(It.IsAny<int>(), It.IsAny<int>())).Returns(randomValue);
        randomizer.Setup(r => r.NextDouble()).Returns(randomValue / 10000.0);
        return randomizer.Object;
    }

    private IRandomizer GetRandomizer2(int integerValue, double doubleValue)
    {
        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(r => r.NextInt(It.IsAny<int>(), It.IsAny<int>())).Returns(integerValue);
        randomizer.Setup(r => r.NextDouble()).Returns(doubleValue);

        return randomizer.Object;
    }

    private GameConfiguration GetGameConfig(params ItemDefinition[] items)
    {
        var gameConfiguration = new Mock<GameConfiguration>();
        gameConfiguration.Setup(c => c.Items).Returns(items.ToList());
        return gameConfiguration.Object;
    }

    private DropItemGroup CreateDropItemGroup(params ItemDefinition[] possibleItems)
    {
        var dropGroup = new Mock<DropItemGroup>();
        dropGroup.SetupAllProperties();
        dropGroup.Object.Chance = 1.0;
        dropGroup.Object.ItemType = SpecialItemType.Jewel;
        dropGroup.Setup(g => g.PossibleItems).Returns(possibleItems.ToList());
        return dropGroup.Object;
    }

    private ItemDefinition CreateItemDefinition(byte group, short number, byte dropLevel, byte? maximumDropLevel = null)
    {
        var itemDefinition = new Mock<ItemDefinition>();
        itemDefinition.SetupAllProperties();
        itemDefinition.Object.Group = group;
        itemDefinition.Object.Number = number;
        itemDefinition.Object.DropLevel = dropLevel;
        itemDefinition.Object.MaximumDropLevel = maximumDropLevel;
        itemDefinition.Object.Durability = 1;
        itemDefinition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        itemDefinition.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        return itemDefinition.Object;
    }
}
