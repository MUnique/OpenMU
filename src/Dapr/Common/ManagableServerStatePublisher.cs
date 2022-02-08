using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;
using Nito.AsyncEx.Synchronous;

namespace MUnique.OpenMU.Dapr.Common
{
    public sealed class ManagableServerStatePublisher : IHostedService, IDisposable
    {
        public const string TopicName = "ServerState";

        private readonly ILogger<ManagableServerStatePublisher> _logger;
        private readonly DaprClient _daprClient;
        private readonly IManageableServer _server;
        private readonly SemaphoreSlim _semaphore = new (1);

        private readonly ServerStateData _data;

        private Task? _heartbeatTask;
        private CancellationTokenSource? _heartbeatCancellationTokenSource;

        public ManagableServerStatePublisher(DaprClient daprClient, IManageableServer server, ILogger<ManagableServerStatePublisher> logger)
        {
            this._daprClient = daprClient;
            this._server = server;
            this._logger = logger;
            this._server.PropertyChanged += this.OnPropertyChanged;
            this._data = new ServerStateData(this._server);
        }

        public void Dispose()
        {
            this.StopAsync(default).WaitAndUnwrapException();
            this._semaphore.Dispose();
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._heartbeatCancellationTokenSource = new CancellationTokenSource();
            this._heartbeatTask = Task.Run(
                async () =>
                {
                    try
                    {
                        await this.HeartbeatLoop(this._heartbeatCancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error in heartbeat loop.");
                    }
                });
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._heartbeatCancellationTokenSource?.Cancel();
            this._heartbeatCancellationTokenSource?.Dispose();
            this._heartbeatTask = null;
            return Task.CompletedTask;
        }

        private async Task HeartbeatLoop(CancellationToken cancellationToken)
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
            if (!await this._semaphore.WaitAsync(TimeSpan.FromSeconds(1)))
            {
                return;
            }

            try
            {
                this._data.UpdateState(this._server);
                await this._daprClient.PublishEventAsync("pubsub", TopicName, this._data);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error sending server status update");
            }
            finally
            {
                this._semaphore.Release();
            }
        }

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(IManageableServer.ServerState))
                {
                    await this.PublishCurrentStateAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error when publishing current state after property change");
            }
        }
    }
}
