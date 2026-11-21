// <copyright file="LordsMixCraftingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;

/// <summary>
/// Tests the preconditions and the limit of the <see cref="LordsMixCrafting"/>.
/// </summary>
[TestFixture]
public class LordsMixCraftingTest
{
    private const uint OwnerGuildId = 10;
    private const uint VisitorGuildId = 20;

    private static readonly DateTime Today = new(2026, 8, 3);

    /// <summary>
    /// Tests that the guild master of the castle owner guild may use the Lord's Mix.
    /// </summary>
    [Test]
    public async ValueTask GuildMasterOfTheOwnerMayMixAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Tests that the assistant master of the castle owner guild may not use the Lord's Mix.
    /// </summary>
    [Test]
    public async ValueTask AssistantMasterOfTheOwnerIsRefusedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Owner.GuildStatus = new GuildMemberStatus(OwnerGuildId, GuildPosition.AssistantMaster);

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
    }

    /// <summary>
    /// Tests that the guild master of another guild may not use the Lord's Mix.
    /// </summary>
    [Test]
    public async ValueTask GuildMasterOfAnotherGuildIsRefusedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Visitor, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
    }

    /// <summary>
    /// Tests that nobody may use the Lord's Mix while the castle has no owner.
    /// </summary>
    [Test]
    public async ValueTask UnoccupiedCastleIsRefusedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.SiegeData.IsOccupied = false;

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
    }

    /// <summary>
    /// Tests that the Lord's Mix is refused when the limit of the siege cycle is reached.
    /// </summary>
    [Test]
    public async ValueTask LimitReachedIsRefusedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.Configuration.LordsMixLimit = 1;
        fixture.Context.Configuration.LordsMixLimitPeriod = LordsMixLimitPeriod.SiegeCycle;
        fixture.Context.SiegeData.LordsMixCount = 1;

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
    }

    /// <summary>
    /// Tests that a limit of 0 means no limit.
    /// </summary>
    [Test]
    public async ValueTask LimitZeroIsUnlimitedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.Configuration.LordsMixLimit = 0;
        fixture.Context.SiegeData.LordsMixCount = 5;

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Tests that a successful mix is counted and saved.
    /// </summary>
    [Test]
    public async ValueTask SuccessfulMixIsCountedAndSavedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);

        await LordsMixCrafting.CountSuccessfulMixAsync(fixture.Context, Today).ConfigureAwait(false);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewContext();
        var persistentData = await persistenceContext.GetByIdAsync<CastleSiegeData>(fixture.Context.SiegeData.Id).ConfigureAwait(false);
        Assert.That(fixture.Context.SiegeData.LordsMixCount, Is.EqualTo(1));
        Assert.That(persistentData?.LordsMixCount, Is.EqualTo(1));
        Assert.That(persistentData?.LordsMixCountDate, Is.EqualTo(Today));
    }

    /// <summary>
    /// Tests that with a limit per day, the Lord's Mix is refused when the limit is reached on the same day.
    /// </summary>
    [Test]
    public async ValueTask DailyLimitReachedTodayIsRefusedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.Configuration.LordsMixLimitPeriod = LordsMixLimitPeriod.Day;
        fixture.Context.SiegeData.LordsMixCount = 1;
        fixture.Context.SiegeData.LordsMixCountDate = Today;

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today.AddHours(23)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
    }

    /// <summary>
    /// Tests that with a limit per day, the mixes of an earlier day don't count.
    /// </summary>
    [Test]
    public async ValueTask DailyLimitOfAnEarlierDayIsNotCountedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.Configuration.LordsMixLimitPeriod = LordsMixLimitPeriod.Day;
        fixture.Context.SiegeData.LordsMixCount = 1;
        fixture.Context.SiegeData.LordsMixCountDate = Today.AddDays(-1);

        var result = await LordsMixCrafting.CheckPreconditionsAsync(fixture.Owner, fixture.Context, Today).ConfigureAwait(false);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Tests that with a limit per day, a mix on a new day starts the count again at one, with the new date.
    /// </summary>
    [Test]
    public async ValueTask DailyCountStartsAgainOnANewDayAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.Configuration.LordsMixLimitPeriod = LordsMixLimitPeriod.Day;
        fixture.Context.SiegeData.LordsMixCount = 3;
        fixture.Context.SiegeData.LordsMixCountDate = Today.AddDays(-1);

        await LordsMixCrafting.CountSuccessfulMixAsync(fixture.Context, Today).ConfigureAwait(false);

        Assert.That(fixture.Context.SiegeData.LordsMixCount, Is.EqualTo(1));
        Assert.That(fixture.Context.SiegeData.LordsMixCountDate, Is.EqualTo(Today));
        Assert.That(fixture.Context.SiegeData.LordsMixCountDate!.Value.Kind, Is.EqualTo(DateTimeKind.Utc), "PostgreSQL only stores UTC date times");
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
            castleSiegeConfiguration.LordsMixLimit = 1;
            castleSiegeConfiguration.CastleSiegeMapDefinition = siegeMap;
            castleSiegeConfiguration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Ready, DayOfWeek = DayOfWeek.Monday });
            castleSiegeConfiguration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Start, DayOfWeek = DayOfWeek.Tuesday });
            gameConfiguration.CastleSiegeConfiguration = castleSiegeConfiguration;

            var ownerGuild = persistenceContext.CreateNew<BasicModel.Guild>();
            ownerGuild.Name = "CastleOwners";
            persistentOwnerGuildId = ownerGuild.Id;
            var siegeData = persistenceContext.CreateNew<BasicModel.CastleSiegeData>();
            siegeData.OwnerGuildId = persistentOwnerGuildId;
            siegeData.IsOccupied = true;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(server => server.GetGuildAsync(OwnerGuildId)).Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "CastleOwners" }));
        guildServer.Setup(server => server.GetGuildAsync(VisitorGuildId)).Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "Visitors" }));
        guildServer.Setup(server => server.GetPersistentGuildIdAsync(OwnerGuildId)).Returns(new ValueTask<Guid?>(persistentOwnerGuildId));
        guildServer.Setup(server => server.GetPersistentGuildIdAsync(VisitorGuildId)).Returns(new ValueTask<Guid?>(Guid.NewGuid()));

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

        var owner = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        owner.GuildStatus = new GuildMemberStatus(OwnerGuildId, GuildPosition.GuildMaster);
        var visitor = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        visitor.GuildStatus = new GuildMemberStatus(VisitorGuildId, GuildPosition.GuildMaster);
        var context = new CastleSiegeContext(gameServerContext, castleSiegeConfiguration);
        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        return new(persistenceContextProvider, context, owner, visitor);
    }

    private sealed record TestFixture(
        InMemoryPersistenceContextProvider PersistenceContextProvider,
        CastleSiegeContext Context,
        Player Owner,
        Player Visitor);
}
