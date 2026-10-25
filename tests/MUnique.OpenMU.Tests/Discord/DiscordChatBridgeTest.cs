// <copyright file="DiscordChatBridgeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

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
    /// Tests that a saved character name does not authorize its former owner after a transfer.
    /// </summary>
    [TestCase(GuildChatScope.Guild)]
    [TestCase(GuildChatScope.Alliance)]
    public async Task FormerCharacterOwnerCannotChangeBindingsAsync(GuildChatScope scope)
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, scope, ServerId, ChannelId, false).ConfigureAwait(false);
        using (var context = this._persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var previousOwner = await context.GetAccountByLoginNameAsync("hero").ConfigureAwait(false);
            var newOwner = await context.GetAccountByLoginNameAsync("outsider").ConfigureAwait(false);
            var character = previousOwner!.Characters.Single(c => c.Name == "Hero");
            previousOwner.Characters.Remove(character);
            newOwner!.Characters.Add(character);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var (result, _) = await bridge.BindAsync(GuildMasterUser, scope, ServerId, ChannelId + 1, false).ConfigureAwait(false);
        Assert.That(result, Is.EqualTo(DiscordChatBindResult.NotGuildMaster));
        Assert.That(await bridge.UnbindAsync(ChannelId, GuildMasterUser, false).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.NotGuildMaster));
        Assert.That(bridge.Store.GetByChannel(ChannelId), Is.Not.Null);
        Assert.That(bridge.Store.GetByChannel(ChannelId + 1), Is.Null);
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

        Assert.That(result, Is.EqualTo((DiscordChatPostResult.Sent, "Elf", "Hello guild")));
        this._publisher.Verify(p => p.GuildMessageAsync(LegendsRuntimeId, "@Elf", "Hello guild"), Times.Once);
    }

    /// <summary>
    /// Tests the reasons why a message isn't sent to the game.
    /// </summary>
    [Test]
    public async Task MessagesAreRejectedAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        Assert.That((await bridge.PostToGameAsync(ChannelId + 5, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.NotBridged));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, 99, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.NotLinked));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, OutsiderUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.NotMember));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "<@123>").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.Empty));
        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.Sent));

        using (var context = this._persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = await context.GetAccountByLoginNameAsync("elf").ConfigureAwait(false);
            account!.ChatBanUntil = DateTime.UtcNow.AddHours(1);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        Assert.That((await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatPostResult.ChatBanned));
    }

    /// <summary>
    /// Tests that the flood limit is checked before anything else, so that spamming doesn't cause queries to the database.
    /// </summary>
    [Test]
    public async Task FloodLimitIsCheckedFirstAsync()
    {
        this._settings.MaximumMessagesPerMinute = 1;
        var bridge = this.CreateBridge();

        var first = await bridge.PostToGameAsync(WorldChannelId, WorldChannelId, 99, "Hi").ConfigureAwait(false);
        var second = await bridge.PostToGameAsync(WorldChannelId, WorldChannelId, 99, "Hi").ConfigureAwait(false);

        Assert.That(first.Result, Is.EqualTo(DiscordChatPostResult.NotLinked));
        Assert.That(second.Result, Is.EqualTo(DiscordChatPostResult.TooFast));
    }

    /// <summary>
    /// Tests that a plugin of the server, e.g. a filter, can stop a message from Discord.
    /// </summary>
    [Test]
    public async Task FilterPlugInStopsMessageAsync()
    {
        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        var filter = new Mock<IExternalChatMessageReceivedPlugIn>();
        filter.Setup(f => f.ExternalChatMessageReceived("Outsider", "bad word", GameChatChannel.World, It.IsAny<CancelEventArgs>()))
            .Callback<string, string, GameChatChannel, CancelEventArgs>((_, _, _, e) => e.Cancel = true);
        plugInManager.RegisterPlugInAtPlugInPoint(filter.Object);

        var result = await this.CreateBridge(plugInManager).PostToGameAsync(WorldChannelId, WorldChannelId, OutsiderUser, "bad word").ConfigureAwait(false);

        Assert.That(result.Result, Is.EqualTo(DiscordChatPostResult.Filtered));
        this._publisher.Verify(p => p.WorldChatMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

    /// <summary>
    /// Tests that the guilds whose guild master is linked get a hosted channel, unless their chat is already bound,
    /// and that a guild master who isn't linked doesn't get one.
    /// </summary>
    [Test]
    public async Task GuildsOfLinkedGuildMastersGetHostedChannelsAsync()
    {
        var bridge = this.CreateBridge();
        var targets = await bridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false);
        Assert.That(targets.Select(t => t.CharacterName), Is.EquivalentTo(new[] { "Hero", "Ally" }));
        Assert.That(targets.Single(t => t.CharacterName == "Hero"), Is.EqualTo(new GuildChatTarget(this._legendsId, "Legends", "Hero")));

        // A bound chat, also on another Discord server, keeps its binding.
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);
        await this.CreateLinkService().UnlinkUserAsync(AccountLinkService.DiscordProvider, AllyUser.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        Assert.That(await bridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false), Is.Empty);

        // Binding it to the created channel happens only once.
        await this.LinkAgainAsync("ally", "Ally", AllyUser).ConfigureAwait(false);
        var allies = (await bridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false)).Single();
        Assert.That(await bridge.BindHostedAsync(allies, ServerId, ChannelId + 5).ConfigureAwait(false), Is.True);
        Assert.That(bridge.Store.GetByChannel(ChannelId + 5), Is.Not.Null.And.Property(nameof(GuildChatBinding.IsHosted)).True);
        Assert.That(await bridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false), Is.Empty);
        Assert.That(await bridge.BindHostedAsync(allies, ServerId, ChannelId + 6).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that an automatic binding doesn't replace a binding which the guild master made in the meantime,
    /// e.g. while the bot created the channel.
    /// </summary>
    [Test]
    public async Task AutomaticBindingDoesNotReplaceBindingOfGuildMasterAsync()
    {
        var bridge = this.CreateBridge();
        var legends = (await bridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false)).Single(t => t.GuildId == this._legendsId);

        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId + 1, ChannelId, false).ConfigureAwait(false);
        Assert.That(await bridge.BindHostedAsync(legends, ServerId, ChannelId + 5).ConfigureAwait(false), Is.False);

        var binding = bridge.Store.Get(this._legendsId, GuildChatScope.Guild);
        Assert.That(binding?.ExternalChannelId, Is.EqualTo(ChannelId.ToString(CultureInfo.InvariantCulture)));
        Assert.That(binding?.IsHosted, Is.False);
        Assert.That(bridge.Store.GetByChannel(ChannelId + 5), Is.Null);
    }

    /// <summary>
    /// Tests that the binding of a deleted hosted channel is only removed while it still references the channel,
    /// also when it happens at the same time as a new binding.
    /// </summary>
    [Test]
    public async Task StaleHostedBindingIsRemovedConditionallyAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);
        Assert.That(await bridge.RemoveHostedBindingAsync(ChannelId).ConfigureAwait(false), Is.False, "a binding on another Discord server isn't hosted");
        Assert.That(bridge.Store.GetByChannel(ChannelId), Is.Not.Null);

        await bridge.UnbindAsync(ChannelId, GuildMasterUser, false).ConfigureAwait(false);
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId + 1, true).ConfigureAwait(false);

        // The old channel is gone, while the guild master binds the chat again.
        var removal = bridge.RemoveHostedBindingAsync(ChannelId + 1).AsTask();
        var rebinding = bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId + 2, true).AsTask();
        await Task.WhenAll(removal, rebinding).ConfigureAwait(false);

        // Either order ends with at most one binding, which isn't the deleted channel, or the rebinding was refused before the removal.
        Assert.That(bridge.Store.GetByChannel(ChannelId + 1), Is.Null);
        var binding = bridge.Store.Get(this._legendsId, GuildChatScope.Guild);
        Assert.That(binding is null || binding.ExternalChannelId == (ChannelId + 2).ToString(CultureInfo.InvariantCulture), Is.True);
        Assert.That(bridge.Store.Bindings.Count(b => b.GuildId == this._legendsId && b.Scope == GuildChatScope.Guild), Is.LessThanOrEqualTo(1));
        Assert.That(await bridge.RemoveHostedBindingAsync(ChannelId + 1).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that the guild master rights need the guild master character to be selected for the link,
    /// like <c>/guildchat</c>, and not just any character of the same account.
    /// </summary>
    [Test]
    public async Task GuildMasterRightsNeedTheGuildMasterCharacterAsync()
    {
        using (var context = this._persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = await context.GetAccountByLoginNameAsync("hero").ConfigureAwait(false);
            var alt = context.CreateNew<Character>();
            alt.Name = "HeroAlt";
            account!.Characters.Add(alt);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var linkService = this.CreateLinkService();
        var userText = GuildMasterUser.ToString(CultureInfo.InvariantCulture);
        Assert.That(await linkService.SelectCharacterAsync(AccountLinkService.DiscordProvider, userText, "HeroAlt").ConfigureAwait(false), Is.EqualTo(CharacterSelectionResult.Selected));

        var bridge = this.CreateBridge();
        await bridge.BindAsync(AllyUser, GuildChatScope.Guild, ServerId, ChannelId + 1, true).ConfigureAwait(false);
        Assert.That((await bridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false)).Select(t => t.GuildId), Does.Not.Contain(this._legendsId));
        Assert.That((await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, true).ConfigureAwait(false)).Result, Is.EqualTo(DiscordChatBindResult.NotGuildMaster));

        await linkService.SelectCharacterAsync(AccountLinkService.DiscordProvider, userText, "Hero").ConfigureAwait(false);
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, true).ConfigureAwait(false);
        await linkService.SelectCharacterAsync(AccountLinkService.DiscordProvider, userText, "HeroAlt").ConfigureAwait(false);
        Assert.That(await bridge.GetModeratorsAsync(bridge.Store.Bindings).ConfigureAwait(false), Does.Not.ContainKey(this._legendsId));
    }

    /// <summary>
    /// Tests that the linked guild master moderates the channel, and that the moderation and the right to change
    /// the binding follow a change of the guild master.
    /// </summary>
    [Test]
    public async Task ModerationFollowsTheGuildMasterAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, true).ConfigureAwait(false);
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Alliance, ServerId, ChannelId + 1, true).ConfigureAwait(false);

        var moderators = await bridge.GetModeratorsAsync(bridge.Store.Bindings).ConfigureAwait(false);
        Assert.That(moderators, Is.EquivalentTo(new Dictionary<Guid, ulong> { [this._legendsId] = GuildMasterUser }));

        await this.SetPositionAsync("Hero", GuildPosition.NormalMember).ConfigureAwait(false);
        await this.SetPositionAsync("Elf", GuildPosition.GuildMaster).ConfigureAwait(false);

        moderators = await bridge.GetModeratorsAsync(bridge.Store.Bindings).ConfigureAwait(false);
        Assert.That(moderators, Is.EquivalentTo(new Dictionary<Guid, ulong> { [this._legendsId] = MemberUser }));
        Assert.That(await bridge.GetLinkedMembersAsync(bridge.Store.GetByChannel(ChannelId)!).ConfigureAwait(false), Does.Contain(GuildMasterUser), "the former master stays a member");

        // The binding stays, but only the new guild master can change it.
        Assert.That(bridge.Store.GetByChannel(ChannelId), Is.Not.Null);
        Assert.That(await bridge.UnbindAsync(ChannelId, GuildMasterUser, false).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.NotGuildMaster));
        Assert.That(await bridge.UnbindAsync(ChannelId, MemberUser, false).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.Success));
    }

    /// <summary>
    /// Tests that a bound chat isn't moved silently to another channel, but can be bound again after unbinding it.
    /// </summary>
    [Test]
    public async Task BoundChatIsNotMovedSilentlyAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);

        var (otherServerResult, target) = await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId + 1, ChannelId + 2, false).ConfigureAwait(false);
        Assert.That(otherServerResult, Is.EqualTo(DiscordChatBindResult.AlreadyBound));
        Assert.That(target?.GuildId, Is.EqualTo(this._legendsId));
        Assert.That(bridge.Store.Get(this._legendsId, GuildChatScope.Guild)?.ExternalChannelId, Is.EqualTo(ChannelId.ToString(CultureInfo.InvariantCulture)));

        var (sameChannelResult, _) = await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);
        Assert.That(sameChannelResult, Is.EqualTo(DiscordChatBindResult.Success), "binding the same channel again changes nothing");

        // A manager of the old channel, who isn't the guild master, can release it, e.g. the administrator of that Discord server.
        Assert.That(await bridge.UnbindAsync(ChannelId, OutsiderUser, false).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.NotGuildMaster));
        Assert.That(await bridge.UnbindAsync(ChannelId, OutsiderUser, true).ConfigureAwait(false), Is.EqualTo(DiscordChatBindResult.Success));
        var (movedResult, _) = await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId + 1, ChannelId + 2, false).ConfigureAwait(false);
        Assert.That(movedResult, Is.EqualTo(DiscordChatBindResult.Success));
    }

    /// <summary>
    /// Tests that the bindings of a Discord server are removed, e.g. when the bot is removed from it,
    /// and that a kicked member can't write into the chat anymore.
    /// </summary>
    [Test]
    public async Task LifecycleRemovesAccessAsync()
    {
        var bridge = this.CreateBridge();
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Guild, ServerId, ChannelId, false).ConfigureAwait(false);
        await bridge.BindAsync(GuildMasterUser, GuildChatScope.Alliance, ServerId, ChannelId + 1, false).ConfigureAwait(false);

        await this.KickAsync("Elf").ConfigureAwait(false);
        var (kickedResult, _, _) = await bridge.PostToGameAsync(ChannelId, WorldChannelId, MemberUser, "Hi").ConfigureAwait(false);
        Assert.That(kickedResult, Is.EqualTo(DiscordChatPostResult.NotMember));
        Assert.That(await bridge.GetLinkedMembersAsync(bridge.Store.GetByChannel(ChannelId)!).ConfigureAwait(false), Does.Not.Contain(MemberUser));

        Assert.That(await bridge.Store.RemoveAsync(null, ServerId).ConfigureAwait(false), Is.EqualTo(2));
        var (removedResult, _, _) = await bridge.PostToGameAsync(ChannelId, WorldChannelId, GuildMasterUser, "Hi").ConfigureAwait(false);
        Assert.That(removedResult, Is.EqualTo(DiscordChatPostResult.NotBridged));
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

    private async Task LinkAgainAsync(string loginName, string characterName, ulong userId)
    {
        using var context = this._persistence.CreateNewPlayerContext(new GameConfiguration());
        var accountId = (await context.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false))!.GetId();
        await LinkAsync(this.CreateLinkService(), accountId, characterName, userId).ConfigureAwait(false);
    }

    private async Task SetPositionAsync(string characterName, GuildPosition position)
    {
        using var context = this._persistence.CreateNewGuildContext();
        var membership = await context.GetGuildMembershipAsync(characterName).ConfigureAwait(false);
        var member = (await context.GetAsync<GuildMember>().ConfigureAwait(false)).Single(m => m.Id == membership!.CharacterId);
        member.Status = position;
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task KickAsync(string characterName)
    {
        using var context = this._persistence.CreateNewGuildContext();
        var membership = await context.GetGuildMembershipAsync(characterName).ConfigureAwait(false);
        var guild = (await context.GetAsync<DataModel.Entities.Guild>().ConfigureAwait(false)).Single(g => g.GetId() == membership!.GuildId);
        var member = guild.Members.Single(m => m.Id == membership!.CharacterId);
        guild.Members.Remove(member);
        await context.DeleteAsync(member).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private AccountLinkService CreateLinkService() => new(() => this._persistence.CreateNewPlayerContext(new GameConfiguration()));

    private DiscordChatBridge CreateBridge(PlugInManager? plugInManager = null)
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
            NullLogger<DiscordChatBridge>.Instance,
            plugInManager);
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
