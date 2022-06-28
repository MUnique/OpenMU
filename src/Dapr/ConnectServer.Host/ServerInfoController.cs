// <copyright file="ServerInfoController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.Host;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// API Controller which provides information about the connection and game servers.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ServerInfoController : ControllerBase
{
    private readonly ConnectServer _connectServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerInfoController"/> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    public ServerInfoController(ConnectServer connectServer)
    {
        this._connectServer = connectServer;
    }

    /// <summary>
    /// Gets the complete information about the connection server and all known online game servers.
    /// </summary>
    /// <returns>The complete information about the connection server and all known online game servers.</returns>
    [HttpGet]
    public object GetCompleteInfo()
    {
        return new
        {
            PatchAddress = this._connectServer.Settings.PatchAddress,
            CurrentPatchVersion = this._connectServer.Settings.CurrentPatchVersion,
            Version = this._connectServer.Settings.Client.Version,
            Season = this._connectServer.Settings.Client.Season,
            Episode = this._connectServer.Settings.Client.Episode,
            Port = this._connectServer.Settings.ClientListenerPort,
            State = this._connectServer.ServerState,
            GameServers = this._connectServer.RegisteredGameServers
                .OrderBy(gs => gs.ServerId)
                .Select(gs => new
                {
                    gs.ServerId,
                    EndPoint = gs.EndPoint.ToString(),
                    gs.ServerLoadPercentage,
                    gs.CurrentConnections,
                }).ToList(),
        };
    }

    /// <summary>
    /// Gets the connection count of all game servers.
    /// </summary>
    /// <returns></returns>
    [HttpGet("playerCount")]
    public int GetOverallConnectionCount()
    {
        return this._connectServer.CurrentGameServerConnections;
    }

    /// <summary>
    /// Gets the connection count of all game servers of a realm.
    /// </summary>
    /// <param name="realmIndex">Index of the realm.</param>
    /// <returns>The connection count of all game servers of a realm.</returns>
    [HttpGet("{realmIndex}/playerCount")]
    public int GetRealmConnectionCount(byte realmIndex)
    {
        const int realmOffset = 20;

        return this._connectServer.RegisteredGameServers
            .Where(gs => gs.ServerId >= realmIndex * realmOffset && gs.ServerId < (realmIndex + 1) * realmOffset)
            .Sum(gs => gs.CurrentConnections);
    }
}