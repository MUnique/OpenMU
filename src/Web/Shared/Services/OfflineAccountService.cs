// <copyright file="OfflineAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Service for the offline player table on the <c>LoggedIn</c> page.
/// </summary>
public class OfflineAccountService : IDataService<OfflineAccount>, ISupportDataChangedNotification
{
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineAccountService"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    public OfflineAccountService(IServerProvider serverProvider)
    {
        this._serverProvider = serverProvider;
    }

    /// <summary>
    /// Event raised when the data has changed.
    /// </summary>
    public event EventHandler? DataChanged;

    /// <summary>
    /// Stops the offline session for the given account.
    /// </summary>
    /// <param name="account">The account whose session should be stopped.</param>
    public async Task StopOfflinePlayerAsync(OfflineAccount account)
    {
        var server = this._serverProvider.Servers.FirstOrDefault(s => s.Id == account.ServerId);
        if (server is IGameServer gameServer)
        {
            await gameServer.DisconnectAccountAsync(account.LoginName).ConfigureAwait(false);
        }

        this.DataChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Determines whether the <c>/offlevel</c> chat command plugin is active on any in-process game server.
    /// </summary>
    public bool IsOfflevelFeatureAvailable()
    {
        return this._serverProvider.Servers
            .OfType<IGameServerContextProvider>()
            .Any(s => s.Context.PlugInManager.IsPlugInActive(typeof(OfflineLevelingChatCommandPlugIn)));
    }

    /// <inheritdoc />
    public async Task<List<OfflineAccount>> GetAsync(int offset, int count)
    {
        // Note: bots never show up here - they are managed by the BotManager, not the OfflinePlayerManager.
        // Materialized once: guild names resolve in bulk afterwards, and a second snapshot could disagree.
        var rows = this._serverProvider.Servers
            .OfType<IGameServerContextProvider>()
            .SelectMany(s => s.Context.OfflinePlayerManager
                .OfflinePlayers
                .Select(p => (ServerId: (byte)((IManageableServer)s).Id, Player: p)))
            .ToList();

        var guildNames = await GuildNames.ResolveAsync(
                GuildNames.FindServer(this._serverProvider),
                rows.Select(r => r.Player.GuildStatus?.GuildId).OfType<uint>())
            .ConfigureAwait(false);

        return rows
            .Select(r =>
            {
                var (partyMaster, partySize) = PartyDisplay.From(r.Player.Party);
                var guildId = r.Player.GuildStatus?.GuildId;
                return new OfflineAccount(
                    r.Player.AccountLoginName ?? string.Empty,
                    r.ServerId,
                    r.Player.StartTimestamp,
                    r.Player.SelectedCharacter?.Name,
                    guildId is { } id ? guildNames.GetValueOrDefault(id) : null,
                    partyMaster,
                    partySize);
            })
            .OrderPartyGrouped()
            .Skip(offset)
            .Take(count)
            .ToList();
    }
}