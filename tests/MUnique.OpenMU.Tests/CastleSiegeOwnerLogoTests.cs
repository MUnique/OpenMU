// <copyright file="CastleSiegeOwnerLogoTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using CastleSiegeOwnerLogo = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeOwnerLogo;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;

/// <summary>
/// Tests the request for the guild logo of the castle owner.
/// </summary>
[TestFixture]
public class CastleSiegeOwnerLogoTests
{
    private const uint RuntimeGuildId = 7;

    /// <summary>
    /// Verifies the handler's packet group and subcode.
    /// </summary>
    [Test]
    public void HandlerUsesExpectedSubcode()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CastleSiegeHuntZoneGroupHandlerPlugIn.GroupKey, Is.EqualTo(0xB9));
            Assert.That(new CastleSiegeOwnerLogoHandlerPlugIn().Key, Is.EqualTo(0x02));
        });
    }

    /// <summary>
    /// Verifies that the logo of the castle owner guild is sent.
    /// </summary>
    [Test]
    public async ValueTask SendsLogoOfOwnerAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = true;
        fixture.Context.SiegeData.OwnerGuildId = fixture.PersistentGuildId;

        await new CastleSiegeOwnerLogoAction().ShowOwnerLogoAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        Assert.That(fixture.ReceivedLogo, Is.EqualTo(fixture.Logo));
    }

    /// <summary>
    /// Verifies that an empty logo is sent for an unoccupied castle, even if a previous owner id is still stored.
    /// </summary>
    [Test]
    public async ValueTask SendsEmptyLogoForUnoccupiedCastleAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = false;
        fixture.Context.SiegeData.OwnerGuildId = fixture.PersistentGuildId;

        await new CastleSiegeOwnerLogoAction().ShowOwnerLogoAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        Assert.That(fixture.ReceivedLogo, Is.Empty);
        fixture.GuildServer.Verify(server => server.GetGuildIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    /// <summary>
    /// Verifies that an empty logo is sent when the owner guild can't be found anymore, e.g. because it was disbanded.
    /// </summary>
    [Test]
    public async ValueTask SendsEmptyLogoForUnknownOwnerAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = true;
        fixture.Context.SiegeData.OwnerGuildId = Guid.NewGuid();

        await new CastleSiegeOwnerLogoAction().ShowOwnerLogoAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        Assert.That(fixture.ReceivedLogo, Is.Empty);
    }

    /// <summary>
    /// Verifies that an empty logo is sent when Castle Siege is disabled or not available.
    /// </summary>
    [Test]
    public async ValueTask SendsEmptyLogoWithoutActiveContextAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = true;
        fixture.Context.SiegeData.OwnerGuildId = fixture.PersistentGuildId;
        var action = new CastleSiegeOwnerLogoAction();

        await action.ShowOwnerLogoAsync(fixture.Player, null).ConfigureAwait(false);
        Assert.That(fixture.ReceivedLogo, Is.Empty);

        fixture.CastleSiegeConfiguration.Enabled = false;
        await action.ShowOwnerLogoAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Assert.That(fixture.ReceivedLogo, Is.Empty);
    }

    /// <summary>
    /// Verifies the serialization of the logo packet, which the client reads as <c>PRECEIVE_CASTLE_FLAG</c>.
    /// </summary>
    [Test]
    public async ValueTask SerializeLogoAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var logo = Enumerable.Range(1, 32).Select(value => (byte)value).ToArray();

        await new CastleSiegeOwnerLogoPlugIn(player).ShowOwnerLogoAsync(logo).ConfigureAwait(false);

        var data = output.ToArray();
        var packet = (CastleSiegeOwnerLogo)data.AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(CastleSiegeOwnerLogo.Length));
            Assert.That(data[..4], Is.EqualTo(new byte[] { 0xC1, 36, 0xB9, 0x02 }));
            Assert.That(packet.Logo.ToArray(), Is.EqualTo(logo));
        });
    }

    /// <summary>
    /// Verifies that an empty logo is sent as zeros, and that an oversized logo can't exceed the packet.
    /// </summary>
    [Test]
    public async ValueTask SerializeEmptyAndOversizedLogoAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CastleSiegeOwnerLogoPlugIn(player);

        await view.ShowOwnerLogoAsync(ReadOnlyMemory<byte>.Empty).ConfigureAwait(false);
        await view.ShowOwnerLogoAsync(Enumerable.Repeat((byte)0xFF, 40).ToArray()).ConfigureAwait(false);

        var data = output.ToArray();
        var empty = (CastleSiegeOwnerLogo)data.AsMemory(0, CastleSiegeOwnerLogo.Length);
        var oversized = (CastleSiegeOwnerLogo)data.AsMemory(CastleSiegeOwnerLogo.Length);
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(2 * CastleSiegeOwnerLogo.Length));
            Assert.That(empty.Logo.ToArray(), Is.All.Zero);
            Assert.That(oversized.Logo.ToArray(), Is.All.EqualTo(0xFF));
        });
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync()
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        BasicModel.GameConfiguration gameConfiguration;
        BasicModel.CastleSiegeConfiguration castleSiegeConfiguration;
        using (var persistenceContext = persistenceContextProvider.CreateNewContext())
        {
            gameConfiguration = persistenceContext.CreateNew<BasicModel.GameConfiguration>();
            gameConfiguration.Maps.Add(new BasicModel.GameMapDefinition());
            castleSiegeConfiguration = persistenceContext.CreateNew<BasicModel.CastleSiegeConfiguration>();
            castleSiegeConfiguration.Enabled = true;
            castleSiegeConfiguration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.RegisterGuild,
                DayOfWeek = DayOfWeek.Monday,
            });
            castleSiegeConfiguration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.RegisterMark,
                DayOfWeek = DayOfWeek.Tuesday,
            });
            gameConfiguration.CastleSiegeConfiguration = castleSiegeConfiguration;
            persistenceContext.CreateNew<CastleSiegeData>();
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var persistentGuildId = Guid.NewGuid();
        var logo = Enumerable.Range(0, 32).Select(value => (byte)(value * 7)).ToArray();
        var guildServer = new Mock<IGuildServer>();
        guildServer
            .Setup(server => server.GetGuildIdAsync(persistentGuildId))
            .Returns(new ValueTask<uint>(RuntimeGuildId));
        guildServer
            .Setup(server => server.GetGuildAsync(RuntimeGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "Lords", Logo = logo }));

        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            guildServer.Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            persistenceContextProvider,
            mapInitializer,
            NullLoggerFactory.Instance,
            new PlugInManager([], NullLoggerFactory.Instance, null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        var player = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        var castleSiegeContext = new CastleSiegeContext(gameServerContext, castleSiegeConfiguration);
        await castleSiegeContext.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);
        var logoView = Mock.Get(player.ViewPlugIns.GetPlugIn<ICastleSiegeOwnerLogoPlugIn>()!);
        return new(castleSiegeConfiguration, castleSiegeContext, player, logoView, persistentGuildId, logo, guildServer);
    }

    private sealed record TestFixture(
        CastleSiegeConfiguration CastleSiegeConfiguration,
        CastleSiegeContext Context,
        Player Player,
        Mock<ICastleSiegeOwnerLogoPlugIn> LogoView,
        Guid PersistentGuildId,
        byte[] Logo,
        Mock<IGuildServer> GuildServer)
    {
        public byte[]? ReceivedLogo => this.LogoView.Invocations
            .LastOrDefault(invocation => invocation.Method.Name == nameof(ICastleSiegeOwnerLogoPlugIn.ShowOwnerLogoAsync))
            ?.Arguments[0] is ReadOnlyMemory<byte> logo
            ? logo.ToArray()
            : null;
    }
}
