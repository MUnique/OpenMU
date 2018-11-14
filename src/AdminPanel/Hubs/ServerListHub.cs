// <copyright file="ServerListHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Hubs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.AspNetCore.SignalR;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A signalR hub which offers data about the running sub-servers.
    /// </summary>
    public class ServerListHub : Hub<IServerListClient>
    {
        private const string SubscriberGroup = "Subscribers";

        /// <summary>
        /// The class logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerListHub));

        /// <summary>
        /// The servers which are available to the hub.
        /// </summary>
        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerListHub"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public ServerListHub(IList<IManageableServer> servers)
        {
            this.servers = servers;
        }

        /// <summary>
        /// Subscribes to this hub.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task Subscribe()
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, SubscriberGroup);
            var currentServerInfos = this.CreateServerInfos();
            if (currentServerInfos == null)
            {
                Log.Warn("Client connected too early. servers not available yet.");
            }
            else
            {
                await this.Clients.Caller.Initialize(currentServerInfos);
            }
        }

        /// <summary>
        /// Unsubscribes from this hub.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task Unsubscribe()
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, SubscriberGroup);
        }

        /// <summary>
        /// Adds the server to this hub and registers for property change events.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <remarks>Usually just used internally. Currently, there are no servers dynamically added or removed.</remarks>
        public void AddServer(ServerInfo server)
        {
            this.Clients.Group(SubscriberGroup).AddedServer(server);
        }

        /// <summary>
        /// Removes the server from this hub and unregisters from property changed events.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <remarks>Usually just used internally. Currently, there are no servers dynamically added or removed.</remarks>
        public void RemoveServer(int serverId)
        {
            this.Clients.Group(SubscriberGroup).RemovedServer(serverId);
        }

        private IList<ServerInfo> CreateServerInfos()
        {
            if (this.servers == null)
            {
                return null;
            }

            var result = new List<ServerInfo>();

            foreach (var gameServer in this.servers.OfType<IGameServer>().OrderBy(s => s.Id))
            {
                var serverInfo = new GameServerInfo(gameServer);
                result.Add(serverInfo);
            }

            foreach (var server in this.servers.Where(server => !(server is IGameServer)))
            {
                var serverInfo = new ServerInfo(server);
                result.Add(serverInfo);
            }

            return result;
        }
    }
}