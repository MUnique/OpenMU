// <copyright file="OptimizedConnectionManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Optimized;

using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

/// <summary>
/// Manages database connections with improved pooling and performance monitoring.
/// </summary>
public class OptimizedConnectionManager : IDisposable
{
    private readonly ILogger<OptimizedConnectionManager> _logger;
    private readonly ConnectionPoolMetrics _metrics;
    private readonly Timer _metricsTimer;
    private readonly ConcurrentDictionary<string, ConnectionPoolInfo> _connectionPools = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizedConnectionManager"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public OptimizedConnectionManager(ILogger<OptimizedConnectionManager> logger)
    {
        this._logger = logger;
        this._metrics = new ConnectionPoolMetrics();
        
        // Log metrics every 30 seconds
        this._metricsTimer = new Timer(this.LogMetrics, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Configures the DbContext with optimized connection settings.
    /// </summary>
    /// <param name="optionsBuilder">The options builder.</param>
    /// <param name="connectionString">The connection string.</param>
    public void ConfigureOptimizedConnection(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        
        // Optimize connection string for performance
        builder.Pooling = true;
        builder.MinPoolSize = 5;
        builder.MaxPoolSize = 100;
        builder.ConnectionLifetime = 300; // 5 minutes
        builder.ConnectionIdleLifetime = 60; // 1 minute
        builder.KeepAlive = 30; // 30 seconds
        builder.CommandTimeout = 30;
        builder.Timeout = 15;
        
        // Performance optimizations
        builder.NoResetOnClose = true;
        builder.ReadBufferSize = 8192;
        builder.WriteBufferSize = 8192;
        
        var optimizedConnectionString = builder.ToString();
        
        optionsBuilder.UseNpgsql(optimizedConnectionString, options =>
        {
            options.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
            
            options.CommandTimeout(30);
        });

        // Enable sensitive data logging in development only
        #if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        #endif

        // Configure query optimization
        optionsBuilder.ConfigureWarnings(warnings =>
        {
            warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.FirstWithoutOrderByAndFilterWarning);
            warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning);
        });

        this.TrackConnectionPool(builder.Host ?? "localhost", builder.Database ?? "default");
        
        this._logger.LogDebug("Configured optimized database connection for {Database}", builder.Database);
    }

    /// <summary>
    /// Gets connection pool metrics for monitoring.
    /// </summary>
    /// <returns>Current connection pool metrics.</returns>
    public ConnectionPoolMetrics GetMetrics()
    {
        return this._metrics;
    }

    /// <summary>
    /// Gets detailed connection pool information.
    /// </summary>
    /// <returns>Dictionary of connection pool information by pool identifier.</returns>
    public Dictionary<string, ConnectionPoolInfo> GetConnectionPoolInfo()
    {
        return new Dictionary<string, ConnectionPoolInfo>(this._connectionPools);
    }

    /// <summary>
    /// Tracks connection pool usage for a specific database.
    /// </summary>
    /// <param name="host">The database host.</param>
    /// <param name="database">The database name.</param>
    private void TrackConnectionPool(string host, string database)
    {
        var poolId = $"{host}:{database}";
        this._connectionPools.AddOrUpdate(poolId,
            new ConnectionPoolInfo
            {
                Host = host,
                Database = database,
                CreatedAt = DateTime.UtcNow,
                LastUsed = DateTime.UtcNow
            },
            (_, existing) =>
            {
                existing.LastUsed = DateTime.UtcNow;
                existing.ConnectionCount++;
                return existing;
            });
    }

    /// <summary>
    /// Logs connection pool metrics for monitoring.
    /// </summary>
    /// <param name="state">Timer state (not used).</param>
    private void LogMetrics(object? state)
    {
        try
        {
            var activePools = this._connectionPools.Values.ToList();
            if (activePools.Count == 0)
            {
                return;
            }

            var totalConnections = activePools.Sum(p => p.ConnectionCount);
            var avgConnectionsPerPool = activePools.Average(p => p.ConnectionCount);
            var oldestPool = activePools.OrderBy(p => p.CreatedAt).FirstOrDefault();

            this._logger.LogInformation(
                "Database Connection Metrics: {PoolCount} pools, {TotalConnections} total connections, " +
                "avg {AvgConnections:F1} per pool, oldest pool: {OldestAge:F1}min",
                activePools.Count,
                totalConnections,
                avgConnectionsPerPool,
                oldestPool != null ? (DateTime.UtcNow - oldestPool.CreatedAt).TotalMinutes : 0);

            // Update global metrics
            this._metrics.ActivePools = activePools.Count;
            this._metrics.TotalConnections = totalConnections;
            this._metrics.AverageConnectionsPerPool = avgConnectionsPerPool;
            this._metrics.LastUpdated = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error logging connection pool metrics");
        }
    }

    /// <summary>
    /// Disposes the connection manager and its resources.
    /// </summary>
    public void Dispose()
    {
        this._metricsTimer?.Dispose();
        this._connectionPools.Clear();
    }
}

/// <summary>
/// Information about a specific connection pool.
/// </summary>
public class ConnectionPoolInfo
{
    /// <summary>
    /// Gets or sets the database host.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the pool was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the pool was last used.
    /// </summary>
    public DateTime LastUsed { get; set; }

    /// <summary>
    /// Gets or sets the current connection count.
    /// </summary>
    public int ConnectionCount { get; set; }
}

/// <summary>
/// Metrics about database connection pools.
/// </summary>
public class ConnectionPoolMetrics
{
    /// <summary>
    /// Gets or sets the number of active connection pools.
    /// </summary>
    public int ActivePools { get; set; }

    /// <summary>
    /// Gets or sets the total number of connections across all pools.
    /// </summary>
    public int TotalConnections { get; set; }

    /// <summary>
    /// Gets or sets the average number of connections per pool.
    /// </summary>
    public double AverageConnectionsPerPool { get; set; }

    /// <summary>
    /// Gets or sets when these metrics were last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}