// <copyright file="ServerInfoController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PublicApi.Models;

/// <summary>
/// Controller which provides data about the state of the <see cref="IConnectServer"/>s and its registered <see cref="IGameServer"/>s.
/// </summary>
[Route("[controller]")]
[ApiController]
public class ServerInfoController : ControllerBase
{
    private readonly IEnumerable<IConnectServer> _connectServers;
    private readonly ICollection<IGameServer> _gameServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerInfoController"/> class.
    /// </summary>
    /// <param name="connectServers">The connect servers.</param>
    /// <param name="gameServers">The game servers.</param>
    public ServerInfoController(IEnumerable<IConnectServer> connectServers, ICollection<IGameServer> gameServers)
    {
        this._connectServers = connectServers;
        this._gameServers = gameServers;
    }

    /// <summary>
    /// Gets the data about all <see cref="IConnectServer"/>s.
    /// </summary>
    /// <returns>The data about all <see cref="IConnectServer"/>s.</returns>
    [HttpGet]
    public IList<ConnectServerDto> Get()
    {
        return this._connectServers.Select(cs => ConnectServerDto.Create(cs, this._gameServers)).ToList();
    }

    /// <summary>
    /// Gets the total player count of all game servers.
    /// </summary>
    /// <returns>
    /// The total player count.
    /// </returns>
    [HttpGet("playerCount")]
    public int PlayerCount()
    {
        return this._gameServers.Sum(gs => gs.CurrentConnections);
    }

    /// <summary>
    /// Gets the total player count of the specific realm.
    /// </summary>
    /// <param name="realmIndex">Index of the realm.</param>
    /// <returns>
    /// The total player count.
    /// </returns>
    [HttpGet("{realmIndex:int}/playerCount")]
    public int PlayerCount(int realmIndex)
    {
        const int realmOffset = 20;
        return this._gameServers
            .Where(gs => gs.Id >= realmIndex * realmOffset && gs.Id < (realmIndex + 1) * realmOffset)
            .Sum(gs => gs.CurrentConnections);
    }
}