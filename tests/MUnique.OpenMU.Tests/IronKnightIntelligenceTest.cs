// <copyright file="IronKnightIntelligenceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.Pathfinding;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the <see cref="IronKnightIntelligence"/>.
/// </summary>
[TestFixture]
public class IronKnightIntelligenceTest
{
    private static readonly Point IronKnightPosition = new(100, 100);

    /// <summary>
    /// Tests that the iron knight stabs a player in its attack range, when the chance is 100 percent.
    /// </summary>
    [Test]
    public async Task StabsPlayerInRangeAsync()
    {
        var gameContext = CreateGameContext(100);
        var intelligence = await CreateIronKnightAsync(gameContext).ConfigureAwait(false);
        var player = await CreatePlayerAsync(gameContext, new Point(103, 100)).ConfigureAwait(false);

        Assert.That(await intelligence.TryStabAsync(player).ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that the iron knight doesn't stab, when the chance is 0 percent.
    /// </summary>
    [Test]
    public async Task DoesNotStabWithoutChanceAsync()
    {
        var gameContext = CreateGameContext(0);
        var intelligence = await CreateIronKnightAsync(gameContext).ConfigureAwait(false);
        var player = await CreatePlayerAsync(gameContext, new Point(103, 100)).ConfigureAwait(false);

        Assert.That(await intelligence.TryStabAsync(player).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that the iron knight doesn't stab a player outside of its attack range.
    /// </summary>
    [Test]
    public async Task DoesNotStabPlayerOutOfRangeAsync()
    {
        var gameContext = CreateGameContext(100);
        var intelligence = await CreateIronKnightAsync(gameContext).ConfigureAwait(false);
        var player = await CreatePlayerAsync(gameContext, new Point(110, 100)).ConfigureAwait(false);

        Assert.That(await intelligence.TryStabAsync(player).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that the iron knight doesn't stab without a target.
    /// </summary>
    [Test]
    public async Task DoesNotStabWithoutTargetAsync()
    {
        var gameContext = CreateGameContext(100);
        var intelligence = await CreateIronKnightAsync(gameContext).ConfigureAwait(false);

        Assert.That(await intelligence.TryStabAsync(null).ConfigureAwait(false), Is.False);
    }

    private static IGameContext CreateGameContext(int stabChance)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var raklionPlugIn = gameContext.FeaturePlugIns.GetPlugIn<RaklionPlugIn>();
        Assert.That(raklionPlugIn, Is.Not.Null);
        raklionPlugIn!.Configuration = new RaklionEventDefinition { IronKnightStabChance = stabChance };
        return gameContext;
    }

    private static async ValueTask<IronKnightIntelligence> CreateIronKnightAsync(IGameContext gameContext)
    {
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var monsterDefinition = new MonsterDefinition { Id = Guid.NewGuid(), Number = 458, ObjectKind = NpcObjectKind.Monster, AttackRange = 6 };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = IronKnightPosition.X,
            Y1 = IronKnightPosition.Y,
            X2 = IronKnightPosition.X,
            Y2 = IronKnightPosition.Y,
            Quantity = 1,
        };

        var intelligence = new IronKnightIntelligence();
        var monster = new Monster(spawnArea, monsterDefinition, map, NullDropGenerator.Instance, intelligence, gameContext.PlugInManager, gameContext.PathFinderPool);
        monster.Initialize();
        await map.AddAsync(monster).ConfigureAwait(false);
        return intelligence;
    }

    private static async ValueTask<Player> CreatePlayerAsync(IGameContext gameContext, Point position)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.IsAlive = true;
        player.Position = position;
        await player.CurrentMap!.AddAsync(player).ConfigureAwait(false);
        return player;
    }
}
