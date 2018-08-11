// <copyright file="ServerModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.Interfaces;
    using Nancy;

    /// <summary>
    /// <see cref="NancyModule"/> for all server related functions.
    /// </summary>
    public sealed class ServerModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerModule));
        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerModule"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public ServerModule(IList<IManageableServer> servers)
            : base("admin/server")
        {
            this.servers = servers;
            this.Get("shutdown/{serverId:int}", args => this.ShutdownServer(args));
            this.Get("start/{serverId:int}", args => this.StartServer(args));
            this.Get("list", args => this.GetServerList().ToList());
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
                return new GameServerInfo(gameServer);
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