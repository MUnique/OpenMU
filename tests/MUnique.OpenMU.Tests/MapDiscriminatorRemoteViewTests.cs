// <copyright file="MapDiscriminatorRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using System.IO.Pipelines;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Character;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the extended client (MuMain) gets the discriminator of its map, which tells maps with
/// the same number apart (the stages of Devil Square), and that other clients get the original packets.
/// The packets are written into memory which is filled with 0xFF, as a reused output buffer may hold
/// the bytes of earlier packets.
/// </summary>
[TestFixture]
public class MapDiscriminatorRemoteViewTests
{
    private const short DevilSquareMapNumber = 9;
    private const byte MapChangedCode = 0x1C;
    private const byte CharacterInformationCode = 0xF3;
    private const byte CharacterInformationSubCode = 0x03;

    /// <summary>
    /// The value of the bytes in the memory which the output hands out, before a packet is written.
    /// </summary>
    private const byte StaleByte = 0xFF;

    /// <summary>
    /// The unused byte of <see cref="CharacterInformationExtended"/>, between the inventory extensions and the resets.
    /// </summary>
    private const int CharacterInformationUnusedByte = 89;

    /// <summary>
    /// The unused bytes of <see cref="MapChangedExtended"/>, between the rotation and the discriminator.
    /// </summary>
    private static readonly System.Range MapChangedUnusedBytes = 10..15;

    private static ClientVersion ExtendedClient { get; } = new(106, 3, ClientLanguage.Invariant);

    private static ClientVersion Season6Client { get; } = new(6, 3, ClientLanguage.English);

    /// <summary>
    /// The extended client gets the discriminator of the map with a map change, a teleport on the map and a
    /// failed map change. A discriminator which doesn't fit into a byte is sent as 0, not cut to another one.
    /// </summary>
    /// <param name="discriminator">The discriminator of the map.</param>
    /// <param name="expected">The discriminator which the client gets.</param>
    [TestCase(2, 2)]
    [TestCase(257, 0)]
    [TestCase(-1, 0)]
    public async Task ExtendedClientGetsTheDiscriminatorWithMapChangesAsync(int discriminator, int expected)
    {
        var (player, output, stage) = await CreatePlayerInWorldAsync(ExtendedClient, discriminator, enterOnStage: false).ConfigureAwait(false);

        await player.WarpToAsync(new ExitGate { Map = stage, X1 = 130, X2 = 130, Y1 = 90, Y2 = 90 }).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeFailedAsync()).ConfigureAwait(false);

        var packets = ReadPackets(output).Where(p => p[2] == MapChangedCode).ToList();
        Assert.That(packets.Select(p => p.Length), Is.EqualTo(new[] { MapChangedExtendedRef.Length, MapChangedExtendedRef.Length, MapChangedExtendedRef.Length }));
        var (mapChange, teleport, failedMapChange) = (Read(packets[0]), Read(packets[1]), Read(packets[2]));
        Assert.Multiple(() =>
        {
            Assert.That(mapChange.IsMapChange, Is.True, "map change");
            Assert.That(teleport.IsMapChange, Is.False, "teleport");
            Assert.That(failedMapChange.IsMapChange, Is.False, "failed map change");
            Assert.That(new[] { mapChange.MapNumber, teleport.MapNumber, failedMapChange.MapNumber }, Is.All.EqualTo(DevilSquareMapNumber));
            Assert.That(new[] { mapChange.MapDiscriminator, teleport.MapDiscriminator, failedMapChange.MapDiscriminator }, Is.All.EqualTo(expected));
            Assert.That(mapChange.PositionX, Is.EqualTo(130), "position x");
            Assert.That(mapChange.PositionY, Is.EqualTo(90), "position y");
        });

