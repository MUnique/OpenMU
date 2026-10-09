// <copyright file="GuildChatBindingStore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Globalization;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Stores the bindings of the chats of guilds to Discord channels, and keeps them in memory,
/// so that a chat message doesn't need a query to the database.
/// </summary>
public sealed class GuildChatBindingStore
{
    private const string Provider = AccountLinkService.DiscordProvider;

    private readonly Func<IPlayerContext> _createContext;
    private IReadOnlyList<GuildChatBinding> _bindings = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildChatBindingStore"/> class.
    /// </summary>
    /// <param name="createContext">The function which creates a persistence context.</param>
    public GuildChatBindingStore(Func<IPlayerContext> createContext)
    {
        this._createContext = createContext;
    }

    /// <summary>
    /// Gets the bindings.
    /// </summary>
    public IReadOnlyList<GuildChatBinding> Bindings => this._bindings;

    /// <summary>
    /// Loads the bindings from the database again, e.g. because the bindings of deleted guilds were deleted with them.
    /// </summary>
    /// <returns>The task.</returns>
    public async ValueTask ReloadAsync()
    {
        using var context = this._createContext();
        var bindings = await context.GetAsync<GuildChatBinding>().ConfigureAwait(false);
        this._bindings = bindings.Where(binding => binding.Provider == Provider).ToList();
    }

    /// <summary>
    /// Gets the binding of a channel.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <returns>The binding; or <c>null</c>, if the channel isn't bound.</returns>
    public GuildChatBinding? GetByChannel(ulong channelId)
    {
        var id = ToText(channelId);
        return this._bindings.FirstOrDefault(binding => binding.ExternalChannelId == id);
    }

    /// <summary>
    /// Gets the binding of the chat of a guild or alliance.
    /// </summary>
    /// <param name="guildId">The identifier of the guild; for an alliance, of its master guild.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>The binding; or <c>null</c>, if the chat isn't bound.</returns>
    public GuildChatBinding? Get(Guid guildId, GuildChatScope scope)
    {
        return this._bindings.FirstOrDefault(binding => binding.GuildId == guildId && binding.Scope == scope);
    }

    /// <summary>
    /// Binds the chat of a guild or alliance to a channel. An existing binding of the chat is moved to the channel.
    /// </summary>
    /// <param name="guildId">The identifier of the guild; for an alliance, of its master guild.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="serverId">The identifier of the Discord server.</param>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="isHosted">A value indicating whether the channel is on the Discord server of the game server.</param>
    /// <param name="boundBy">The name of the character which binds the chat.</param>
    /// <param name="boundAt">The timestamp.</param>
    /// <returns>The task.</returns>
    public async ValueTask SetAsync(Guid guildId, GuildChatScope scope, ulong serverId, ulong channelId, bool isHosted, string boundBy, DateTime boundAt)
    {
        using (var context = this._createContext())
        {
            var bindings = await context.GetAsync<GuildChatBinding>().ConfigureAwait(false);
            var binding = bindings.FirstOrDefault(b => b.Provider == Provider && b.GuildId == guildId && b.Scope == scope);
            if (binding is null)
            {
                binding = context.CreateNew<GuildChatBinding>();
                binding.GuildId = guildId;
                binding.Scope = scope;
                binding.Provider = Provider;
            }

            binding.ExternalServerId = ToText(serverId);
            binding.ExternalChannelId = ToText(channelId);
            binding.IsHosted = isHosted;
            binding.BoundBy = boundBy;
            binding.BoundAt = boundAt;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        await this.ReloadAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the bindings of channels or of a Discord server.
    /// </summary>
    /// <param name="channelId">The identifier of the channel; or <c>null</c>, to remove the bindings of the Discord server.</param>
    /// <param name="serverId">The identifier of the Discord server; or <c>null</c>, to remove the binding of the channel.</param>
    /// <returns>The number of removed bindings.</returns>
    public async ValueTask<int> RemoveAsync(ulong? channelId, ulong? serverId)
    {
        var channelText = channelId is { } c ? ToText(c) : null;
        var serverText = serverId is { } s ? ToText(s) : null;
        if (!this._bindings.Any(Matches))
        {
            return 0;
        }

        int count;
        using (var context = this._createContext())
        {
            var bindings = (await context.GetAsync<GuildChatBinding>().ConfigureAwait(false)).Where(Matches).ToList();
            foreach (var binding in bindings)
            {
                await context.DeleteAsync(binding).ConfigureAwait(false);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            count = bindings.Count;
        }

        await this.ReloadAsync().ConfigureAwait(false);
        return count;

        bool Matches(GuildChatBinding binding) => binding.Provider == Provider
                                                  && (channelText is null || binding.ExternalChannelId == channelText)
                                                  && (serverText is null || binding.ExternalServerId == serverText);
    }

    /// <summary>
    /// Converts an identifier of Discord to the text which is stored.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The text.</returns>
    internal static string ToText(ulong id) => id.ToString(CultureInfo.InvariantCulture);
}
