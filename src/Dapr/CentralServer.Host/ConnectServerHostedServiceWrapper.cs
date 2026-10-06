// <copyright file="ConnectServerHostedServiceWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Dapr.Common;
using ConnectServer = MUnique.OpenMU.ConnectServer.ConnectServer;

/// <summary>
/// A wrapper which takes the <see cref="ConnectServerCollection"/> and wraps it as <see cref="IHostedLifecycleService"/>,
/// so that additional initialization can be done before actually starting it.
/// The actual server start is deferred to <see cref="StartedAsync"/> which is called after the web application
/// has started (i.e. the HTTP API is already available), breaking the circular startup dependency with the Dapr sidecar.
/// TODO: listen to configuration changes/database reinit.
/// See also: ServerContainerBase.
/// </summary>
public class ConnectServerHostedServiceWrapper : IHostedLifecycleService
{
    private readonly IServiceProvider _serviceProvider;
    private ConnectServerCollection? _connectServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerHostedServiceWrapper"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ConnectServerHostedServiceWrapper(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        await this._serviceProvider.WaitForDatabaseInitializationAsync(cancellationToken).ConfigureAwait(false);
        this._connectServers = this._serviceProvider.GetRequiredService<ConnectServerCollection>();
        foreach (var connectServer in this._connectServers)
        {
            await connectServer.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var connectServer in this._connectServers ?? Enumerable.Empty<ConnectServer>())
        {
            await connectServer.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}