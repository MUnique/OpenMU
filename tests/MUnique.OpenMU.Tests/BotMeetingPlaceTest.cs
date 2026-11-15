// <copyright file="BotMeetingPlaceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
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
/// Tests how a bot gets to the meeting place a player whispered to it (see <see cref="BotChatPlugIn"/>):
/// reading a warp like "devias3" or "losttower7" from the whisper, choosing the warp entry which gets
/// it there, and warping through it instead of walking from the town.
/// </summary>
[TestFixture]
public class BotMeetingPlaceTest
{
    private const byte LostTowerNumber = 4;

    /// <summary>
    /// A warp of the warp list names the place as well as a map: "devias3 227 227" is Devias, through
    /// the warp Devias3. "devias" alone is the map - the warp of the same name doesn't take its place.
    /// </summary>
    /// <param name="message">The whisper.</param>
    /// <param name="expectedMap">The expected map number.</param>
    /// <param name="expectedWarp">The expected warp name, or <c>null</c> for none.</param>
    /// <param name="expectedX">The expected x coordinate, or -1 for none.</param>
    /// <param name="expectedY">The expected y coordinate, or -1 for none.</param>
    [TestCase("devias3 227 227", 2, "Devias3", 227, 227)]
    [TestCase("come to LostTower7 pls", 4, "LostTower7", -1, -1)]
    [TestCase("tarkan2, 90 160", 8, "Tarkan2", 90, 160)]
    [TestCase("devias 215 40", 2, null, 215, 40)]
    [TestCase("lost tower 100 120", 4, null, 100, 120)]
    public void ReadsAWarpAsThePlace(string message, int expectedMap, string? expectedWarp, int expectedX, int expectedY)
    {
        var maps = new List<GameMapDefinition>
        {
            new() { Number = 2, Name = "Devias" },
            new() { Number = LostTowerNumber, Name = "Lost Tower" },
            new() { Number = 8, Name = "Tarkan" },
        };
        var warps = new List<WarpInfo>
        {
            new() { Name = "Devias", Gate = new ExitGate { Map = maps[0] } },
            new() { Name = "Devias3", Gate = new ExitGate { Map = maps[0] } },
            new() { Name = "LostTower", Gate = new ExitGate { Map = maps[1] } },
            new() { Name = "LostTower7", Gate = new ExitGate { Map = maps[1] } },
            new() { Name = "Tarkan2", Gate = new ExitGate { Map = maps[2] } },
        };

        var place = BotChatComposer.FindPlace(message, maps, warps);

        Assert.That(place, Is.Not.Null);
        Assert.That(place!.Value.Map.Number, Is.EqualTo(expectedMap));
        Assert.That(place.Value.Warp?.Name.ValueInNeutralLanguage, Is.EqualTo(expectedWarp));
        Point? expectedPosition = expectedX < 0 ? null : new Point((byte)expectedX, (byte)expectedY);
        Assert.That(place.Value.Position, Is.EqualTo(expectedPosition));
    }

    /// <summary>
    /// Coordinates on a floor of the Lost Tower which can't be walked to from the entrance: the bot
    /// enters through the warp of that floor.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask EntersTheFloorOfTheCoordinatesAsync()
    {
        var (bot, _, _, upperGate) = await CreateLostTowerAsync().ConfigureAwait(false);

        var entry = BotChatPlugIn.FindMeetingEntry(bot, upperGate.Map!, null, new Point(200, 200));

        Assert.That(entry, Is.Not.Null);
        Assert.That(entry!.Value.Gate, Is.SameAs(upperGate));
        Assert.That(entry.Value.Position, Is.EqualTo(new Point(200, 200)));
    }

    /// <summary>
    /// A bot whose level isn't enough for the warp to that floor can't come - the other warps of the
    /// map don't lead there.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask CantComeWithoutTheLevelOfTheFloorAsync()
    {
        var (bot, _, upperWarp, upperGate) = await CreateLostTowerAsync().ConfigureAwait(false);
        upperWarp.LevelRequirement = 400;

        Assert.That(BotChatPlugIn.FindMeetingEntry(bot, upperGate.Map!, null, new Point(200, 200)), Is.Null);
        Assert.That(BotChatPlugIn.FindMeetingEntry(bot, upperGate.Map!, upperWarp, null), Is.Null);
    }

