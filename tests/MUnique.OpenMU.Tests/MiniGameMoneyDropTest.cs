// <copyright file="MiniGameMoneyDropTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests where the money of monsters which are killed in a mini game ends up.
/// </summary>
[TestFixture]
public class MiniGameMoneyDropTest
{
    private const uint MoneyAmount = 1000;

    private readonly List<IAsyncDisposable> _disposables = [];

    /// <summary>
    /// Disposes the created mini game contexts.
    /// </summary>
    /// <returns>The task.</returns>
    [TearDown]
    public async ValueTask TearDownAsync()
    {
        foreach (var disposable in this._disposables)
        {
            await disposable.DisposeAsync().ConfigureAwait(false);
        }

        this._disposables.Clear();
    }

    /// <summary>
    /// Tests that the monsters of the Blood Castle drop their money on the ground, like on any other map,
    /// instead of adding it to the inventory of the killer.
    /// </summary>
    [Test]
    public async Task BloodCastleMonsterDropsMoneyOnTheGroundAsync()
    {
        var (killer, plugIn) = await this.KillMonsterInMiniGameAsync(MiniGameType.BloodCastle).ConfigureAwait(false);

        Assert.That(plugIn.Drops, Has.Count.EqualTo(1));
        Assert.That(plugIn.Drops.Single().Amount, Is.EqualTo(MoneyAmount));
        Assert.That(killer.Money, Is.Zero);
    }

    /// <summary>
    /// Tests that the monsters of the Devil Square still drop no money on the ground,
    /// but add it to the inventory of the killer.
    /// </summary>
    [Test]
    public async Task DevilSquareMonsterAddsMoneyToTheKillerAsync()
    {
        var (killer, plugIn) = await this.KillMonsterInMiniGameAsync(MiniGameType.DevilSquare).ConfigureAwait(false);

        Assert.That(plugIn.Drops, Is.Empty);
        Assert.That(killer.Money, Is.GreaterThan(0));
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

    private async ValueTask<(Player Killer, RecordingPlugIn PlugIn)> KillMonsterInMiniGameAsync(MiniGameType type)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        gameContext.Configuration.ShouldDropMoney = true;
        var plugIn = new RecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IMonsterMoneyDroppedPlugIn>(plugIn);
        var dropGenerator = new Mock<IDropGenerator>();
        dropGenerator.Setup(g => g.GenerateItemDropsAsync(It.IsAny<DataModel.Configuration.MonsterDefinition>(), It.IsAny<int>(), It.IsAny<Player>()))
            .Returns(ValueTask.FromResult<(IEnumerable<Item> Items, uint? Money)>(([], MoneyAmount)));
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        killer.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.MoneyAmountRate);
        killer.CurrentMiniGame = this.CreateMiniGame(type, gameContext);
        var monster = await CreateMonsterAsync(gameContext, dropGenerator.Object).ConfigureAwait(false);

        await monster.ApplyDamageAsync(killer, 1_000_000).ConfigureAwait(false);

        // The drop happens delayed, after the death animation.
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (plugIn.Drops.IsEmpty && killer.Money == 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }

        return (killer, plugIn);
    }

    private MiniGameContext CreateMiniGame(MiniGameType type, IGameContext gameContext)
    {
        var mapDefinition = new GameMapDefinition { Number = 11 };
        var definitionMock = new Mock<MiniGameDefinition>();
        definitionMock.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definitionMock.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definitionMock.SetupGet(d => d.Entrance).Returns(new ExitGate { Map = mapDefinition });
        var definition = definitionMock.Object;
        definition.Type = type;
        definition.EnterDuration = TimeSpan.FromMinutes(1);
        definition.GameDuration = TimeSpan.FromMinutes(5);
        definition.ExitDuration = TimeSpan.FromMinutes(1);

        var mapInitializerMock = new Mock<IMapInitializer>();
        mapInitializerMock.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(map => new GameMap(map, TimeSpan.FromMinutes(1), 16));
        var key = new MiniGameMapKey(mapDefinition.Number, definition.GameLevel, string.Empty);
        MiniGameContext context = type == MiniGameType.BloodCastle
            ? new BloodCastleContext(key, definition, gameContext, mapInitializerMock.Object)
            : new DevilSquareContext(key, definition, gameContext, mapInitializerMock.Object);
        this._disposables.Add(context);
        return context;
    }

    [Guid("0E5B8F44-4F0B-4D2E-9C51-3A7D2B9E6F18")]
    private sealed class RecordingPlugIn : IMonsterMoneyDroppedPlugIn
    {
        public System.Collections.Concurrent.ConcurrentBag<DroppedMoney> Drops { get; } = new();

        public ValueTask MonsterMoneyDroppedAsync(AttackableNpcBase monster, Player killer, DroppedMoney droppedMoney)
        {
            this.Drops.Add(droppedMoney);
            return ValueTask.CompletedTask;
        }
    }
}
