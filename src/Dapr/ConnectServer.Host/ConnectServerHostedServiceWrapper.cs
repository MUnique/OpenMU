using System.Threading;
using Dapr.Client;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.ConnectServer.Host;

public class ConnectServerHostedServiceWrapper : IHostedService
{
    private readonly IConnectServer _connectServer;

    public ConnectServerHostedServiceWrapper(IConnectServer connectServer)
    {
        _connectServer = connectServer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this._connectServer.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._connectServer.StopAsync(cancellationToken);
    }
}