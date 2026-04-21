// <copyright file="ConnectServerHostedServiceWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.Host;

using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Dapr.Common;

/// <summary>
/// A wrapper which takes a <see cref="Interfaces.IConnectServer"/> and wraps it as <see cref="IHostedLifecycleService"/>,
/// so that additional initialization can be done before actually starting it.
/// The actual server start is deferred to <see cref="StartedAsync"/> which is called after the web application
/// has started (i.e. the HTTP API is already available), breaking the circular startup dependency with the Dapr sidecar.
/// TODO: listen to configuration changes/database reinit.
/// See also: ServerContainerBase.
/// </summary>
public class ConnectServerHostedServiceWrapper : IHostedLifecycleService
{
    private readonly IServiceProvider _serviceProvider;
    private ConnectServer? _connectServer;

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
        this._connectServer = this._serviceProvider.GetRequiredService<ConnectServer>();
        await this._connectServer.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._connectServer?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}