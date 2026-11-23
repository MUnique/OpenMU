// <copyright file="MiniGameMoneyDropTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.MiniGames;

using System.Runtime.InteropServices;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using Item = MUnique.OpenMU.DataModel.Entities.Item;

/// <summary>
/// Tests the money of monsters which are killed in a mini game.
/// </summary>
[TestFixture]
public class MiniGameMoneyDropTest
{
    private const uint MoneyAmount = 1234;

    /// <summary>
    /// Tests that the money of a monster which is killed in a mini game is dropped like anywhere else,
    /// instead of being added to the inventory of the killer.
    /// </summary>
    [Test]
    public async Task MoneyIsDroppedInMiniGameAsync()
    {
        var (gameContext, killer, monster, plugIn) = await CreateWorldAsync(shouldDropMoney: true).ConfigureAwait(false);
        await using var miniGame = CreateMiniGame(gameContext, killer);
        killer.CurrentMiniGame = miniGame;
        var moneyBefore = killer.Money;

        await monster.ApplyDamageAsync(killer, 1_000_000).ConfigureAwait(false);
        await WaitForAsync(() => !plugIn.Drops.IsEmpty).ConfigureAwait(false);

        var (droppingMonster, reportedKiller, droppedMoney) = plugIn.Drops.Single();
        Assert.That(droppingMonster, Is.SameAs(monster));
        Assert.That(reportedKiller, Is.SameAs(killer));
        Assert.That(droppedMoney.Amount, Is.EqualTo(MoneyAmount));
        Assert.That(killer.Money, Is.EqualTo(moneyBefore));
    }

    /// <summary>
    /// Tests that the money of a monster which is killed in a mini game is added to the inventory,
    /// when the server is configured to not drop money.
    /// </summary>
    [Test]
    public async Task MoneyIsAddedToInventoryWhenMoneyIsNotDroppedAsync()
    {
        var (gameContext, killer, monster, plugIn) = await CreateWorldAsync(shouldDropMoney: false).ConfigureAwait(false);
        await using var miniGame = CreateMiniGame(gameContext, killer);
        killer.CurrentMiniGame = miniGame;
        var moneyBefore = killer.Money;

        await monster.ApplyDamageAsync(killer, 1_000_000).ConfigureAwait(false);
        await WaitForAsync(() => killer.Money != moneyBefore).ConfigureAwait(false);

        Assert.That(killer.Money, Is.EqualTo(moneyBefore + MoneyAmount));
        Assert.That(plugIn.Drops, Is.Empty);
    }

    private static async Task<(IGameContext GameContext, Player Killer, Monster Monster, RecordingPlugIn PlugIn)> CreateWorldAsync(bool shouldDropMoney)
    {
        var dropGenerator = new Mock<IDropGenerator>();
        dropGenerator.Setup(g => g.GenerateItemDropsAsync(It.IsAny<MonsterDefinition>(), It.IsAny<int>(), It.IsAny<Player>()))
            .Returns(ValueTask.FromResult<(IEnumerable<Item> Items, uint? Money)>(([], MoneyAmount)));
        var gameContext = GameContextTestHelper.CreateGameContext(dropGenerator: dropGenerator.Object);
        gameContext.Configuration.ShouldDropMoney = shouldDropMoney;
        var plugIn = new RecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IMonsterMoneyDroppedPlugIn>(plugIn);
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        killer.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.MoneyAmountRate);
        var monster = await CreateMonsterAsync(gameContext, dropGenerator.Object).ConfigureAwait(false);
        return (gameContext, killer, monster, plugIn);
    }

    private static MiniGameContext CreateMiniGame(IGameContext gameContext, Player player)
    {
        var definition = new BasicModel.MiniGameDefinition
        {
            Entrance = new BasicModel.ExitGate { Map = player.CurrentMap!.Definition },
            EnterDuration = TimeSpan.FromHours(1),
        };
        var initializer = new Mock<IMapInitializer>();
        initializer.Setup(value => value.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(map => new GameMap(map, TimeSpan.FromMinutes(1), 16));
        return new MiniGameContext(new MiniGameMapKey(0, 0, string.Empty), definition, gameContext, initializer.Object);
    }

    private static async ValueTask<Monster> CreateMonsterAsync(IGameContext gameContext, IDropGenerator dropGenerator)
    {
        // A unique id, because the attributes of monster definitions are cached by their id.
        var monsterDefinition = new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), ObjectKind = NpcObjectKind.Monster };
        monsterDefinition.Attributes.Add(new BasicModel.MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var spawnArea = new BasicModel.MonsterSpawnArea
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

    /// <summary>
    /// Waits until the condition is met, because the drop happens delayed, after the death animation.
    /// </summary>
    private static async Task WaitForAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }
    }

    [Guid("5B5AC954-A8DC-4937-9895-CE7259E222A7")]
    private sealed class RecordingPlugIn : IMonsterMoneyDroppedPlugIn
    {
        public System.Collections.Concurrent.ConcurrentBag<(AttackableNpcBase Monster, Player Killer, DroppedMoney Money)> Drops { get; } = new();

        public ValueTask MonsterMoneyDroppedAsync(AttackableNpcBase monster, Player killer, DroppedMoney droppedMoney)
        {
            this.Drops.Add((monster, killer, droppedMoney));
            return ValueTask.CompletedTask;
        }
    }
}
