// <copyright file="SignalRMapStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using MUnique.OpenMU.AdminPanel.Hubs;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A <see cref="IMapStateObserver"/> which notifies the <see cref="ServerListHub"/> about changes.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IMapStateObserver" />
    public class SignalRMapStateObserver : IMapStateObserver
    {
        private readonly int serverId;

        private IHubContext<ServerListHub> hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRMapStateObserver"/> class.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        public SignalRMapStateObserver(int serverId)
        {
            this.serverId = serverId;
        }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <remarks>
        /// Unfortunately, this looks like an ugly hack :-(
        /// </remarks>
        internal static IServiceProvider Services { get; set; }

        private IHubContext<ServerListHub> HubContext => this.hubContext ?? (this.hubContext = Services.GetService(typeof(IHubContext<ServerListHub>)) as IHubContext<ServerListHub>);

        /// <inheritdoc/>
        public void PlayerCountChanged(int mapId, int playerCount)
        {
            this.HubContext.Clients.All.SendAsync(nameof(IServerListClient.MapPlayerCountChanged), this.serverId, mapId, playerCount);
        }
    }
}
