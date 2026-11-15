// <copyright file="BotFollowMovementTest.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;

/// <summary>
/// Tests for the pace at which a bot travels, and how a party member keeps up with its leader.
/// </summary>
[TestFixture]
public class BotFollowMovementTest
{
    /// <summary>
    /// The number of the weak test monster. <c>CombatHandler</c> caches the combat stats of a monster
    /// definition statically by its number, so it must not be shared with the monsters of other tests.
    /// </summary>
    private const short WeakMonsterNumber = 30002;

    private static readonly DateTimeOffset Start = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Without a running hop, the bot takes one navigation decision per half second.
    /// </summary>
    [Test]
    public async ValueTask IdleBotDecidesOncePerIntervalAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        Assert.That(navigator.TryBeginEvaluation(Start), Is.True);
        Assert.That(navigator.TryBeginEvaluation(Start.AddMilliseconds(400)), Is.False);
        Assert.That(navigator.TryBeginEvaluation(Start.AddMilliseconds(500)), Is.True);
    }

    /// <summary>
    /// The next decision comes right after a travel hop ended, not only at the next full interval -
    /// otherwise the bot stands between hops and travels at about half the speed of a player.
    /// </summary>
    [Test]
    public async ValueTask NextDecisionFollowsRightAfterTheHopAsync()
    {
        var (bot, leader, map) = await CreatePartyOnOpenMapAsync(new Point(10, 10), new Point(40, 10)).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        Assert.That(navigator.TryBeginEvaluation(Start), Is.True);
        var walked = await navigator.TryRegroupWithLeaderOnSameMapAsync(map, leader, CancellationToken.None).ConfigureAwait(false);
        Assert.That(walked, Is.True);
        Assert.That(bot.IsWalking, Is.True);

        Assert.That(navigator.TryBeginEvaluation(Start.AddMilliseconds(200)), Is.False, "the hop is still running");

        await bot.StopWalkingAsync().ConfigureAwait(false);

        Assert.That(navigator.TryBeginEvaluation(Start.AddMilliseconds(400)), Is.True, "the hop ended, so the bot goes on");
        Assert.That(navigator.TryBeginEvaluation(Start.AddMilliseconds(600)), Is.False, "no hop runs, so the regular interval applies again");
    }

    /// <summary>
    /// In the safezone a follower walks after its leader instead of standing until the stuck watchdog fires.
    /// </summary>
    [Test]
    public async ValueTask FollowerInSafezoneWalksAfterLeaderAsync()
    {
        var (bot, leader, map) = await CreatePartyOnOpenMapAsync(new Point(10, 10), new Point(20, 10), safezone: true).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        await navigator.EvaluateFollowerHuntingAsync(map, leader, true, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.IsWalking, Is.True);
    }

    /// <summary>
    /// A follower next to its leader in the safezone stays where it is.
    /// </summary>
    [Test]
    public async ValueTask FollowerNextToLeaderInSafezoneStaysAsync()
    {
        var (bot, leader, map) = await CreatePartyOnOpenMapAsync(new Point(10, 10), new Point(11, 10), safezone: true).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        await navigator.EvaluateFollowerHuntingAsync(map, leader, true, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.IsWalking, Is.False);
    }

    /// <summary>
    /// With monsters near the leader, a follower within the leash holds position for the combat handler.
    /// </summary>
    [Test]
    public async ValueTask FollowerWithinLeashHoldsForTheFightAsync()
    {
        var (bot, leader, map) = await CreatePartyOnOpenMapAsync(new Point(10, 10), new Point(18, 10)).ConfigureAwait(false);
        await CreateMonsterAsync(bot.GameContext, map, new Point(20, 10)).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        await navigator.EvaluateFollowerHuntingAsync(map, leader, false, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.IsWalking, Is.False);
    }

    /// <summary>
    /// A follower which fell far behind walks back to its leader even while there are monsters near him.
    /// </summary>
    [Test]
    public async ValueTask FollowerBeyondLeashWalksBackDespiteMonstersAsync()
    {
        var (bot, leader, map) = await CreatePartyOnOpenMapAsync(new Point(10, 10), new Point(40, 10)).ConfigureAwait(false);
        await CreateMonsterAsync(bot.GameContext, map, new Point(42, 10)).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        await navigator.EvaluateFollowerHuntingAsync(map, leader, false, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.IsWalking, Is.True);
    }

    private static async ValueTask<(OfflinePlayer Bot, Player Leader, GameMap Map)> CreatePartyOnOpenMapAsync(Point botPosition, Point leaderPosition, bool safezone = false)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.IsAlive = true;
        bot.Attributes![Stats.Level] = 100;

        // The bot only fights monsters it can safely kill (CombatHandler.IsSafeTarget), which requires
        // some attack power - it comes from a learned skill, like in the other bot tests.
        await bot.SkillList!.AddLearnedSkillAsync(new TestSkill { AttackDamage = 100 }).ConfigureAwait(false);

        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);

        // Terrain attribute 0 is walkable ground, 1 is walkable safezone.
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, safezone ? (byte)1 : (byte)0, 3, ushort.MaxValue);
        var mapDefinition = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = 0,
            TerrainData = terrain,
        };
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);

        bot.SetCurrentMapSilently(map);
        bot.Position = botPosition;
        leader.SetCurrentMapSilently(map);
        leader.Position = leaderPosition;
        return (bot, leader, map);
    }

    private static async ValueTask CreateMonsterAsync(IGameContext gameContext, GameMap map, Point position)
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
