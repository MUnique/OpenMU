// <copyright file="CastleSiegeStatusTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Immutable;
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
using CastleSiegeStatusResponse = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeStatusResponse;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;
using WireState = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeState;

/// <summary>
/// Tests the Castle Siege status request of the guardsman window.
/// </summary>
[TestFixture]
public class CastleSiegeStatusTests
{
    private const uint RuntimeGuildId = 7;
    private const string OwnerGuildName = "Lords";
    private const string OwnerGuildMasterName = "King";

    /// <summary>
    /// The server time zone used by the tests. A non-zero offset proves that times are sent in server time.
    /// </summary>
    private static readonly TimeZoneInfo ServerTimeZone = TimeZoneInfo.CreateCustomTimeZone("Test+02", TimeSpan.FromHours(2), "Test+02", "Test+02");

    /// <summary>
    /// Monday, 2026-08-03 14:00 in server time.
    /// </summary>
    private static readonly DateTime UtcNow = new(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Verifies the handler's protocol subcode.
    /// </summary>
    [Test]
    public void HandlerUsesExpectedSubcode()
    {
        Assert.That(new CastleSiegeStatusHandlerPlugIn().Key, Is.EqualTo(0x00));
        Assert.That(CastleSiegeStatusRequest.SubCode, Is.EqualTo(0x00));
    }

    /// <summary>
    /// Verifies that the status contains the current state period, the next battle and the remaining time in server time.
    /// </summary>
    [Test]
    public async ValueTask StatusContainsScheduleInServerTimeAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);

