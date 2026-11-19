// <copyright file="DiscordGameDataProviderTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="DiscordGameDataProvider"/>.
/// </summary>
[TestFixture]
public class DiscordGameDataProviderTest
{
    private const uint GuildRuntimeId = 7;

    private readonly Guid _guildId = Guid.NewGuid();
    private Guid _classId;
    private IServerProvider _serverProvider = null!;
    private InMemoryPersistenceContextProvider _persistence = null!;
    private Mock<IFriendServer> _friendServer = null!;
    private Mock<IGuildServer> _guildServer = null!;
    private PlugInManager _plugInManager = null!;
    private DiscordGameDataProvider _provider = null!;

    /// <summary>
    /// Sets up the provider with in-memory data and mocked servers.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        var gameServer = new Mock<IGameServer>();
        gameServer.SetupGet(s => s.Id).Returns(3);
        gameServer.SetupGet(s => s.Description).Returns("Server 4");
        gameServer.SetupGet(s => s.ServerState).Returns(ServerState.Started);
        gameServer.SetupGet(s => s.CurrentConnections).Returns(5);
        gameServer.SetupGet(s => s.MaximumConnections).Returns(100);
        var serverProvider = new Mock<IServerProvider>();
        serverProvider.SetupGet(p => p.Servers).Returns(new List<IManageableServer> { gameServer.Object, Mock.Of<IManageableServer>() });

        this._friendServer = new Mock<IFriendServer>();
        this._friendServer.Setup(s => s.GetOnlineServerIdAsync("Hero")).ReturnsAsync((byte?)3);

        this._guildServer = new Mock<IGuildServer>();
        this._guildServer.Setup(s => s.GetPersistentGuildIdByNameAsync("Legends")).ReturnsAsync(this._guildId);
        this._guildServer.Setup(s => s.GetGuildIdAsync(this._guildId)).ReturnsAsync(GuildRuntimeId);
        this._guildServer.Setup(s => s.GetGuildAsync(GuildRuntimeId)).ReturnsAsync(new Interfaces.Guild { Name = "Legends" });
        this._guildServer.Setup(s => s.GetGuildListAsync(GuildRuntimeId)).ReturnsAsync(ImmutableList.Create(
            new GuildListEntry { PlayerName = "Hero", PlayerPosition = GuildPosition.GuildMaster, ServerId = 3 },
            new GuildListEntry { PlayerName = "Elf", PlayerPosition = GuildPosition.NormalMember, ServerId = (byte)SpecialServerId.Offline },
            new GuildListEntry { PlayerName = "Ghost", PlayerPosition = GuildPosition.NormalMember, ServerId = (byte)SpecialServerId.Invisible }) as IImmutableList<GuildListEntry>);

