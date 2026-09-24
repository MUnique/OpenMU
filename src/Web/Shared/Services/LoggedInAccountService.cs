// <copyright file="LoggedInAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.Extensions.Caching.Memory;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Services for the <see cref="LoggedInAccount"/> page.
/// </summary>
public class LoggedInAccountService : IDataService<LoggedInAccount>, ISupportDataChangedNotification
{
    private static readonly TimeSpan LookupCacheLifetime = TimeSpan.FromSeconds(5);

    private const string LookupCacheKey = "LoggedInAccountService.PlayerLookup";

    private readonly ILoginServer _loginServer;
    private readonly IServerProvider _serverProvider;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggedInAccountService"/> class.
    /// </summary>
    /// <param name="loginServer">The login server.</param>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="cache">The memory cache.</param>
    public LoggedInAccountService(ILoginServer loginServer, IServerProvider serverProvider, IMemoryCache cache)
    {
        this._loginServer = loginServer;
        this._serverProvider = serverProvider;
        this._cache = cache;
    }

    /// <summary>
    /// Event raised when the data has changed.
    /// </summary>
    public event EventHandler? DataChanged;

    /// <summary>
    /// Sets the account offline.
    /// </summary>
    /// <param name="account">The account.</param>
    public async Task SetAccountOfflineAsync(LoggedInAccount account)
    {
        await this._loginServer.LogOffAsync(account.LoginName, account.Server).ConfigureAwait(false);
        var server = this._serverProvider.Servers.FirstOrDefault(s => s.Id == account.Server);
        if (server is IGameServer gameServer)
        {
            await gameServer.DisconnectAccountAsync(account.LoginName).ConfigureAwait(false);
        }

        this._cache.Remove(LookupCacheKey);
        this.DataChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public async Task<List<LoggedInAccount>> GetAsync(int offset, int count)
    {
        var snapshot = await this._loginServer.GetSnapshotAsync().ConfigureAwait(false);
        var playerLookup = await this.GetPlayerLookupAsync().ConfigureAwait(false);
        return snapshot
            .Select(entry =>
            {
                if (playerLookup.TryGetValue(entry.Key, out var playerInfo))
                {
                    return new LoggedInAccount(entry.Key, entry.Value, playerInfo.CharacterName, playerInfo.GuildName, playerInfo.PartyMaster, playerInfo.PartySize, playerInfo.PersistentGuildId);
                }

                return new LoggedInAccount(entry.Key, entry.Value);
            })
            .OrderPartyGrouped()
            .Skip(offset)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Builds a lookup of account login name to character, guild and party info from the in-process game servers.
    /// Empty when the servers run in another process (distributed deployment).
    /// The lookup is cached for a few seconds: building it copies the whole player list of every
    /// game server, while the table only renders one page. External changes (logins, party changes)
    /// become visible with a short delay; the service's own mutations invalidate the cache.
    /// </summary>
    private async Task<Dictionary<string, PlayerInfo>> GetPlayerLookupAsync()
    {
        if (this._cache.TryGetValue<Dictionary<string, PlayerInfo>>(LookupCacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        var result = new Dictionary<string, PlayerInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var context in this._serverProvider.Servers.OfType<IGameServerContextProvider>().Select(s => s.Context))
        {
            var players = await context.GetPlayersAsync().ConfigureAwait(false);
            foreach (var player in players)
            {
                if (player is OfflinePlayer)
                {
                    // Defensive only: offline sessions are logged off from the login server when they start,
                    // so their accounts are normally absent from the snapshot. Bots never log in at all.
                    continue;
                }

                var loginName = player.Account?.LoginName;
                if (string.IsNullOrEmpty(loginName))
                {
                    continue;
                }

                var (partyMaster, partySize) = PartyDisplay.From(player.Party);
                result.TryAdd(loginName, new PlayerInfo(player.SelectedCharacter?.Name, partyMaster, partySize, player.GuildStatus?.GuildId, null));
            }
        }

        var guildIds = result.Values.Select(v => v.GuildId).OfType<uint>().ToList();
        var guildServer = GuildNames.FindServer(this._serverProvider);
        var guildNames = await GuildNames.ResolveAsync(guildServer, guildIds).ConfigureAwait(false);
        var persistentGuildIds = await GuildNames.ResolvePersistentIdsAsync(guildServer, guildIds).ConfigureAwait(false);
        foreach (var key in result.Keys.ToList())
        {
            var info = result[key];
            if (info.GuildId is { } guildId)
            {
                guildNames.TryGetValue(guildId, out var guildName);
                persistentGuildIds.TryGetValue(guildId, out var persistentGuildId);
                result[key] = info with
                {
                    GuildName = guildName,
                    PersistentGuildId = persistentGuildId == Guid.Empty ? null : persistentGuildId,
                };
            }
        }

        this._cache.Set(LookupCacheKey, result, LookupCacheLifetime);
        return result;
    }

    private sealed record PlayerInfo(string? CharacterName, string? PartyMaster, int PartySize, uint? GuildId, string? GuildName, Guid? PersistentGuildId = null);
}