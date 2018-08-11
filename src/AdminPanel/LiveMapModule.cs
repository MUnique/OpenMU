// <copyright file="LiveMapModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using Nancy;
    using SkiaSharp;

    /// <summary>
    /// Module for all map related functions.
    /// </summary>
    public sealed class LiveMapModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LiveMapModule));
        private readonly IList<IManageableServer> servers;
        private readonly dynamic model = new ExpandoObject();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveMapModule"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public LiveMapModule(IList<IManageableServer> servers)
            : base("admin/livemap")
        {
            this.servers = servers;
            this.Get("/", _ => this.View["livemap", this.model]);
            this.Get("terrain/{serverId:int}/{mapId:int}", args => this.RenderMap(args));
        }

        private Response RenderMap(dynamic parameters)
        {
            var gameServer = this.servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == (byte)parameters.serverId);

            var map = gameServer?.ServerInfo.Maps.FirstOrDefault(m => m.MapNumber == (short)parameters.mapId);
            if (map == null)
            {
                Log.Warn($"requested map not available. map number: {parameters.mapId}; server id: {parameters.serverId}");
                return null;
            }

            var terrain = new GameMapTerrain(map.MapName, map.TerrainData);
            using (var bitmap = new SkiaSharp.SKBitmap(0x100, 0x100))
            {
                for (int y = 0; y < 0x100; y++)
                {
                    for (int x = 0; x < 0x100; x++)
                    {
                        var color = SKColors.Black;
                        if (terrain.SafezoneMap[y, x])
                        {
                            color = SKColors.Gray;
                        }
                        else if (terrain.WalkMap[y, x])
                        {
                            color = SKColors.SpringGreen;
                        }

                        bitmap.SetPixel(x, y, color);
                    }
                }

                using (var memoryStream = new SKDynamicMemoryWStream())
                {
                    if (SKPixmap.Encode(memoryStream, bitmap, SKEncodedImageFormat.Png, 100))
                    {
                        return this.Response.FromStream(memoryStream.DetachAsData().AsStream, "image/png");
                    }
                }

                return null;
            }
        }
    }
}