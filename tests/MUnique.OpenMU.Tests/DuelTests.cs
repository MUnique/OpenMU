// <copyright file="DuelTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using DuelConfiguration = MUnique.OpenMU.DataModel.Configuration.DuelConfiguration;
using GameConfiguration = MUnique.OpenMU.Persistence.BasicModel.GameConfiguration;
using ExitGate = MUnique.OpenMU.Persistence.BasicModel.ExitGate;

/// <summary>
/// Tests for the two duel variants, <see cref="DuelVariant.DuelArena"/> and
/// <see cref="DuelVariant.CurrentMap"/>.
/// </summary>
[TestFixture]
public class DuelTests
{
    private const byte FirstPlayerGateX = 10;
    private const byte SecondPlayerGateX = 20;
    private const byte SpectatorsGateX = 30;
    private const byte GateY = 50;

    /// <summary>
    /// Tests that a duel of the <see cref="DuelVariant.CurrentMap"/> variant takes place where
    /// the duelists are standing, so they are neither teleported nor assigned a duel area.
    /// </summary>
    [Test]
    public async ValueTask CurrentMapVariantKeepsDuelistsWhereTheyAreAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.CurrentMap);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var map = requester.CurrentMap;
        requester.Position = new Point(100, 100);
        opponent.Position = new Point(102, 100);

        var duelRoom = await StartDuelAsync(requester, opponent).ConfigureAwait(false);

        try
        {
            Assert.Multiple(() =>
            {
                Assert.That(duelRoom.IsInCurrentMap, Is.True);
                Assert.That(duelRoom.Area, Is.Null);
                Assert.That(duelRoom.Map, Is.EqualTo(map!.Definition));
                Assert.That(requester.CurrentMap, Is.EqualTo(map));
                Assert.That(opponent.CurrentMap, Is.EqualTo(map));
                Assert.That(requester.Position, Is.EqualTo(new Point(100, 100)));
                Assert.That(opponent.Position, Is.EqualTo(new Point(102, 100)));
            });
        }
        finally
        {
            await duelRoom.CancelDuelAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that a duel of the <see cref="DuelVariant.DuelArena"/> variant teleports the
    /// duelists to the gates of a duel area.
    /// </summary>
    [Test]
    public async ValueTask DuelArenaVariantWarpsDuelistsToTheirGatesAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.DuelArena);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);

        var duelRoom = await StartDuelAsync(requester, opponent).ConfigureAwait(false);

        try
        {
            Assert.Multiple(() =>
            {
                Assert.That(duelRoom.IsInCurrentMap, Is.False);
                Assert.That(duelRoom.Area, Is.EqualTo(gameContext.Configuration.DuelConfiguration!.DuelAreas.First()));
                Assert.That(requester.SelectedCharacter!.PositionX, Is.EqualTo(FirstPlayerGateX));
                Assert.That(requester.SelectedCharacter.PositionY, Is.EqualTo(GateY));
                Assert.That(opponent.SelectedCharacter!.PositionX, Is.EqualTo(SecondPlayerGateX));
                Assert.That(opponent.SelectedCharacter.PositionY, Is.EqualTo(GateY));
            });
        }
        finally
        {
            await duelRoom.CancelDuelAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that duels of the <see cref="DuelVariant.CurrentMap"/> variant don't occupy a duel
    /// room, so more of them can run at the same time than there are configured duel areas.
    /// </summary>
    [Test]
    public async ValueTask CurrentMapVariantIsNotLimitedByTheDuelAreasAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.CurrentMap, areaCount: 1);
        var (firstRequester, firstOpponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var (secondRequester, secondOpponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);

        var firstRoom = await gameContext.DuelRoomManager.GetFreeDuelRoomAsync(firstRequester, firstOpponent).ConfigureAwait(false);
        var secondRoom = await gameContext.DuelRoomManager.GetFreeDuelRoomAsync(secondRequester, secondOpponent).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(firstRoom, Is.Not.Null);
            Assert.That(secondRoom, Is.Not.Null);
            Assert.That(secondRoom, Is.Not.EqualTo(firstRoom));
        });
    }

    /// <summary>
    /// Tests that a duel of the <see cref="DuelVariant.DuelArena"/> variant is only possible as
    /// long as a duel area is free.
    /// </summary>
    [Test]
    public async ValueTask DuelArenaVariantIsLimitedByTheDuelAreasAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.DuelArena, areaCount: 1);
        var (firstRequester, firstOpponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var (secondRequester, secondOpponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);

        var firstRoom = await gameContext.DuelRoomManager.GetFreeDuelRoomAsync(firstRequester, firstOpponent).ConfigureAwait(false);
        var secondRoom = await gameContext.DuelRoomManager.GetFreeDuelRoomAsync(secondRequester, secondOpponent).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(firstRoom, Is.Not.Null);
            Assert.That(secondRoom, Is.Null);
        });
    }

    /// <summary>
    /// Tests that a duelist of a duel in the current map respawns at the spawn gate of the map,
    /// instead of being moved to a duel area.
    /// </summary>
    [Test]
    public async ValueTask CurrentMapDuelistRespawnsAtTheMapSpawnGateAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.CurrentMap);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var mapDefinition = requester.CurrentMap!.Definition;
        var spawnGate = CreateGate(mapDefinition, 55, 66);
        spawnGate.IsSpawnGate = true;
        mapDefinition.ExitGates.Add(spawnGate);
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new DuelSpawnGatePlugIn());
        var duelRoom = new DuelRoom(null, requester, opponent) { State = DuelState.DuelStarted };
        requester.DuelRoom = duelRoom;
        opponent.DuelRoom = duelRoom;

        await requester.WarpToSafezoneAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(duelRoom.GetSpawnGate(requester), Is.Null);
            Assert.That(requester.SelectedCharacter!.PositionX, Is.EqualTo(55));
            Assert.That(requester.SelectedCharacter.PositionY, Is.EqualTo(66));
        });
    }

    /// <summary>
    /// Tests that a duelist of a duel in the duel arena is spawned at its gate of the duel area.
    /// </summary>
    [Test]
    public async ValueTask DuelArenaDuelistRespawnsAtItsDuelAreaGateAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.DuelArena);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new DuelSpawnGatePlugIn());
        var area = gameContext.Configuration.DuelConfiguration!.DuelAreas.First();
        var duelRoom = new DuelRoom(area, requester, opponent) { State = DuelState.DuelStarted };
        requester.DuelRoom = duelRoom;
        opponent.DuelRoom = duelRoom;

        await requester.WarpToSafezoneAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(duelRoom.GetSpawnGate(requester), Is.EqualTo(area.FirstPlayerGate));
            Assert.That(requester.SelectedCharacter!.PositionX, Is.EqualTo(FirstPlayerGateX));
            Assert.That(requester.SelectedCharacter.PositionY, Is.EqualTo(GateY));
        });
    }

    /// <summary>
    /// Tests that killing the duel opponent doesn't make the duelist a player killer, but
    /// killing somebody else during the duel does. The latter is only possible when the duel
    /// takes place in the current map.
    /// </summary>
    [Test]
    public async ValueTask OnlyTheOpponentKillIsExemptedFromThePlayerKillerPenaltyAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.CurrentMap);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var bystander = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var duelRoom = new DuelRoom(null, requester, opponent) { State = DuelState.DuelStarted };
        requester.DuelRoom = duelRoom;
        opponent.DuelRoom = duelRoom;
        requester.SelectedCharacter!.State = HeroState.Normal;
        opponent.SelectedCharacter!.State = HeroState.Normal;
        bystander.SelectedCharacter!.State = HeroState.Normal;

        await requester.AfterKilledPlayerAsync(opponent).ConfigureAwait(false);
        var stateAfterOpponentKill = requester.SelectedCharacter.State;

        await requester.AfterKilledPlayerAsync(bystander).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(stateAfterOpponentKill, Is.EqualTo(HeroState.Normal));
            Assert.That(requester.SelectedCharacter.State, Is.EqualTo(HeroState.PlayerKillWarning));
        });
    }

    /// <summary>
    /// Tests that a player which is hit by a duelist keeps its right of self-defense, while the
    /// duelists don't get self-defense against each other.
    /// </summary>
    [Test]
    public async ValueTask OnlyTheOpponentHitIsExemptedFromSelfDefenseAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.CurrentMap);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var bystander = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var duelRoom = new DuelRoom(null, requester, opponent) { State = DuelState.DuelStarted };
        requester.DuelRoom = duelRoom;
        opponent.DuelRoom = duelRoom;
        var selfDefense = new SelfDefensePlugIn();
        var hitInfo = new HitInfo(100, 0, DamageAttributes.Undefined);

        selfDefense.AttackableGotHit(opponent, requester, hitInfo);
        selfDefense.AttackableGotHit(bystander, requester, hitInfo);

        Assert.Multiple(() =>
        {
            Assert.That(gameContext.SelfDefenseState.ContainsKey((requester, opponent)), Is.False);
            Assert.That(gameContext.SelfDefenseState.ContainsKey((requester, bystander)), Is.True);
        });
    }

    /// <summary>
    /// Tests that only a kill of the duel opponent counts for the duel score.
    /// </summary>
    [Test]
    public async ValueTask OnlyTheOpponentKillCountsForTheDuelScoreAsync()
    {
        var gameContext = CreateGameContext(DuelVariant.CurrentMap);
        var (requester, opponent) = await CreateDuelistsAsync(gameContext).ConfigureAwait(false);
        var bystander = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var duelRoom = new DuelRoom(null, requester, opponent) { State = DuelState.DuelStarted };
        requester.DuelRoom = duelRoom;
        opponent.DuelRoom = duelRoom;
        bystander.DuelRoom = duelRoom; // e.g. a spectator of the duel
        var scorePlugIn = new UpdateDuelScorePlugIn();

        await scorePlugIn.AttackableGotKilledAsync(bystander, requester).ConfigureAwait(false);
        var scoreAfterBystanderKill = duelRoom.ScoreRequester;

        await scorePlugIn.AttackableGotKilledAsync(opponent, requester).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(scoreAfterBystanderKill, Is.EqualTo(0));
            Assert.That(duelRoom.ScoreRequester, Is.EqualTo(1));
        });
    }

    private static async ValueTask<DuelRoom> StartDuelAsync(Player requester, Player opponent)
    {
        var duelActions = new DuelActions();
        await duelActions.HandleDuelRequestAsync(requester, opponent).ConfigureAwait(false);
        Assert.That(requester.DuelRoom, Is.Not.Null, "The duel was not requested.");

        await duelActions.HandleDuelResponseAsync(opponent, requester, true).ConfigureAwait(false);
        var duelRoom = requester.DuelRoom;
        Assert.That(duelRoom, Is.Not.Null, "The duel was not accepted.");
        Assert.That(duelRoom!.State, Is.EqualTo(DuelState.DuelAccepted));

        return duelRoom;
    }

    private static async ValueTask<(Player Requester, Player Opponent)> CreateDuelistsAsync(IGameContext gameContext)
    {
        var requester = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var opponent = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await requester.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await opponent.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return (requester, opponent);
    }

    private static IGameContext CreateGameContext(DuelVariant variant, int areaCount = 4)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var persistenceContext = contextProvider.CreateNewContext();
        var gameConfiguration = persistenceContext.CreateNew<GameConfiguration>();
        var map = persistenceContext.CreateNew<Persistence.BasicModel.GameMapDefinition>();
        map.Number = 0;
        map.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfiguration.Maps.Add(map);
        gameConfiguration.RecoveryInterval = int.MaxValue;
        gameConfiguration.MaximumInventoryMoney = int.MaxValue;
        gameConfiguration.DuelConfiguration = CreateDuelConfiguration(persistenceContext, map, variant, areaCount);

        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(gameConfiguration, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;

        return gameContext;
    }

    private static DuelConfiguration CreateDuelConfiguration(Persistence.IContext persistenceContext, GameMapDefinition map, DuelVariant variant, int areaCount)
    {
        var duelConfiguration = persistenceContext.CreateNew<DuelConfiguration>();
        duelConfiguration.Variant = variant;
        duelConfiguration.MaximumScore = 10;
        duelConfiguration.MaximumSpectatorsPerDuelRoom = 5;
        duelConfiguration.MinimumCharacterLevel = 0;
        duelConfiguration.EntranceFee = 0;

        for (short i = 0; i < areaCount; i++)
        {
            var area = persistenceContext.CreateNew<DuelArea>();
            area.Index = i;
            area.FirstPlayerGate = CreateGate(map, FirstPlayerGateX, (byte)(GateY + i));
            area.SecondPlayerGate = CreateGate(map, SecondPlayerGateX, (byte)(GateY + i));
            area.SpectatorsGate = CreateGate(map, SpectatorsGateX, (byte)(GateY + i));
            duelConfiguration.DuelAreas.Add(area);
        }

        return duelConfiguration;
    }

    private static ExitGate CreateGate(GameMapDefinition map, byte x, byte y)
    {
        return new ExitGate
        {
            Map = map,
            X1 = x,
            X2 = x,
            Y1 = y,
            Y2 = y,
            Direction = Direction.West,
        };
    }
}
