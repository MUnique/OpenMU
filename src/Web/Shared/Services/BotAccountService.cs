// <copyright file="BotAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.Extensions.Caching.Memory;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Service for the bot player table on the <c>LoggedIn</c> page.
/// Bots are connection-less <see cref="BotPlayer"/> instances in the game contexts,
/// so they are only visible in the all-in-one deployment.
/// The table is read-only: there is no supported way to stop a single bot from the admin panel.
/// </summary>
public class BotAccountService : IDataService<BotAccount>
{
    private static readonly TimeSpan ListCacheLifetime = TimeSpan.FromSeconds(5);

    private const string ListCacheKey = "BotAccountService.BotList";

    private readonly IServerProvider _serverProvider;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotAccountService"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="cache">The memory cache.</param>
    public BotAccountService(IServerProvider serverProvider, IMemoryCache cache)
    {
        this._serverProvider = serverProvider;
        this._cache = cache;
    }

    /// <summary>
    /// Determines whether the bot feature plugin is active on any in-process game server.
    /// </summary>
    public bool IsBotFeatureAvailable()
    {
        return this._serverProvider.Servers
            .OfType<IGameServerContextProvider>()
            .Any(s => s.Context.PlugInManager.IsPlugInActive(typeof(BotFeaturePlugIn)));
    }

    /// <inheritdoc />
    /// <remarks>
    /// The full list is cached for a few seconds: building it copies the whole player list of every
    /// game server, while the table only renders one page. Paging applies on top of the cached,
    /// globally ordered list, so party grouping stays correct across pages.
    /// </remarks>
    public async Task<List<BotAccount>> GetAsync(int offset, int count)
    {
        if (this._cache.TryGetValue<List<BotAccount>>(ListCacheKey, out var cached) && cached is not null)
        {
            return cached.Skip(offset).Take(count).ToList();
        }

        var rows = new List<(byte ServerId, BotPlayer Player)>();
        foreach (var server in this._serverProvider.Servers.OfType<IGameServerContextProvider>())
        {
            var serverId = (byte)((IManageableServer)server).Id;
            var players = await server.Context.GetPlayersAsync().ConfigureAwait(false);
            rows.AddRange(players.OfType<BotPlayer>().Select(p => (serverId, p)));
        }

        var guildNames = await GuildNames.ResolveAsync(
                GuildNames.FindServer(this._serverProvider),
                rows.Select(r => r.Player.GuildStatus?.GuildId).OfType<uint>())
            .ConfigureAwait(false);

        var ordered = rows
            .Select(r =>
            {
                var (partyMaster, partySize) = PartyDisplay.From(r.Player.Party);
                var guildId = r.Player.GuildStatus?.GuildId;
                return new BotAccount(
                    r.Player.Account?.LoginName ?? string.Empty,
                    r.ServerId,
                    r.Player.SelectedCharacter?.Name,
                    r.Player.StartTimestamp,
                    guildId is { } id ? guildNames.GetValueOrDefault(id) : null,
                    partyMaster,
                    partySize);
            })
            .OrderPartyGrouped()
            .ToList();

        this._cache.Set(ListCacheKey, ordered, ListCacheLifetime);
        return ordered
            .Skip(offset)
            .Take(count)
            .ToList();
    }
}
