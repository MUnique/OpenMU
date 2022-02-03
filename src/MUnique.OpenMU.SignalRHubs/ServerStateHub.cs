using System.Net;

using Microsoft.AspNetCore.SignalR;
using MUnique.OpenMU.Interfaces;

internal record ServerEndpoint(ushort Id, IPEndPoint EndPoint, string Version, int MaximumConnections, string Description);

// todo: this is bullshit, because we want to use pubsub of dapr!!!
internal class ServerStateHub : Hub<IGameServerStateObserver>
{
    private readonly GameServerRegistry _registry;

    public ServerStateHub(GameServerRegistry registry)
    {
        _registry = registry;
    }

    public void RequestEndpoints()
    {
        foreach (var (serverInfo, ipEndPoint) in this._registry.CreateServerInfos())
        {
            this.Clients.Caller.RegisterGameServer(serverInfo, ipEndPoint);
        }
    }

    public void RegisterGameServer(ServerInfo serverInfo, IPEndPoint publicEndPoint, string version)
    {
        this._registry.RegisterGameServer(serverInfo, publicEndPoint, version);
        this.Clients.All.RegisterGameServer(serverInfo, publicEndPoint);
    }

    public void UnregisterGameServer(ushort serverId)
    {
        this._registry.UnregisterGameServer(serverId);
        this.Clients.All.UnregisterGameServer(serverId);
    }

    /// <summary>
    /// Is called when the number of <see cref="ServerInfo.CurrentConnections"/> changed for a server.
    /// </summary>
    /// <param name="serverId">The server id.</param>
    /// <param name="currentConnections">The number of current connections, <see cref="ServerInfo.CurrentConnections"/>.</param>
    public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
    {
        this.Clients.All.CurrentConnectionsChanged(serverId, currentConnections);
    }
}
