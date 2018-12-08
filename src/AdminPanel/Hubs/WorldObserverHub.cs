// <copyright file="WorldObserverHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Hubs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A hub with which someone can observe whole <see cref="GameMap"/>s.
    /// </summary>
    public class WorldObserverHub : Hub
    {
        private static readonly IDictionary<string, WorldObserverToHubAdapter> Observers = new Dictionary<string, WorldObserverToHubAdapter>();

        private static readonly ConcurrentQueue<ushort> FreeIds = new ConcurrentQueue<ushort>(Enumerable.Range(ushort.MaxValue - 0x100, 0xFF).Select(id => (ushort)id));

        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldObserverHub" /> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public WorldObserverHub(IList<IManageableServer> servers)
        {
            this.servers = servers;
        }

        /// <summary>
        /// Registers the connected client to listen on the events on the specified map. Is called from the client.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <remarks>
        /// This is a bit dirty... the AdminPanel should not have the reference to  GameLogic.
        /// Instead we need to extract some interfaces.
        /// TODO: It should also be possible to observe multiple maps through one connection.
        /// </remarks>
        public void Subscribe(byte serverId, ushort mapId)
        {
            IGameServer gameServer = this.servers.OfType<IGameServer>().FirstOrDefault(g => g.Id == serverId);
            if (gameServer == null)
            {
                throw new ArgumentException($"unknown server id {serverId}", nameof(serverId));
            }

            if (!FreeIds.TryDequeue(out ushort observerKey))
            {
                throw new OutOfObserverKeysException();
            }

            var clientProxy = this.Clients.Client(this.Context.ConnectionId);

            WorldObserverToHubAdapter observer = new WorldObserverToHubAdapter(observerKey, serverId, mapId, clientProxy);
            Observers.Add(this.Context.ConnectionId, observer);

            try
            {
                gameServer.RegisterMapObserver(mapId, observer);
            }
            catch (ArgumentException)
            {
                Observers.Remove(this.Context.ConnectionId);
                observer.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Unsubscribes from this hub.
        /// </summary>
        public void Unsubscribe()
        {
            if (Observers.TryGetValue(this.Context.ConnectionId, out WorldObserverToHubAdapter observer))
            {
                Observers.Remove(this.Context.ConnectionId);
                var gameServer = this.servers.OfType<IGameServer>().First(g => g.Id == observer.ServerId);
                gameServer.UnregisterMapObserver(observer.MapId, observer.Id);
                FreeIds.Enqueue(observer.Id);
                observer.Dispose();
            }
        }

        /// <inheritdoc />
        public override Task OnDisconnectedAsync(Exception exception)
        {
            this.Unsubscribe();
            return base.OnDisconnectedAsync(exception);
        }

        public class OutOfObserverKeysException : Exception
        {
        }
    }
}
