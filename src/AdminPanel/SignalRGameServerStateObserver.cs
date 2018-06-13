// <copyright file="SignalRGameServerStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using Microsoft.AspNet.SignalR;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Implementation of the <see cref="IServerStateObserver"/> which notifies the <see cref="GlobalHost"/> of the <see cref="ServerListHub"/> about changes of the server.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IServerStateObserver" />
    public class SignalRGameServerStateObserver : IServerStateObserver
    {
        private IHubContext<IServerListClient> HubContext => GlobalHost.ConnectionManager.GetHubContext<ServerListHub, IServerListClient>();

        /// <inheritdoc />
        public IMapStateObserver GetMapStateObserver(int serverId) => new SignalRMapStateObserver(serverId);

        /// <inheritdoc />
        public void PlayerCountChanged(int serverId, int playerCount)
        {
            this.HubContext.Clients.All.PlayerCountChanged(serverId, playerCount);
        }

        /// <inheritdoc />
        public void MapAdded(int serverId, IGameMapInfo mapInfo)
        {
            this.HubContext.Clients.All.MapAdded(new GameServerInfo.GameMapInfo()
            {
                Id = mapInfo.MapNumber,
                Name = mapInfo.MapName,
                PlayerCount = mapInfo.Players.Count,
                ServerId = serverId,
            });
        }

        /// <inheritdoc />
        public void MapRemoved(int serverId, int mapId)
        {
            this.HubContext.Clients.All.MapRemoved(serverId, mapId);
        }

        /// <inheritdoc />
        public void ServerStateChanged(int serverId, ServerState newState)
        {
            this.HubContext.Clients.All.ServerStateChanged(serverId, newState);
        }
    }
}
