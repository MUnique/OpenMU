// <copyright file="ApiKeyRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Resolves a presented API key to the client which uses it.
/// </summary>
/// <remarks>
/// There are two sources: the keys which are managed in the admin panel and stored as hashes, and
/// the keys from the configuration. The configured ones stay supported because the API has to work
/// before the database exists - the same reason the admin panel has a bootstrap user.
/// </remarks>
public class ApiKeyRegistry
{
    /// <summary>
    /// The interval in which the <see cref="ApiKey.LastUsedAt"/> of a key is updated at most, so a
    /// busy client doesn't cause a database write on every single request.
    /// </summary>
    private static readonly TimeSpan TouchInterval = TimeSpan.FromMinutes(1);

    private readonly IApiKeyRepository _repository;
    private readonly ILogger<ApiKeyRegistry> _logger;
    private readonly IReadOnlyList<ConfiguredApiKey> _configuredKeys;
    private readonly ConcurrentDictionary<Guid, DateTime> _lastTouched = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyRegistry"/> class.
    /// </summary>
    /// <param name="options">The configured keys.</param>
    /// <param name="repository">The repository of the keys which are managed in the admin panel.</param>
    /// <param name="logger">The logger.</param>
    public ApiKeyRegistry(IOptions<ApiKeyOptions> options, IApiKeyRepository repository, ILogger<ApiKeyRegistry> logger)
    {
        this._repository = repository;
        this._logger = logger;
        this._configuredKeys = this.CreateConfiguredKeys(options.Value);
    }

    /// <summary>
    /// Finds the client which presented the specified key.
    /// </summary>
    /// <param name="presentedKey">The key of the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The client; <c>null</c>, if neither a configured nor a stored key matches.</returns>
    public async ValueTask<ApiKeyClient?> FindAsync(string presentedKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(presentedKey))
        {
            return null;
        }

        if (this.FindConfigured(presentedKey) is { } configuredClient)
        {
            return configuredClient;
        }

        var storedKey = await this._repository
            .GetEnabledByHashAsync(ApiKeyGenerator.Hash(presentedKey), cancellationToken)
            .ConfigureAwait(false);
        if (storedKey is null)
        {
            return null;
        }

        this.TouchInBackground(storedKey);
        return new ApiKeyClient(storedKey.Name, GetEffectiveRoles(storedKey.Roles));
    }

    /// <summary>
    /// Finds the client of a configured key.
    /// </summary>
    /// <param name="presentedKey">The key of the request.</param>
    /// <returns>The client; <c>null</c>, if no configured key matches.</returns>
    /// <remarks>
    /// All configured keys are compared, and each of them in constant time, so neither the duration
    /// of the comparison nor the number of comparisons tells an attacker how much of a guessed key
    /// was right. The stored keys don't need this: they are looked up by their hash.
    /// </remarks>
    private ApiKeyClient? FindConfigured(string presentedKey)
    {
        if (this._configuredKeys.Count == 0)
        {
            return null;
        }

        var presentedBytes = Encoding.UTF8.GetBytes(presentedKey);
        ConfiguredApiKey? match = null;
        foreach (var candidate in this._configuredKeys)
        {
            if (CryptographicOperations.FixedTimeEquals(presentedBytes, candidate.KeyBytes))
            {
                match = candidate;
            }
        }

        return match is null ? null : new ApiKeyClient(match.Name, match.Roles);
    }

    private void TouchInBackground(ApiKey storedKey)
    {
        var now = DateTime.UtcNow;
        var lastTouched = this._lastTouched.GetOrAdd(storedKey.Id, DateTime.MinValue);
        if (now - lastTouched < TouchInterval
            || !this._lastTouched.TryUpdate(storedKey.Id, now, lastTouched))
        {
            return;
        }

        // Deliberately not awaited: the last usage is a convenience for the admin panel and must
        // neither slow a request down nor fail it. The repository swallows its own errors.
        _ = this._repository.TouchAsync(storedKey.Id, now, CancellationToken.None).AsTask();
    }

    private IReadOnlyList<ConfiguredApiKey> CreateConfiguredKeys(ApiKeyOptions options)
    {
        var keys = new List<ConfiguredApiKey>();
        var knownKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in options.Keys)
        {
            var name = string.IsNullOrWhiteSpace(entry.Name) ? $"api-client-{keys.Count + 1}" : entry.Name.Trim();
            if (string.IsNullOrWhiteSpace(entry.Key))
            {
                this._logger.LogWarning("The configured API key of client {ClientName} is empty and is ignored.", name);
                continue;
            }

            if (entry.Key.Length < ApiKeyAuthenticationDefaults.MinimumKeyLength)
            {
                this._logger.LogWarning(
                    "The configured API key of client {ClientName} is shorter than the required {MinimumLength} characters and is ignored.",
                    name,
                    ApiKeyAuthenticationDefaults.MinimumKeyLength);
                continue;
            }

            if (!knownKeys.Add(entry.Key))
            {
                this._logger.LogWarning("The configured API key of client {ClientName} is used by another client as well and is ignored.", name);
                continue;
            }

            keys.Add(new ConfiguredApiKey(name, Encoding.UTF8.GetBytes(entry.Key), GetEffectiveRoles(entry.Roles)));
        }

        return keys;
    }

    private static IReadOnlyList<string> GetEffectiveRoles(string? configuredRoles)
    {
        var assignedRoles = (configuredRoles ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (assignedRoles.Length == 0)
        {
            assignedRoles = [AdminRoles.Viewer];
        }

        return assignedRoles
            .SelectMany(AdminRoles.GetEffectiveRoles)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private sealed record ConfiguredApiKey(string Name, byte[] KeyBytes, IReadOnlyList<string> Roles);
}

/// <summary>
/// An external application which is allowed to use the public API.
/// </summary>
/// <param name="Name">The name of the client.</param>
/// <param name="Roles">The effective roles of the client.</param>
public record ApiKeyClient(string Name, IReadOnlyList<string> Roles)
{
    /// <summary>
    /// Creates the claims of this client.
    /// </summary>
    /// <returns>The claims.</returns>
    public IEnumerable<Claim> CreateClaims()
    {
        yield return new Claim(ClaimTypes.Name, this.Name);
        yield return new Claim(ApiKeyAuthenticationDefaults.ClientNameClaimType, this.Name);
        foreach (var role in this.Roles)
        {
            yield return new Claim(ClaimTypes.Role, role);
        }
    }
}
