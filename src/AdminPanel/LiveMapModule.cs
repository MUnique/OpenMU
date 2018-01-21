// <copyright file="LiveMapModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using Nancy;

    /// <summary>
    /// Module for all map related functions.
    /// </summary>
    public class LiveMapModule : NancyModule
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
            this.Get["/"] = _ => this.View["livemap", this.model];
            this.Get["terrain/{serverId:int}/{mapId:int}"] = this.RenderMap;
        }

        private Response RenderMap(dynamic parameters)
        {
            var gameServer = this.servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == (byte)parameters.serverId);

            var map = gameServer?.ServerInfo.Maps.FirstOrDefault(m => m.Map.Number == (short)parameters.mapId);
            if (map == null)
            {
                Log.Warn($"requested map not available. map number: {parameters.mapId}; server id: {parameters.serverId}");
                return null;
            }

            var terrain = new GameMapTerrain(map.Map);
            using (var bitmap = new Bitmap(0x100, 0x100, PixelFormat.Format32bppArgb))
            {
                for (int y = 0; y < 0x100; y++)
                {
                    for (int x = 0; x < 0x100; x++)
                    {
                        Color color = Color.FromArgb(unchecked((int)0xFF000000));
                        if (terrain.SafezoneMap[y, x])
                        {
                            color = Color.FromArgb(unchecked((int)0xFF808080));
                        }
                        else if (terrain.WalkMap[y, x])
                        {
                            color = Color.FromArgb(unchecked((int)0xFF00FF7F));
                        }

                        bitmap.SetPixel(x, y, color);
                    }
                }

                var memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                var response = this.Response.FromStream(memoryStream, "image/png");
                return response;
            }
        }
    }
}