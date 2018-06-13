// <copyright file="SignalRMapStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using Microsoft.AspNet.SignalR;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A <see cref="IMapStateObserver"/> which notifies the <see cref="GlobalHost"/> of the <see cref="ServerListHub"/> about changes.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IMapStateObserver" />
    public class SignalRMapStateObserver : IMapStateObserver
    {
        private readonly int serverId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRMapStateObserver"/> class.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        public SignalRMapStateObserver(int serverId)
        {
            this.serverId = serverId;
        }

        /// <inheritdoc/>
        public void PlayerCountChanged(int mapId, int playerCount)
        {
            GlobalHost.ConnectionManager.GetHubContext<ServerListHub, IServerListClient>().Clients.All.MapPlayerCountChanged(this.serverId, mapId, playerCount);
        }
    }
}
