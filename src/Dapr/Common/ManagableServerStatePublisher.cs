// <copyright file="ManagableServerStatePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using global::Dapr.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// A state publisher for a <see cref="IManageableServer"/>,
/// which can be handled with a corresponding <see cref="ManagableServerRegistry"/>.
/// The server registration is deferred to <see cref="StartedAsync"/> which is called after the web application
/// has started (i.e. the HTTP API is already available), breaking the circular startup dependency with the Dapr sidecar.
/// </summary>
public sealed class ManagableServerStatePublisher : IHostedLifecycleService, IDisposable
{
    /// <summary>
    /// The topic name for the state updates.
    /// </summary>
    public const string TopicName = "ServerState";

    private readonly ILogger<ManagableServerStatePublisher> _logger;
    private readonly DaprClient _daprClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly AsyncLock _lock = new();

    private IManageableServer? _server;
    private ServerStateData? _data;

    private Task? _heartbeatTask;
    private CancellationTokenSource? _heartbeatCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagableServerStatePublisher"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="serviceProvider">The service provider used to lazily resolve <see cref="IManageableServer"/>.</param>
    /// <param name="logger">The logger.</param>
    public ManagableServerStatePublisher(DaprClient daprClient, IServiceProvider serviceProvider, ILogger<ManagableServerStatePublisher> logger)
    {
        this._daprClient = daprClient;
        this._serviceProvider = serviceProvider;
        this._logger = logger;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.StopAsync(default).WaitAndUnwrapException();
    }

    /// <inheritdoc />
    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public Task StartedAsync(CancellationToken cancellationToken)
    {
        this._heartbeatCancellationTokenSource = new CancellationTokenSource();

        async Task RunHeartbeatTask()
        {
            try
            {
                await this.HeartbeatLoopAsync(this._heartbeatCancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in heartbeat loop.");
            }
        }

        this._heartbeatTask = RunHeartbeatTask();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await (this._heartbeatCancellationTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._heartbeatCancellationTokenSource?.Dispose();
        if (this._heartbeatTask is { } heartbeatTask)
        {
            this._heartbeatTask = null;
            await heartbeatTask.ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
    {
        await this.InitializeServerAsync(cancellationToken).ConfigureAwait(false);

        while (!cancellationToken.IsCancellationRequested)
        {
            await this.PublishCurrentStateAsync().ConfigureAwait(false);
            await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task InitializeServerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && this._server is null)
        {
            try
            {
                var server = this._serviceProvider.GetRequiredService<IManageableServer>();
                this._data = new ServerStateData(server);
                server.PropertyChanged += this.OnPropertyChanged;
                this._server = server;
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "Could not resolve IManageableServer yet, retrying...");
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task PublishCurrentStateAsync()
    {
        if (this._server is null || this._data is null)
        {
            return;
        }

        using var asyncLock = await this._lock.LockAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        if (asyncLock is null)
        {
            return;
        }

        try
        {
            this._data.UpdateState(this._server);
            await this._daprClient.PublishEventAsync("pubsub", TopicName, this._data).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error sending server status update");
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Exceptions are catched.")]
    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(IManageableServer.ServerState))
            {
                await this.PublishCurrentStateAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when publishing current state after property change");
        }
    }
}