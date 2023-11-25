// <copyright file="ManagableServerStatePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::Dapr.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A state publisher for a <see cref="IManageableServer"/>,
/// which can be handled with a corresponding <see cref="ManagableServerRegistry"/>.
/// </summary>
public sealed class ManagableServerStatePublisher : IHostedService, IDisposable
{
    /// <summary>
    /// The topic name for the state updates.
    /// </summary>
    public const string TopicName = "ServerState";

    private readonly ILogger<ManagableServerStatePublisher> _logger;
    private readonly DaprClient _daprClient;
    private readonly IManageableServer _server;
    private readonly AsyncLock _lock = new();

    private readonly ServerStateData _data;

    private Task? _heartbeatTask;
    private CancellationTokenSource? _heartbeatCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagableServerStatePublisher"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="server">The server.</param>
    /// <param name="logger">The logger.</param>
    public ManagableServerStatePublisher(DaprClient daprClient, IManageableServer server, ILogger<ManagableServerStatePublisher> logger)
    {
        this._daprClient = daprClient;
        this._server = server;
        this._logger = logger;
        this._server.PropertyChanged += this.OnPropertyChanged;
        this._data = new ServerStateData(this._server);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.StopAsync(default).WaitAndUnwrapException();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
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

    private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        while (!cancellationToken.IsCancellationRequested)
        {
            await this.PublishCurrentStateAsync().ConfigureAwait(false);
            await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task PublishCurrentStateAsync()
    {
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