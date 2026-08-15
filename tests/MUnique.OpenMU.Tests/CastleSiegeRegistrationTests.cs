// <copyright file="CastleSiegeRegistrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Immutable;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;

/// <summary>
/// Tests Castle Siege guild and Sign of Lord registration.
/// </summary>
[TestFixture]
public class CastleSiegeRegistrationTests
{
    private const uint RuntimeGuildId = 42;
    private const string GuildName = "TestGuild";

    /// <summary>
    /// Verifies the protocol result values documented by the Castle Siege registration issue.
    /// </summary>
    [Test]
    public void RegistrationResultValuesMatchProtocol()
    {
        Assert.Multiple(() =>
        {
            Assert.That((byte)CastleSiegeRegistrationResult.Failed, Is.Zero);
            Assert.That((byte)CastleSiegeRegistrationResult.Success, Is.EqualTo(1));
            Assert.That((byte)CastleSiegeRegistrationResult.AlreadyRegistered, Is.EqualTo(2));
            Assert.That((byte)CastleSiegeRegistrationResult.IsDefender, Is.EqualTo(3));
            Assert.That((byte)CastleSiegeRegistrationResult.InvalidGuild, Is.EqualTo(4));
            Assert.That((byte)CastleSiegeRegistrationResult.LevelInsufficient, Is.EqualTo(5));
            Assert.That((byte)CastleSiegeRegistrationResult.NoGuild, Is.EqualTo(6));
            Assert.That((byte)CastleSiegeRegistrationResult.NotRegistrationPeriod, Is.EqualTo(7));
            Assert.That((byte)CastleSiegeRegistrationResult.NotEnoughMembers, Is.EqualTo(8));
            Assert.That((byte)CastleSiegeMarkRegistrationResult.Failed, Is.Zero);
            Assert.That((byte)CastleSiegeMarkRegistrationResult.Success, Is.EqualTo(1));
            Assert.That((byte)CastleSiegeMarkRegistrationResult.GuildNotRegistered, Is.EqualTo(2));
            Assert.That((byte)CastleSiegeMarkRegistrationResult.IncorrectItem, Is.EqualTo(3));
        });
    }

