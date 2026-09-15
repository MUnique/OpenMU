// <copyright file="GuildService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components;
using MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// Service for guild information in the admin panel.
/// It reads guild data from the guild context; enrichment of members with character/account
/// data is delegated to an <see cref="IGuildMemberEnricher"/>, which lives in a separate context.
/// </summary>
public class GuildService : IGuildService, ISupportDataChangedNotification, IDisposable
{
    private readonly IPersistenceContextProvider _contextProvider;
    private readonly IGuildMemberEnricher _memberEnricher;
    private readonly ILogger<GuildService> _logger;
    private readonly Debouncer _debouncer = new(300);

    private string _searchFilter = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildService"/> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    /// <param name="memberEnricher">The enricher which adds character/account data to guild members.</param>
    /// <param name="logger">The logger.</param>
    public GuildService(IPersistenceContextProvider contextProvider, IGuildMemberEnricher memberEnricher, ILogger<GuildService> logger)
    {
        this._contextProvider = contextProvider;
        this._memberEnricher = memberEnricher;
        this._logger = logger;
    }

    /// <inheritdoc />
    public event EventHandler? DataChanged;

    /// <summary>
    /// Gets or sets the search filter query which is applied to the guild name.
    /// </summary>
    public string SearchFilter
    {
        get => this._searchFilter;
        set
        {
            var newValue = value ?? string.Empty;
            if (this._searchFilter != newValue)
            {
                this._searchFilter = newValue;
                if (string.IsNullOrEmpty(newValue))
                {
                    this._debouncer.Cancel();
                    this.DataChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _ = this._debouncer.DebounceAsync(this.RaiseDataChangedAsync);
                }
            }
        }
    }

