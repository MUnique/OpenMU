// <copyright file="ManagableServerRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A registry for all <see cref="IManageableServer"/>s in the system.
/// </summary>
public class ManagableServerRegistry : IServerProvider, IDisposable
{
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly ILogger<ManagableServerRegistry> _logger;
    private readonly DaprClient _daprClient;
    private readonly ConcurrentDictionary<int, ManageableServerClient> _serverClients = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagableServerRegistry" /> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public ManagableServerRegistry(DaprClient daprClient, ILogger<ManagableServerRegistry> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;

        async Task RunTimeoutLoop()
        {
            try
            {
                await this.TimeoutLoopAsync(this._disposeCts.Token);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in cleanup loop");
            }
        }

        _ = RunTimeoutLoop();
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IList<IManageableServer> Servers => this._serverClients.Values.OfType<IManageableServer>().ToList();

    /// <summary>
    /// Handles an update of a server state.
    /// </summary>
    /// <param name="serverData">The server data.</param>
    public void HandleUpdate(ServerStateData serverData)
    {
        var isNew = false;
        this._serverClients.AddOrUpdate(
            serverData.Id,
            _ =>
        {
            isNew = true;
            return new ManageableServerClient(this._daprClient, serverData);
        },
            (_, client) =>
            {
                client.Update(serverData);
                return client;
            });

        if (isNew)
        {
            this.RaisePropertyChanged(nameof(this.Servers));
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeCts.Cancel();
        this._disposeCts.Dispose();
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with the specified parameters.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task TimeoutLoopAsync(CancellationToken cancellationToken)
    {
        while (!this._disposeCts.IsCancellationRequested)
        {
            await Task.Delay(2000, cancellationToken);

            foreach (var server in this._serverClients.Values)
            {
                var diff = DateTime.UtcNow - server.LastUpdate;
                if (diff > this._timeout)
                {
                    this._logger.LogInformation("Difference of {0} higher than timeout for server {1}", diff, server.Id);
                    server.ServerState = ServerState.Timeout;
                }
            }
        }
    }
}