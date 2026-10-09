// <copyright file="DiscordChatBridgeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="DiscordChatBridge"/>.
/// </summary>
[TestFixture]
public class DiscordChatBridgeTest
{
    private const uint LegendsRuntimeId = 1;
    private const ulong ServerId = 500;
    private const ulong ChannelId = 600;
    private const ulong WorldChannelId = 700;
    private const ulong GuildMasterUser = 42;
    private const ulong MemberUser = 43;
    private const ulong OutsiderUser = 44;
    private const ulong AllyUser = 45;

    private InMemoryPersistenceContextProvider _persistence = null!;
    private Mock<IEventPublisher> _publisher = null!;
    private DiscordChatBridgeSettings _settings = null!;
    private Guid _legendsId;

    /// <summary>
    /// Sets up the accounts, guilds and links:
    /// Hero is the guild master of Legends, which is the master of an alliance with Allies.
    /// Elf is a member of Legends, Ally a member of Allies, and Outsider isn't in a guild.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        this._publisher = new Mock<IEventPublisher>();
        this._settings = new DiscordChatBridgeSettings();

        var hero = await this.CreateAccountAsync("hero", "Hero").ConfigureAwait(false);
        var elf = await this.CreateAccountAsync("elf", "Elf").ConfigureAwait(false);
        var ally = await this.CreateAccountAsync("ally", "Ally").ConfigureAwait(false);
        var outsider = await this.CreateAccountAsync("outsider", "Outsider").ConfigureAwait(false);

        using (var guildContext = this._persistence.CreateNewGuildContext())
        {
            var legends = guildContext.CreateNew<DataModel.Entities.Guild>();
            legends.Name = "Legends";
            legends.Members.Add(CreateMember(guildContext, legends, hero.CharacterId, GuildPosition.GuildMaster));
            legends.Members.Add(CreateMember(guildContext, legends, elf.CharacterId, GuildPosition.NormalMember));
            var allies = guildContext.CreateNew<DataModel.Entities.Guild>();
            allies.Name = "Allies";
            allies.AllianceGuild = legends;
            allies.Members.Add(CreateMember(guildContext, allies, ally.CharacterId, GuildPosition.GuildMaster));
            await guildContext.SaveChangesAsync().ConfigureAwait(false);
            this._legendsId = legends.GetId();
        }

        var linkService = this.CreateLinkService();
        await LinkAsync(linkService, hero.AccountId, "Hero", GuildMasterUser).ConfigureAwait(false);
        await LinkAsync(linkService, elf.AccountId, "Elf", MemberUser).ConfigureAwait(false);
        await LinkAsync(linkService, ally.AccountId, "Ally", AllyUser).ConfigureAwait(false);
        await LinkAsync(linkService, outsider.AccountId, "Outsider", OutsiderUser).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that only the guild master can bind the chat of the guild.
    /// </summary>
    [Test]
    public async Task OnlyGuildMasterCanBindAsync()
    {
        var bridge = this.CreateBridge();

        var (memberResult, _) = await bridge.BindAsync(MemberUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);
        var (notLinkedResult, _) = await bridge.BindAsync(99, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);
        var (masterResult, target) = await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        Assert.That(memberResult, Is.EqualTo(DiscordChatBindResult.NotGuildMaster));
        Assert.That(notLinkedResult, Is.EqualTo(DiscordChatBindResult.NotLinked));
        Assert.That(masterResult, Is.EqualTo(DiscordChatBindResult.Success));
        Assert.That(target, Is.EqualTo(new GuildChatTarget(this._legendsId, "Legends", "Hero")));
        var binding = bridge.Store.GetByChannel(ChannelId);
        Assert.That(binding, Is.Not.Null);
        Assert.That(binding!.GuildId, Is.EqualTo(this._legendsId));
        Assert.That(binding.IsHosted, Is.False);

        // The binding survives a restart.
        var reloaded = this.CreateBridge();
        await reloaded.Store.ReloadAsync().ConfigureAwait(false);
        Assert.That(reloaded.Store.GetByChannel(ChannelId)?.GuildId, Is.EqualTo(this._legendsId));
    }

