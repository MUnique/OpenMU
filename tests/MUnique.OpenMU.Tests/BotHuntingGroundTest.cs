// <copyright file="BotHuntingGroundTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;

/// <summary>
/// Tests for the way a bot picks the spot it hunts on.
/// </summary>
[TestFixture]
public class BotHuntingGroundTest
{
    /// <summary>
    /// The number of the weak test monster. <c>CombatHandler</c> caches the combat stats of a monster
    /// definition statically by its number, so a number shared with the monster definitions of other
    /// tests (which default to 0) would make the result depend on which test ran first.
    /// </summary>
    private const short WeakMonsterNumber = 30001;

    /// <summary>
    /// A hunting ground on the far side of the map stays a realistic choice. Bots enter a map through the
    /// same gate and rate every ground from that one spot, so a steep distance penalty made all of them
    /// queue up on the few spawns next to the entrance.
    /// </summary>
    [Test]
    public void DistantGroundKeepsARealisticShare()
    {
        var atTheGate = new MonsterSpawnArea { X1 = 200, X2 = 200, Y1 = 200, Y2 = 200, Quantity = 1 };
        var acrossTheMap = new MonsterSpawnArea { X1 = 33, X2 = 33, Y1 = 95, Y2 = 95, Quantity = 1 };
        var gate = new Point(222, 211);

        var near = BotNavigator.GroundWeight(atTheGate, gate);
        var far = BotNavigator.GroundWeight(acrossTheMap, gate);

        Assert.That(near, Is.GreaterThan(far), "closer grounds are still preferred");
        Assert.That(near, Is.LessThan(far * 10), "but not to the point where the rest of the map never gets picked");
    }

    /// <summary>
    /// The number of monsters an area holds still counts.
    /// </summary>
    [Test]
    public void DenserGroundOutweighsThinnerOneAtTheSameDistance()
    {
        var thin = new MonsterSpawnArea { X1 = 100, X2 = 100, Y1 = 100, Y2 = 100, Quantity = 1 };
        var dense = new MonsterSpawnArea { X1 = 100, X2 = 100, Y1 = 100, Y2 = 100, Quantity = 20 };
        var from = new Point(150, 150);

        Assert.That(BotNavigator.GroundWeight(dense, from), Is.GreaterThan(BotNavigator.GroundWeight(thin, from)));
    }

    /// <summary>
    /// An unreachable ground must not keep its claim: otherwise other bots avoid a spot nobody can
    /// actually hunt on until the next successful ground pick.
    /// </summary>
    [Test]
    public async ValueTask UnreachableGroundDropsItsClaimAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.IsAlive = true;
        bot.Attributes![Stats.Level] = 100;
        bot.Attributes[Stats.MinimumPhysBaseDmg] = 100;
        bot.Attributes[Stats.MaximumPhysBaseDmg] = 100;
        bot.Attributes[Stats.DefensePvm] = 0;
        bot.Attributes[Stats.MaximumHealth] = 1000;
        bot.Attributes[Stats.CurrentHealth] = 1000;

        var mapDefinition = CreateIsolatedGroundMap(0);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        bot.SetCurrentMapSilently(map);
        bot.Position = new Point(10, 10);

        var navigator = new BotNavigator(bot);
        await navigator.PickGroundAndTravelAsync(map, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.HuntingSpot, Is.Null);
    }

    /// <summary>
    /// Homing in on a nearby monster must claim it, so another independent bot does not target the
    /// same monster while this bot is still walking towards it.
    /// </summary>
    [Test]
    public async ValueTask NearestMonsterIsClaimedAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.IsAlive = true;
        bot.Attributes![Stats.Level] = 100;

        // The bot only walks towards monsters it can safely kill (CombatHandler.IsSafeTarget), which
        // requires some attack power. The damage attributes are derived and can't be assigned, so the
        // power comes from a learned skill - the same way the CombatHandler tests do it.
        await bot.SkillList!.AddLearnedSkillAsync(new TestSkill { AttackDamage = 100 }).ConfigureAwait(false);

        var mapDefinition = CreateIsolatedGroundMap(0);
        MarkWalkable(mapDefinition.TerrainData!, 90, 90);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        bot.SetCurrentMapSilently(map);
        bot.Position = new Point(90, 90);

        var monster = await CreateMonsterAsync(gameContext, map, CreateWeakMonsterDefinition(), new Point(100, 100)).ConfigureAwait(false);

        var navigator = new BotNavigator(bot);
        var found = navigator.TryFindNearestMonsterGround(map, out var ground);

        Assert.That(found, Is.True);
        Assert.That(bot.HuntingSpot, Is.Not.Null);
        Assert.That(bot.HuntingSpot!.Position, Is.EqualTo(ground));
        Assert.That(monster.Position, Is.EqualTo(ground));
    }

    private static GameMapDefinition CreateIsolatedGroundMap(byte number)
    {
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, (byte)4, 3, ushort.MaxValue);
        MarkWalkable(terrain, 10, 10);
        MarkWalkable(terrain, 100, 100);

        var monsterDefinition = CreateWeakMonsterDefinition();

        var mapDefinition = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = terrain,
        };
        mapDefinition.MonsterSpawns.Add(new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = mapDefinition,
            X1 = 100,
            X2 = 100,
            Y1 = 100,
            Y2 = 100,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.Automatic,
        });

        return mapDefinition;
    }

    private static MonsterDefinition CreateWeakMonsterDefinition()
    {
        var definition = new MonsterDefinition
        {
            Id = Guid.NewGuid(),
            Number = WeakMonsterNumber,
            ObjectKind = NpcObjectKind.Monster,
        };
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = 1 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MinimumPhysBaseDmg, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumPhysBaseDmg, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.DefenseBase, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.AttackRatePvm, Value = 0 });
        return definition;
    }

    private static async ValueTask<Monster> CreateMonsterAsync(IGameContext gameContext, GameMap map, MonsterDefinition definition, Point position)
    {
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = definition,
            GameMap = map.Definition,
            X1 = position.X,
            X2 = position.X,
            Y1 = position.Y,
            Y2 = position.Y,
            Quantity = 1,
        };

        var monster = new Monster(
            spawnArea,
            definition,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            gameContext.PlugInManager,
            gameContext.PathFinderPool);
        monster.Initialize();
        monster.Attributes[Stats.CurrentHealth] = 1;
        await map.AddAsync(monster).ConfigureAwait(false);
        return monster;
    }

    private static void MarkWalkable(byte[] terrain, byte x, byte y)
    {
        terrain[3 + (y * 256) + x] = 0;
    }

    private sealed class TestSkill : Skill
    {
        public TestSkill()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.ConsumeRequirements = new List<AttributeRequirement>();
            this.Range = 1;
            this.SkillType = SkillType.DirectHit;
            this.DamageType = DamageType.Curse;
        }
    }
}