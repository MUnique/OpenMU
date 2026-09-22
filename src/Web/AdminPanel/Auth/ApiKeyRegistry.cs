// <copyright file="ApiKeyRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Resolves a presented API key to the client which uses it.
/// </summary>
/// <remarks>
/// The keys are managed in the admin panel and stored as hashes. There is no way to configure a
/// key outside of the panel: the public API is only needed by a few installations, so setting the
/// keys up in the panel once the server runs is good enough, and it keeps a credential out of the
/// configuration and the environment.
/// </remarks>
public class ApiKeyRegistry
{
    private readonly IApiKeyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyRegistry"/> class.
    /// </summary>
    /// <param name="repository">The repository of the keys which are managed in the admin panel.</param>
    public ApiKeyRegistry(IApiKeyRepository repository)
    {
        this._repository = repository;
    }

    /// <summary>
    /// Finds the client which presented the specified key.
    /// </summary>
    /// <param name="presentedKey">The key of the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The client; <c>null</c>, if no stored key matches.</returns>
    /// <remarks>
    /// The stored keys are looked up by the hash of the presented key, which is an indexed lookup
    /// and doesn't compare the key itself - so its duration doesn't tell an attacker how much of a
    /// guessed key was right.
    /// </remarks>
    public async ValueTask<ApiKeyClient?> FindAsync(string presentedKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(presentedKey))
        {
            return null;
        }

        var storedKey = await this._repository
            .GetEnabledByHashAsync(ApiKeyGenerator.Hash(presentedKey), cancellationToken)
            .ConfigureAwait(false);

        return storedKey is null
            ? null
            : new ApiKeyClient(storedKey.Name, GetEffectiveRoles(storedKey.Roles));
    }

    private static IReadOnlyList<string> GetEffectiveRoles(string? assignedRoles)
    {
        var roles = (assignedRoles ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (roles.Length == 0)
        {
            roles = [AdminRoles.Viewer];
        }

        return roles
            .SelectMany(AdminRoles.GetEffectiveRoles)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
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
