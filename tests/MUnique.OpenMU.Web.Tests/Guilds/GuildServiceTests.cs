// <copyright file="GuildServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Guilds;

using Moq;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.Shared.Services;

using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="GuildService"/> which backs the admin panel guild pages.
/// </summary>
[TestFixture]
public class GuildServiceTests
{
    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    private Mock<IDataSource<GameConfiguration>> _gameConfigurationSource = null!;

    private GuildService _service = null!;

    /// <summary>
    /// Setups the test objects.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        this._gameConfigurationSource = new Mock<IDataSource<GameConfiguration>>();
        this._gameConfigurationSource
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GameConfiguration());
        var enricher = new CharacterGuildMemberEnricher(this._persistenceContextProvider, this._gameConfigurationSource.Object, Mock.Of<ILogger<CharacterGuildMemberEnricher>>());
        this._service = new GuildService(this._persistenceContextProvider, enricher, Mock.Of<ILogger<GuildService>>());
    }

    /// <summary>
    /// Tears down the test objects.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._service.Dispose();
    }

    /// <summary>
    /// Guilds are listed ordered by name.
    /// </summary>
    [Test]
    public async Task GetAsync_ReturnsGuildsOrderedByName()
    {
        await this.CreateGuildAsync("Zealots").ConfigureAwait(false);
        await this.CreateGuildAsync("Alphas").ConfigureAwait(false);

        var result = await this._service.GetAsync(0, 20).ConfigureAwait(false);

        Assert.That(result.Select(g => g.Name), Is.EqualTo(new[] { "Alphas", "Zealots" }));
    }

    /// <summary>
    /// Pagination applies after ordering.
    /// </summary>
    [Test]
    public async Task GetAsync_PaginationAppliesAfterOrdering()
    {
        await this.CreateGuildAsync("Charlie").ConfigureAwait(false);
        await this.CreateGuildAsync("Alpha").ConfigureAwait(false);
        await this.CreateGuildAsync("Bravo").ConfigureAwait(false);

        var result = await this._service.GetAsync(1, 1).ConfigureAwait(false);

        Assert.That(result.Select(g => g.Name), Is.EqualTo(new[] { "Bravo" }));
    }

    /// <summary>
    /// The search filter narrows the list down by guild name, case-insensitively.
    /// </summary>
    [Test]
    public async Task GetAsync_SearchFilter_NarrowsByName()
    {
        await this.CreateGuildAsync("DarkNights").ConfigureAwait(false);
        await this.CreateGuildAsync("LightBringers").ConfigureAwait(false);

        this._service.SearchFilter = "dark";

        var result = await this._service.GetAsync(0, 20).ConfigureAwait(false);

        Assert.That(result.Select(g => g.Name), Is.EqualTo(new[] { "DarkNights" }));
    }

    /// <summary>
    /// When the search filter narrows the list such that the current page offset is out of range,
    /// it falls back to the first page (offset 0).
    /// </summary>
    [Test]
    public async Task GetAsync_SearchFilterBeyondRange_FallsBackToFirstPage()
    {
        await this.CreateGuildAsync("DarkNights").ConfigureAwait(false);
        await this.CreateGuildAsync("LightBringers").ConfigureAwait(false);

        this._service.SearchFilter = "dark";

        // Offset 10 is beyond the 1 matching entry, so it falls back to page 0.
        var result = await this._service.GetAsync(10, 20).ConfigureAwait(false);

        Assert.That(result.Select(g => g.Name), Is.EqualTo(new[] { "DarkNights" }));
    }

    /// <summary>
    /// The alliance of a guild is shown on its list item.
    /// </summary>
    [Test]
    public async Task GetAsync_ShowsAllianceOfGuild()
    {
        var master = await this.CreateGuildAsync("Masters").ConfigureAwait(false);
        var member = await this.CreateGuildAsync("Minions").ConfigureAwait(false);
        await this.SetAllianceAsync(member.Id, master.Id).ConfigureAwait(false);

        var result = await this._service.GetAsync(0, 20).ConfigureAwait(false);

        var minions = result.Single(g => g.Name == "Minions");
        Assert.That(minions.AllianceGuildId, Is.EqualTo(master.Id));
        Assert.That(minions.AllianceName, Is.EqualTo("Masters"));
    }

    /// <summary>
    /// Guild logo is mapped to the list item.
    /// </summary>
    [Test]
    public async Task GetAsync_IncludesGuildLogo()
    {
        var logo = new byte[] { 0x18, 0x88, 0x88, 0x81 };
        await this.CreateGuildAsync("LogoGuild", logo).ConfigureAwait(false);

        var result = await this._service.GetAsync(0, 20).ConfigureAwait(false);

        var item = result.Single(g => g.Name == "LogoGuild");
        Assert.That(item.Logo, Is.EqualTo(logo));
    }

    /// <summary>
    /// Members are returned with the guild master first, including character details.
    /// </summary>
    [Test]
    public async Task GetGuildMembersAsync_ReturnsMasterFirstWithCharacterDetails()
    {
        var guild = await this.CreateGuildAsync("Knights").ConfigureAwait(false);
        await this.AddMemberAsync(guild.Id, "Squire", Interfaces.GuildPosition.NormalMember, 50, 0, "squireAccount").ConfigureAwait(false);
        var (lordCharacterId, lordAccountId) = await this.AddMemberAsync(guild.Id, "Lord", Interfaces.GuildPosition.GuildMaster, 400, 200, "lordAccount").ConfigureAwait(false);

        var members = await this._service.GetGuildMembersAsync(guild.Id).ConfigureAwait(false);

        Assert.That(members.Select(m => m.CharacterName), Is.EqualTo(new[] { "Lord", "Squire" }));
        var master = members.First();
        Assert.Multiple(() =>
        {
            Assert.That(master.Position, Is.EqualTo(Interfaces.GuildPosition.GuildMaster));
            Assert.That(master.Level, Is.EqualTo(400));
            Assert.That(master.MasterLevel, Is.EqualTo(200));
            Assert.That(master.AccountLoginName, Is.EqualTo("lordAccount"));
            Assert.That(master.CharacterId, Is.EqualTo(lordCharacterId));
            Assert.That(master.AccountId, Is.EqualTo(lordAccountId));
        });
    }

    /// <summary>
    /// Without a player context, members are still listed with their names and positions.
    /// </summary>
    [Test]
    public async Task GetGuildMembersAsync_WithoutPlayerContext_StillListsNamesAndPositions()
    {
        var guild = await this.CreateGuildAsync("Rogues").ConfigureAwait(false);
        await this.AddMemberAsync(guild.Id, "Shadow", Interfaces.GuildPosition.BattleMaster, 100, 0, null).ConfigureAwait(false);
        this._gameConfigurationSource
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("no database"));

        var members = await this._service.GetGuildMembersAsync(guild.Id).ConfigureAwait(false);

        Assert.That(members, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(members[0].CharacterName, Is.EqualTo("Shadow"));
            Assert.That(members[0].Position, Is.EqualTo(Interfaces.GuildPosition.BattleMaster));
        });
    }

    /// <summary>
    /// Members are ordered by rank: GuildMaster -> AssistantMaster -> BattleMaster -> NormalMember,
    /// and then alphabetically by character name within the same rank.
    /// </summary>
    [Test]
    public async Task GetGuildMembersAsync_OrdersMembersByPositionRankThenByName()
    {
        var guild = await this.CreateGuildAsync("OrderGuild").ConfigureAwait(false);
        await this.AddGuildMemberAsync(guild.Id, "ZackNormal", Interfaces.GuildPosition.NormalMember).ConfigureAwait(false);
        await this.AddGuildMemberAsync(guild.Id, "AliceNormal", Interfaces.GuildPosition.NormalMember).ConfigureAwait(false);
        await this.AddGuildMemberAsync(guild.Id, "BobBattle", Interfaces.GuildPosition.BattleMaster).ConfigureAwait(false);
        await this.AddGuildMemberAsync(guild.Id, "CharlieAssistant", Interfaces.GuildPosition.AssistantMaster).ConfigureAwait(false);
        await this.AddGuildMemberAsync(guild.Id, "DaveMaster", Interfaces.GuildPosition.GuildMaster).ConfigureAwait(false);

        var members = await this._service.GetGuildMembersAsync(guild.Id).ConfigureAwait(false);

        Assert.That(members.Select(m => m.CharacterName), Is.EqualTo(new[]
        {
            "DaveMaster",
            "CharlieAssistant",
            "BobBattle",
            "AliceNormal",
            "ZackNormal",
        }));
    }

    /// <summary>
    /// An empty guild (or unknown guild) returns an empty list without error.
    /// </summary>
    [Test]
    public async Task GetGuildMembersAsync_EmptyGuild_ReturnsEmpty()
    {
        var guild = await this.CreateGuildAsync("EmptyGuild").ConfigureAwait(false);

        var members = await this._service.GetGuildMembersAsync(guild.Id).ConfigureAwait(false);

        Assert.That(members, Is.Empty);
    }

    /// <summary>
    /// The alliance page lists all guilds of the alliance, including the master.
    /// </summary>
    [Test]
    public async Task GetAllianceAsync_ListsAllGuildsOfAlliance()
    {
        var master = await this.CreateGuildAsync("AllianceMaster").ConfigureAwait(false);
        var member = await this.CreateGuildAsync("AllianceMember").ConfigureAwait(false);
        await this.SetAllianceAsync(member.Id, master.Id).ConfigureAwait(false);
        await this.SetAllianceAsync(master.Id, master.Id).ConfigureAwait(false);

        var alliance = await this._service.GetAllianceAsync(member.Id).ConfigureAwait(false);

        Assert.That(alliance, Is.Not.Null);
        Assert.That(alliance!.Master.Id, Is.EqualTo(master.Id));
        Assert.That(alliance.Guilds.Select(g => g.Name), Is.EquivalentTo(new[] { "AllianceMaster", "AllianceMember" }));
    }

    /// <summary>
    /// An unknown guild identifier returns <c>null</c>.
    /// </summary>
    [Test]
    public async Task GetAllianceAsync_UnknownGuild_ReturnsNull()
    {
        Assert.That(await this._service.GetAllianceAsync(Guid.NewGuid()).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// An unknown guild identifier returns <c>null</c> from <see cref="GuildService.GetGuildAsync"/>.
    /// </summary>
    [Test]
    public async Task GetGuildAsync_UnknownGuild_ReturnsNull()
    {
        Assert.That(await this._service.GetGuildAsync(Guid.NewGuid()).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// A guild which is itself the master of an alliance (other guilds point to it via
    /// <see cref="Interfaces.Guild.AllianceGuild"/>, but it doesn't point to anyone) still gets an
    /// "Alliance" link pointing to itself - both in the paged list and on its own detail page.
    /// Without this, only member guilds would be able to reach the alliance page.
    /// </summary>
    [Test]
    public async Task GetAsync_And_GetGuildAsync_MasterGuild_HasSelfAllianceLink()
    {
        var master = await this.CreateGuildAsync("AllianceMaster").ConfigureAwait(false);
        var member = await this.CreateGuildAsync("AllianceMember").ConfigureAwait(false);
        await this.SetAllianceAsync(member.Id, master.Id).ConfigureAwait(false);

        var listResult = await this._service.GetAsync(0, 20).ConfigureAwait(false);
        var masterInList = listResult.Single(g => g.Id == master.Id);
        Assert.Multiple(() =>
        {
            Assert.That(masterInList.AllianceGuildId, Is.EqualTo(master.Id));
            Assert.That(masterInList.AllianceName, Is.EqualTo("AllianceMaster"));
        });

        var masterDetail = await this._service.GetGuildAsync(master.Id).ConfigureAwait(false);
        Assert.That(masterDetail, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(masterDetail!.AllianceGuildId, Is.EqualTo(master.Id));
            Assert.That(masterDetail.AllianceName, Is.EqualTo("AllianceMaster"));
        });
    }

    /// <summary>
    /// A guild which is not part of any alliance - neither a member nor a master - has no alliance link.
    /// </summary>
    [Test]
    public async Task GetGuildAsync_GuildWithoutAlliance_HasNoAllianceLink()
    {
        var guild = await this.CreateGuildAsync("Loners").ConfigureAwait(false);

        var detail = await this._service.GetGuildAsync(guild.Id).ConfigureAwait(false);

        Assert.That(detail, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(detail!.AllianceGuildId, Is.Null);
            Assert.That(detail.AllianceName, Is.Null);
        });
    }

    /// <summary>
    /// Enrichment must not use the generic character-by-id lookup: in production (EF)
    /// that path eagerly walks config-type navigations which are excluded from the
    /// account database model, so it throws and members lose all details.
    /// A strict player context mock throws on any unexpected call, simulating that.
    /// </summary>
    [Test]
    public async Task GetGuildMembersAsync_WorksWithoutCharacterByIdLookup()
    {
        var guild = await this.CreateGuildAsync("Knights").ConfigureAwait(false);
        var characterId = await this.AddGuildMemberAsync(guild.Id, "Lord", Interfaces.GuildPosition.GuildMaster).ConfigureAwait(false);

        var accountId = Guid.NewGuid();
        var account = new BasicModel.Account { Id = accountId, LoginName = "lordAccount" };
        var character = new BasicModel.Character { Id = characterId, Name = "Lord" };
        character.Attributes.Add(new BasicModel.StatAttribute(Stats.Level, 400));
        character.Attributes.Add(new BasicModel.StatAttribute(Stats.MasterLevel, 200));
        account.Characters.Add(character);

        var playerContextMock = new Mock<IPlayerContext>(MockBehavior.Strict);
        playerContextMock.Setup(c => c.Dispose());
        playerContextMock
            .Setup(c => c.GetAccountByCharacterNameAsync("Lord", It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var providerMock = new Mock<IPersistenceContextProvider>();
        providerMock.Setup(p => p.CreateNewGuildContext()).Returns(() => this._persistenceContextProvider.CreateNewGuildContext());
        providerMock.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(playerContextMock.Object);

        var enricher = new CharacterGuildMemberEnricher(providerMock.Object, this._gameConfigurationSource.Object, Mock.Of<ILogger<CharacterGuildMemberEnricher>>());
        using var service = new GuildService(providerMock.Object, enricher, Mock.Of<ILogger<GuildService>>());

        var members = await service.GetGuildMembersAsync(guild.Id).ConfigureAwait(false);

        Assert.That(members, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(members[0].CharacterName, Is.EqualTo("Lord"));
            Assert.That(members[0].Position, Is.EqualTo(Interfaces.GuildPosition.GuildMaster));
            Assert.That(members[0].Level, Is.EqualTo(400));
            Assert.That(members[0].MasterLevel, Is.EqualTo(200));
            Assert.That(members[0].AccountLoginName, Is.EqualTo("lordAccount"));
            Assert.That(members[0].AccountId, Is.EqualTo(accountId));
        });
        playerContextMock.VerifyAll();
    }

    private async Task<Guild> CreateGuildAsync(string name, byte[]? logo = null)
    {
        using var context = this._persistenceContextProvider.CreateNewGuildContext();
        var guild = context.CreateNew<Guild>();
        guild.Name = name;
        guild.Logo = logo;
        await context.SaveChangesAsync().ConfigureAwait(false);
        return guild;
    }

    private async Task SetAllianceAsync(Guid guildId, Guid allianceMasterId)
    {
        using var context = this._persistenceContextProvider.CreateNewGuildContext();
        var guild = await context.GetByIdAsync<Guild>(guildId).ConfigureAwait(false);
        var master = await context.GetByIdAsync<Guild>(allianceMasterId).ConfigureAwait(false);
        guild!.AllianceGuild = master;
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task<Guid> AddGuildMemberAsync(Guid guildId, string characterName, Interfaces.GuildPosition position)
    {
        var (characterId, _) = await this.AddMemberAsync(guildId, characterName, position, 0, 0, null).ConfigureAwait(false);
        return characterId;
    }

    private async Task<(Guid CharacterId, Guid? AccountId)> AddMemberAsync(Guid guildId, string characterName, Interfaces.GuildPosition position, int level, int masterLevel, string? accountLoginName)
    {
        var gameConfiguration = await this._gameConfigurationSource.Object.GetOwnerAsync().ConfigureAwait(false);
        using var playerContext = this._persistenceContextProvider.CreateNewPlayerContext(gameConfiguration);
        var character = playerContext.CreateNew<Character>();
        character.Name = characterName;
        character.Attributes.Add(new BasicModel.StatAttribute(Stats.Level, level));
        character.Attributes.Add(new BasicModel.StatAttribute(Stats.MasterLevel, masterLevel));
        Guid? accountId = null;
        if (accountLoginName is not null)
        {
            var account = playerContext.CreateNew<Account>();
            account.LoginName = accountLoginName;
            account.Characters.Add(character);
            accountId = account.GetId();
        }

        await playerContext.SaveChangesAsync().ConfigureAwait(false);

        using var guildContext = this._persistenceContextProvider.CreateNewGuildContext();
        var member = guildContext.CreateNew<GuildMember>(character.Id);
        member.Status = position;
        member.GuildId = guildId;
        var guild = await guildContext.GetByIdAsync<Guild>(guildId).ConfigureAwait(false);
        guild!.Members.Add(member);
        await guildContext.SaveChangesAsync().ConfigureAwait(false);
        return (character.Id, accountId);
    }
}
