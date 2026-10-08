// <copyright file="DiscordGameDataProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Provides the data of the game for the Discord bot, through the interfaces of the servers and the database.
/// It works in-process and with the servers in separate processes.
/// </summary>
public sealed class DiscordGameDataProvider : IDiscordGameDataProvider
{
    /// <summary>
    /// The time span in which the upcoming events are shown.
    /// </summary>
    private static readonly TimeSpan UpcomingEventsTimeSpan = TimeSpan.FromHours(24);

    private readonly IServerProvider _serverProvider;
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly IFriendServer _friendServer;
    private readonly IGuildServer _guildServer;
    private readonly PlugInManager _plugInManager;
    private readonly Func<TimeZoneInfo> _getServerTimeZone;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordGameDataProvider"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="friendServer">The friend server, which knows the online state of the characters.</param>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="plugInManager">The plugin manager, with the active plugins which run the events.</param>
    /// <param name="getServerTimeZone">The function which gets the time zone of the server, in which the schedules of the events are defined.</param>
    public DiscordGameDataProvider(
        IServerProvider serverProvider,
        IPersistenceContextProvider persistenceContextProvider,
        IFriendServer friendServer,
        IGuildServer guildServer,
        PlugInManager plugInManager,
        Func<TimeZoneInfo> getServerTimeZone)
    {
        this._serverProvider = serverProvider;
        this._persistenceContextProvider = persistenceContextProvider;
        this._friendServer = friendServer;
        this._guildServer = guildServer;
        this._plugInManager = plugInManager;
        this._getServerTimeZone = getServerTimeZone;
    }

    /// <inheritdoc />
    public IReadOnlyList<GameServerStatus> GetGameServers()
    {
        return this._serverProvider.Servers
            .OfType<IGameServer>()
            .OrderBy(server => server.Id)
            .Select(server => new GameServerStatus(
                server.Id,
                server.Description,
                server.ServerState == ServerState.Started,
                server.CurrentConnections,
                server.MaximumConnections))
            .ToList();
    }

    /// <inheritdoc />
    public async ValueTask<CharacterInfo?> GetCharacterAsync(string name)
    {
        CharacterSummary? summary;
        using (var context = this.CreatePlayerContext())
        {
            summary = await context.GetCharacterSummaryAsync(name, Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id).ConfigureAwait(false);
        }

        if (summary is null)
        {
            return null;
        }

        var serverId = await this._friendServer.GetOnlineServerIdAsync(summary.Name).ConfigureAwait(false);
        return ToInfo(summary, serverId is { } id ? this.GetServerName(id) : null);
    }

    /// <inheritdoc />
    public async ValueTask<GuildInfo?> GetGuildAsync(string name)
    {
        if (await this._guildServer.GetPersistentGuildIdByNameAsync(name).ConfigureAwait(false) is not { } persistentId
            || await this._guildServer.GetGuildIdAsync(persistentId).ConfigureAwait(false) is not (> 0 and var guildId)
            || await this._guildServer.GetGuildAsync(guildId).ConfigureAwait(false) is not { } guild)
        {
            return null;
        }

        var members = await this._guildServer.GetGuildListAsync(guildId).ConfigureAwait(false);
        return new GuildInfo(
            guild.Name ?? name,
            members.FirstOrDefault(member => member.PlayerPosition == GuildPosition.GuildMaster)?.PlayerName,
            members.Count,
            members
                .Where(member => member.ServerId != (byte)SpecialServerId.Offline && member.ServerId != (byte)SpecialServerId.Invisible)
                .Select(member => member.PlayerName)
                .OfType<string>()
                .ToList());
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<UpcomingEventInfo>> GetUpcomingEventsAsync(CultureInfo culture)
    {
        IReadOnlyList<UpcomingEventInfo> events = UpcomingEvents
            .Get(this._plugInManager, DateTime.UtcNow, this._getServerTimeZone(), UpcomingEventsTimeSpan, culture)
            .Select(e => new UpcomingEventInfo(e.Name, e.StartsAtUtc))
            .ToList();
        return ValueTask.FromResult(events);
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<CharacterInfo>> GetRankingAsync(int count)
    {
        using var context = this.CreatePlayerContext();
        var ranking = await context.GetCharacterRankingAsync(Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, count).ConfigureAwait(false);
        return ranking.Select(summary => ToInfo(summary, null)).ToList();
    }

    private static CharacterInfo ToInfo(CharacterSummary summary, string? onlineServerName)
        => new(summary.Name, summary.CharacterClassName, summary.Level, summary.MasterLevel, summary.Resets, onlineServerName);

    private IPlayerContext CreatePlayerContext()
    {
        // The queries only read summaries, so they don't need the actual game configuration,
        // which would have to be loaded completely otherwise.
        return this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration());
    }

    private string GetServerName(byte serverId)
    {
        return this._serverProvider.Servers.OfType<IGameServer>().FirstOrDefault(server => server.Id == serverId)?.Description
               ?? serverId.ToString(CultureInfo.InvariantCulture);
    }
}