        await new CastleSiegeStatusAction().ShowStatusAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        var status = fixture.ReceivedStatus;
        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status!.State, Is.EqualTo(CastleSiegeState.RegisterGuild));
            Assert.That(status.StateStart, Is.EqualTo(new DateTime(2026, 8, 3, 0, 0, 0)));
            Assert.That(status.StateEnd, Is.EqualTo(new DateTime(2026, 8, 4, 0, 0, 0)));
            Assert.That(status.SiegeStart, Is.EqualTo(new DateTime(2026, 8, 8, 20, 0, 0)));
            Assert.That(status.RemainingTime, Is.EqualTo(TimeSpan.FromHours(10)));
            Assert.That(status.OwnerGuildName, Is.Empty);
            Assert.That(status.OwnerGuildMasterName, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies that the castle owner guild and its master are resolved, even when the guild is not selected for the battle.
    /// </summary>
    [Test]
    public async ValueTask StatusContainsCastleOwnerAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = true;
        fixture.Context.SiegeData.OwnerGuildId = fixture.PersistentGuildId;

        await new CastleSiegeStatusAction().ShowStatusAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.ReceivedStatus!.OwnerGuildName, Is.EqualTo(OwnerGuildName));
            Assert.That(fixture.ReceivedStatus.OwnerGuildMasterName, Is.EqualTo(OwnerGuildMasterName));
        });
    }

    /// <summary>
    /// Verifies that an unoccupied castle has no owner, even if a previous owner id is still stored.
    /// </summary>
    [Test]
    public async ValueTask UnoccupiedCastleHasNoOwnerAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = false;
        fixture.Context.SiegeData.OwnerGuildId = fixture.PersistentGuildId;

        await new CastleSiegeStatusAction().ShowStatusAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        Assert.That(fixture.ReceivedStatus!.OwnerGuildName, Is.Empty);
        fixture.GuildServer.Verify(server => server.GetGuildIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the battle start is the start of the running battle while it is in progress.
    /// </summary>
    [Test]
    public async ValueTask SiegeStartIsCurrentBattleDuringBattleAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var battleStartUtc = new DateTime(2026, 8, 8, 18, 0, 0, DateTimeKind.Utc);
        fixture.Context.SetPeriod(fixture.Context.Schedule.CreatePeriod(CastleSiegeState.Start, battleStartUtc));
        fixture.Clock.UtcNow = battleStartUtc.AddMinutes(30);

        await new CastleSiegeStatusAction().ShowStatusAsync(fixture.Player, fixture.Context).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.ReceivedStatus!.State, Is.EqualTo(CastleSiegeState.Start));
            Assert.That(fixture.ReceivedStatus.SiegeStart, Is.EqualTo(new DateTime(2026, 8, 8, 20, 0, 0)));
            Assert.That(fixture.ReceivedStatus.RemainingTime, Is.EqualTo(TimeSpan.FromMinutes(90)));
        });
    }

    /// <summary>
    /// Verifies that no status is available when Castle Siege is disabled or not initialized.
    /// </summary>
    [Test]
    public async ValueTask NoStatusWithoutActiveContextAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var action = new CastleSiegeStatusAction();

        await action.ShowStatusAsync(fixture.Player, null).ConfigureAwait(false);
        Assert.That(fixture.ReceivedStatus, Is.Null);

        fixture.CastleSiegeConfiguration.Enabled = false;
        await action.ShowStatusAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Assert.That(fixture.ReceivedStatus, Is.Null);
        fixture.StatusView.Verify(view => view.ShowStatusAsync(null), Times.Exactly(2));
    }

    /// <summary>
    /// Verifies the serialization of the status response, which the client reads as <c>PMSG_ANS_CASTLESIEGESTATE</c>.
    /// </summary>
    [Test]
    public async ValueTask SerializeStatusAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var status = new CastleSiegeStatus(
            CastleSiegeState.RegisterMark,
            new DateTime(2026, 8, 4, 0, 0, 0),
            new DateTime(2026, 8, 5, 6, 30, 0),
            new DateTime(2026, 8, 8, 20, 15, 0),
            OwnerGuildName,
            OwnerGuildMasterName,
            TimeSpan.FromSeconds(3600.2));

        await new CastleSiegeStatusPlugIn(player).ShowStatusAsync(status).ConfigureAwait(false);

        var data = output.ToArray();
        var packet = (CastleSiegeStatusResponse)data.AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(CastleSiegeStatusResponse.Length));
            Assert.That(packet.Result, Is.EqualTo(1));
            Assert.That(packet.State, Is.EqualTo(WireState.RegisterMark));
            Assert.That(packet.StartYear, Is.EqualTo(2026));
            Assert.That(packet.StartMonth, Is.EqualTo(8));
            Assert.That(packet.StartDay, Is.EqualTo(4));
            Assert.That(packet.EndDay, Is.EqualTo(5));
            Assert.That(packet.EndHour, Is.EqualTo(6));
            Assert.That(packet.EndMinute, Is.EqualTo(30));
            Assert.That(packet.SiegeStartDay, Is.EqualTo(8));
            Assert.That(packet.SiegeStartHour, Is.EqualTo(20));
            Assert.That(packet.SiegeStartMinute, Is.EqualTo(15));
            Assert.That(packet.GuildName, Is.EqualTo(OwnerGuildName));
            Assert.That(packet.GuildMasterName, Is.EqualTo(OwnerGuildMasterName));
            Assert.That(packet.RemainingTime, Is.EqualTo(3601));

            // The client reads the year big-endian (high byte first) and the remaining seconds from the most significant byte.
            Assert.That(data[6..8], Is.EqualTo(new byte[] { 0x07, 0xEA }));
            Assert.That(data[42..46], Is.EqualTo(new byte[] { 0x00, 0x00, 0x0E, 0x11 }));
        });
    }

    /// <summary>
    /// Verifies that an unavailable status is reported with the failure result, which the client shows as an error message.
    /// </summary>
    [Test]
    public async ValueTask SerializeUnavailableStatusAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeStatusPlugIn(player).ShowStatusAsync(null).ConfigureAwait(false);

        var packet = (CastleSiegeStatusResponse)output.ToArray().AsMemory();
        Assert.That(packet.Result, Is.Zero);
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync()
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        BasicModel.GameConfiguration gameConfiguration;
        BasicModel.CastleSiegeConfiguration castleSiegeConfiguration;
        Guid persistentGuildId;
        using (var persistenceContext = persistenceContextProvider.CreateNewContext())
        {
            gameConfiguration = persistenceContext.CreateNew<BasicModel.GameConfiguration>();
            gameConfiguration.Maps.Add(new BasicModel.GameMapDefinition());
            castleSiegeConfiguration = persistenceContext.CreateNew<BasicModel.CastleSiegeConfiguration>();
            castleSiegeConfiguration.Enabled = true;
            AddState(castleSiegeConfiguration, CastleSiegeState.RegisterGuild, DayOfWeek.Monday, 0);
            AddState(castleSiegeConfiguration, CastleSiegeState.RegisterMark, DayOfWeek.Tuesday, 0);
            AddState(castleSiegeConfiguration, CastleSiegeState.Start, DayOfWeek.Saturday, 20);
            AddState(castleSiegeConfiguration, CastleSiegeState.End, DayOfWeek.Saturday, 22);
            gameConfiguration.CastleSiegeConfiguration = castleSiegeConfiguration;
            persistenceContext.CreateNew<CastleSiegeData>();
            var persistentGuild = persistenceContext.CreateNew<MUnique.OpenMU.DataModel.Entities.Guild>();
            persistentGuild.Name = OwnerGuildName;
            persistentGuildId = persistentGuild.Id;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var guildServer = new Mock<IGuildServer>();
        guildServer
            .Setup(server => server.GetGuildIdAsync(persistentGuildId))
            .Returns(new ValueTask<uint>(RuntimeGuildId));
        guildServer
            .Setup(server => server.GetGuildAsync(RuntimeGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = OwnerGuildName }));
        guildServer
            .Setup(server => server.GetGuildListAsync(RuntimeGuildId))
            .Returns(new ValueTask<IImmutableList<GuildListEntry>>(ImmutableList.Create(
                new GuildListEntry { PlayerName = "Member", PlayerPosition = GuildPosition.NormalMember },
                new GuildListEntry { PlayerName = OwnerGuildMasterName, PlayerPosition = GuildPosition.GuildMaster })));

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
            new ConfigurationChangeMediator())
        {
            ServerTimeZone = ServerTimeZone,
        };
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        var player = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        var clock = new ManualTimeProvider { UtcNow = UtcNow };
        var castleSiegeContext = new CastleSiegeContext(gameServerContext, castleSiegeConfiguration, clock);
        await castleSiegeContext.InitializeAsync(UtcNow).ConfigureAwait(false);
        var statusView = Mock.Get(player.ViewPlugIns.GetPlugIn<ICastleSiegeStatusPlugIn>()!);
        return new(castleSiegeConfiguration, castleSiegeContext, player, statusView, clock, persistentGuildId, guildServer);
    }

    private static void AddState(BasicModel.CastleSiegeConfiguration configuration, CastleSiegeState state, DayOfWeek dayOfWeek, byte hour)
    {
        configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
        {
            State = state,
            DayOfWeek = dayOfWeek,
            Hour = hour,
        });
    }

    private sealed record TestFixture(
        CastleSiegeConfiguration CastleSiegeConfiguration,
        CastleSiegeContext Context,
        Player Player,
        Mock<ICastleSiegeStatusPlugIn> StatusView,
        ManualTimeProvider Clock,
        Guid PersistentGuildId,
        Mock<IGuildServer> GuildServer)
    {
        public CastleSiegeStatus? ReceivedStatus => this.StatusView.Invocations
            .LastOrDefault(invocation => invocation.Method.Name == nameof(ICastleSiegeStatusPlugIn.ShowStatusAsync))
            ?.Arguments[0] as CastleSiegeStatus;
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        public DateTime UtcNow { get; set; }

        public override DateTimeOffset GetUtcNow() => new(this.UtcNow, TimeSpan.Zero);
    }
}
