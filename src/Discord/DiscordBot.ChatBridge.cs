// <copyright file="DiscordBot.ChatBridge.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Threading;
using global::Discord;
using global::Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.Discord.Provisioning;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The part of the <see cref="DiscordBot"/> which runs the chat bridge between the game and Discord.
/// </summary>
public sealed partial class DiscordBot
{
    /// <summary>
    /// The number of status updates after which the channels of the guilds are synchronized with the members of the guilds.
    /// </summary>
    private const int HostedChannelSyncInterval = 10;

    /// <summary>
    /// The maximum number of chat messages per channel which wait to be sent.
    /// </summary>
    private const int MaximumQueuedChatMessages = 100;

    /// <summary>
    /// The maximum number of permission overwrites of a Discord channel.
    /// </summary>
    private const int MaximumPermissionOverwrites = 500;

    private readonly ConcurrentDictionary<ulong, DiscordChatQueue> _chatQueues = new();

    /// <summary>
    /// The lock for creating hosted channels, so that a guild doesn't get two channels.
    /// </summary>
    private readonly SemaphoreSlim _hostedChannelLock = new(1, 1);
    private int _maintenanceCount;

    /// <inheritdoc />
    public async Task SendTextAsync(ulong channelId, string text, CancellationToken cancellationToken)
    {
        await this._ready.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        if (await this.GetMessageChannelAsync(channelId).ConfigureAwait(false) is not { } channel)
        {
            this._logger.LogWarning("The Discord channel {channelId} of the chat isn't available for the bot.", channelId);
            return;
        }

        await channel.SendMessageAsync(text, allowedMentions: AllowedMentions.None).ConfigureAwait(false);
    }

    private static bool IsChatCommand(string commandName) => commandName is DiscordChatCommands.SayCommandName or DiscordChatCommands.GuildChatCommandName;

    /// <summary>
    /// Gets the name of the channel of a guild, e.g. <c>guild-legends</c>.
    /// </summary>
    private static string GetHostedChannelName(GuildChatScope scope, string guildName)
    {
        var name = new StringBuilder(scope == GuildChatScope.Alliance ? "alliance-" : "guild-");
        foreach (var character in guildName.ToLowerInvariant())
        {
            name.Append(char.IsLetterOrDigit(character) ? character : '-');
        }

        return name.ToString().TrimEnd('-');
    }

    private GatewayIntents GetGatewayIntents()
    {
        // Reading the messages requires the privileged intent for their content.
        return this._settings.ChatBridge.ReadMessages
            ? GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
            : GatewayIntents.Guilds;
    }

    private void RegisterChatEvents(DiscordSocketClient client)
    {
        if (this._settings.ChatBridge.ReadMessages)
        {
            client.MessageReceived += this.OnMessageReceivedAsync;
        }

        client.ChannelDestroyed += channel => this.RemoveBindingsAsync(channel.Id, null);
        client.LeftGuild += guild => this.RemoveBindingsAsync(null, guild.Id);
    }

    private IEnumerable<ApplicationCommandProperties> CreateChatCommands()
    {
        var say = DiscordChatCommands.SayDefinition;
        yield return new SlashCommandBuilder()
            .WithName(say.Name)
            .WithDescription(this._chatCommands.Text(say.DescriptionKey))
            .WithContextTypes(InteractionContextType.Guild)
            .AddOption(say.OptionName, ApplicationCommandOptionType.String, this._chatCommands.Text(say.OptionDescriptionKey!), isRequired: true)
            .Build();

        SlashCommandOptionBuilder SubCommand(string name, string descriptionKey) => new SlashCommandOptionBuilder()
            .WithName(name)
            .WithDescription(this._chatCommands.Text(descriptionKey))
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName(DiscordChatCommands.ScopeOptionName)
                .WithDescription(this._chatCommands.Text(nameof(Resources.Command_GuildChat_ScopeOption)))
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(false)
                .AddChoice(this._chatCommands.Text(nameof(Resources.Command_GuildChat_ScopeGuild)), DiscordChatCommands.GuildScope)
                .AddChoice(this._chatCommands.Text(nameof(Resources.Command_GuildChat_ScopeAlliance)), DiscordChatCommands.AllianceScope));