    /// <summary>
    /// Tests that the alliance chat can only be bound by the master of the alliance, and that a channel can't be bound twice.
    /// </summary>
    [Test]
    public async Task AllianceChatIsBoundByAllianceMasterAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        var (allyResult, _) = await bridge.BindAsync(AllyUser, GuildChatScope.Alliance, ServerId, ChannelId + 1, false).ConfigureAwait(false);
        var (inUseResult, _) = await bridge.BindAsync(GuildMasterUser, GuildChatScope.Alliance, ServerId, ChannelId, false).ConfigureAwait(false);
        var (masterResult, _) = await bridge.BindAsync(GuildMasterUser, GuildChatScope.Alliance, ServerId, ChannelId + 1, false).ConfigureAwait(false);

        Assert.That(allyResult, Is.EqualTo(DiscordChatBindResult.NotAllianceMaster));
        Assert.That(inUseResult, Is.EqualTo(DiscordChatBindResult.ChannelInUse));
        Assert.That(masterResult, Is.EqualTo(DiscordChatBindResult.Success));
    }

    /// <summary>
    /// Tests that the binding mode and the allowed Discord servers are respected.
    /// </summary>
    [Test]
    public void SettingsRestrictBindings()
    {
        this._settings.BindingMode = DiscordGuildChatBindingMode.HostedOnly;
        Assert.That(this.CreateBridge().CheckAllowed(ServerId, false), Is.EqualTo(DiscordChatBindResult.ModeNotAllowed));
        Assert.That(this.CreateBridge().CheckAllowed(ServerId, true), Is.Null);

        this._settings.BindingMode = DiscordGuildChatBindingMode.Both;
        this._settings.AllowedDiscordServerIds.Add(ServerId + 1);
        Assert.That(this.CreateBridge().CheckAllowed(ServerId, false), Is.EqualTo(DiscordChatBindResult.ServerNotAllowed));
        Assert.That(this.CreateBridge().CheckAllowed(ServerId + 1, false), Is.Null);
    }

    /// <summary>
    /// Tests that the guild and alliance chat messages of the game are mirrored to the bound channels.
    /// </summary>
    [Test]
    public async Task GameMessagesAreMirroredToBoundChannelsAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        var guildMessage = await bridge.GetDiscordMessageAsync(new ChatMessageEvent(0, DateTime.UtcNow, GameChatChannel.Guild, LegendsRuntimeId, "Elf", "Hi *all*"), WorldChannelId).ConfigureAwait(false);
        var allianceMessage = await bridge.GetDiscordMessageAsync(new ChatMessageEvent(0, DateTime.UtcNow, GameChatChannel.Alliance, LegendsRuntimeId, "Elf", "Hi"), WorldChannelId).ConfigureAwait(false);
        var worldMessage = await bridge.GetDiscordMessageAsync(new ChatMessageEvent(0, DateTime.UtcNow, GameChatChannel.World, 0, "Elf", "Hi"), WorldChannelId).ConfigureAwait(false);

        Assert.That(guildMessage, Is.EqualTo((ChannelId, @"**Elf**: Hi \*all\*")));
        Assert.That(allianceMessage, Is.Null);
        Assert.That(worldMessage, Is.EqualTo((WorldChannelId, "**Elf**: Hi")));
    }

    /// <summary>
    /// Tests that a member of the guild can write into the chat of the game, as its character with a prefix.
    /// </summary>
    [Test]
    public async Task MemberWritesIntoGuildChatAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        var result = await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hello <@123> **guild**").ConfigureAwait(false);

        Assert.That(result, Is.EqualTo((DiscordChatPostResult.Sent, "Elf")));
        this._publisher.Verify(p => p.GuildMessageAsync(LegendsRuntimeId, "@Elf", "Hello guild"), Times.Once);
    }

    /// <summary>
    /// Tests the reasons why a message isn't sent to the game.
    /// </summary>
    [Test]
    public async Task MessagesAreRejectedAsync()
    {
        this._settings.MaximumMessagesPerMinute = 1;
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        Assert.That((await bridge.PostToGameAsync(ChannelId + 5, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.NotBridged));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, 99, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.NotLinked));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, OutsiderUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.NotMember));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "<@123>").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.Empty));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.Sent));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.TooFast));

        using (var context = this._persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = await context.GetAccountByLoginNameAsync("elf").ConfigureAwait(false);
            account!.ChatBanUntil = DateTime.UtcNow.AddHours(1);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.ChatBanned));
    }

    /// <summary>
    /// Tests that every linked user can write into the world chat.
    /// </summary>
    [Test]
    public async Task LinkedUserWritesIntoWorldChatAsync()
    {
        var result = await this.CreateBridge().PostToGameAsync(WorldChannelId, WorldChannelId, OutsiderUser, "Hello world").ConfigureAwait(false);

        Assert.That(result.Result, Is.EqualTo(DiscordChatPostResult.Sent));
        this._publisher.Verify(p => p.WorldChatMessageAsync("@Outsider", "Hello world"), Times.Once);
    }

    /// <summary>
    /// Tests that the binding can be removed by the guild master and by users who manage the channel.
    /// </summary>
    [Test]
    public async Task UnbindAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        Assert.That(await bridge.UnbindAsync(ChannelId, MemberUser, false).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.NotGuildMaster));
        Assert.That(await bridge.UnbindAsync(ChannelId, MemberUser, true).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.Success));
        Assert.That(await bridge.UnbindAsync(ChannelId, GuildMasterUser, false).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.NotBound));
        Assert.That(bridge.Store.Bindings, Is.Empty);
    }

    /// <summary>
    /// Tests that the linked members of a guild and an alliance are determined, e.g. for the visibility of hosted channels.
    /// </summary>
    [Test]
    public async Task LinkedMembersAreDeterminedAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, true).ConfigureAwait(false);
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Alliance, ServerId, ChannelId + 1, true).ConfigureAwait(false);

        var guildMembers = await bridge.GetLinkedMembersAsync(bridge.Store.GetByChannel(ChannelId)!).ConfigureAwait(false);
        var allianceMembers = await bridge.GetLinkedMembersAsync(bridge.Store.GetByChannel(ChannelId + 1)!).ConfigureAwait(false);

        Assert.That(guildMembers, Is.EquivalentTo(new[] { GuildMasterUser, MemberUser }));
        Assert.That(allianceMembers, Is.EquivalentTo(new[] { GuildMasterUser, MemberUser, AllyUser }));
    }

    private static GuildMember CreateMember(IContext context, DataModel.Entities.Guild guild, Guid characterId, GuildPosition position)
    {
        var member = context.CreateNew<GuildMember>(characterId);
        member.GuildId = guild.GetId();
        member.Status = position;
        return member;
    }

    private static async Task LinkAsync(AccountLinkService linkService, Guid accountId, string characterName, ulong userId)
    {
        var code = await linkService.CreateCodeAsync(accountId, AccountLinkService.DiscordProvider, characterName).ConfigureAwait(false);
        await linkService.LinkAsync(AccountLinkService.DiscordProvider, code, userId.ToString(), characterName).ConfigureAwait(false);
    }

    private AccountLinkService CreateLinkService() => new(() => this._persistence.CreateNewPlayerContext(new GameConfiguration()));

    private DiscordChatBridge CreateBridge()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(s => s.GetPersistentGuildIdAsync(LegendsRuntimeId)).ReturnsAsync(this._legendsId);
        guildServer.Setup(s => s.GetPersistentAllianceMasterGuildIdAsync(LegendsRuntimeId)).ReturnsAsync(this._legendsId);
        guildServer.Setup(s => s.GetGuildIdAsync(this._legendsId)).ReturnsAsync(LegendsRuntimeId);
        guildServer.Setup(s => s.GetPersistentGuildNameAsync(this._legendsId)).ReturnsAsync("Legends");
        IPlayerContext CreateContext() => this._persistence.CreateNewPlayerContext(new GameConfiguration());
        return new DiscordChatBridge(
            this._settings,
            new GuildChatBindingStore(CreateContext),
            this.CreateLinkService(),
            guildServer.Object,
            CreateContext,
            this._persistence.CreateNewGuildContext,
            () => this._publisher.Object,
            NullLogger<DiscordChatBridge>.Instance);
    }

    private async Task<(Guid AccountId, Guid CharacterId)> CreateAccountAsync(string loginName, string characterName)
    {
        using var context = this._persistence.CreateNewPlayerContext(new GameConfiguration());
        var account = context.CreateNew<Account>();
        account.LoginName = loginName;
        var character = context.CreateNew<Character>();
        character.Name = characterName;
        account.Characters.Add(character);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return (account.GetId(), character.GetId());
    }
}