    /// <summary>
    /// A named warp without coordinates: the bot waits at the warp's gate.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask WaitsAtTheGateOfTheNamedWarpAsync()
    {
        var (bot, _, upperWarp, upperGate) = await CreateLostTowerAsync().ConfigureAwait(false);

        var entry = BotChatPlugIn.FindMeetingEntry(bot, upperGate.Map!, upperWarp, null);

        Assert.That(entry, Is.Not.Null);
        Assert.That(entry!.Value.Gate, Is.SameAs(upperGate));
        Assert.That(entry.Value.Position, Is.EqualTo(new Point(200, 200)));
    }

    /// <summary>
    /// A gate with some blocked tiles - like the town gates of Lorencia and Devias - still counts for a
    /// meeting: a bot landing on a blocked tile ends up at the spawn gate, which is the town anyway.
    /// Otherwise the bot would walk over from a far away gate.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask TakesTheNearGateWithBlockedTilesAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = 100;
        var terrain = CreateBlockedTerrain();
        for (var x = 10; x <= 250; x++)
        {
            MarkWalkable(terrain, (byte)x, 10);
        }

        var map = CreateMap(2, terrain);
        var townGate = CreateGate(map, 10, 9, 13, 12);
        var farGate = CreateGate(map, 240, 10, 241, 11);
        gameContext.Configuration.WarpList.Add(new MUnique.OpenMU.Persistence.BasicModel.WarpInfo { Id = Guid.NewGuid(), Name = "Devias", Gate = townGate });
        gameContext.Configuration.WarpList.Add(new MUnique.OpenMU.Persistence.BasicModel.WarpInfo { Id = Guid.NewGuid(), Name = "Devias3", Gate = farGate });

        var entry = BotChatPlugIn.FindMeetingEntry(bot, map, null, new Point(30, 10));

