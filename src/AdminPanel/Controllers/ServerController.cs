// <copyright file="ServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The controller for the server list of the admin panel.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("admin/[controller]/[action]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerController));
        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerController"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public ServerController(IList<IManageableServer> servers)
        {
            this.servers = servers;
        }

        /// <summary>
        /// Gets a list of all servers.
        /// </summary>
        /// <returns>A list of all servers.</returns>
        [HttpGet]
        public ActionResult<List<object>> List()
        {
            return this.servers.Select(this.GetServerItem).ToList();
        }

        /// <summary>
        /// Starts the specified server.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>The new current server data.</returns>
        [HttpGet("{serverId}")]
        public ActionResult<object> Start(int serverId)
        {
            try
            {
                Log.Info($"requested start for server {serverId}");
                var server = this.servers.FirstOrDefault(s => s.Id == serverId);
                if (server != null)
                {
                    server.Start();
                    return this.Ok(this.GetServerItem(server));
                }

                return this.NotFound();
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during requested start of server {serverId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Shuts the specified server down.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>The new current server data.</returns>
        [HttpGet("{serverId}")]
        public ActionResult<object> Shutdown(int serverId)
        {
            try
            {
                Log.Info($"requested shutdown for server {serverId}");
                var server = this.servers.FirstOrDefault(s => s.Id == serverId);
                if (server != null)
                {
                    server.Shutdown();
                    return this.Ok(this.GetServerItem(server));
                }

                return this.NotFound();
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during requested shutdown of server {serverId}", ex);
                throw;
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

            if (server is IConnectServer connectServer)
            {
                return new ConnectServerInfo(connectServer);
            }

            return new ServerInfo(server);
        }
    }
}
