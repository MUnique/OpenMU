// <copyright file="GameServerHostedServiceWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.Map;

/// <summary>
/// A wrapper which takes a <see cref="Interfaces.IGameServer"/> and wraps it as <see cref="IHostedLifecycleService"/>,
/// so that additional initialization can be done before actually starting it.
/// The actual server start is deferred to <see cref="StartedAsync"/> which is called after the web application
/// has started (i.e. the HTTP API is already available), breaking the circular startup dependency with the Dapr sidecar.
/// TODO: listen to configuration changes/database reinit.
/// See also: ServerContainerBase.
/// </summary>
public class GameServerHostedServiceWrapper : IHostedLifecycleService
{
    private readonly IServiceProvider _serviceProvider;
    private IGameServer? _gameServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerHostedServiceWrapper"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public GameServerHostedServiceWrapper(IServiceProvider serviceProvider)
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

        if (this._serviceProvider.GetService<ICollection<PlugInConfiguration>>() is List<PlugInConfiguration> plugInConfigurations)
        {
            await this._serviceProvider.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);
        }

        this._gameServer = this._serviceProvider.GetRequiredService<IGameServer>();
        var initializer = this._serviceProvider.GetRequiredService<GameServerInitializer>();
        await initializer.InitializeAsync().ConfigureAwait(false);

        await ((ObservableGameServerAdapter)this._serviceProvider.GetRequiredService<IObservableGameServer>())
            .InitializeAsync().ConfigureAwait(false);

        await this._gameServer.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._gameServer?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}