// <copyright file="TerrainController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;

    /// <summary>
    /// Controller for all map related functions.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TerrainController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TerrainController));
        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainController"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public TerrainController(IList<IManageableServer> servers)
        {
            this.servers = servers;
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
            var gameServer = this.servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == serverId);

            var map = gameServer?.ServerInfo.Maps.FirstOrDefault(m => m.MapNumber == mapId);
            if (map == null)
            {
                Log.Warn($"requested map not available. map number: {mapId}; server id: {serverId}");
                return this.NotFound();
            }

            this.Response.ContentType = "image/png";
            await using var mapStream = this.RenderMap(map);

            // we need to set the length before writing the data into the body,
            // otherwise it gets "chunked".
            this.Response.ContentLength = mapStream.Length;
            await mapStream.CopyToAsync(this.Response.Body).ConfigureAwait(false);
            return this.Ok();
        }

        private Stream RenderMap(IGameMapInfo map)
        {
            var terrain = new GameMapTerrain(map.MapName, map.TerrainData);
            using var bitmap = new Image<Rgba32>(0x100, 0x100);
            for (int y = 0; y < 0x100; y++)
            {
                for (int x = 0; x < 0x100; x++)
                {
                    var color = Rgba32.Black;
                    if (terrain.SafezoneMap[y, x])
                    {
                        color = Rgba32.Gray;
                    }
                    else if (terrain.WalkMap[y, x])
                    {
                        color = Rgba32.SpringGreen;
                    }
                    else
                    {
                        // we use the default color.
                    }

                    bitmap[x, y] = color;
                }
            }

            var memoryStream = new MemoryStream();
            bitmap.SaveAsPng(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
