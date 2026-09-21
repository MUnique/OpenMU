// <copyright file="BotAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

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
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotAccountService"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    public BotAccountService(IServerProvider serverProvider)
    {
        this._serverProvider = serverProvider;
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
    public async Task<List<BotAccount>> GetAsync(int offset, int count)
    {
        var result = new List<BotAccount>();
        foreach (var server in this._serverProvider.Servers.OfType<IGameServerContextProvider>())
        {
            var serverId = (byte)((IManageableServer)server).Id;
            var players = await server.Context.GetPlayersAsync().ConfigureAwait(false);
            result.AddRange(players
                .OfType<BotPlayer>()
                .Select(p => new BotAccount(
                    p.Account?.LoginName ?? string.Empty,
                    serverId,
                    p.SelectedCharacter?.Name,
                    p.StartTimestamp,
                    p.Party?.PartyMaster?.Name,
                    p.Party?.PartyList.Count ?? 0)));
        }

        return result
            .OrderPartyGrouped()
            .Skip(offset)
            .Take(count)
            .ToList();
    }
}
