// <copyright file="BotMapFloorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;

/// <summary>
/// Tests how bots deal with maps whose parts cannot be walked between, like the floors of the
/// Lost Tower: hunting there through the right warp entry, and following a leader to his floor.
/// </summary>
[TestFixture]
public class BotMapFloorTest
{
    private const short LowerFloorMonsterNumber = 30003;
    private const short UpperFloorMonsterNumber = 30004;
    private const byte LowerFloorMonsterLevel = 10;
    private const byte UpperFloorMonsterLevel = 50;

    /// <summary>
    /// Two walkable blocks separated by walls are two regions, and each block is the zone of the warp
    /// entry leading into it.
    /// </summary>
    [Test]
    public void FloorsWhichCannotBeWalkedBetweenAreSeparateRegions()
    {
        var map = CreateTwoFloorMap(1);
        var regions = BotMapRegions.Get(map);
        var lowerRegion = regions.GetRegion(new Point(12, 12));
        var upperRegion = regions.GetRegion(new Point(202, 202));

        Assert.That(regions.RegionCount, Is.EqualTo(2));
        Assert.That(lowerRegion, Is.Not.Zero);
        Assert.That(upperRegion, Is.Not.Zero.And.Not.EqualTo(lowerRegion));
        Assert.That(regions.GetRegion(new Point(100, 100)), Is.Zero, "a wall belongs to no region");

        var upperArea = new MonsterSpawnArea { X1 = 203, X2 = 203, Y1 = 203, Y2 = 203 };
        Assert.That(regions.IsAreaWithin(upperArea, [upperRegion], 12, 6), Is.True);
        Assert.That(regions.IsAreaWithin(upperArea, [lowerRegion], 12, 6), Is.False);

        var zones = regions.GetZones([CreateGate(map, 10, 10), CreateGate(map, 200, 200)]);
        Assert.That(zones.GetZone(new Point(15, 15)), Is.Zero);
        Assert.That(zones.GetZone(new Point(203, 203)), Is.EqualTo(1));
        Assert.That(zones.GetZone(new Point(100, 100)), Is.EqualTo(-1), "a wall belongs to no zone");
        Assert.That(zones.IsAreaWithin(upperArea, 1, 12, 6), Is.True);
    }

