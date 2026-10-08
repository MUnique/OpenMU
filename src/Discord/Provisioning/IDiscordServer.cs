// <copyright file="IDiscordServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// A Discord server (guild), as far as the <see cref="DiscordServerProvisioner"/> needs it.
/// </summary>
public interface IDiscordServer
{
    /// <summary>
    /// Gets the identifier of the role which everybody has.
    /// </summary>
    ulong EveryoneRoleId { get; }

    /// <summary>
    /// Gets the user identifier of the bot.
    /// </summary>
    ulong BotUserId { get; }

    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <returns>The roles.</returns>
    IReadOnlyCollection<DiscordRoleInfo> GetRoles();

    /// <summary>
    /// Gets the channels and categories.
    /// </summary>
    /// <returns>The channels and categories.</returns>
    IReadOnlyCollection<DiscordChannelInfo> GetChannels();

    /// <summary>
    /// Creates a role.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>The identifier of the created role.</returns>
    Task<ulong> CreateRoleAsync(DiscordRoleLayout role);

    /// <summary>
    /// Creates a category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The identifier of the created category.</returns>
    Task<ulong> CreateCategoryAsync(DiscordCategoryLayout category);

    /// <summary>
    /// Creates a text channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="categoryId">The identifier of the category of the channel.</param>
    /// <returns>The identifier of the created channel.</returns>
    Task<ulong> CreateChannelAsync(DiscordChannelLayout channel, ulong? categoryId);

    /// <summary>
    /// Applies the overwrites to the permissions of a channel or category.
    /// </summary>
    /// <param name="channelId">The identifier of the channel or category.</param>
    /// <param name="overwrites">The overwrites.</param>
    /// <returns>The task.</returns>
    Task ApplyPermissionsAsync(ulong channelId, IReadOnlyList<DiscordPermissionOverwrite> overwrites);
}
