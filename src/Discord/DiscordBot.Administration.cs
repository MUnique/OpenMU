// <copyright file="DiscordBot.Administration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using System.Net;
using global::Discord;
using global::Discord.Net;
using global::Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// The part of the <see cref="DiscordBot"/> which the admin panel uses to show its state and to set up a Discord server.
/// </summary>
public sealed partial class DiscordBot
{
    /// <summary>
    /// The close code of the gateway when the bot requests a privileged intent which isn't enabled for it.
    /// </summary>
    private const int DisallowedIntentsCloseCode = 4014;

    /// <summary>
    /// The close code of the gateway when Discord rejects the token.
    /// </summary>
    private const int AuthenticationFailedCloseCode = 4004;

    /// <summary>
    /// The permissions which the bot needs on the Discord server whose channels receive the notifications and the status.
    /// </summary>
    private static readonly GuildPermission[] RequiredPermissions =
    [
        GuildPermission.ViewChannel,
        GuildPermission.SendMessages,
        GuildPermission.EmbedLinks,
        GuildPermission.ReadMessageHistory,
        GuildPermission.AddReactions,
        GuildPermission.ManageChannels,
        GuildPermission.ManageRoles,

        // A bot can only give the permissions which it has, e.g. to the guild masters in their channels.
        GuildPermission.ManageMessages,
    ];

    private DiscordBotProblem _problem;

