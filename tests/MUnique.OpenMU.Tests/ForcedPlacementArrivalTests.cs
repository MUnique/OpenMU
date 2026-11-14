// <copyright file="ForcedPlacementArrivalTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using System.IO.Pipelines;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the arrival of forced placements with the view plugins a game server selects for the client version:
/// a relocated, still reserved death uses the map change handshake, and arrival keeps a death of the placed life.
/// </summary>
[TestFixture]
public class ForcedPlacementArrivalTests
{
    private const byte MapChangedCode = 0x1C;
    private const byte MeetPlayersCode = 0x12;
    private const byte RespawnCode = 0xF3;

    /// <summary>
    /// A relocated reserved death announces only the map change; the world is sent after the client is ready,
    /// a repeated ready packet is ignored and the original death timer revives the life once.
    /// </summary>
    /// <param name="season">The client season.</param>
    /// <param name="episode">The client episode.</param>
    /// <param name="sameMap">Whether the relocation stays on the current map.</param>
    [TestCase((byte)6, (byte)3, false)]
    [TestCase((byte)6, (byte)3, true)]
    [TestCase((byte)106, (byte)3, false)]
    [TestCase((byte)0, (byte)75, false)]
    public async Task ReservedRelocationWaitsForClientReadyAsync(byte season, byte episode, bool sameMap)
    {
        await using var fixture = await ArrivalFixture.CreateAsync(new ClientVersion(season, episode, ClientLanguage.English), sameMap).ConfigureAwait(false);
        var victim = fixture.Victim;
        var death = await fixture.KillVictimAsync().ConfigureAwait(false);
        var source = await ReadDeathRespawnCancellationAsync(victim).ConfigureAwait(false);

        var relocation = await fixture.MarkAsync().ConfigureAwait(false);
        await fixture.Peer.RunPersistenceExclusiveAsync(() => victim.RespawnAtAsync(fixture.Gate)).ConfigureAwait(false);
        var beforeReady = await fixture.ReadSinceAsync(relocation).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(beforeReady.Select(p => p.Code).ToList(), Is.EqualTo(new List<byte> { MapChangedCode }), "Only the map change precedes the client's ready packet.");
            Assert.That(victim.CurrentMap, Is.Null);
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
            Assert.That(victim.SelectedCharacter!.CurrentMap, Is.SameAs(fixture.Destination.Definition));
            Assert.That(victim.IsAlive, Is.False);
            Assert.That(victim.LastDeath, Is.SameAs(death));
            Assert.That(victim.GetPlacementDeathSnapshot().Reserved, Is.True);
        });

        var ready = await fixture.MarkAsync().ConfigureAwait(false);
        await fixture.SendReadyAsync().ConfigureAwait(false);
        var afterReady = await fixture.ReadSinceAsync(ready).ConfigureAwait(false);
        var arrivedSource = await ReadDeathRespawnCancellationAsync(victim).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(afterReady.Select(p => p.Code), Does.Contain(MeetPlayersCode), "The world is sent after the client is ready.");
            Assert.That(victim.CurrentMap, Is.SameAs(fixture.Destination));
            Assert.That(fixture.Destination.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(victim.IsAlive, Is.False, "Arrival does not revive a reserved death.");
            Assert.That(arrivedSource, Is.SameAs(source));
        });

        var repeated = await fixture.MarkAsync().ConfigureAwait(false);
        await fixture.SendReadyAsync().ConfigureAwait(false);
        Assert.That(await fixture.ReadSinceAsync(repeated).ConfigureAwait(false), Is.Empty);
        Assert.That(fixture.Destination.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));

        var revival = await fixture.MarkAsync().ConfigureAwait(false);
        await fixture.WaitAliveAsync().ConfigureAwait(false);
        var revived = await fixture.ReadSinceAsync(revival).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(revived.Select(p => p.Code), Does.Contain(RespawnCode));
            Assert.That(victim.LastDeath, Is.SameAs(death));
            Assert.That(fixture.Deaths, Is.EqualTo(1));
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        });
    }

    /// <summary>
    /// The original death timer which ends while the client is still loading defers to the actual arrival.
    /// </summary>
    [Test]
    public async Task ReservedRelocationDefersDueRespawnUntilReadyAsync()
    {
        await using var fixture = await ArrivalFixture.CreateAsync(new ClientVersion(6, 3, ClientLanguage.English), false).ConfigureAwait(false);
        var victim = fixture.Victim;
        var death = await fixture.KillVictimAsync().ConfigureAwait(false);
        var source = await ReadDeathRespawnCancellationAsync(victim).ConfigureAwait(false);
        await fixture.Peer.RunPersistenceExclusiveAsync(() => victim.RespawnAtAsync(fixture.Gate)).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(4)).ConfigureAwait(false);
        var loadingSource = await ReadDeathRespawnCancellationAsync(victim).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
            Assert.That(victim.CurrentMap, Is.Null);
            Assert.That(victim.IsAlive, Is.False);
            Assert.That(victim.LastDeath, Is.SameAs(death));
            Assert.That(loadingSource, Is.SameAs(source));
        });

        await fixture.SendReadyAsync().ConfigureAwait(false);
        await fixture.WaitAliveAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(victim.LastDeath, Is.SameAs(death));
            Assert.That(fixture.Deaths, Is.EqualTo(1));
            Assert.That(victim.CurrentMap, Is.Not.Null);
            Assert.That(victim.CurrentMap!.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
        });
    }

    /// <summary>A session which leaves while its relocation waits for the client keeps no registration of it.</summary>
    /// <param name="transition">The lifecycle operation while the client is loading.</param>
    [TestCase("disconnect")]
    [TestCase("reselect")]
    public async Task ReservedRelocationPendingReadyFollowsTeardownAsync(string transition)
    {
        await using var fixture = await ArrivalFixture.CreateAsync(new ClientVersion(6, 3, ClientLanguage.English), false).ConfigureAwait(false);
        var victim = fixture.Victim;
        await fixture.KillVictimAsync().ConfigureAwait(false);
        await fixture.Peer.RunPersistenceExclusiveAsync(() => victim.RespawnAtAsync(fixture.Gate)).ConfigureAwait(false);
        Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));

        if (transition == "disconnect")
        {
            await victim.DisconnectAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Destination.GetPlayers(), Does.Not.Contain(victim));
                Assert.That(fixture.Source.GetPlayers(), Does.Not.Contain(victim));
                Assert.That(victim.SelectedCharacter, Is.Null);
            });
            return;
        }

        var character = victim.SelectedCharacter!;
        var attributes = victim.Attributes;
        await victim.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(victim, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
        await victim.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(victim)).ConfigureAwait(false);
        await victim.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(victim, character.Name)).ConfigureAwait(false);
        await fixture.SendReadyAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(victim.Attributes, Is.Not.SameAs(attributes));
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(victim.CurrentMap!.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
            Assert.That(fixture.Deaths, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Placements which revive or keep a living life still use the respawn packet and arrive at once,
    /// and a lethal hit published during that arrival dies once and respawns.
    /// </summary>
    /// <param name="placement">living-generic relocation, or the actual death respawn.</param>
    /// <param name="lethalOnArrival">Whether a real lethal hit lands while the placed life is added to the map.</param>
    [TestCase("living-generic", false)]
    [TestCase("living-generic", true)]
    [TestCase("death-respawn", false)]
    [TestCase("death-respawn", true)]
    public async Task RespawnPlacementArrivesWithItsDeathAsync(string placement, bool lethalOnArrival)
    {
        await using var fixture = await ArrivalFixture.CreateAsync(new ClientVersion(6, 3, ClientLanguage.English), false).ConfigureAwait(false);
        var victim = fixture.Victim;
        var expectedDeaths = 0;
        if (placement == "death-respawn")
        {
            await fixture.KillVictimAsync().ConfigureAwait(false);
            expectedDeaths++;
        }

        var arrivalDeath = default(DeathInformation);
        var arrivalHits = 0;
        var arrivalMap = placement == "death-respawn" ? fixture.Source : fixture.Destination;
        arrivalMap.ObjectAdded += async args =>
        {
            if (lethalOnArrival && ReferenceEquals(args.Object, victim) && Interlocked.Increment(ref arrivalHits) == 1)
            {
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                await victim.ApplyPoisonDamageAsync(fixture.Peer, (uint)Math.Max(1, victim.Attributes![Stats.CurrentHealth])).ConfigureAwait(false);
                arrivalDeath = victim.LastDeath;
            }
        };

        var start = await fixture.MarkAsync().ConfigureAwait(false);
        if (placement == "death-respawn")
        {
            await ArrivalFixture.WaitUntilAsync(
                () => lethalOnArrival
                    ? Volatile.Read(ref arrivalHits) > 0 && victim.PlayerState.CurrentState == PlayerState.Dead
                    : victim.PlayerState.CurrentState == PlayerState.EnteredWorld,
                TimeSpan.FromSeconds(6)).ConfigureAwait(false);
        }
        else
        {
            await fixture.Peer.RunPersistenceExclusiveAsync(() => victim.RespawnAtAsync(fixture.Gate)).ConfigureAwait(false);
        }

        var packets = await fixture.ReadSinceAsync(start).ConfigureAwait(false);
        Assert.That(packets.Select(p => p.Code), Does.Contain(RespawnCode).And.Not.Contain(MapChangedCode));
        if (!lethalOnArrival)
        {
            Assert.Multiple(() =>
            {
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(victim.IsAlive, Is.True);
                Assert.That(victim.CurrentMap!.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
                Assert.That(fixture.Deaths, Is.EqualTo(expectedDeaths));
            });
            return;
        }

        Assert.Multiple(() =>
        {
            Assert.That(arrivalDeath, Is.Not.Null);
            Assert.That(victim.LastDeath, Is.SameAs(arrivalDeath));
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead), "The arrival hands the published death on instead of leaving the life in the world.");
            Assert.That(fixture.Deaths, Is.EqualTo(expectedDeaths + 1));
        });

        await fixture.WaitAliveAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(victim.LastDeath, Is.SameAs(arrivalDeath));
            Assert.That(fixture.Deaths, Is.EqualTo(expectedDeaths + 1));
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        });
    }

    [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_respawnAfterDeathCts")]
    private static extern ref CancellationTokenSource? DeathRespawnCancellation(Player player);

    private static async ValueTask<CancellationTokenSource?> ReadDeathRespawnCancellationAsync(Player player)
    {
        // Copies the private timer under the state exclusion which owns it, without changing the state.
        await using var scope = await player.PlayerState.TryBeginAdvanceToAsync(player.PlayerState.CurrentState).ConfigureAwait(false);
        scope.Allowed = false;
        return DeathRespawnCancellation(player);
    }

    private sealed class ArrivalFixture : IAsyncDisposable
    {
        private readonly MemoryStream _output;
        private readonly PipeWriter _writer;
        private int _deaths;

        public ArrivalFixture(GameServerContext context, MemoryStream output, PipeWriter writer, RemotePlayer victim, Player peer, GameMap source, GameMap destination)
        {
            this.Context = context;
            this._output = output;
            this._writer = writer;
            this.Victim = victim;
            this.Peer = peer;
            this.Source = source;
            this.Destination = destination;
            this.Gate = new ExitGate { Map = destination.Definition, X1 = 101, X2 = 101, Y1 = 100, Y2 = 100 };
            victim.Died += (_, _) => Interlocked.Increment(ref this._deaths);
        }

        public GameServerContext Context { get; }

        public RemotePlayer Victim { get; }

        public Player Peer { get; }

        public GameMap Source { get; }

        public GameMap Destination { get; }

        public ExitGate Gate { get; }

        public int Deaths => Volatile.Read(ref this._deaths);

        public static async ValueTask<ArrivalFixture> CreateAsync(ClientVersion version, bool sameMap)
        {
            var context = CreateContext();
            var output = new MemoryStream();
            var writer = PipeWriter.Create(output, new StreamPipeWriterOptions(leaveOpen: true));
            var connection = new Mock<IConnection>();
            connection.SetupGet(c => c.Connected).Returns(true);
            connection.SetupGet(c => c.Output).Returns(writer);
            connection.SetupGet(c => c.OutputLock).Returns(new AsyncLock());
            var victim = new RemotePlayer(context, connection.Object, version);
            var character = PlayerTestHelper.CreateCharacter(context);
            character.Name = "Victim";
            await PlayerTestHelper.EnterWorldAsync(victim, character).ConfigureAwait(false);
            Mock.Get(victim.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
            var peer = await PlayerTestHelper.CreatePlayerAsync(context).ConfigureAwait(false);
            var source = (await context.GetMapAsync(0).ConfigureAwait(false))!;
            var destination = sameMap ? source : (await context.GetMapAsync(1).ConfigureAwait(false))!;
            await PlaceAsync(victim, source.Definition, 20).ConfigureAwait(false);
            await PlaceAsync(peer, destination.Definition, 100).ConfigureAwait(false);
            victim.Attributes![Stats.CurrentHealth] = 10;
            Assert.That(victim.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>(), Is.Not.Null, "Every client version selects a respawn view.");
            return new ArrivalFixture(context, output, writer, victim, peer, source, destination);
        }

        public async ValueTask<DeathInformation> KillVictimAsync()
        {
            this.Victim.Attributes![Stats.CurrentHealth] = 10;
            await this.Victim.ApplyPoisonDamageAsync(this.Peer, 10).ConfigureAwait(false);
            Assert.That(this.Victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            return this.Victim.LastDeath!;
        }

        public async ValueTask<int> MarkAsync()
        {
            await this._writer.FlushAsync().ConfigureAwait(false);
            return checked((int)this._output.Length);
        }

        public async ValueTask<List<(byte Type, byte Code)>> ReadSinceAsync(int start)
        {
            await this._writer.FlushAsync().ConfigureAwait(false);
            var bytes = this._output.ToArray().AsMemory(start);
            var packets = new List<(byte Type, byte Code)>();
            while (!bytes.IsEmpty)
            {
                var span = bytes.Span;
                var length = span.GetPacketSize();
                packets.Add((span[0], span[..length].GetPacketType()));
                bytes = bytes[length..];
            }

            return packets;
        }

        public ValueTask SendReadyAsync()
        {
            var ready = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef.Length];
            _ = new MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef(ready);
            return new ValueTask(this.Victim.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(this.Victim, ready)).AsTask());
        }

        public async ValueTask WaitAliveAsync(TimeSpan? timeout = null)
        {
            var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(6));
            // Liveness is restored before the awaited map add; the arrival is complete with the state.
            while (!(this.Victim.IsAlive && this.Victim.PlayerState.CurrentState == PlayerState.EnteredWorld) && DateTime.UtcNow < deadline)
            {
                await Task.Delay(25).ConfigureAwait(false);
            }

            Assert.That(this.Victim.IsAlive, Is.True);
            Assert.That(this.Victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        }

        public static async ValueTask WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (!condition() && DateTime.UtcNow < deadline)
            {
                await Task.Delay(25).ConfigureAwait(false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await this.Victim.DisposeAsync().ConfigureAwait(false);
            await this.Peer.DisposeAsync().ConfigureAwait(false);
            await this._writer.CompleteAsync().ConfigureAwait(false);
            await this._output.DisposeAsync().ConfigureAwait(false);
            await this.Context.DisposeAsync().ConfigureAwait(false);
        }

        private static async ValueTask PlaceAsync(Player player, GameMapDefinition map, byte x)
        {
            await player.WarpToAsync(new ExitGate { Map = map, X1 = x, X2 = x, Y1 = 100, Y2 = 100 }).ConfigureAwait(false);
            await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
        }

        private static GameServerContext CreateContext()
        {
            var configuration = PlayerTestHelper.CreateGameConfiguration();
            configuration.MaximumLevel = 400;
            var first = configuration.Maps.Single();
            first.ExitGates.Add(new ExitGate { Map = first, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
            var second = new Mock<GameMapDefinition>();
            second.SetupAllProperties();
            second.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
            second.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
            second.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
            second.Object.Number = 1;
            second.Object.TerrainData = new byte[ushort.MaxValue + 3];
            second.Object.ExitGates.Add(new ExitGate { Map = second.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
            configuration.Maps.Add(second.Object);

            // The view plugins of the game server; the remote player selects them by its client version.
            var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
            plugInManager.DiscoverAndRegisterPlugInsOf<IViewPlugIn>();
            var mapInitializer = new MapInitializer(configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
            var context = new GameServerContext(
                new BasicModel.GameServerDefinition { GameConfiguration = configuration, ServerConfiguration = new BasicModel.GameServerConfiguration() },
                new Mock<IGuildServer>().Object,
                new Mock<IEventPublisher>().Object,
                new Mock<ILoginServer>().Object,
                new Mock<IFriendServer>().Object,
                new InMemoryPersistenceContextProvider(),
                mapInitializer,
                NullLoggerFactory.Instance,
                plugInManager,
                NullDropGenerator.Instance,
                new ConfigurationChangeMediator());
            mapInitializer.PlugInManager = context.PlugInManager;
            mapInitializer.PathFinderPool = context.PathFinderPool;
            return context;
        }
    }
}
