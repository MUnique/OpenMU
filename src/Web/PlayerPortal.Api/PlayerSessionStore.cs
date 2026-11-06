// <copyright file="PlayerSessionStore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.Security.Cryptography;

/// <summary>Stores opaque sessions in memory, retaining only token hashes and fixed expirations.</summary>
public sealed class PlayerSessionStore(TimeProvider timeProvider)
{
    /// <summary>The maximum number of active sessions held by one process.</summary>
    public const int MaximumSessions = 100_000;

    private readonly object _sync = new();
    private readonly Dictionary<string, StoredSession> _sessions = new(StringComparer.Ordinal);

    /// <summary>Creates a 256-bit opaque session and stores only its SHA-256 digest.</summary>
    /// <param name="accountId">The authenticated OpenMU account identifier.</param>
    /// <param name="lifetime">The fixed session lifetime.</param>
    /// <returns>The opaque token and its expiry timestamp.</returns>
    public PlayerSessionDescriptor Create(Guid accountId, TimeSpan lifetime)
    {
        var token = Base64Url(RandomNumberGenerator.GetBytes(32));
        var expiresAt = timeProvider.GetUtcNow().Add(lifetime);
        var hash = HashToken(token);

        lock (this._sync)
        {
            this.RemoveExpiredSessions(timeProvider.GetUtcNow());
            if (this._sessions.Count >= MaximumSessions)
            {
                throw new InvalidOperationException("The player session capacity is full.");
            }

            this._sessions.Add(hash, new StoredSession(accountId, expiresAt));
        }

        return new PlayerSessionDescriptor(token, expiresAt);
    }

    /// <summary>Resolves a presented token to its account while rejecting malformed or expired tokens.</summary>
    /// <param name="token">The opaque token presented by the Worker.</param>
    /// <param name="accountId">The resolved owner ID when the token is valid.</param>
    /// <returns><c>true</c> when the token resolves to a non-expired session.</returns>
    public bool TryGetAccountId(string? token, out Guid accountId)
    {
        accountId = Guid.Empty;
        if (token is null || token.Length != 43 || token.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '-' or '_')))
        {
            return false;
        }

        var hash = HashToken(token);
        lock (this._sync)
        {
            if (!this._sessions.TryGetValue(hash, out var session))
            {
                return false;
            }

            if (session.ExpiresAt <= timeProvider.GetUtcNow())
            {
                this._sessions.Remove(hash);
                return false;
            }

            accountId = session.AccountId;
            return true;
        }
    }

    /// <summary>Revokes a presented token. Returns false for an unknown or expired token.</summary>
    /// <param name="token">The opaque token to revoke.</param>
    /// <returns><c>true</c> when an active token was removed.</returns>
    public bool Revoke(string? token)
    {
        if (token is null || token.Length != 43 || token.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '-' or '_')))
        {
            return false;
        }

        lock (this._sync)
        {
            var hash = HashToken(token);
            if (!this._sessions.TryGetValue(hash, out var session))
            {
                return false;
            }

            if (session.ExpiresAt <= timeProvider.GetUtcNow())
            {
                this._sessions.Remove(hash);
                return false;
            }

            return this._sessions.Remove(hash);
        }
    }

    private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private void RemoveExpiredSessions(DateTimeOffset now)
    {
        foreach (var (hash, session) in this._sessions)
        {
            if (session.ExpiresAt <= now)
            {
                this._sessions.Remove(hash);
            }
        }
    }

    private sealed record StoredSession(Guid AccountId, DateTimeOffset ExpiresAt);
}
