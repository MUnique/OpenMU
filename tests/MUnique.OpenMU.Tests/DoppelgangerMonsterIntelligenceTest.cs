// <copyright file="DoppelgangerMonsterIntelligenceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the <see cref="DoppelgangerMonsterIntelligence"/>.
/// </summary>
[TestFixture]
public class DoppelgangerMonsterIntelligenceTest
{
    /// <summary>
    /// A short path with three overlapping areas along the x axis. The last one is the magic circle.
    /// </summary>
    private static readonly IList<DoppelgangerPathArea> Path =
    [
        new(100, 100, 110, 110),
        new(108, 100, 118, 110),
        new(116, 100, 126, 110),
    ];

    private int _reachedMagicCircleCount;

    /// <summary>
    /// Sets up the test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._reachedMagicCircleCount = 0;
    }

    /// <summary>
    /// Tests that the position on the path only increases when the monster entered one of the next areas,
    /// although the areas overlap.
    /// </summary>
    [Test]
    public async Task PathPositionAdvancesWithTheNextAreaAsync()
    {
        var (monster, intelligence) = await this.CreateMonsterAsync(true).ConfigureAwait(false);

        await monster.MoveAsync(new Point(109, 105)).ConfigureAwait(false);
        await intelligence.TickAsync().ConfigureAwait(false);
        Assert.That(intelligence.PathPosition, Is.EqualTo(1), "The point is part of the first and the second area.");

        await monster.MoveAsync(new Point(105, 105)).ConfigureAwait(false);
        await intelligence.TickAsync().ConfigureAwait(false);
        Assert.That(intelligence.PathPosition, Is.EqualTo(1), "The position never decreases.");
        Assert.That(this._reachedMagicCircleCount, Is.Zero);
    }

    /// <summary>
    /// Tests that a walking monster in the last area reaches the magic circle exactly once.
    /// </summary>
    [Test]
    public async Task WalkingMonsterReachesMagicCircleOnceAsync()
    {
        var (monster, intelligence) = await this.CreateMonsterAsync(true).ConfigureAwait(false);

        await monster.MoveAsync(new Point(125, 105)).ConfigureAwait(false);
        await intelligence.TickAsync().ConfigureAwait(false);
        await intelligence.TickAsync().ConfigureAwait(false);

        Assert.That(intelligence.PathPosition, Is.EqualTo(Path.Count - 1));
        Assert.That(this._reachedMagicCircleCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a monster which doesn't walk along the path, e.g. a larva of a chest,
    /// doesn't count as reaching the magic circle when it stands next to it.
    /// </summary>
    [Test]
    public async Task StationaryMonsterDoesntReachMagicCircleAsync()
    {
        var (monster, intelligence) = await this.CreateMonsterAsync(false).ConfigureAwait(false);

        await monster.MoveAsync(new Point(125, 105)).ConfigureAwait(false);
        await intelligence.TickAsync().ConfigureAwait(false);

        Assert.That(this._reachedMagicCircleCount, Is.Zero);
    }

    private async Task<(Monster Monster, DoppelgangerMonsterIntelligence Intelligence)> CreateMonsterAsync(bool walksAlongPath)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var monsterDefinition = new MonsterDefinition { Id = Guid.NewGuid(), ObjectKind = NpcObjectKind.Monster, AttackDelay = TimeSpan.FromHours(1) };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = 101,
            Y1 = 105,
            X2 = 101,
            Y2 = 105,
            Quantity = 1,
        };

        var intelligence = new DoppelgangerMonsterIntelligence(
            Path,
            0,
            walksAlongPath,
            false,
            _ =>
            {
                this._reachedMagicCircleCount++;
                return ValueTask.CompletedTask;
            },
            NullLogger.Instance);
        var monster = new Monster(spawnArea, monsterDefinition, map, NullDropGenerator.Instance, intelligence, gameContext.PlugInManager, gameContext.PathFinderPool);
        monster.Initialize();
        await map.AddAsync(monster).ConfigureAwait(false);
        return (monster, intelligence);
    }
}
