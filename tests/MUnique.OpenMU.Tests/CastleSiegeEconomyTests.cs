// <copyright file="CastleSiegeEconomyTests.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;

/// <summary>
/// Tests Castle Siege taxes, treasury operations, and Land of Trials access.
/// </summary>
[TestFixture]
public class CastleSiegeEconomyTests
{
    private const uint OwnerGuildId = 10;
    private const uint VisitorGuildId = 20;
    private const uint AllianceGuildId = 30;
    private const string OwnerGuildName = "CastleOwners";

    /// <summary>
    /// Verifies tax calculation, batched persistence, Chaos Machine scoping, and owner/alliance exemptions.
    /// </summary>
    [Test]
    public async ValueTask TaxesAreCollectedPersistedAndExemptOwnerAllianceAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var provider = new CastleSiegeTaxProvider();
        fixture.Context.SiegeData.TaxChaos = 3;
        fixture.Context.SiegeData.TaxStore = 2;
        fixture.Visitor.Money = 2_000;
        SetOpenedNpc(fixture.Visitor, NpcWindow.ChaosMachine);

        Assert.That(
            await provider.TryPayChaosCostAsync(fixture.Visitor, 1_000, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.That(
            await provider.TryPayStoreCostAsync(fixture.Visitor, 500, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Visitor.Money, Is.EqualTo(460));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(40));
        });

