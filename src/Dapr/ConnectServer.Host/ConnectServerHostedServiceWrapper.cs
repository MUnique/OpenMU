// <copyright file="ConnectServerHostedServiceWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.Host;

using System.Threading;
using Microsoft.Extensions.Hosting;

/// <summary>
/// A wrapper which takes a <see cref="Interfaces.IConnectServer"/> and wraps it as <see cref="IHostedService"/>,
/// so that additional initialization can be done before actually starting it.
/// TODO: listen to configuration changes/database reinit.
/// See also: ServerContainerBase.
/// </summary>
public class ConnectServerHostedServiceWrapper : IHostedService
{
    private readonly ConnectServer _connectServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerHostedServiceWrapper"/> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    public ConnectServerHostedServiceWrapper(ConnectServer connectServer)
    {
        this._connectServer = connectServer;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this._connectServer.StartAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._connectServer.StopAsync(cancellationToken);
    }
}