// <copyright file="FakeDiscordServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// A <see cref="IDiscordServer"/> in memory.
/// </summary>
internal sealed class FakeDiscordServer : IDiscordServer
{
    private ulong _nextId = 1000;

    /// <inheritdoc />
    public ulong EveryoneRoleId => 1;

    /// <inheritdoc />
    public ulong BotUserId => 2;

    /// <summary>
    /// Gets the roles.
    /// </summary>
    public List<DiscordRoleInfo> Roles { get; } = new();

    /// <summary>
    /// Gets the channels and categories.
    /// </summary>
    public List<DiscordChannelInfo> Channels { get; } = new();

    /// <summary>
    /// Gets the applied overwrites per channel.
    /// </summary>
    public Dictionary<ulong, IReadOnlyList<DiscordPermissionOverwrite>> Overwrites { get; } = new();

    /// <summary>
    /// Gets the number of created roles, categories and channels.
    /// </summary>
    public int CreatedCount { get; private set; }

    /// <summary>
    /// Gets or sets the name of a role, category or channel which can't be created.
    /// </summary>
    public string? FailingName { get; set; }

    /// <summary>
    /// Adds an existing category.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The identifier.</returns>
    public ulong AddCategory(string name)
    {
        var id = this._nextId++;
        this.Channels.Add(new DiscordChannelInfo(id, name, true, null));
        return id;
    }

    /// <summary>
    /// Adds an existing channel.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="categoryId">The identifier of the category.</param>
    /// <returns>The identifier.</returns>
    public ulong AddChannel(string name, ulong? categoryId)
    {
        var id = this._nextId++;
        this.Channels.Add(new DiscordChannelInfo(id, name, false, categoryId));
        return id;
    }

    /// <summary>
    /// Gets the identifier of a channel or category by its name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The identifier.</returns>
    public ulong GetId(string name) => this.Channels.Single(c => c.Name == name).Id;

    /// <inheritdoc />
    public IReadOnlyCollection<DiscordRoleInfo> GetRoles() => this.Roles.ToList();

    /// <inheritdoc />
    public IReadOnlyCollection<DiscordChannelInfo> GetChannels() => this.Channels.ToList();

    /// <inheritdoc />
    public Task<ulong> CreateRoleAsync(DiscordRoleLayout role)
    {
        this.ThrowIfFailing(role.Name);
        var id = this._nextId++;
        this.Roles.Add(new DiscordRoleInfo(id, role.Name));
        this.CreatedCount++;
        return Task.FromResult(id);
    }

    /// <inheritdoc />
    public Task<ulong> CreateCategoryAsync(DiscordCategoryLayout category)
    {
        this.ThrowIfFailing(category.Name);
        this.CreatedCount++;
        return Task.FromResult(this.AddCategory(category.Name));
    }

    /// <inheritdoc />
    public Task<ulong> CreateChannelAsync(DiscordChannelLayout channel, ulong? categoryId)
    {
        this.ThrowIfFailing(channel.Name);
        this.CreatedCount++;
        return Task.FromResult(this.AddChannel(channel.Name, categoryId));
    }

    /// <inheritdoc />
    public Task ApplyPermissionsAsync(ulong channelId, IReadOnlyList<DiscordPermissionOverwrite> overwrites)
    {
        this.Overwrites[channelId] = overwrites;
        return Task.CompletedTask;
    }

    private void ThrowIfFailing(string name)
    {
        if (name == this.FailingName)
        {
            throw new InvalidOperationException("Missing Permissions");
        }
    }
}
