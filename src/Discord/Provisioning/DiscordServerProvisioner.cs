// <copyright file="DiscordServerProvisioner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

/// <summary>
/// Sets up a Discord server according to a <see cref="DiscordServerLayout"/>.
/// </summary>
/// <remarks>
/// It can run any number of times: Existing roles, categories and channels are adopted by their name,
/// and only the missing ones are created. Nothing is deleted or renamed, and permissions which
/// the layout doesn't define stay as they are.
/// </remarks>
public sealed class DiscordServerProvisioner
{
    private readonly ILogger<DiscordServerProvisioner> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordServerProvisioner"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public DiscordServerProvisioner(ILogger<DiscordServerProvisioner> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// Finds the channels of the layout in the existing channels, by their name.
    /// </summary>
    /// <param name="layout">The layout.</param>
    /// <param name="channels">The existing channels and categories.</param>
    /// <returns>The identifiers of the found channels by their key.</returns>
    public static IReadOnlyDictionary<string, ulong> FindChannels(DiscordServerLayout layout, IReadOnlyCollection<DiscordChannelInfo> channels)
    {
        var result = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in layout.Categories)
        {
            var categoryId = FindCategory(channels, category.Name)?.Id;
            foreach (var channel in category.Channels)
            {
                if (FindChannel(channels, channel.Name, categoryId) is { } existing
                    && (category.VisibleTo.Count == 0 || existing.IsPrivate))
                {
                    result[channel.Key] = existing.Id;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Sets up the Discord server according to the layout.
    /// </summary>
    /// <param name="server">The Discord server.</param>
    /// <param name="layout">The layout.</param>
    /// <returns>The result.</returns>
    public async Task<DiscordProvisioningResult> ProvisionAsync(IDiscordServer server, DiscordServerLayout layout)
    {
        var result = new DiscordProvisioningResult();
        var roleIds = await this.ProvisionRolesAsync(server, layout, result).ConfigureAwait(false);

        var channels = server.GetChannels();
        foreach (var category in layout.Categories)
        {
            if (category.VisibleTo.Count > 0 && category.VisibleTo.Any(key => !roleIds.ContainsKey(key)))
            {
                // Do not create or adopt staff channels until every required role exists.
                result.Failed.Add((category.Name, Properties.Resources.ResourceManager.GetString("Setup_PrivateRolesMissing") ?? string.Empty));
                continue;
            }

            var categoryId = await this.GetOrCreateAsync(
                result,
                category.Name,
                () => FindCategory(channels, category.Name)?.Id,
                () => server.CreateCategoryAsync(category)).ConfigureAwait(false);
            if (categoryId is null)
            {
                continue;
            }

            if (!await this.ApplyPermissionsAsync(server, result, category.Name, categoryId.Value, CreateOverwrites(server, roleIds, category, null)).ConfigureAwait(false))
            {
                continue;
            }

            await this.ProvisionChannelsAsync(server, channels, roleIds, category, categoryId.Value, result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Creates the overwrites of the permissions of a category or a channel.
    /// </summary>
    /// <param name="server">The Discord server.</param>
    /// <param name="roleIds">The identifiers of the roles by their key.</param>
    /// <param name="category">The category.</param>
    /// <param name="channel">The channel; or <c>null</c>, for the category itself.</param>
    /// <returns>The overwrites.</returns>
    internal static IReadOnlyList<DiscordPermissionOverwrite> CreateOverwrites(IDiscordServer server, IReadOnlyDictionary<string, ulong> roleIds, DiscordCategoryLayout category, DiscordChannelLayout? channel)
    {
        // Privacy is a property of the layout, never of whether role creation happened to succeed.
        var isPrivate = category.VisibleTo.Count > 0;
        var isReadOnly = channel?.IsReadOnly ?? false;
        if (!isPrivate && !isReadOnly)
        {
            return [];
        }

        var everyoneDeny = (isPrivate ? DiscordChannelAccess.View : DiscordChannelAccess.None)
                           | (isReadOnly ? DiscordChannelAccess.Send : DiscordChannelAccess.None);
        var overwrites = new List<DiscordPermissionOverwrite>
        {
            // The bot always needs to see the channels and to post into them. Its overwrite comes first: the overwrites
            // are set one after another, and once @everyone is denied to view a channel, the bot couldn't set the others.
            new(server.BotUserId, false, DiscordChannelAccess.View | DiscordChannelAccess.Send, DiscordChannelAccess.None),
            new(server.EveryoneRoleId, true, DiscordChannelAccess.None, everyoneDeny),
        };

        if (isPrivate)
        {
            var roleAccess = isReadOnly ? DiscordChannelAccess.View : DiscordChannelAccess.View | DiscordChannelAccess.Send;
            overwrites.AddRange(category.VisibleTo
                .Where(roleIds.ContainsKey)
                .Select(roleKey => new DiscordPermissionOverwrite(roleIds[roleKey], true, roleAccess, DiscordChannelAccess.None)));
        }

        return overwrites;
    }

    private static DiscordChannelInfo? FindCategory(IReadOnlyCollection<DiscordChannelInfo> channels, string name)
    {
        return channels.FirstOrDefault(c => c.IsCategory && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds a channel by its name. A channel in the category is preferred, but it may also have been moved somewhere else.
    /// </summary>
    private static DiscordChannelInfo? FindChannel(IReadOnlyCollection<DiscordChannelInfo> channels, string name, ulong? categoryId)
    {
        var candidates = channels.Where(c => !c.IsCategory && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)).ToList();
        return candidates.FirstOrDefault(c => c.CategoryId == categoryId) ?? candidates.FirstOrDefault();
    }

    private async Task ProvisionChannelsAsync(
        IDiscordServer server,
        IReadOnlyCollection<DiscordChannelInfo> channels,
        IReadOnlyDictionary<string, ulong> roleIds,
        DiscordCategoryLayout category,
        ulong categoryId,
        DiscordProvisioningResult result)
    {
        foreach (var channel in category.Channels)
        {
            var channelId = await this.GetOrCreateAsync(
                result,
                "#" + channel.Name,
                () => FindChannel(channels, channel.Name, categoryId)?.Id,
                () => server.CreateChannelAsync(channel, categoryId)).ConfigureAwait(false);
            if (channelId is null)
            {
                continue;
            }

            if (await this.ApplyPermissionsAsync(server, result, "#" + channel.Name, channelId.Value, CreateOverwrites(server, roleIds, category, channel)).ConfigureAwait(false))
            {
                result.ChannelIds[channel.Key] = channelId.Value;
            }
        }
    }

    private async Task<Dictionary<string, ulong>> ProvisionRolesAsync(IDiscordServer server, DiscordServerLayout layout, DiscordProvisioningResult result)
    {
        var roles = server.GetRoles();
        var roleIds = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in layout.Roles)
        {
            var roleId = await this.GetOrCreateAsync(
                result,
                "@" + role.Name,
                () => roles.FirstOrDefault(r => string.Equals(r.Name, role.Name, StringComparison.OrdinalIgnoreCase))?.Id,
                () => server.CreateRoleAsync(role)).ConfigureAwait(false);
            if (roleId is { } id)
            {
                roleIds[role.Key] = id;
            }
        }

        return roleIds;
    }

    private async Task<ulong?> GetOrCreateAsync(DiscordProvisioningResult result, string displayName, Func<ulong?> find, Func<Task<ulong>> create)
    {
        if (find() is { } existingId)
        {
            result.Adopted.Add(displayName);
            return existingId;
        }

        try
        {
            var id = await create().ConfigureAwait(false);
            result.Created.Add(displayName);
            return id;
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "{name} couldn't be created on the Discord server.", displayName);
            result.Failed.Add((displayName, ex.Message));
            return null;
        }
    }

    private async Task<bool> ApplyPermissionsAsync(IDiscordServer server, DiscordProvisioningResult result, string displayName, ulong channelId, IReadOnlyList<DiscordPermissionOverwrite> overwrites)
    {
        if (overwrites.Count == 0)
        {
            return true;
        }

        try
        {
            await server.ApplyPermissionsAsync(channelId, overwrites).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "The permissions of {name} couldn't be set on the Discord server.", displayName);
            result.Failed.Add((displayName, ex.Message));
            return false;
        }
    }
}
