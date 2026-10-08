// <copyright file="DatabaseHealthCheck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common.HealthChecks;

using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Checks if the database is reachable, up to date and contains a game configuration.
/// </summary>
/// <remarks>
/// Before that, a central or game server can't work: e.g. right after the deployment, the
/// database still has to be installed through the setup of the admin panel.
/// </remarks>
public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDatabaseConnectionSettingProvider _connectionSettingProvider;
    private readonly PersistenceContextProvider _persistenceContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseHealthCheck"/> class.
    /// </summary>
    /// <param name="connectionSettingProvider">The connection setting provider.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    public DatabaseHealthCheck(IDatabaseConnectionSettingProvider connectionSettingProvider, PersistenceContextProvider persistenceContextProvider)
    {
        this._connectionSettingProvider = connectionSettingProvider;
        this._persistenceContextProvider = persistenceContextProvider;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (this._connectionSettingProvider.Initialization is not { IsCompletedSuccessfully: true })
        {
            return HealthCheckResult.Unhealthy("The connection settings are not loaded yet.");
        }

        if (!await this._persistenceContextProvider.DatabaseExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return HealthCheckResult.Unhealthy("The database is not reachable or not installed yet.");
        }

        if (!await this._persistenceContextProvider.IsDatabaseUpToDateAsync(cancellationToken).ConfigureAwait(false))
        {
            return HealthCheckResult.Unhealthy("The database schema is not up to date.");
        }

        if (!await this._persistenceContextProvider.ConfigurationExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return HealthCheckResult.Unhealthy("The database contains no game configuration yet.");
        }

        return HealthCheckResult.Healthy();
    }
}
