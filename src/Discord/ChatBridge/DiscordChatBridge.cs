// <copyright file="DiscordChatBridge.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The chat bridge between the game and Discord: It mirrors the guild, alliance and world chat
/// to Discord channels and back, and manages the bindings of the chats of guilds to Discord channels.
/// </summary>
/// <remarks>
/// Only Discord users who are linked to a game account can write into the game, as the selected character
/// of their account. They have to be a member of the guild or alliance, and the chat ban of the account applies.
/// The messages go through the <see cref="IExternalChatMessageReceivedPlugIn"/>s, e.g. filters, instead of the
/// <see cref="IChatMessageReceivedPlugIn"/>s, because there is no player who sends them.
/// Only the guild master can bind the chat of a guild, and the master of an alliance the chat of the alliance.
/// </remarks>
public sealed class DiscordChatBridge
{
    private const string Provider = AccountLinkService.DiscordProvider;

    private readonly DiscordChatBridgeSettings _settings;
    private readonly GuildChatBindingStore _store;
    private readonly AccountLinkService _linkService;
    private readonly IGuildServer _guildServer;
    private readonly Func<IPlayerContext> _createPlayerContext;
    private readonly Func<IGuildServerContext> _createGuildContext;
    private readonly Func<IEventPublisher?> _getEventPublisher;
    private readonly ILogger<DiscordChatBridge> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly DiscordFloodGuard _floodGuard;
    private readonly PlugInManager? _plugInManager;

    /// <summary>
    /// The lock for changing the bindings, so that e.g. an automatic binding doesn't replace a binding of the guild master.
    /// </summary>
    private readonly SemaphoreSlim _bindingLock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordChatBridge"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="store">The store of the bindings.</param>
    /// <param name="linkService">The service of the account links.</param>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="createPlayerContext">The function which creates a player persistence context.</param>
    /// <param name="createGuildContext">The function which creates a guild persistence context.</param>
    /// <param name="getEventPublisher">The function which gets the event publisher, through which the messages are sent to the game servers.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="plugInManager">The plugin manager, for the <see cref="IExternalChatMessageReceivedPlugIn"/>s.</param>
    /// <param name="timeProvider">The time provider.</param>
    public DiscordChatBridge(
        DiscordChatBridgeSettings settings,
        GuildChatBindingStore store,
        AccountLinkService linkService,
        IGuildServer guildServer,
        Func<IPlayerContext> createPlayerContext,
        Func<IGuildServerContext> createGuildContext,
        Func<IEventPublisher?> getEventPublisher,
        ILogger<DiscordChatBridge> logger,
        PlugInManager? plugInManager = null,
        TimeProvider? timeProvider = null)
    {
        this._settings = settings;
        this._store = store;
        this._linkService = linkService;
        this._guildServer = guildServer;
        this._createPlayerContext = createPlayerContext;
        this._createGuildContext = createGuildContext;
        this._getEventPublisher = getEventPublisher;
        this._logger = logger;
        this._plugInManager = plugInManager;
        this._timeProvider = timeProvider ?? TimeProvider.System;
        this._floodGuard = new DiscordFloodGuard(settings.MaximumMessagesPerMinute, this._timeProvider);
    }

    /// <summary>
    /// Gets the store of the bindings.
    /// </summary>
    public GuildChatBindingStore Store => this._store;

    /// <summary>
    /// Gets the name of a guild.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    /// <returns>The name, or <c>null</c>, if the guild doesn't exist.</returns>
    public ValueTask<string?> GetGuildNameAsync(Guid guildId) => this._guildServer.GetPersistentGuildNameAsync(guildId);

    /// <summary>
    /// Gets the Discord channel and the text of a chat message of the game.
    /// </summary>
    /// <param name="chatMessage">The chat message.</param>
    /// <param name="worldChatChannelId">The identifier of the channel of the world chat, if there is one.</param>
    /// <returns>The channel and the text; or <c>null</c>, if the chat isn't bound.</returns>
    public async ValueTask<(ulong ChannelId, string Text)?> GetDiscordMessageAsync(ChatMessageEvent chatMessage, ulong? worldChatChannelId)
    {
        ulong? channelId = chatMessage.Channel switch
        {
            GameChatChannel.World => worldChatChannelId,
            GameChatChannel.Guild => GetChannelId(await this._guildServer.GetPersistentGuildIdAsync(chatMessage.GuildId).ConfigureAwait(false), GuildChatScope.Guild),
            GameChatChannel.Alliance => GetChannelId(await this._guildServer.GetPersistentAllianceMasterGuildIdAsync(chatMessage.GuildId).ConfigureAwait(false), GuildChatScope.Alliance),
            _ => null,
        };

        return channelId is { } id ? (id, DiscordChatText.ToDiscord(chatMessage.Sender, chatMessage.Message)) : null;

        ulong? GetChannelId(Guid? guildId, GuildChatScope scope)
        {
            return guildId is { } g && this._store.Get(g, scope) is { } binding
                   && ulong.TryParse(binding.ExternalChannelId, NumberStyles.None, CultureInfo.InvariantCulture, out var bindingChannelId)
                ? bindingChannelId
                : null;
        }
    }