        Assert.That(entry?.Gate, Is.SameAs(townGate));
    }

    /// <summary>
    /// The bot stands on the entrance floor of the Lost Tower when the player names a place on another
    /// floor: it warps through the meeting's gate once, and then waits there instead of trying to walk.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask WarpsToTheOtherFloorOnTheSameMapAsync()
    {
        var (bot, lowerGate, _, upperGate) = await CreateLostTowerAsync().ConfigureAwait(false);
        var mapDefinition = upperGate.Map!;
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8);
        var mapChanges = new MapChangeRecordingPlugIn(bot);
        bot.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangedPlugIn>(mapChanges);
        var navigator = new BotNavigator(bot);
        bot.SetCurrentMapSilently(map);
        bot.SelectedCharacter!.CurrentMap = mapDefinition;
        bot.SelectedCharacter.PositionX = lowerGate.X1;
        bot.SelectedCharacter.PositionY = lowerGate.Y1;
        bot.EventMeeting = new BotEventMeeting(mapDefinition, new Point(201, 201), DateTime.UtcNow.AddMinutes(5), upperGate);

        var consumed = await navigator.TryEventMeetingAsync(map, false, CancellationToken.None).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(mapChanges.MapChangeCount, Is.EqualTo(1));
        Assert.That(bot.SelectedCharacter.PositionX, Is.InRange(200, 201));
        Assert.That(bot.SelectedCharacter.PositionY, Is.InRange(200, 201));

        bot.SetCurrentMapSilently(map);
        var waits = await navigator.TryEventMeetingAsync(map, false, CancellationToken.None).ConfigureAwait(false);

        Assert.That(waits, Is.True);
        Assert.That(mapChanges.MapChangeCount, Is.EqualTo(1), "standing at the meeting point, the bot doesn't warp again");
    }

    /// <summary>
    /// A map without an entry in the warp list: a bot standing on it walks to the named coordinates;
    /// a bot elsewhere can't get there.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask WalksThereOnAMapWithoutWarpAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var terrain = CreateBlockedTerrain();
        for (var x = 10; x <= 30; x++)
        {
            MarkWalkable(terrain, (byte)x, 10);
        }

        var mapDefinition = CreateMap(LostTowerNumber, terrain);
        var target = new Point(30, 10);

        Assert.That(BotChatPlugIn.FindMeetingEntry(bot, mapDefinition, null, target), Is.Null);

        bot.SetCurrentMapSilently(new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 8));
        bot.Position = new Point(10, 10);
        var entry = BotChatPlugIn.FindMeetingEntry(bot, mapDefinition, null, target);

        Assert.That(entry, Is.Not.Null);
        Assert.That(entry!.Value.Gate, Is.Null);
        Assert.That(entry.Value.Position, Is.EqualTo(target));
    }

    /// <summary>
    /// A place the bot can't get to is declined, but the bot keeps waiting for the player's whisper:
    /// the player may name another place. A place it can get to ends the offer - the bot sets off.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask KeepsTheOfferUntilAPlaceItCanGetToAsync()
    {
        var (bot, _, upperWarp, _) = await CreateLostTowerAsync().ConfigureAwait(false);
        upperWarp.LevelRequirement = 400;
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.IsAlive = true;
        bot.Account!.IsBot = true;
        var requester = await PlayerTestHelper.CreatePlayerAsync(bot.GameContext).ConfigureAwait(false);
        var plugIn = new BotChatPlugIn { Configuration = BotChatConfiguration.CreateDefault() };
        var throttle = plugIn.GetThrottle(bot.GameContext);
        var offer = new BotPartyOffer(requester, DateTime.UtcNow.AddMinutes(3));
        Assert.That(throttle.TryReservePartyOffer(bot, offer, DateTime.UtcNow), Is.True);

        plugIn.WhisperMessageReceived(requester, bot, "lost tower 200 200", new CancelEventArgs());

        Assert.That(throttle.GetOpenPartyOffer(bot, DateTime.UtcNow), Is.SameAs(offer));
        Assert.That(bot.EventMeeting, Is.Null);

        plugIn.WhisperMessageReceived(requester, bot, "lost tower 11 11", new CancelEventArgs());

        Assert.That(throttle.GetOpenPartyOffer(bot, DateTime.UtcNow), Is.Null);
        Assert.That(bot.EventMeeting?.NpcPosition, Is.EqualTo(new Point(11, 11)));
    }

    /// <summary>
    /// Creates a Lost Tower of two floors which aren't connected by walkable ground, with a warp to each
    /// (the upper one at level 70), and a bot of level 100.
    /// </summary>
    private static async ValueTask<(OfflinePlayer Bot, ExitGate LowerGate, WarpInfo UpperWarp, ExitGate UpperGate)> CreateLostTowerAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = 100;

        var terrain = CreateBlockedTerrain();
        for (var x = 10; x <= 12; x++)
        {
            for (var y = 10; y <= 12; y++)
            {
                MarkWalkable(terrain, (byte)x, (byte)y);
                MarkWalkable(terrain, (byte)(x + 190), (byte)(y + 190));
            }
        }

        var map = CreateMap(LostTowerNumber, terrain);
        map.Name = "Lost Tower";
        gameContext.Configuration.Maps.Add(map);
        var lowerGate = CreateGate(map, 10, 10, 11, 11);
        lowerGate.IsSpawnGate = true;
        var upperGate = CreateGate(map, 200, 200, 201, 201);
        var lowerWarp = new MUnique.OpenMU.Persistence.BasicModel.WarpInfo { Id = Guid.NewGuid(), Name = "LostTower", Gate = lowerGate, LevelRequirement = 50 };
        var upperWarp = new MUnique.OpenMU.Persistence.BasicModel.WarpInfo { Id = Guid.NewGuid(), Name = "LostTower7", Gate = upperGate, LevelRequirement = 70 };
        gameContext.Configuration.WarpList.Add(lowerWarp);
        gameContext.Configuration.WarpList.Add(upperWarp);
        return (bot, lowerGate, upperWarp, upperGate);
    }

    private static byte[] CreateBlockedTerrain()
    {
        var terrain = new byte[ushort.MaxValue + 3];
        Array.Fill(terrain, (byte)4, 3, ushort.MaxValue);
        return terrain;
    }

    private static void MarkWalkable(byte[] terrain, byte x, byte y)
    {
        terrain[3 + (y * 256) + x] = 0;
    }

    private static GameMapDefinition CreateMap(byte number, byte[] terrain)
        => new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = terrain,
        };

    private static ExitGate CreateGate(GameMapDefinition map, byte x1, byte y1, byte x2, byte y2)
    {
        var gate = new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = map,
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
        };

        map.ExitGates.Add(gate);
        return gate;
    }

    [Guid("0E0B1D57-2C59-4C7E-8E39-7A4C5F0B6C11")]
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
}