        static MapChangedExtended Read(byte[] packet) => packet.AsMemory();
    }

    /// <summary>
    /// The extended client gets the discriminator of the map when it enters the world. That's how it learns
    /// the stage of Devil Square, when it rejoins the game after losing its connection: the mini game puts
    /// the character on the map of its stage before the character information is sent.
    /// </summary>
    [Test]
    public async Task ExtendedClientGetsTheDiscriminatorWhenEnteringTheWorldAsync()
    {
        var (_, output, _) = await CreatePlayerInWorldAsync(ExtendedClient, 2, enterOnStage: true).ConfigureAwait(false);

        var packet = ReadPackets(output).Single(p => p[2] == CharacterInformationCode && p[3] == CharacterInformationSubCode);
        Assert.That(packet, Has.Length.EqualTo(CharacterInformationExtendedRef.Length));
        CharacterInformationExtended characterInformation = packet.AsMemory();
        Assert.That(characterInformation.MapId, Is.EqualTo(DevilSquareMapNumber));
        Assert.That(characterInformation.MapDiscriminator, Is.EqualTo(2));
    }

    /// <summary>
    /// The packets of the extended client write their unused bytes as 0, although the memory of the output
    /// still holds other bytes, e.g. of an earlier packet.
    /// </summary>
    [Test]
    public async Task ExtendedPacketsWriteTheirUnusedBytesAsZeroAsync()
    {
        var (player, output, stage) = await CreatePlayerInWorldAsync(ExtendedClient, 2, enterOnStage: false).ConfigureAwait(false);

        await player.WarpToAsync(new ExitGate { Map = stage, X1 = 130, X2 = 130, Y1 = 90, Y2 = 90 }).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeFailedAsync()).ConfigureAwait(false);

        var packets = ReadPackets(output);
        var mapChanges = packets.Where(p => p[2] == MapChangedCode).ToList();
        var characterInformation = packets.Single(p => p[2] == CharacterInformationCode && p[3] == CharacterInformationSubCode);
        Assert.Multiple(() =>
        {
            Assert.That(mapChanges, Has.Count.EqualTo(3), "map change, teleport and failed map change");
            Assert.That(mapChanges.Select(p => p[MapChangedUnusedBytes]), Is.All.All.EqualTo(0), "unused bytes of MapChangedExtended");
            Assert.That(characterInformation[CharacterInformationUnusedByte], Is.EqualTo(0), "unused byte of CharacterInformationExtended");
        });
    }

    /// <summary>
    /// Other clients get the original map change packet, which doesn't carry the discriminator.
    /// </summary>
    [Test]
    public async Task OtherClientsGetTheOriginalMapChangedAsync()
    {
        var (player, output, stage) = await CreatePlayerInWorldAsync(Season6Client, 6, enterOnStage: false).ConfigureAwait(false);

        await player.WarpToAsync(new ExitGate { Map = stage, X1 = 130, X2 = 130, Y1 = 90, Y2 = 90 }).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeFailedAsync()).ConfigureAwait(false);

        var packets = ReadPackets(output).Where(p => p[2] == MapChangedCode).ToList();
        Assert.That(packets.Select(p => p.Length), Is.EqualTo(new[] { MapChangedRef.Length, MapChangedRef.Length, MapChangedRef.Length }));
        Assert.That(packets.Select(p => ((MapChanged)p.AsMemory()).MapNumber), Is.All.EqualTo(DevilSquareMapNumber));
    }

    /// <summary>
    /// Creates a remote player, which entered the world on map 0 or on a map with the number of Devil Square and
    /// the specified discriminator, with the plugins of the original and the extended client.
    /// </summary>
    private static async ValueTask<(RemotePlayer Player, MemoryStream Output, GameMapDefinition Stage)> CreatePlayerInWorldAsync(ClientVersion clientVersion, int discriminator, bool enterOnStage)
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();

        // The character information carries the experience of the next level and the maximum attack speed.
        configuration.MaximumLevel = 400;
        Mock.Get(configuration).Setup(c => c.Attributes).Returns(new List<AttributeDefinition>());
        var stage = new BasicModel.GameMapDefinition { Number = DevilSquareMapNumber, Discriminator = discriminator, TerrainData = new byte[ushort.MaxValue + 3] };
        configuration.Maps.Add(stage);

        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugIn<IViewPlugIn, MapChangePlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, MapChangeExtendedPlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, TeleportPlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, TeleportExtendedPlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, UpdateCharacterStatsExtendedPlugIn>();

        var mapInitializer = new MapInitializer(configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = configuration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
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
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        var character = PlayerTestHelper.CreateCharacter(gameServerContext);

        // The fruit points of the character information are looked up by the level, which starts at 1.
        character.Attributes.First(a => a.Definition == Stats.Level).Value = 1;
        if (enterOnStage)
        {
            character.CurrentMap = stage;
        }

        var output = new MemoryStream();
        var connection = new Mock<IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(new PrefilledPipeWriter(output));
        connection.SetupGet(c => c.OutputLock).Returns(new AsyncLock());
        var player = new RemotePlayer(gameServerContext, connection.Object, clientVersion);
        await PlayerTestHelper.EnterWorldAsync(player, character).ConfigureAwait(false);
        return (player, output, stage);
    }

    /// <summary>
    /// Splits what was written to the connection into its packets, by the length in their headers.
    /// </summary>
    private static List<byte[]> ReadPackets(MemoryStream output)
    {
        var data = output.ToArray();
        var packets = new List<byte[]>();
        var offset = 0;
        while (offset < data.Length)
        {
            var length = data.AsSpan(offset).GetPacketSize();
            if (length <= 0)
            {
                Assert.Fail($"No packet header at offset {offset}.");
            }

            packets.Add(data[offset..(offset + length)]);
            offset += length;
        }

        return packets;
    }

    /// <summary>
    /// A pipe writer which hands out memory filled with <see cref="StaleByte"/>, and writes what was
    /// advanced to a stream.
    /// </summary>
    private sealed class PrefilledPipeWriter : PipeWriter
    {
        private const int MinimumSize = 256;

        private readonly Stream _stream;

        private byte[] _buffer = new byte[MinimumSize];

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefilledPipeWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream which gets the advanced bytes.</param>
        public PrefilledPipeWriter(Stream stream) => this._stream = stream;

        /// <inheritdoc/>
        public override void Advance(int bytes) => this._stream.Write(this._buffer, 0, bytes);

        /// <inheritdoc/>
        public override Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (this._buffer.Length < sizeHint)
            {
                this._buffer = new byte[sizeHint];
            }

            Array.Fill(this._buffer, StaleByte);
            return this._buffer;
        }

        /// <inheritdoc/>
        public override Span<byte> GetSpan(int sizeHint = 0) => this.GetMemory(sizeHint).Span;

        /// <inheritdoc/>
        public override void CancelPendingFlush()
        {
            // Nothing to cancel: the flush completes at once, because the bytes are already in the stream.
        }

        /// <inheritdoc/>
        public override void Complete(Exception? exception = null)
        {
            // Nothing to complete: the tests read the stream after the player was used.
        }

        /// <inheritdoc/>
        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(new FlushResult(false, false));
        }
    }
}