    /// <summary>
    /// Gets the problem which stopped the bot, until it's started again.
    /// </summary>
    public DiscordBotProblem Problem
    {
        get => this._problem;
        private set
        {
            if (this._problem != value)
            {
                this._problem = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the name of the bot user, when it's connected.
    /// </summary>
    public string? UserName => this._client?.CurrentUser?.Username;

    /// <summary>
    /// Gets the identifier of the Discord server whose channels receive the notifications and the status.
    /// </summary>
    public ulong? RoutingServerId => this._client is { } client ? this.GetRoutingGuild(client)?.Id : null;

    /// <summary>
    /// Gets the Discord servers which the bot is in.
    /// </summary>
    /// <returns>The Discord servers, ordered by their name.</returns>
    public IReadOnlyList<DiscordServerInfo> GetDiscordServers()
    {
        return this._client?.Guilds
                   .Select(guild => new DiscordServerInfo(guild.Id, guild.Name))
                   .OrderBy(server => server.Name, StringComparer.CurrentCultureIgnoreCase)
                   .ToList()
               ?? [];
    }

    /// <summary>
    /// Sets up the roles, categories and channels of the layout on a Discord server, like <c>/openmu setup</c>.
    /// </summary>
    /// <param name="discordServerId">The identifier of the Discord server.</param>
    /// <returns>The result, or <c>null</c>, if the bot isn't connected or isn't in the Discord server.</returns>
    public async ValueTask<DiscordProvisioningResult?> SetUpDiscordServerAsync(ulong discordServerId)
    {
        if (this._client is not { } client || client.GetGuild(discordServerId) is not { } guild)
        {
            return null;
        }

        this._logger.LogInformation("The Discord server {guild} ({guildId}) is set up from the admin panel.", guild.Name, guild.Id);
        return await this.ProvisionAsync(client, guild).ConfigureAwait(false);
    }

    /// <summary>
    /// Posts a test message into a channel, and tells why it failed.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="embed">The test message.</param>
    /// <returns>The result.</returns>
    public async ValueTask<DiscordTestResult> SendTestMessageAsync(ulong channelId, DiscordEmbed embed)
    {
        if (await this.GetMessageChannelAsync(channelId).ConfigureAwait(false) is not { } channel)
        {
            return new DiscordTestResult(DiscordTestOutcome.ChannelUnavailable, channelId.ToString(CultureInfo.InvariantCulture));
        }

        var channelName = $"#{channel.Name}";
        try
        {
            await channel.SendMessageAsync(embed: ToDiscordEmbed(embed), allowedMentions: AllowedMentions.None).ConfigureAwait(false);
            return new DiscordTestResult(DiscordTestOutcome.SentByBot, channelName);
        }
        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.Forbidden)
        {
            return new DiscordTestResult(DiscordTestOutcome.MissingPermissions, channelName);
        }
        catch (HttpException ex)
        {
            this._logger.LogWarning(ex, "The test message couldn't be posted into {channel}.", channelName);
            return new DiscordTestResult(DiscordTestOutcome.Failed, ex.Message);
        }
    }

    /// <summary>
    /// Checks the Discord server whose channels receive the notifications and the status: the permissions of the bot,
    /// the role of linked players and the channels of the categories.
    /// </summary>
    /// <returns>The result, or <c>null</c>, if the bot isn't connected or doesn't know which Discord server to use.</returns>
    /// <remarks>
    /// The roles are loaded from Discord, because the positions in the cache of the client may be outdated: when a role
    /// is created, Discord moves the others up without telling the client about each of them.
    /// </remarks>
    public async ValueTask<DiscordServerCheck?> CheckDiscordServerAsync()
    {
        if (this._client is not { } client || this.GetRoutingGuild(client) is not { } guild)
        {
            return null;
        }

        var permissions = guild.CurrentUser.GuildPermissions;
        var missingPermissions = RequiredPermissions
            .Where(permission => !permissions.Has(permission))
            .Select(permission => permission.ToString())
            .ToList();
        if (this._settings.CreateScheduledEvents && !permissions.Has(GuildPermission.ManageEvents))
        {
            missingPermissions.Add(nameof(GuildPermission.ManageEvents));
        }

        var linkedRoleName = this._layout.Roles.FirstOrDefault(role => role.Key == DiscordServerLayout.LinkedRoleKey)?.Name;
        var roles = await client.Rest.GetGuildAsync(guild.Id).ConfigureAwait(false) is { } restGuild
            ? restGuild.Roles.Select(role => (role.Id, role.Name, role.Position)).ToList()
            : guild.Roles.Select(role => (role.Id, role.Name, role.Position)).ToList();
        var botRoleIds = guild.CurrentUser.Roles.Where(role => !role.IsEveryone).Select(role => role.Id).ToList();
        var linkedRoleState = DiscordRoleHierarchy.GetState(roles, botRoleIds, linkedRoleName);

        var categoriesWithoutChannel = Enum.GetValues<DiscordChannelCategory>().Where(category => this.GetChannelId(category) is null).ToList();
        return new DiscordServerCheck(guild.Name, missingPermissions, linkedRoleState, categoriesWithoutChannel, this._routing.StatusChannelId is not null);
    }

    /// <summary>
    /// Gets the text channels of the Discord server whose channels receive the notifications and the status.
    /// </summary>
    /// <returns>The channels, in the order of Discord.</returns>
    public IReadOnlyList<DiscordChannelOption> GetTextChannels()
    {
        if (this._client is not { } client || this.GetRoutingGuild(client) is not { } guild)
        {
            return [];
        }

        return guild.TextChannels
            .OrderBy(channel => channel.Category?.Position ?? -1)
            .ThenBy(channel => channel.Position)
            .Select(channel => new DiscordChannelOption(channel.Id, channel.Name, channel.Category?.Name))
            .ToList();
    }

    /// <summary>
    /// Gets the chats of guilds and alliances which are bound to Discord channels.
    /// </summary>
    /// <returns>The bound chats.</returns>
    public async ValueTask<IReadOnlyList<DiscordChatBindingInfo>> GetChatBindingsAsync()
    {
        var result = new List<DiscordChatBindingInfo>();
        foreach (var binding in this._chatBridge.Store.Bindings.ToList())
        {
            if (!ulong.TryParse(binding.ExternalChannelId, NumberStyles.None, CultureInfo.InvariantCulture, out var channelId))
            {
                continue;
            }

            var channel = this._client?.GetChannel(channelId) as IGuildChannel;
            var guildName = await this._chatBridge.GetGuildNameAsync(binding.GuildId).ConfigureAwait(false);
            result.Add(new DiscordChatBindingInfo(channelId, guildName, binding.Scope, channel?.Name, channel?.Guild.Name, binding.IsHosted, binding.BoundBy, binding.BoundAt));
        }

        return result.OrderBy(binding => binding.GuildName, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    /// <summary>
    /// Removes the binding of a channel, like <c>/guildchat unbind</c>. The channel itself is kept.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <returns><c>true</c>, if a binding was removed.</returns>
    public async ValueTask<bool> RemoveChatBindingAsync(ulong channelId)
    {
        var removed = await this._chatBridge.Store.RemoveAsync(channelId, null).ConfigureAwait(false) > 0;
        if (removed)
        {
            this._logger.LogInformation("The binding of the Discord channel {channelId} was removed in the admin panel.", channelId);
        }

        return removed;
    }

    /// <summary>
    /// Determines the problem which an exception of the client reports, and which stops the bot.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The problem, or <see cref="DiscordBotProblem.None"/>.</returns>
    private static DiscordBotProblem GetStoppingProblem(Exception? exception)
    {
        for (; exception is not null; exception = exception.InnerException)
        {
            switch (exception)
            {
                case global::Discord.Net.HttpException { HttpCode: System.Net.HttpStatusCode.Unauthorized }:
                case global::Discord.Net.WebSocketClosedException { CloseCode: AuthenticationFailedCloseCode }:
                    return DiscordBotProblem.TokenRejected;
                case global::Discord.Net.WebSocketClosedException { CloseCode: DisallowedIntentsCloseCode }:
                    return DiscordBotProblem.IntentNotEnabled;
            }
        }

        return DiscordBotProblem.None;
    }

    private async ValueTask<DiscordProvisioningResult> ProvisionAsync(DiscordSocketClient client, SocketGuild guild)
    {
        var result = await this._provisioner.ProvisionAsync(new SocketGuildServer(guild), this._layout).ConfigureAwait(false);
        if (this.GetRoutingGuild(client)?.Id == guild.Id)
        {
            // The cache of the client might not know the created channels yet.
            this._routing = DiscordChannelRouting.Create(this._settings, this._layout, result.ChannelIds);
        }

        return result;
    }
}
