// <copyright file="SocketCountDropTest.cs" company="MUnique">
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
/// Tests the number of sockets of dropped socket items, which is selected by
/// <see cref="ItemDefinition.SocketCountTable"/> and <see cref="DropItemGroup.SocketCountTable"/>.
/// </summary>
[TestFixture]
public class SocketCountDropTest
{
    /// <summary>
    /// Tests that without a table, the number of sockets is selected uniformly, as before.
    /// </summary>
    [Test]
    public async ValueTask TestWithoutTableSelectsUniformlyAsync()
    {
        var socketItem = CreateSocketItemDefinition(maximumSockets: 3);

        var item = await GenerateSocketItemMapDropAsync(socketItem, groupTable: null, randomDouble: 0.99, randomInt: 2).ConfigureAwait(false);

        Assert.That(item?.SocketCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the table of the item definition selects the number of sockets by its weights.
    /// </summary>
    /// <param name="randomDouble">The random value between 0 and 1.</param>
    /// <param name="expectedSocketCount">The expected number of sockets.</param>
    [TestCase(0.0, 1)]
    [TestCase(0.839, 1)]
    [TestCase(0.841, 2)]
    [TestCase(0.994, 2)]
    [TestCase(0.996, 3)]
    public async ValueTask TestItemTableSelectsByWeightAsync(double randomDouble, int expectedSocketCount)
    {
        var socketItem = CreateSocketItemDefinition(maximumSockets: 5);
        socketItem.SocketCountTable = CreateTable((1, 84), (2, 15.5), (3, 0.5), (4, 0), (5, 0));

        var item = await GenerateSocketItemMapDropAsync(socketItem, groupTable: null, randomDouble).ConfigureAwait(false);

        Assert.That(item?.SocketCount, Is.EqualTo(expectedSocketCount));
    }

    /// <summary>
    /// Tests that values of the table above the maximum number of sockets of the item are never selected.
    /// </summary>
    [Test]
    public async ValueTask TestItemTableIgnoresValuesAboveMaximumAsync()
    {
        var socketItem = CreateSocketItemDefinition(maximumSockets: 3);
        socketItem.SocketCountTable = CreateTable((1, 1), (5, 1000));

        var item = await GenerateSocketItemMapDropAsync(socketItem, groupTable: null, randomDouble: 0.99).ConfigureAwait(false);

        Assert.That(item?.SocketCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a table without a selectable value falls back to the uniform selection,
    /// so that a socket item doesn't drop without sockets.
    /// </summary>
    [Test]
    public async ValueTask TestTableWithoutSelectableValueFallsBackToUniformAsync()
    {
        var socketItem = CreateSocketItemDefinition(maximumSockets: 3);
        socketItem.SocketCountTable = CreateTable((1, 0), (4, 1));

        var item = await GenerateSocketItemMapDropAsync(socketItem, groupTable: null, randomDouble: 0.5, randomInt: 3).ConfigureAwait(false);

        Assert.That(item?.SocketCount, Is.EqualTo(3));
    }

    /// <summary>
    /// Tests that the table of the drop group overrides the table of the item definition.
    /// </summary>
    [Test]
    public async ValueTask TestGroupTableOverridesItemTableAsync()
    {
        var socketItem = CreateSocketItemDefinition(maximumSockets: 5);
        socketItem.SocketCountTable = CreateTable((1, 84), (2, 15.5), (3, 0.5));

        var item = await GenerateSocketItemMapDropAsync(socketItem, CreateTable((4, 1)), randomDouble: 0.5).ConfigureAwait(false);

        Assert.That(item?.SocketCount, Is.EqualTo(4));
    }

    /// <summary>
    /// Tests that the table of a drop group with configured items, e.g. of a box, selects the number of sockets.
    /// </summary>
    [Test]
    public void TestGroupTableOfConfiguredItems()
    {
        var socketItem = CreateSocketItemDefinition(maximumSockets: 5);
        socketItem.SocketCountTable = CreateTable((1, 1));
        var group = CreateDropGroup(SpecialItemType.None, CreateTable((5, 1)), socketItem);
        var generator = new DefaultDropGenerator(CreateGameConfiguration(socketItem), CreateRandomizer(0.5, 1));

        var item = generator.GenerateItemDrop(group);

        Assert.That(item?.SocketCount, Is.EqualTo(5));
    }

    /// <summary>
    /// Tests that the table of a drop group doesn't give sockets to items which can't have sockets.
    /// </summary>
    [Test]
    public void TestGroupTableDoesNotAddSocketsToNormalItems()
    {
        var normalItem = CreateSocketItemDefinition(maximumSockets: 0);
        var group = CreateDropGroup(SpecialItemType.None, CreateTable((3, 1)), normalItem);
        var generator = new DefaultDropGenerator(CreateGameConfiguration(normalItem), CreateRandomizer(0.5, 1));

        var item = generator.GenerateItemDrop(group);

        Assert.That(item?.SocketCount, Is.EqualTo(0));
    }

    private static async ValueTask<Item?> GenerateSocketItemMapDropAsync(ItemDefinition socketItem, WeightedValueTable? groupTable, double randomDouble, int? randomInt = null)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.CurrentMap!.Definition.DropItemGroups.Add(CreateDropGroup(SpecialItemType.SocketItem, groupTable));

        var generator = new DefaultDropGenerator(CreateGameConfiguration(socketItem), CreateRandomizer(randomDouble, randomInt));
        var (items, _) = await generator.GenerateItemDropsAsync(CreateMonster(level: 100), 1, player).ConfigureAwait(false);
        return items.FirstOrDefault();
    }

    private static WeightedValueTable CreateTable(params (int Value, double Weight)[] values)
    {
        var table = new BasicModel.WeightedValueTable();
        foreach (var (value, weight) in values)
        {
            table.Values.Add(new BasicModel.WeightedValue { Value = value, Weight = weight });
        }

        return table;
    }

    /// <summary>
    /// Creates a randomizer which returns <paramref name="randomDouble"/> for <see cref="IRandomizer.NextDouble"/>,
    /// and <paramref name="randomInt"/> (or the minimum, if <c>null</c>) for <see cref="IRandomizer.NextInt"/>.
    /// </summary>
    private static IRandomizer CreateRandomizer(double randomDouble, int? randomInt = null)
    {
        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(r => r.NextInt(It.IsAny<int>(), It.IsAny<int>()))
            .Returns((int min, int max) => randomInt is { } value && value >= min && value < max ? value : min);
        randomizer.Setup(r => r.NextDouble()).Returns(randomDouble);
        return randomizer.Object;
    }

    private static DropItemGroup CreateDropGroup(SpecialItemType itemType, WeightedValueTable? socketCountTable, params ItemDefinition[] possibleItems)
    {
        var dropGroup = new Mock<DropItemGroup>();
        dropGroup.SetupAllProperties();
        dropGroup.Object.Chance = 1.0;
        dropGroup.Object.ItemType = itemType;
        dropGroup.Object.SocketCountTable = socketCountTable;
        dropGroup.Setup(g => g.PossibleItems).Returns(possibleItems.ToList());
        return dropGroup.Object;
    }

    private static ItemDefinition CreateSocketItemDefinition(int maximumSockets)
    {
        var itemDefinition = new Mock<ItemDefinition>();
        itemDefinition.SetupAllProperties();
        itemDefinition.Object.Group = 0;
        itemDefinition.Object.Number = 28;
        itemDefinition.Object.DropLevel = 100;
        itemDefinition.Object.Durability = 1;
        itemDefinition.Object.DropsFromMonsters = true;
        itemDefinition.Object.MaximumSockets = maximumSockets;
        itemDefinition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        itemDefinition.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        return itemDefinition.Object;
    }

    private static GameConfiguration CreateGameConfiguration(params ItemDefinition[] items)
    {
        var gameConfiguration = new Mock<GameConfiguration>();
        gameConfiguration.Setup(c => c.Items).Returns(items.ToList());
        return gameConfiguration.Object;
    }

    private static MonsterDefinition CreateMonster(byte level)
    {
        var monster = new Mock<MonsterDefinition>();
        monster.SetupAllProperties();
        monster.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        monster.Setup(m => m.Attributes).Returns(new List<MonsterAttribute>());
        monster.Object.NumberOfMaximumItemDrops = 1;
        monster.Object.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = level });
        return monster.Object;
    }
}
