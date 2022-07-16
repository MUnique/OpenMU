// <copyright file="TerrainController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Controller for all map related functions.
/// </summary>
[Route("/[controller]")]
[ApiController]
public class TerrainController : Controller
{
    private readonly IList<IManageableServer> _servers;
    private readonly ILogger<TerrainController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerrainController"/> class.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <param name="logger">The logger.</param>
    public TerrainController(IList<IManageableServer> servers, ILogger<TerrainController> logger)
    {
        this._servers = servers;
        this._logger = logger;
    }

    /// <summary>
    /// Renders and returns the terrain of the specified server and map identifier.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="mapId">The map identifier.</param>
    /// <returns>The rendered terrain.</returns>
    [HttpGet("{serverId}/{mapId}")]
    public async Task<IActionResult> TerrainAsync(int serverId, Guid mapId)
    {
        if (this._servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == serverId) is not IGameServerContextProvider server)
        {
            this._logger.LogWarning($"requested server not available. server id: {serverId}");
            return this.NotFound();
        }

        // TODO: Do this without creating an ObservableGameServerAdapter, because that's a very expensive operation.
        using var gameServer = new ObservableGameServerAdapter(server.Context);
        await gameServer.InitializeAsync().ConfigureAwait(false);
        var map = gameServer.Maps.FirstOrDefault(m => m.Id == mapId);
        if (map is null)
        {
            this._logger.LogWarning($"requested map not available. map id: {mapId}");
            return this.NotFound();
        }

        this.Response.ContentType = "image/png";
        await using var terrainStream = map.GetTerrainStream();

        // we need to set the length before writing the data into the body,
        // otherwise it gets "chunked".
        this.Response.ContentLength = terrainStream.Length;
        await terrainStream.CopyToAsync(this.Response.Body).ConfigureAwait(false);
        return this.Ok();
    }
}