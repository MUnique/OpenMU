// <copyright file="OptimizedDbContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Optimized;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

/// <summary>
/// An optimized database context with enhanced performance features.
/// </summary>
public class OptimizedDbContext : EntityDataContext
{
    private readonly ILogger<OptimizedDbContext> _logger;
    private readonly QueryCacheManager _queryCache;
    private readonly OptimizedConnectionManager _connectionManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizedDbContext"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="queryCache">The query cache manager.</param>
    /// <param name="connectionManager">The connection manager.</param>
    public OptimizedDbContext(
        ILogger<OptimizedDbContext> logger,
        QueryCacheManager queryCache,
        OptimizedConnectionManager connectionManager)
    {
        this._logger = logger;
        this._queryCache = queryCache;
        this._connectionManager = connectionManager;
        
        this.ConfigureOptimizations();
    }

    /// <summary>
    /// Saves changes with performance optimizations and monitoring.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var changeCount = this.ChangeTracker.Entries().Count(e => e.State != EntityState.Unchanged);

        try
        {
            // Pre-save optimizations
            this.OptimizeBeforeSave();

            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            var saveTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Saved {ChangeCount} changes to database in {SaveTime}ms",
                changeCount,
                saveTime.TotalMilliseconds);

            // Invalidate related cache entries
            this.InvalidateRelatedCaches();

            return result;
        }
        catch (Exception ex)
        {
            var saveTime = DateTime.UtcNow - startTime;
            this._logger.LogError(ex,
                "Failed to save {ChangeCount} changes to database after {SaveTime}ms",
                changeCount,
                saveTime.TotalMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Executes a bulk update operation with optimized performance.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    public async Task<int> BulkUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : class
    {
        var startTime = DateTime.UtcNow;
        var entityList = entities.ToList();

        try
        {
            this.Set<T>().UpdateRange(entityList);
            var result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            var updateTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Bulk updated {Count} {EntityType} entities in {UpdateTime}ms",
                entityList.Count,
                typeof(T).Name,
                updateTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            var updateTime = DateTime.UtcNow - startTime;
            this._logger.LogError(ex,
                "Failed to bulk update {Count} {EntityType} entities after {UpdateTime}ms",
                entityList.Count,
                typeof(T).Name,
                updateTime.TotalMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Executes a bulk delete operation with optimized performance.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    public async Task<int> BulkDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : class
    {
        var startTime = DateTime.UtcNow;
        var entityList = entities.ToList();

        try
        {
            this.Set<T>().RemoveRange(entityList);
            var result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            var deleteTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Bulk deleted {Count} {EntityType} entities in {DeleteTime}ms",
                entityList.Count,
                typeof(T).Name,
                deleteTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            var deleteTime = DateTime.UtcNow - startTime;
            this._logger.LogError(ex,
                "Failed to bulk delete {Count} {EntityType} entities after {DeleteTime}ms",
                entityList.Count,
                typeof(T).Name,
                deleteTime.TotalMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Configures database-specific optimizations.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!ConnectionConfigurator.IsInitialized)
        {
            ConnectionConfigurator.Initialize(new ConfigFileDatabaseConnectionStringProvider());
        }

        base.OnConfiguring(optionsBuilder);
        
        // Apply optimized connection settings
        var connectionString = ConnectionConfigurator.Instance.GetConnectionString(this.GetType());
        this._connectionManager.ConfigureOptimizedConnection(optionsBuilder, connectionString);
    }

    /// <summary>
    /// Configures various performance optimizations for the context.
    /// </summary>
    private void ConfigureOptimizations()
    {
        // Configure change tracking behavior for better performance
        this.ChangeTracker.LazyLoadingEnabled = false;
        this.ChangeTracker.AutoDetectChangesEnabled = false;
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        // Configure entity state change logging
        this.ChangeTracker.StateChanged += this.OnEntityStateChanged;
        this.ChangeTracker.Tracked += this.OnEntityTracked;
    }

    /// <summary>
    /// Optimizes the context before saving changes.
    /// </summary>
    private void OptimizeBeforeSave()
    {
        // Enable auto-detection temporarily for save
        var wasAutoDetectEnabled = this.ChangeTracker.AutoDetectChangesEnabled;
        this.ChangeTracker.AutoDetectChangesEnabled = true;
        
        try
        {
            this.ChangeTracker.DetectChanges();
        }
        finally
        {
            this.ChangeTracker.AutoDetectChangesEnabled = wasAutoDetectEnabled;
        }

        // Log save operation details
        var additions = this.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
        var modifications = this.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
        var deletions = this.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted);

        if (additions > 0 || modifications > 0 || deletions > 0)
        {
            this._logger.LogDebug(
                "Preparing to save: {Additions} additions, {Modifications} modifications, {Deletions} deletions",
                additions,
                modifications,
                deletions);
        }
    }

    /// <summary>
    /// Invalidates cache entries that might be affected by the recent changes.
    /// </summary>
    private void InvalidateRelatedCaches()
    {
        var changedEntityTypes = this.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Unchanged)
            .Select(e => e.Entity.GetType().Name)
            .Distinct()
            .ToList();

        foreach (var entityType in changedEntityTypes)
        {
            // Invalidate GetAll cache for this entity type
            this._queryCache.RemoveByPattern($"GetAll_{entityType}*");
            
            // Invalidate GetById cache for this entity type
            this._queryCache.RemoveByPattern($"GetById_{entityType}*");
            
            this._logger.LogDebug("Invalidated cache entries for entity type: {EntityType}", entityType);
        }
    }

    /// <summary>
    /// Handles entity state change events for monitoring.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnEntityStateChanged(object? sender, EntityStateChangedEventArgs e)
    {
        this._logger.LogTrace(
            "Entity {EntityType} state changed from {OldState} to {NewState}",
            e.Entry.Entity.GetType().Name,
            e.OldState,
            e.Entry.State);
    }

    /// <summary>
    /// Handles entity tracking events for monitoring.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnEntityTracked(object? sender, EntityTrackedEventArgs e)
    {
        this._logger.LogTrace(
            "Entity {EntityType} is now being tracked with state {State}",
            e.Entry.Entity.GetType().Name,
            e.Entry.State);
    }
}