    /// <summary>
    /// Sends a message of a Discord user to the chat of the game, which is bound to the channel.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="worldChatChannelId">The identifier of the channel of the world chat, if there is one.</param>
    /// <param name="userId">The identifier of the Discord user.</param>
    /// <param name="text">The text of the message.</param>
    /// <returns>The result, the name of the character as which the user wrote, and the message as it was sent to the game.</returns>
    public async ValueTask<(DiscordChatPostResult Result, string? CharacterName, string? Message)> PostToGameAsync(ulong channelId, ulong? worldChatChannelId, ulong userId, string text)
    {
        var binding = this._store.GetByChannel(channelId);
        if (binding is null && channelId != worldChatChannelId)
        {
            return (DiscordChatPostResult.NotBridged, null, null);
        }

        // Checked first, so that spamming doesn't cause queries to the database.
        if (!this._floodGuard.TryRegister(userId))
        {
            return (DiscordChatPostResult.TooFast, null, null);
        }

        if (await this._linkService.GetLinkByUserAsync(Provider, GuildChatBindingStore.ToText(userId)).ConfigureAwait(false) is not { ExternalUserId: not null } link)
        {
            return (DiscordChatPostResult.NotLinked, null, null);
        }

        if (link.CharacterName is not { } characterName)
        {
            return (DiscordChatPostResult.NoCharacter, null, null);
        }

        using (var context = this._createPlayerContext())
        {
            // The character might have been deleted or moved to another account in the meantime.
            if (await context.GetAccountIdByCharacterNameAsync(characterName).ConfigureAwait(false) != link.AccountId)
            {
                return (DiscordChatPostResult.NoCharacter, null, null);
            }

            if (await context.GetAccountChatBanUntilAsync(link.AccountId).ConfigureAwait(false) is { } chatBanUntil
                && chatBanUntil > this._timeProvider.GetUtcNow().UtcDateTime)
            {
                return (DiscordChatPostResult.ChatBanned, characterName, null);
            }
        }

        if (binding is not null && !await this.IsMemberAsync(characterName, binding).ConfigureAwait(false))
        {
            return (DiscordChatPostResult.NotMember, characterName, null);
        }

        var message = DiscordChatText.ToGame(text, this._settings.MaximumMessageLength);
        if (message.Length == 0)
        {
            return (DiscordChatPostResult.Empty, characterName, null);
        }

        var channel = binding is null ? GameChatChannel.World : binding.Scope == GuildChatScope.Alliance ? GameChatChannel.Alliance : GameChatChannel.Guild;
        var eventArgs = new CancelEventArgs();
        this._plugInManager?.GetPlugInPoint<IExternalChatMessageReceivedPlugIn>()?.ExternalChatMessageReceived(characterName, message, channel, eventArgs);
        if (eventArgs.Cancel)
        {
            return (DiscordChatPostResult.Filtered, characterName, null);
        }

        if (this._getEventPublisher() is not { } publisher)
        {
            this._logger.LogWarning("A chat message from Discord couldn't be sent to the game, because the event publisher isn't available.");
            return (DiscordChatPostResult.NotBridged, characterName, null);
        }

        var sender = DiscordChatText.ToGameSender(characterName);
        if (binding is null)
        {
            await publisher.WorldChatMessageAsync(sender, message).ConfigureAwait(false);
        }
        else if (await this._guildServer.GetGuildIdAsync(binding.GuildId).ConfigureAwait(false) is var guildId and not 0)
        {
            // Without non-persistent identifier, no member of the guild is online.
            if (binding.Scope == GuildChatScope.Alliance)
            {
                await publisher.AllianceMessageAsync(guildId, sender, message).ConfigureAwait(false);
            }
            else
            {
                await publisher.GuildMessageAsync(guildId, sender, message).ConfigureAwait(false);
            }
        }

        return (DiscordChatPostResult.Sent, characterName, message);
    }

