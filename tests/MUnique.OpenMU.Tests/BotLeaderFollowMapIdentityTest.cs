// <copyright file="BotLeaderFollowMapIdentityTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;
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
    /// walking cannot reach him, the follower should regroup through the legal warp entry closest to
    /// the leader.
    /// </summary>
    [Test]
    public async ValueTask SameMapFollowerWarpsToNearestLeaderFloorWhenWalkingIsImpossibleAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var leader = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var navigator = new BotNavigator(bot);

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

        var consumed = await navigator.TryFollowLeaderAsync(map, leader, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 201));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));
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

    private static WarpInfo CreateWarpInfo(string name, ExitGate gate)
    {
        return new MUnique.OpenMU.Persistence.BasicModel.WarpInfo
        {
            Id = Guid.NewGuid(),
            Name = name,
            Gate = gate,
        };
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
