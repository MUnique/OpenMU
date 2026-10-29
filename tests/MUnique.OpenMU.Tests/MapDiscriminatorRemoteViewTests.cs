// <copyright file="MapDiscriminatorRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the extended client (MuMain) gets the discriminator of its map, which tells maps with
/// the same number apart (the stages of Devil Square), and that other clients get the original packets.
/// </summary>
[TestFixture]
public class MapDiscriminatorRemoteViewTests
{
    private const short DevilSquareMapNumber = 9;
    private const byte MapChangedCode = 0x1C;

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
        var (player, output, stage) = await CreatePlayerInWorldAsync(ExtendedClient, discriminator).ConfigureAwait(false);

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
    /// Other clients get the original map change packet, which doesn't carry the discriminator.
    /// </summary>
    [Test]
    public async Task OtherClientsGetTheOriginalMapChangedAsync()
    {
        var (player, output, stage) = await CreatePlayerInWorldAsync(Season6Client, 6).ConfigureAwait(false);

        await player.WarpToAsync(new ExitGate { Map = stage, X1 = 130, X2 = 130, Y1 = 90, Y2 = 90 }).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeFailedAsync()).ConfigureAwait(false);

        var packets = ReadPackets(output).Where(p => p[2] == MapChangedCode).ToList();
        Assert.That(packets.Select(p => p.Length), Is.EqualTo(new[] { MapChangedRef.Length, MapChangedRef.Length, MapChangedRef.Length }));
        Assert.That(packets.Select(p => ((MapChanged)p.AsMemory()).MapNumber), Is.All.EqualTo(DevilSquareMapNumber));
    }

    /// <summary>
    /// Creates a remote player with the plugins of the original and the extended client, which entered the world
    /// on map 0, and a map with the number of Devil Square and the specified discriminator.
    /// </summary>
    private static async ValueTask<(RemotePlayer Player, MemoryStream Output, GameMapDefinition Stage)> CreatePlayerInWorldAsync(ClientVersion clientVersion, int discriminator)
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();

        var stage = new BasicModel.GameMapDefinition { Number = DevilSquareMapNumber, Discriminator = discriminator, TerrainData = new byte[ushort.MaxValue + 3] };
        configuration.Maps.Add(stage);

        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugIn<IViewPlugIn, MapChangePlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, MapChangeExtendedPlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, TeleportPlugIn>();
        plugInManager.RegisterPlugIn<IViewPlugIn, TeleportExtendedPlugIn>();

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

        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(gameServerContext, clientVersion);
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
        for (var offset = 0; offset < data.Length;)
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
}
