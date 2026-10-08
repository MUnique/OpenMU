// <copyright file="MonsterItemDroppedPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the <see cref="IMonsterItemDroppedPlugIn"/>.
/// </summary>
[TestFixture]
public class MonsterItemDroppedPlugInTest
{
    /// <summary>
    /// Tests that the item which a killed monster dropped is reported with the monster and the killer.
    /// </summary>
    [Test]
    public async Task DroppedItemIsReportedAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var plugIn = new RecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IMonsterItemDroppedPlugIn>(plugIn);
        var item = new Item { Definition = new ItemDefinition() };
        var dropGenerator = new Mock<IDropGenerator>();
        dropGenerator.Setup(g => g.GenerateItemDropsAsync(It.IsAny<DataModel.Configuration.MonsterDefinition>(), It.IsAny<int>(), It.IsAny<Player>()))
            .Returns(ValueTask.FromResult<(IEnumerable<Item> Items, uint? Money)>((new[] { item }, null)));
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var monster = await CreateMonsterAsync(gameContext, dropGenerator.Object).ConfigureAwait(false);

        await monster.ReflectDamageAsync(killer, 1_000_000).ConfigureAwait(false);

        // The drop happens delayed, after the death animation.
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (plugIn.Drops.IsEmpty && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }

        Assert.That(plugIn.Drops, Has.Count.EqualTo(1));
        var (reportedMonster, reportedKiller, droppedItem) = plugIn.Drops.Single();
        Assert.That(reportedMonster, Is.SameAs(monster));
        Assert.That(reportedKiller, Is.SameAs(killer));
        Assert.That(droppedItem.Item, Is.SameAs(item));
    }

    private static async ValueTask<Monster> CreateMonsterAsync(IGameContext gameContext, IDropGenerator dropGenerator)
    {
        // A unique id, because the attributes of monster definitions are cached by their id.
        var monsterDefinition = new MonsterDefinition { Id = Guid.NewGuid(), ObjectKind = NpcObjectKind.Monster };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
        };

        var monster = new Monster(
            spawnArea,
            monsterDefinition,
            map,
            dropGenerator,
            new Mock<INpcIntelligence>().Object,
            gameContext.PlugInManager,
            gameContext.PathFinderPool);
        monster.Initialize();
        await map.AddAsync(monster).ConfigureAwait(false);
        return monster;
    }

    [Guid("9635516F-6DC6-446E-88D3-8DD33CA5C42D")]
    private sealed class RecordingPlugIn : IMonsterItemDroppedPlugIn
    {
        public System.Collections.Concurrent.ConcurrentBag<(AttackableNpcBase Monster, Player Killer, DroppedItem Item)> Drops { get; } = new();

        public ValueTask MonsterItemDroppedAsync(AttackableNpcBase monster, Player killer, DroppedItem droppedItem)
        {
            this.Drops.Add((monster, killer, droppedItem));
            return ValueTask.CompletedTask;
        }
    }
}
