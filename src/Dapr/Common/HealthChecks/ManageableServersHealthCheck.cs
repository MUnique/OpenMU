// <copyright file="ManageableServersHealthCheck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common.HealthChecks;

using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Reports the states of the <see cref="IManageableServer"/>s of this process.
/// </summary>
/// <remarks>
/// A server which isn't started is reported as degraded, but not as unhealthy: e.g. it may have
/// been stopped in the admin panel on purpose, which is no reason to restart or to bypass the process.
/// </remarks>
public sealed class ManageableServersHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageableServersHealthCheck"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ManageableServersHealthCheck(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IManageableServer> servers;
        try
        {
            servers = this._serviceProvider.GetManageableServers().ToList();
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("The servers are not initialized yet.", ex));
        }

        var states = servers.ToDictionary(server => $"{server.Description} ({server.Id})", server => (object)server.ServerState.ToString());
        var result = servers.All(server => server.ServerState == ServerState.Started)
            ? HealthCheckResult.Healthy(data: states)
            : HealthCheckResult.Degraded("Not all servers are started.", data: states);
        return Task.FromResult(result);
    }
}
