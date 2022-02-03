using System.Collections.Concurrent;
using System.Net;
using MUnique.OpenMU.Interfaces;

internal class GameServerRegistry
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly List<ServerEndpoint> _gameServerEndPoints = new List<ServerEndpoint>();
    
    private readonly ConcurrentDictionary<ushort, int> _currentConnectionsPerServer = new ();

    public ICollection<(ServerInfo, IPEndPoint)> CreateServerInfos()
    {
        this._semaphore.Wait();
        try
        {
            return this._gameServerEndPoints.Select(e => (new ServerInfo(e.Id, e.Description, GetCurrentConnections(e.Id), e.MaximumConnections), e.EndPoint)).ToList();
        }
        finally
        {
            this._semaphore.Release();
        }
    }


    public void RegisterGameServer(ServerInfo serverInfo, IPEndPoint publicEndPoint, string version)
    {
        this._semaphore.Wait();
        try
        {
            this._gameServerEndPoints.Add(new ServerEndpoint(serverInfo.Id, publicEndPoint, version, serverInfo.MaximumConnections, serverInfo.Description));
            _currentConnectionsPerServer[serverInfo.Id] = serverInfo.CurrentConnections;
        }
        finally
        {
            this._semaphore.Release();
        }
    }

    public void UnregisterGameServer(ushort id)
    {
        this._semaphore.Wait();
        try
        {
            while (this._gameServerEndPoints.FirstOrDefault(e => e.Id == id) is { } endpoint)
            {
                this._gameServerEndPoints.Remove(endpoint);
            }

            this._currentConnectionsPerServer.TryRemove(id, out _);
        }
        finally
        {
            this._semaphore.Release();
        }
    }

    /// <summary>
    /// Is called when the number of <see cref="ServerInfo.CurrentConnections"/> changed for a server.
    /// </summary>
    /// <param name="serverId">The server id.</param>
    /// <param name="currentConnections">The number of current connections, <see cref="ServerInfo.CurrentConnections"/>.</param>
    public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
    {
        this._currentConnectionsPerServer[serverId] = currentConnections;
    }

    public int GetCurrentConnections(ushort serverId)
    {
        if (this._currentConnectionsPerServer.TryGetValue(serverId, out var result))
        {
            return result;
        }

        return 0;
    }
}