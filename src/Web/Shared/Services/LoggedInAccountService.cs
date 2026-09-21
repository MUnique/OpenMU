// <copyright file="LoggedInAccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Services for the <see cref="LoggedInAccount"/> page.
/// </summary>
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
                    return new LoggedInAccount(entry.Key, entry.Value, playerInfo.CharacterName, playerInfo.PartyMaster, playerInfo.PartySize);
                }

                return new LoggedInAccount(entry.Key, entry.Value);
            })
            .OrderPartyGrouped()
            .Skip(offset)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Builds a lookup of account login name to character and party info from the in-process game servers.
    /// Empty when the servers run in another process (distributed deployment).
    /// </summary>
    private async Task<Dictionary<string, PlayerInfo>> GetPlayerLookupAsync()
    {
        var result = new Dictionary<string, PlayerInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var context in this._serverProvider.Servers.OfType<IGameServerContextProvider>().Select(s => s.Context))
        {
            var players = await context.GetPlayersAsync().ConfigureAwait(false);
            foreach (var player in players)
            {
                if (player is OfflinePlayer)
                {
                    continue; // offline sessions and bots are shown on their own tabs.
                }

                var loginName = player.Account?.LoginName;
                if (string.IsNullOrEmpty(loginName) || result.ContainsKey(loginName))
                {
                    continue;
                }

                result[loginName] = new PlayerInfo(player.SelectedCharacter?.Name, player.Party?.PartyMaster?.Name, player.Party?.PartyList.Count ?? 0);
            }
        }

        return result;
    }

    private sealed record PlayerInfo(string? CharacterName, string? PartyMaster, int PartySize);
}