        fixture.Owner.Money = 1_000;
        Assert.That(
            await provider.TryPayStoreCostAsync(fixture.Owner, 100, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Owner.Money, Is.EqualTo(900));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(40));
        });

        var allianceMember = await PlayerTestHelper.CreatePlayerAsync(fixture.GameServerContext).ConfigureAwait(false);
        allianceMember.GuildStatus = new GuildMemberStatus(AllianceGuildId, GuildPosition.NormalMember);
        allianceMember.Money = 1_000;
        SetOpenedNpc(allianceMember, NpcWindow.ChaosMachine);
        Assert.That(
            await provider.TryPayChaosCostAsync(allianceMember, 100, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(allianceMember.Money, Is.EqualTo(900));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(40));
        });

        fixture.Visitor.Money = 1_000;
        SetOpenedNpc(fixture.Visitor, NpcWindow.ElphisRefinery);
        Assert.That(
            await provider.TryPayChaosCostAsync(fixture.Visitor, 100, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Visitor.Money, Is.EqualTo(900));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(40));
        });

        fixture.Visitor.Money = 10;
        SetOpenedNpc(fixture.Visitor, NpcWindow.ChaosMachine);
        Assert.That(
            await provider.TryPayChaosCostAsync(fixture.Visitor, 100, fixture.Context).ConfigureAwait(false),
            Is.False);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Visitor.Money, Is.EqualTo(10));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(40));
        });

        Assert.That(fixture.Context.IsEconomyPersistencePending, Is.True);
        var saveDueUtc = new DateTime(2026, 8, 3, 12, 0, 30, DateTimeKind.Utc);
        fixture.Context.NextEconomySaveUtc = saveDueUtc;
        await CastleSiegePlugIn
            .PersistEconomyIfDueAsync(fixture.Context, saveDueUtc.AddTicks(-1))
            .ConfigureAwait(false);
        Assert.That(fixture.Context.IsEconomyPersistencePending, Is.True);

        await CastleSiegePlugIn.PersistEconomyIfDueAsync(fixture.Context, saveDueUtc).ConfigureAwait(false);
        Assert.That(fixture.Context.IsEconomyPersistencePending, Is.False);
        using (var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
                   typeof(CastleSiegeData),
                   false,
                   fixture.GameServerContext.Configuration))
        {
            var persistedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
            Assert.That(persistedData.TributeMoney, Is.EqualTo(40));
        }
    }

    /// <summary>
    /// Verifies that a zero store-tax rate keeps the purchase hot path free of guild-server calls.
    /// </summary>
    [Test]
    public async ValueTask ZeroStoreTaxSkipsGuildLookupAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.TaxStore = 0;
        fixture.Visitor.Money = 100;

        Assert.That(
            await new CastleSiegeTaxProvider()
                .TryPayStoreCostAsync(fixture.Visitor, 25, fixture.Context)
                .ConfigureAwait(false),
            Is.True);
        Assert.That(fixture.Visitor.Money, Is.EqualTo(75));
        fixture.GuildServer.Verify(
            server => server.GetPersistentAllianceMasterGuildIdAsync(It.IsAny<uint>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that tax notifications are only sent to players who entered the world.
    /// </summary>
    [Test]
    public async ValueTask TaxBroadcastSkipsPlayersOutsideTheWorldAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        Assert.That(
            await fixture.Visitor.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false),
            Is.True);
        var ownerView = Mock.Get(fixture.Owner.ViewPlugIns.GetPlugIn<ICastleSiegeTaxChangeResultPlugIn>()!);
        var visitorView = Mock.Get(fixture.Visitor.ViewPlugIns.GetPlugIn<ICastleSiegeTaxChangeResultPlugIn>()!);

        await CastleSiegeEconomyNotifier
            .BroadcastTaxRateAsync(fixture.Context, CastleSiegeTaxType.Store, 2)
            .ConfigureAwait(false);

        ownerView.Verify(view => view.ShowTaxRateUpdateAsync(CastleSiegeTaxType.Store, 2), Times.Once);
        visitorView.Verify(view => view.ShowTaxRateUpdateAsync(It.IsAny<CastleSiegeTaxType>(), It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// Verifies owner-only rate changes, valid limits, public-access changes, and treasury withdrawals.
    /// </summary>
    [Test]
    public async ValueTask OwnerGuildMasterCanManageEconomyAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.CurrentState = CastleSiegeState.End;
        fixture.Context.SiegeData.TributeMoney = 500;
        fixture.Owner.Money = 100;
        var changeAction = new CastleSiegeTaxRateChangeAction();

        Assert.That(
            await changeAction.ChangeAsync(
                    fixture.Owner,
                    fixture.Context,
                    CastleSiegeTaxType.Store,
                    4)
                .ConfigureAwait(false),
            Is.False);
        Assert.That(
            await changeAction.ChangeAsync(
                    fixture.Owner,
                    fixture.Context,
                    CastleSiegeTaxType.Store,
                    3)
                .ConfigureAwait(false),
            Is.True);
        Assert.That(
            await changeAction.ChangeAsync(
                    fixture.Owner,
                    fixture.Context,
                    CastleSiegeTaxType.HuntZone,
                    300_001)
                .ConfigureAwait(false),
            Is.False);
        Assert.That(
            await changeAction.ChangeAsync(
                    fixture.Owner,
                    fixture.Context,
                    CastleSiegeTaxType.HuntZone,
                    300_000)
                .ConfigureAwait(false),
            Is.True);
        Assert.That(
            await new CastleSiegeHuntZoneToggleAction()
                .SetPublicAccessAsync(fixture.Owner, fixture.Context, true)
                .ConfigureAwait(false),
            Is.True);
        Assert.That(
            await new CastleSiegeTributeWithdrawAction()
                .WithdrawAsync(fixture.Owner, fixture.Context, 200)
                .ConfigureAwait(false),
            Is.True);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.SiegeData.TaxStore, Is.EqualTo(3));
            Assert.That(fixture.Context.SiegeData.TaxHunt, Is.EqualTo(300_000));
            Assert.That(fixture.Context.SiegeData.IsHuntZoneEnabled, Is.True);
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(300));
            Assert.That(fixture.Owner.Money, Is.EqualTo(300));
        });
        Mock.Get(fixture.Owner.ViewPlugIns.GetPlugIn<ICastleSiegeTaxChangeResultPlugIn>()!)
            .Verify(view => view.ShowTaxRateUpdateAsync(CastleSiegeTaxType.Store, 3), Times.Once);

        fixture.Context.CurrentState = CastleSiegeState.Start;
        Assert.That(
            await changeAction.ChangeAsync(
                    fixture.Owner,
                    fixture.Context,
                    CastleSiegeTaxType.Store,
                    2)
                .ConfigureAwait(false),
            Is.False);
        Assert.That(
            await new CastleSiegeHuntZoneToggleAction()
                .SetPublicAccessAsync(fixture.Owner, fixture.Context, false)
                .ConfigureAwait(false),
            Is.False);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.SiegeData.TaxStore, Is.EqualTo(3));
            Assert.That(fixture.Context.SiegeData.IsHuntZoneEnabled, Is.True);
        });

        fixture.Context.CurrentState = CastleSiegeState.End;

        fixture.Owner.GuildStatus = new GuildMemberStatus(OwnerGuildId, GuildPosition.NormalMember);
        Assert.That(
            await changeAction.ChangeAsync(
                    fixture.Owner,
                    fixture.Context,
                    CastleSiegeTaxType.ChaosMachine,
                    3)
                .ConfigureAwait(false),
            Is.False);
        Assert.That(
            await new CastleSiegeTributeWithdrawAction()
                .WithdrawAsync(fixture.Owner, fixture.Context, 100)
                .ConfigureAwait(false),
            Is.False);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameServerContext.Configuration);
        var persistedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        Assert.Multiple(() =>
        {
            Assert.That(persistedData.TaxStore, Is.EqualTo(3));
            Assert.That(persistedData.TaxHunt, Is.EqualTo(300_000));
            Assert.That(persistedData.IsHuntZoneEnabled, Is.True);
            Assert.That(persistedData.TributeMoney, Is.EqualTo(300));
        });
    }

    /// <summary>
    /// Verifies public Land of Trials access, configured entry fees, and owner exemption.
    /// </summary>
    [Test]
    public async ValueTask HuntZoneFeeHonorsAccessAndOwnerExemptionAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var provider = new CastleSiegeTaxProvider();
        fixture.Context.SiegeData.TaxHunt = 300_000;
        fixture.Context.SiegeData.IsHuntZoneEnabled = false;
        fixture.Visitor.Money = 400_000;
        fixture.Owner.Money = 1_000;

        Assert.That(
            await provider.TryPayHuntEntryFeeAsync(fixture.Visitor, fixture.Context).ConfigureAwait(false),
            Is.False);
        Assert.That(
            await provider.TryPayHuntEntryFeeAsync(fixture.Owner, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Visitor.Money, Is.EqualTo(400_000));
            Assert.That(fixture.Owner.Money, Is.EqualTo(1_000));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.Zero);
        });

        fixture.Context.SiegeData.IsHuntZoneEnabled = true;
        Assert.That(
            await provider.TryPayHuntEntryFeeAsync(fixture.Visitor, fixture.Context).ConfigureAwait(false),
            Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Visitor.Money, Is.EqualTo(100_000));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(300_000));
        });

        fixture.Context.SiegeData.TaxHunt = -1;
        fixture.Visitor.Money = 100;
        Assert.That(
            await provider.TryPayHuntEntryFeeAsync(fixture.Visitor, fixture.Context).ConfigureAwait(false),
            Is.False);
        Assert.Multiple(() =>
        {
            Assert.That(fixture.Visitor.Money, Is.EqualTo(100));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(300_000));
        });
    }

    /// <summary>
    /// Verifies the request identifiers required by the Castle Siege economy protocol.
    /// </summary>
    [Test]
    public void EconomyRequestHandlersUseExpectedIdentifiers()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new CastleSiegeTaxInfoHandlerPlugIn().Key, Is.EqualTo(0x08));
            Assert.That(new CastleSiegeTaxChangeHandlerPlugIn().Key, Is.EqualTo(0x09));
            Assert.That(new CastleSiegeTributeWithdrawHandlerPlugIn().Key, Is.EqualTo(0x10));
            Assert.That(new CastleSiegeHuntZoneToggleHandlerPlugIn().Key, Is.EqualTo(0x1F));
            Assert.That(CastleSiegeHuntZoneGroupHandlerPlugIn.GroupKey, Is.EqualTo(0xB9));
            Assert.That(new CastleSiegeHuntZoneEnterHandlerPlugIn().Key, Is.EqualTo(0x05));
        });
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync()
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        BasicModel.GameConfiguration gameConfiguration;
        BasicModel.CastleSiegeConfiguration castleSiegeConfiguration;
        Guid persistentOwnerGuildId;
        using (var persistenceContext = persistenceContextProvider.CreateNewContext())
        {
            gameConfiguration = persistenceContext.CreateNew<BasicModel.GameConfiguration>();
            gameConfiguration.MaximumInventoryMoney = int.MaxValue;
            var normalMap = persistenceContext.CreateNew<BasicModel.GameMapDefinition>();
            normalMap.Number = 0;
            normalMap.TerrainData = new byte[65_539];
            gameConfiguration.Maps.Add(normalMap);
            var siegeMap = persistenceContext.CreateNew<BasicModel.GameMapDefinition>();
            siegeMap.Number = 30;
            siegeMap.TerrainData = new byte[65_539];
            gameConfiguration.Maps.Add(siegeMap);

            castleSiegeConfiguration = persistenceContext.CreateNew<BasicModel.CastleSiegeConfiguration>();
            castleSiegeConfiguration.Enabled = true;
            castleSiegeConfiguration.CastleSiegeMapDefinition = siegeMap;
            castleSiegeConfiguration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.Ready,
                DayOfWeek = DayOfWeek.Monday,
            });
            castleSiegeConfiguration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.Start,
                DayOfWeek = DayOfWeek.Tuesday,
            });
            gameConfiguration.CastleSiegeConfiguration = castleSiegeConfiguration;

            var ownerGuild = persistenceContext.CreateNew<BasicModel.Guild>();
            ownerGuild.Name = OwnerGuildName;
            persistentOwnerGuildId = ownerGuild.Id;
            var siegeData = persistenceContext.CreateNew<BasicModel.CastleSiegeData>();
            siegeData.OwnerGuildId = persistentOwnerGuildId;
            siegeData.IsOccupied = true;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var runtimeOwnerGuild = new RuntimeGuild { Name = OwnerGuildName };
        var persistentVisitorGuildId = Guid.NewGuid();
        var guildServer = new Mock<IGuildServer>();
        guildServer
            .Setup(server => server.GetGuildAsync(OwnerGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(runtimeOwnerGuild));
        guildServer
            .Setup(server => server.GetGuildAsync(VisitorGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "Visitors" }));
        guildServer
            .Setup(server => server.GetGuildAsync(AllianceGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild
            {
                Name = "AllianceMember",
                AllianceGuild = runtimeOwnerGuild,
            }));
        guildServer
            .Setup(server => server.GetPersistentGuildIdAsync(OwnerGuildId))
            .Returns(new ValueTask<Guid?>(persistentOwnerGuildId));
        guildServer
            .Setup(server => server.GetPersistentAllianceMasterGuildIdAsync(OwnerGuildId))
            .Returns(new ValueTask<Guid?>(persistentOwnerGuildId));
        guildServer
            .Setup(server => server.GetPersistentAllianceMasterGuildIdAsync(VisitorGuildId))
            .Returns(new ValueTask<Guid?>(persistentVisitorGuildId));
        guildServer
            .Setup(server => server.GetPersistentAllianceMasterGuildIdAsync(AllianceGuildId))
            .Returns(new ValueTask<Guid?>(persistentOwnerGuildId));

        var mapInitializer = new MapInitializer(
            gameConfiguration,
            new NullLogger<MapInitializer>(),
            NullDropGenerator.Instance,
            null);
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

        var owner = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        owner.GuildStatus = new GuildMemberStatus(OwnerGuildId, GuildPosition.GuildMaster);
        var visitor = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        visitor.GuildStatus = new GuildMemberStatus(VisitorGuildId, GuildPosition.NormalMember);
        await gameServerContext.AddPlayerAsync(owner).ConfigureAwait(false);
        await gameServerContext.AddPlayerAsync(visitor).ConfigureAwait(false);
        var context = new CastleSiegeContext(gameServerContext, castleSiegeConfiguration);
        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        return new(
            persistenceContextProvider,
            gameServerContext,
            guildServer,
            context,
            owner,
            visitor);
    }

    private static void SetOpenedNpc(Player player, NpcWindow npcWindow)
    {
        player.OpenedNpc = new NonPlayerCharacter(
            null!,
            new MonsterDefinition { NpcWindow = npcWindow },
            null!);
    }

    private sealed record TestFixture(
        InMemoryPersistenceContextProvider PersistenceContextProvider,
        GameServerContext GameServerContext,
        Mock<IGuildServer> GuildServer,
        CastleSiegeContext Context,
        Player Owner,
        Player Visitor);
}