    /// <summary>
    /// Gets the guild or alliance whose chat the Discord user can bind, because its character is the guild master.
    /// </summary>
    /// <param name="userId">The identifier of the Discord user.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>The result, and the guild or alliance, if the user can bind its chat.</returns>
    public async ValueTask<(DiscordChatBindResult Result, GuildChatTarget? Target)> GetBindableTargetAsync(ulong userId, GuildChatScope scope)
    {
        if (await this._linkService.GetLinkByUserAsync(Provider, GuildChatBindingStore.ToText(userId)).ConfigureAwait(false) is not { ExternalUserId: not null } link)
        {
            return (DiscordChatBindResult.NotLinked, null);
        }

        if (link.CharacterName is not { } characterName)
        {
            return (DiscordChatBindResult.NotGuildMaster, null);
        }

        // A saved name can outlive a character transfer or be reused after deletion.
        using (var playerContext = this._createPlayerContext())
        {
            if (await playerContext.GetAccountIdByCharacterNameAsync(characterName).ConfigureAwait(false) != link.AccountId)
            {
                return (DiscordChatBindResult.NotGuildMaster, null);
            }
        }

        using var guildContext = this._createGuildContext();
        if (await guildContext.GetGuildMembershipAsync(characterName).ConfigureAwait(false) is not { Position: GuildPosition.GuildMaster } membership)
        {
            return (DiscordChatBindResult.NotGuildMaster, null);
        }

        if (scope == GuildChatScope.Alliance
            && ((membership.AllianceMasterGuildId is { } master && master != membership.GuildId)
                || (await guildContext.GetAlliancesAsync(membership.GuildId).ConfigureAwait(false)).Count == 0))
        {
            return (DiscordChatBindResult.NotAllianceMaster, null);
        }

        var guildName = await this._guildServer.GetPersistentGuildNameAsync(membership.GuildId).ConfigureAwait(false) ?? characterName;
        return (DiscordChatBindResult.Success, new GuildChatTarget(membership.GuildId, guildName, characterName));
    }

    /// <summary>
    /// Binds the chat of the guild or alliance of the Discord user to a channel.
    /// </summary>
    /// <param name="userId">The identifier of the Discord user, whose character has to be the guild master.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="serverId">The identifier of the Discord server.</param>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="isHosted">A value indicating whether the channel is on the Discord server of the game server.</param>
    /// <returns>The result, and the bound guild or alliance.</returns>
    public async ValueTask<(DiscordChatBindResult Result, GuildChatTarget? Target)> BindAsync(ulong userId, GuildChatScope scope, ulong serverId, ulong channelId, bool isHosted)
    {
        if (this.CheckAllowed(serverId, isHosted) is { } notAllowed)
        {
            return (notAllowed, null);
        }

        var (result, target) = await this.GetBindableTargetAsync(userId, scope).ConfigureAwait(false);
        if (target is null)
        {
            return (result, null);
        }

        await this._bindingLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (this._store.GetByChannel(channelId) is { } existing
                && (existing.GuildId != target.GuildId || existing.Scope != scope))
            {
                return (DiscordChatBindResult.ChannelInUse, null);
            }

            // Moving the chat silently to another channel could take it away from the guild, e.g. by a mistake.
            if (this._store.Get(target.GuildId, scope) is { } current && current.ExternalChannelId != GuildChatBindingStore.ToText(channelId))
            {
                return (DiscordChatBindResult.AlreadyBound, target);
            }

            await this._store.SetAsync(target.GuildId, scope, serverId, channelId, isHosted, target.CharacterName, this._timeProvider.GetUtcNow().UtcDateTime).ConfigureAwait(false);
        }
        finally
        {
            this._bindingLock.Release();
        }

