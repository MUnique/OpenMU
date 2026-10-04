// <copyright file="SelupanIntelligenceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.Pathfinding;
using AttributeRelationship = MUnique.OpenMU.Persistence.BasicModel.AttributeRelationship;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using Skill = MUnique.OpenMU.Persistence.BasicModel.Skill;

/// <summary>
/// Tests for the <see cref="SelupanIntelligence"/>.
/// </summary>
[TestFixture]
public class SelupanIntelligenceTest
{
    private static readonly Point SelupanPosition = new(100, 100);

    /// <summary>
    /// Tests that the fall of Selupan hits the players which are closer to it than the fall radius,
    /// but not the ones further away.
    /// </summary>
    [Test]
    public async Task FallHitsOnlyPlayersAroundSelupanAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var definition = new RaklionEventDefinition();
        var (monster, intelligence) = await CreateSelupanAsync(gameContext, definition).ConfigureAwait(false);

        var near = await CreatePlayerAsync(gameContext, new Point(102, 102)).ConfigureAwait(false);
        var atRadius = await CreatePlayerAsync(gameContext, new Point((byte)(SelupanPosition.X + definition.FallRadius), SelupanPosition.Y)).ConfigureAwait(false);
        var far = await CreatePlayerAsync(gameContext, new Point(108, 100)).ConfigureAwait(false);

        var targets = intelligence.GetFallTargets(monster);

        Assert.That(targets, Is.EquivalentTo(new[] { near }));
        Assert.That(targets, Does.Not.Contain(atRadius).And.Not.Contain(far));
    }

    /// <summary>
    /// Tests that the fall pushes a player away from Selupan by the push distance.
    /// </summary>
    [Test]
    public async Task FallPushesPlayerAwayFromSelupanAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var (monster, _) = await CreateSelupanAsync(gameContext, new RaklionEventDefinition()).ConfigureAwait(false);
        var player = await CreatePlayerAsync(gameContext, new Point(102, 100)).ConfigureAwait(false);

        await monster.PushAwayAsync(player, 4).ConfigureAwait(false);

        Assert.That(player.Position, Is.EqualTo(new Point(106, 100)));
    }

    /// <summary>
    /// Tests that the push stops in front of a field which isn't walkable.
    /// </summary>
    [Test]
    public async Task PushStopsAtUnwalkableFieldAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var (monster, _) = await CreateSelupanAsync(gameContext, new RaklionEventDefinition()).ConfigureAwait(false);
        var player = await CreatePlayerAsync(gameContext, new Point(102, 100)).ConfigureAwait(false);
        monster.CurrentMap.Terrain.WalkMap[104, 100] = false;

        await monster.PushAwayAsync(player, 4).ConfigureAwait(false);

        Assert.That(player.Position, Is.EqualTo(new Point(103, 100)));
    }

    /// <summary>
    /// Tests that the damage multiplier of a skill of Selupan can be calculated with the attributes of the monster.
    /// The attribute system of a monster didn't support attribute relationships, so the first attack with each
    /// skill threw an exception, and the multiplier was missing afterwards.
    /// </summary>
    [Test]
    public async Task SkillMultiplierIsCalculatedWithMonsterAttributesAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var (monster, _) = await CreateSelupanAsync(gameContext, new RaklionEventDefinition()).ConfigureAwait(false);
        var skill = new Skill { Number = 253, Name = "Selupan Fall" };
        skill.AttributeRelationships.Add(new AttributeRelationship(Stats.SkillFinalMultiplier, 2.5f, Stats.SkillMultiplier, AggregateType.AddRaw) { InputOperator = InputOperator.Maximum });
        var skillEntry = new SkillEntry { Skill = skill };

        var skillAttributes = skillEntry.EnsureSkillAttributes(monster.Attributes);

        Assert.That(skillAttributes?[Stats.SkillFinalMultiplier], Is.EqualTo(2.5f).Within(0.001f));
    }

    private static async ValueTask<(Monster Monster, SelupanIntelligence Intelligence)> CreateSelupanAsync(GameContext gameContext, RaklionEventDefinition definition)
    {
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var monsterDefinition = new MonsterDefinition { Id = Guid.NewGuid(), Number = 459, ObjectKind = NpcObjectKind.Monster, AttackRange = 10 };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.SkillMultiplier, Value = 2 });
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = SelupanPosition.X,
            Y1 = SelupanPosition.Y,
            X2 = SelupanPosition.X,
            Y2 = SelupanPosition.Y,
            Quantity = 1,
        };

        var intelligence = new SelupanIntelligence(new RaklionContext(gameContext, definition), definition, NullLogger.Instance);
        var monster = new Monster(spawnArea, monsterDefinition, map, NullDropGenerator.Instance, intelligence, gameContext.PlugInManager, gameContext.PathFinderPool);
        monster.Initialize();
        await map.AddAsync(monster).ConfigureAwait(false);
        return (monster, intelligence);
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
