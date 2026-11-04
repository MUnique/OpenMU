// <copyright file="MonsterRespawnTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the respawn of a killed <see cref="Monster"/>.
/// </summary>
[TestFixture]
public class MonsterRespawnTest
{
    private static readonly TimeSpan RespawnDelay = TimeSpan.FromMilliseconds(100);

    private static readonly Point SpawnPoint = new(100, 100);

    /// <summary>
    /// Tests that a killed monster respawns on its map after the respawn delay.
    /// </summary>
    [Test]
    public async Task KilledMonsterRespawnsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var monster = await CreateMonsterAsync(gameContext).ConfigureAwait(false);

        await monster.ReflectDamageAsync(killer, 1_000_000).ConfigureAwait(false);
        Assert.That(monster.IsAlive, Is.False);

        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!monster.IsAlive && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }

        Assert.That(monster.IsAlive, Is.True);
        Assert.That(monster.CurrentMap.GetNpcsInRange(SpawnPoint, 1), Does.Contain(monster));
    }

    /// <summary>
    /// Tests that a killed monster which is disposed while it waits to respawn,
    /// e.g. because its spawn area got deleted, doesn't come back to the map.
    /// </summary>
    [Test]
    public async Task KilledMonsterDoesNotRespawnAfterDisposeAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var monster = await CreateMonsterAsync(gameContext).ConfigureAwait(false);
        var map = monster.CurrentMap;
        var monsterId = monster.Id;

        await monster.ReflectDamageAsync(killer, 1_000_000).ConfigureAwait(false);
        await monster.DisposeAsync().ConfigureAwait(false);

        await Task.Delay(RespawnDelay * 5).ConfigureAwait(false);

        Assert.That(monster.IsAlive, Is.False);
        Assert.That(map.GetNpcsInRange(SpawnPoint, 1), Does.Not.Contain(monster));
        Assert.That(map.GetAttackablesInRange(SpawnPoint, 1), Does.Not.Contain(monster));
        Assert.That(map.GetObject(monsterId), Is.Not.SameAs(monster));
    }

    private static async ValueTask<Monster> CreateMonsterAsync(IGameContext gameContext)
    {
        // A unique id, because the attributes of monster definitions are cached by their id.
        var monsterDefinition = new MonsterDefinition
        {
            Id = Guid.NewGuid(),
            ObjectKind = NpcObjectKind.Monster,
            RespawnDelay = RespawnDelay,
        };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            SpawnTrigger = SpawnTrigger.Automatic,
            X1 = SpawnPoint.X,
            Y1 = SpawnPoint.Y,
            X2 = SpawnPoint.X,
            Y2 = SpawnPoint.Y,
            Quantity = 1,
        };

        var monster = new Monster(
            spawnArea,
            monsterDefinition,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            gameContext.PlugInManager,
            gameContext.PathFinderPool);
        monster.Initialize();
        await map.AddAsync(monster).ConfigureAwait(false);
        return monster;
    }
}