    /// <summary>
    /// Verifies registration validation, persistence, and duplicate detection.
    /// </summary>
    [Test]
    public async ValueTask RegistrationValidatesAndPersistsAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var view = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeRegistrationResultPlugIn>()!;
        var action = new CastleSiegeRegisterGuildAction();

        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.NoGuild, string.Empty),
            Times.Once);

        fixture.Player.GuildStatus = new GuildMemberStatus(RuntimeGuildId, GuildPosition.NormalMember);
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.InvalidGuild, string.Empty),
            Times.Once);

        fixture.Player.GuildStatus = new GuildMemberStatus(RuntimeGuildId, GuildPosition.GuildMaster);
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.LevelInsufficient, GuildName),
            Times.Once);

        fixture.Player.Attributes![Stats.Level] = 400;
        fixture.GuildMembers.Clear();
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.NotEnoughMembers, GuildName),
            Times.Once);

        fixture.GuildMembers.AddRange([new(), new()]);
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.Success, GuildName),
            Times.Once);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.AlreadyRegistered, GuildName),
            Times.Once);

        Assert.That(fixture.Context.RegisteredGuilds, Contains.Key(fixture.PersistentGuildId));
        Assert.That(fixture.Context.RegisteredGuilds[fixture.PersistentGuildId].RegistrationOrder, Is.EqualTo(1));

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            fixture.GameConfiguration);
        var registration = (await persistenceContext.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false)).Single();
        Assert.That(registration.GuildId, Is.EqualTo(fixture.PersistentGuildId));
        Assert.That(registration.GuildName, Is.EqualTo(GuildName));

        var restartedContext = new CastleSiegeContext(fixture.GameServerContext, fixture.CastleSiegeConfiguration);
        await restartedContext.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);
        Assert.That(restartedContext.RegisteredGuilds, Contains.Key(fixture.PersistentGuildId));
    }

    /// <summary>
    /// Verifies the registration-period and defender validation results.
    /// </summary>
    [Test]
    public async ValueTask RegistrationRejectsClosedPeriodAndDefenderAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var view = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeRegistrationResultPlugIn>()!;
        var action = new CastleSiegeRegisterGuildAction();

        fixture.CastleSiegeConfiguration.Enabled = false;
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.Failed, string.Empty),
            Times.Once);
        fixture.CastleSiegeConfiguration.Enabled = true;

        fixture.Player.GuildStatus = new GuildMemberStatus(RuntimeGuildId, GuildPosition.GuildMaster);
        fixture.Player.Attributes![Stats.Level] = 400;

        fixture.Context.SetPeriod(fixture.Context.Schedule.GetCurrentPeriod(new DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc)));
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.NotRegistrationPeriod, string.Empty),
            Times.Once);

        fixture.Context.SetPeriod(fixture.Context.Schedule.GetCurrentPeriod(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)));
        fixture.Context.SiegeData.OwnerGuildId = fixture.PersistentGuildId;
        await action.RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.IsDefender, GuildName),
            Times.Once);
    }

    /// <summary>
    /// Verifies that unregistration removes both runtime and persistent registration state.
    /// </summary>
    [Test]
    public async ValueTask UnregistrationRemovesPersistentRegistrationAsync()
    {
        var fixture = await CreateRegisteredFixtureAsync().ConfigureAwait(false);
        var view = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeRegistrationResultPlugIn>()!;

        await new CastleSiegeUnregisterGuildAction().UnregisterAsync(fixture.Player, fixture.Context, true).ConfigureAwait(false);

        Mock.Get(view).Verify(
            plugIn => plugIn.ShowUnregistrationResultAsync(
                CastleSiegeUnregistrationResult.Success,
                true,
                GuildName),
            Times.Once);
        Assert.That(fixture.Context.RegisteredGuilds, Is.Empty);
        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            fixture.GameConfiguration);
        Assert.That(await persistenceContext.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false), Is.Empty);
    }

    /// <summary>
    /// Verifies Sign of Lord validation, consumption, mark persistence, and registration-state queries.
    /// </summary>
    [Test]
    public async ValueTask MarkRegistrationConsumesValidSignOfLordAndPersistsCountAsync()
    {
        const byte itemSlot = 20;
        var fixture = await CreateRegisteredFixtureAsync().ConfigureAwait(false);
        fixture.Context.SetPeriod(fixture.Context.Schedule.GetCurrentPeriod(new DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc)));
        var markView = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeMarkRegistrationResultPlugIn>()!;
        var item = fixture.Player.PersistenceContext.CreateNew<Item>();
        item.Definition = fixture.CastleSiegeConfiguration.SignOfLordItemDefinition;
        item.Level = 2;
        await fixture.Player.Inventory!.AddItemAsync(itemSlot, item).ConfigureAwait(false);

        var action = new CastleSiegeRegisterMarkAction();
        await action.RegisterMarkAsync(fixture.Player, fixture.Context, itemSlot).ConfigureAwait(false);
        Mock.Get(markView).Verify(
            plugIn => plugIn.ShowMarkRegistrationResultAsync(CastleSiegeMarkRegistrationResult.IncorrectItem, GuildName, 0),
            Times.Once);
        Assert.That(fixture.Player.Inventory.GetItem(itemSlot), Is.SameAs(item));

        item.Level = fixture.CastleSiegeConfiguration.SignOfLordItemLevel;
        await action.RegisterMarkAsync(fixture.Player, fixture.Context, itemSlot).ConfigureAwait(false);
        Mock.Get(markView).Verify(
            plugIn => plugIn.ShowMarkRegistrationResultAsync(CastleSiegeMarkRegistrationResult.Success, GuildName, 1),
            Times.Once);
        Assert.That(fixture.Player.Inventory.GetItem(itemSlot), Is.Null);
        Assert.That(fixture.Context.RegisteredGuilds[fixture.PersistentGuildId].Marks, Is.EqualTo(1));

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            fixture.GameConfiguration);
        var registration = (await persistenceContext.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false)).Single();
        Assert.That(registration.Marks, Is.EqualTo(1));

        var stateView = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeRegistrationStatePlugIn>()!;
        await new CastleSiegeRegistrationStateAction().ShowStateAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(stateView).Verify(
            plugIn => plugIn.ShowRegistrationStateAsync(
                CastleSiegeRegistrationStateResult.Registered,
                GuildName,
                1,
                false,
                1),
            Times.Once);
    }

    /// <summary>
    /// Verifies that a Sign of Lord is preserved if the persistent guild registration disappeared.
    /// </summary>
    [Test]
    public async ValueTask MarkRegistrationPreservesItemWhenRegistrationDisappearedAsync()
    {
        const byte itemSlot = 20;
        var fixture = await CreateRegisteredFixtureAsync().ConfigureAwait(false);
        fixture.Context.SetPeriod(fixture.Context.Schedule.GetCurrentPeriod(new DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc)));
        var cachedRegistration = fixture.Context.RegisteredGuilds[fixture.PersistentGuildId];
        using (var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
                   typeof(CastleSiegeGuildRegistration),
                   false,
                   fixture.GameConfiguration))
        {
            var persistentRegistration = await persistenceContext.GetByIdAsync<CastleSiegeGuildRegistration>(cachedRegistration.Id).ConfigureAwait(false);
            Assert.That(persistentRegistration, Is.Not.Null);
            await persistenceContext.DeleteAsync(persistentRegistration!).ConfigureAwait(false);
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var signOfLord = fixture.Player.PersistenceContext.CreateNew<Item>();
        signOfLord.Definition = fixture.CastleSiegeConfiguration.SignOfLordItemDefinition;
        signOfLord.Level = fixture.CastleSiegeConfiguration.SignOfLordItemLevel;
        await fixture.Player.Inventory!.AddItemAsync(itemSlot, signOfLord).ConfigureAwait(false);

        await new CastleSiegeRegisterMarkAction().RegisterMarkAsync(fixture.Player, fixture.Context, itemSlot).ConfigureAwait(false);

        var view = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeMarkRegistrationResultPlugIn>()!;
        Mock.Get(view).Verify(
            plugIn => plugIn.ShowMarkRegistrationResultAsync(
                CastleSiegeMarkRegistrationResult.GuildNotRegistered,
                GuildName,
                0),
            Times.Once);
        Assert.That(fixture.Player.Inventory.GetItem(itemSlot), Is.SameAs(signOfLord));
        Assert.That(fixture.Context.RegisteredGuilds, Does.Not.ContainKey(fixture.PersistentGuildId));
    }

    /// <summary>
    /// Verifies that alliance members can query, but cannot mutate, the alliance master's registration.
    /// </summary>
    [Test]
    public async ValueTask AllianceMemberUsesOfflineMasterRegistrationForQueriesOnlyAsync()
    {
        const uint allianceMemberGuildId = 43;
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var allianceMaster = new RuntimeGuild { Name = GuildName };
        allianceMaster.AllianceGuild = allianceMaster;
        fixture.GuildServer
            .Setup(server => server.GetGuildAsync(RuntimeGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(allianceMaster));
        fixture.Player.GuildStatus = new GuildMemberStatus(RuntimeGuildId, GuildPosition.GuildMaster);
        fixture.Player.Attributes![Stats.Level] = 400;

        var registrationView = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeRegistrationResultPlugIn>()!;
        await new CastleSiegeRegisterGuildAction().RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(registrationView).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.Success, GuildName),
            Times.Once);

        var allianceMember = new RuntimeGuild { Name = "Member", AllianceGuild = allianceMaster };
        fixture.GuildServer
            .Setup(server => server.GetGuildAsync(allianceMemberGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(allianceMember));
        fixture.GuildServer
            .Setup(server => server.GetPersistentAllianceMasterGuildIdAsync(allianceMemberGuildId))
            .Returns(new ValueTask<Guid?>(fixture.PersistentGuildId));
        fixture.GuildServer
            .Setup(server => server.IsAllianceMasterAsync(allianceMemberGuildId))
            .Returns(new ValueTask<bool>(false));
        fixture.Player.GuildStatus = new GuildMemberStatus(allianceMemberGuildId, GuildPosition.GuildMaster);

        await new CastleSiegeRegisterGuildAction().RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(registrationView).Verify(
            plugIn => plugIn.ShowRegistrationResultAsync(CastleSiegeRegistrationResult.InvalidGuild, string.Empty),
            Times.Once);

        fixture.GuildServer
            .Setup(server => server.GetGuildAsync(RuntimeGuildId))
            .Returns(new ValueTask<RuntimeGuild?>((RuntimeGuild?)null));
        var stateView = fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeRegistrationStatePlugIn>()!;
        await new CastleSiegeRegistrationStateAction().ShowStateAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        Mock.Get(stateView).Verify(
            plugIn => plugIn.ShowRegistrationStateAsync(
                CastleSiegeRegistrationStateResult.Registered,
                GuildName,
                0,
                false,
                1),
            Times.Once);
        fixture.GuildServer.Verify(server => server.GetGuildIdByNameAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies the four client request subcodes handled by this issue.
    /// </summary>
    [Test]
    public void RequestHandlersUseExpectedSubcodes()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new CastleSiegeRegistrationHandlerPlugIn().Key, Is.EqualTo(0x01));
            Assert.That(new CastleSiegeUnregisterHandlerPlugIn().Key, Is.EqualTo(0x02));
            Assert.That(new CastleSiegeRegistrationStateHandlerPlugIn().Key, Is.EqualTo(0x03));
            Assert.That(new CastleSiegeMarkRegistrationHandlerPlugIn().Key, Is.EqualTo(0x04));
        });
    }

    /// <summary>
    /// Verifies that the zero-based backpack-grid index sent by MuMain is translated past the equipment slots.
    /// </summary>
    [Test]
    public void MarkRegistrationHandlerTranslatesClientBackpackIndex()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CastleSiegeMarkRegistrationHandlerPlugIn.TryGetInventorySlot(8, out var inventorySlot), Is.True);
            Assert.That(inventorySlot, Is.EqualTo(20));
            Assert.That(CastleSiegeMarkRegistrationHandlerPlugIn.TryGetInventorySlot(byte.MaxValue, out _), Is.False);
        });
    }

    /// <summary>
    /// Verifies that a truncated mark-registration packet is ignored.
    /// </summary>
    [Test]
    public async ValueTask MarkRegistrationHandlerIgnoresTruncatedPacketAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var packet = new byte[CastleSiegeMarkRegistration.Length - 1];

        await new CastleSiegeMarkRegistrationHandlerPlugIn()
            .HandlePacketAsync(fixture.Player, packet)
            .ConfigureAwait(false);
    }

    private static async ValueTask<TestFixture> CreateRegisteredFixtureAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Player.GuildStatus = new GuildMemberStatus(RuntimeGuildId, GuildPosition.GuildMaster);
        fixture.Player.Attributes![Stats.Level] = 400;
        await new CastleSiegeRegisterGuildAction().RegisterAsync(fixture.Player, fixture.Context).ConfigureAwait(false);
        return fixture;
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
            castleSiegeConfiguration.RegisterMinLevel = 200;
            castleSiegeConfiguration.RegisterMinMembers = 2;
            var signOfLordDefinition = persistenceContext.CreateNew<BasicModel.ItemDefinition>();
            signOfLordDefinition.Name = "Rena";
            signOfLordDefinition.Group = 14;
            signOfLordDefinition.Number = 21;
            signOfLordDefinition.MaximumItemLevel = 3;
            signOfLordDefinition.Width = 1;
            signOfLordDefinition.Height = 1;
            gameConfiguration.Items.Add(signOfLordDefinition);
            castleSiegeConfiguration.SignOfLordItemDefinition = signOfLordDefinition;
            castleSiegeConfiguration.SignOfLordItemLevel = 3;
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
            var persistentGuild = persistenceContext.CreateNew<MUnique.OpenMU.DataModel.Entities.Guild>();
            persistentGuild.Name = GuildName;
            persistentGuildId = persistentGuild.Id;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var guildMembers = new List<GuildListEntry> { new(), new() };
        var guildServer = new Mock<IGuildServer>();
        guildServer
            .Setup(server => server.GetGuildAsync(RuntimeGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = GuildName }));
        guildServer
            .Setup(server => server.GetGuildListAsync(RuntimeGuildId))
            .Returns(() => new ValueTask<IImmutableList<GuildListEntry>>(guildMembers.ToImmutableList()));
        guildServer
            .Setup(server => server.IsAllianceMasterAsync(RuntimeGuildId))
            .Returns(new ValueTask<bool>(true));
        guildServer
            .Setup(server => server.GetGuildIdByNameAsync(GuildName))
            .Returns(new ValueTask<uint>(RuntimeGuildId));
        guildServer
            .Setup(server => server.GetPersistentGuildIdAsync(RuntimeGuildId))
            .Returns(new ValueTask<Guid?>(persistentGuildId));
        guildServer
            .Setup(server => server.GetPersistentAllianceMasterGuildIdAsync(RuntimeGuildId))
            .Returns(new ValueTask<Guid?>(persistentGuildId));

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
        return new(
            persistenceContextProvider,
            gameConfiguration,
            castleSiegeConfiguration,
            gameServerContext,
            castleSiegeContext,
            player,
            persistentGuildId,
            guildMembers,
            guildServer);
    }

    private sealed record TestFixture(
        InMemoryPersistenceContextProvider PersistenceContextProvider,
        GameConfiguration GameConfiguration,
        CastleSiegeConfiguration CastleSiegeConfiguration,
        GameServerContext GameServerContext,
        CastleSiegeContext Context,
        Player Player,
        Guid PersistentGuildId,
        List<GuildListEntry> GuildMembers,
        Mock<IGuildServer> GuildServer);
}
