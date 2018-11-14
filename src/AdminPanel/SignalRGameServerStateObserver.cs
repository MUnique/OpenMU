// <copyright file="SignalRGameServerStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using MUnique.OpenMU.AdminPanel.Hubs;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Implementation of the <see cref="IServerStateObserver"/> which notifies the the <see cref="ServerListHub"/> about changes of the server.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IServerStateObserver" />
    public class SignalRGameServerStateObserver : IServerStateObserver
    {
        private IHubContext<ServerListHub> hubContext;

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <remarks>
        /// Unfortunately, this looks like an ugly hack :-(
        /// </remarks>
        internal static IServiceProvider Services { get; set; }

        private IHubContext<ServerListHub> HubContext => this.hubContext ?? (this.hubContext = Services.GetService(typeof(IHubContext<ServerListHub>)) as IHubContext<ServerListHub>);

        /// <inheritdoc />
        public IMapStateObserver GetMapStateObserver(int serverId) => new SignalRMapStateObserver(serverId);

        /// <inheritdoc />
        public void PlayerCountChanged(int serverId, int playerCount)
        {
            this.HubContext?.Clients.All.SendAsync(nameof(IServerListClient.PlayerCountChanged), serverId, playerCount);
        }

        /// <inheritdoc />
        public void MapAdded(int serverId, IGameMapInfo mapInfo)
        {
            var gameMapInfo = new GameServerInfo.GameMapInfo
            {
                Id = mapInfo.MapNumber,
                Name = mapInfo.MapName,
                PlayerCount = mapInfo.Players.Count,
                ServerId = serverId,
            };
            this.HubContext?.Clients.All.SendAsync(nameof(IServerListClient.MapAdded), gameMapInfo);
        }

        /// <inheritdoc />
        public void MapRemoved(int serverId, int mapId)
        {
            this.HubContext?.Clients.All.SendAsync(nameof(IServerListClient.MapRemoved), serverId, mapId);
        }

        /// <inheritdoc />
        public void ServerStateChanged(int serverId, ServerState newState)
        {
            this.HubContext?.Clients.All.SendAsync(nameof(IServerListClient.ServerStateChanged), serverId, newState);
        }
    }
}
