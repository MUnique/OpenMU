// <copyright file="MulticastConnectionServerStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.Collections.Concurrent;
using System.Net;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of a <see cref="IGameServerStateObserver"/> which forwards registrations to multiple state observers,
/// e.g when there are multiple connect servers defined for the same game client.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Interfaces.IGameServerStateObserver" />
internal class MulticastConnectionServerStateObserver : IGameServerStateObserver
{
    private readonly MemorizingObserver _memorizingObserver = new();
    private readonly List<IGameServerStateObserver> _observers = new();
    
    public MulticastConnectionServerStateObserver()
    {
        this._observers.Add(this._memorizingObserver);
    }
    /// <summary>
    /// Adds the observer which wants to get notified about changes.
    /// </summary>
    /// <param name="observer">The observer.</param>
    public void AddObserver(IGameServerStateObserver observer) => this._observers.Add(observer);

    /// <inheritdoc />
    public void RegisterGameServer(ServerInfo gameServer, IPEndPoint publicEndPoint)
    {
        for (int i = 0; i < this._observers.Count; i++)
        {
            this._observers[i].RegisterGameServer(gameServer, publicEndPoint);
        }
    }

    /// <inheritdoc />
    public void UnregisterGameServer(ushort gameServerId)
    {
        for (int i = 0; i < this._observers.Count; i++)
        {
            this._observers[i].UnregisterGameServer(gameServerId);
        }
    }

    /// <inheritdoc />
    public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
    {
        for (int i = 0; i < this._observers.Count; i++)
        {
            this._observers[i].CurrentConnectionsChanged(serverId, currentConnections);
        }
    }

    /// <summary>
    /// Pulls the registrations.
    /// </summary>
    /// <param name="observer">The observer.</param>
    public void PullRegistrations(IGameServerStateObserver observer)
    {
        foreach (var (serverInfo, endpoint) in this._memorizingObserver.ServerInfos.Values)
        {
            observer.RegisterGameServer(serverInfo, endpoint);
        }
    }

    private class MemorizingObserver : IGameServerStateObserver
    {
        public ConcurrentDictionary<ushort, (ServerInfo, IPEndPoint)> ServerInfos { get; } = new();

        public void RegisterGameServer(ServerInfo gameServer, IPEndPoint publicEndPoint)
        {
            this.ServerInfos.TryAdd(gameServer.Id, (gameServer, publicEndPoint));
        }

        public void UnregisterGameServer(ushort gameServerId)
        {
            this.ServerInfos.TryRemove(gameServerId, out _);
        }

        public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
        {
            if (this.ServerInfos.TryGetValue(serverId, out var tuple))
            {
                tuple.Item1.CurrentConnections = currentConnections;
            }
        }
    }
}