        this._logger.LogInformation("The {scope} chat of {guild} was bound to the Discord channel {channelId} by {character}.", scope, target.GuildName, channelId, target.CharacterName);
        return (DiscordChatBindResult.Success, target);
    }

    /// <summary>
    /// Binds the chat of a guild to its channel on the Discord server of the game server, which the bot created for it,
    /// unless the chat or the channel was bound in the meantime, e.g. by the guild master.
    /// </summary>
    /// <param name="target">The guild.</param>
    /// <param name="serverId">The identifier of the Discord server.</param>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <returns><c>true</c>, if it was bound; <c>false</c>, if the chat or the channel is already bound.</returns>
    public async ValueTask<bool> BindHostedAsync(GuildChatTarget target, ulong serverId, ulong channelId)
    {
        await this._bindingLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (this._store.Get(target.GuildId, GuildChatScope.Guild) is not null || this._store.GetByChannel(channelId) is not null)
            {
                return false;
            }

            await this._store.SetAsync(target.GuildId, GuildChatScope.Guild, serverId, channelId, true, target.CharacterName, this._timeProvider.GetUtcNow().UtcDateTime).ConfigureAwait(false);
        }
        finally
        {
            this._bindingLock.Release();
        }

        this._logger.LogInformation("The guild chat of {guild} was bound to the hosted Discord channel {channelId}, because its guild master {character} is linked.", target.GuildName, channelId, target.CharacterName);
        return true;
    }

    /// <summary>
    /// Removes the binding of a hosted channel which doesn't exist anymore, e.g. because it was deleted while the bot wasn't connected.
    /// It only removes the binding, if it still references the channel.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <returns><c>true</c>, if a binding was removed.</returns>
    public async ValueTask<bool> RemoveHostedBindingAsync(ulong channelId)
    {
        await this._bindingLock.WaitAsync().ConfigureAwait(false);
        try
        {
            return this._store.GetByChannel(channelId) is { IsHosted: true }
                   && await this._store.RemoveAsync(channelId, null).ConfigureAwait(false) > 0;
        }
        finally
        {
            this._bindingLock.Release();
        }
    }

    /// <summary>
    /// Gets the guilds which get a channel on the Discord server of the game server, because their guild master is linked
    /// to a Discord user, as the selected character like for <c>/guildchat</c>, and the guild chat isn't bound yet.
    /// </summary>
    /// <returns>The guilds.</returns>
    public async ValueTask<IReadOnlyList<GuildChatTarget>> GetGuildsWithoutHostedChannelAsync()
    {
        IReadOnlyDictionary<Guid, string> masters;
        using (var guildContext = this._createGuildContext())
        {
            masters = await guildContext.GetGuildMasterNamesAsync().ConfigureAwait(false);
        }

        var unbound = masters.Where(master => this._store.Get(master.Key, GuildChatScope.Guild) is null).ToList();
        if (unbound.Count == 0)
        {
            return [];
        }

        var linkedMasters = await this.GetLinkedCharactersAsync(unbound.Select(master => master.Value).ToList()).ConfigureAwait(false);
        var targets = new List<GuildChatTarget>();
        foreach (var (guildId, masterName) in unbound.Where(master => linkedMasters.ContainsKey(master.Value)))
        {
            var guildName = await this._guildServer.GetPersistentGuildNameAsync(guildId).ConfigureAwait(false) ?? masterName;
            targets.Add(new GuildChatTarget(guildId, guildName, masterName));
        }

        return targets;
    }

    /// <summary>
    /// Gets the Discord users who moderate the channels of bindings: the users who are linked to the guild masters,
    /// as their selected character like for <c>/guildchat</c>.
    /// For an alliance, it's the guild master of the master guild of the alliance.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>The identifiers of the Discord users by the identifiers of the guilds of the bindings.</returns>
    public async ValueTask<IReadOnlyDictionary<Guid, ulong>> GetModeratorsAsync(IReadOnlyCollection<GuildChatBinding> bindings)
    {
        if (bindings.Count == 0)
        {
            return new Dictionary<Guid, ulong>();
        }

        IReadOnlyDictionary<Guid, string> masters;
        using (var guildContext = this._createGuildContext())
        {
            masters = await guildContext.GetGuildMasterNamesAsync().ConfigureAwait(false);
        }

        var bindingMasters = bindings
            .Select(binding => binding.GuildId)
            .Distinct()
            .Where(masters.ContainsKey)
            .ToDictionary(guildId => guildId, guildId => masters[guildId]);
        var linkedMasters = await this.GetLinkedCharactersAsync(bindingMasters.Values.Distinct().ToList()).ConfigureAwait(false);
        return bindingMasters
            .Where(master => linkedMasters.ContainsKey(master.Value))
            .ToDictionary(master => master.Key, master => linkedMasters[master.Value]);
    }

    /// <summary>
    /// Checks whether the binding mode and the allowed Discord servers allow a binding.
    /// </summary>
    /// <param name="serverId">The identifier of the Discord server.</param>
    /// <param name="isHosted">A value indicating whether the channel is on the Discord server of the game server.</param>
    /// <returns>The result, if it's not allowed; otherwise, <c>null</c>.</returns>
    public DiscordChatBindResult? CheckAllowed(ulong serverId, bool isHosted)
    {
        if (this._settings.BindingMode == (isHosted ? DiscordGuildChatBindingMode.GuildOwnedOnly : DiscordGuildChatBindingMode.HostedOnly))
        {
            return DiscordChatBindResult.ModeNotAllowed;
        }

        if (!isHosted && this._settings.AllowedDiscordServerIds is { Count: > 0 } allowed && !allowed.Contains(serverId))
        {
            return DiscordChatBindResult.ServerNotAllowed;
        }

        return null;
    }

    /// <summary>
    /// Removes the binding of a channel. It's allowed for the guild master, and for users who manage the channel.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="userId">The identifier of the Discord user.</param>
    /// <param name="userManagesChannel">A value indicating whether the user has the permission to manage the channel.</param>
    /// <returns>The result.</returns>
    public async ValueTask<DiscordChatBindResult> UnbindAsync(ulong channelId, ulong userId, bool userManagesChannel)
    {
        if (this._store.GetByChannel(channelId) is not { } binding)
        {
            return DiscordChatBindResult.NotBound;
        }

        if (!userManagesChannel)
        {
            var (result, target) = await this.GetBindableTargetAsync(userId, binding.Scope).ConfigureAwait(false);
            if (target?.GuildId != binding.GuildId)
            {
                return result == DiscordChatBindResult.Success ? DiscordChatBindResult.NotGuildMaster : result;
            }
        }

        await this._bindingLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await this._store.RemoveAsync(channelId, null).ConfigureAwait(false);
        }
        finally
        {
            this._bindingLock.Release();
        }

        this._logger.LogInformation("The binding of the Discord channel {channelId} was removed by the user {userId}.", channelId, userId);
        return DiscordChatBindResult.Success;
    }

    /// <summary>
    /// Gets the Discord users who are linked to a member of the guild or alliance of a binding.
    /// They can see the hosted channel of the binding.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <returns>The identifiers of the Discord users.</returns>
    public async ValueTask<IReadOnlySet<ulong>> GetLinkedMembersAsync(GuildChatBinding binding)
    {
        var guildIds = new List<Guid> { binding.GuildId };
        var memberNames = new List<string>();
        using (var guildContext = this._createGuildContext())
        {
            if (binding.Scope == GuildChatScope.Alliance)
            {
                guildIds.AddRange((await guildContext.GetAlliancesAsync(binding.GuildId).ConfigureAwait(false)).Select(guild => guild.GetId()));
            }

            foreach (var guildId in guildIds.Distinct())
            {
                memberNames.AddRange((await guildContext.GetMemberNamesAsync(guildId).ConfigureAwait(false)).Values);
            }
        }

        // Two queries for all members, instead of two per member.
        using var context = this._createPlayerContext();
        var accountIds = (await context.GetAccountIdsByCharacterNamesAsync(memberNames.Distinct().ToList()).ConfigureAwait(false)).Values.Distinct().ToList();
        var links = await context.GetAccountExternalLinksAsync(Provider, accountIds).ConfigureAwait(false);
        return links
            .Select(link => ulong.TryParse(link.ExternalUserId, NumberStyles.None, CultureInfo.InvariantCulture, out var userId) ? userId : (ulong?)null)
            .OfType<ulong>()
            .ToHashSet();
    }

    /// <summary>
    /// Gets the Discord users who are linked as characters: their link has the character selected, and the account still owns it.
    /// It's the same rule as for the guild master in <see cref="GetBindableTargetAsync"/>.
    /// </summary>
    /// <param name="characterNames">The names of the characters.</param>
    /// <returns>The identifiers of the Discord users by the names of the characters.</returns>
    private async ValueTask<IReadOnlyDictionary<string, ulong>> GetLinkedCharactersAsync(IReadOnlyCollection<string> characterNames)
    {
        using var context = this._createPlayerContext();
        var accountIds = await context.GetAccountIdsByCharacterNamesAsync(characterNames).ConfigureAwait(false);
        var links = await context.GetAccountExternalLinksAsync(Provider, accountIds.Values.Distinct().ToList()).ConfigureAwait(false);
        var result = new Dictionary<string, ulong>();
        foreach (var link in links)
        {
            if (link.CharacterName is { } characterName
                && accountIds.TryGetValue(characterName, out var accountId)
                && accountId == link.AccountId
                && ulong.TryParse(link.ExternalUserId, NumberStyles.None, CultureInfo.InvariantCulture, out var userId))
            {
                result[characterName] = userId;
            }
        }

        return result;
    }

    private async ValueTask<bool> IsMemberAsync(string characterName, GuildChatBinding binding)
    {
        using var guildContext = this._createGuildContext();
        if (await guildContext.GetGuildMembershipAsync(characterName).ConfigureAwait(false) is not { } membership)
        {
            return false;
        }

        return membership.GuildId == binding.GuildId
               || (binding.Scope == GuildChatScope.Alliance && membership.AllianceMasterGuildId == binding.GuildId);
    }
}