        yield return new SlashCommandBuilder()
            .WithName(DiscordChatCommands.GuildChatCommandName)
            .WithDescription(this._chatCommands.Text(nameof(Resources.Command_GuildChat_Description)))
            .WithContextTypes(InteractionContextType.Guild)
            .AddOption(SubCommand(DiscordChatCommands.BindCommandName, nameof(Resources.Command_GuildChat_Bind_Description)))
            .AddOption(SubCommand(DiscordChatCommands.CreateCommandName, nameof(Resources.Command_GuildChat_Create_Description)))
            .AddOption(SubCommand(DiscordChatCommands.UnbindCommandName, nameof(Resources.Command_GuildChat_Unbind_Description)))
            .Build();
    }

    private async Task ExecuteChatCommandAsync(SocketSlashCommand command)
    {
        try
        {
            await command.DeferAsync(ephemeral: true).ConfigureAwait(false);
            var answer = command.Data.Name == DiscordChatCommands.SayCommandName
                ? await this.SayAsync(command).ConfigureAwait(false)
                : await this.ExecuteGuildChatCommandAsync(command).ConfigureAwait(false);
            await command.FollowupAsync(embed: ToDiscordEmbed(answer), ephemeral: true, allowedMentions: AllowedMentions.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when answering the Discord command {command}.", command.Data.Name);
            await this.AnswerFailureAsync(command, true).ConfigureAwait(false);
        }
    }

    private async Task<DiscordEmbed> SayAsync(SocketSlashCommand command)
    {
        var text = command.Data.Options.FirstOrDefault()?.Value as string ?? string.Empty;
        var channelId = command.ChannelId ?? 0;
        var (result, characterName, sentMessage) = await this._chatBridge.PostToGameAsync(channelId, this._routing.WorldChatChannelId, command.User.Id, text).ConfigureAwait(false);
        if (result == DiscordChatPostResult.Sent)
        {
            // The command itself isn't visible to the others in the channel, so the bot shows the message, exactly as the game got it.
            this.EnqueueChatMessage(channelId, DiscordChatText.ToDiscord(characterName!, sentMessage!));
        }

        return this._chatCommands.CreatePostAnswer(result, characterName);
    }

    private async Task<DiscordEmbed> ExecuteGuildChatCommandAsync(SocketSlashCommand command)
    {
        var subCommand = command.Data.Options.First();
        var scope = DiscordChatCommands.ParseScope(subCommand.Options.FirstOrDefault(option => option.Name == DiscordChatCommands.ScopeOptionName)?.Value as string);
        if (this._client is not { } client
            || command.GuildId is not { } guildId
            || command.Channel is not IGuildChannel channel
            || command.User is not SocketGuildUser user)
        {
            return this._chatCommands.Answer(nameof(Resources.Bind_Title), nameof(Resources.Bind_NotInChannel));
        }

        var managesChannel = user.GetPermissions(channel).ManageChannel;
        var isHostedServer = this.GetRoutingGuild(client)?.Id == guildId;
        switch (subCommand.Name)
        {
            case DiscordChatCommands.BindCommandName when isHostedServer:
                return this._chatCommands.Answer(nameof(Resources.Bind_Title), nameof(Resources.Bind_UseCreate));
            case DiscordChatCommands.BindCommandName when !managesChannel:
                return this._chatCommands.Answer(nameof(Resources.Bind_Title), nameof(Resources.Bind_ManageChannelRequired));
            case DiscordChatCommands.BindCommandName:
                var (bindResult, target) = await this._chatBridge.BindAsync(user.Id, scope, guildId, channel.Id, false).ConfigureAwait(false);
                return this._chatCommands.CreateBindAnswer(
                    bindResult,
                    scope == GuildChatScope.Alliance ? nameof(Resources.Bind_AllianceBound) : nameof(Resources.Bind_GuildBound),
                    target,
                    bindResult == DiscordChatBindResult.AlreadyBound ? this.GetBoundChannelMention(target, scope) : MentionUtils.MentionChannel(channel.Id));
            case DiscordChatCommands.CreateCommandName:
                return await this.CreateHostedChannelAsync(client, user.Id, scope).ConfigureAwait(false);
            default:
                var unbindResult = await this._chatBridge.UnbindAsync(channel.Id, user.Id, managesChannel).ConfigureAwait(false);
                return this._chatCommands.CreateBindAnswer(unbindResult, nameof(Resources.Bind_Unbound), null, null);
        }
    }

    /// <summary>
    /// Creates the channel of a guild on the Discord server of the game server, and binds the chat of the guild to it.
    /// If it already exists, it's bound again.
    /// </summary>
    private async Task<DiscordEmbed> CreateHostedChannelAsync(DiscordSocketClient client, ulong userId, GuildChatScope scope)
    {
        if (this.GetRoutingGuild(client) is not { } guild)
        {
            return this._chatCommands.Answer(nameof(Resources.Bind_Title), nameof(Resources.Bind_HostedServerUnavailable));
        }

        if (this._chatBridge.CheckAllowed(guild.Id, true) is { } notAllowed)
        {
            return this._chatCommands.CreateBindAnswer(notAllowed, string.Empty, null, null);
        }

        var (result, target) = await this._chatBridge.GetBindableTargetAsync(userId, scope).ConfigureAwait(false);
        if (target is null)
        {
            return this._chatCommands.CreateBindAnswer(result, string.Empty, null, null);
        }

        IGuildChannel channel;
        DiscordChatBindResult bindResult;
        GuildChatTarget? boundTarget;
        await this._hostedChannelLock.WaitAsync().ConfigureAwait(false);
        try
        {
            // A channel on another Discord server, e.g. the one of the guild or the previous server of the bot, has to be unbound there first.
            if (this._chatBridge.Store.Get(target.GuildId, scope) is { } current
                && (!current.IsHosted || current.ExternalServerId != GuildChatBindingStore.ToText(guild.Id)))
            {
                return this._chatCommands.CreateBindAnswer(DiscordChatBindResult.AlreadyBound, string.Empty, target, this.GetBoundChannelMention(target, scope));
            }

            bool created;
            (channel, created) = await this.GetOrCreateHostedChannelAsync(client, guild, scope, target).ConfigureAwait(false);
            (bindResult, boundTarget) = await this._chatBridge.BindAsync(userId, scope, guild.Id, channel.Id, true).ConfigureAwait(false);
            if (bindResult != DiscordChatBindResult.Success && created)
            {
                // E.g. the chat was bound elsewhere in the meantime. The new channel would stay empty.
                await channel.DeleteAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            this._hostedChannelLock.Release();
        }

        var allMembersFit = true;
        if (bindResult == DiscordChatBindResult.Success && this._chatBridge.Store.GetByChannel(channel.Id) is { } binding)
        {
            var moderators = await this._chatBridge.GetModeratorsAsync([binding]).ConfigureAwait(false);
            allMembersFit = await this.SyncHostedChannelAsync(client, channel, binding, moderators.GetValueOrDefault(binding.GuildId)).ConfigureAwait(false);
        }

        var answer = this._chatCommands.CreateBindAnswer(bindResult, nameof(Resources.Bind_Created), boundTarget, MentionUtils.MentionChannel(channel.Id));
        return allMembersFit
            ? answer
            : answer with { Description = answer.Description + "\n" + DiscordCommands.GetText(this._chatCommands.Culture, nameof(Resources.Bind_TooManyMembers), MaximumPermissionOverwrites) };
    }

    /// <summary>
    /// Gets the hosted channel of a guild or alliance on this Discord server: the bound one, otherwise a new one.
    /// An existing channel which isn't bound isn't reused, even with the name of the guild, because others might see it.
    /// Only the bot can see a new channel, until the members are synchronized.
    /// The caller holds the <see cref="_hostedChannelLock"/>.
    /// </summary>
    /// <returns>The channel, and if it was created.</returns>
    private async Task<(IGuildChannel Channel, bool Created)> GetOrCreateHostedChannelAsync(DiscordSocketClient client, SocketGuild guild, GuildChatScope scope, GuildChatTarget target)
    {
        if (this._chatBridge.Store.Get(target.GuildId, scope) is { IsHosted: true } existing
            && existing.ExternalServerId == GuildChatBindingStore.ToText(guild.Id)
            && ulong.TryParse(existing.ExternalChannelId, NumberStyles.None, CultureInfo.InvariantCulture, out var existingId))
        {
            if (guild.GetTextChannel(existingId) is { } existingChannel)
            {
                return (existingChannel, false);
            }

            // The channel was deleted while the bot wasn't connected.
            await this._chatBridge.RemoveHostedBindingAsync(existingId).ConfigureAwait(false);
        }

        var categoryName = this._layout.Categories.FirstOrDefault(c => c.Key == DiscordServerLayout.GuildsCategoryKey)?.Name;
        var category = guild.CategoryChannels.FirstOrDefault(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        var channel = await guild.CreateTextChannelAsync(GetHostedChannelName(scope, target.GuildName), properties =>
        {
            properties.CategoryId = category?.Id;
            properties.PermissionOverwrites = new[]
            {
                new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny)),
                new Overwrite(client.CurrentUser.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow)),
            };
        }).ConfigureAwait(false);
        return (channel, true);
    }

    /// <summary>
    /// Creates the hosted channels of the guilds whose guild master is linked, if the settings allow it.
    /// It runs when a user is linked, and from time to time for e.g. new guilds and new guild masters.
    /// Guilds which already have a bound chat, also on another Discord server, keep it.
    /// </summary>
    private async Task CreateMissingHostedChannelsAsync()
    {
        if (!this._settings.ChatBridge.AutoCreateHostedChannels
            || this._client is not { } client
            || this.GetRoutingGuild(client) is not { } guild
            || this._chatBridge.CheckAllowed(guild.Id, true) is not null)
        {
            return;
        }

        try
        {
            foreach (var target in await this._chatBridge.GetGuildsWithoutHostedChannelAsync().ConfigureAwait(false))
            {
                IGuildChannel channel;
                await this._hostedChannelLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    // E.g. the guild master used /guildchat in the meantime.
                    if (this._chatBridge.Store.Get(target.GuildId, GuildChatScope.Guild) is not null)
                    {
                        continue;
                    }

                    (channel, var created) = await this.GetOrCreateHostedChannelAsync(client, guild, GuildChatScope.Guild, target).ConfigureAwait(false);
                    if (!await this._chatBridge.BindHostedAsync(target, guild.Id, channel.Id).ConfigureAwait(false))
                    {
                        // The guild master bound the chat elsewhere while the channel was created.
                        if (created)
                        {
                            await channel.DeleteAsync().ConfigureAwait(false);
                        }

                        continue;
                    }
                }
                finally
                {
                    this._hostedChannelLock.Release();
                }

                if (this._chatBridge.Store.GetByChannel(channel.Id) is { } binding)
                {
                    var moderators = await this._chatBridge.GetModeratorsAsync([binding]).ConfigureAwait(false);
                    await this.SyncHostedChannelAsync(client, channel, binding, moderators.GetValueOrDefault(binding.GuildId)).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "The hosted channels of guilds couldn't be created. Has the bot the permission to manage channels?");
        }
    }

    private string GetBoundChannelMention(GuildChatTarget? target, GuildChatScope scope)
    {
        return target is not null
               && this._chatBridge.Store.Get(target.GuildId, scope) is { } binding
               && ulong.TryParse(binding.ExternalChannelId, NumberStyles.None, CultureInfo.InvariantCulture, out var channelId)
            ? MentionUtils.MentionChannel(channelId)
            : "-";
    }

    private async ValueTask OnChatMessageAsync(ChatMessageEvent chatMessage)
    {
        try
        {
            if (await this._chatBridge.GetDiscordMessageAsync(chatMessage, this._routing.WorldChatChannelId).ConfigureAwait(false) is { } message)
            {
                this.EnqueueChatMessage(message.ChannelId, message.Text);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when mirroring a chat message of the game to Discord.");
        }
    }

    private void EnqueueChatMessage(ulong channelId, string text)
    {
        this._chatQueues
            .GetOrAdd(channelId, id => new DiscordChatQueue(id, this, this._logger, MaximumQueuedChatMessages))
            .Enqueue(text);
    }

    private async ValueTask StopChatQueuesAsync()
    {
        foreach (var queue in this._chatQueues.Values)
        {
            await queue.DisposeAsync().ConfigureAwait(false);
        }

        this._chatQueues.Clear();
    }

    private Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot
            || message.Author.IsWebhook
            || message is not SocketUserMessage userMessage
            || message.Channel is not SocketTextChannel channel
            || (this._chatBridge.Store.GetByChannel(channel.Id) is null && channel.Id != this._routing.WorldChatChannelId))
        {
            return Task.CompletedTask;
        }

        // The gateway shouldn't wait for the database.
        _ = Task.Run(async () =>
        {
            try
            {
                var (result, _, _) = await this._chatBridge.PostToGameAsync(channel.Id, this._routing.WorldChatChannelId, message.Author.Id, message.Content).ConfigureAwait(false);
                if (result != DiscordChatPostResult.Sent)
                {
                    // The user should notice that the message didn't reach the game. /say tells why.
                    await userMessage.AddReactionAsync(new Emoji("❌")).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "A Discord message couldn't be sent to the chat of the game.");
            }
        });

        return Task.CompletedTask;
    }

    private async Task RemoveBindingsAsync(ulong? channelId, ulong? serverId)
    {
        try
        {
            if (await this._chatBridge.Store.RemoveAsync(channelId, serverId).ConfigureAwait(false) is var count and > 0)
            {
                this._logger.LogInformation("Removed {count} binding(s) of guild chats, because the Discord channel {channelId} or server {serverId} isn't available anymore.", count, channelId, serverId);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when removing the bindings of guild chats.");
        }
    }

    private async ValueTask ReloadChatBindingsAsync()
    {
        try
        {
            await this._chatBridge.Store.ReloadAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "The bindings of guild chats couldn't be loaded.");
        }
    }

    /// <summary>
    /// Loads the bindings again from time to time, because the bindings of deleted guilds are deleted with them,
    /// and synchronizes the channels of the guilds with their members.
    /// </summary>
    private async ValueTask MaintainChatBridgeAsync()
    {
        // The store knows the changes of the bot. A binding of a deleted guild fails harmlessly until the next reload.
        if (++this._maintenanceCount % HostedChannelSyncInterval == 0)
        {
            await this.ReloadChatBindingsAsync().ConfigureAwait(false);
            await this.CreateMissingHostedChannelsAsync().ConfigureAwait(false);
            await this.SyncHostedChannelsAsync().ConfigureAwait(false);
        }
    }

    private async Task SyncHostedChannelsAsync()
    {
        if (this._client is not { } client || this.GetRoutingGuild(client) is not { } guild)
        {
            return;
        }

        var serverId = guild.Id.ToString(CultureInfo.InvariantCulture);
        var bindings = this._chatBridge.Store.Bindings.Where(b => b.IsHosted && b.ExternalServerId == serverId).ToList();
        IReadOnlyDictionary<Guid, ulong> moderators;
        try
        {
            moderators = await this._chatBridge.GetModeratorsAsync(bindings).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "The guild masters of the hosted channels couldn't be determined.");
            return;
        }

        foreach (var binding in bindings)
        {
            if (ulong.TryParse(binding.ExternalChannelId, NumberStyles.None, CultureInfo.InvariantCulture, out var channelId)
                && guild.GetTextChannel(channelId) is { } channel)
            {
                await this.SyncHostedChannelAsync(client, channel, binding, moderators.GetValueOrDefault(binding.GuildId)).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Allows the linked members of the guild or alliance to see the channel, and removes the others.
    /// The linked guild master can also moderate the messages, only in this channel. When the guild master changes,
    /// the previous one becomes a normal member.
    /// The bot doesn't use roles for it, because it can't see the members of roles without the privileged intent for the members.
    /// Discord allows <see cref="MaximumPermissionOverwrites"/> overwrites per channel, so a very large alliance doesn't fit completely.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="channel">The channel.</param>
    /// <param name="binding">The binding of the channel.</param>
    /// <param name="moderatorId">The Discord user who is linked to the guild master, if there is one.</param>
    /// <returns><c>true</c>, if all linked members can see the channel; <c>false</c>, if some don't fit.</returns>
    private async Task<bool> SyncHostedChannelAsync(DiscordSocketClient client, IGuildChannel channel, GuildChatBinding binding, ulong moderatorId)
    {
        try
        {
            var linkedMembers = await this._chatBridge.GetLinkedMembersAsync(binding).ConfigureAwait(false);
            var currentOverwrites = channel.PermissionOverwrites
                .Where(overwrite => overwrite.TargetType == PermissionTarget.User && overwrite.TargetId != client.CurrentUser.Id)
                .ToDictionary(overwrite => overwrite.TargetId, overwrite => overwrite.Permissions);

            // E.g. the overwrites of @everyone and of the bot.
            var otherOverwrites = channel.PermissionOverwrites.Count(overwrite => overwrite.TargetType != PermissionTarget.User || overwrite.TargetId == client.CurrentUser.Id);
            var permissions = DiscordHostedChannelPermissions.Create(currentOverwrites, linkedMembers, moderatorId, MaximumPermissionOverwrites - otherOverwrites);

            if (permissions.HasChanges)
            {
                // Only members of the Discord server get an overwrite. The members aren't cached, because the bot doesn't use the privileged intent for them.
                var addableUserIds = new HashSet<ulong>();
                foreach (var (userId, _) in permissions.Changes.Where(change => !currentOverwrites.ContainsKey(change.UserId)))
                {
                    if (await client.Rest.GetGuildUserAsync(channel.GuildId, userId).ConfigureAwait(false) is not null)
                    {
                        addableUserIds.Add(userId);
                    }
                }

                // One request for all overwrites: a failure changes nothing and is retried with the next synchronization.
                var overwrites = permissions.Apply(channel.PermissionOverwrites, client.CurrentUser.Id, addableUserIds);
                await channel.ModifyAsync(properties => properties.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(overwrites)).ConfigureAwait(false);
            }

            if (permissions.NotFitting.Count > 0)
            {
                this._logger.LogWarning("The Discord channel {channel} can't be opened for {count} linked members, because Discord allows only {maximum} permission overwrites per channel.", channel.Name, permissions.NotFitting.Count, MaximumPermissionOverwrites);
                return false;
            }
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "The members of the Discord channel {channel} couldn't be synchronized. Has the bot the permission to manage it?", channel.Name);
        }

        return true;
    }
}
