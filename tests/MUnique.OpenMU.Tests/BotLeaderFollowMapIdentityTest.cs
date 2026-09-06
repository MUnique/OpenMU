// <copyright file="BotLeaderFollowMapIdentityTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the map identity tracking of the bot's party-leader follow logic.
/// </summary>
[TestFixture]
public class BotLeaderFollowMapIdentityTest
{
    /// <summary>
    /// Multi-level maps can share a map number while using distinct map definitions/terrain. The
    /// follower must treat such a transition as a new leader location and wait for the settle delay
    /// again, otherwise it may follow using stale state from the previous floor.
    /// </summary>
    [Test]
    public async ValueTask LeaderSettleTrackingDistinguishesMapsWithSameNumberAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 31, 1, 0, 0, TimeSpan.Zero));
        var navigator = new BotNavigator(bot, timeProvider);

        var firstFloor = CreateMap(33, 5);
        var secondFloor = CreateMap(33, 6);

        leader.SetCurrentMapSilently(firstFloor);
        Assert.That(navigator.HasLeaderSettled(leader), Is.False);

        timeProvider.Advance(TimeSpan.FromMinutes(1));
        Assert.That(navigator.HasLeaderSettled(leader), Is.True);

        leader.SetCurrentMapSilently(secondFloor);

        Assert.That(navigator.HasLeaderSettled(leader), Is.False);
    }

    /// <summary>
    /// Multi-floor maps such as Dungeon and Lost Tower are represented as one game map with several
    /// warp entries into isolated regions. When the leader is on another floor of the same map and
    /// walking cannot reach him, the follower should regroup through a legal warp entry whose landing
    /// area can reach the leader.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerWarpsThroughReachableGateWhenWalkingIsImpossibleAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 31, 1, 0, 0, TimeSpan.Zero));
        var navigator = new BotNavigator(bot, timeProvider);
        var mapChangeRecorder = new MapChangeRecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangedPlugIn>(mapChangeRecorder);

        var mapDefinition = CreateBlockedTwoFloorMap(4);
        gameContext.Configuration.Maps.Add(mapDefinition);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        var lowerFloorGate = CreateGate(mapDefinition, 10, 10);
        var leaderFloorGate = CreateGate(mapDefinition, 200, 200);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("LostTower", lowerFloorGate));
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("LostTower7", leaderFloorGate));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = mapDefinition;
        bot.SelectedCharacter.PositionX = 10;
        bot.SelectedCharacter.PositionY = 10;

        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        var consumed = await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(mapChangeRecorder.MapChangeCount, Is.EqualTo(1), "the follower must execute a warp, not only change its stored position");
        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 201));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));

        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter.PositionX = 10;
        bot.SelectedCharacter.PositionY = 10;

        var consumedDuringCooldown = await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumedDuringCooldown, Is.False, "no follow action was taken while the warp cooldown was active");
        Assert.That(mapChangeRecorder.MapChangeCount, Is.EqualTo(1), "the cooldown must suppress a second warp");
        Assert.That(bot.SelectedCharacter.PositionX, Is.EqualTo(10));
        Assert.That(bot.SelectedCharacter.PositionY, Is.EqualTo(10));

        timeProvider.Advance(TimeSpan.FromSeconds(20));
        var consumedAfterCooldown = await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumedAfterCooldown, Is.True);
        Assert.That(mapChangeRecorder.MapChangeCount, Is.EqualTo(2), "the follower must warp again after the cooldown");
        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 201));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));
    }

    /// <summary>
    /// Same-map movement belongs to follower hunting, which first checks whether combat around the
    /// leader should continue. Cross-map following must not bypass that decision and warp directly.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowDefersMovementToFollowerHuntingAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);
        var mapChangeRecorder = new MapChangeRecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangedPlugIn>(mapChangeRecorder);

        var mapDefinition = CreateBlockedTwoFloorMap(4);
        gameContext.Configuration.Maps.Add(mapDefinition);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("LostTower7", CreateGate(mapDefinition, 200, 200)));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = mapDefinition;
        bot.SelectedCharacter.PositionX = 10;
        bot.SelectedCharacter.PositionY = 10;
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        var consumed = await navigator.TryFollowLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(mapChangeRecorder.MapChangeCount, Is.Zero);
        Assert.That(bot.SelectedCharacter.PositionX, Is.EqualTo(10));
        Assert.That(bot.SelectedCharacter.PositionY, Is.EqualTo(10));
    }

    /// <summary>
    /// A failed walk with no legal warp leaves the tick available for normal local behavior.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerDoesNotConsumeTickWhenNoFollowRouteExistsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);
        var mapChangeRecorder = new MapChangeRecordingPlugIn();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangedPlugIn>(mapChangeRecorder);

        var mapDefinition = CreateBlockedTwoFloorMap(4);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = mapDefinition;
        bot.SelectedCharacter.PositionX = 10;
        bot.SelectedCharacter.PositionY = 10;
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        var consumed = await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(mapChangeRecorder.MapChangeCount, Is.Zero);
    }

    /// <summary>
    /// Coordinate distance alone can prefer the gate of the follower's isolated region. The follower
    /// must instead choose a gate whose landing area has an actual path to the leader.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerSkipsCloserWarpWhichCannotReachLeaderAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        var mapDefinition = CreateMapWithMisleadingCloserGate(4);
        gameContext.Configuration.Maps.Add(mapDefinition);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        var isolatedGate = CreateGate(mapDefinition, 200, 189);
        var reachableGate = CreateGate(mapDefinition, 150, 200);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("Isolated", isolatedGate));
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("Reachable", reachableGate));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = mapDefinition;
        bot.SelectedCharacter.PositionX = 200;
        bot.SelectedCharacter.PositionY = 189;

        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(150, 151));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));
    }

    /// <summary>
    /// Every coordinate which the production warp can select must be safe: a gate with one blocked
    /// landing point is rejected even if its other point can reach the leader.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerRejectsGateWithBlockedPossibleLandingPointAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        var mapDefinition = CreateMapWithMisleadingCloserGate(4);
        gameContext.Configuration.Maps.Add(mapDefinition);
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        map.Terrain.WalkMap[191, 200] = false;
        map.Terrain.UpdateAiGridValue(191, 200);
        var partiallyBlockedGate = CreateGate(mapDefinition, 190, 200);
        partiallyBlockedGate.X2 = 192;
        var fallbackGate = CreateGate(mapDefinition, 150, 200);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("PartiallyBlocked", partiallyBlockedGate));
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("Fallback", fallbackGate));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = mapDefinition;
        bot.SelectedCharacter.PositionX = 200;
        bot.SelectedCharacter.PositionY = 189;
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(150, 151));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));
    }

    /// <summary>
    /// Warp eligibility is defined by the map number, as it was before party following was added.
    /// The runtime map and the warp-list configuration may be separate entity instances.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerAcceptsWarpFromEquivalentMapDefinitionAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        var runtimeDefinition = CreateBlockedTwoFloorMap(4);
        var warpListDefinition = CreateBlockedTwoFloorMap(4);
        var map = new GameMap(runtimeDefinition, TimeSpan.FromMinutes(1), 8);
        var leaderFloorGate = CreateGate(warpListDefinition, 200, 200);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("EquivalentMap", leaderFloorGate));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = runtimeDefinition;
        bot.SelectedCharacter.PositionX = 10;
        bot.SelectedCharacter.PositionY = 10;
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 201));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));
    }

    /// <summary>
    /// When the warp list contains entries for distinct map definitions with the same number, entries
    /// for the requested definition take precedence over the compatibility fallback.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerPrefersExactMapDefinitionOverSameNumberFallbackAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        var runtimeDefinition = CreateMapWithMisleadingCloserGate(4);
        var otherDefinition = CreateMapWithMisleadingCloserGate(4);
        var map = new GameMap(runtimeDefinition, TimeSpan.FromMinutes(1), 8);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("SameNumber", CreateGate(otherDefinition, 190, 200)));
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("Exact", CreateGate(runtimeDefinition, 150, 200)));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = runtimeDefinition;
        bot.SelectedCharacter.PositionX = 200;
        bot.SelectedCharacter.PositionY = 189;
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(150, 151));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));
    }

    /// <summary>
    /// Finding an exact map entry selects the map identity tier before checking access. An under-level
    /// bot must not bypass that entry by falling back to a lower-level entry with the same map number.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerDoesNotBypassExactWarpLevelThroughSameNumberFallbackAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

        var runtimeDefinition = CreateMapWithMisleadingCloserGate(4);
        var otherDefinition = CreateMapWithMisleadingCloserGate(4);
        var map = new GameMap(runtimeDefinition, TimeSpan.FromMinutes(1), 8);
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("SameNumber", CreateGate(otherDefinition, 190, 200)));
        gameContext.Configuration.WarpList.Add(CreateWarpInfo("Exact", CreateGate(runtimeDefinition, 150, 200), 400));

        bot.Attributes![Stats.Level] = 100;
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = runtimeDefinition;
        bot.SelectedCharacter.PositionX = 200;
        bot.SelectedCharacter.PositionY = 189;
        leader.SetCurrentMapSilently(map);
        leader.Position = new Point(200, 200);

        await navigator.TryRegroupWithLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(bot.SelectedCharacter.PositionX, Is.EqualTo(200));
        Assert.That(bot.SelectedCharacter.PositionY, Is.EqualTo(189));
    }

    private static GameMap CreateMap(byte number, byte discriminator)
    {
        var definition = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            Discriminator = discriminator,
            TerrainData = new byte[ushort.MaxValue + 3],
        };

        definition.ExitGates.Add(new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = definition,
            X1 = 10,
            Y1 = 10,
            X2 = 12,
            Y2 = 12,
            IsSpawnGate = true,
        });

        return new GameMap(definition, TimeSpan.FromMinutes(1), 8);
    }

    private static GameMapDefinition CreateBlockedTwoFloorMap(byte number)
    {
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, (byte)4, 3, ushort.MaxValue);

        MarkWalkable(terrain, 10, 10);
        MarkWalkable(terrain, 11, 10);
        MarkWalkable(terrain, 10, 11);
        MarkWalkable(terrain, 11, 11);
        MarkWalkable(terrain, 200, 200);
        MarkWalkable(terrain, 201, 200);
        MarkWalkable(terrain, 200, 201);
        MarkWalkable(terrain, 201, 201);

        return new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = terrain,
        };
    }

    private static GameMapDefinition CreateMapWithMisleadingCloserGate(byte number)
    {
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, (byte)4, 3, ushort.MaxValue);

        MarkWalkable(terrain, 200, 189);
        MarkWalkable(terrain, 201, 189);
        MarkWalkable(terrain, 200, 190);
        MarkWalkable(terrain, 201, 190);
        for (var x = 150; x <= 201; x++)
        {
            MarkWalkable(terrain, (byte)x, 200);
            MarkWalkable(terrain, (byte)x, 201);
        }

        return new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = terrain,
        };
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

    private static WarpInfo CreateWarpInfo(string name, ExitGate gate, int levelRequirement = 0)
    {
        return new MUnique.OpenMU.Persistence.BasicModel.WarpInfo
        {
            Id = Guid.NewGuid(),
            Name = name,
            Gate = gate,
            LevelRequirement = levelRequirement,
        };
    }

    [Guid("609DE0CD-938A-46CC-8731-C5CF1C791ED5")]
    private sealed class MapChangeRecordingPlugIn : IPlayerStateChangedPlugIn
    {
        public int MapChangeCount { get; private set; }

        public ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
        {
            if (currentState == PlayerState.ChangingMap)
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
