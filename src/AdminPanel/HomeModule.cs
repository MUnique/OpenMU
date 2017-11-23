// <copyright file="HomeModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
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
    /// The home module.
    /// </summary>
    public class HomeModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HomeModule));

        private readonly IList<IManageableServer> servers;
        private readonly dynamic model = new ExpandoObject();

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeModule"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public HomeModule(IList<IManageableServer> servers)
            : base("/admin")
        {
            this.servers = servers;

            this.Get["/"] = _ => this.View["index", this.model];
            this.Get["log"] = _ => this.View["log", this.model];
            this.Get["livemap"] = _ => this.View["livemap", this.model];
            this.Get["livemap/terrain/{serverId:int}/{mapId:int}"] = this.RenderMap;
            this.Get["loggers"] = this.GetLoggers;
            this.Get["shutdown/{serverId:int}"] = this.ShutdownServer;
            this.Get["start/{serverId:int}"] = this.StartServer;
            this.Get["list"] = _ => this.GetServerList().ToList();
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

        private IEnumerable<string> GetLoggers(dynamic parameters)
        {
            return log4net.LogManager.GetCurrentLoggers().Select(log => log.Logger.Name);
        }

        private IEnumerable<object> GetServerList()
        {
            foreach (var server in this.servers.OrderBy(s => s.Id))
            {
                yield return this.GetServerItem(server);
            }
        }

        private object GetServerItem(IManageableServer server)
        {
            if (server == null)
            {
                return null;
            }

            if (server is IGameServer gameServer)
            {
                return new ExtendedGameServerInfo(gameServer);
            }

            return
                new
                {
                    State = server.ServerState,
                    Description = server.Description,
                    Id = server.Id,
                    OnlinePlayerCount = server.CurrentConnections,
                    MaximumPlayers = server.MaximumConnections
                };
        }

        private object ShutdownServer(dynamic parameters)
        {
            IManageableServer server = null;
            try
            {
                Log.Info($"requested shutdown for server {parameters.serverId}");
                var serverId = (int)parameters.serverId;

                server = this.servers.FirstOrDefault(s => s.Id == serverId);
                server?.Shutdown();
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during requested shutdown of server {parameters.serverId}", ex);
            }

            return this.Negotiate.WithModel(this.Response.AsRedirect("/admin"))
                .WithMediaRangeModel("application/json", this.GetServerItem(server));
        }

        private object StartServer(dynamic parameters)
        {
            IManageableServer server = null;
            try
            {
                Log.Info($"requested start for server {parameters.serverId}");
                var serverId = (int)parameters.serverId;

                server = this.servers.FirstOrDefault(s => s.Id == serverId);
                server?.Start();
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during requested start of server {parameters.serverId}", ex);
            }

            return this.Negotiate.WithModel(this.Response.AsRedirect("/admin"))
                .WithMediaRangeModel("application/json", this.GetServerItem(server));
        }
    }
}