    /// <inheritdoc />
    public async Task<List<GuildListItem>> GetAsync(int offset, int count)
    {
        return await this.SafeAsync(
            async () =>
            {
                using var context = this._contextProvider.CreateNewGuildContext();
                var filter = this.SearchFilter.Trim();

                IReadOnlyList<Guild> guilds;
                if (string.IsNullOrWhiteSpace(filter))
                {
                    guilds = await context.GetGuildsOrderedByNameAsync(offset, count).ConfigureAwait(false);
                }
                else
                {
                    guilds = await context.SearchGuildsAsync(filter, offset, count).ConfigureAwait(false);
                    if (guilds.Count == 0 && offset > 0)
                    {
                        // The filter narrowed the result set down to less entries than the current page offset - show the first page instead.
                        guilds = await context.SearchGuildsAsync(filter, 0, count).ConfigureAwait(false);
                    }
                }

                return await ToListItemsWithAllianceInfoAsync(guilds, context).ConfigureAwait(false);
            },
            [],
            "Failed to retrieve guilds (offset: {Offset}, count: {Count}).",
            offset,
            count).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<GuildListItem?> GetGuildAsync(Guid guildId)
    {
        return await this.SafeAsync<GuildListItem?>(
            async () =>
            {
                using var context = this._contextProvider.CreateNewGuildContext();
                var guild = await context.GetByIdAsync<Guild>(guildId).ConfigureAwait(false);
                if (guild is null)
                {
                    return null;
                }

                var items = await ToListItemsWithAllianceInfoAsync([guild], context).ConfigureAwait(false);
                return items[0];
            },
            null,
            "Failed to retrieve guild with ID {GuildId}.",
            guildId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GuildMemberViewItem>> GetGuildMembersAsync(Guid guildId)
    {
        return await this.SafeAsync<IReadOnlyList<GuildMemberViewItem>>(
            async () =>
            {
                using var guildContext = this._contextProvider.CreateNewGuildContext();
                var guild = await guildContext.GetByIdAsync<Guild>(guildId).ConfigureAwait(false);
                if (guild?.Members is not { } members || members.Count == 0)
                {
                    return [];
                }

                var names = await guildContext.GetMemberNamesAsync(guildId).ConfigureAwait(false);
                var result = members
                    .Select(member => new GuildMemberViewItem
                    {
                        CharacterId = member.Id,
                        CharacterName = names.TryGetValue(member.Id, out var name) ? name : member.Id.ToString(),
                        Position = member.Status,
                    })
                    .ToList();

                await this._memberEnricher.EnrichAsync(result).ConfigureAwait(false);

                result.Sort(GuildPositionComparer.Instance);
                return result;
            },
            [],
            "Failed to retrieve members for guild with ID {GuildId}.",
            guildId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<AllianceDetails?> GetAllianceAsync(Guid guildId)
    {
        return await this.SafeAsync<AllianceDetails?>(
            async () =>
            {
                using var context = this._contextProvider.CreateNewGuildContext();
                var guild = await context.GetByIdAsync<Guild>(guildId).ConfigureAwait(false);
                if (guild is null)
                {
                    return null;
                }

                var master = guild.AllianceGuild as Guild ?? guild;
                var allianceGuilds = await context.GetAlliancesAsync(master.Id).ConfigureAwait(false);
                var guilds = allianceGuilds
                    .Select(ToListItem)
                    .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (guilds.All(item => item.Id != master.Id))
                {
                    guilds.Insert(0, ToListItem(master));
                }

                var masterItem = ToListItem(master);

                // The master's own AllianceGuild navigation is null (it doesn't point to itself), so
                // annotate it explicitly as its own alliance here, consistent with the list/detail pages.
                masterItem.AllianceGuildId ??= master.Id;
                masterItem.AllianceName ??= master.Name;

                return new AllianceDetails
                {
                    Master = masterItem,
                    Guilds = guilds,
                };
            },
            null,
            "Failed to retrieve alliance for guild with ID {GuildId}.",
            guildId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._debouncer.Dispose();
    }

    private static GuildListItem ToListItem(Guild guild)
    {
        var alliance = guild.AllianceGuild as Guild;
        return new GuildListItem
        {
            Id = guild.Id,
            Name = guild.Name ?? string.Empty,
            Score = guild.Score,
            Notice = guild.Notice,
            MemberCount = guild.Members?.Count ?? 0,
            AllianceGuildId = alliance?.Id,
            AllianceName = alliance?.Name,
            Logo = guild.Logo,
        };
    }

    /// <summary>
    /// Maps guilds to list items and additionally marks guilds which are themselves the master of
    /// an alliance (i.e. other guilds point to them via <see cref="Interfaces.Guild.AllianceGuild"/>,
    /// but they don't point to anyone). Without this, only member guilds would get a working
    /// "Alliance" link - the master's own row/page would show nothing, even though it heads an alliance.
    /// </summary>
    private static async Task<List<GuildListItem>> ToListItemsWithAllianceInfoAsync(IReadOnlyList<Guild> guilds, IGuildServerContext context)
    {
        var items = guilds.Select(ToListItem).ToList();

        // Only guilds without an AllianceGuild of their own are candidates for being a master.
        // this keeps the lookup bounded to the current page instead of scanning the whole guild table.
        var candidateMasterIds = items
            .Where(item => item.AllianceGuildId is null)
            .Select(item => item.Id)
            .ToList();

        if (candidateMasterIds.Count > 0)
        {
            var masterIds = await context.GetAllianceMasterIdsAsync(candidateMasterIds).ConfigureAwait(false);
            if (masterIds.Count > 0)
            {
                foreach (var item in items)
                {
                    if (masterIds.Contains(item.Id))
                    {
                        item.AllianceGuildId = item.Id;
                        item.AllianceName = item.Name;
                    }
                }
            }
        }

        return items;
    }

    private async Task<T> SafeAsync<T>(Func<Task<T>> action, T fallback, string errorMessage, params object?[] args)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, errorMessage, args);
            return fallback;
        }
    }

    private Task RaiseDataChangedAsync()
    {
        this.DataChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }
}