        this._plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);

        var persistence = this._persistence = new InMemoryPersistenceContextProvider();
        using (var context = persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var characterClass = context.CreateNew<CharacterClass>();
            characterClass.Name = "Blade Knight";
            this._classId = characterClass.GetId();
            var account = context.CreateNew<Account>();
            account.LoginName = "hero";
            var character = context.CreateNew<Character>();
            character.Name = "Hero";
            character.CharacterClass = characterClass;
            character.Attributes.Add(new Persistence.BasicModel.StatAttribute(Stats.Level, 400));
            account.Characters.Add(character);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        this._serverProvider = serverProvider.Object;
        this._provider = new DiscordGameDataProvider(serverProvider.Object, persistence, this._friendServer.Object, this._guildServer.Object, this._plugInManager, () => TimeZoneInfo.Utc);
    }

    /// <summary>
    /// Tests that only the game servers are listed.
    /// </summary>
    [Test]
    public void GameServersAreListed()
    {
        Assert.That(this._provider.GetGameServers(), Is.EqualTo(new[] { new GameServerStatus(3, "Server 4", true, 5, 100) }));
    }

    /// <summary>
    /// Tests that a character is found with its online state.
    /// </summary>
    [Test]
    public async Task CharacterIsFoundWithOnlineServerAsync()
    {
        var character = await this._provider.GetCharacterAsync("Hero").ConfigureAwait(false);

        Assert.That(character, Is.EqualTo(new CharacterInfo("Hero", "Blade Knight", 400, 0, 0, "Server 4")));
        Assert.That(await this._provider.GetCharacterAsync("Nobody").ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that a guild is found with its master and the members which are visibly online.
    /// </summary>
    [Test]
    public async Task GuildIsFoundWithOnlineMembersAsync()
    {
        var guild = await this._provider.GetGuildAsync("Legends").ConfigureAwait(false);

        Assert.That(guild?.MasterName, Is.EqualTo("Hero"));
        Assert.That(guild?.MemberCount, Is.EqualTo(3));
        Assert.That(guild?.OnlineMemberNames, Is.EqualTo(new[] { "Hero" }));
        Assert.That(await this._provider.GetGuildAsync("Unknown").ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that the upcoming events come from the schedules of the active plugins.
    /// </summary>
    [Test]
    public async Task UpcomingEventsComeFromThePlugInsAsync()
    {
        var nextHour = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(1));
        this._plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new GoldenInvasionPlugIn { Configuration = new PeriodicInvasionConfiguration { Timetable = { nextHour } } });

        var events = await this._provider.GetUpcomingEventsAsync(CultureInfo.InvariantCulture).ConfigureAwait(false);

        Assert.That(events.Select(e => e.Name), Is.EqualTo(new[] { "Golden Invasion" }));
    }

    /// <summary>
    /// Tests that the ranking comes from the database.
    /// </summary>
    [Test]
    public async Task RankingComesFromTheDatabaseAsync()
    {
        var ranking = await this._provider.GetRankingAsync(10).ConfigureAwait(false);

        Assert.That(ranking.Select(c => c.Name), Is.EqualTo(new[] { "Hero" }));
    }

    /// <summary>
    /// Tests that the class names are resolved from the configuration, because the entity framework queries
    /// only return the identifier of the class; a summary with an unknown class keeps its name.
    /// </summary>
    [Test]
    public async Task ClassNamesAreResolvedFromTheConfigurationAsync()
    {
        var unknownClassId = Guid.NewGuid();
        var playerContext = new Mock<IPlayerContext>();
        playerContext
            .Setup(c => c.GetCharacterRankingAsync(Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new CharacterSummary("Hero", string.Empty, 400, 0, 0, this._classId), new CharacterSummary("Stranger", "Unknown", 1, 0, 0, unknownClassId) });
        playerContext
            .Setup(c => c.GetCharacterSummaryAsync("Hero", Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CharacterSummary("Hero", string.Empty, 400, 0, 0, this._classId));
        var persistence = new Mock<IPersistenceContextProvider>();
        persistence.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(playerContext.Object);
        persistence.Setup(p => p.CreateNewTypedContext(typeof(CharacterClass), false, null)).Returns(() => this._persistence.CreateNewTypedContext(typeof(CharacterClass), false));
        var provider = new DiscordGameDataProvider(this._serverProvider, persistence.Object, this._friendServer.Object, this._guildServer.Object, this._plugInManager, () => TimeZoneInfo.Utc);

        var ranking = await provider.GetRankingAsync(10).ConfigureAwait(false);
        var character = await provider.GetCharacterAsync("Hero").ConfigureAwait(false);

        Assert.That(ranking.Select(c => (c.Name, c.ClassName)), Is.EqualTo(new[] { ("Hero", "Blade Knight"), ("Stranger", "Unknown") }));
        Assert.That(character?.ClassName, Is.EqualTo("Blade Knight"));
    }

    /// <summary>
    /// Tests that the ranking is cached, because every member of the Discord server can request it.
    /// </summary>
    [Test]
    public async Task RankingIsCachedAsync()
    {
        var first = await this._provider.GetRankingAsync(10).ConfigureAwait(false);
        using (var context = this._persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "newcomer";
            var character = context.CreateNew<Character>();
            character.Name = "Newcomer";
            character.CharacterClass = context.CreateNew<CharacterClass>();
            character.Attributes.Add(new Persistence.BasicModel.StatAttribute(Stats.Level, 1));
            account.Characters.Add(character);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var second = await this._provider.GetRankingAsync(10).ConfigureAwait(false);

        Assert.That(second, Is.SameAs(first));
    }
}
