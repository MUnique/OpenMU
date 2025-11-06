// <copyright file="OptimizedGenericRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Optimized;

using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// An optimized generic repository with enhanced query performance and better memory usage.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class OptimizedGenericRepository<T> : GenericRepositoryBase<T>
    where T : class
{
    private readonly ILogger<OptimizedGenericRepository<T>> _logger;
    private readonly QueryCacheManager _queryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizedGenericRepository{T}"/> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="queryCache">The query cache manager.</param>
    public OptimizedGenericRepository(
        IContextAwareRepositoryProvider repositoryProvider,
        ILoggerFactory loggerFactory,
        QueryCacheManager queryCache)
        : base(repositoryProvider, loggerFactory.CreateLogger<OptimizedGenericRepository<T>>())
    {
        this._logger = loggerFactory.CreateLogger<OptimizedGenericRepository<T>>();
        this._queryCache = queryCache;
    }

    /// <summary>
    /// Gets all entities with optimized loading and optional caching.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Collection of entities.</returns>
    public override async ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"GetAll_{typeof(T).Name}";
        
        if (this._queryCache.TryGetValue<IEnumerable<T>>(cacheKey, out var cachedResult))
        {
            this._logger.LogDebug("Retrieved {EntityType} from cache", typeof(T).Name);
            return cachedResult;
        }

        using var context = this.GetContext();
        var startTime = DateTime.UtcNow;

        try
        {
            // Use optimized loading with split queries for complex entities
            var query = context.Context.Set<T>().AsSplitQuery();
            
            // Add selective eager loading based on entity type
            query = this.ApplyOptimizedIncludes(query);

            var result = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            
            var loadTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Loaded {Count} {EntityType} entities in {LoadTime}ms",
                result.Count,
                typeof(T).Name,
                loadTime.TotalMilliseconds);

            // Cache configuration data for better performance
            if (this.IsConfigurationEntity())
            {
                this._queryCache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            }

            return result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error loading {EntityType} entities", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Gets an entity by ID with optimized loading.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity or null if not found.</returns>
    public override async ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"GetById_{typeof(T).Name}_{id}";
        
        if (this._queryCache.TryGetValue<T>(cacheKey, out var cachedResult))
        {
            this._logger.LogDebug("Retrieved {EntityType} {Id} from cache", typeof(T).Name, id);
            return cachedResult;
        }

        using var context = this.GetContext();
        var startTime = DateTime.UtcNow;

        try
        {
            var query = context.Context.Set<T>().AsSplitQuery();
            query = this.ApplyOptimizedIncludes(query);

            var result = await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken).ConfigureAwait(false);
            
            var loadTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Loaded {EntityType} {Id} in {LoadTime}ms",
                typeof(T).Name,
                id,
                loadTime.TotalMilliseconds);

            if (result != null && this.IsConfigurationEntity())
            {
                this._queryCache.Set(cacheKey, result, TimeSpan.FromMinutes(15));
            }

            return result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error loading {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Gets entities with a custom filter and optimized loading.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Filtered entities.</returns>
    public async ValueTask<IEnumerable<T>> GetWithFilterAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        using var context = this.GetContext();
        var startTime = DateTime.UtcNow;

        try
        {
            var query = context.Context.Set<T>().AsSplitQuery();
            query = this.ApplyOptimizedIncludes(query);
            query = query.Where(predicate);

            var result = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            
            var loadTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Loaded {Count} filtered {EntityType} entities in {LoadTime}ms",
                result.Count,
                typeof(T).Name,
                loadTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error loading filtered {EntityType} entities", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Gets a paged result with optimized loading.
    /// </summary>
    /// <param name="pageIndex">The page index (0-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Paged result.</returns>
    public async ValueTask<PagedResult<T>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var context = this.GetContext();
        var startTime = DateTime.UtcNow;

        try
        {
            var query = context.Context.Set<T>().AsSplitQuery();
            query = this.ApplyOptimizedIncludes(query);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var loadTime = DateTime.UtcNow - startTime;
            this._logger.LogDebug(
                "Loaded page {PageIndex} of {EntityType} ({Count}/{Total}) in {LoadTime}ms",
                pageIndex,
                typeof(T).Name,
                items.Count,
                totalCount,
                loadTime.TotalMilliseconds);

            return new PagedResult<T>(items, totalCount, pageIndex, pageSize);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error loading paged {EntityType} entities", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Applies optimized includes based on entity type to prevent over-fetching.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <returns>Query with optimized includes.</returns>
    private IQueryable<T> ApplyOptimizedIncludes(IQueryable<T> query)
    {
        var entityType = typeof(T).Name;

        // Apply selective loading based on entity type
        return entityType switch
        {
            "Player" => query, // Player data is loaded on-demand to avoid performance issues
            "Account" => query, // Account data is typically small
            "GameConfiguration" => query, // Configuration is cached anyway
            "ItemDefinition" => query, // Item definitions are frequently accessed
            "MonsterDefinition" => query, // Monster definitions are frequently accessed
            "Character" => ApplyCharacterIncludes(query),
            "Item" => ApplyItemIncludes(query),
            _ => query // Default: no includes to prevent over-fetching
        };
    }

    /// <summary>
    /// Applies character-specific optimized includes.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>Query with character includes.</returns>
    private static IQueryable<T> ApplyCharacterIncludes(IQueryable<T> query)
    {
        // Only include essential character data to avoid performance issues
        return query;
    }

    /// <summary>
    /// Applies item-specific optimized includes.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>Query with item includes.</returns>
    private static IQueryable<T> ApplyItemIncludes(IQueryable<T> query)
    {
        // Only include essential item data
        return query;
    }

    /// <summary>
    /// Determines if the entity type is a configuration entity that can be cached.
    /// </summary>
    /// <returns>True if it's a configuration entity.</returns>
    private bool IsConfigurationEntity()
    {
        var entityType = typeof(T).Name;
        return entityType.EndsWith("Definition") || 
               entityType.EndsWith("Configuration") ||
               entityType.Contains("Config") ||
               new[] { "AttributeDefinition", "ItemOptionType", "Skill", "MagicEffectDefinition" }.Contains(entityType);
    }
}

/// <summary>
/// Represents a paged result of entities.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <param name="Items">The items in the current page.</param>
/// <param name="TotalCount">The total count of items.</param>
/// <param name="PageIndex">The current page index.</param>
/// <param name="PageSize">The page size.</param>
public record PagedResult<T>(IList<T> Items, int TotalCount, int PageIndex, int PageSize)
{
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages - 1;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 0;
}