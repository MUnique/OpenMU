// <copyright file="LoggedInAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Services for the <see cref="LoggedIn"/> page.
/// </summary>
/// <seealso cref="LoggedInAccount" />
/// <seealso cref="MUnique.OpenMU.Web.AdminPanel.Services.ISupportDataChangedNotification" />
public class LoggedInAccountService : IDataService<LoggedInAccount>, ISupportDataChangedNotification
{
    private readonly ILoginServer _loginServer;
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggedInAccountService"/> class.
    /// </summary>
    /// <param name="loginServer">The login server.</param>
    /// <param name="serverProvider">The server provider.</param>
    public LoggedInAccountService(ILoginServer loginServer, IServerProvider serverProvider)
    {
        this._loginServer = loginServer;
        this._serverProvider = serverProvider;
    }

    /// <inheritdoc />
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

        this.DataChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public async Task<List<LoggedInAccount>> GetAsync(int offset, int count)
    {
        var snapshot = await this._loginServer.GetSnapshotAsync().ConfigureAwait(false);
        return snapshot
            .Select(entry => new LoggedInAccount(entry.Key, entry.Value))
            .OrderBy(e => e.LoginName)
            .Skip(offset)
            .Take(count)
            .ToList();
    }
}