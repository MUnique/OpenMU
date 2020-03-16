// <copyright file="MulticastConnectionServerStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// An implementation of a <see cref="IGameServerStateObserver"/> which forwards registrations to multiple state observers,
    /// e.g when there are multiple connect servers defined for the same game client.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IGameServerStateObserver" />
    internal class MulticastConnectionServerStateObserver : IGameServerStateObserver
    {
        private readonly IList<IGameServerStateObserver> observers = new List<IGameServerStateObserver>();

        /// <inheritdoc />
        public ICollection<(ushort Id, IPEndPoint Endpoint)> GameServerEndPoints => this.observers.SelectMany(o => o.GameServerEndPoints).Distinct().ToList();

        /// <summary>
        /// Adds the observer which wants to get notified about changes.
        /// </summary>
        /// <param name="observer">The observer.</param>
        public void AddObserver(IGameServerStateObserver observer) => this.observers.Add(observer);

        /// <inheritdoc />
        public void RegisterGameServer(IGameServerInfo gameServer, IPEndPoint publicEndPoint)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].RegisterGameServer(gameServer, publicEndPoint);
            }
        }

        /// <inheritdoc />
        public void UnregisterGameServer(IGameServerInfo gameServer)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].UnregisterGameServer(gameServer);
            }
        }
    }
}