// <copyright file="TerrainController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/// <summary>
/// Controller for all map related functions.
/// </summary>
[Route("[controller]")]
[ApiController]
public class TerrainController : Controller
{
    private readonly IObservableGameServer _gameServer;
    private readonly ILogger<TerrainController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerrainController"/> class.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    /// <param name="logger">The logger.</param>
    public TerrainController(IObservableGameServer gameServer, ILogger<TerrainController> logger)
    {
        this._gameServer = gameServer;
        this._logger = logger;
    }

    /// <summary>
    /// Renders and returns the terrain of the specified server and map identifier.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="mapId">The map identifier.</param>
    /// <returns>The rendered terrain.</returns>
    [HttpGet("{serverId}/{mapId}")]
    public async Task<IActionResult> Terrain(int serverId, int mapId)
    {
        var map = this._gameServer.Maps.FirstOrDefault(m => m.MapNumber == mapId);
        if (map is null)
        {
            this._logger.LogWarning($"requested map not available. map number: {mapId}; server id: {serverId}");
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