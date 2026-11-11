// <copyright file="PlayerLoginAttemptLimiter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>Applies a bounded process-local login attempt limit. The edge Worker must also rate-limit by visitor IP.</summary>
public sealed class PlayerLoginAttemptLimiter(TimeProvider timeProvider)
{
    /// <summary>The number of login attempts accepted per normalized account identifier in each window.</summary>
    public const int MaximumAttempts = 10;

    /// <summary>The fixed login attempt window.</summary>
    public static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    private const int MaximumBuckets = 10_000;
    private readonly object _sync = new();
    private readonly Dictionary<string, AttemptBucket> _buckets = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Consumes one attempt or returns the remaining wait time when the account bucket is full.</summary>
    /// <param name="loginName">The validated game login identifier.</param>
    /// <param name="retryAfter">The duration until this identifier's window resets, or zero on acceptance.</param>
    /// <returns><c>true</c> when this attempt was accepted.</returns>
    public bool TryConsume(string loginName, out TimeSpan retryAfter)
    {
        var now = timeProvider.GetUtcNow();
        var key = loginName.ToUpperInvariant();
        lock (this._sync)
        {
            if (this._buckets.TryGetValue(key, out var bucket) && now - bucket.StartedAt >= Window)
            {
                this._buckets.Remove(key);
                bucket = null;
            }

            if (bucket is not null && bucket.Attempts >= MaximumAttempts)
            {
                retryAfter = bucket.StartedAt.Add(Window) - now;
                return false;
            }

            if (bucket is null)
            {
                this.RemoveExpiredBuckets(now);
                if (this._buckets.Count >= MaximumBuckets)
                {
                    retryAfter = Window;
                    return false;
                }

                bucket = new AttemptBucket(now, 0);
                this._buckets.Add(key, bucket);
            }

            this._buckets[key] = bucket with { Attempts = bucket.Attempts + 1 };
            retryAfter = TimeSpan.Zero;
            return true;
        }
    }

    private void RemoveExpiredBuckets(DateTimeOffset now)
    {
        foreach (var (key, bucket) in this._buckets)
        {
            if (now - bucket.StartedAt >= Window)
            {
                this._buckets.Remove(key);
            }
        }
    }

    private sealed record AttemptBucket(DateTimeOffset StartedAt, int Attempts);
}
