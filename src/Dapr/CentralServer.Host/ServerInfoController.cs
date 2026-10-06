// <copyright file="ServerInfoController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using Microsoft.AspNetCore.Mvc;
using ConnectServer = MUnique.OpenMU.ConnectServer.ConnectServer;

/// <summary>
/// API Controller which provides information about the connection and game servers.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ServerInfoController : ControllerBase
{
    private const int RealmOffset = 20;

    private readonly ConnectServerCollection _connectServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerInfoController"/> class.
    /// </summary>
    /// <param name="connectServers">The connect servers.</param>
    public ServerInfoController(ConnectServerCollection connectServers)
    {
        this._connectServers = connectServers;
    }

    /// <summary>
    /// Gets the complete information about the connect servers and all known online game servers.
    /// </summary>
    /// <returns>
    /// The complete information about the first connect server and its game servers, and in
    /// <c>ConnectServers</c> the same for each connect server, i.e. for each client version.
    /// </returns>
    [HttpGet]
    public object? GetCompleteInfo()
    {
        if (this._connectServers.Count == 0)
        {
            return null;
        }

        var first = this._connectServers[0];
        return new
        {
            PatchAddress = first.Settings.PatchAddress,
            CurrentPatchVersion = first.Settings.CurrentPatchVersion,
            Version = first.Settings.Client.Version,
            Season = first.Settings.Client.Season,
            Episode = first.Settings.Client.Episode,
            Port = first.Settings.ClientListenerPort,
            State = first.ServerState,
            GameServers = GetGameServers(first),
            ConnectServers = this._connectServers.Select(connectServer => new
            {
                connectServer.Description,
                connectServer.Settings.PatchAddress,
                connectServer.Settings.CurrentPatchVersion,
                connectServer.Settings.Client.Version,
                connectServer.Settings.Client.Season,
                connectServer.Settings.Client.Episode,
                Port = connectServer.Settings.ClientListenerPort,
                State = connectServer.ServerState,
                GameServers = GetGameServers(connectServer),
            }).ToList(),
        };
    }

    /// <summary>
    /// Gets the connection count of all game servers.
    /// </summary>
    /// <returns>The overall count of current connections.</returns>
    [HttpGet("playerCount")]
    public int GetOverallConnectionCount()
    {
        return this.GetDistinctGameServers().Sum(gs => gs.CurrentConnections);
    }

    /// <summary>
    /// Gets the connection count of all game servers of a realm.
    /// </summary>
    /// <param name="realmIndex">Index of the realm.</param>
    /// <returns>The connection count of all game servers of a realm.</returns>
    [HttpGet("{realmIndex}/playerCount")]
    public int GetRealmConnectionCount(byte realmIndex)
    {
        return this.GetDistinctGameServers()
            .Where(gs => gs.ServerId >= realmIndex * RealmOffset && gs.ServerId < (realmIndex + 1) * RealmOffset)
            .Sum(gs => gs.CurrentConnections);
    }

    private static object GetGameServers(ConnectServer connectServer)
    {
        return connectServer.RegisteredGameServers
            .OrderBy(gs => gs.ServerId)
            .Select(gs => new
            {
                gs.ServerId,
                EndPoint = gs.EndPoint.ToString(),
                gs.ServerLoadPercentage,
                gs.CurrentConnections,
            }).ToList();
    }

    /// <summary>
    /// Gets the game servers of all connect servers. A game server which is listed at multiple
    /// connect servers, i.e. for multiple client versions, is only counted once.
    /// </summary>
    private IEnumerable<(ushort ServerId, int CurrentConnections)> GetDistinctGameServers()
    {
        return this._connectServers
            .SelectMany(connectServer => connectServer.RegisteredGameServers)
            .Select(gs => (gs.ServerId, gs.CurrentConnections))
            .DistinctBy(gs => gs.ServerId);
    }
}
