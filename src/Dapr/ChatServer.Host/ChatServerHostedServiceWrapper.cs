// <copyright file="ChatServerHostedServiceWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Host;

using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;
using ChatServer = MUnique.OpenMU.ChatServer.ChatServer;

/// <summary>
/// A wrapper which takes a <see cref="ChatServer"/> and wraps it as <see cref="IHostedLifecycleService"/>,
/// so that additional initialization can be done before actually starting it.
/// The actual server start is deferred to <see cref="StartedAsync"/> which is called after the web application
/// has started (i.e. the HTTP API is already available), breaking the circular startup dependency with the Dapr sidecar.
/// TODO: listen to configuration changes/database reinit.
/// See also: ServerContainerBase.
/// </summary>
public class ChatServerHostedServiceWrapper : IHostedLifecycleService
{
    private readonly IServiceProvider _serviceProvider;
    private ChatServer? _chatServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServerHostedServiceWrapper"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ChatServerHostedServiceWrapper(IServiceProvider serviceProvider)
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

        var settings = this._serviceProvider.GetRequiredService<ChatServerDefinition>().ConvertToSettings();
        this._chatServer = this._serviceProvider.GetRequiredService<ChatServer>();
        this._chatServer.Initialize(settings);
        await this._chatServer.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._chatServer?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
