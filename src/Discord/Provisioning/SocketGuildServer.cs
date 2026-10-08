// <copyright file="SocketGuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

using System.Globalization;
using global::Discord;
using global::Discord.WebSocket;

/// <summary>
/// A <see cref="IDiscordServer"/> of the Discord library.
/// </summary>
/// <remarks>
/// The cache of the guild learns about created roles and channels a bit later through the gateway,
/// so they are remembered here, until then.
/// </remarks>
internal sealed class SocketGuildServer : IDiscordServer
{
    private readonly SocketGuild _guild;
    private readonly Dictionary<ulong, IRole> _createdRoles = new();
    private readonly Dictionary<ulong, IGuildChannel> _createdChannels = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SocketGuildServer"/> class.
    /// </summary>
    /// <param name="guild">The guild.</param>
    public SocketGuildServer(SocketGuild guild)
    {
        this._guild = guild;
    }

    /// <inheritdoc />
    public ulong EveryoneRoleId => this._guild.EveryoneRole.Id;

    /// <inheritdoc />
    public ulong BotUserId => this._guild.CurrentUser.Id;

    /// <summary>
    /// Gets the channels and categories of a guild.
    /// </summary>
    /// <param name="guild">The guild.</param>
    /// <returns>The channels and categories.</returns>
    public static IReadOnlyCollection<DiscordChannelInfo> GetChannels(SocketGuild guild)
    {
        return guild.Channels
            .Where(channel => channel is SocketCategoryChannel or SocketTextChannel)
            .Select(channel => new DiscordChannelInfo(channel.Id, channel.Name, channel is SocketCategoryChannel, (channel as INestedChannel)?.CategoryId))
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<DiscordRoleInfo> GetRoles()
    {
        return this._guild.Roles.Select(role => new DiscordRoleInfo(role.Id, role.Name)).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<DiscordChannelInfo> GetChannels() => GetChannels(this._guild);

    /// <inheritdoc />
    public async Task<ulong> CreateRoleAsync(DiscordRoleLayout role)
    {
        RoleColors? colors = null;
        if (role.Color is { } colorText
            && uint.TryParse(colorText.TrimStart('#'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var color))
        {
            colors = RoleColors.Solid(new Color(color));
        }

        var createdRole = await this._guild.CreateRoleAsync(role.Name, GuildPermissions.None, colors, role.IsHoisted, false).ConfigureAwait(false);
        this._createdRoles[createdRole.Id] = createdRole;
        return createdRole.Id;
    }

    /// <inheritdoc />
    public async Task<ulong> CreateCategoryAsync(DiscordCategoryLayout category)
    {
        var createdCategory = await this._guild.CreateCategoryChannelAsync(category.Name).ConfigureAwait(false);
        this._createdChannels[createdCategory.Id] = createdCategory;
        return createdCategory.Id;
    }

    /// <inheritdoc />
    public async Task<ulong> CreateChannelAsync(DiscordChannelLayout channel, ulong? categoryId)
    {
        void SetProperties(TextChannelProperties properties)
        {
            properties.CategoryId = categoryId;
            if (channel.Topic is { } topic)
            {
                properties.Topic = topic;
            }
        }

        // Announcement channels are only available on community servers.
        ITextChannel createdChannel = channel.IsAnnouncement && this._guild.Features.HasFeature(GuildFeature.Community)
            ? await this._guild.CreateNewsChannelAsync(channel.Name, SetProperties).ConfigureAwait(false)
            : await this._guild.CreateTextChannelAsync(channel.Name, SetProperties).ConfigureAwait(false);
        this._createdChannels[createdChannel.Id] = createdChannel;
        return createdChannel.Id;
    }

    /// <inheritdoc />
    public async Task ApplyPermissionsAsync(ulong channelId, IReadOnlyList<DiscordPermissionOverwrite> overwrites)
    {
        if ((this._guild.GetChannel(channelId) as IGuildChannel ?? this._createdChannels.GetValueOrDefault(channelId)) is not { } channel)
        {
            throw new InvalidOperationException($"The channel {channelId} wasn't found.");
        }

        foreach (var overwrite in overwrites)
        {
            if (overwrite.IsRole)
            {
                var role = (this._guild.GetRole(overwrite.TargetId) as IRole ?? this._createdRoles.GetValueOrDefault(overwrite.TargetId)) ?? throw new InvalidOperationException($"The role {overwrite.TargetId} wasn't found.");
                var permissions = Merge(channel.GetPermissionOverwrite(role), overwrite);
                await channel.AddPermissionOverwriteAsync(role, permissions).ConfigureAwait(false);
            }
            else
            {
                var user = this._guild.GetUser(overwrite.TargetId) ?? throw new InvalidOperationException($"The user {overwrite.TargetId} wasn't found.");
                var permissions = Merge(channel.GetPermissionOverwrite(user), overwrite);
                await channel.AddPermissionOverwriteAsync(user, permissions).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Merges the overwrite into the existing permissions, so that the permissions which the overwrite doesn't define stay as they are.
    /// </summary>
    private static OverwritePermissions Merge(OverwritePermissions? existing, DiscordPermissionOverwrite overwrite)
    {
        PermValue? GetValue(DiscordChannelAccess access) =>
            overwrite.Allow.HasFlag(access) ? PermValue.Allow
            : overwrite.Deny.HasFlag(access) ? PermValue.Deny
            : null;

        return (existing ?? OverwritePermissions.InheritAll).Modify(
            viewChannel: GetValue(DiscordChannelAccess.View),
            sendMessages: GetValue(DiscordChannelAccess.Send));
    }
}
