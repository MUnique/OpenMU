// <copyright file="QueryCacheManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Optimized;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

/// <summary>
/// Manages caching of frequently accessed query results to improve database performance.
/// </summary>
public class QueryCacheManager
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly ILogger<QueryCacheManager> _logger;
    private readonly Timer _cleanupTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCacheManager"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public QueryCacheManager(ILogger<QueryCacheManager> logger)
    {
        this._logger = logger;
        
        // Run cleanup every 5 minutes
        this._cleanupTimer = new Timer(this.CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Gets a cached value if it exists and is not expired.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The cached value if found.</param>
    /// <returns>True if the value was found and is not expired.</returns>
    public bool TryGetValue<T>(string key, out T value)
    {
        if (this._cache.TryGetValue(key, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
        {
            if (entry.Value is T typedValue)
            {
                entry.LastAccessedAt = DateTime.UtcNow;
                entry.AccessCount++;
                value = typedValue;
                this._logger.LogDebug("Cache hit for key: {Key} (access #{AccessCount})", key, entry.AccessCount);
                return true;
            }
        }

        value = default!;
        this._logger.LogDebug("Cache miss for key: {Key}", key);
        return false;
    }

    /// <summary>
    /// Sets a value in the cache with the specified expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The expiration time.</param>
    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        var entry = new CacheEntry
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.Add(expiration),
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow,
            AccessCount = 0
        };

        this._cache.AddOrUpdate(key, entry, (_, _) => entry);
        this._logger.LogDebug("Cached value for key: {Key} (expires at {ExpiresAt})", key, entry.ExpiresAt);
    }

    /// <summary>
    /// Removes a specific key from the cache.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <returns>True if the key was removed.</returns>
    public bool Remove(string key)
    {
        var removed = this._cache.TryRemove(key, out _);
        if (removed)
        {
            this._logger.LogDebug("Removed key from cache: {Key}", key);
        }
        return removed;
    }

    /// <summary>
    /// Removes all keys that match the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match (simple wildcard support with *).</param>
    /// <returns>The number of keys removed.</returns>
    public int RemoveByPattern(string pattern)
    {
        var keysToRemove = this._cache.Keys
            .Where(key => this.MatchesPattern(key, pattern))
            .ToList();

        var removedCount = 0;
        foreach (var key in keysToRemove)
        {
            if (this._cache.TryRemove(key, out _))
            {
                removedCount++;
            }
        }

        this._logger.LogDebug("Removed {Count} keys matching pattern: {Pattern}", removedCount, pattern);
        return removedCount;
    }

    /// <summary>
    /// Clears all cached entries.
    /// </summary>
    public void Clear()
    {
        var count = this._cache.Count;
        this._cache.Clear();
        this._logger.LogInformation("Cleared all {Count} cached entries", count);
    }

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    /// <returns>Cache statistics.</returns>
    public CacheStatistics GetStatistics()
    {
        var now = DateTime.UtcNow;
        var entries = this._cache.Values.ToList();

        return new CacheStatistics
        {
            TotalEntries = entries.Count,
            ExpiredEntries = entries.Count(e => e.ExpiresAt <= now),
            TotalAccessCount = entries.Sum(e => e.AccessCount),
            AverageAge = entries.Any() ? entries.Average(e => (now - e.CreatedAt).TotalMinutes) : 0,
            MostAccessedKey = entries
                .OrderByDescending(e => e.AccessCount)
                .Select(e => this._cache.FirstOrDefault(kvp => kvp.Value == e).Key)
                .FirstOrDefault()
        };
    }

    /// <summary>
    /// Cleans up expired entries from the cache.
    /// </summary>
    /// <param name="state">Timer state (not used).</param>
    private void CleanupExpiredEntries(object? state)
    {
        var now = DateTime.UtcNow;
        var expiredKeys = this._cache
            .Where(kvp => kvp.Value.ExpiresAt <= now)
            .Select(kvp => kvp.Key)
            .ToList();

        var removedCount = 0;
        foreach (var key in expiredKeys)
        {
            if (this._cache.TryRemove(key, out _))
            {
                removedCount++;
            }
        }

        if (removedCount > 0)
        {
            this._logger.LogDebug("Cleaned up {Count} expired cache entries", removedCount);
        }
    }

    /// <summary>
    /// Checks if a key matches a pattern with simple wildcard support.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <param name="pattern">The pattern with * wildcards.</param>
    /// <returns>True if the key matches the pattern.</returns>
    private bool MatchesPattern(string key, string pattern)
    {
        if (pattern == "*")
        {
            return true;
        }

        if (!pattern.Contains('*'))
        {
            return key == pattern;
        }

        var parts = pattern.Split('*');
        var keyIndex = 0;

        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (string.IsNullOrEmpty(part))
            {
                continue;
            }

            var partIndex = key.IndexOf(part, keyIndex, StringComparison.Ordinal);
            if (partIndex == -1)
            {
                return false;
            }

            if (i == 0 && partIndex != 0)
            {
                return false; // First part must be at the beginning
            }

            keyIndex = partIndex + part.Length;
        }

        // If the pattern ends with *, we don't need to check the end
        if (!pattern.EndsWith('*') && keyIndex != key.Length)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Disposes the cache manager and its resources.
    /// </summary>
    public void Dispose()
    {
        this._cleanupTimer?.Dispose();
        this.Clear();
    }

    /// <summary>
    /// Represents a cached entry.
    /// </summary>
    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessedAt { get; set; }
        public int AccessCount { get; set; }
    }
}

/// <summary>
/// Statistics about the cache performance.
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Gets or sets the total number of entries in the cache.
    /// </summary>
    public int TotalEntries { get; set; }

    /// <summary>
    /// Gets or sets the number of expired entries.
    /// </summary>
    public int ExpiredEntries { get; set; }

    /// <summary>
    /// Gets or sets the total access count across all entries.
    /// </summary>
    public long TotalAccessCount { get; set; }

    /// <summary>
    /// Gets or sets the average age of cache entries in minutes.
    /// </summary>
    public double AverageAge { get; set; }

    /// <summary>
    /// Gets or sets the key of the most accessed cache entry.
    /// </summary>
    public string? MostAccessedKey { get; set; }

    /// <summary>
    /// Gets the cache hit ratio (valid entries / total entries).
    /// </summary>
    public double HitRatio => TotalEntries > 0 ? (double)(TotalEntries - ExpiredEntries) / TotalEntries : 0;
}