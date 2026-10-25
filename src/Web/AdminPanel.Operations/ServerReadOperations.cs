// <copyright file="ServerReadOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Reads server health, statistics and live players in either deployment.</summary>
public sealed class ServerReadOperations(
    IServerProvider servers,
    ILoginServer loginServer,
    IPersistenceContextProvider persistence,
    IDataSource<GameConfiguration> configurations,
    IAdminReadModel readModel,
    IGuildServer? guildServer = null)
{
    /// <summary>Reads the server dashboard as data.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Server states, version, uptime and database health.</returns>
    [AdminOperation(AdminRole.Viewer)]
    public async ValueTask<JsonObject> OverviewAsync(CancellationToken cancellationToken)
    {
        var databaseReachable = await this.CanConnectAsync(cancellationToken).ConfigureAwait(false);
        int? pendingUpdates = databaseReachable ? await readModel.GetPendingUpdatesAsync(cancellationToken).ConfigureAwait(false) : null;
        using var process = Process.GetCurrentProcess();
        return AdminReadResult.From(new
        {
            summary = "Current admin host and server status.",
            version = typeof(ServerReadOperations).Assembly.GetName().Version?.ToString(),
            uptimeSeconds = (DateTime.UtcNow - process.StartTime.ToUniversalTime()).TotalSeconds,
            databaseReachable,
            pendingConfigurationUpdates = pendingUpdates,
            servers = servers.Servers.OrderBy(s => s.Type).ThenBy(s => s.Id).Take(AdminReadResult.MaximumRows).Select(s => new
            {
                s.Id,
                s.Description,
                type = s.Type.ToString(),
                state = s.ServerState.ToString(),
                s.CurrentConnections,
                s.MaximumConnections,
            }),
        });
    }

    /// <summary>Reads account, character and guild counts and recent player history.</summary>
    /// <param name="newAccountDays">The period in days for counting new accounts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The dashboard statistics.</returns>
    [AdminOperation(AdminRole.Viewer)]
    public async ValueTask<AdminStatisticsSnapshot> StatisticsAsync(int newAccountDays, CancellationToken cancellationToken)
    {
        if (newAccountDays is < 1 or > 365)
        {
            throw new ArgumentException("New account period must be between 1 and 365 days.", nameof(newAccountDays));
        }

        var accounts = await this.ReadAccountStatisticsAsync(newAccountDays, cancellationToken).ConfigureAwait(false);
        var guilds = await this.ReadGuildStatisticsAsync(cancellationToken).ConfigureAwait(false);
        return new AdminStatisticsSnapshot("Player accounts exclude bots and templates. History starts when the admin host starts.", newAccountDays, accounts, guilds, readModel.PlayerHistory);
    }

    /// <summary>Lists online accounts, enriched with live character data where available.</summary>
    /// <param name="serverId">The optional game server identifier.</param>
    /// <param name="mapNumber">The optional map number.</param>
    /// <param name="guildName">The optional exact guild name.</param>
    /// <param name="offset">The number of matches to skip.</param>
    /// <param name="limit">The maximum number of matches.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A bounded page of online players with explicit live-data availability.</returns>
    [AdminOperation(AdminRole.Viewer)]
    public async ValueTask<JsonObject> PlayersAsync(int? serverId, ushort? mapNumber, string? guildName, int offset, int limit, CancellationToken cancellationToken)
    {
        AdminReadResult.ValidatePage(offset, limit);
        var online = await loginServer.GetSnapshotAsync().ConfigureAwait(false);
        var live = new Dictionary<string, LivePlayerInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var server in servers.Servers.Where(s => s is IGameServerAdministration or IGameServerContextProvider))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (serverId is not null && server.Id != serverId)
            {
                continue;
            }

            if (server is IGameServerAdministration administration)
            {
                foreach (var player in await administration.GetPlayerSnapshotAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (online.TryGetValue(player.Account, out var owner) && owner == server.Id)
                    {
                        live[player.Account] = player;
                    }
                }
            }
            else if (server is IGameServerContextProvider provider)
            {
                foreach (var player in await provider.Context.GetPlayersAsync().ConfigureAwait(false))
                {
                    if (player is not OfflinePlayer && player.Account?.LoginName is { } loginName)
                    {
                        live[loginName] = new(player.SelectedCharacter?.Id ?? Guid.Empty, loginName, player.Name, player.CurrentMap?.MapId, player.Position.X, player.Position.Y, player.Level, (int)(player.Attributes?[Stats.MasterLevel] ?? 0), player.GuildStatus?.GuildId, false);
                    }
                }
            }
            else
            {
                // Older servers retain account-only status; live filters fail closed below.
            }
        }

        if ((mapNumber is not null || !string.IsNullOrEmpty(guildName))
            && online.Where(e => serverId is null || e.Value == serverId).Any(e => !live.ContainsKey(e.Key)))
        {
            throw new ArgumentException("Map and guild filters require live game server data for every matching account. Wait for character selection or restore the owning server's administration channel.");
        }

        using var guildContext = persistence.CreateNewGuildContext();
        var guildId = string.IsNullOrEmpty(guildName) ? null : await guildContext.GetPersistentGuildIdByNameAsync(guildName).ConfigureAwait(false);
        uint? liveGuildId = null;
        if (guildId is { } persistentId && guildServer is not null)
        {
            liveGuildId = await guildServer.GetGuildIdAsync(persistentId).ConfigureAwait(false);
        }

        var matches = online.Where(e => serverId is null || e.Value == serverId)
            .Where(e => mapNumber is null || (live.TryGetValue(e.Key, out var p) && p.Map == mapNumber))
            .Where(e => string.IsNullOrEmpty(guildName) || (liveGuildId is not null && live.TryGetValue(e.Key, out var p) && p.GuildId == liveGuildId))
            .OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase).ToList();
        var rows = matches.Skip(offset).Take(limit).Select(e =>
        {
            live.TryGetValue(e.Key, out var player);
            return new
            {
                account = e.Key,
                serverId = e.Value,
                liveDataAvailable = player is not null,
                character = player?.Character,
                map = player?.Map,
                x = player?.X,
                y = player?.Y,
                level = player?.Level,
                guildId = player?.GuildId,
            };
        });
        return AdminReadResult.Page(rows, matches.Count, offset, limit);
    }

    /// <summary>Checks database connectivity and reports unresponsive servers.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Read-only diagnostics without repairing or changing anything.</returns>
    [AdminOperation(AdminRole.Viewer)]
    public async ValueTask<JsonObject> DiagnosticsAsync(CancellationToken cancellationToken)
    {
        var databaseReachable = await this.CanConnectAsync(cancellationToken).ConfigureAwait(false);
        var unavailable = servers.Servers.Where(s => s.ServerState == ServerState.Timeout).Select(s => new { s.Id, type = s.Type.ToString() }).Take(AdminReadResult.MaximumRows).ToArray();
        var gameServersWithoutConnectServer = servers.Servers.Any(s => s.Type == ServerType.GameServer)
            && !servers.Servers.Any(s => s.Type == ServerType.ConnectServer && s.ServerState == ServerState.Started);
        int? pendingUpdates = databaseReachable ? await readModel.GetPendingUpdatesAsync(cancellationToken).ConfigureAwait(false) : null;
        return AdminReadResult.From(new
        {
            summary = "Database, heartbeat, connect server and pending update checks. Verify public connect server addresses from a game client outside the server network.",
            healthy = databaseReachable && unavailable.Length == 0 && !gameServersWithoutConnectServer && pendingUpdates == 0,
            databaseReachable,
            unresponsiveServers = unavailable,
            gameServersWithoutConnectServer,
            pendingConfigurationUpdates = pendingUpdates,
            externalAddressReachability = "not_tested",
        });
    }

    /// <summary>Loads account counts independently so the dashboard can still show guild counts on failure.</summary>
    /// <param name="newAccountDays">The period for counting new accounts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account and character counts.</returns>
    internal async ValueTask<AccountStatistics> ReadAccountStatisticsAsync(int newAccountDays, CancellationToken cancellationToken)
    {
        using var context = persistence.CreateNewPlayerContext(await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
        return await context.GetAccountStatisticsAsync(DateTime.UtcNow.AddDays(-newAccountDays), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Loads guild counts independently of account statistics.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The guild and alliance counts.</returns>
    internal async ValueTask<GuildStatistics> ReadGuildStatisticsAsync(CancellationToken cancellationToken)
    {
        using var context = persistence.CreateNewGuildContext();
        return await context.GetGuildStatisticsAsync(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        return persistence is IMigratableDatabaseContextProvider migratable
            && await migratable.CanConnectToDatabaseAsync(cancellationToken).ConfigureAwait(false);
    }
}
