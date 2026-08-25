// <copyright file="ApiKeyRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Holds the configured API keys and resolves a presented key to its client.
/// </summary>
public class ApiKeyRegistry
{
    private readonly ILogger<ApiKeyRegistry> _logger;
    private readonly IReadOnlyList<ApiKeyClient> _clients;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyRegistry"/> class.
    /// </summary>
    /// <param name="options">The configured keys.</param>
    /// <param name="logger">The logger.</param>
    public ApiKeyRegistry(IOptions<ApiKeyOptions> options, ILogger<ApiKeyRegistry> logger)
    {
        this._logger = logger;
        this._clients = this.CreateClients(options.Value);
    }

    /// <summary>
    /// Gets a value indicating whether any usable key is configured.
    /// </summary>
    public bool IsConfigured => this._clients.Count > 0;

    /// <summary>
    /// Finds the client which presented the specified key.
    /// </summary>
    /// <param name="presentedKey">The key of the request.</param>
    /// <returns>The client; <c>null</c>, if no configured key matches.</returns>
    /// <remarks>
    /// All configured keys are compared, and each of them in constant time, so neither the
    /// duration of the comparison nor the number of comparisons tells an attacker how much of a
    /// guessed key was right.
    /// </remarks>
    public ApiKeyClient? Find(string presentedKey)
    {
        if (string.IsNullOrEmpty(presentedKey))
        {
            return null;
        }

        var presentedBytes = Encoding.UTF8.GetBytes(presentedKey);
        ApiKeyClient? match = null;
        foreach (var client in this._clients)
        {
            if (CryptographicOperations.FixedTimeEquals(presentedBytes, client.KeyBytes))
            {
                match = client;
            }
        }

        return match;
    }

    private IReadOnlyList<ApiKeyClient> CreateClients(ApiKeyOptions options)
    {
        var clients = new List<ApiKeyClient>();
        var knownKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in options.Keys)
        {
            var name = string.IsNullOrWhiteSpace(entry.Name) ? $"api-client-{clients.Count + 1}" : entry.Name.Trim();
            if (string.IsNullOrWhiteSpace(entry.Key))
            {
                this._logger.LogWarning("The API key of client {ClientName} is empty and is ignored.", name);
                continue;
            }

            if (entry.Key.Length < ApiKeyAuthenticationDefaults.MinimumKeyLength)
            {
                this._logger.LogWarning(
                    "The API key of client {ClientName} is shorter than the required {MinimumLength} characters and is ignored.",
                    name,
                    ApiKeyAuthenticationDefaults.MinimumKeyLength);
                continue;
            }

            if (!knownKeys.Add(entry.Key))
            {
                this._logger.LogWarning("The API key of client {ClientName} is used by another client as well and is ignored.", name);
                continue;
            }

            clients.Add(new ApiKeyClient(name, Encoding.UTF8.GetBytes(entry.Key), GetEffectiveRoles(entry.Roles)));
        }

        if (clients.Count == 0)
        {
            this._logger.LogInformation("No API key is configured; the public API is only reachable with a logged in admin panel user.");
        }

        return clients;
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
}

/// <summary>
/// An external application which is allowed to use the public API.
/// </summary>
/// <param name="Name">The configured name of the client.</param>
/// <param name="KeyBytes">The utf-8 bytes of its key.</param>
/// <param name="Roles">The effective roles of the client.</param>
public record ApiKeyClient(string Name, byte[] KeyBytes, IReadOnlyList<string> Roles)
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
