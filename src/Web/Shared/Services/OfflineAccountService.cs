// <copyright file="OfflineAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.GameLogic;
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

    /// <inheritdoc />
    public Task<List<OfflineAccount>> GetAsync(int offset, int count)
    {
        var result = this._serverProvider.Servers
            .OfType<IGameServerContextProvider>()
            .SelectMany(s => s.Context.OfflinePlayerManager
                .OfflinePlayers
                .Select(p => new OfflineAccount(
                    p.AccountLoginName ?? string.Empty,
                    (byte)((IManageableServer)s).Id,
                    p.StartTimestamp)))
            .OrderBy(a => a.LoginName)
            .Skip(offset)
            .Take(count)
            .ToList();

        return Task.FromResult(result);
    }
}