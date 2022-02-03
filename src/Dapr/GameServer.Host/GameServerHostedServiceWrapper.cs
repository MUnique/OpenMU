using System.Threading;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

namespace MUnique.OpenMU.GameServer.Host;

public class GameServerHostedServiceWrapper : IHostedService
{
    private readonly IGameServer _gameServer;
    private readonly GameServerInitializer _initializer;
    private bool _isInitialized;

    public GameServerHostedServiceWrapper(IGameServer gameServer, GameServerInitializer initializer)
    {
        _gameServer = gameServer;
        _initializer = initializer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!this._isInitialized)
        {
            this._initializer.Initialize();
            this._isInitialized = true;
        }

        return this._gameServer.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._gameServer.StopAsync(cancellationToken);
    }
}