    /// <summary>
    /// A bot which keeps away from a zone doesn't home in on the monsters standing in it either - on a
    /// map it can walk through, that would lead it right back to where it kept dying.
    /// </summary>
    [Test]
    public async ValueTask MonstersOfAnAvoidedZoneAreNotHomedInOnAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateHunterAsync(gameContext).ConfigureAwait(false);
        var (mapDefinition, _, _) = AddCorridorMap(gameContext, 12);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        bot.SetCurrentMapSilently(map);
        bot.Position = new Point(100, 10);
        await CreateMonsterAsync(gameContext, map, CreateMonsterDefinition(UpperFloorMonsterNumber, UpperFloorMonsterLevel), new Point(110, 10)).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);
        Assert.That(navigator.TryFindNearestMonsterGround(map, out _), Is.True);

        for (var i = 0; i < 3; i++)
        {
            navigator.RegisterMonsterDeath(mapDefinition, new Point(150, 10));
        }

        Assert.That(navigator.TryFindNearestMonsterGround(map, out _), Is.False);
    }

    /// <summary>
    /// On a map which can be walked through in one piece, each entry is judged by the monsters closest
    /// to it on foot, so a bot enters next to the monsters it came for instead of through the first
    /// entry of the list and a long walk.
    /// </summary>
    [Test]
    public async ValueTask HuntingWarpsOfAConnectedMapAreJudgedByTheirZoneAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateHunterAsync(gameContext).ConfigureAwait(false);
        var (map, firstWarp, secondWarp) = AddCorridorMap(gameContext, 8);
        bot.SetCurrentMapSilently(CreateOpenMap(9));

        var warps = new BotNavigator(bot).GetHuntingWarps(map, 0).ToList();

        Assert.That(warps, Is.EquivalentTo(new[] { (firstWarp, (int)LowerFloorMonsterLevel), (secondWarp, (int)UpperFloorMonsterLevel) }));

        // An area reaching into both zones belongs to the one holding most of its tiles.
        var zones = BotMapRegions.Get(map).GetZones([firstWarp.Gate!, secondWarp.Gate!]);
        Assert.That(zones.GetAreaZone(new MonsterSpawnArea { X1 = 60, X2 = 200, Y1 = 10, Y2 = 11 }, 12, 6), Is.EqualTo(1));
        Assert.That(zones.GetAreaZone(new MonsterSpawnArea { X1 = 10, X2 = 150, Y1 = 10, Y2 = 11 }, 12, 6), Is.Zero);
    }

    /// <summary>
    /// A bot which keeps dying to the monsters of one zone stays away from it for a while and settles
    /// for the next best one - judging one monster at a time can't see dense packs, and without this
    /// the bot respawned, warped back and died again in a loop.
    /// </summary>
    [Test]
    public async ValueTask ZoneIsAvoidedAfterRepeatedDeathsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateHunterAsync(gameContext).ConfigureAwait(false);
        var (floorMap, lowerWarp, upperWarp) = AddTwoFloorMap(gameContext, 10);
        bot.SetCurrentMapSilently(CreateOpenMap(11));
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 11, 9, 12, 0, 0, TimeSpan.Zero));
        var navigator = new BotNavigator(bot, timeProvider);

        navigator.RegisterMonsterDeath(floorMap, new Point(203, 203));
        navigator.RegisterMonsterDeath(floorMap, new Point(202, 204));
        Assert.That(navigator.GetHuntingWarps(floorMap, 0).Select(w => w.Warp), Does.Contain(upperWarp), "two deaths are not enough");

        timeProvider.Advance(TimeSpan.FromMinutes(1));
        navigator.RegisterMonsterDeath(floorMap, new Point(204, 202));
        Assert.That(navigator.GetHuntingWarps(floorMap, 0).ToList(), Is.EquivalentTo(new[] { (lowerWarp, (int)LowerFloorMonsterLevel) }));

        timeProvider.Advance(TimeSpan.FromMinutes(31));
        Assert.That(navigator.GetHuntingWarps(floorMap, 0).Select(w => w.Warp), Does.Contain(upperWarp), "the avoidance ends");
    }

    /// <summary>
    /// Each warp entry of a multi-floor map is judged by the monsters of its own floor. Judging the map
    /// by its strongest monsters sent the bots through the first entry to a floor which had none of them.
    /// </summary>
    [Test]
    public async ValueTask HuntingWarpsAreJudgedByTheMonstersOfTheirFloorAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateHunterAsync(gameContext).ConfigureAwait(false);
        var (floorMap, lowerWarp, upperWarp) = AddTwoFloorMap(gameContext, 2);
        bot.SetCurrentMapSilently(CreateOpenMap(3));

        var warps = new BotNavigator(bot).GetHuntingWarps(floorMap, 0).ToList();

        Assert.That(warps, Is.EquivalentTo(new[] { (lowerWarp, (int)LowerFloorMonsterLevel), (upperWarp, (int)UpperFloorMonsterLevel) }));
    }

    /// <summary>
    /// On its own multi-floor map, a bot is offered only the entries to the other floors - the one to
    /// its own floor would not move it anywhere.
    /// </summary>
    [Test]
    public async ValueTask HuntingWarpsOfTheOwnMapLeadToTheOtherFloorsOnlyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateHunterAsync(gameContext).ConfigureAwait(false);
        var (floorMap, _, upperWarp) = AddTwoFloorMap(gameContext, 4);
        bot.SetCurrentMapSilently(new GameMap(floorMap, TimeSpan.FromMinutes(1), 8));
        bot.Position = new Point(12, 12);

        var warps = new BotNavigator(bot).GetHuntingWarps(floorMap, 0).ToList();

        Assert.That(warps, Is.EquivalentTo(new[] { (upperWarp, (int)UpperFloorMonsterLevel) }));
    }

    /// <summary>
    /// A bot picks its hunting ground on the floor it stands on. The stronger monsters of another floor
    /// used to win the pick, the walk there failed, and the bot stood without a ground.
    /// </summary>
    [Test]
    public async ValueTask HuntingGroundIsPickedOnTheOwnFloorAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateHunterAsync(gameContext).ConfigureAwait(false);
        var (floorMap, _, _) = AddTwoFloorMap(gameContext, 5);
        var map = new GameMap(floorMap, TimeSpan.FromMinutes(1), 8);
        bot.SetCurrentMapSilently(map);
        bot.Position = new Point(10, 10);

        await new BotNavigator(bot).PickGroundAndTravelAsync(map, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.HuntingSpot, Is.Not.Null);
        Assert.That(bot.HuntingSpot!.Position.X, Is.InRange(10, 15));
        Assert.That(bot.HuntingSpot.Position.Y, Is.InRange(10, 15));
    }

    /// <summary>
    /// A follower warps straight to the floor of its leader with a single warp, instead of landing on
    /// the map's spawn gate on another floor first and waiting out the warp cooldown for a second one.
    /// </summary>
    [Test]
    public async ValueTask FollowerWarpsStraightToTheFloorOfItsLeaderAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 11, 9, 12, 0, 0, TimeSpan.Zero));
        var navigator = new BotNavigator(bot, timeProvider);
        var mapChangeRecorder = new MapChangeRecordingPlugIn(bot);
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangedPlugIn>(mapChangeRecorder);

        var (floorMap, _, _) = AddTwoFloorMap(gameContext, 6);
        floorMap.ExitGates.Single(g => g.X1 == 10).IsSpawnGate = true;
        var leaderMap = new GameMap(floorMap, TimeSpan.FromMinutes(1), 8);
        var botMap = CreateOpenMap(7);

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(botMap);
        bot.SelectedCharacter!.CurrentMap = botMap.Definition;
        leader.SetCurrentMapSilently(leaderMap);
        leader.Position = new Point(203, 203);

        Assert.That(await navigator.TryHandleCrossMapLeaderFollowAsync(botMap, leader, CancellationToken.None).ConfigureAwait(false), Is.True);
        Assert.That(mapChangeRecorder.MapChangeCount, Is.Zero, "the leader has not settled yet");

        timeProvider.Advance(TimeSpan.FromSeconds(20));
        Assert.That(await navigator.TryHandleCrossMapLeaderFollowAsync(botMap, leader, CancellationToken.None).ConfigureAwait(false), Is.True);

        Assert.That(mapChangeRecorder.MapChangeCount, Is.EqualTo(1));
        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 205));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 205));
    }

    /// <summary>
    /// A follower which misses a requirement of the leader's map - like wings for Icarus - leaves the
    /// party instead of following, as a player couldn't enter the map either.
    /// </summary>
    [Test]
    public async ValueTask FollowerLeavesThePartyWhenItMissesARequirementOfTheLeadersMapAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var (floorMap, _, _) = AddTwoFloorMap(gameContext, 13);
        floorMap.MapRequirements.Add(new MUnique.OpenMU.Persistence.BasicModel.AttributeRequirement { Attribute = Stats.CanFly, MinimumValue = 1 });
        var botMap = CreateOpenMap(14);
        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(botMap);
        leader.SetCurrentMapSilently(new GameMap(floorMap, TimeSpan.FromMinutes(1), 8));
        leader.Position = new Point(203, 203);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(leader).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);

        await new BotNavigator(bot).TryHandleCrossMapLeaderFollowAsync(botMap, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.Party, Is.Null);
        Assert.That(bot.CurrentMap, Is.SameAs(botMap));
    }

    /// <summary>
    /// A follower pays the fare of its warp to the leader, like a player using the warp list.
    /// </summary>
    [Test]
    public async ValueTask FollowerPaysTheFareOfItsWarpAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 11, 9, 12, 0, 0, TimeSpan.Zero));
        var navigator = new BotNavigator(bot, timeProvider);
        var (floorMap, _, upperWarp) = AddTwoFloorMap(gameContext, 15);
        upperWarp.Costs = 3000;
        var botMap = CreateOpenMap(16);
        bot.Attributes![Stats.Level] = 100;
        bot.Money = 10000;
        bot.SetCurrentMapSilently(botMap);
        bot.SelectedCharacter!.CurrentMap = botMap.Definition;
        leader.SetCurrentMapSilently(new GameMap(floorMap, TimeSpan.FromMinutes(1), 8));
        leader.Position = new Point(203, 203);

        await navigator.TryHandleCrossMapLeaderFollowAsync(botMap, leader, CancellationToken.None).ConfigureAwait(false);
        timeProvider.Advance(TimeSpan.FromSeconds(20));
        await navigator.TryHandleCrossMapLeaderFollowAsync(botMap, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.Money, Is.EqualTo(7000));
        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 205));
    }

    /// <summary>
    /// A follower which can't afford any warp to its leader saves up for it, while one on the leader's
    /// part of the map needs no warp at all.
    /// </summary>
    [Test]
    public async ValueTask FollowerWithoutTheFareSavesUpForItAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var (floorMap, lowerWarp, upperWarp) = AddTwoFloorMap(gameContext, 17);
        lowerWarp.Costs = 3000;
        upperWarp.Costs = 5000;
        var map = new GameMap(floorMap, TimeSpan.FromMinutes(1), 8);
        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.Position = new Point(12, 12);
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(203, 203);
        var navigator = new BotNavigator(bot);

        bot.Money = 1000;
        Assert.That(navigator.IsSavingForFollowFare(map, leader), Is.True);

        bot.Money = 3000;
        Assert.That(navigator.IsSavingForFollowFare(map, leader), Is.False, "the cheaper entry is affordable");

        bot.Money = 0;
        leader.Position = new Point(14, 14);
        Assert.That(navigator.IsSavingForFollowFare(map, leader), Is.False, "the leader can be reached on foot");
    }

    private static async ValueTask<GameLogic.Offline.OfflinePlayer> CreateHunterAsync(IGameContext gameContext)
    {
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.IsAlive = true;
        bot.Attributes![Stats.Level] = 100;

        // Monsters only count when the bot can safely kill them (CombatHandler.IsSafeTarget), which
        // requires attack power; it comes from a learned skill, as the derived damage can't be assigned.
        await bot.SkillList!.AddLearnedSkillAsync(new TestSkill { AttackDamage = 100 }).ConfigureAwait(false);
        return bot;
    }

    /// <summary>
    /// Adds a map with a lower floor around (10,10)-(15,15) and an upper floor around (200,200)-(205,205),
    /// each with its own warp entry and monsters, to the configuration.
    /// </summary>
    private static (GameMapDefinition Map, WarpInfo LowerWarp, WarpInfo UpperWarp) AddTwoFloorMap(IGameContext gameContext, byte number)
    {
        var map = CreateTwoFloorMap(number);
        map.MonsterSpawns.Add(CreateSpawn(map, CreateMonsterDefinition(LowerFloorMonsterNumber, LowerFloorMonsterLevel), 13, 13));
        map.MonsterSpawns.Add(CreateSpawn(map, CreateMonsterDefinition(UpperFloorMonsterNumber, UpperFloorMonsterLevel), 203, 203));
        gameContext.Configuration.Maps.Add(map);

        var lowerWarp = CreateWarpInfo("Tower", CreateGate(map, 10, 10));
        var upperWarp = CreateWarpInfo("Tower7", CreateGate(map, 200, 200));
        gameContext.Configuration.WarpList.Add(lowerWarp);
        gameContext.Configuration.WarpList.Add(upperWarp);
        return (map, lowerWarp, upperWarp);
    }

    /// <summary>
    /// Adds a map which is one corridor from (10,10) to (200,11), with a warp entry and monsters at each end.
    /// </summary>
    private static (GameMapDefinition Map, WarpInfo FirstWarp, WarpInfo SecondWarp) AddCorridorMap(IGameContext gameContext, byte number)
    {
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, (byte)4, 3, ushort.MaxValue);
        for (var x = 10; x <= 200; x++)
        {
            MarkWalkable(terrain, (byte)x, 10);
            MarkWalkable(terrain, (byte)x, 11);
        }

        var map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = terrain,
        };
        map.MonsterSpawns.Add(CreateSpawn(map, CreateMonsterDefinition(LowerFloorMonsterNumber, LowerFloorMonsterLevel), 15, 10));
        map.MonsterSpawns.Add(CreateSpawn(map, CreateMonsterDefinition(UpperFloorMonsterNumber, UpperFloorMonsterLevel), 195, 10));
        gameContext.Configuration.Maps.Add(map);
        var firstWarp = CreateWarpInfo("Field", CreateGate(map, 10, 10));
        var secondWarp = CreateWarpInfo("Field2", CreateGate(map, 200, 10));
        gameContext.Configuration.WarpList.Add(firstWarp);
        gameContext.Configuration.WarpList.Add(secondWarp);
        return (map, firstWarp, secondWarp);
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

    private static GameMapDefinition CreateTwoFloorMap(byte number)
    {
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, (byte)4, 3, ushort.MaxValue);
        for (var offset = 0; offset < 6; offset++)
        {
            for (var other = 0; other < 6; other++)
            {
                MarkWalkable(terrain, (byte)(10 + offset), (byte)(10 + other));
                MarkWalkable(terrain, (byte)(200 + offset), (byte)(200 + other));
            }
        }

        return new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = terrain,
        };
    }

    private static GameMap CreateOpenMap(byte number)
    {
        var definition = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = new byte[ushort.MaxValue + 3],
        };

        return new GameMap(definition, TimeSpan.FromMinutes(1), 8);
    }

    private static MonsterSpawnArea CreateSpawn(GameMapDefinition map, MonsterDefinition monster, byte x, byte y)
    {
        return new MonsterSpawnArea
        {
            MonsterDefinition = monster,
            GameMap = map,
            X1 = x,
            X2 = x,
            Y1 = y,
            Y2 = y,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.Automatic,
        };
    }

    private static MonsterDefinition CreateMonsterDefinition(short number, byte level)
    {
        var definition = new MonsterDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            ObjectKind = NpcObjectKind.Monster,
        };
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = level });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MinimumPhysBaseDmg, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumPhysBaseDmg, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.DefenseBase, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.AttackRatePvm, Value = 0 });
        return definition;
    }

    private static void MarkWalkable(byte[] terrain, byte x, byte y)
    {
        terrain[3 + (y * 256) + x] = 0;
    }

    private static ExitGate CreateGate(GameMapDefinition map, byte x, byte y)
    {
        var gate = new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = map,
            X1 = x,
            Y1 = y,
            X2 = (byte)(x + 1),
            Y2 = (byte)(y + 1),
        };

        map.ExitGates.Add(gate);
        return gate;
    }

    private static WarpInfo CreateWarpInfo(string name, ExitGate gate, int costs = 0)
    {
        return new MUnique.OpenMU.Persistence.BasicModel.WarpInfo
        {
            Id = Guid.NewGuid(),
            Name = name,
            Gate = gate,
            Costs = costs,
        };
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

    [Guid("3C0E7B5A-9F42-4D1B-8E6A-2B7D5C9F1A34")]
    private sealed class MapChangeRecordingPlugIn : IPlayerStateChangedPlugIn
    {
        private readonly Player _observedPlayer;

        public MapChangeRecordingPlugIn(Player observedPlayer)
        {
            this._observedPlayer = observedPlayer;
        }

        public int MapChangeCount { get; private set; }

        public ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
        {
            if (ReferenceEquals(player, this._observedPlayer)
                && previousState != PlayerState.ChangingMap
                && currentState == PlayerState.ChangingMap)
            {
                this.MapChangeCount++;
            }

            return ValueTask.CompletedTask;
        }
    }

    private sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset _utcNow = utcNow;

        public override DateTimeOffset GetUtcNow() => this._utcNow;

        public void Advance(TimeSpan timeSpan)
        {
            this._utcNow += timeSpan;
        }
    }
}
